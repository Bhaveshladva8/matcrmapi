using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class CalendarListGetAllResposne
    {
        
        public CalendarListGetAllResposne()
        {
            RemainingCalendarTaskList = new List<CalendarTaskDto>();
            CompletedCalendarTaskList = new List<CalendarTaskDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public List<CalendarTaskDto> RemainingCalendarTaskList { get; set; }
        public List<CalendarTaskDto> CompletedCalendarTaskList { get; set; }     
        
    }
}