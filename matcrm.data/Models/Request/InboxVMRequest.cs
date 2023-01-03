using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class InboxVMRequest
    {
        public UserEmail UserEmail { get; set; }

        public int? Skip { get; set; }

        public int? Top { get; set; }

        public string NextPageToken { get; set; }

        public string Label { get; set; }

        public string Query { get; set; }

        public long UserEmailId { get; set; }

        public long? MailAssignUserId { get; set; }

        public List<int> Filterdata { get; set; }

        public List<NextPageToken> NextPageTokens { get; set; }
        public int? UserId { get; set; }
        public string Code { get; set; }
        public string EventType { get; set; }
        public long? TeamInboxId { get; set; }
    }
}
