using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class CalendarListDto
    {
        public CalendarListDto()
        {
            RemainingCalendarTaskList = new List<CalendarTaskDto>();
            CompletedCalendarTaskList = new List<CalendarTaskDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<CalendarTaskDto> RemainingCalendarTaskList { get; set; }
        public List<CalendarTaskDto> CompletedCalendarTaskList { get; set; }
    }
}