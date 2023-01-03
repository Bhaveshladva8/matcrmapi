using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace matcrm.data.Models.Request
{
    public class ClientAddUpdateRequest
    {
        public ClientAddUpdateRequest()
        {
            Emails = new List<ClientEmailAddUpdateRequest>();
            Phones = new List<ClientPhoneAddUpdateRequest>();
            CustomFields = new List<CustomFieldDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? OrganizationName { get; set; }
        public string SiteName { get; set; }
        public string SiteContactNumber { get; set; }
        public string SiteAddressLine1 { get; set; }
        public string SiteAddressLine2 { get; set; }
        public string SiteAddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public long? CountryId { get; set; }
        public long? StateId { get; set; }
        public long? CityId { get; set; }
        public long? TimeZoneId { get; set; }
        public bool IsActive { get; set; }
        public string FileName { get; set; }
        public IFormFile File { get; set; }
        public List<ClientEmailAddUpdateRequest> Emails { get; set; }
        public List<ClientPhoneAddUpdateRequest> Phones { get; set; }
        public long? InvoiceIntervalId { get; set; }
        public long? Interval { get; set; }
        public bool IsContractBaseInvoice { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
    }
}