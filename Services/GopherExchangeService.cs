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

        public enum Response
        {
            Success,
            Failure
        }
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

        public async Task<Response> MakeAListing(BindingListingModel cmd)
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

            return Response.Success;
        }

        public async Task<ICollection<Listing>> GetUserListings()
        {

            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Account acc = await _context.Accounts.FindAsync(claim);

            return await _context.Listings.Where(e => e.Userid == acc.Userid).ToListAsync();
        }

        public async Task<Response> DeleteListingById(int id)
        {


            Listing list = await _context.Listings.FindAsync(id);

            _context.Listings.Remove(list);

            await _context.SaveChangesAsync();

            return Response.Success;
        }

        public async Task<Listing> GetListingById(int id)
        {

            return await _context.Listings.FirstOrDefaultAsync(e => e.Listingid == id);
        }

        public async Task<Response> MakeAWishlistAsync(BindingWishlistModel cmd)
        {

            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var acc = await _context.Accounts.FirstOrDefaultAsync(e => e.Userid == claim);

            Wishlist wishlist = new Wishlist
            {
                Wishlistid = GenerateListingId(),
                Userid = claim,
                Title = cmd.Title
            };
            acc.Wishlists.Add(wishlist);

            _context.Add(wishlist);
            await _context.SaveChangesAsync();

            return Response.Success;
        }

        public async Task<Response> MakeAReport(BindingReportModel cmd)
        {
            Report report = new Report
            {
                Reportid = GenerateReportId(),
                Listingid = cmd.Listingid,
                Adminid = 0,
                Description = cmd.Description,
                Incidentdate = cmd.Incidentdate,
                IncidentType = cmd.IncidentType,
                Action = "None",
                Actiondate = null,
            };

            await _context.Reports.AddAsync(report);
            await _context.SaveChangesAsync();

            return Response.Success;
        }

        public async Task<List<Report>> GetReportsByListingId(int id)
        {

            var reports = await _context.Reports.Where(e => e.Listingid == id).ToListAsync();

            return reports;
        }

        public async Task<List<Report>> GetNoActionReports()
        {
            var reports = await _context.Reports.Where(e => e.Action == "None").ToListAsync();

            return reports;
        }

        public async Task<Report> GetReportById(int id)
        {
            return await _context.Reports.FirstOrDefaultAsync(e => e.Reportid == id);
        }
        public async Task<List<Tuple<Listing, String>>> GetListingsInWishlistById(int id)
        {
            var contains = await _context.Contains.Where(e => e.Wishlistid == id).ToListAsync();

            List<Tuple<Listing, String>> list = new List<Tuple<Listing, String>>();

            foreach (Contain c in contains)
            {
                Listing l = await _context.Listings.FirstOrDefaultAsync(e => e.Listingid == c.Listingid);
                if (l == null) continue;
                String Goucheremail = await _context.Accounts.Where(e => e.Userid == l.Userid).Select(e => e.Goucheremail).SingleOrDefaultAsync();
                list.Add(Tuple.Create(l, Goucheremail));
            }

            return list;
        }

        public async Task<ICollection<Wishlist>> GetUserWishlistAsync()
        {
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return await _context.Wishlists.Where(e => e.Userid == claim).ToListAsync();
        }

        public async Task<Wishlist> GetWishlistById(int id)
        {
            return await _context.Wishlists.FirstOrDefaultAsync(e => e.Wishlistid == id);
        }

        public async Task<Response> AddToWishList(AddToWishlistBindingModel cmd)
        {
            Contain contain = new Contain
            {
                Listingid = cmd.ListingId,
                Wishlistid = cmd.Wishlistid
            };

            _context.Add(contain);
            await _context.SaveChangesAsync();

            return Response.Success;
        }

        public async Task<Response> DeleteWishListById(int id)
        {

            Wishlist wishlist = await _context.Wishlists.FindAsync(id);

            _context.Remove(wishlist);

            await _context.SaveChangesAsync();

            return Response.Success;

        }

        public async Task<Response> DeleteFromWishlist(int wid, int lid)
        {

            Contain contain = await _context.Contains.FindAsync(lid, wid);

            _context.Remove(contain);

            await _context.SaveChangesAsync();

            return Response.Success;
        }

        public async Task<List<Tuple<Listing, String>>> FindListingByTitle(string title)
        {
            List<Tuple<Listing, String>> listings = new List<Tuple<Listing, String>>();

            var p = await _context.Listings.Where(e => e.Title.Contains(title)).ToListAsync();

            foreach (Listing l in p)
            {
                String Goucheremail = await _context.Accounts.Where(e => e.Userid == l.Userid).Select(e => e.Goucheremail).SingleOrDefaultAsync();
                listings.Add(Tuple.Create(l, Goucheremail));
            }
            return listings;
        }

        private int DetermineListingType(string s)
        {
            if (s.Equals("Exchange")) return 1;
            else if (s.Equals("Give away")) return 2;
            else return -1;
        }

        private int GenerateReportId()
        {
            byte[] reportId = new byte[8];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {

                rngCsp.GetNonZeroBytes(reportId);
            }

            int x = BitConverter.ToInt32(reportId);
            if (x < 0) return -x;
            return x;
        }

        private int GenerateListingId()
        {

            byte[] listingId = new byte[5];

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {

                rngCsp.GetNonZeroBytes(listingId);
            }

            int x = BitConverter.ToInt32(listingId);
            if (x < 0) return -x;
            return x;
        }
    }
}