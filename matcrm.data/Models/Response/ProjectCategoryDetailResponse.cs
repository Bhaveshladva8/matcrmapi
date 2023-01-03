using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ProjectCategoryDetailResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }   
        public string IconURL { get; set; }
    }
}