using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Dto
{
    public class UserSubscriptionDto
    {

        public long? Id { get; set; }

        public long? SubScriptionPlanId { get; set; }

        public int? UserId { get; set; }

        public long? SubscriptionTypeId { get; set; }
        public bool IsSubscribed { get; set; }
        public long? Price { get; set; }
        public string WebhookUrl { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? SubscribedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}