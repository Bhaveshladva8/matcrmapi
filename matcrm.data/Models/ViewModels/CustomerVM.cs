using System.Collections.Generic;

namespace matcrm.data.Models.ViewModels
{
    public class CustomerVM
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerNumber { get; set; }
        public string  Error { get; set; }
    }

    public class CustomerResult
    {
        public List<CustomerVM> Result { get; set; }
    }

    public class CustomerCountResult
    {
        public long Result { get; set; }
    }

    public class DynamicResultVM
    {
        // public IEnumerable<object> Result { get; set; }

    }

    public class SomeData
    {
        // public string Id { get; set; }

        public List<IDictionary<string, string>> Result { get; set; }
    }
}