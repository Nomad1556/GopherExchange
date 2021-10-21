using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Services;
using GopherExchange.Data;
using GopherExchange.Models;
using Microsoft.Extensions.Logging;

namespace GopherExchange.Pages.Account
{
    public class UserAccountModel : PageModel
    {
        private readonly userManager _usermanager;

        private readonly loginManager _login;

        private readonly ILogger _logger;

        private readonly GEService _service;

        public Dictionary<String, String> _account { get; private set; }

        public ICollection<Listing> _userlisting { get; private set; }

        public ICollection<Wishlist> _userwishlist { get; private set; }

        [BindProperty]
        public EditAccountModel Input { get; set; }

        public UserAccountModel(userManager usermanager, ILoggerFactory factory, loginManager login, GEService service)
        {
            _usermanager = usermanager;
            _logger = factory.CreateLogger<UserAccountModel>();
            _login = login;
            _service = service;
        }
        public async Task OnGet()
        {
            _account = await _usermanager.getSessionAccount();
            _userlisting = await _service.GetUserListings();
            _userwishlist = await _service.GetUserWishlistAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _usermanager.editAccount(Input);
                    return RedirectToPage("UserAccount");
                }
                else return Page();
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                return Page();
            }
        }
        public async Task<IActionResult> OnPostLogout()
        {
            await _login.signOut();
            return RedirectToPage("../Index");
        }
    }
}
