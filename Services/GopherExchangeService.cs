using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GopherExchange.Data;
using GopherExchange.Models;
using GopherExchange.JoinClasses;
using System.Security.Cryptography;
using System.Security.Claims;

namespace GopherExchange.Services
{
    public class GEService
    {
        private readonly GeDbContext _context;
        private readonly ILogger _logger;

        private readonly userManager _usermanager;

        private readonly IHttpContextAccessor _accessor;
        public GEService(GeDbContext context, ILoggerFactory factory, IHttpContextAccessor accessor, userManager userManager)
        {
            _context = context;
            _logger = factory.CreateLogger<GEService>();
            _accessor = accessor;
            _usermanager = userManager;
        }

        public async Task<List<Tuple<Listing, String>>> GetListingsAsyncByDate()
        {
            List<Tuple<Listing, String>> listings = new List<Tuple<Listing, String>>();
            var fivedaysbefore = DateTime.UtcNow.AddDays(-5);
            try
            {
                var x = await _context.Listings
                        .Where(x => x.Date >= fivedaysbefore)
                        .Join(_context.Accounts,
                            t => t.Userid,
                            a => a.Userid,
                            (t, a) => new
                            {
                                t,
                                a.Goucheremail
                            }).ToListAsync();
                foreach (var p in x)
                {
                    listings.Add(Tuple.Create(p.t, p.Goucheremail));
                }

            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
            }
            return listings;
        }

        public async Task MakeAListing(BindingListingModel cmd)
        {
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Account acc = await _context.Accounts.FindAsync(claim);

            Listing listing = new Listing
            {
                Title = cmd.Title,
                Listingid = GenerateListingId(),
                Description = cmd.Description,
                Date = cmd.time,
                Typeid = DetermineListingType(cmd.ListingType),
                Userid = acc.Userid,
                User = acc
            };

            acc.Listings.Add(listing);

            await _context.AddAsync(listing);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Listing>> GetUserListings()
        {

            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Account acc = await _context.Accounts.FindAsync(claim);

            return await _context.Listings.Where(e => e.Userid == acc.Userid).ToListAsync();
        }

        public async Task DeleteListingById(int id)
        {


            Listing list = await _context.Listings.FindAsync(id);

            _context.Listings.Remove(list);

            await _context.SaveChangesAsync();
        }

        public async Task MakeAWishlistAsync(BindingWishlistModel cmd)
        {

            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Wishlist wishlist = new Wishlist
            {
                Wishlistid = GenerateListingId(),
                Userid = claim,
                Title = cmd.Title
            };

            _context.Add(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<WishlistContains>> GetWishlistsByUserAsync()
        {
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return await _context.Wishlists.Where(e => e.Userid == claim).Join(_context.Contains,
                    w => w.Wishlistid,
                    c => c.Wishlistid,
                    (w, c) => new
                    {
                        w,
                        c
                    }).Join(_context.Listings,
                        c => c.c.Listingid,
                        l => l.Listingid,
                        (c, l) => new WishlistContains
                        {
                            wishlist = c.w,
                            listing = l
                        }
                    ).ToListAsync();
        }

        public async Task<ICollection<Wishlist>> GetUserWishlistAsync()
        {
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return await _context.Wishlists.Where(e => e.Userid == claim).ToListAsync();
        }

        public async Task AddToWishList(AddToWishlistBindingModel cmd)
        {
            Contain contain = new Contain
            {
                Listingid = cmd.ListingId,
                Wishlistid = cmd.Wishlistid
            };

            _context.Add(contain);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWishListById(int id)
        {

            Wishlist wishlist = await _context.Wishlists.FindAsync(id);

            _context.Remove(wishlist);

            await _context.SaveChangesAsync();

        }

        public async Task DeleteFromWishlist(int wid, int lid)
        {

            Contain contain = await _context.Contains.FindAsync(wid, lid);

            _context.Remove(contain);

            await _context.SaveChangesAsync();
        }

        private int DetermineListingType(string s)
        {
            if (s.Equals("Exchange")) return 1;
            else if (s.Equals("Give away")) return 2;
            else return -1;
        }

        private int GenerateListingId()
        {

            byte[] accNumber = new byte[5];

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {

                rngCsp.GetBytes(accNumber);
            }

            int x = BitConverter.ToInt32(accNumber);
            if (x < 0) return -x;
            return x;
        }
    }
}