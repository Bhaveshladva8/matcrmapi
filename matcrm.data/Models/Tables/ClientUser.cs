using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace matcrm.data.Models.Tables
{
    [Table("ClientUser", Schema = "AppCRM")]
    public class ClientUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public long? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public long? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public long? ReportTo { get; set; }
        [ForeignKey("ReportTo")]
        public virtual ClientUser ReportToUser { get; set; }
        public long? ClientUserRoleId { get; set; }
        [ForeignKey("ClientUserRoleId")]
        public virtual ClientUserRole ClientUserRole { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsUnSubscribe { get; set; }
        public long? IntProviderId { get; set; }
        [ForeignKey("IntProviderId")]
        public virtual IntProvider IntProvider { get; set; }
        public string PersonalEmail { get; set; }
        public string WorkEmail { get; set; }
        public string MobileNo { get; set; }
        public string WorkTelephoneNo { get; set; }
        public string PrivateTelephoneNo { get; set; }
        public string Logo { get; set; }
        public int? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public long? SalutationId { get; set; }
        [ForeignKey("SalutationId")]
        public virtual Salutation Salutation { get; set; }

    }
}