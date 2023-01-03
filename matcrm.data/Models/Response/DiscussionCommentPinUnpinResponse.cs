using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class DiscussionCommentPinUnpinResponse
    {        
        public long? Id { get; set; }
        public bool IsPinned { get; set; }
        public int? PinnedBy { get; set; }
    }
}
