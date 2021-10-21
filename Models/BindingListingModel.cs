using System.ComponentModel.DataAnnotations;
using System;

namespace GopherExchange.Models
{
    public class BindingListingModel
    {

        [Required]
        public string Title { get; set; }

        [Required]
        public string ListingType { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime time = DateTime.UtcNow;
    }
}