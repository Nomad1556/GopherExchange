using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using GopherExchange.Services;
using GopherExchange.Data;
using GopherExchange.Models;

namespace GopherExchange.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManageReportsModel : PageModel
    {
        private readonly GEService _service;

        public Report report { get; private set; }

        public Listing listing { get; private set; }

        [BindProperty]
        public BindingReportModel Input { get; set; }
        public ManageReportsModel(GEService service)
        {
            _service = service;
        }
        public async Task OnGet(int reportId, int listingId)
        {
            report = await _service.GetReportById(reportId);
            listing = await _service.GetListingById(listingId);
        }
    }
}
