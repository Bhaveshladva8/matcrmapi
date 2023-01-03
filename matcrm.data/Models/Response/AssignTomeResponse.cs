using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class AssignTomeResponse
    {
        public List<InboxThreadAndDiscussion> MailAndDiscussions { get; set; }
        public int count { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

    }
}
