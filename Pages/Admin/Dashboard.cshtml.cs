using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public List<Report> reports { get; private set; }
        public DashboardModel(GEService service)
        {
            _service = service;
        }
        public async Task OnGet()
        {
            reports = await _service.GetNoActionReports();
        }
    }
}
