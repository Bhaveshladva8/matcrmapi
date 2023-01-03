using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class CustomDomainAuthenticationResponse
    {
        public long? Id { get; set; }
        public long? CustomDomainEmailConfigId { get; set; }
        public CustomDomainEmailConfigDto CustomDomainEmailConfigDto { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Email { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelect { get; set; }        
        
    }
}
