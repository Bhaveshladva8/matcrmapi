using System;

namespace matcrm.data.Models.Response
{
    public class DiscussionReadMarkAsReadResponse
    {
       public long? DiscussionId { get; set; }
        public int? ReadBy { get; set; }       
    }
}