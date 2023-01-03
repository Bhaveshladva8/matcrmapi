
using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class DeleteOrganizationResponse
    {
        public DeleteOrganizationResponse(){
            
            CustomFields = new List<CustomFieldDto>();
            
        }
        public long? Id { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}