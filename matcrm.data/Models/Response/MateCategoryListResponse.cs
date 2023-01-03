using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateCategoryListResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        //public string Color { get; set; }
        public string IconURL { get; set; }        
        public long CustomTableId { get; set; }
        public string CustomTableName { get; set; }
    }
}