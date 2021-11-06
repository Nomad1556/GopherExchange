using System.ComponentModel.DataAnnotations;
using System;

namespace GophereExchange.Models
{
    public class BindingReportActionModel
    {
        [Required(ErrorMessage = "Please indicate how you handeled this report")]
        public string ActionDescription { get; set; }

        [Required]
        public string Action { get; set; }

        public int listingId { get; set; }

        public int reportId { get; set; }
        public DateTime? Actiondate = DateTime.UtcNow;
    }
}