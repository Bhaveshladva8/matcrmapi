using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Request
{
    public class CalendarTaskAddUpdateRequest
    {
        public CalendarTaskAddUpdateRequest()
        {
            CalendarSubTaskList = new List<CalendarSubTaskDto>();
        }
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public long? CalendarListId { get; set; }
        public DateTime? StartDate { get; set; }
        public string StartTime { get; set; }        
        public bool IsDone { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }        
        public List<CalendarSubTaskDto> CalendarSubTaskList { get; set; }
    }
}