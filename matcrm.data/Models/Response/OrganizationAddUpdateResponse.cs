using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class OrganizationAddUpdateResponse
    {
        public OrganizationAddUpdateResponse()
        {            
            CustomFields = new List<CustomFieldDto>();
        }
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }        
        public List<CustomFieldDto> CustomFields { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
