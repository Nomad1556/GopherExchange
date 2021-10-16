using System.ComponentModel.DataAnnotations;

namespace GopherExchange.Models
{
    public class AddToWishlistBindingModel{

        public int Wishlistid{get;set;}
        public int ListingId{get;set;}
    }
}