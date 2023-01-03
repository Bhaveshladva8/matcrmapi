using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class Office365ReplayEmailResponse
    {   
        public string ErrorMessage { get; set; } = "";        
        public bool IsValid { get; set; } = true;
       
    }
}
