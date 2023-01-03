using System;

namespace matcrm.data.Models.Dto
{
    public class JobDto
    {
        public string Customer { get; set; }
        public string Ticket { get; set; }
        public string TicketType { get; set; }
        public string TicketCategory { get; set; }
        public double StartAt { get; set; }
        public DateTime StartTime { get; set; }
        public double Duration { get; set; }
        public string Zone { get; set; }
        public string Description { get; set; }
        public string Signature { get; set; }
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public string Tenant { get; set; }
    }
}