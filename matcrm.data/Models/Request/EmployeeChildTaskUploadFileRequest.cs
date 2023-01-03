using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class EmployeeChildTaskUploadFileRequest
    {
        public long? EmployeeChildTaskId { get; set; }
        public IFormFile[] FileList { get; set; }
    }
}