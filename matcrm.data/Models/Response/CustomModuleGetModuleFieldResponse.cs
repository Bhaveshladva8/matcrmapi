using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CustomModuleGetModuleFieldResponse
    {
        public CustomModuleGetModuleFieldResponse(){
            CustomFields = new List<CustomFieldDto>();
            ModuleFields = new List<ModuleFieldDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? MasterTableId { get; set; }        
        public long? TenantId { get; set; }        
        public List<CustomFieldDto> CustomFields { get; set; }
        public List<ModuleFieldDto> ModuleFields { get; set; }
    }
}