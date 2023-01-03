using matcrm.data.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class MultipleThreadMarkAsReadUnReadRequest
    {
        public List<string> ThreadId { get; set; }

        public string ThreadType { get; set; }

        public bool IsRead { get; set; }

        public UserEmail UserEmail { get; set; }
    }
}
