using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using GopherExchange.Models;
using System.ComponentModel.DataAnnotations;
using GopherExchange.Services;


namespace GopherExchange.Pages.Account
{
    public class RegistrationModel : PageModel
    {
        private readonly GEService _service;
        private readonly userManager _usermanager;

        private readonly loginManager _login;

        public RegistrationModel(GEService service, userManager usermanager, loginManager login){
            _service = service; 
            _usermanager = usermanager;
            _login = login;
        }

        [BindProperty]
        public GenerateAccountModel Input {get;set;}
        
        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(){
            try{
                if(ModelState.IsValid){
                    
                   var result = await _usermanager.createAccountAsync(Input);
                   if (result == userManager.ResponseType.Success){
                       
                       var account = _usermanager.validateUser(new BindingLoginModel{
                           goucherEmail = Input.goucherEmail,
                           password = Input.password
                       });
                       await _login.signIn(this.HttpContext, account,isPersistent: false);
                       
                       return RedirectToPage("/Index");
                   }
                   else{
                       return Page();
                   }
                   
                }
            }catch(Exception){
                Console.WriteLine("Uh oh doodoo");
            }
            return Page();
        }

    }
}
