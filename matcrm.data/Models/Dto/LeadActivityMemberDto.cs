namespace matcrm.data.Models.Dto
{
    public class LeadActivityMemberDto
    {
         public long Id { get; set; }
        public long? LeadActivityId { get; set; }
        public string Email { get; set; }
    }
}