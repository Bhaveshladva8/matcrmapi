using System;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class MailAssignCustomerDto: CommonResponse
    {
         public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public long? IntProviderAppSecretId { get; set; }
        public string? ThreadId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}