using System;
using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Dto
{
    public class SalutationDto
    {
        public long Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}