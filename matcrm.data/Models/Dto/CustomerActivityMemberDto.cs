namespace matcrm.data.Models.Dto
{
    public class CustomerActivityMemberDto
    {
        public long Id { get; set; }
        public long? CustomerActivityId { get; set; }
        public string Email { get; set; }
    }
}