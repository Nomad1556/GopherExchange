using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Services;
using GopherExchange.Data;

namespace GopherExchange.Pages
{
    public class SearchModel : PageModel
    {

        private readonly GEService _service;

        public List<Tuple<Listing, String>> listings { get; private set; }
        public String searchTerm { get; private set; }
        public SearchModel(GEService service)
        {
            _service = service;
        }

        public async Task OnGet(string term)
        {
            searchTerm = term;
            listings = await _service.FindListingByTitle(term);
        }
    }
}
