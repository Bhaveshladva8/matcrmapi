using System;

namespace matcrm.data.Models.Response
{
    public class CustomerDeleteEmailResponse
    {
        public long? Id { get; set; }
        public string Email { get; set; }
        public long? EmailTypeId { get; set; }
        public long? CustomerId { get; set; }       
       
    }
}