using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class SyncWeClappToBemateResponse
    {
        public SyncWeClappToBemateResponse(){
            
            CustomFields = new List<CustomFieldDto>();            
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? LabelId { get; set; }
        public string Address { get; set; }
        public long? WeClappOrganizationId { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        
    }
}