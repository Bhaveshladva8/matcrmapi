namespace matcrm.data.Models.Request
{
    public class DiscussionReadMarkAsUnReadRequest
    {
        public long? Id { get; set; }
        public long? DiscussionId { get; set; }
        public int? ReadBy { get; set; }
    }
}