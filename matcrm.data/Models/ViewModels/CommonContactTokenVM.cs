using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.ViewModels
{
    public class CommonContactTokenVM
    {
        public string code { get; set; }
        public string redirect_uri { get; set; }
        public string grant_type { get; set; }
        public string type { get; set; }
        public string refresh_token { get; set; }
    }
}