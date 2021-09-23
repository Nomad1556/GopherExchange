using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Wishlist
    {
        public int Wishlistid { get; set; }
        public int Userid { get; set; }
        public string Name { get; set; }

        public virtual Normaluser User { get; set; }
    }
}
