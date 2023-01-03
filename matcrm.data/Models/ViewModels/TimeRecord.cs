using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels {
    public class TimeRecord {
        public string id { get; set; }
        public string ticketNumber { get; set; }
        public double durationSeconds { get; set; }
        public double startDate { get; set; }
        public string description { get; set; }
        public double createdDate { get; set; }
        public bool billable { get; set; }
        public string userUsername { get; set; }
        public string ticketId { get; set; }
        public string projectId { get; set; }
        public string projectTaskId { get; set; }
        public string customerId { get; set; }
        public string userId { get; set; }
        public string ApiKey { get; set; }
        public string Tenant { get; set; }

        //public string TicketCategory { get; set; }
        //public string TicketType { get; set; }
    }

    public class TimeRecordResult {
        public List<TimeRecord> Result { get; set; }
    }
}