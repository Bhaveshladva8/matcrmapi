using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class LayoutBackgroundAddUpdateRequest
    {
        public long? Id { get; set; }
        public IFormFile CustomBackgroundImageFile { get; set; }
    }
}