using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class SendEmailResponse
    {
        public string threadId { get; set; }        
        public string ErrorMessage { get; set; } = "";        
        public bool IsValid { get; set; } = true;
        public GmailSent MailSent { get; set; }
        public GmailError GmailError { get; set; }        
    }
}
