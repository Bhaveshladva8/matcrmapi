using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class SubscriptionPlanDto
    {
        public SubscriptionPlanDto(){
            Details = new List<SubscriptionPlanDetailDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
         public long? MonthlyPrice { get; set; }
        public long? YearlyPrice { get; set; }
        public long? TrialPeriod { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<SubscriptionPlanDetailDto> Details { get; set; }
    }
}