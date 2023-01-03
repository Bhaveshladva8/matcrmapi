namespace matcrm.data.Models.ViewModels {
    
    public class PostTimeRecord {
        public string ticketNumber { get; set; }
        public double durationSeconds { get; set; }
        public double startDate { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        // public string userId { get; set; }
        // public string userUsername { get; set; }

        //public string Customer { get; set; }
        //public string TicketCategory { get; set; }
        //public string TicketType { get; set; }
    }

    public class PostTimeRecordResponse{
        public string ticketNumber { get; set; }
        public double durationSeconds { get; set; }
        public double startDate { get; set; }
        public string description { get; set; }
        public string id { get; set; }
        public string userId { get; set; }
        public string userUsername { get; set; }
        public string Customer { get; set; }
        public string TicketCategory { get; set; }
        public string TicketType { get; set; }
    }
}