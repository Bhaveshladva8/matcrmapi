using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormLayoutBackgroundDto
    {
        public long? Id { get; set; }
        [StringLength(1000)]
        public string BackgroundImage { get; set; }
        [StringLength(1000)]
        public string CustomBackgroundImage { get; set; }
        public IFormFile CustomBackgroundImageFile { get; set; }
        [StringLength(1000)]
        public string BackgroundColor { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}