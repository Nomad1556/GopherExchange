using System.ComponentModel.DataAnnotations;


namespace GopherExchange.Models
{
    public class GenerateAccountModel
    {
        [Required(ErrorMessage = "Email required"), RegularExpression(@"^[a-zA-Z0-9_.+-]+@(goucher|mail.goucher)\.edu$",
        ErrorMessage = "Please enter valid Goucher email.")]
        public string goucherEmail { get; set; }

        [Required(ErrorMessage = "Password required"), DataType(DataType.Password)]
        public string password { get; set; }

        [Required(ErrorMessage = "Please confirm password"), DataType(DataType.Password), Compare("password", ErrorMessage = "Passwords do not match.")]

        public string confirmPassword { get; set; }

        [Required]
        public int accountType { get; set; }


        [Range(typeof(bool), "true", "true", ErrorMessage = "Please agree to the Terms of Service and Privacy Policy")]
        public bool agreedToServiceAndPrivacy { get; set; }

    }
}