using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class DeleteCustomEmailResponse
    {
        public bool IsValid { get; set; }
        public string Label { get; set; }
        public string SelectedEmail { get; set; }
    }
}
