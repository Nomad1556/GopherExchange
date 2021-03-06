using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using GopherExchange.Models;
using GopherExchange.Services;

namespace GopherExchange.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly loginManager _login;
        private readonly userManager _usermanager;

        private readonly ILogger _logger;

        [BindProperty]
        public BindingLoginModel Input { get; set; }

        public LoginModel(loginManager login, userManager usermanager, ILoggerFactory factory)
        {
            _login = login;
            _usermanager = usermanager;
            _logger = factory.CreateLogger<LoginModel>();
        }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = _usermanager.validateUser(Input);
                    if (account == null)
                    {
                        TempData["Failure"] = "Email or password is incorrect";
                        return Page();
                    }
                    await _login.signIn(account);
                    return RedirectToPage("../Index");
                }
                else return Page();
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.ToString());
                return Page();
            }

        }
    }
}
