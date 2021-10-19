using System.ComponentModel.DataAnnotations;

namespace GopherExchange.Models
{

    public class EditAccountModel
    {

        [Required(ErrorMessage = "Please enter a new username"), StringLength(10, ErrorMessage = "New username too large")]
        public string NewUserName { get; set; }
    }
}