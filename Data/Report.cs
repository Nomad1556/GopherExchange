using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Report
    {
        public int Reportid { get; set; }
        public int Listingid { get; set; }
        public int Adminid { get; set; }
        public string Description { get; set; }
        public DateTime Incidentdate { get; set; }
        public int Incidentid { get; set; }
        public string Action { get; set; }
        public DateTime? Actiondate { get; set; }

        public virtual Listing Listing { get; set; }
    }
}
