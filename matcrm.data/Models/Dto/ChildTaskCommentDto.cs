using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Dto
{
    public class ChildTaskCommentDto
    {
        public long? Id { get; set; }
        public long? ChildTaskId { get; set; }
        public long? UserId { get; set; }
        public string Comment { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}