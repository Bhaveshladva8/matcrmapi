using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MultipleThreadMarkAsReadUnReadResponse
    {
        public int count { get; set; }
        public List<InboxThread> InboxThread { get; set; }
        public List<InboxThreadAndDiscussion> MailAndDiscussions { get; set; }
        public string NextPageToken { get; set; }
        public List<InboxLabel> Labels { get; set; }
        public List<NextPageToken> NextPageTokens { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
