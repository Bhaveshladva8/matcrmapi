using System;

namespace matcrm.data.Models.Response
{
    public class OneClappLatestThemeConfigAddUpdateResponse
    {
        public long Id { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? OneClappLatestThemeId { get; set; }
        public long? OneClappLatestThemeLayoutId { get; set; }
        public long? OneClappLatestThemeSchemeId { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}