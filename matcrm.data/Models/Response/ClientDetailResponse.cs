using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientDetailResponse
    {        
        public long Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganizationName { get; set; }
        public string SiteName { get; set; }
        public string SiteContactNumber { get; set; }
        public string SiteAddressLine1 { get; set; }
        public string SiteAddressLine2 { get; set; }
        public string SiteAddressLine3 { get; set; }
        public string PostalCode { get; set; }
        public long? CountryId { get; set; }
        public string CountryName { get; set; }
        public long? StateId { get; set; }
        public string StateName { get; set; }
        public long? CityId { get; set; }
        public string CityName { get; set; }
        public long? TimeZoneId { get; set; }
        public string Time { get; set; }
        public bool IsActive { get; set; }
        public List<ClientEmailDetailResponse> Emails { get; set; }
        public List<ClientPhoneDetailResponse> Phones { get; set; }
        public long? InvoiceIntervalId { get; set; }
        public string InvoiceInterval { get; set; }
        public long? Interval { get; set; }
        public bool IsContractBaseInvoice { get; set; }
        public string Logo { get; set; }   
        public string LogoURL { get; set; }     
    }
    public class ClientEmailDetailResponse
    {
        public long? Id { get; set; }
        public string Email { get; set; }        
        public long? EmailTypeId { get; set; }        
        public bool IsPrimary { get; set; }
    }

    public class ClientPhoneDetailResponse
    {
        public long? Id { get; set; }
        public string PhoneNo { get; set; }        
        public long? PhoneNoTypeId { get; set; }
        public bool IsPrimary { get; set; }       
    }
    
}