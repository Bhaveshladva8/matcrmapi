using System;
using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class CalendarTaskDto
    {
        public CalendarTaskDto()
        {
            CalendarSubTaskList = new List<CalendarSubTaskDto>();
        }
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public long? CalendarListId { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartTime { get; set; }
        public int? RepeatCount { get; set; }
        public long? RepeatTypeId { get; set; }
        public bool IsDone { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public CalendarRepeatTypeDto calendarRepeatType { get; set; }
        public List<CalendarSubTaskDto> CalendarSubTaskList { get; set; }
    }
}