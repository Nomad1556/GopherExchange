using System.ComponentModel.DataAnnotations;
using System;

namespace GopherExchange.Models
{
    public class BindingReportActionModel
    {
        [Required(ErrorMessage = "Please indicate how you handeled this report")]
        public string ActionDescription { get; set; }

        [Required]
        public string Action { get; set; }

        public string ChangeTitle { get; set; }

        public string ChangeDesc { get; set; }

        public int listingId { get; set; }

        public int reportId { get; set; }
        public DateTime? Actiondate = DateTime.UtcNow;
    }
}