using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ShareTomeResponse
    {
        public int count { get; set; }        
        public List<InboxThreadAndDiscussion> MailAndDiscussions { get; set; }
        public bool IsValid { get; set; }
        
    }
}
