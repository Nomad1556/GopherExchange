using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GopherExchange.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GEServce _service;

        public IndexModel(ILogger<IndexModel> logger, GEServce service)
        {
            _logger = logger;
            _service = service;
        }

        public void OnGet()
        {

        }
    }
}
