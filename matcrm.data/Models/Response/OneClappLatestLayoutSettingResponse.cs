using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.Response
{
    public class OneClappLatestLayoutSettingResponse
    {
        public OneClappLatestLayoutSettingResponse()
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