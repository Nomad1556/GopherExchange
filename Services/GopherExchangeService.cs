using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GopherExchange.Data;
using System.Security.Cryptography;
using GopherExchange.Models;
using System.Security.Claims;

namespace GopherExchange.Services{
    public class GEService{
        readonly GeDbContext _context;
        readonly ILogger _logger;

        readonly userManager _usermanager;

        readonly IHttpContextAccessor _accessor;
        public GEService(GeDbContext context,ILoggerFactory factory, IHttpContextAccessor accessor, userManager userManager){
            _context = context;
            _logger = factory.CreateLogger<GEService>();
            _accessor = accessor;
            _usermanager = userManager;
        }

        public async Task<ICollection<Listing>> GetListingsAsyncByDate(){
            ICollection<Listing> listings = null;
            var fivedaysbefore = DateTime.UtcNow.AddDays(-5);
            try{
                var x = await _context.Listings.Where(x => x.Date >= fivedaysbefore).ToListAsync();
                listings = x;
            }catch(Exception e){
                _logger.LogInformation(e.ToString());
            }
            return listings;
        }

        public async Task MakeAListing(BindingListingModel cmd){
            int claim = int.Parse(_accessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Account acc = await _context.Accounts.FindAsync(claim);

            Listing listing = new Listing{
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

        private int DetermineListingType(string s){
            if(s.Equals("Exchange")) return 1;
            else if(s.Equals("Give away")) return 2;
            else return -1;
        }

        private int GenerateListingId(){

             byte[] accNumber = new byte[5];

            using(RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider()){

                rngCsp.GetBytes(accNumber);
            }

            int x = BitConverter.ToInt32(accNumber);
            if (x < 0) return -x;
            return x;
        }
    }
}