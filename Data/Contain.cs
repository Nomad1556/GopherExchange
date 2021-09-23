using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Contain
    {
        public int Wishlistid { get; set; }
        public int Listingid { get; set; }

        public virtual Listing Listing { get; set; }
        public virtual Wishlist Wishlist { get; set; }
    }
}
