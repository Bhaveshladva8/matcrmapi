using System;
using System.ComponentModel.DataAnnotations;

namespace matcrm.data.Models.Dto {
    public class UserERPSystemDto {
        public long? Id { get; set; }

        [StringLength (254)]
        public string Email { get; set; }
        public string AuthKey { get; set; }
        public string Tenant { get; set; }
        public long? ERPId { get; set; }
        public long? UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}