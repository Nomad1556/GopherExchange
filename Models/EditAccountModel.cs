using System.ComponentModel.DataAnnotations;

namespace GopherExchange.Models{
    
    public class EditAccountModel{

        [Required(ErrorMessage = "Please enter a new username")]
        public string NewUserName {get;set;}
    }
}