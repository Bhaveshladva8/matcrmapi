using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace matcrm.data.Models.Response
{
    public class MateTimeRecordInvoiceResponse
    {
        public MateTimeRecordInvoiceResponse()
        {
            Projects = new List<MateTimeRecordProjectListInvoiceResponse>();
            Tasks = new List<MateTimeRecordTaskListInvoiceResponse>();
            SubTasks = new List<MateTimeRecordSubTaskListInvoiceResponse>();
            ChildTasks = new List<MateTimeRecordChildTaskListInvoiceResponse>();
        }
        public List<MateTimeRecordProjectListInvoiceResponse> Projects { get; set; }
        public List<MateTimeRecordTaskListInvoiceResponse> Tasks { get; set; }
        public List<MateTimeRecordSubTaskListInvoiceResponse> SubTasks { get; set; }
        public List<MateTimeRecordChildTaskListInvoiceResponse> ChildTasks { get; set; }
    }

    public class MateTimeRecordProjectListInvoiceResponse
    {
        public MateTimeRecordProjectListInvoiceResponse()
        {
            ProjectTimeRecords = new List<TimeRecordInvoiceResponse>();
        }
        public long EmployeeProjectId { get; set; }
        public string Description { get; set; }
        public long TotalBillable { get; set; }
        public long TotalNonBillable { get; set; }
        public List<TimeRecordInvoiceResponse> ProjectTimeRecords { get; set; }
    }

    // public class MateProjectTimeRecordInvoiceResponse
    // {
    //     public string UserName { get; set; }
    //     public long MateTimeRecordId { get; set; }
    //     public string Comment { get; set; }
    //     public string Startdate { get; set; }
    //     public long? Duration { get; set; }
    //     public bool? IsBillable { get; set; }
    // }

    public class MateTimeRecordTaskListInvoiceResponse
    {
        public MateTimeRecordTaskListInvoiceResponse()
        {
            TaskTimerecords = new List<TimeRecordInvoiceResponse>();
        }

        public long EmployeeTaskId { get; set; }
        public string Description { get; set; }
        public long TotalBillable { get; set; }
        public long TotalNonBillable { get; set; }
        public List<TimeRecordInvoiceResponse> TaskTimerecords { get; set; }

    }

    // public class MateTaskTimeRecordInvoiceResponse
    // {
    //     public string UserName { get; set; }
    //     public long MateTimeRecordId { get; set; }
    //     public string Comment { get; set; }
    //     public string Startdate { get; set; }
    //     public long? Duration { get; set; }
    //     public bool? IsBillable { get; set; }
    // }

    public class MateTimeRecordSubTaskListInvoiceResponse
    {
        public MateTimeRecordSubTaskListInvoiceResponse()
        {
            SubTaskTimerecords = new List<TimeRecordInvoiceResponse>();
        }

        public long SubTaskId { get; set; }
        public string Description { get; set; }
        public long TotalBillable { get; set; }
        public long TotalNonBillable { get; set; }
        public List<TimeRecordInvoiceResponse> SubTaskTimerecords { get; set; }

    }

    public class TimeRecordInvoiceResponse
    {
        public string UserName { get; set; }
        public long MateTimeRecordId { get; set; }
        public string Comment { get; set; }
        public string Startdate { get; set; }
        public long? Duration { get; set; }
        public bool? IsBillable { get; set; }
    }

    public class MateTimeRecordChildTaskListInvoiceResponse
    {
        public MateTimeRecordChildTaskListInvoiceResponse()
        {
            ChildTaskTimerecords = new List<TimeRecordInvoiceResponse>();
        }

        public long ChildTaskId { get; set; }
        public string Description { get; set; }
        public long TotalBillable { get; set; }
        public long TotalNonBillable { get; set; }
        public List<TimeRecordInvoiceResponse> ChildTaskTimerecords { get; set; }

    }

    // public class MateChildTaskTimeRecordInvoiceResponse
    // {
    //     public string UserName { get; set; }
    //     public long MateTimeRecordId { get; set; }
    //     public string Comment { get; set; }
    //     public string Startdate { get; set; }
    //     public long? Duration { get; set; }
    //     public bool? IsBillable { get; set; }
    // }

}