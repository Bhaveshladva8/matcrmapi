using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MailCommentPinUnpinResponse
    {              
        
       
        public long? Id { get; set; }
        public bool IsPinned { get; set; }
        public int? PinnedBy { get; set; }        
        public string? ThreadId { get; set; }
    }
}
