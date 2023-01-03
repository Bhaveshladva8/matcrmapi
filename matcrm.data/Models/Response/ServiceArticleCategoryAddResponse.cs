using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ServiceArticleCategoryAddResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int CreatedBy { get; set; }
    }
}