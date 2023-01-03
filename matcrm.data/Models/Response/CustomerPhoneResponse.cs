using System;
namespace matcrm.data.Models.Response
{
    public class CustomerPhoneResponse
    {
        public long? Id { get; set; }
        public long? CustomerId { get; set; }
        public string PhoneNo { get; set; }
        public long? PhoneNoTypeId { get; set; }        
    }
}