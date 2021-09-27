using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GopherExchange.Data;
using GopherExchange.Models;

namespace GopherExchange{
    public class GEServce{
        readonly gopherexchangeContext _context;
        readonly ILogger _logger;

        public GEServce(gopherexchangeContext context, ILoggerFactory factory){
            _context = context;
            _logger = factory.CreateLogger<GEServce>();

        }

        public async Task createAccountModel(GenerateAccountModel command){

            Account acc = command.GenerateAccount();
            _context.Add(acc);
            await _context.SaveChangesAsync();
            Console.WriteLine("Account made successfully!");
            
        }
    }
}