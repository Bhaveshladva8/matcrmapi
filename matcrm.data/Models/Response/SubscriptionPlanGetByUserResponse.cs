using System;

namespace matcrm.data.Models.Response
{
    public class SubscriptionPlanGetByUserResponse
    {
        public long? Id { get; set; }
        public long? SubScriptionPlanId { get; set; }
        public int? UserId { get; set; }
        public long? SubscriptionTypeId { get; set; }
        public bool IsSubscribed { get; set; }
        public long? Price { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? SubscribedOn { get; set; }
       
    }
}