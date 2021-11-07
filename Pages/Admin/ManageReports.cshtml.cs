using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
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

        private readonly ILogger<ManageReportsModel> _logger;

        public Report report { get; private set; }

        public Listing listing { get; private set; }

        [BindProperty]
        public BindingReportActionModel Input { get; set; }
        public ManageReportsModel(GEService service, ILogger<ManageReportsModel> logger)
        {
            _service = service;
            _logger = logger;
        }
        public async Task OnGet(int reportId, int listingId)
        {
            report = await _service.GetReportById(reportId);
            listing = await _service.GetListingById(listingId);
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("Dashboard");
        }
    }
}
