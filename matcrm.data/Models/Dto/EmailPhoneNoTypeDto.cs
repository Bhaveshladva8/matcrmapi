using System;

namespace matcrm.data.Models.Dto
{
    public class EmailPhoneNoTypeDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}