using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Services;

namespace GopherExchange.Pages.Delete
{
    public class DeleteFromWishlistModel : PageModel
    {

        private readonly GEService _service;

        public DeleteFromWishlistModel(GEService service)
        {
            _service = service;
        }
        public async Task<IActionResult> OnGet(int wid, int lid)
        {
            await _service.DeleteFromWishlist(wid, lid);
            return RedirectToPage("../Wishlists/ManageWishlist", new { id = wid.ToString() });
        }
    }
}
