using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Listing
    {
        public Listing()
        {
            Reports = new HashSet<Report>();
        }

        public int Listingid { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public int? Typeid { get; set; }

        public virtual ICollection<Report> Reports { get; set; }
    }
}
