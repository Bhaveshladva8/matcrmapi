namespace matcrm.data.Models.Request
{
    public class OrganizationRemoveActivityMemberRequest
    {
        public long Id { get; set; }
        public long? OrganizationActivityId { get; set; }        
    }
}