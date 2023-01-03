using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    public class VerificationCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long VerificationCodeId { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string? VerificationFor { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string? Code { get; set; }

        public int? UserId { get; set; }
        public string? Email { get; set; }

        public bool IsUsed { get; set; }

        public bool IsExpired { get; set; }

        public DateTime? ExpiredOn { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}