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

namespace MyApp.Namespace
{
    public class UserAccountModel : PageModel
    {
        private readonly userManager _usermanager;

        public Dictionary<String,String> _account;

        private readonly ILogger _logger;

        [BindProperty]
        public EditAccountModel Input{get;set;}

        public UserAccountModel(userManager usermanager, ILoggerFactory factory){
            _usermanager = usermanager;
            _logger = factory.CreateLogger<UserAccountModel>();
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
    }
}
