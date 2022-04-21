using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using GopherExchange.Models;
using GopherExchange.Services;


namespace GopherExchange.Pages.Account
{
    [AllowAnonymous]
    public class RegistrationModel : PageModel
    {
        private readonly GEService _service;
        private readonly userManager _usermanager;

        private readonly loginManager _login;

        private readonly ILogger _logger;

        public RegistrationModel(GEService service, userManager usermanager, loginManager login, ILoggerFactory factory)
        {
            _service = service;
            _usermanager = usermanager;
            _login = login;
            _logger = factory.CreateLogger<RegistrationModel>();
        }

        [BindProperty]
        public GenerateAccountModel Input { get; set; }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool duplicate = await _usermanager.checkForDuplicate(Input.goucherEmail);

                    if (duplicate)
                    {
                        TempData["Duplicate"] = "An account with that email already exists";
                        return Page();
                    }

                    var result = await _usermanager.createAccountAsync(Input);
                    if (result == userManager.Response.Success)
                    {

                        var account = _usermanager.validateUser(new BindingLoginModel
                        {
                            goucherEmail = Input.goucherEmail,
                            password = Input.password
                        });
                        await _login.signIn(account, isPersistent: false);

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        return Page();
                    }

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return Page();
        }

    }
}
