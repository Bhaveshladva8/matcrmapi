using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class EmployeeProjectDto
    {

        public EmployeeProjectDto()
        {
            CustomFields = new List<CustomFieldDto>();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? Duration { get; set; }
        // [DataType(DataType.Time)]
        // [DisplayFormat(DataFormatString = "{hh:mm:ss}", ApplyFormatInEditMode = true)]
        // [JsonIgnore]
        //  [System.Text.Json.Serialization.JsonConverterAttribute(typeof(TimeSpanConverter))]
        public TimeSpan? EstimatedTime { get; set; }
        // [NotMapped]
        // public long MyTimeSpanSerializer
        // {
        //     get => EstimatedTime?.Ticks ?? -1;
        //     set => EstimatedTime = value >= 0 ? new TimeSpan(value) : (TimeSpan?)null;
        // }
        public long? Priority { get; set; }
        public long? CurrentPriority { get; set; }
        public long? PreviousPriority { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? StatusId { get; set; }
        public int? TenantId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsKeepTasks { get; set; }
        public IFormFile File { get; set; }
        public List<CustomFieldDto> CustomFields { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchString { get; set; }
        public List<EmployeeProjectUser> AssignedUsers { get; set; }
        public long? ClientId { get; set; }
        public long? MateCategoryId { get; set; }
    }
}