namespace matcrm.data.Models.Request
{
    public class LeadActivityMemberRequest
    {
        public long Id { get; set; }
        public long? LeadActivityId { get; set; }
        public string Email { get; set; }
    }
}