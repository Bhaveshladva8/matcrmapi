using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientAddUpdateResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? OrganizationName { get; set; }
        public string SiteName { get; set; }
        public string SiteContactNumber { get; set; }
        public string SiteAddressLine1 { get; set; }
        public string PostalCode { get; set; }
        public string LogoURL { get; set; }
        public string Status{ get; set; }
        public int? CreatedBy { get; set; }
    }
}