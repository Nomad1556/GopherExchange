using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Services;

namespace GopherExchange.Pages.Delete
{  
    public class DeleteListingModel : PageModel
    {
        private readonly GEService _service;

        public DeleteListingModel(GEService service){
            _service = service;
        }
        public async Task<IActionResult> OnGet(int id)
        {
            
            if(id != 0){

                await _service.DeleteListingById(id);

                return RedirectToPage("../Account/UserAccount");
            }
            return RedirectToPage("../Account/UserAccount");
        }
    }
}
