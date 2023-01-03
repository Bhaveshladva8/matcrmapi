using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("OneClappForm", Schema = "AppForm")]
    public class OneClappForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? Name { get; set; }
        public Guid FormGuid { get; set; }
        public string? FormKey { get; set; }
        public long? FormTypeId { get; set; }

        [Column(TypeName = "jsonb")]
        public object? FormStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? LayoutStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? HeaderStyle { get; set; }

        [ForeignKey("FormTypeId")]
        public virtual OneClappFormType OneClappFormType { get; set; }
        public long? FormActionId { get; set; }

        [ForeignKey("FormActionId")]
        public virtual OneClappFormAction OneClappFormAction { get; set; }
        public bool IsActive { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? ButtonText { get; set; }
        [Column(TypeName = "varchar(1000)")]
        public string? ButtonCssClass { get; set; }
        public bool IsUsePlaceHolder { get; set; }
        public bool IsUseCssClass { get; set; }
        public string? RedirectUrl { get; set; }
        public string? EmbededUrl { get; set; }
        public string? EmbededCode { get; set; }
        public string? SlidingFormPosition { get; set; }
        public long? FormHeaderId { get; set; }
        [ForeignKey("FormHeaderId")]
        public virtual OneClappFormHeader OneClappFormHeader { get; set; }
        public long? FormLayoutId { get; set; }
        [ForeignKey("FormLayoutId")]
        public virtual OneClappFormLayout OneClappFormLayout { get; set; }
        public int? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Tenant Tenant { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

    }
}