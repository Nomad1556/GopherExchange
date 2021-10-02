using System.ComponentModel.DataAnnotations;

namespace GopherExchange.Models
{
    public class BindingLoginModel{

        [Required(ErrorMessage = "Email required"),RegularExpression(@"^[a-zA-Z0-9_.+-]+@(goucher|mail.goucher)\.edu$", 
        ErrorMessage = "Please enter valid Goucher email.")]
        public string goucherEmail{get;set;}
        [Required(ErrorMessage = "Password required"), DataType(DataType.Password)]
        public string password{get;set;}
    }
}