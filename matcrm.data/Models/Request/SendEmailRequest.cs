using matcrm.data.Models.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class SendEmailRequest
    {
        public SendEmailRequest()
        {           
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            FileList = new IFormFile[] { };
        }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }
        public IFormFile[] FileList { get; set; }
        public List<long> FileIds { get; set; }
        public List<QuickEmailAttachments> QuickEmailAttachments { get; set; }
        public bool IsSendMassEmail { get; set; } = false;
    }
}
