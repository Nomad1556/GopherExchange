using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Services;
using GopherExchange.Data;

namespace GopherExchange.Pages.Wishlists
{
    public class ManageWishlistModel : PageModel
    {

        private readonly GEService _service;

        public List<Tuple<Listing, String>> _listings { get; private set; }

        public Wishlist _wishlistToManage { get; private set; }

        public ManageWishlistModel(GEService service)
        {

            _service = service;
        }

        public async Task OnGet(int id)
        {
            _listings = await _service.GetListingsInWishlistById(id);
            _wishlistToManage = await _service.GetWishlistById(id);
        }
    }
}
