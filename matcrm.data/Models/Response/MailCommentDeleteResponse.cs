using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MailCommentDeleteResponse
    {
        public long? Id { get; set; }
        public bool IsPinned { get; set; }
    }
}
