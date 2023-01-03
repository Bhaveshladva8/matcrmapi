using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using matcrm.data.Context;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto {
    public class CustomModuleDto {
        public CustomModuleDto(){
            CustomFields = new List<CustomFieldDto>();
            ModuleFields = new List<ModuleFieldDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? MasterTableId { get; set; }
        public long? RecordId { get; set; }
        public long? TenantId { get; set; }
        public TenantModuleDto TenantModule { get; set; }
        public ModuleField ModuleField { get; set; }
        public CustomTableDto CustomTable { get; set; }
        public CustomTenantFieldDto CustomTenantField { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public List<ModuleFieldDto> ModuleFields { get; set; }

    }
}