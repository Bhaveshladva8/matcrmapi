using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto
{
    public class MollieSubscriptionDto
    {
        public long? Id { get; set; }
        public int? UserId { get; set; }
        public string SubscriptionId { get; set; }
        public string PaymentId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}