using System;
using System.Collections.Generic;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class TeamInboxAccessDto
    {
       
        public long? Id { get; set; }
        public long? TeamInboxId { get; set; }
        public virtual TeamInbox TeamInbox { get; set; }
        public int? TeamMateId { get; set; }
        public virtual User TeamMate { get; set; }

        public bool IsPublic { get; set; }

       
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}