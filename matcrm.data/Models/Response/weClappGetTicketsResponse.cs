namespace matcrm.data.Models.Response
{
    public class weClappGetTicketsResponse
    {
        public string Id { get; set; }
        public string AssignedUserId { get; set; }
        public string AssignedUserUsername { get; set; }
        public bool Billable { get; set; }
        public double CreatedDate { get; set; }
        public string CustomerId { get; set; }
        public bool DisableEmailTemplates { get; set; }
        public string Subject { get; set; }
        public string TicketCategoryId { get; set; }
        public string TicketNumber { get; set; }
        public string TicketPriorityId { get; set; }
        public string TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
    }
}