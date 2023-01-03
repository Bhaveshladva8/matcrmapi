using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class SubscriptionPlanGetAllResponse
    {
        public SubscriptionPlanGetAllResponse(){
            Details = new List<SubscriptionPlanDetailDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public long? MonthlyPrice { get; set; }
        public long? YearlyPrice { get; set; }
        public long? TrialPeriod { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }        
        public List<SubscriptionPlanDetailDto> Details { get; set; }
    }
}