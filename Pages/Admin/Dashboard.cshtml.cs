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

        public List<Report> QueryReports { get; private set; }

        public Dictionary<String, String> QueriedAccount { get; private set; }

        public Listing listing { get; private set; }

        public List<Tuple<Listing, String>> listings { get; private set; }
        public DashboardModel(GEService service, userManager usermanager)
        {
            _service = service;
            _usermanager = usermanager;
        }
        public async Task OnGet()
        {
            reports = await _service.GetNoActionReports();
            QueriedAccount = new Dictionary<string, string>();
            QueryReports = new List<Report>();
            listings = new List<Tuple<Listing, string>>();
        }

        public async Task<IActionResult> OnPost()
        {
            reports = await _service.GetNoActionReports();
            if (ModelState.IsValid)
            {
                switch (Query.searchType)
                {
                    case "email":
                        QueriedAccount = await _usermanager.getUserInformationByEmail(Query.searchTerm);
                        if (QueriedAccount == null)
                        {
                            TempData["SearchError"] = "Sorry the search result did not return anything";
                            return Page();
                        }
                        else return Page();
                    case "id":
                        try
                        {
                            int id = int.Parse(Query.searchTerm);
                            var listing = await _service.GetListingById(id);
                            var areports = await _service.GetReportsByListingId(id);

                            QueryReports = areports;
                            this.listing = listing;
                            if (listing == null)
                            {
                                TempData["SearchError"] = "Sorry the search result did not return anything";
                                return Page();
                            }
                            else return Page();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            TempData["SearchError"] = "Sorry the search result did not return anything";
                            return Page();
                        }
                    case "title":
                        listings = await _service.FindListingsByTitle(Query.searchTerm);
                        if (listings == null || listings.Count == 0)
                        {
                            TempData["SearchError"] = "Sorry the search result did not return anything";
                            return Page();
                        }
                        else return Page();
                    default:
                        TempData["SearchError"] = "Sorry the search result did not return anything";
                        return Page();
                }
            }
            else
            {
                return Page();
            }
        }
    }

    public class SearchQuery
    {
        [Required(ErrorMessage = "Please enter a search term")]
        [DataType("String"), StringLength(30)]
        public string searchTerm { get; set; }

        [DataType("String")]
        public string searchType { get; set; }
    }
}
