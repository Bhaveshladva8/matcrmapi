using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class Office365CreateDraftResponse
    {
        public Office365CreateDraftResponse()
        {
            FileIds = new List<long>();
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            FileList = new IFormFile[] { };
        }
        public List<string> Bcc { get; set; }
        public string Body { get; set; }
        public List<string> Cc { get; set; }
        public string ErrorMessage { get; set; } = "";
        public List<long> FileIds { get; set; }
        public IFormFile[] FileList { get; set; }
        public string From { get; set; }
        public bool IsSendMassEmail { get; set; } = false;
        public bool IsValid { get; set; } = true;
        public GmailSent MailSent { get; set; }
        public GmailError GmailError { get; set; }

        public string Provider { get; set; }
        public string ProviderApp { get; set; }
        public string SelectedEmail { get; set; }
        public string Subject { get; set; }
        public List<string> To { get; set; }
        public int UserId { get; set; }
    }
}
