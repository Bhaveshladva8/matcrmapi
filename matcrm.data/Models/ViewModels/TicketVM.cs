namespace matcrm.data.Models.ViewModels {
    public class TicketVM {
        public string id { get; set; }
        public string version { get; set; }
        public string assignedUserId { get; set; }
        public string assignedUserUsername { get; set; }
        public bool billable { get; set; }
        public long createdDate { get; set; }
        public string customerId { get; set; }
        public string customerNumber { get; set; }
        public bool disableEmailTemplates { get; set; }
        public long lastModifiedDate { get; set; }
        public string partyId { get; set; }
        public string status { get; set; }
        public string subject { get; set; }
        public string ticketNumber { get; set; }
        public string ticketTypeId { get; set; }
        public string ticketCategoryId { get; set; }
        public string ticketTypeName { get; set; }
        public string ticketPriorityId { get; set; }
    }
}