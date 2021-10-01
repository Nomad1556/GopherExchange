using System.ComponentModel.DataAnnotations;
using GopherExchange.Data;


namespace GopherExchange.Models{
    public class GenerateAccountModel{
        [Required,EmailAddress]
        public string goucherEmail {get;set;}

        [Required,StringLength(20)]
        public string password{get;set;}

        //Dynamically use their gouhcerEmail handle to be their userName
        [Required, StringLength(20)]
        public string userName{get;set;}

        [Required]
        public int accountType{get;set;}


        public Account GenerateAccount(){
            return new Account{
                Userid = 1,
                Goucheremail = goucherEmail,
                Username = goucherEmail.Split("@")[0],
                Accounttype = accountType,
                HashedPassword = password
                
            };
        }
    }
}