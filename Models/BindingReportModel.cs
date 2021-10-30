using System;
using System.ComponentModel.DataAnnotations;

namespace GopherExchange.Models
{
    public class BindingReportModel
    {

        [Required]
        public int Listingid { get; set; }

        [Required(ErrorMessage = "Please provide a description"), StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string IncidentType { get; set; }

        public DateTime Incidentdate = DateTime.UtcNow;
    }
}