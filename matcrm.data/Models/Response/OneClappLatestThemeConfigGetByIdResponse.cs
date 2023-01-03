using System;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class OneClappLatestThemeConfigGetByIdResponse
    {
        public long Id { get; set; }        
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
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        
    }
}