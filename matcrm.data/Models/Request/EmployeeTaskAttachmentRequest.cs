using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class EmployeeTaskAttachmentRequest
    {
        public long? EmployeeTaskId { get; set; }

        public IFormFile[] FileList { get; set; }
    }
}