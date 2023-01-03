namespace matcrm.data.Models.Dto
{
    public class OrganizationActivityMemberDto
    {
        public long Id { get; set; }
        public long? OrganizationActivityId { get; set; }
        public string Email { get; set; }
    }
}