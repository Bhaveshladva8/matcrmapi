
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.ViewModels {
    public class FinalizeVM {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; }
        public string FileName { get; set; }
        public string Doc { get; set; }
        public IFormFile Image { get; set; }
        public string BgImageUrl { get; set; }
        public string Fonts { get; set; }
        public string HeaderImageUrl { get; set; }
        public string FooterImageUrl { get; set; }
        public string ApiKey { get; set; }
        public string Tenant { get; set; }
    }
}