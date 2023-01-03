using System;

namespace matcrm.data.Models.ViewModels
{
    public class LanguageVM
    {
        public int LanguageId { get; set; }
        
        public string LanguageCode { get; set; }

        public string LanguageName { get; set; }        

        public bool IsActive { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}