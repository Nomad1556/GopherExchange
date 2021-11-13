using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using GopherExchange.Services;
using GopherExchange.Models;
using GopherExchange.Data;

namespace GopherExchange.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        private readonly GEService _service;

        private readonly userManager _usermanager;

        [BindProperty]
        public SearchQuery Query { get; set; }
        public List<Report> reports { get; private set; }

        public Dictionary<String, String> QueriedAccount { get; private set; }

        public Listing listing { get; private set; }
        public DashboardModel(GEService service, userManager usermanager)
        {
            _service = service;
            _usermanager = usermanager;
        }
        public async Task OnGet()
        {
            reports = await _service.GetNoActionReports();
        }

        public IActionResult OnPost()
        {
            Console.WriteLine(Query.searchTerm);
            Console.WriteLine(Query.searchType);
            return Page();
        }
    }

    public class SearchQuery
    {
        [DataType("String"), StringLength(20)]
        public string searchTerm { get; set; }

        [DataType("String")]
        public string searchType { get; set; }
    }
}
