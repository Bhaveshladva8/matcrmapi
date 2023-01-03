using System;

namespace matcrm.data.Models.Dto {
    public class EmailProviderDto {
        public int? Id { get; set; }
        public string ProviderName { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}