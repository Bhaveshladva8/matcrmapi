using System;

namespace matcrm.data.Models.Dto
{
    public class OneClappLatestThemeConfigDto
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public UserDto User { get; set; }
        public long? OneClappLatestThemeId { get; set; }
        public string Theme { get; set; }
        public OneClappLatestThemeDto LatestTheme { get; set; }
        public long? OneClappLatestThemeLayoutId { get; set; }
        public string Layout { get; set; }
        public OneClappLatestThemeLayoutDto LatestThemeLayout { get; set; }
        public long? OneClappLatestThemeSchemeId { get; set; }
        public string Scheme { get; set; }
        public OneClappLatestThemeSchemeDto LatestThemeScheme { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }              
    }
}