using System;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Dto {
    public class OrganizationAttachmentDto {
        public OrganizationAttachmentDto(){
            FileList = new IFormFile[] {};
        }
        public long? Id { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public IFormFile[] FileList { get; set; }
        public long? OrganizationId { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShortName { get; set; }
    }
}