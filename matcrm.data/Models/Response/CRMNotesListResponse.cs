using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class CRMNotesListResponse
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public string LogoURL { get; set; }
        public string ClientUserName { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}