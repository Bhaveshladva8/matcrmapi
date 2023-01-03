using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class MateCategoryAddRequest
    {
        public MateCategoryAddRequest()
        {
            CustomTableIds = new List<long>();
        }
        public long? Id { get; set; }        
        public string Name { get; set; }
        public IFormFile Icon { get; set; }
        //public string IconName { get; set; }
        //public string Color { get; set; }       
        public List<long> CustomTableIds { get; set; }
    }
}