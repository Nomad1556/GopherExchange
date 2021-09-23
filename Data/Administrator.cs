using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Administrator
    {
        public int Userid { get; set; }
        public int Adminid { get; set; }

        public virtual Account User { get; set; }
    }
}
