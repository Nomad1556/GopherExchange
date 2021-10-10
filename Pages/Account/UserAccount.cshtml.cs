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

        public Dictionary<String,String> _account{get;private set;}

        private readonly loginManager _login;

        private readonly ILogger _logger;

        [BindProperty]
        public EditAccountModel Input{get;set;}

        public UserAccountModel(userManager usermanager, ILoggerFactory factory,loginManager login){
            _usermanager = usermanager;
            _logger = factory.CreateLogger<UserAccountModel>();
            _login = login;
        }
        public async Task OnGet()
        {
            _account = await _usermanager.getSessionAccount();
        }

        public async Task <IActionResult> OnPost(){
            try{
                if(ModelState.IsValid){
                    await _usermanager.editAccount(Input);
                    return RedirectToPage("UserAccount");
                }
                else return Page();
            }catch(Exception e){
                _logger.LogError(e.ToString());
                return Page();
            }
        }
        public async Task <IActionResult> OnPostLogout(){
            await _login.signOut();
            return RedirectToPage("../Index");
        }
    }
}
