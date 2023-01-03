using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class DiscussionCommentPinUnpinRequest
    {
        public long? Id { get; set; }
        public bool IsPinned { get; set; }
    }
}
