using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels {
    public class Ticket {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string TicketNumber { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double CreatedDate { get; set; }
        public bool Billable { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string TicketCategoryId { get; set; }
        public string TicketTypeName { get; set; }
        public string TicketTypeId { get; set; }
        public string Subject { get; set; }
        public string TicketPriorityId { get; set; }
        public bool DisableEmailTemplates { get; set; }
        public string AssignedUserId { get; set; }
        public string AssignedUserUsername { get; set; }
        public string Message { get; set; }
    }

    public class TicketResult
    {
        public List<Ticket> Result { get; set; }
    }
}