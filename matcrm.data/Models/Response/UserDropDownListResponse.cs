using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class UserDropDownListResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ProfileURL { get; set; }
    }
}