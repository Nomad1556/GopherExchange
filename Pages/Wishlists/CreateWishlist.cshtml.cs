using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Models;
using GopherExchange.Services;

namespace GopherExchange.Pages.Wishlists
{

    public class CreateWishlistModel : PageModel
    {
        private readonly GEService _service;

        [BindProperty]
        public BindingWishlistModel Input { get; set; }

        public CreateWishlistModel(GEService service)
        {
            _service = service;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                await _service.MakeAWishlistAsync(Input);
                return RedirectToPage("../Account/UserAccount");
            }
            return Page();
        }
    }
}
