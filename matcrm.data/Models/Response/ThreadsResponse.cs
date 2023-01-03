using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ThreadsResponse
    {
        public int count { get; set; }
        public List<InboxThread> InboxThread { get; set; }       
        public string NextPageToken { get; set; }        
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
