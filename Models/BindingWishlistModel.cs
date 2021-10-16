using System.ComponentModel.DataAnnotations;

namespace GopherExchange.Models
{
    public class BindingWishlistModel{
        
        [Required]
        public string Title{get;set;}
    }
}