using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class File
    {
        public int Userid { get; set; }
        public int Reportid { get; set; }

        public virtual Report Report { get; set; }
        public virtual Normaluser User { get; set; }
    }
}
