namespace matcrm.data.Models.Request
{
    public class OneClappLatestThemeConfigAddUpdateRequest
    {
        public long? CreatedBy { get; set; }
        public string Layout { get; set; }
         public string Scheme { get; set; }
         public string Theme { get; set; }
          public int? UserId { get; set; }
    }
}