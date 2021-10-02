using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using GopherExchange.Services;

namespace GopherExchange.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GEService _service;

        private readonly loginManager _login;

        public IndexModel(ILogger<IndexModel> logger, GEService service, loginManager login)
        {
            _logger = logger;
            _service = service;
            _login = login;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost(){
            await _login.signOut(this.HttpContext);
            return Page();
        }
    }
}
