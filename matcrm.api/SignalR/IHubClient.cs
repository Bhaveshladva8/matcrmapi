using System.Collections.Generic;
using System.Threading.Tasks;

namespace matcrm.api.SignalR
{
    public interface IHubClient
    {
        Task OnTaskCommentEventEmit(long? TaskId);
        Task OnSubTaskCommentEventEmit(long? TaskId);
        Task OnChildTaskCommentEventEmit(long? TaskId);
        Task OnUploadTaskDocumentEventEmit(long? TaskId);
        Task OnEmployeeTaskEventEmit(long? EmployeeTaskId);
        Task OnEmployeeTaskCommentEventEmit(long? EmployeeTaskId);
        Task OnUploadEmployeeTaskDocumentEventEmit(long? EmployeeTaskId);
        Task OnUploadEmployeeSubTaskDocumentEventEmit(long? EmployeeSubTaskId);
        Task OnEmployeeSubTaskCommentEventEmit(long? EmployeeSubTaskId);
         Task OnUploadEmployeeChildTaskDocumentEventEmit(long? EmployeeChildTaskId);
        Task OnEmployeeChildTaskCommentEventEmit(long? EmployeeChildTaskId);
        Task OnCustomerNoteEventEmit(long? CustomerId);
        Task OnOrganizationNoteEventEmit(long? CustomerId);
        Task OnUploadCustomerDocumentEventEmit(long? OrganizationId);
        Task OnUploadOrganizationDocumentEventEmit(long? OrganizationId);
        Task OnUpdateCustomerDescriptionEventEmit(long? CustomerId, long? FileId, string Description);
        Task OnUpdateOrganizationDescriptionEventEmit(long? OrganizationId, long? FileId, string Description);
        Task OnLeadNoteEventEmit(long? LeadId);
        Task OnCustomerActivityEventEmit(long? CustomerId);
        Task OnOrganizationActivityEventEmit(long? OrganizationId);
        Task OnLeadActivityEventEmit(long? LeadId);
        Task OnCustomFieldAddUpdateEmit(long? TenantId, string TableName);
        Task OnDiscussionCommentEmit(long? DiscussionId);
        Task OnMailCommentEmit(string ThreadId);
        Task OnMailModuleEvent(int? userId, string type, string subtype, string updatedId);
        Task OnInviteUserEvent(List<int> userList);
        Task OnMateCommentModuleEvent(long? Id, string type);
        Task OnClientUserEventEmit(long? ClientId);
        Task OnUserSubscriptionEventEmit(int? UserId, string accessToken);
        Task OnProjectModuleEvent(long? ProjectId, int? TenantId);
        Task OnEmployeeTaskModuleEvent(long? TaskId, int? TenantId);
        Task OnMateTicketModuleEvent(long? TicketId, int? TenantId);
    }
}