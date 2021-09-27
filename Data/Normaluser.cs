using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Normaluser
    {
        public Normaluser()
        {
            Wishlists = new HashSet<Wishlist>();
        }

        public int Userid { get; set; }

        public virtual ICollection<Wishlist> Wishlists { get; set; }
    }
}
