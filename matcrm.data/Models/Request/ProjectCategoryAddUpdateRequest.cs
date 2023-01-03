using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class ProjectCategoryAddUpdateRequest
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }
}