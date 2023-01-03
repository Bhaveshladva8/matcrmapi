using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class DiscussionAddUpdateRequest
    {
        public DiscussionAddUpdateRequest()
        {
            
            FileList = new IFormFile[] { };
        }
        public List<int>? ToTeamMateIds { get; set; }
        public IFormFile[] FileList { get; set; }
        public string? Topic { get; set; }
        public string? Note { get; set; }
    }
}
