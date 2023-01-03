using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class CustomTaskCreateResponse
    {
        public CustomTaskCreateResponse()
        {
            Clients = new List<CustomTaskClient>();
        }
        public List<CustomTaskClient> Clients { get; set; }
    }
    public class CustomTaskClient
    {
        public CustomTaskClient()
        {
            Contracts = new List<CustomTaskContract>();
        }        
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsContractBaseInvoice { get; set; }
        public List<CustomTaskContract> Contracts { get; set; }
    }

    public class CustomTaskContract
    {
        public CustomTaskContract()
        {
            Projects = new List<CustomTaskProject>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsBillingFromStartDate { get; set; }
        public long? DefaultUnitPrice { get; set; }
        public long? Discount { get; set; }
        public long? ClientId { get; set; }
        public long? Amount { get; set; }
        public long? InvoiceIntervalId { get; set; }
        public long? FinalInvoiceAmount { get; set; }
        public List<CustomTaskProject> Projects { get; set; }
    }

    public class CustomTaskProject
    {
        public CustomTaskProject()
        {
            ProjectTimeRecords = new List<CustomTaskTimeRecord>();
            Tasks = new List<CustomTaskTask>();            
        }
        public long projectId { get; set; }
        public long? ProjectContractTotalAmount { get; set; }
        public List<CustomTaskTimeRecord> ProjectTimeRecords { get; set; }                
        public List<CustomTaskTask> Tasks { get; set; }
    }   

    public class CustomTaskTask
    {
        public CustomTaskTask()
        {
            TaskTimeRecords = new List<CustomTaskTimeRecord>();
            SubTasks = new List<CustomTaskSubTask>();
        }
        public long TaskId { get; set; }
        public long? TaskContractTotalAmount { get; set; }
        public List<CustomTaskTimeRecord> TaskTimeRecords { get; set; }
        public List<CustomTaskSubTask> SubTasks { get; set; }
    }   

    public class CustomTaskSubTask
    {
        public CustomTaskSubTask()
        {
            SubTimeRecords = new List<CustomTaskTimeRecord>();
            ChildTasks = new List<CustomTaskChildTask>();
        }
        public long SubTaskId { get; set; }
        public long? SubTaskTotalAmountProject { get; set; }
        public List<CustomTaskTimeRecord> SubTimeRecords { get; set; }
        public List<CustomTaskChildTask> ChildTasks { get; set; }
    }

    

    public class CustomTaskChildTask
    {
        public CustomTaskChildTask()
        {
            ChildTimeRecords = new List<CustomTaskTimeRecord>();            
        }
        public long ChildTaskId { get; set; }
        public long? ChildTaskTotalAmountProject { get; set; }
        public List<CustomTaskTimeRecord> ChildTimeRecords { get; set; }
    }

    public class CustomTaskTimeRecord
    {
        public long Id { get; set; } 
        public string? Comment { get; set; }
        public bool? IsBillable { get; set; }
        public long? ServiceArticleId { get; set; }
        public bool IsContractUnitPrice { get; set; }
        public long? Duration { get; set; }
        public long? UnitPrice { get; set; }
        public long? TotalAmount { get; set; }        
        public long? TotalTaxAmount { get; set; }
        public long? PayableAmount { get; set; }        
        public DateTime? CreatedOn { get; set; }
    }

}