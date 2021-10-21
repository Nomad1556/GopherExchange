using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Account
    {
        public Account()
        {
            Listings = new HashSet<Listing>();
            Wishlists = new HashSet<Wishlist>();
        }

        public int Userid { get; set; }
        public string Username { get; set; }
        public string Goucheremail { get; set; }
        public string Hashedpassword { get; set; }
        public int? Accounttype { get; set; }

        public virtual ICollection<Listing> Listings { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
