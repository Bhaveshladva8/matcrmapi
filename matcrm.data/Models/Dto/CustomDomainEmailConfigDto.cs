using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class CustomDomainEmailConfigDto: CommonResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string IMAPHost { get; set; }
        public int? IMAPPort { get; set; }
        public string SMTPHost { get; set; }
        public int? SMTPPort { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Color { get; set; }
        public long? TeamInboxId { get; set; }
        public long? IntProviderAppSecretId { get; set; }

        [ForeignKey("IntProviderAppSecretId")]
        public IntProviderAppSecret IntProviderAppSecret { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}