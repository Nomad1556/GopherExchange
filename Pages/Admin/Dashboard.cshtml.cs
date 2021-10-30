using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace GopherExchange.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
