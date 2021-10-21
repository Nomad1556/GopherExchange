using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Models;
using GopherExchange.Services;
using GopherExchange.Data;

namespace GopherExchange.Pages.Wishlists
{
    public class AddToWishlistModel : PageModel
    {

        private readonly GEService _service;

        public ICollection<Wishlist> _userwishlist { get; private set; }

        [BindProperty]
        public AddToWishlistBindingModel Input { get; set; }

        public Listing listingToAdd { get; private set; }

        public AddToWishlistModel(GEService service)
        {
            _service = service;
        }
        public async Task OnGet(int listingid)
        {

            listingToAdd = await _service.GetListingById(listingid);

            _userwishlist = await _service.GetUserWishlistAsync();

            if (_userwishlist == null || _userwishlist.Count == 0) RedirectToPage("../Index");

        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                await _service.AddToWishList(Input);
                return RedirectToPage("../Index");
            }
            return Page();
        }
    }
}
