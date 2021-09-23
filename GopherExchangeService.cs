using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GopherExchange.Data;

namespace GopherExchange{
    public class GEServce{
        readonly gopherexchangeContext _context;
        readonly ILogger _logger;

        public GEServce(gopherexchangeContext context, ILoggerFactory factory){
            _context = context;
            _logger = factory.CreateLogger<GEServce>();

        }
    }
}