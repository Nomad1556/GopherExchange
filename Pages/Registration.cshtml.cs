using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GopherExchange.Models;
using System.ComponentModel.DataAnnotations;

namespace GopherExchange.Pages
{
    public class RegistrationModel : PageModel
    {
        private readonly GEServce _service;

        public RegistrationModel(GEServce service){
            _service = service;
        }

        [BindProperty]
        public GenerateAccountModel Input {get;set;}
        
        
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost(){

            try{
                if(ModelState.IsValid){
                    await _service.createAccountModel(Input);
                    return RedirectToPage("Index");
                }
            }catch( Exception){
                Console.WriteLine("Something happened");
            }
            return Page();
        }

    }
}
