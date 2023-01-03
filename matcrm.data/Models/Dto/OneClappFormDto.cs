using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using matcrm.data.Models.Tables;

namespace matcrm.data.Models.Dto
{
    public class OneClappFormDto
    {
        public OneClappFormDto()
        {
            // CustomFields = new List<CustomFieldDto>();
            Fields = new List<OneClappFormFieldDto>();
            // OneClappFormStyle = new OneClappFormStyleDto();
            FormHeader = new OneClappFormHeaderDto();
            FormLayout = new OneClappFormLayoutDto();
            // FormFieldStyle = new OneClappFormFieldStyleDto();
        }
        public long? Id { get; set; }
        public string Name { get; set; }
        public Guid? FormGuid { get; set; }
        public string FormKey { get; set; }
        public long? FormTypeId { get; set; }
        public long? FormActionId { get; set; }
        public bool IsActive { get; set; }
        public string ButtonText { get; set; }
        public string? SlidingFormPosition { get; set; }
        public string ButtonCssClass { get; set; }
        public bool IsUsePlaceHolder { get; set; }
        public bool IsUseCssClass { get; set; }
        public string RedirectUrl { get; set; }
        public string EmbededUrl { get; set; }
        public string ModalPopUpUrl { get; set; }
        public string SlidingFormUrl { get; set; }
        public string EmbededCode { get; set; }
        public string JSUrl { get; set; }
        [Column(TypeName = "jsonb")]
        public object? FormStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? LayoutStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? HeaderStyle { get; set; }
        public long? FormHeaderId { get; set; }
        public OneClappFormHeaderDto FormHeader { get; set; }
        public long? FormLayoutId { get; set; }
        public OneClappFormLayoutDto FormLayout { get; set; }
        public int? TenantId { get; set; }
        public long? SubmissionCount { get; set; }
        public long? CreatedBy { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        // public List<CustomFieldDto> CustomFields { get; set; }
        // public CustomFieldDto CustomField { get; set; }
        public List<OneClappFormFieldDto> Fields { get; set; }
    }
}