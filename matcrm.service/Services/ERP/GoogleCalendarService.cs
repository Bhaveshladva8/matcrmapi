using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using matcrm.data.Models.ViewModels;
using matcrm.service.Utility;

namespace matcrm.service.Services.ERP
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        public async Task<List<GoogleCalendarEventVM>> GetEvents(string calendarToken, string calendarKey, string calendarId)
        {
            try
            {
                int pageNumber = 1;
                var googleCalendarEventVMList = new List<GoogleCalendarEventVM>();

                bool hasBoards = true;

                while (hasBoards)
                {
                    var records = await GoogleCalendarApiManager<GoogleCalendarEventVM>.GetAsync("calendars/" + calendarId + "/events?key=" + calendarKey, null, calendarKey, calendarToken);
                    pageNumber++;
                    var data = JsonConvert.DeserializeObject<GoogleCalendarVM>(records);

                    // boards.AddRange (data);
                    // googleCalendarEventVMList = data.Items;
                    googleCalendarEventVMList.AddRange(data.Items);
                    if (data.Items.Count < 1000) hasBoards = false;
                }

                if (googleCalendarEventVMList.Count > 0)
                {
                    return googleCalendarEventVMList;
                }
                return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        public async Task<List<GoogleCalendarEventVM>> GetEventById(string calendarToken, string calendarKey, string calendarId, string eventId)
        {
            try
            {
                int pageNumber = 1;
                var googleCalendarEventVMList = new List<GoogleCalendarEventVM>();

                bool hasBoards = true;

                while (hasBoards)
                {
                    var records = await GoogleCalendarApiManager<GoogleCalendarEventVM>.GetAsync("calendars/" + calendarId + "/events/" + eventId, null, calendarKey, calendarToken);
                    pageNumber++;
                    var data = JsonConvert.DeserializeObject<GoogleCalendarVM>(records);

                    // boards.AddRange (data);
                    googleCalendarEventVMList = data.Items;
                    if (data.Items.Count < 1000) hasBoards = false;
                }

                if (googleCalendarEventVMList.Count > 0)
                {
                    return googleCalendarEventVMList;
                }
                return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        public async Task<GoogleCalendarUser> GetUserInfo(string calendarToken, string calendarKey)
        {
            try
            {
                int pageNumber = 1;
                // var boards = new List<GoogleCalendarEventVM> ();

                // bool hasBoards = true;

                // while (hasBoards) {
                var records = await GoogleCalendarApiManager<GoogleCalendarUser>.GetAsyncByUrl("userinfo", null, "https://www.googleapis.com/oauth2/v3/", calendarToken);
                pageNumber++;
                var data = JsonConvert.DeserializeObject<GoogleCalendarUser>(records);

                // boards.AddRange (data);
                //     boards = data.Items;
                //     if (data.Items.Count < 1000) hasBoards = false;
                // }

                // if (boards.Count > 0) {
                //     return boards;
                // }
                if (data != null)
                {
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        public async Task<GoogleCalendarTokenInfo> GetTokenInfo(string calendarToken, string calendarKey)
        {
            try
            {
                int pageNumber = 1;

                var records = await GoogleCalendarApiManager<GoogleCalendarTokenInfo>.GetAsyncByUrl("tokeninfo?access_token=" + calendarToken, null, "https://www.googleapis.com/oauth2/v3/", calendarToken);
                pageNumber++;
                var data = JsonConvert.DeserializeObject<GoogleCalendarTokenInfo>(records);
                if (data != null)
                {
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        public async Task<GoogleCalendarTokenVM> GetAccessToken(string trelloKey, GoogleCalendarTokenVM listVM)
        {

            var board = await GoogleCalendarApiManager<GoogleCalendarTokenVM>.PostAcccessTokenAsync("token", listVM, trelloKey, "Access_Token");
            if (!string.IsNullOrEmpty(board))
            {
                GoogleCalendarTokenVM result = JsonConvert.DeserializeObject<GoogleCalendarTokenVM>(board);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<GoogleCalendarEventVM> AddEvent(string trelloKey, GoogleCalendarEventVM listVM, string calendarId, string token)
        {
            var googleCalendarEventPostVM = new GoogleCalendarEventPostVM { start = listVM.start, end = listVM.end };
            if (!string.IsNullOrEmpty(listVM.summary))
            {
                googleCalendarEventPostVM.summary = listVM.summary;
            }
            // else
            // {
            //     myObject.summary = "";
            // }
            if (!string.IsNullOrEmpty(listVM.description))
            {
                googleCalendarEventPostVM.description = listVM.description;
            }
            // else
            // {
            //     myObject.description = "";
            // }
            if (listVM.Recurrence != null && listVM.Recurrence.Length > 0)
            {
                googleCalendarEventPostVM.Recurrence = listVM.Recurrence;
            }
            if (listVM.attendees.Count > 0)
            {
                // myObject.Attendee = model.attendee;

                googleCalendarEventPostVM.attendees = new List<CalendarAttendee>();
                foreach (var attendeeObj in listVM.attendees)
                {
                    CalendarAttendee calendarAttendeeObj = new CalendarAttendee();
                    calendarAttendeeObj.email = attendeeObj.email;
                    googleCalendarEventPostVM.attendees.Add(calendarAttendeeObj);
                }
                // myObject.attendees = myObject.attendees.ToArray();
            }
            if (!string.IsNullOrEmpty(listVM.Location))
            {
                googleCalendarEventPostVM.Location = listVM.Location;
            }
            if (!string.IsNullOrEmpty(listVM.colorId))
            {
                googleCalendarEventPostVM.colorId = listVM.colorId;
            }
            if (listVM.creator != null)
            {
                googleCalendarEventPostVM.creator.email = listVM.creator.email;
            }
            var board = await GoogleCalendarApiManager<GoogleCalendarEventPostVM>.PostAsync("calendars/" + calendarId + "/events", googleCalendarEventPostVM, trelloKey, token);
            if (!string.IsNullOrEmpty(board))
            {
                GoogleCalendarEventVM result = JsonConvert.DeserializeObject<GoogleCalendarEventVM>(board);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<GoogleCalendarEventVM> UpdateEvent(string trelloKey, GoogleCalendarEventVM listVM, string calendarId, string token)
        {
            var googleCalendarEventPostVM = new GoogleCalendarEventPostVM { start = listVM.start, end = listVM.end };
            if (!string.IsNullOrEmpty(listVM.summary))
            {
                googleCalendarEventPostVM.summary = listVM.summary;
            }
            // else
            // {
            //     myObject.summary = "";
            // }
            if (!string.IsNullOrEmpty(listVM.description))
            {
                googleCalendarEventPostVM.description = listVM.description;
            }
            // else
            // {
            //     myObject.description = "";
            // }
            if (listVM.Recurrence != null && listVM.Recurrence.Length > 0)
            {
                googleCalendarEventPostVM.Recurrence = listVM.Recurrence;
            }
            if (listVM.attendees.Count > 0)
            {
                googleCalendarEventPostVM.attendees = listVM.attendees;
            }
            if (!string.IsNullOrEmpty(listVM.Location))
            {
                googleCalendarEventPostVM.Location = listVM.Location;
            }
            if (!string.IsNullOrEmpty(listVM.colorId))
            {
                googleCalendarEventPostVM.colorId = listVM.colorId;
            }

            var board = await GoogleCalendarApiManager<GoogleCalendarEventPostVM>.PutAsync("calendars/" + calendarId + "/events/" + listVM.id, googleCalendarEventPostVM, trelloKey, token);
            if (!string.IsNullOrEmpty(board))
            {
                GoogleCalendarEventVM result = JsonConvert.DeserializeObject<GoogleCalendarEventVM>(board);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<GoogleCalendarEventVM> DeleteEvent(string trelloKey, GoogleCalendarEventVM listVM, string calendarId, string token)
        {
            var googleCalendarEventPostVM = await GoogleCalendarApiManager<GoogleCalendarEventPostVM>.DeleteAsync("calendars/" + calendarId + "/events/" + listVM.id, null, trelloKey, token);
            if (!string.IsNullOrEmpty(googleCalendarEventPostVM))
            {
                GoogleCalendarEventVM result = JsonConvert.DeserializeObject<GoogleCalendarEventVM>(googleCalendarEventPostVM);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<GoogleCalendarTokenVM> GetRefreshToken(string trelloKey, GoogleCalendarTokenVM listVM)
        {

            var board = await GoogleCalendarApiManager<GoogleCalendarTokenVM>.PostAcccessTokenAsync("token", listVM, trelloKey, "Refresh_Token");
            if (!string.IsNullOrEmpty(board))
            {
                GoogleCalendarTokenVM result = JsonConvert.DeserializeObject<GoogleCalendarTokenVM>(board);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }
    }

    public interface IGoogleCalendarService
    {
        Task<List<GoogleCalendarEventVM>> GetEvents(string calendarToken, string calendarKey, string calendarId);
        Task<GoogleCalendarTokenVM> GetAccessToken(string trelloKey, GoogleCalendarTokenVM listVM);
        Task<GoogleCalendarTokenVM> GetRefreshToken(string trelloKey, GoogleCalendarTokenVM listVM);
        Task<List<GoogleCalendarEventVM>> GetEventById(string calendarToken, string calendarKey, string calendarId, string eventId);
        Task<GoogleCalendarEventVM> AddEvent(string trelloKey, GoogleCalendarEventVM listVM, string calendarId, string token);
        Task<GoogleCalendarUser> GetUserInfo(string calendarToken, string calendarKey);
        Task<GoogleCalendarTokenInfo> GetTokenInfo(string calendarToken, string calendarKey);
        Task<GoogleCalendarEventVM> UpdateEvent(string trelloKey, GoogleCalendarEventVM listVM, string calendarId, string token);
        Task<GoogleCalendarEventVM> DeleteEvent(string trelloKey, GoogleCalendarEventVM listVM, string calendarId, string token);
    }
}