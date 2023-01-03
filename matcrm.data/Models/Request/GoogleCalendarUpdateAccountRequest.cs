namespace matcrm.data.Models.Request
{
    public class GoogleCalendarUpdateAccountRequest
    {
        public long? Id { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelect { get; set; }
    }
}