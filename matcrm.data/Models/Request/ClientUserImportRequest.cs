using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Request
{
    public class ClientUserImportRequest
    {
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public long ClientId { get; set; }
        public string Department { get; set; }
        public string ReportTo { get; set; }
        public string ClientUserRole { get; set; }
        public string AdditionalInfo { get; set; }        
        public string WorkEmail { get; set; }
        public string PersonalEmail { get; set; }
        public string MobileNo { get; set; }
        public string WorkTelephoneNo { get; set; }
        public string PrivateTelephoneNo { get; set; }
    }
}