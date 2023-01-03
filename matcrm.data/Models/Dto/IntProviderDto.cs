using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto
{
    public class IntProviderDto
    {
        public long? Id { get; set; }
        [StringLength(500)]
        public string Name { get; set; }
        public List<IntProviderAppDto> Apps { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}