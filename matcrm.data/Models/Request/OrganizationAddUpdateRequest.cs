using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class OrganizationAddUpdateRequest
    {
        public OrganizationAddUpdateRequest(){
            
            CustomFields = new List<CustomFieldDto>();
            
        }
        public long? Id { get; set; }
        public string Address { get; set; }        
        public List<CustomFieldDto> CustomFields { get; set; }
        public long? LabelId { get; set; }
        public string Name { get; set; }        
    }
}
