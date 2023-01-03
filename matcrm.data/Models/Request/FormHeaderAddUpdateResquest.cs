using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class FormHeaderAddUpdateResquest
    {
        public long? Id { get; set; }
        public bool IsEnabled { get; set; }
        public IFormFile CustomHeaderFile { get; set; }
    }
}