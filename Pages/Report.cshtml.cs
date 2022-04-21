using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Data;
using GopherExchange.Models;
using GopherExchange.Services;

namespace GopherExchange.Pages
{
    public class ReportModel : PageModel
    {
        private readonly GEService _service;

        [BindProperty]
        public BindingReportModel Input { get; set; }

        public int? Listingid { get; set; }


        public ReportModel(GEService service)
        {
            _service = service;
        }
        public void OnGet(int? id, string sub)
        {
            Listingid = id;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var response = await _service.MakeAReport(Input);
                if (response == GEService.Response.Success)
                {
                    TempData["Success"] = "Report submitted successfuly! Thank you for keeping Gopher Exchange safe.";
                    return RedirectToPage("Report");
                }
                else return Page();
            }
            else return Page();
        }
    }
}
