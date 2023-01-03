using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables {
    public class Language {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }

       [Column (TypeName = "varchar(20)")]
        public string? LanguageCode { get; set; }

        [Column (TypeName = "varchar(150)")]
        public string? LanguageName { get; set; }

        public bool IsActive { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}