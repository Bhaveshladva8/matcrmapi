using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class AddUpdateEmployeeSubTaskCommentResponse
    {

        public long? Id { get; set; }
        public long? EmployeeSubTaskId { get; set; }
        public int? UserId { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

    }
}