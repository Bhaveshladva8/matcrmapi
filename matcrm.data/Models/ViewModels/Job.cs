using System;
using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.ViewModels
{
    public class Job
    {
        [Required]
        public string Customer { get; set; }
        [Required]
        public string Ticket { get; set; }
        [Required]
        public string TicketType { get; set; }
        [Required]
        public string TicketCategory { get; set; }
        [Required]
        public double StartAt { get; set; }
        public DateTime StartTime { get; set; }

        [Required]
        public double Duration { get; set; }

        public string Zone { get; set; }
        [Required]
        public string Description { get; set; }
        public string Signature { get; set; }
    }
}