using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Dto
{
    public class MateCategoryDto
    {        
        public long? Id { get; set; }    
        public string Name { get; set; }
        public string Icon { get; set; }
        public string IconName { get; set; }        
        //public string Color { get; set; }        
        public long? CustomTableId { get; set; }                
        public int? CreatedBy { get; set; }        
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }        
        public DateTime? UpdatedOn { get; set; }       
        public DateTime? DeletedOn { get; set; }
    }
}