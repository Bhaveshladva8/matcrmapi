using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class ClientUserAddUpdateResponse
    {
        public long Id { get; set; }        
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public long? ClientId { get; set; }
        public long? DepartmentId { get; set; }
        public long? ReportTo { get; set; }
        public long? ClientUserRoleId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnSubscribe { get; set; }
        public long? IntProviderId { get; set; }
        public string PersonalEmail { get; set; }
        public string WorkEmail { get; set; }
        public string MobileNo { get; set; }
        public string WorkTelephoneNo { get; set; }
        public string PrivateTelephoneNo { get; set; }
        public long SalutationId { get; set; }
        //public string FileName { get; set; }
        public string LogoURL { get; set; }        
    }
}