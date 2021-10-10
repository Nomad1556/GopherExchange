using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using GopherExchange.Services;
using GopherExchange.Data;
using GopherExchange.Models;

namespace GopherExchange.Pages
{

    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly GEService _service;

        public ICollection<Listing> _listings{get;private set;}

        [BindProperty]
        public BindingListingModel Input{get;set;}

        public IndexModel(ILogger<IndexModel> logger, GEService service)
        {
            _logger = logger;
            _service = service;
        }

        public async Task OnGet()
        {
            _listings = await _service.GetListingsAsyncByDate();
        }

        public async Task<IActionResult> OnPost(){
            if(ModelState.IsValid){
                await _service.MakeAListing(Input);
                return RedirectToPage("Index");
            }
            else return Page();
        }
    }
}
