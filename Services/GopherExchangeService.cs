using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
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

        readonly IHttpContextAccessor _accessor;
        public GEService(GeDbConext context, ILoggerFactory factory, IHttpContextAccessor accessor){
            _context = context;
            _logger = factory.CreateLogger<GEService>();
            _accessor = accessor;
        }
    }
}