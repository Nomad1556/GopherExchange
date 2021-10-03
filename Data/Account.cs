using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Account
    {
        public Account()
        {
            Wishlists = new HashSet<Wishlist>();
        }

        public int Userid { get; set; }
        public string Username { get; set; }
        public string Goucheremail { get; set; }
        public int? Accounttype { get; set; }

        public string HashedPassword {get;set;}

        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
