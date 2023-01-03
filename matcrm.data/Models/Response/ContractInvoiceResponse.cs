using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Response
{
    public class ClientContractInvoiceResponse
    {
        public ClientContractInvoiceResponse()
        {
            Contracts = new List<ClientContract>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public long? TotalAmount { get; set; }
        public string? CurrencyName { get; set; }
        public List<ClientContract> Contracts { get; set; }
    }

    public class ClientVM
    {
        public ClientVM()
        {
            Tasks = new List<ClientTask>();
            Projects = new List<ClientProject>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public long? TotalAmount { get; set; }
        public long? PayableAmount { get; set; }
        public string? InvoiceIntervalName { get; set; }
        public long? Interval { get; set; }
        public List<ClientTask> Tasks { get; set; }
        public List<ClientProject> Projects { get; set; }
    }

    public class ClientContract
    {
        public ClientContract()
        {
            Tasks = new List<ClientTask>();
            Projects = new List<ClientProject>();
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CancelDate { get; set; }
        public bool IsBillingFromStartDate { get; set; }
        public long? DefaultUnitPrice { get; set; }
        public bool IsContractUnitPrice { get; set; }
        public long? Discount { get; set; }
        public long? ClientId { get; set; }
        public long? ContractTypeId { get; set; }
        public long? Amount { get; set; }
        public long? CurrencyId { get; set; }
        public string? CurrencyName { get; set; }
        public long? InvoiceIntervalId { get; set; }
        public string? InvoiceIntervalName { get; set; }
        public long? Interval { get; set; }
        public int? CreatedBy { get; set; }
        public long? PayableAmount { get; set; }
        public List<ClientTask> Tasks { get; set; }
        public List<ClientProject> Projects { get; set; }
    }

    public class ClientProject
    {
        public ClientProject()
        {
            TaskTrackedTime = new List<CustomTimeRecord>();
            Tasks = new List<ClientTask>();
        }
        public long Id { get; set; }
        [Column(TypeName = "varchar(1500)")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? EstimatedTime { get; set; }
        public DateTime? EndDate { get; set; }
        public long? ClientId { get; set; }
        // public long? Priority { get; set; }
        // public long? StatusId { get; set; }
        // public int? TenantId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public List<CustomTimeRecord> TaskTrackedTime { get; set; }
        public long? PayableAmount { get; set; }
        public List<ClientTask> Tasks { get; set; }
    }

    public class ClientTask
    {
        public ClientTask()
        {
            TaskTrackedTime = new List<CustomTimeRecord>();
            SubTasks = new List<ClientSubTask>();
        }
        public long Id { get; set; }
        // public long? StatusId { get; set; }

        // [ForeignKey("StatusId")]
        // public virtual Status Status { get; set; }
        // public int? TenantId { get; set; }

        // [ForeignKey("TenantId")]
        // public virtual Tenant Tenant { get; set; }
        // public bool IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // public long? Priority { get; set; }
        public long? ClientId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? PayableAmount { get; set; }
        public List<ClientSubTask> SubTasks { get; set; }
        public List<CustomTimeRecord> TaskTrackedTime { get; set; }
    }

    public class ClientSubTask
    {
        public ClientSubTask()
        {
            TaskTrackedTime = new List<CustomTimeRecord>();
            ChildTasks = new List<ClientChildTask>();
        }
        public long Id { get; set; }
        // public long? StatusId { get; set; }

        // [ForeignKey("StatusId")]
        // public virtual Status Status { get; set; }
        // public int? TenantId { get; set; }

        // [ForeignKey("TenantId")]
        // public virtual Tenant Tenant { get; set; }
        // public bool IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // public long? Priority { get; set; }
        public long? ClientId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? TaskId { get; set; }
        public long? PayableAmount { get; set; }
        public List<ClientChildTask> ChildTasks { get; set; }
        public List<CustomTimeRecord> TaskTrackedTime { get; set; }
    }

    public class ClientChildTask
    {
        public ClientChildTask()
        {
            TaskTrackedTime = new List<CustomTimeRecord>();
        }
        public long Id { get; set; }
        // public long? StatusId { get; set; }

        // [ForeignKey("StatusId")]
        // public virtual Status Status { get; set; }
        // public int? TenantId { get; set; }

        // [ForeignKey("TenantId")]
        // public virtual Tenant Tenant { get; set; }
        // public bool IsActive { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // public long? Priority { get; set; }
        public long? ClientId { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? SubTaskId { get; set; }
        public long? PayableAmount { get; set; }
        public List<CustomTimeRecord> TaskTrackedTime { get; set; }
    }

    public class CustomTimeRecord
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public long? Duration { get; set; }
        [Column(TypeName = "varchar")]
        public string? Comment { get; set; }
        public long? UnitPrice { get; set; }
        public bool? IsBillable { get; set; }
        public long? ServiceArticleId { get; set; }
        public long? TotalAmount { get; set; }
        // public long? TotalDiscount { get; set; }
        public long? TotalTaxAmount { get; set; }
        public long? PayableAmount { get; set; }
        public string? CurrencyName { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}