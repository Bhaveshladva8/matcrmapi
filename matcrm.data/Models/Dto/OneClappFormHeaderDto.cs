using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormHeaderDto
    {
        public long? Id { get; set; }
        public bool IsEnabled { get; set; }
        [StringLength(1000)]
        public string BackgroundColor { get; set; }
        [StringLength(1000)]
        public string HeaderImage { get; set; }
        [StringLength(1000)]
        public string CustomHeaderImage { get; set; }
        public IFormFile CustomHeaderFile { get; set; }
        [StringLength(1000)]
        public string ImageLink { get; set; }
        public long? ImageSize { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}