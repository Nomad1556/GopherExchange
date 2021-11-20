using System.ComponentModel.DataAnnotations;
using System;

namespace GopherExchange.Models
{
    public class BindingListingModel
    {

        [Required, StringLength(50)]
        public string Title { get; set; }

        [Required]
        public string ListingType { get; set; }

        [Required, StringLength(150)]
        public string Description { get; set; }

        public DateTime time = DateTime.UtcNow;
    }
}