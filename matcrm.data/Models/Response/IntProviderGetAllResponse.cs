using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class IntProviderGetAllResponse
    {
        public long? Id { get; set; }
        
        public string Name { get; set; }
        public List<IntProviderAppDto> Apps { get; set; }
        public DateTime? CreatedOn { get; set; }        
        public bool IsDeleted { get; set; }
        
    }
}
