using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Add
    {
        public int Wishlistid { get; set; }
        public int Userid { get; set; }

        public virtual Normaluser User { get; set; }
        public virtual Wishlist Wishlist { get; set; }
    }
}
