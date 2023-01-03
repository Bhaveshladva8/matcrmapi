using System;

namespace matcrm.data.Models.Response
{
    public class DiscussionReadMarkAsUnReadResponse
    {
        public long? Id { get; set; }
        public long? DiscussionId { get; set; }
        public int? ReadBy { get; set; }
    }
}