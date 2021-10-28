using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Flag
    {
        public int Listingid { get; set; }
        public int Reportid { get; set; }

        public virtual Report Report { get; set; }
        public virtual Listing Listing { get; set; }
    }
}
