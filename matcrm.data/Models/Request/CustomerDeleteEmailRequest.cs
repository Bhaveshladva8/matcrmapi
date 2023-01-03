using System;

namespace matcrm.data.Models.Request
{
    public class CustomerDeleteEmailRequest
    {
        public long? Id { get; set; }
        public string Email { get; set; }
        public long? EmailTypeId { get; set; }
        public long? CustomerId { get; set; }       
        //public bool isEmailValid { get; set; }
    }
}