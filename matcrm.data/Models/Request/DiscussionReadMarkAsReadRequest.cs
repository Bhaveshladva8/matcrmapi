namespace matcrm.data.Models.Request
{
    public class DiscussionReadMarkAsReadRequest
    {
        public long? Id { get; set; }
        public long? DiscussionId { get; set; }
        public int? ReadBy { get; set; }       
        
    }
}