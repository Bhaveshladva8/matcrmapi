using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateCategoryUpdateResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        //public string Color { get; set; }
        public long CustomTableId { get; set; }
        public string CustomTableName { get; set; }
        public string Category { get; set; }
        public string IconURL { get; set; }
        public string IconName { get; set; }
    }
}