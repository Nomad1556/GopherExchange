using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class List
    {
        public int Userid { get; set; }
        public int Listingid { get; set; }

        public virtual Listing Listing { get; set; }
        public virtual Normaluser User { get; set; }
    }
}
