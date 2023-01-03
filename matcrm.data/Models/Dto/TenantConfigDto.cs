using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Dto {
    public class TenantConfigDto {
        public long? Id { get; set; }
        public long? TenantId { get; set; }
        public long? UserId { get; set; }
        public string LogoImage { get; set; }
        public bool IsUploadLogoImg { get; set; }
        public string BackgroundImage { get; set; }
        public bool IsUploadBgImg { get; set; }
        public string Font { get; set; }
        public IFormFile LogoImgFile { get; set; }
        public IFormFile BgImgFile { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}