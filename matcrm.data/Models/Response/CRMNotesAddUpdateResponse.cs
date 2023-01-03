using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class CRMNotesAddUpdateResponse
    {
        public long Id { get; set; }
        public string Note { get; set; }
        public long? ClientUserId { get; set; }
    }
}