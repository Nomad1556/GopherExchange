using System;
using System.Collections.Generic;

#nullable disable

namespace GopherExchange.Data
{
    public partial class Account
    {
        public int Userid { get; set; }
        public string Username { get; set; }
        public string Goucheremail { get; set; }

        public virtual Administrator Administrator { get; set; }
        public virtual Normaluser Normaluser { get; set; }
    }
}
