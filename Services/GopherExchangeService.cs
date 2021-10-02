using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using GopherExchange.Data;
using GopherExchange.Models;

namespace GopherExchange.Services{
    public class GEService{
        readonly GeDbConext _context;
        readonly ILogger _logger;

        public GEService(GeDbConext context, ILoggerFactory factory){
            _context = context;
            _logger = factory.CreateLogger<GEService>();
        }
    }
}