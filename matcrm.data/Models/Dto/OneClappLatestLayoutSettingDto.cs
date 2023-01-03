using System.Collections.Generic;

namespace matcrm.data.Models.Dto
{
    public class OneClappLatestLayoutSettingDto
    {
        public OneClappLatestLayoutSettingDto()
        {
            ThemeList = new List<OneClappLatestThemeDto>();
            SchemeList = new List<OneClappLatestThemeSchemeDto>();
            LayoutList = new List<OneClappLatestThemeLayoutDto>();
        }
        public List<OneClappLatestThemeDto> ThemeList { get; set; }
        public List<OneClappLatestThemeSchemeDto> SchemeList { get; set; }
        public List<OneClappLatestThemeLayoutDto> LayoutList { get; set; }
    }
}