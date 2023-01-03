using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CalendarTaskAddUpdateResponse
    {
        public CalendarTaskAddUpdateResponse()
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
        public List<CalendarSubTaskDto> CalendarSubTaskList { get; set; }
    }
}