namespace matcrm.data.Models.ViewModels
{
    public class GoogleCalendarTokenVM
    {
        public string code { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string redirect_uri { get; set; }
        public string access_token { get; set; }
        public long? expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string eventId { get; set; }
        public string email { get; set; }
        public int? userId { get; set; }
    }
}