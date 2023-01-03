using AutoMapper;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Dto.CustomEmail;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.data.Models.ViewModels.Calendar;
using Ticket = matcrm.data.Models.ViewModels.Ticket;

namespace matcrm.data.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<UserVM, User>();
            CreateMap<User, UserVM>();

            CreateMap<Tenant, TenantDto>();
            CreateMap<TenantDto, Tenant>();

            CreateMap<Language, LanguageDto>();
            CreateMap<LanguageDto, Language>();

            CreateMap<Role, RoleDto>();
            CreateMap<RoleDto, Role>();

            CreateMap<EmailTemplate, EmailTemplateDto>();
            CreateMap<EmailTemplateDto, EmailTemplate>();

            CreateMap<TenantConfig, TenantConfigDto>();
            CreateMap<TenantConfigDto, TenantConfig>();

            CreateMap<TenantActivity, TenantActivityDto>();
            CreateMap<TenantActivityDto, TenantActivity>();

            CreateMap<EmailProvider, EmailProviderDto>();
            CreateMap<EmailProviderDto, EmailProvider>();

            CreateMap<EmailConfig, EmailConfigDto>();
            CreateMap<EmailConfigDto, EmailConfig>();

            CreateMap<EmailLog, EmailLogDto>();
            CreateMap<EmailLogDto, EmailLog>();

            CreateMap<ERPSystem, ERPSystemDto>();
            CreateMap<ERPSystemDto, ERPSystem>();

            CreateMap<UserERPSystem, UserERPSystemDto>();
            CreateMap<UserERPSystemDto, UserERPSystem>();

            CreateMap<EmailConfig, EmailProvider>();
            CreateMap<EmailProvider, EmailConfig>();

            CreateMap<EmailConfig, EmailProviderConfigDto>();
            CreateMap<EmailProviderConfigDto, EmailConfig>();

            CreateMap<CustomControl, CustomControlDto>();
            CreateMap<CustomControlDto, CustomControl>();

            CreateMap<CustomControlOption, CustomControlOptionDto>();
            CreateMap<CustomControlOptionDto, CustomControlOption>();

            CreateMap<CustomField, CustomFieldDto>();
            CreateMap<CustomFieldDto, CustomField>();

            CreateMap<CustomModule, CustomModuleDto>();
            CreateMap<CustomModuleDto, CustomModule>();

            CreateMap<CustomTenantField, CustomTenantFieldDto>();
            CreateMap<CustomTenantFieldDto, CustomTenantField>();

            CreateMap<TenantModule, TenantModuleDto>();
            CreateMap<TenantModuleDto, TenantModule>();

            CreateMap<ModuleField, ModuleFieldDto>();
            CreateMap<ModuleFieldDto, ModuleField>();

            CreateMap<CustomTable, CustomTableDto>();
            CreateMap<CustomTableDto, CustomTable>();

            CreateMap<Models.Tables.Customer, CustomerDto>();
            CreateMap<CustomerDto, Models.Tables.Customer>();

            CreateMap<CustomFieldValue, CustomFieldValueDto>();
            CreateMap<CustomFieldValueDto, CustomFieldValue>();

            CreateMap<CustomerType, CustomerTypeDto>();
            CreateMap<CustomerTypeDto, CustomerType>();

            CreateMap<Tag, TagDto>();
            CreateMap<TagDto, Tag>();

            CreateMap<Models.Tables.Ticket, CustomTicketDto>();
            CreateMap<CustomTicketDto, Models.Tables.Ticket>();

            CreateMap<Organization, OrganizationDto>();
            CreateMap<OrganizationDto, Organization>();

            CreateMap<OrganizationLabel, OrganizationLabelDto>();
            CreateMap<OrganizationLabelDto, OrganizationLabel>();

            CreateMap<CustomerLabel, CustomerLabelDto>();
            CreateMap<CustomerLabelDto, CustomerLabel>();

            CreateMap<EmailPhoneNoType, EmailPhoneNoTypeDto>();
            CreateMap<EmailPhoneNoTypeDto, EmailPhoneNoType>();

            CreateMap<CustomerPhone, CustomerPhoneDto>();
            CreateMap<CustomerPhoneDto, CustomerPhone>();

            CreateMap<CustomerNote, CustomerNoteDto>();
            CreateMap<CustomerNoteDto, CustomerNote>();

            CreateMap<CustomerEmail, CustomerEmailDto>();
            CreateMap<CustomerEmailDto, CustomerEmail>();

            CreateMap<CustomerActivity, CustomerActivityDto>();
            CreateMap<CustomerActivityDto, CustomerActivity>();

            CreateMap<ActivityType, ActivityTypeDto>();
            CreateMap<ActivityTypeDto, ActivityType>();

            CreateMap<CustomerAttachment, CustomerAttachmentDto>();
            CreateMap<CustomerAttachmentDto, CustomerAttachment>();

            CreateMap<OrganizationAttachment, OrganizationAttachmentDto>();
            CreateMap<OrganizationAttachmentDto, OrganizationAttachment>();

            CreateMap<OrganizationNote, OrganizationNoteDto>();
            CreateMap<OrganizationNoteDto, OrganizationNote>();

            CreateMap<OrganizationNotesComment, OrganizationNotesCommentDto>();
            CreateMap<OrganizationNotesCommentDto, OrganizationNotesComment>();

            CreateMap<CustomerNotesComment, CustomerNotesCommentDto>();
            CreateMap<CustomerNotesCommentDto, CustomerNotesComment>();

            CreateMap<CustomTableColumn, CustomTableColumnDto>();
            CreateMap<CustomTableColumnDto, CustomTableColumn>();

            CreateMap<TableColumnUser, TableColumnUserDto>();
            CreateMap<TableColumnUserDto, TableColumnUser>();

            CreateMap<LeadLabel, LeadLabelDto>();
            CreateMap<LeadLabelDto, LeadLabel>();

            CreateMap<Lead, LeadDto>();
            CreateMap<LeadDto, Lead>();

            CreateMap<LeadNote, LeadNoteDto>();
            CreateMap<LeadNoteDto, LeadNote>();

            CreateMap<Label, LabelDto>();
            CreateMap<LabelDto, Label>();

            CreateMap<LabelCategory, LabelCategoryDto>();
            CreateMap<LabelCategoryDto, LabelCategory>();

            CreateMap<ActivityAvailability, ActivityAvailabilityDto>();
            CreateMap<ActivityAvailabilityDto, ActivityAvailability>();

            CreateMap<CustomerActivityMember, CustomerActivityMemberDto>();
            CreateMap<CustomerActivityMemberDto, CustomerActivityMember>();

            CreateMap<OrganizationActivity, OrganizationActivityDto>();
            CreateMap<OrganizationActivityDto, OrganizationActivity>();

            CreateMap<OrganizationActivityMember, OrganizationActivityMemberDto>();
            CreateMap<OrganizationActivityMemberDto, OrganizationActivityMember>();

            CreateMap<LeadActivity, LeadActivityDto>();
            CreateMap<LeadActivityDto, LeadActivity>();

            CreateMap<LeadActivityMember, LeadActivityMemberDto>();
            CreateMap<LeadActivityMemberDto, LeadActivityMember>();

            CreateMap<CalendarList, CalendarListDto>();
            CreateMap<CalendarListDto, CalendarList>();

            CreateMap<CalendarTask, CalendarTaskDto>();
            CreateMap<CalendarTaskDto, CalendarTask>();

            CreateMap<CalendarSubTask, CalendarSubTaskDto>();
            CreateMap<CalendarSubTaskDto, CalendarSubTask>();

            CreateMap<CalendarRepeatType, CalendarRepeatTypeDto>();
            CreateMap<CalendarRepeatTypeDto, CalendarRepeatType>();

            CreateMap<OneClappLatestTheme, OneClappLatestThemeDto>();
            CreateMap<OneClappLatestThemeDto, OneClappLatestTheme>();

            CreateMap<OneClappLatestThemeLayout, OneClappLatestThemeLayoutDto>();
            CreateMap<OneClappLatestThemeLayoutDto, OneClappLatestThemeLayout>();

            CreateMap<OneClappLatestThemeScheme, OneClappLatestThemeSchemeDto>();
            CreateMap<OneClappLatestThemeSchemeDto, OneClappLatestThemeScheme>();

            CreateMap<OneClappLatestThemeConfig, OneClappLatestThemeConfigDto>();
            CreateMap<OneClappLatestThemeConfigDto, OneClappLatestThemeConfig>();

            CreateMap<IntProvider, IntProviderDto>();
            CreateMap<IntProviderDto, IntProvider>();

            CreateMap<IntProviderApp, IntProviderAppDto>();
            CreateMap<IntProviderAppDto, IntProviderApp>();

            CreateMap<IntProviderAppSecret, IntProviderAppSecretDto>();
            CreateMap<IntProviderAppSecretDto, IntProviderAppSecret>();

            CreateMap<CheckList, CheckListDto>();
            CreateMap<CheckListDto, CheckList>();

            CreateMap<CheckListUser, CheckListUserDto>();
            CreateMap<CheckListUserDto, CheckListUser>();

            CreateMap<CalendarSyncActivity, CalendarSyncActivityDto>();
            CreateMap<CalendarSyncActivityDto, CalendarSyncActivity>();

            CreateMap<OneClappModule, OneClappModuleDto>();
            CreateMap<OneClappModuleDto, OneClappModule>();

            CreateMap<OneClappFormType, OneClappFormTypeDto>();
            CreateMap<OneClappFormTypeDto, OneClappFormType>();

            CreateMap<OneClappForm, OneClappFormDto>();
            CreateMap<OneClappFormDto, OneClappForm>();

            CreateMap<OneClappFormField, OneClappFormFieldDto>();
            CreateMap<OneClappFormFieldDto, OneClappFormField>();

            CreateMap<CheckListAssignUser, CheckListAssignUserDto>();
            CreateMap<CheckListAssignUserDto, CheckListAssignUser>();

            CreateMap<User, CheckListAssignUserDto>();
            CreateMap<CheckListAssignUserDto, User>();

            CreateMap<OneClappFormStatus, OneClappFormStatusDto>();
            CreateMap<OneClappFormStatusDto, OneClappFormStatus>();

            CreateMap<OneClappRequestForm, OneClappRequestFormDto>();
            CreateMap<OneClappRequestFormDto, OneClappRequestForm>();

            CreateMap<OneClappFormFieldValue, OneClappFormFieldValueDto>();
            CreateMap<OneClappFormFieldValueDto, OneClappFormFieldValue>();

            CreateMap<OneClappFormField, OneClappFormFieldVM>();
            CreateMap<OneClappFormFieldVM, OneClappFormField>();

            CreateMap<OneClappFormAction, OneClappFormActionDto>();
            CreateMap<OneClappFormActionDto, OneClappFormAction>();

            CreateMap<WeClappUser, WeClappUserDto>();
            CreateMap<WeClappUserDto, WeClappUser>();

            CreateMap<ERPSystemColumnMap, ERPSystemColumnMapDto>();
            CreateMap<ERPSystemColumnMapDto, ERPSystemColumnMap>();

            CreateMap<SubscriptionPlan, SubscriptionPlanDto>();
            CreateMap<SubscriptionPlanDto, SubscriptionPlan>();

            CreateMap<SubscriptionPlanDetail, SubscriptionPlanDetailDto>();
            CreateMap<SubscriptionPlanDetailDto, SubscriptionPlanDetail>();

            CreateMap<UserSubscription, UserSubscriptionDto>();
            CreateMap<UserSubscriptionDto, UserSubscription>();

            CreateMap<SubscriptionType, SubscriptionTypeDto>();
            CreateMap<SubscriptionTypeDto, SubscriptionType>();

            CreateMap<ModuleRecordCustomField, ModuleRecordCustomFieldDto>();
            CreateMap<ModuleRecordCustomFieldDto, ModuleRecordCustomField>();

            CreateMap<MollieCustomer, MollieCustomerDto>();
            CreateMap<MollieCustomerDto, MollieCustomer>();

            CreateMap<MollieSubscription, MollieSubscriptionDto>();
            CreateMap<MollieSubscriptionDto, MollieSubscription>();

            CreateMap<Salutation, SalutationDto>();
            CreateMap<SalutationDto, Salutation>();

            CreateMap<ImportContactAttachment, ImportContactAttachmentDto>();
            CreateMap<ImportContactAttachmentDto, ImportContactAttachment>();

            // Start Mollie map
            CreateMap<CustomerRequest, CreateCustomerModel>();
            CreateMap<CreateCustomerModel, CustomerRequest>();
            // End

            // Start Dynamic form design map

            CreateMap<BorderStyle, BorderStyleDto>();
            CreateMap<BorderStyleDto, BorderStyle>();

            CreateMap<OneClappFormHeader, OneClappFormHeaderDto>();
            CreateMap<OneClappFormHeaderDto, OneClappFormHeader>();

            CreateMap<OneClappFormLayoutBackground, OneClappFormLayoutBackgroundDto>();
            CreateMap<OneClappFormLayoutBackgroundDto, OneClappFormLayoutBackground>();

            CreateMap<OneClappFormLayout, OneClappFormLayoutDto>();
            CreateMap<OneClappFormLayoutDto, OneClappFormLayout>();

            CreateMap<ExternalUser, ExternalUserDto>();
            CreateMap<ExternalUserDto, ExternalUser>();

            CreateMap<MailBoxTeam, MailBoxTeamDto>();
            CreateMap<MailBoxTeamDto, MailBoxTeam>();

            CreateMap<MailAssignUser, MailAssignUserDto>();
            CreateMap<MailAssignUserDto, MailAssignUser>();

            CreateMap<MailRead, MailReadDto>();
            CreateMap<MailReadDto, MailRead>();

            CreateMap<MailComment, MailCommentDto>();
            CreateMap<MailCommentDto, MailComment>();

            CreateMap<Discussion, DiscussionDto>();
            CreateMap<DiscussionDto, Discussion>();

            CreateMap<DiscussionParticipant, DiscussionParticipantDto>();
            CreateMap<DiscussionParticipantDto, DiscussionParticipant>();

            CreateMap<DiscussionCommentAttachment, DiscussionCommentAttachmentDto>();
            CreateMap<DiscussionCommentAttachmentDto, DiscussionCommentAttachment>();

            CreateMap<DiscussionRead, DiscussionReadDto>();
            CreateMap<DiscussionReadDto, DiscussionRead>();

            CreateMap<InboxThread, InboxThreadAndDiscussion>();
            CreateMap<InboxThreadAndDiscussion, InboxThread>();

            CreateMap<Discussion, InboxThreadAndDiscussion>();
            CreateMap<InboxThreadAndDiscussion, Discussion>();

            CreateMap<DiscussionDto, InboxThreadAndDiscussion>();
            CreateMap<InboxThreadAndDiscussion, DiscussionDto>();

            CreateMap<TeamInbox, TeamInboxDto>();
            CreateMap<TeamInboxDto, TeamInbox>();

            CreateMap<DiscussionComment, DiscussionCommentDto>();
            CreateMap<DiscussionCommentDto, DiscussionComment>();

            CreateMap<MailCommentAttachment, MailCommentAttachmentDto>();
            CreateMap<MailCommentAttachmentDto, MailCommentAttachment>();

            CreateMap<ComposeEmail, ComposeEmail1>();
            CreateMap<ComposeEmail1, ComposeEmail>();

            CreateMap<InboxThread, InboxThreadItem>();
            CreateMap<InboxThreadItem, InboxThread>();

            CreateMap<MailParticipant, MailParticipantDto>();
            CreateMap<MailParticipantDto, MailParticipant>();

            CreateMap<TeamInboxAccess, TeamInboxAccessDto>();
            CreateMap<TeamInboxAccessDto, TeamInboxAccess>();

            CreateMap<MailAssignCustomer, MailAssignCustomerDto>();
            CreateMap<MailAssignCustomerDto, MailAssignCustomer>();



            CreateMap<MSEventDate, CalendarEventDate>();
            CreateMap<CalendarEventDate, MSEventDate>();

            CreateMap<MSEventBody, CalendarEventBody>();
            CreateMap<CalendarEventBody, MSEventBody>();

            CreateMap<MSEventAttendee, GoogleMSCalendarAttendee>();
            CreateMap<GoogleMSCalendarAttendee, MSEventAttendee>();


            // CreateMap<MSEventLocation, CalendarEventLocation>();
            // CreateMap<CalendarEventLocation, MSEventLocation>();

            CreateMap<MSEventOrganizer, EventCreatorOrganizerTest>();
            CreateMap<EventCreatorOrganizerTest, MSEventOrganizer>();

            CreateMap<CalendarEventAttendeeStatus, MSEventAttendeeStatus>();
            CreateMap<MSEventAttendeeStatus, CalendarEventAttendeeStatus>();

            CreateMap<CalendarEventEmail, MSEventEmail>();
            CreateMap<MSEventEmail, CalendarEventEmail>();

            CreateMap<EventDate, CalendarEventDate>();
            CreateMap<CalendarEventDate, EventDate>();

            CreateMap<MicrosoftCalendarEventVM, GoogleMicrosoftCalendarEventVM>();
            CreateMap<GoogleMicrosoftCalendarEventVM, MicrosoftCalendarEventVM>();

            CreateMap<CustomDomainEmailConfig, CustomDomainEmailConfigDto>();
            CreateMap<CustomDomainEmailConfigDto, CustomDomainEmailConfig>();

            // End
            //New Request Responce

            CreateMap<MailTokenDto, AuthenticationCompleteRequest>();
            CreateMap<AuthenticationCompleteRequest, MailTokenDto>();

            CreateMap<MailTokenDto, AuthenticationCompleteResponse>();
            CreateMap<AuthenticationCompleteResponse, MailTokenDto>();

            CreateMap<AuthenticationCompleteResponse, AuthenticationCompleteRequest>();
            CreateMap<AuthenticationCompleteRequest, AuthenticationCompleteResponse>();
            //---
            CreateMap<MailBoxTeam, MailBoxTeamRequest>();
            CreateMap<MailBoxTeamRequest, MailBoxTeam>();

            CreateMap<MailBoxTeam, MailBoxTeamResponse>();
            CreateMap<MailBoxTeamResponse, MailBoxTeam>();

            CreateMap<MailBoxTeamDto, MailBoxTeamRequest>();
            CreateMap<MailBoxTeamRequest, MailBoxTeamDto>();

            CreateMap<MailBoxTeamRequest, MailBoxTeamResponse>();
            CreateMap<MailBoxTeamResponse, MailBoxTeamRequest>();
            //====
            CreateMap<TeamInboxAddUpdateAccessRequest, TeamInboxDto>();
            CreateMap<TeamInboxDto, TeamInboxAddUpdateAccessRequest>();

            CreateMap<TeamInboxAddUpdateAccessResponse, TeamInboxDto>();
            CreateMap<TeamInboxDto, TeamInboxAddUpdateAccessResponse>();

            CreateMap<TeamInboxAddUpdateAccessResponse, TeamInboxAddUpdateAccessRequest>();
            CreateMap<TeamInboxAddUpdateAccessRequest, TeamInboxAddUpdateAccessResponse>();
            //==
            CreateMap<CustomDomainEmailConfigDto, CustomDomainEmailConnectionRequest>();
            CreateMap<CustomDomainEmailConnectionRequest, CustomDomainEmailConfigDto>();

            CreateMap<CustomDomainEmailConfigDto, CustomDomainEmailConnectionResponse>();
            CreateMap<CustomDomainEmailConnectionResponse, CustomDomainEmailConfigDto>();

            CreateMap<CustomDomainEmailConnectionRequest, CustomDomainEmailConnectionResponse>();
            CreateMap<CustomDomainEmailConnectionResponse, CustomDomainEmailConnectionRequest>();
            //==            
            CreateMap<TeamInboxDto, TeamInboxResponse>();
            CreateMap<TeamInboxResponse, TeamInboxDto>();

            CreateMap<TeamInboxDto, TeamInboxRequest>();
            CreateMap<TeamInboxRequest, TeamInboxDto>();

            CreateMap<TeamInboxResponse, TeamInboxRequest>();
            CreateMap<TeamInboxRequest, TeamInboxResponse>();
            //==

            CreateMap<CustomDomainAuthenticationRequest, CustomDomainEmailConfigDto>();
            CreateMap<CustomDomainEmailConfigDto, CustomDomainAuthenticationRequest>();

            CreateMap<CustomDomainAuthenticationResponse, CustomDomainEmailConfigDto>();
            CreateMap<CustomDomainEmailConfigDto, CustomDomainAuthenticationResponse>();

            CreateMap<CustomDomainAuthenticationResponse, IntProviderAppSecretDto>();
            CreateMap<IntProviderAppSecretDto, CustomDomainAuthenticationResponse>();

            CreateMap<CustomDomainAuthenticationRequest, CustomDomainAuthenticationResponse>();
            CreateMap<CustomDomainAuthenticationResponse, CustomDomainAuthenticationRequest>();

            //==
            CreateMap<MailTokenDto, MailTokenRequest>();
            CreateMap<MailTokenRequest, MailTokenDto>();

            CreateMap<MailTokenDto, MailTokenResponse>();
            CreateMap<MailTokenResponse, MailTokenDto>();

            CreateMap<MailTokenResponse, MailTokenRequest>();
            CreateMap<MailTokenRequest, MailTokenResponse>();
            //==
            CreateMap<MailTokenDto, CustomEmailFolderRequest>();
            CreateMap<CustomEmailFolderRequest, MailTokenDto>();
            //==
            CreateMap<MailTokenDto, ThreadsRequest>();
            CreateMap<ThreadsRequest, MailTokenDto>();

            CreateMap<InboxThreads, ThreadsResponse>();
            CreateMap<ThreadsResponse, InboxThreads>();
            //===
            CreateMap<MailTokenDto, AssignTomeRequest>();
            CreateMap<AssignTomeRequest, MailTokenDto>();

            CreateMap<InboxThreads, AssignTomeResponse>();
            CreateMap<AssignTomeResponse, InboxThreads>();
            //==
            CreateMap<InboxVM, ThreadByThreadIdRequest>();
            CreateMap<ThreadByThreadIdRequest, InboxVM>();

            CreateMap<InboxThreadItem, ThreadByThreadIdResponse>();
            CreateMap<ThreadByThreadIdResponse, InboxThreadItem>();
            //==
            CreateMap<InboxThreads, ShareTomeResponse>();
            CreateMap<ShareTomeResponse, InboxThreads>();
            //==
            CreateMap<UserEmail, ReadUnReadByThreadRequest>();
            CreateMap<ReadUnReadByThreadRequest, UserEmail>();

            CreateMap<InboxThreads, ReadUnReadByThreadResponse>();
            CreateMap<ReadUnReadByThreadResponse, InboxThreads>();
            //==
            CreateMap<ThreadOperationVM, MultipleThreadMarkAsReadUnReadRequest>();
            CreateMap<MultipleThreadMarkAsReadUnReadRequest, ThreadOperationVM>();

            CreateMap<InboxThreads, MultipleThreadMarkAsReadUnReadResponse>();
            CreateMap<MultipleThreadMarkAsReadUnReadResponse, InboxThreads>();
            //==
            CreateMap<MailTokenDto, DeleteCustomEmailRequest>();
            CreateMap<DeleteCustomEmailRequest, MailTokenDto>();

            CreateMap<MailTokenDto, DeleteCustomEmailResponse>();
            CreateMap<DeleteCustomEmailResponse, MailTokenDto>();

            CreateMap<DeleteCustomEmailRequest, DeleteCustomEmailResponse>();
            CreateMap<DeleteCustomEmailResponse, DeleteCustomEmailRequest>();
            //==
            CreateMap<MailTokenDto, TrashEmailByThreadRequest>();
            CreateMap<TrashEmailByThreadRequest, MailTokenDto>();

            CreateMap<InboxThreads, TrashEmailByThreadResponse>();
            CreateMap<TrashEmailByThreadResponse, InboxThreads>();
            //===
            CreateMap<UserEmail, MarkAsReadUnReadRequest>();
            CreateMap<MarkAsReadUnReadRequest, UserEmail>();

            CreateMap<InboxThreads, MarkAsReadUnReadResponse>();
            CreateMap<MarkAsReadUnReadResponse, InboxThreads>();

            //==
            CreateMap<ComposeEmail1, Office365ForwardEmailRequest>();
            CreateMap<Office365ForwardEmailRequest, ComposeEmail1>();

            CreateMap<Value, Office365ForwardEmailResponse>();
            CreateMap<Office365ForwardEmailResponse, Value>();

            //==

            CreateMap<ComposeEmail1, Office365ReplayEmailRequest>();
            CreateMap<Office365ReplayEmailRequest, ComposeEmail1>();

            CreateMap<Value, Office365ReplayEmailResponse>();
            CreateMap<Office365ReplayEmailResponse, Value>();

            //==
            CreateMap<ComposeEmail1, GmailReplyEmailRequest>();
            CreateMap<GmailReplyEmailRequest, ComposeEmail1>();

            CreateMap<ComposeEmail1, GmailReplyEmailResponse>();
            CreateMap<GmailReplyEmailResponse, ComposeEmail1>();

            //==
            CreateMap<ComposeEmail1, SendEmailRequest>();
            CreateMap<SendEmailRequest, ComposeEmail1>();

            CreateMap<ComposeEmail1, SendEmailResponse>();
            CreateMap<SendEmailResponse, ComposeEmail1>();

            //==
            CreateMap<ComposeEmail1, CreateGmailDraftRequest>();
            CreateMap<CreateGmailDraftRequest, ComposeEmail1>();

            CreateMap<ComposeEmail, CreateGmailDraftResponse>();
            CreateMap<CreateGmailDraftResponse, ComposeEmail>();

            //==

            CreateMap<ComposeEmail1, Office365CreateDraftRequest>();
            CreateMap<Office365CreateDraftRequest, ComposeEmail1>();

            CreateMap<Value, Office365CreateDraftResponse>();
            CreateMap<Office365CreateDraftResponse, Value>();

            //==

            CreateMap<MailAssignUserDto, MailAssignUserRequest>();
            CreateMap<MailAssignUserRequest, MailAssignUserDto>();

            CreateMap<MailAssignUserDto, MailAssignUserResposne>();
            CreateMap<MailAssignUserResposne, MailAssignUserDto>();

            CreateMap<MailAssignUserRequest, MailAssignUserResposne>();
            CreateMap<MailAssignUserResposne, MailAssignUserRequest>();

            //==

            CreateMap<MailAssignUserDto, MailUnAssignUserRequest>();
            CreateMap<MailUnAssignUserRequest, MailAssignUserDto>();

            CreateMap<MailAssignUserDto, MailUnAssignUserResponse>();
            CreateMap<MailUnAssignUserResponse, MailAssignUserDto>();

            CreateMap<MailUnAssignUserRequest, MailUnAssignUserResponse>();
            CreateMap<MailUnAssignUserResponse, MailUnAssignUserRequest>();

            //==

            CreateMap<MailAssignCustomerDto, MailAssignCustomerRequest>();
            CreateMap<MailAssignCustomerRequest, MailAssignCustomerDto>();

            CreateMap<MailAssignCustomerDto, MailAssignCustomerResponse>();
            CreateMap<MailAssignCustomerResponse, MailAssignCustomerDto>();

            CreateMap<MailAssignCustomerRequest, MailAssignCustomerResponse>();
            CreateMap<MailAssignCustomerResponse, MailAssignCustomerRequest>();

            //==
            CreateMap<MailAssignCustomerDto, MailUnAssignCustomerRequest>();
            CreateMap<MailUnAssignCustomerRequest, MailAssignCustomerDto>();

            CreateMap<MailAssignCustomerDto, MailUnAssignCustomerResponse>();
            CreateMap<MailUnAssignCustomerResponse, MailAssignCustomerDto>();

            CreateMap<MailUnAssignCustomerRequest, MailUnAssignCustomerResponse>();
            CreateMap<MailUnAssignCustomerResponse, MailUnAssignCustomerRequest>();

            //==
            CreateMap<MailParticipantDto, MailParticipantRequest>();
            CreateMap<MailParticipantRequest, MailParticipantDto>();

            CreateMap<MailParticipantDto, MailParticipantResponse>();
            CreateMap<MailParticipantResponse, MailParticipantDto>();

            CreateMap<MailParticipantRequest, MailParticipantResponse>();
            CreateMap<MailParticipantResponse, MailParticipantRequest>();

            //==

            CreateMap<MailTokenDto, TrashRequest>();
            CreateMap<TrashRequest, MailTokenDto>();

            CreateMap<InboxThreads, TrashResponse>();
            CreateMap<TrashResponse, InboxThreads>();

            //==

            CreateMap<MailTokenDto, AssignToCustomerRequest>();
            CreateMap<AssignToCustomerRequest, MailTokenDto>();

            CreateMap<InboxThreads, AssignToCustomerResponse>();
            CreateMap<AssignToCustomerResponse, InboxThreads>();

            //==

            CreateMap<TeamInboxDto, TeamInboxAddUpdateRequest>();
            CreateMap<TeamInboxAddUpdateRequest, TeamInboxDto>();

            CreateMap<TeamInboxDto, TeamInboxAddUpdateResponse>();
            CreateMap<TeamInboxAddUpdateResponse, TeamInboxDto>();

            CreateMap<TeamInboxAddUpdateRequest, TeamInboxAddUpdateResponse>();
            CreateMap<TeamInboxAddUpdateResponse, TeamInboxAddUpdateRequest>();
            //===
            CreateMap<TeamInboxDto, RemoveTeamInboxResponse>();
            CreateMap<RemoveTeamInboxResponse, TeamInboxDto>();

            //==
            //===
            CreateMap<ComposeEmail1, SendReplyRequest>();
            CreateMap<SendReplyRequest, ComposeEmail1>();

            CreateMap<ComposeEmail1, SendReplyResponse>();
            CreateMap<SendReplyResponse, ComposeEmail1>();

            CreateMap<SendReplyRequest, SendReplyResponse>();
            CreateMap<SendReplyResponse, SendReplyRequest>();

            //===
            CreateMap<MailCommentDto, MailCommentAddUpdateRequest>();
            CreateMap<MailCommentAddUpdateRequest, MailCommentDto>();

            CreateMap<MailCommentDto, MailCommentAddUpdateResponse>();
            CreateMap<MailCommentAddUpdateResponse, MailCommentDto>();

            CreateMap<MailComment, MailCommentAddUpdateResponse>();
            CreateMap<MailCommentAddUpdateResponse, MailComment>();

            CreateMap<MailCommentAddUpdateRequest, MailCommentAddUpdateResponse>();
            CreateMap<MailCommentAddUpdateResponse, MailCommentAddUpdateRequest>();

            //==
            CreateMap<MailCommentDto, MailCommentPinUnpinRequest>();
            CreateMap<MailCommentPinUnpinRequest, MailCommentDto>();

            CreateMap<MailComment, MailCommentPinUnpinResponse>();
            CreateMap<MailCommentPinUnpinResponse, MailComment>();

            CreateMap<MailCommentPinUnpinRequest, MailCommentPinUnpinResponse>();
            CreateMap<MailCommentPinUnpinResponse, MailCommentPinUnpinRequest>();

            //==
            CreateMap<MailComment, MailCommentGetAllResponse>();
            CreateMap<MailCommentGetAllResponse, MailComment>();

            CreateMap<MailCommentDto, MailCommentGetAllResponse>();
            CreateMap<MailCommentGetAllResponse, MailCommentDto>();
            //==
            CreateMap<MailCommentAttachment, NewMailCommentAttachment>();
            CreateMap<NewMailCommentAttachment, MailCommentAttachment>();

            //==
            CreateMap<MailCommentAttachmentDto, NewMailCommentAttachment>();
            CreateMap<NewMailCommentAttachment, MailCommentAttachmentDto>();

            //==
            CreateMap<MailCommentDto, MailCommentDeleteRequest>();
            CreateMap<MailCommentDeleteRequest, MailCommentDto>();

            CreateMap<MailCommentDto, MailCommentDeleteResponse>();
            CreateMap<MailCommentDeleteResponse, MailCommentDto>();

            //==
            CreateMap<DiscussionDto, DiscussionAddUpdateRequest>();
            CreateMap<DiscussionAddUpdateRequest, DiscussionDto>();

            CreateMap<DiscussionDto, DiscussionAddUpdateResponse>();
            CreateMap<DiscussionAddUpdateResponse, DiscussionDto>();

            //==
            CreateMap<MailTokenDto, DiscussionGetAllRequest>();
            CreateMap<DiscussionGetAllRequest, MailTokenDto>();

            CreateMap<DiscussionDto, DiscussionGetAllResponse>();
            CreateMap<DiscussionGetAllResponse, DiscussionDto>();

            //==
            CreateMap<DiscussionDto, GetDiscussionResponse>();
            CreateMap<GetDiscussionResponse, DiscussionDto>();

            //==
            CreateMap<DiscussionDto, DiscussionAssignUserRequest>();
            CreateMap<DiscussionAssignUserRequest, DiscussionDto>();

            CreateMap<DiscussionDto, DiscussionAssignUserResponse>();
            CreateMap<DiscussionAssignUserResponse, DiscussionDto>();
            //==
            CreateMap<DiscussionDto, DiscussionUnAssignRequest>();
            CreateMap<DiscussionUnAssignRequest, DiscussionDto>();

            CreateMap<DiscussionDto, DiscussionUnAssignResponse>();
            CreateMap<DiscussionUnAssignResponse, DiscussionDto>();
            //==
            CreateMap<DiscussionDto, DiscussionAssignCustomerRequest>();
            CreateMap<DiscussionAssignCustomerRequest, DiscussionDto>();

            CreateMap<DiscussionDto, DiscussionAssignCustomerResponse>();
            CreateMap<DiscussionAssignCustomerResponse, DiscussionDto>();
            //==
            CreateMap<DiscussionDto, DiscussionUnAssignCustRequest>();
            CreateMap<DiscussionUnAssignCustRequest, DiscussionDto>();

            CreateMap<DiscussionDto, DiscussionUnAssignCustResponse>();
            CreateMap<DiscussionUnAssignCustResponse, DiscussionDto>();
            //==
            CreateMap<DiscussionParticipantDto, DiscussionParticipantRequest>();
            CreateMap<DiscussionParticipantRequest, DiscussionParticipantDto>();

            CreateMap<DiscussionParticipantDto, DiscussionParticipantResponse>();
            CreateMap<DiscussionParticipantResponse, DiscussionParticipantDto>();
            //==
            CreateMap<DiscussionDto, DiscussionTrashRequest>();
            CreateMap<DiscussionTrashRequest, DiscussionDto>();

            CreateMap<DiscussionDto, DiscussionTrashResponse>();
            CreateMap<DiscussionTrashResponse, DiscussionDto>();

            //==
            CreateMap<UserDto, DiscussionAllTeamMateResponse>();
            CreateMap<DiscussionAllTeamMateResponse, UserDto>();

            //==
            //==
            CreateMap<DiscussionCommentDto, DiscussionCommentAddUpdateRequest>();
            CreateMap<DiscussionCommentAddUpdateRequest, DiscussionCommentDto>();

            CreateMap<DiscussionCommentDto, DiscussionCommentAddUpdateResponse>();
            CreateMap<DiscussionCommentAddUpdateResponse, DiscussionCommentDto>();
            //==
            CreateMap<DiscussionCommentDto, DiscussionCommentPinUnpinRequest>();
            CreateMap<DiscussionCommentPinUnpinRequest, DiscussionCommentDto>();

            CreateMap<DiscussionCommentDto, DiscussionCommentPinUnpinResponse>();
            CreateMap<DiscussionCommentPinUnpinResponse, DiscussionCommentDto>();
            //==
            CreateMap<DiscussionCommentAttachment, NewDiscussionCommentAttachment>();
            CreateMap<NewDiscussionCommentAttachment, DiscussionCommentAttachment>();

            //==
            CreateMap<DiscussionCommentAttachmentDto, NewDiscussionCommentAttachment>();
            CreateMap<NewDiscussionCommentAttachment, DiscussionCommentAttachmentDto>();
            //==
            CreateMap<DiscussionCommentDto, DiscussionCommentRequest>();
            CreateMap<DiscussionCommentRequest, DiscussionCommentDto>();

            CreateMap<DiscussionCommentDto, DiscussionCommentResponse>();
            CreateMap<DiscussionCommentResponse, DiscussionCommentDto>();

            //==
            CreateMap<IntProviderDto, IntProviderGetAllResponse>();
            CreateMap<IntProviderGetAllResponse, IntProviderDto>();
            //End Request Responce
            //Task management module start

            CreateMap<Models.Tables.TaskStatus, TaskStatusDto>();
            CreateMap<TaskStatusDto, Models.Tables.TaskStatus>();

            CreateMap<OneClappTask, OneClappTaskDto>();
            CreateMap<OneClappTaskDto, OneClappTask>();

            CreateMap<OneClappTaskUser, OneClappTaskUserDto>();
            CreateMap<OneClappTaskUserDto, OneClappTaskUser>();

            CreateMap<OneClappSubTask, OneClappSubTaskDto>();
            CreateMap<OneClappSubTaskDto, OneClappSubTask>();

            CreateMap<OneClappSubTaskUser, OneClappSubTaskUserDto>();
            CreateMap<OneClappSubTaskUserDto, OneClappSubTaskUser>();

            CreateMap<OneClappChildTask, OneClappChildTaskDto>();
            CreateMap<OneClappChildTaskDto, OneClappChildTask>();

            CreateMap<OneClappChildTaskUser, OneClappChildTaskUserDto>();
            CreateMap<OneClappChildTaskUserDto, OneClappChildTaskUser>();

            CreateMap<OneClappTask, OneClappTaskVM>();
            CreateMap<OneClappTaskVM, OneClappTask>();

            CreateMap<OneClappSubTask, OneClappSubTaskVM>();
            CreateMap<OneClappSubTaskVM, OneClappSubTask>();

            CreateMap<OneClappChildTask, OneClappChildTaskVM>();
            CreateMap<OneClappChildTaskVM, OneClappChildTask>();

            CreateMap<TaskTimeRecord, TaskTimeRecordDto>();
            CreateMap<TaskTimeRecordDto, TaskTimeRecord>();

            CreateMap<SubTaskTimeRecord, SubTaskTimeRecordDto>();
            CreateMap<SubTaskTimeRecordDto, SubTaskTimeRecord>();

            CreateMap<ChildTaskTimeRecord, ChildTaskTimeRecordDto>();
            CreateMap<ChildTaskTimeRecordDto, ChildTaskTimeRecord>();

            CreateMap<TaskAttachment, TaskAttachmentDto>();
            CreateMap<TaskAttachmentDto, TaskAttachment>();

            CreateMap<SubTaskAttachment, SubTaskAttachmentDto>();
            CreateMap<SubTaskAttachmentDto, SubTaskAttachment>();

            CreateMap<ChildTaskAttachment, ChildTaskAttachmentDto>();
            CreateMap<ChildTaskAttachmentDto, ChildTaskAttachment>();

            CreateMap<TaskComment, TaskCommentDto>();
            CreateMap<TaskCommentDto, TaskComment>();

            CreateMap<SubTaskComment, SubTaskCommentDto>();
            CreateMap<SubTaskCommentDto, SubTaskComment>();

            CreateMap<ChildTaskComment, ChildTaskCommentDto>();
            CreateMap<ChildTaskCommentDto, ChildTaskComment>();

            CreateMap<TaskActivity, TaskActivityDto>();
            CreateMap<TaskActivityDto, TaskActivity>();

            CreateMap<SubTaskActivity, SubTaskActivityDto>();
            CreateMap<SubTaskActivityDto, SubTaskActivity>();

            CreateMap<ChildTaskActivity, ChildTaskActivityDto>();
            CreateMap<ChildTaskActivityDto, ChildTaskActivity>();

            CreateMap<Section, SectionDto>();
            CreateMap<SectionDto, Section>();

            CreateMap<Section, SectionVM>();
            CreateMap<SectionVM, Section>();

            CreateMap<SectionActivity, SectionActivityDto>();
            CreateMap<SectionActivityDto, SectionActivity>();

            CreateMap<TaskWeclappUser, TaskWeclappUserDto>();
            CreateMap<TaskWeclappUserDto, TaskWeclappUser>();

            // End
            //start NEW
            CreateMap<DashboardInfoDto, DashboardNewInfo>();
            CreateMap<DashboardNewInfo, DashboardInfoDto>();
            //
            CreateMap<CustomerDto, CustomerNewGetAllResponse>();
            CreateMap<CustomerNewGetAllResponse, CustomerDto>();
            //
            CreateMap<CustomerDto, CustomerAddUpdateResponse>();
            CreateMap<CustomerAddUpdateResponse, CustomerDto>();

            CreateMap<CustomerDto, CustomerAddUpdateRequest>();
            CreateMap<CustomerAddUpdateRequest, CustomerDto>();
            //
            CreateMap<CustomerDto, DeleteCustomerRequest>();
            CreateMap<DeleteCustomerRequest, CustomerDto>();

            CreateMap<CustomerDto, DeleteCustomerResponse>();
            CreateMap<DeleteCustomerResponse, CustomerDto>();
            //
            CreateMap<CustomerDto, CustomerGetDetailResponse>();
            CreateMap<CustomerGetDetailResponse, CustomerDto>();
            //
            CreateMap<SalutationDto, CustomerGetAllSalutationResponse>();
            CreateMap<CustomerGetAllSalutationResponse, SalutationDto>();
            //
            CreateMap<EmailPhoneNoTypeDto, CustomerGetAllEmailPhoneTypeResponse>();
            CreateMap<CustomerGetAllEmailPhoneTypeResponse, EmailPhoneNoTypeDto>();
            //
            CreateMap<CustomerEmailDto, CustomerDeleteEmailRequest>();
            CreateMap<CustomerDeleteEmailRequest, CustomerEmailDto>();

            CreateMap<CustomerEmailDto, CustomerDeleteEmailResponse>();
            CreateMap<CustomerDeleteEmailResponse, CustomerEmailDto>();
            //
            CreateMap<CustomerDto, CustomerSyncToWeClappResponse>();
            CreateMap<CustomerSyncToWeClappResponse, CustomerDto>();
            //
            CreateMap<SyncContactDto, SyncCustomerRequest>();
            CreateMap<SyncCustomerRequest, SyncContactDto>();

            CreateMap<CustomerDto, SyncCustomerResponse>();
            CreateMap<SyncCustomerResponse, CustomerDto>();
            //
            CreateMap<CustomerPhoneDto, CustomerPhoneRequest>();
            CreateMap<CustomerPhoneRequest, CustomerPhoneDto>();

            CreateMap<CustomerPhoneDto, CustomerPhoneResponse>();
            CreateMap<CustomerPhoneResponse, CustomerPhoneDto>();
            //
            CreateMap<LeadDto, LeadResponse>();
            CreateMap<LeadResponse, LeadDto>();
            //
            CreateMap<LeadDto, LeadAddUpdateRequest>();
            CreateMap<LeadAddUpdateRequest, LeadDto>();

            CreateMap<LeadDto, LeadAddUpdateResponse>();
            CreateMap<LeadAddUpdateResponse, LeadDto>();
            //
            CreateMap<LeadDto, DeleteLeadRequest>();
            CreateMap<DeleteLeadRequest, LeadDto>();

            CreateMap<LeadDto, DeleteLeadResponse>();
            CreateMap<DeleteLeadResponse, LeadDto>();
            //
            CreateMap<LeadDto, GetLeadResponse>();
            CreateMap<GetLeadResponse, LeadDto>();
            //
            CreateMap<OrganizationDto, GetAllOrganizationResponse>();
            CreateMap<GetAllOrganizationResponse, OrganizationDto>();
            //
            CreateMap<OrganizationDto, OrganizationAddUpdateRequest>();
            CreateMap<OrganizationAddUpdateRequest, OrganizationDto>();

            CreateMap<Organization, OrganizationAddUpdateResponse>();
            CreateMap<OrganizationAddUpdateResponse, Organization>();
            //
            CreateMap<OrganizationDto, GetOrganizationResponse>();
            CreateMap<GetOrganizationResponse, OrganizationDto>();
            //
            CreateMap<OrganizationDto, DeleteOrganizationRequest>();
            CreateMap<DeleteOrganizationRequest, OrganizationDto>();

            CreateMap<OrganizationDto, DeleteOrganizationResponse>();
            CreateMap<DeleteOrganizationResponse, OrganizationDto>();
            //
            CreateMap<OrganizationDto, SyncbeMateToWeClappResponse>();
            CreateMap<SyncbeMateToWeClappResponse, OrganizationDto>();
            //
            CreateMap<SyncContactDto, SyncWeClappToBemateResquest>();
            CreateMap<SyncWeClappToBemateResquest, SyncContactDto>();

            CreateMap<OrganizationDto, SyncWeClappToBemateResponse>();
            CreateMap<SyncWeClappToBemateResponse, OrganizationDto>();
            //
            CreateMap<CustomerNoteDto, CustomerNoteAddUpdateRequest>();
            CreateMap<CustomerNoteAddUpdateRequest, CustomerNoteDto>();

            CreateMap<CustomerNote, CustomerNoteAddUpdateResponse>();
            CreateMap<CustomerNoteAddUpdateResponse, CustomerNote>();
            //
            CreateMap<CustomerNoteDto, CustomerNoteDeleteRequest>();
            CreateMap<CustomerNoteDeleteRequest, CustomerNoteDto>();

            CreateMap<CustomerNote, CustomerNoteDeleteResponse>();
            CreateMap<CustomerNoteDeleteResponse, CustomerNote>();
            //
            CreateMap<CustomerNotesCommentDto, CustomerNotesCommentAddUpdateRequest>();
            CreateMap<CustomerNotesCommentAddUpdateRequest, CustomerNotesCommentDto>();

            CreateMap<CustomerNotesCommentDto, CustomerNotesCommentAddUpdateResponse>();
            CreateMap<CustomerNotesCommentAddUpdateResponse, CustomerNotesCommentDto>();
            //
            CreateMap<CustomerNotesCommentDto, CustomerNotesCommentdeleteRequest>();
            CreateMap<CustomerNotesCommentdeleteRequest, CustomerNotesCommentDto>();

            CreateMap<CustomerNotesCommentDto, CustomerNotesCommentdeleteResponse>();
            CreateMap<CustomerNotesCommentdeleteResponse, CustomerNotesCommentDto>();

            //
            CreateMap<CustomerAttachmentDto, CustomerAttachmentDeleteRequest>();
            CreateMap<CustomerAttachmentDeleteRequest, CustomerAttachmentDto>();

            CreateMap<CustomerAttachmentDto, CustomerAttachmentDeleteResponse>();
            CreateMap<CustomerAttachmentDeleteResponse, CustomerAttachmentDto>();
            //
            CreateMap<CustomerAttachmentDto, CustomerAttachmentUploadFilesRequest>();
            CreateMap<CustomerAttachmentUploadFilesRequest, CustomerAttachmentDto>();

            CreateMap<CustomerAttachment, CustomerAttachmentUploadFilesResponse>();
            CreateMap<CustomerAttachmentUploadFilesResponse, CustomerAttachment>();
            //
            CreateMap<CustomerAttachmentDto, CustomerAttachmentGetAllResposne>();
            CreateMap<CustomerAttachmentGetAllResposne, CustomerAttachmentDto>();
            //
            CreateMap<CustomerAttachmentDto, CustomerAttachDescriptionRequest>();
            CreateMap<CustomerAttachDescriptionRequest, CustomerAttachmentDto>();

            CreateMap<CustomerAttachmentDto, CustomerAttachDescriptionResponse>();
            CreateMap<CustomerAttachDescriptionResponse, CustomerAttachmentDto>();

            CreateMap<CustomerAttachDescriptionRequest, CustomerAttachDescriptionResponse>();
            CreateMap<CustomerAttachDescriptionResponse, CustomerAttachDescriptionRequest>();
            //
            CreateMap<ActivityTypeDto, CustomerActivityTypeResponse>();
            CreateMap<CustomerActivityTypeResponse, ActivityTypeDto>();
            //
            CreateMap<ActivityAvailabilityDto, CustomerActivityAvailResposne>();
            CreateMap<CustomerActivityAvailResposne, ActivityAvailabilityDto>();
            //
            CreateMap<CustomerActivityDto, CustomerActivityGetAllResponse>();
            CreateMap<CustomerActivityGetAllResponse, CustomerActivityDto>();
            //
            CreateMap<CustomerActivityDto, CustomerActivityAddUpdateRequest>();
            CreateMap<CustomerActivityAddUpdateRequest, CustomerActivityDto>();

            CreateMap<CustomerActivityDto, CustomerActivityAddUpdateResponse>();
            CreateMap<CustomerActivityAddUpdateResponse, CustomerActivityDto>();
            //
            CreateMap<CustomerActivityDto, CustomerActivityDeleteRequest>();
            CreateMap<CustomerActivityDeleteRequest, CustomerActivityDto>();

            CreateMap<CustomerActivityDto, CustomerActivityDeleteResponse>();
            CreateMap<CustomerActivityDeleteResponse, CustomerActivityDto>();
            //
            CreateMap<OrganizationNoteDto, OrganizationNoteAddUpdateRequest>();
            CreateMap<OrganizationNoteAddUpdateRequest, OrganizationNoteDto>();

            CreateMap<OrganizationNote, OrganizationNoteAddUpdateResponse>();
            CreateMap<OrganizationNoteAddUpdateResponse, OrganizationNote>();
            //
            CreateMap<OrganizationNoteDto, OrganizationNoteDeleteRequest>();
            CreateMap<OrganizationNoteDeleteRequest, OrganizationNoteDto>();

            CreateMap<OrganizationNote, OrganizationNoteDeleteResponse>();
            CreateMap<OrganizationNoteDeleteResponse, OrganizationNote>();
            //
            CreateMap<OrganizationNoteDto, OrganizationNoteGetAllRespnse>();
            CreateMap<OrganizationNoteGetAllRespnse, OrganizationNoteDto>();
            //
            CreateMap<OrganizationNotesCommentDto, OrganizationNotesCommentAddUpdateRequest>();
            CreateMap<OrganizationNotesCommentAddUpdateRequest, OrganizationNotesCommentDto>();

            CreateMap<OrganizationNotesCommentDto, OrganizationNotesCommentAddUpdateResponse>();
            CreateMap<OrganizationNotesCommentAddUpdateResponse, OrganizationNotesCommentDto>();
            //
            CreateMap<OrganizationNotesCommentDto, OrganizationNotesCommentDeleteRequest>();
            CreateMap<OrganizationNotesCommentDeleteRequest, OrganizationNotesCommentDto>();

            CreateMap<OrganizationNotesCommentDto, OrganizationNotesCommentDeleteResponse>();
            CreateMap<OrganizationNotesCommentDeleteResponse, OrganizationNotesCommentDto>();
            //
            CreateMap<OrganizationNotesCommentDto, OrganizationNotesCommentGetAllResponse>();
            CreateMap<OrganizationNotesCommentGetAllResponse, OrganizationNotesCommentDto>();
            //
            CreateMap<OrganizationActivityDto, OrganizationActivityGetAllResponse>();
            CreateMap<OrganizationActivityGetAllResponse, OrganizationActivityDto>();
            //
            CreateMap<OrganizationActivityDto, OrganizationActivityAddUpdateRequest>();
            CreateMap<OrganizationActivityAddUpdateRequest, OrganizationActivityDto>();

            CreateMap<OrganizationActivityDto, OrganizationActivityAddUpdateResponse>();
            CreateMap<OrganizationActivityAddUpdateResponse, OrganizationActivityDto>();
            //
            CreateMap<OrganizationActivityDto, OrganizationActivityDeleteRequest>();
            CreateMap<OrganizationActivityDeleteRequest, OrganizationActivityDto>();

            CreateMap<OrganizationActivityDto, OrganizationActivityDeleteResponse>();
            CreateMap<OrganizationActivityDeleteResponse, OrganizationActivityDto>();
            //
            CreateMap<ActivityTypeDto, OrganizationActivityTypeResponse>();
            CreateMap<OrganizationActivityTypeResponse, ActivityTypeDto>();
            //
            CreateMap<ActivityAvailabilityDto, OrganizationActivityAvailabilityResponse>();
            CreateMap<OrganizationActivityAvailabilityResponse, ActivityAvailabilityDto>();
            //
            CreateMap<OrganizationActivityMemberDto, OrganizationRemoveActivityMemberRequest>();
            CreateMap<OrganizationRemoveActivityMemberRequest, OrganizationActivityMemberDto>();

            CreateMap<OrganizationActivityMemberDto, OrganizationRemoveActivityMemberResponse>();
            CreateMap<OrganizationRemoveActivityMemberResponse, OrganizationActivityMemberDto>();
            //
            CreateMap<OrganizationAttachmentDto, OrganizationAttachUploadFilesRequest>();
            CreateMap<OrganizationAttachUploadFilesRequest, OrganizationAttachmentDto>();

            CreateMap<OrganizationAttachment, OrganizationAttachUploadFilesResponse>();
            CreateMap<OrganizationAttachUploadFilesResponse, OrganizationAttachment>();
            //
            CreateMap<OrganizationAttachmentDto, OrganizationAttachmentGetAllResponse>();
            CreateMap<OrganizationAttachmentGetAllResponse, OrganizationAttachmentDto>();
            //
            CreateMap<OrganizationAttachmentDto, OrganizationAttachmentDeleteRequest>();
            CreateMap<OrganizationAttachmentDeleteRequest, OrganizationAttachmentDto>();

            CreateMap<OrganizationAttachmentDto, OrganizationAttachmentDeleteResponse>();
            CreateMap<OrganizationAttachmentDeleteResponse, OrganizationAttachmentDto>();
            //
            CreateMap<OrganizationAttachmentDto, OrganizationAttachUpdateDescRequest>();
            CreateMap<OrganizationAttachUpdateDescRequest, OrganizationAttachmentDto>();

            CreateMap<OrganizationAttachmentDto, OrganizationAttachUpdateDescResponse>();
            CreateMap<OrganizationAttachUpdateDescResponse, OrganizationAttachmentDto>();
            //
            CreateMap<LeadNoteDto, LeadNoteAddUpdateRequest>();
            CreateMap<LeadNoteAddUpdateRequest, LeadNoteDto>();

            CreateMap<LeadNote, LeadNoteAddUpdateResponse>();
            CreateMap<LeadNoteAddUpdateResponse, LeadNote>();
            //
            CreateMap<LeadNoteDto, LeadNoteDeleteRequest>();
            CreateMap<LeadNoteDeleteRequest, LeadNoteDto>();

            CreateMap<LeadNote, LeadNoteDeleteResponse>();
            CreateMap<LeadNoteDeleteResponse, LeadNote>();
            //
            CreateMap<LeadNoteDto, LeadNoteGetAllResponse>();
            CreateMap<LeadNoteGetAllResponse, LeadNoteDto>();
            //
            CreateMap<LeadActivityDto, LeadActivityGetAllResponse>();
            CreateMap<LeadActivityGetAllResponse, LeadActivityDto>();
            //
            CreateMap<LeadActivityDto, LeadActivityAddUpdateRequest>();
            CreateMap<LeadActivityAddUpdateRequest, LeadActivityDto>();

            CreateMap<LeadActivityDto, LeadActivityAddUpdateResponse>();
            CreateMap<LeadActivityAddUpdateResponse, LeadActivityDto>();
            //
            CreateMap<LeadActivityDto, LeadActivityDeleteRequest>();
            CreateMap<LeadActivityDeleteRequest, LeadActivityDto>();

            CreateMap<LeadActivityDto, LeadActivityDeleteResponse>();
            CreateMap<LeadActivityDeleteResponse, LeadActivityDto>();
            //
            CreateMap<ActivityTypeDto, LeadActivityTypeResponse>();
            CreateMap<LeadActivityTypeResponse, ActivityTypeDto>();
            //
            CreateMap<ActivityAvailabilityDto, LeadActivityAvailabilityResponse>();
            CreateMap<LeadActivityAvailabilityResponse, ActivityAvailabilityDto>();
            //
            CreateMap<LeadActivityMemberDto, LeadActivityMemberRequest>();
            CreateMap<LeadActivityMemberRequest, LeadActivityMemberDto>();

            CreateMap<LeadActivityMemberDto, LeadActivityMemberResponse>();
            CreateMap<LeadActivityMemberResponse, LeadActivityMemberDto>();
            //
            CreateMap<CalendarListDto, CalendarListAddUpdateRequest>();
            CreateMap<CalendarListAddUpdateRequest, CalendarListDto>();

            CreateMap<CalendarListDto, CalendarListAddUpdateResponse>();
            CreateMap<CalendarListAddUpdateResponse, CalendarListDto>();
            //
            CreateMap<CalendarListDto, CalendarListDeleteRequest>();
            CreateMap<CalendarListDeleteRequest, CalendarListDto>();

            CreateMap<CalendarListDto, CalendarListDeleteResponse>();
            CreateMap<CalendarListDeleteResponse, CalendarListDto>();
            //
            CreateMap<CalendarListDto, CalendarListGetAllResposne>();
            CreateMap<CalendarListGetAllResposne, CalendarListDto>();
            //
            CreateMap<CalendarTaskDto, CalendarTaskAddUpdateRequest>();
            CreateMap<CalendarTaskAddUpdateRequest, CalendarTaskDto>();

            CreateMap<CalendarTaskDto, CalendarTaskAddUpdateResponse>();
            CreateMap<CalendarTaskAddUpdateResponse, CalendarTaskDto>();
            //
            CreateMap<CalendarTaskDto, CalendarTaskDeleteRequest>();
            CreateMap<CalendarTaskDeleteRequest, CalendarTaskDto>();

            CreateMap<CalendarTaskDto, CalendarTaskDeleteResponse>();
            CreateMap<CalendarTaskDeleteResponse, CalendarTaskDto>();
            //
            CreateMap<CalendarRepeatTypeDto, CalendarRepeatTypeResponse>();
            CreateMap<CalendarRepeatTypeResponse, CalendarRepeatTypeDto>();
            //
            CreateMap<CalendarSubTaskDto, CalendarSubTaskAddUpdateRequest>();
            CreateMap<CalendarSubTaskAddUpdateRequest, CalendarSubTaskDto>();

            CreateMap<CalendarSubTaskDto, CalendarSubTaskAddUpdateResponse>();
            CreateMap<CalendarSubTaskAddUpdateResponse, CalendarSubTaskDto>();
            //
            CreateMap<CalendarSubTaskDto, CalendarSubTaskDeleteRequest>();
            CreateMap<CalendarSubTaskDeleteRequest, CalendarSubTaskDto>();

            CreateMap<CalendarSubTaskDto, CalendarSubTaskDeleteResponse>();
            CreateMap<CalendarSubTaskDeleteResponse, CalendarSubTaskDto>();
            //
            CreateMap<LabelDto, LabelGetAllResponse>();
            CreateMap<LabelGetAllResponse, LabelDto>();
            //
            CreateMap<LabelDto, LabelAddUpdateRequest>();
            CreateMap<LabelAddUpdateRequest, LabelDto>();

            CreateMap<Label, LabelAddUpdateResponse>();
            CreateMap<LabelAddUpdateResponse, Label>();
            //
            CreateMap<LabelDto, LabelDeleteRequest>();
            CreateMap<LabelDeleteRequest, LabelDto>();

            CreateMap<Label, LabelDeleteResponse>();
            CreateMap<LabelDeleteResponse, Label>();
            //
            CreateMap<LabelCategoryDto, LabelCategoryResponse>();
            CreateMap<LabelCategoryResponse, LabelCategoryDto>();
            //
            CreateMap<LabelDto, LabelGetAllLabelResponse>();
            CreateMap<LabelGetAllLabelResponse, LabelDto>();

            CreateMap<LabelDto, LabelGetAllLabelRequest>();
            CreateMap<LabelGetAllLabelRequest, LabelDto>();
            //
            CreateMap<OneClappFormDto, OneClappFormGetAllRequest>();
            CreateMap<OneClappFormGetAllRequest, OneClappFormDto>();

            CreateMap<OneClappFormDto, OneClappFormGetAllResponse>();
            CreateMap<OneClappFormGetAllResponse, OneClappFormDto>();
            //
            CreateMap<OneClappFormDto, OneClappFormGetByKeyResponse>();
            CreateMap<OneClappFormGetByKeyResponse, OneClappFormDto>();
            //
            CreateMap<OneClappFormActionDto, OneClappFormActionResponse>();
            CreateMap<OneClappFormActionResponse, OneClappFormActionDto>();
            //
            CreateMap<OneClappFormDto, OneClappFormAddUpdateRequest>();
            CreateMap<OneClappFormAddUpdateRequest, OneClappFormDto>();

            CreateMap<OneClappFormDto, OneClappFormAddUpdateResponse>();
            CreateMap<OneClappFormAddUpdateResponse, OneClappFormDto>();
            //
            CreateMap<OneClappFormDto, OneClappFormUpdateStatusRequest>();
            CreateMap<OneClappFormUpdateStatusRequest, OneClappFormDto>();

            CreateMap<OneClappFormDto, OneClappFormUpdateStatusResponse>();
            CreateMap<OneClappFormUpdateStatusResponse, OneClappFormDto>();
            //
            CreateMap<OneClappFormDto, OneClappFormPositionRequest>();
            CreateMap<OneClappFormPositionRequest, OneClappFormDto>();

            CreateMap<OneClappFormDto, OneClappFormPositionResponse>();
            CreateMap<OneClappFormPositionResponse, OneClappFormDto>();
            //
            CreateMap<OneClappFormDto, OneClappFormRemoveRequest>();
            CreateMap<OneClappFormRemoveRequest, OneClappFormDto>();

            CreateMap<OneClappFormDto, OneClappFormRemoveResponse>();
            CreateMap<OneClappFormRemoveResponse, OneClappFormDto>();
            //
            CreateMap<OneClappLatestThemeConfigDto, OneClappLatestThemeConfigAddUpdateRequest>();
            CreateMap<OneClappLatestThemeConfigAddUpdateRequest, OneClappLatestThemeConfigDto>();

            CreateMap<OneClappLatestThemeConfigDto, OneClappLatestThemeConfigAddUpdateResponse>();
            CreateMap<OneClappLatestThemeConfigAddUpdateResponse, OneClappLatestThemeConfigDto>();
            //
            CreateMap<OneClappLatestThemeConfigDto, OneClappLatestThemeConfigGetByIdResponse>();
            CreateMap<OneClappLatestThemeConfigGetByIdResponse, OneClappLatestThemeConfigDto>();
            //
            CreateMap<OneClappLatestLayoutSettingDto, OneClappLatestLayoutSettingResponse>();
            CreateMap<OneClappLatestLayoutSettingResponse, OneClappLatestLayoutSettingDto>();
            //
            CreateMap<SubscriptionPlanDto, SubscriptionPlanGetAllResponse>();
            CreateMap<SubscriptionPlanGetAllResponse, SubscriptionPlanDto>();
            //
            CreateMap<UserSubscriptionDto, SubscriptionPlanGetByUserResponse>();
            CreateMap<SubscriptionPlanGetByUserResponse, UserSubscriptionDto>();
            //
            CreateMap<IntProviderAppSecretDto, GoogleCalendarGetAllResponse>();
            CreateMap<GoogleCalendarGetAllResponse, IntProviderAppSecretDto>();
            //
            CreateMap<IntProviderAppSecretDto, GoogleCalendarUpdateAccountRequest>();
            CreateMap<GoogleCalendarUpdateAccountRequest, IntProviderAppSecretDto>();

            CreateMap<IntProviderAppSecretDto, GoogleCalendarUpdateAccountResponse>();
            CreateMap<GoogleCalendarUpdateAccountResponse, IntProviderAppSecretDto>();
            //
            CreateMap<IntProviderAppSecretDto, GoogleCalendarDeleteRequest>();
            CreateMap<GoogleCalendarDeleteRequest, IntProviderAppSecretDto>();

            CreateMap<IntProviderAppSecretDto, GoogleCalendarDeleteResponse>();
            CreateMap<GoogleCalendarDeleteResponse, IntProviderAppSecretDto>();
            //
            CreateMap<SecretDto, GoogleCalendarSelectRequest>();
            CreateMap<GoogleCalendarSelectRequest, SecretDto>();

            CreateMap<SecretDto, GoogleCalendarSelectResponse>();
            CreateMap<GoogleCalendarSelectResponse, SecretDto>();
            //
            CreateMap<WeClappUserDto, WeClappUserResponse>();
            CreateMap<WeClappUserResponse, WeClappUserDto>();
            //
            CreateMap<WeClappUserDto, WeClappUserAddUpdateRequest>();
            CreateMap<WeClappUserAddUpdateRequest, WeClappUserDto>();

            CreateMap<WeClappUserDto, WeClappUserAddUpdateResponse>();
            CreateMap<WeClappUserAddUpdateResponse, WeClappUserDto>();
            //
            CreateMap<WeClappUserDto, WeClappUserDeleteRequest>();
            CreateMap<WeClappUserDeleteRequest, WeClappUserDto>();

            CreateMap<WeClappUserDto, WeClappUserDeleteResponse>();
            CreateMap<WeClappUserDeleteResponse, WeClappUserDto>();
            //
            CreateMap<AuthenticateModel, UserAuthenticateRequest>();
            CreateMap<UserAuthenticateRequest, AuthenticateModel>();

            CreateMap<UserDto, UserAuthenticateResponse>();
            CreateMap<UserAuthenticateResponse, UserDto>();
             CreateMap<User, UserAuthenticateResponse>();
            CreateMap<UserAuthenticateResponse, User>();
            //
            CreateMap<UserDto, UserDetailResponse>();
            CreateMap<UserDetailResponse, UserDto>();
            //
            CreateMap<UserDto, UserUpdateProfileRequest>();
            CreateMap<UserUpdateProfileRequest, UserDto>();

            CreateMap<User, UserUpdateProfileResponse>();
            CreateMap<UserUpdateProfileResponse, User>();

            CreateMap<UserDto, UserUpdateProfileResponse>();
            CreateMap<UserUpdateProfileResponse, UserDto>();
            //
            CreateMap<UserDto, UserRegisterRequest>();
            CreateMap<UserRegisterRequest, UserDto>();

            CreateMap<UserDto, UserRegisterResponse>();
            CreateMap<UserRegisterResponse, UserDto>();
            //
            CreateMap<InviteUserDto, UserInviteRequest>();
            CreateMap<UserInviteRequest, InviteUserDto>();

            CreateMap<UserDto, UserInviteResponse>();
            CreateMap<UserInviteResponse, UserDto>();
            //
            CreateMap<ExternalAuthDto, UserExternalLoginRequest>();
            CreateMap<UserExternalLoginRequest, ExternalAuthDto>();

            CreateMap<UserDto, UserExternalLoginResponse>();
            CreateMap<UserExternalLoginResponse, UserDto>();
            //
            CreateMap<ResetPasswordDto, UserResendOTPRequest>();
            CreateMap<UserResendOTPRequest, ResetPasswordDto>();

            CreateMap<ResetPasswordDto, UserResendOTPResponse>();
            CreateMap<UserResendOTPResponse, ResetPasswordDto>();
            //
            CreateMap<UserDto, UsergetUserEmailRequest>();
            CreateMap<UsergetUserEmailRequest, UserDto>();

            CreateMap<User, UsergetUserEmailResponse>();
            CreateMap<UsergetUserEmailResponse, User>();
            //
            CreateMap<UserDto, UserEmailVerificationRequest>();
            CreateMap<UserEmailVerificationRequest, UserDto>();

            CreateMap<UserDto, UserEmailVerificationResponse>();
            CreateMap<UserEmailVerificationResponse, UserDto>();
            //
            CreateMap<UserDto, UserChangePasswordRequest>();
            CreateMap<UserChangePasswordRequest, UserDto>();

            CreateMap<UserDto, UserChangePasswordResponse>();
            CreateMap<UserChangePasswordResponse, UserDto>();

            CreateMap<User, UserChangePasswordResponse>();
            CreateMap<UserChangePasswordResponse, User>();
            //
            CreateMap<UserDto, UserGetAllUsersByTenantResponse>();
            CreateMap<UserGetAllUsersByTenantResponse, UserDto>();
            //
            CreateMap<IntProviderAppSecretDto, CalendarGetEventsResponse>();
            CreateMap<CalendarGetEventsResponse, IntProviderAppSecretDto>();
            //
            CreateMap<GoogleMicrosoftCalendarEventVM, CalendarAddUpdateEventRequest>();
            CreateMap<CalendarAddUpdateEventRequest, GoogleMicrosoftCalendarEventVM>();

            CreateMap<GoogleMicrosoftCalendarEventVM, CalendarAddUpdateEventResponse>();
            CreateMap<CalendarAddUpdateEventResponse, GoogleMicrosoftCalendarEventVM>();
            //
            CreateMap<GoogleMicrosoftCalendarEventVM, CalendarDeleteEventRequest>();
            CreateMap<CalendarDeleteEventRequest, GoogleMicrosoftCalendarEventVM>();

            CreateMap<GoogleMicrosoftCalendarEventVM, CalendarDeleteEventResponse>();
            CreateMap<CalendarDeleteEventResponse, GoogleMicrosoftCalendarEventVM>();
            //            
            CreateMap<GoogleMicrosoftCalendarEventVM, CalendarSyncActivityRequest>();
            CreateMap<CalendarSyncActivityRequest, GoogleMicrosoftCalendarEventVM>();

            CreateMap<GoogleMicrosoftCalendarEventVM, CalendarSyncActivityResponse>();
            CreateMap<CalendarSyncActivityResponse, GoogleMicrosoftCalendarEventVM>();
            //
            CreateMap<UserVM, WeClappDropdownRequest>();
            CreateMap<WeClappDropdownRequest, UserVM>();

            CreateMap<DropDownVM, WeClappDropdownResponse>();
            CreateMap<WeClappDropdownResponse, DropDownVM>();
            //
            CreateMap<UserVM, weClappGetTicketsRequest>();
            CreateMap<weClappGetTicketsRequest, UserVM>();
            //
            CreateMap<Ticket, weClappGetTicketsResponse>();
            CreateMap<weClappGetTicketsResponse, Ticket>();
            //
            CreateMap<CustomControlDto, CustomModuleGetControlResponse>();
            CreateMap<CustomModuleGetControlResponse, CustomControlDto>();
            //
            CreateMap<CustomTableDto, CustomModuleGetEntityResponse>();
            CreateMap<CustomModuleGetEntityResponse, CustomTableDto>();
            //
            CreateMap<CustomFieldDto, CustomModuleSaveFieldRequest>();
            CreateMap<CustomModuleSaveFieldRequest, CustomFieldDto>();

            CreateMap<CustomFieldDto, CustomModuleSaveFieldResponse>();
            CreateMap<CustomModuleSaveFieldResponse, CustomFieldDto>();
            //
            CreateMap<CustomFieldDto, CustomModuleGetCustomFieldRequest>();
            CreateMap<CustomModuleGetCustomFieldRequest, CustomFieldDto>();

            CreateMap<CustomFieldDto, CustomModuleGetCustomFieldResponse>();
            CreateMap<CustomModuleGetCustomFieldResponse, CustomFieldDto>();
            //
            CreateMap<CustomFieldDto, CustomModuleGetByIdResponse>();
            CreateMap<CustomModuleGetByIdResponse, CustomFieldDto>();
            //
            CreateMap<CustomFieldDto, CustomModuleDeleteFieldRequest>();
            CreateMap<CustomModuleDeleteFieldRequest, CustomFieldDto>();

            CreateMap<CustomFieldDto, CustomModuleDeleteFieldResponse>();
            CreateMap<CustomModuleDeleteFieldResponse, CustomFieldDto>();
            //
            CreateMap<CustomTableColumnDto, CustomModuleGetAllColumnRequest>();
            CreateMap<CustomModuleGetAllColumnRequest, CustomTableColumnDto>();

            CreateMap<CustomTableDto, CustomModuleGetAllColumnResponse>();
            CreateMap<CustomModuleGetAllColumnResponse, CustomTableDto>();
            //
            CreateMap<CustomTableColumnDto, CustomModuleDefaultColumnRequest>();
            CreateMap<CustomModuleDefaultColumnRequest, CustomTableColumnDto>();

            CreateMap<CustomTableDto, CustomModuleDefaultColumnResponse>();
            CreateMap<CustomModuleDefaultColumnResponse, CustomTableDto>();
            //
            CreateMap<ModuleColumnDto, CustomModuleDefaultColumnRequest>();
            CreateMap<CustomModuleDefaultColumnRequest, ModuleColumnDto>();

            CreateMap<ModuleColumnDto, CustomModuleDefaultColumnResponse>();
            CreateMap<CustomModuleDefaultColumnResponse, ModuleColumnDto>();
            //
            CreateMap<CustomFieldDto, CustomModuleIsExistRequest>();
            CreateMap<CustomModuleIsExistRequest, CustomFieldDto>();

            CreateMap<CustomFieldDto, CustomModuleIsExistResponse>();
            CreateMap<CustomModuleIsExistResponse, CustomFieldDto>();
            //
            CreateMap<CustomModuleDto, CustomModuleGetModuleFieldResponse>();
            CreateMap<CustomModuleGetModuleFieldResponse, CustomModuleDto>();
            //
            CreateMap<ModuleColumnDto, CustomModuleSaveColumnRequest>();
            CreateMap<CustomModuleSaveColumnRequest, ModuleColumnDto>();

            CreateMap<ModuleColumnDto, CustomModuleSaveColumnResponse>();
            CreateMap<CustomModuleSaveColumnResponse, ModuleColumnDto>();
            //
            CreateMap<DiscussionReadDto, DiscussionReadMarkAsReadRequest>();
            CreateMap<DiscussionReadMarkAsReadRequest, DiscussionReadDto>();

            CreateMap<DiscussionReadDto, DiscussionReadMarkAsReadResponse>();
            CreateMap<DiscussionReadMarkAsReadResponse, DiscussionReadDto>();
            //
            CreateMap<DiscussionReadDto, DiscussionReadMarkAsUnReadRequest>();
            CreateMap<DiscussionReadMarkAsUnReadRequest, DiscussionReadDto>();

            CreateMap<DiscussionReadDto, DiscussionReadMarkAsUnReadResponse>();
            CreateMap<DiscussionReadMarkAsUnReadResponse, DiscussionReadDto>();
            //
            CreateMap<OneClappRequestFormDto, OneClappRequestFormAddUpdateRequest>();
            CreateMap<OneClappRequestFormAddUpdateRequest, OneClappRequestFormDto>();

            CreateMap<OneClappRequestFormDto, OneClappRequestFormAddUpdateResponse>();
            CreateMap<OneClappRequestFormAddUpdateResponse, OneClappRequestFormDto>();
            //
            CreateMap<VerifyFormRequestDto, OneClappRequestFormRemoveRequest>();
            CreateMap<OneClappRequestFormRemoveRequest, VerifyFormRequestDto>();

            CreateMap<OneClappRequestFormDto, OneClappRequestFormRemoveResponse>();
            CreateMap<OneClappRequestFormRemoveResponse, OneClappRequestFormDto>();
            //
            CreateMap<OneClappRequestFormDto, OneClappRequestFormVerifyRequest>();
            CreateMap<OneClappRequestFormVerifyRequest, OneClappRequestFormDto>();

            CreateMap<OneClappRequestFormDto, OneClappRequestFormVerifyResponse>();
            CreateMap<OneClappRequestFormVerifyResponse, OneClappRequestFormDto>();
            //
            CreateMap<BorderStyleDto, BorderStyleGetAllResponse>();
            CreateMap<BorderStyleGetAllResponse, BorderStyleDto>();
            //
            CreateMap<ERPSystemColumnMapDto, ERPSystemColumnMapAddUpdateRequest>();
            CreateMap<ERPSystemColumnMapAddUpdateRequest, ERPSystemColumnMapDto>();

            CreateMap<ERPSystemColumnMapDto, ERPSystemColumnMapAddUpdateResponse>();
            CreateMap<ERPSystemColumnMapAddUpdateResponse, ERPSystemColumnMapDto>();
            //
            CreateMap<ERPSystemColumnMapDto, ERPSystemColumnMapGetAllRequest>();
            CreateMap<ERPSystemColumnMapGetAllRequest, ERPSystemColumnMapDto>();

            CreateMap<ERPSystemColumnMapDto, ERPSystemColumnMapGetAllResponse>();
            CreateMap<ERPSystemColumnMapGetAllResponse, ERPSystemColumnMapDto>();
            //
            CreateMap<ERPSystemColumnMapDto, ERPSystemColumnMapDeleteResponse>();
            CreateMap<ERPSystemColumnMapDeleteResponse, ERPSystemColumnMapDto>();
            //
            CreateMap<ImportContactAttachmentDto, ImportContactAttachmentRequest>();
            CreateMap<ImportContactAttachmentRequest, ImportContactAttachmentDto>();

            CreateMap<ImportContactAttachmentDto, ImportContactAttachmentResponse>();
            CreateMap<ImportContactAttachmentResponse, ImportContactAttachmentDto>();
            //
            CreateMap<OneClappFormLayoutBackgroundDto, LayoutBackgroundAddUpdateRequest>();
            CreateMap<LayoutBackgroundAddUpdateRequest, OneClappFormLayoutBackgroundDto>();

            CreateMap<OneClappFormLayoutBackgroundDto, LayoutBackgroundAddUpdateResponse>();
            CreateMap<LayoutBackgroundAddUpdateResponse, OneClappFormLayoutBackgroundDto>();
            //
            CreateMap<OneClappFormHeaderDto, FormHeaderAddUpdateResquest>();
            CreateMap<FormHeaderAddUpdateResquest, OneClappFormHeaderDto>();

            CreateMap<OneClappFormHeaderDto, FormHeaderAddUpdateResponse>();
            CreateMap<FormHeaderAddUpdateResponse, OneClappFormHeaderDto>();

            CreateMap<EmployeeProjectDto, AddUpdateEmployeeProjectRequest>();
            CreateMap<AddUpdateEmployeeProjectRequest, EmployeeProjectDto>();

            CreateMap<EmployeeProjectDto, EmployeeProjectListRequest>();
            CreateMap<EmployeeProjectListRequest, EmployeeProjectDto>();

            CreateMap<EmployeeProject, EmployeeProjectListResponse>();
            CreateMap<EmployeeProjectListResponse, EmployeeProject>();

            CreateMap<EmployeeProjectListRequest, EmployeeProjectListResponse>();
            CreateMap<EmployeeProjectListResponse, EmployeeProjectListRequest>();







            //end NEW

            // Employee task tables start

            CreateMap<EmployeeTaskStatus, EmployeeTaskStatusDto>();
            CreateMap<EmployeeTaskStatusDto, EmployeeTaskStatus>();

            CreateMap<EmployeeProject, EmployeeProjectDto>();
            CreateMap<EmployeeProjectDto, EmployeeProject>();

            CreateMap<EmployeeProjectStatus, EmployeeProjectStatusDto>();
            CreateMap<EmployeeProjectStatusDto, EmployeeProjectStatus>();

            CreateMap<EmployeeTask, EmployeeTaskDto>();
            CreateMap<EmployeeTaskDto, EmployeeTask>();

            CreateMap<EmployeeTaskAttachment, EmployeeTaskAttachmentDto>();
            CreateMap<EmployeeTaskAttachmentDto, EmployeeTaskAttachment>();

            CreateMap<EmployeeTaskComment, EmployeeTaskCommentDto>();
            CreateMap<EmployeeTaskCommentDto, EmployeeTaskComment>();

            CreateMap<EmployeeTaskUser, EmployeeTaskUserDto>();
            CreateMap<EmployeeTaskUserDto, EmployeeTaskUser>();

            CreateMap<EmployeeTaskTimeRecord, EmployeeTaskTimeRecordDto>();
            CreateMap<EmployeeTaskTimeRecordDto, EmployeeTaskTimeRecord>();

            CreateMap<EmployeeTaskActivity, EmployeeTaskActivityDto>();
            CreateMap<EmployeeTaskActivityDto, EmployeeTaskActivity>();

            CreateMap<EmployeeTask, EmployeeTaskVM>();
            CreateMap<EmployeeTaskVM, EmployeeTask>();

            CreateMap<EmployeeProject, ProjectVM>();
            CreateMap<ProjectVM, EmployeeProject>();

            CreateMap<EmployeeSubTask, EmployeeSubTaskDto>();
            CreateMap<EmployeeSubTaskDto, EmployeeSubTask>();

            CreateMap<EmployeeSubTaskAttachment, EmployeeSubTaskAttachmentDto>();
            CreateMap<EmployeeSubTaskAttachmentDto, EmployeeSubTaskAttachment>();

            CreateMap<EmployeeSubTaskComment, EmployeeSubTaskCommentDto>();
            CreateMap<EmployeeSubTaskCommentDto, EmployeeSubTaskComment>();

            CreateMap<EmployeeSubTask, EmployeeSubTaskDto>();
            CreateMap<EmployeeSubTaskDto, EmployeeSubTask>();

            CreateMap<EmployeeSubTaskUser, EmployeeSubTaskUserDto>();
            CreateMap<EmployeeSubTaskUserDto, EmployeeSubTaskUser>();

            CreateMap<EmployeeSubTaskTimeRecord, EmployeeSubTaskTimeRecordDto>();
            CreateMap<EmployeeSubTaskTimeRecordDto, EmployeeSubTaskTimeRecord>();

            CreateMap<EmployeeSubTaskActivity, EmployeeSubTaskActivityDto>();
            CreateMap<EmployeeSubTaskActivityDto, EmployeeSubTaskActivity>();

            CreateMap<EmployeeChildTask, EmployeeChildTaskDto>();
            CreateMap<EmployeeChildTaskDto, EmployeeChildTask>();

            CreateMap<EmployeeChildTaskAttachment, EmployeeChildTaskAttachmentDto>();
            CreateMap<EmployeeChildTaskAttachmentDto, EmployeeChildTaskAttachment>();

            CreateMap<EmployeeChildTaskComment, EmployeeChildTaskCommentDto>();
            CreateMap<EmployeeChildTaskCommentDto, EmployeeChildTaskComment>();

            CreateMap<EmployeeChildTaskUser, EmployeeChildTaskUserDto>();
            CreateMap<EmployeeChildTaskUserDto, EmployeeChildTaskUser>();

            CreateMap<EmployeeChildTaskTimeRecord, EmployeeChildTaskTimeRecordDto>();
            CreateMap<EmployeeChildTaskTimeRecordDto, EmployeeChildTaskTimeRecord>();

            CreateMap<EmployeeChildTaskActivity, EmployeeChildTaskActivityDto>();
            CreateMap<EmployeeChildTaskActivityDto, EmployeeChildTaskActivity>();

            CreateMap<EmployeeChildTask, EmployeeChildTaskVM>();
            CreateMap<EmployeeChildTaskVM, EmployeeChildTask>();

            CreateMap<EmployeeSubTask, EmployeeSubTaskVM>();
            CreateMap<EmployeeSubTaskVM, EmployeeSubTask>();

            CreateMap<EmployeeSectionVM, Section>();
            CreateMap<Section, EmployeeSectionVM>();

            CreateMap<AddUpdateEmployeeTaskRequest, EmployeeTaskDto>();
            CreateMap<EmployeeTaskDto, AddUpdateEmployeeTaskRequest>();

            CreateMap<EmployeeTaskAddUpdateResponse, EmployeeTaskDto>();
            CreateMap<EmployeeTaskDto, EmployeeTaskAddUpdateResponse>();

            CreateMap<EmployeeTaskTimeRecordResponse, EmployeeTaskTimeRecord>();
            CreateMap<EmployeeTaskTimeRecord, EmployeeTaskTimeRecordResponse>();

            CreateMap<EmployeeTaskUser, EmployeeTaskUserRequestResponse>();
            CreateMap<EmployeeTaskUserRequestResponse, EmployeeTaskUser>();

            CreateMap<AddUpdateEmployeeTaskTimeRecordRequest, EmployeeTaskTimeRecordDto>();
            CreateMap<EmployeeTaskTimeRecordDto, AddUpdateEmployeeTaskTimeRecordRequest>();

            CreateMap<AddUpdateTaskCommentRequest, TaskCommentDto>();
            CreateMap<TaskCommentDto, AddUpdateTaskCommentRequest>();

            CreateMap<AddUpdateTaskStatusRequest, TaskStatusDto>();
            CreateMap<TaskStatusDto, AddUpdateTaskStatusRequest>();

            CreateMap<AddUpdateEmployeeSubTaskRequest, EmployeeSubTaskDto>();
            CreateMap<EmployeeSubTaskDto, AddUpdateEmployeeSubTaskRequest>();

            CreateMap<EmployeeSubTaskAddUpdateResponse, EmployeeSubTaskDto>();
            CreateMap<EmployeeSubTaskDto, EmployeeSubTaskAddUpdateResponse>();

            CreateMap<EmployeeSubTaskUser, EmployeeSubTaskUserRequestResponse>();
            CreateMap<EmployeeSubTaskUserRequestResponse, EmployeeSubTaskUser>();

            CreateMap<AddUpdateEmployeeSubTaskTimeRecordRequest, EmployeeSubTaskTimeRecordDto>();
            CreateMap<EmployeeSubTaskTimeRecordDto, AddUpdateEmployeeSubTaskTimeRecordRequest>();

            CreateMap<EmployeeSubTaskTimeRecord, EmployeeSubTaskTimeRecordResponse>();
            CreateMap<EmployeeSubTaskTimeRecordResponse, EmployeeSubTaskTimeRecord>();

            CreateMap<EmployeeSubTaskAttachmentRequest, EmployeeSubTaskAttachmentDto>();
            CreateMap<EmployeeSubTaskAttachmentDto, EmployeeSubTaskAttachmentRequest>();

            CreateMap<RemoveEmployeeSubTaskResponse, EmployeeSubTaskDto>();
            CreateMap<EmployeeSubTaskDto, RemoveEmployeeSubTaskResponse>();

            CreateMap<EmployeeSubTaskHistoryResponse, EmployeeSubTaskActivityDto>();
            CreateMap<EmployeeSubTaskActivityDto, EmployeeSubTaskHistoryResponse>();

            CreateMap<RemoveEmployeeSubTaskAttachmentResponse, EmployeeSubTaskAttachmentDto>();
            CreateMap<EmployeeSubTaskAttachmentDto, RemoveEmployeeSubTaskAttachmentResponse>();

            CreateMap<EmployeeChildTaskHistoryResponse, EmployeeChildTaskActivityDto>();
            CreateMap<EmployeeChildTaskActivityDto, EmployeeChildTaskHistoryResponse>();

            CreateMap<EmployeeChildTaskAttachmentListResponse, EmployeeChildTaskAttachment>();
            CreateMap<EmployeeChildTaskAttachment, EmployeeChildTaskAttachmentListResponse>();

            CreateMap<AddUpdateEmployeeChildTaskTimeRecordRequest, EmployeeChildTaskTimeRecordDto>();
            CreateMap<EmployeeChildTaskTimeRecordDto, AddUpdateEmployeeChildTaskTimeRecordRequest>();

            CreateMap<AddUpdateEmployeeChildTaskTimeRecordResponse, EmployeeChildTaskTimeRecord>();
            CreateMap<EmployeeChildTaskTimeRecord, AddUpdateEmployeeChildTaskTimeRecordResponse>();

            CreateMap<AddUpdateEmployeeChildTaskRequest, EmployeeChildTaskDto>();
            CreateMap<EmployeeChildTaskDto, AddUpdateEmployeeChildTaskRequest>();

            CreateMap<EmployeeChildTaskUser, EmployeeChildTaskUserRequestResponse>();
            CreateMap<EmployeeChildTaskUserRequestResponse, EmployeeChildTaskUser>();

            CreateMap<EmployeeChildTaskAddUpdateResponse, EmployeeChildTaskDto>();
            CreateMap<EmployeeChildTaskDto, EmployeeChildTaskAddUpdateResponse>();

            CreateMap<AddUpdateEmployeeChildTaskCommentResponse, EmployeeChildTaskComment>();
            CreateMap<EmployeeChildTaskComment, AddUpdateEmployeeChildTaskCommentResponse>();

            CreateMap<AddUpdateEmployeeChildTaskCommentRequest, EmployeeChildTaskCommentDto>();
            CreateMap<EmployeeChildTaskCommentDto, AddUpdateEmployeeChildTaskCommentRequest>();

            CreateMap<EmployeeChildTaskCommentListResponse, EmployeeChildTaskComment>();
            CreateMap<EmployeeChildTaskComment, EmployeeChildTaskCommentListResponse>();

            CreateMap<AddUpdateEmployeeSubTaskCommentRequest, EmployeeSubTaskCommentDto>();
            CreateMap<EmployeeSubTaskCommentDto, AddUpdateEmployeeSubTaskCommentRequest>();

            CreateMap<AddUpdateEmployeeSubTaskCommentResponse, EmployeeSubTaskComment>();
            CreateMap<EmployeeSubTaskComment, AddUpdateEmployeeSubTaskCommentResponse>();

            CreateMap<AddUpdateEmployeeTaskCommentRequest, EmployeeTaskCommentDto>();
            CreateMap<EmployeeTaskCommentDto, AddUpdateEmployeeTaskCommentRequest>();

            CreateMap<AddUpdateEmployeeTaskCommentResponse, EmployeeTaskComment>();
            CreateMap<EmployeeTaskComment, AddUpdateEmployeeTaskCommentResponse>();

            CreateMap<BorderStyleAddUpdateRequest, BorderStyleDto>();
            CreateMap<BorderStyleDto, BorderStyleAddUpdateRequest>();

            CreateMap<Status, StatusDto>();
            CreateMap<StatusDto, Status>();

            CreateMap<StatusListResponse, StatusDto>();
            CreateMap<StatusDto, StatusListResponse>();

            CreateMap<StatusDetailResponse, Status>();
            CreateMap<Status, StatusDetailResponse>();

            CreateMap<StatusAddRequest, StatusDto>();
            CreateMap<StatusDto, StatusAddRequest>();

            CreateMap<StatusAddResponse, Status>();
            CreateMap<Status, StatusAddResponse>();

            CreateMap<StatusUpdateResponse, Status>();
            CreateMap<Status, StatusUpdateResponse>();

            CreateMap<StatusDeleteResponse, Status>();
            CreateMap<Status, StatusDeleteResponse>();

            CreateMap<AddUpdateEmployeeProjectResponse, EmployeeProject>();
            CreateMap<EmployeeProject, AddUpdateEmployeeProjectResponse>();

            CreateMap<AddUpdateEmployeeProjectRequest, EmployeeProjectDto>();
            CreateMap<EmployeeProjectDto, AddUpdateEmployeeProjectRequest>();

            CreateMap<AddUpdateEmployeeProjectResponse, EmployeeProjectDto>();
            CreateMap<EmployeeProjectDto, AddUpdateEmployeeProjectResponse>();

            CreateMap<EmployeeProjectDeleteResponse, EmployeeProjectDto>();
            CreateMap<EmployeeProjectDto, EmployeeProjectDeleteResponse>();

            CreateMap<EmployeeProjectUser, EmployeeProjectUserDto>();
            CreateMap<EmployeeProjectUserDto, EmployeeProjectUser>();

            CreateMap<EmployeeProjectActivityDto, EmployeeProjectActivity>();
            CreateMap<EmployeeProjectActivity, EmployeeProjectActivityDto>();

            CreateMap<EmployeeTask, EmployeeTaskListResponse>();
            CreateMap<EmployeeTaskListResponse, EmployeeTask>();

            CreateMap<OneClappFormAddRequest, OneClappFormDto>();
            CreateMap<OneClappFormDto, OneClappFormAddRequest>();

            CreateMap<OneClappFormAddResponse, OneClappFormDto>();
            CreateMap<OneClappFormDto, OneClappFormAddResponse>();

            CreateMap<Status, StatusListByTableResponse>();
            CreateMap<StatusListByTableResponse, Status>();

            CreateMap<EmployeeTaskDto, EmployeeTaskDeleteResponse>();
            CreateMap<EmployeeTaskDeleteResponse, EmployeeTaskDto>();

            CreateMap<CustomerNoteDto, CustomerNoteListResponse>();
            CreateMap<CustomerNoteListResponse, CustomerNoteDto>();

            CreateMap<OrganizationNotesCommentListResponse, OrganizationNotesCommentDto>();
            CreateMap<OrganizationNotesCommentDto, OrganizationNotesCommentListResponse>();

            CreateMap<CustomerNotesCommentListResponse, CustomerNotesCommentDto>();
            CreateMap<CustomerNotesCommentDto, CustomerNotesCommentListResponse>();
            CreateMap<EmployeeProjectUser, EmployeeProjectUserRequestResponse>();
            CreateMap<EmployeeProjectUserRequestResponse, EmployeeProjectUser>();

            CreateMap<ProjectDetailResponse, ProjectVM>();
            CreateMap<ProjectVM, ProjectDetailResponse>();

            CreateMap<AssignUserEmployeeProjectResponse, EmployeeProjectDto>();
            CreateMap<EmployeeProjectDto, AssignUserEmployeeProjectResponse>();

            CreateMap<EmployeeProjectUser, EmployeeProjectUserRequestResponse>();
            CreateMap<EmployeeProjectUserRequestResponse, EmployeeProjectUser>();

            CreateMap<AssignUserEmployeeProjectRequest, AssignUserEmployeeProjectResponse>();
            CreateMap<AssignUserEmployeeProjectResponse, AssignUserEmployeeProjectRequest>();

            CreateMap<EmployeeProjectTaskListResponse, EmployeeTask>();
            CreateMap<EmployeeTask, EmployeeProjectTaskListResponse>();

            CreateMap<DiscussionTeamMateListResponse, UserDto>();
            CreateMap<UserDto, DiscussionTeamMateListResponse>();

            CreateMap<ProjectDetailResponse, EmployeeProject>();
            CreateMap<EmployeeProject, ProjectDetailResponse>();

            CreateMap<CustomEmailFolderResponse, CustomEmailFolder>();
            CreateMap<CustomEmailFolder, CustomEmailFolderResponse>();

            CreateMap<ServiceArticleCategory, ServiceArticleCategoryAddRequest>();
            CreateMap<ServiceArticleCategoryAddRequest, ServiceArticleCategory>();

            CreateMap<ServiceArticleCategory, ServiceArticleCategoryAddResponse>();
            CreateMap<ServiceArticleCategoryAddResponse, ServiceArticleCategory>();

            CreateMap<StatusUpdateRequest, StatusDto>();
            CreateMap<StatusDto, StatusUpdateRequest>();

            CreateMap<ServiceArticleCategory, ServiceArticleCategoryListResponse>();
            CreateMap<ServiceArticleCategoryListResponse, ServiceArticleCategory>();

            CreateMap<ServiceArticleCategory, ServiceArticleCategoryDetailResponse>();
            CreateMap<ServiceArticleCategoryDetailResponse, ServiceArticleCategory>();

            CreateMap<Currency, CurrencyAddRequest>();
            CreateMap<CurrencyAddRequest, Currency>();

            CreateMap<Currency, CurrencyAddResponse>();
            CreateMap<CurrencyAddResponse, Currency>();

            CreateMap<Currency, CurrencyListResponse>();
            CreateMap<CurrencyListResponse, Currency>();

            CreateMap<Currency, CurrencyDetailResponse>();
            CreateMap<CurrencyDetailResponse, Currency>();

            CreateMap<Tax, TaxListResponse>();
            CreateMap<TaxListResponse, Tax>();

            CreateMap<Tax, TaxAddRequest>();
            CreateMap<TaxAddRequest, Tax>();

            CreateMap<Tax, TaxAddResponse>();
            CreateMap<TaxAddResponse, Tax>();

            CreateMap<TaxRate, TaxRateAddRequest>();
            CreateMap<TaxRateAddRequest, TaxRate>();

            CreateMap<Tax, TaxDetailResponse>();
            CreateMap<TaxDetailResponse, Tax>();

            CreateMap<TaxRate, TaxRateDetailResponse>();
            CreateMap<TaxRateDetailResponse, TaxRate>();

            CreateMap<Tax, TaxDropdownListResponse>();
            CreateMap<TaxDropdownListResponse, Tax>();

            CreateMap<ServiceArticle, ServiceArticleAddRequest>();
            CreateMap<ServiceArticleAddRequest, ServiceArticle>();

            CreateMap<ServiceArticle, ServiceArticleAddResponse>();
            CreateMap<ServiceArticleAddResponse, ServiceArticle>();

            CreateMap<ServiceArticle, ServiceArticleListResponse>();
            CreateMap<ServiceArticleListResponse, ServiceArticle>();

            CreateMap<ServiceArticle, ServiceArticleDetailResponse>();
            CreateMap<ServiceArticleDetailResponse, ServiceArticle>();

            CreateMap<Currency, CurrencyDropDownListResponse>();
            CreateMap<CurrencyDropDownListResponse, Currency>();

            CreateMap<TaxRate, TaxRateAddResponse>();
            CreateMap<TaxRateAddResponse, TaxRate>();

            CreateMap<TaxRate, TaxRateListResponse>();
            CreateMap<TaxRateListResponse, TaxRate>();

            CreateMap<CustomTableColumnResponse, CustomTableColumnDto>();
            CreateMap<CustomTableColumnDto, CustomTableColumnResponse>();

            CreateMap<EmployeeTask, CustomTaskVM>();
            CreateMap<CustomTaskVM, EmployeeTask>();

            CreateMap<EmployeeSubTask, CustomSubTaskVM>();
            CreateMap<CustomSubTaskVM, EmployeeSubTask>();

            CreateMap<EmployeeChildTask, CustomChildTaskVM>();
            CreateMap<CustomChildTaskVM, EmployeeChildTask>();

            CreateMap<UserDto, UserDetailResponse>();
            CreateMap<UserDetailResponse, UserDto>();

            CreateMap<User, UserDetailResponse>();
            CreateMap<UserDetailResponse, User>();

            CreateMap<User, CustomTaskUserVM>();
            CreateMap<CustomTaskUserVM, User>();

            CreateMap<EmployeeTaskUser, CustomTaskUserVM>();
            CreateMap<CustomTaskUserVM, EmployeeTaskUser>();

            CreateMap<AddUpdateCustomTaskRequestResponse, EmployeeChildTaskDto>();
            CreateMap<EmployeeChildTaskDto, AddUpdateCustomTaskRequestResponse>();

            CreateMap<AddUpdateCustomTaskRequestResponse, EmployeeSubTaskDto>();
            CreateMap<EmployeeSubTaskDto, AddUpdateCustomTaskRequestResponse>();

            CreateMap<AddUpdateCustomTaskRequestResponse, EmployeeTaskDto>();
            CreateMap<EmployeeTaskDto, AddUpdateCustomTaskRequestResponse>();

            CreateMap<Country, CountryDropDownListResponse>();
            CreateMap<CountryDropDownListResponse, Country>();

            CreateMap<State, StateDropDownListResponse>();
            CreateMap<StateDropDownListResponse, State>();

            CreateMap<City, CityDropDownListResponse>();
            CreateMap<CityDropDownListResponse, City>();

            CreateMap<StandardTimeZone, StandardTimeZoneDropDownListResponse>();
            CreateMap<StandardTimeZoneDropDownListResponse, StandardTimeZone>();

            CreateMap<Client, ClientAddUpdateRequest>();
            CreateMap<ClientAddUpdateRequest, Client>();

            CreateMap<Client, ClientAddUpdateResponse>();
            CreateMap<ClientAddUpdateResponse, Client>();

            CreateMap<ClientEmail, ClientEmailAddUpdateRequest>();
            CreateMap<ClientEmailAddUpdateRequest, ClientEmail>();

            CreateMap<ClientPhone, ClientPhoneAddUpdateRequest>();
            CreateMap<ClientPhoneAddUpdateRequest, ClientPhone>();

            CreateMap<Client, ClientListResponse>();
            CreateMap<ClientListResponse, Client>();

            CreateMap<Client, ClientInfoResponse>();
            CreateMap<ClientInfoResponse, Client>();

            CreateMap<ContractType, ContractTypeRequestResponse>();
            CreateMap<ContractTypeRequestResponse, ContractType>();

            CreateMap<Contract, AddUpdateContractRequest>();
            CreateMap<AddUpdateContractRequest, Contract>();

            CreateMap<Contract, ContractListResponse>();
            CreateMap<ContractListResponse, Contract>();

            CreateMap<ContractServiceArticleResponse, ServiceArticle>();
            CreateMap<ServiceArticle, ContractServiceArticleResponse>();

            CreateMap<AddUpdateEmployeeTaskTimeTrackRequest, EmployeeTaskTimeRecordDto>();
            CreateMap<EmployeeTaskTimeRecordDto, AddUpdateEmployeeTaskTimeTrackRequest>();

            CreateMap<AddUpdateTaskTimeTrackeResponse, EmployeeTaskTimeRecord>();
            CreateMap<EmployeeTaskTimeRecord, AddUpdateTaskTimeTrackeResponse>();

            CreateMap<AddUpdateInvoiceIntervalRequestResponse, InvoiceInterval>();
            CreateMap<InvoiceInterval, AddUpdateInvoiceIntervalRequestResponse>();

            CreateMap<MateTimeRecord, MateTimeRecordAddUpdateRequest>();
            CreateMap<MateTimeRecordAddUpdateRequest, MateTimeRecord>();

            CreateMap<MateTimeRecord, MateTimeRecordAddUpdateResponse>();
            CreateMap<MateTimeRecordAddUpdateResponse, MateTimeRecord>();

            CreateMap<MateTimeRecord, MateTimeRecordListResponse>();
            CreateMap<MateTimeRecordListResponse, MateTimeRecord>();

            CreateMap<MateTimeRecord, MateTimeRecordDetailResponse>();
            CreateMap<MateTimeRecordDetailResponse, MateTimeRecord>();

            CreateMap<MateTaskTimeRecord, TimeRecordInvoiceResponse>();
            CreateMap<TimeRecordInvoiceResponse, MateTaskTimeRecord>();

            CreateMap<ClientTask, EmployeeTask>();
            CreateMap<EmployeeTask, ClientTask>();

            CreateMap<Contract, ClientContract>();
            CreateMap<ClientContract, Contract>();

            CreateMap<MateTaskTimeRecord, EmployeeTaskTimeRecordDetailResponse>();
            CreateMap<EmployeeTaskTimeRecordDetailResponse, MateTaskTimeRecord>();

            CreateMap<EmployeeProjectDto, EmployeeProjectAssignClientRequest>();
            CreateMap<EmployeeProjectAssignClientRequest, EmployeeProjectDto>();

            CreateMap<EmployeeProject, EmployeeProjectAssignClientResponse>();
            CreateMap<EmployeeProjectAssignClientResponse, EmployeeProject>();

            CreateMap<EmployeeTask, EmployeeTaskAssignClientResponse>();
            CreateMap<EmployeeTaskAssignClientResponse, EmployeeTask>();

            CreateMap<ContractActivity, ContractActivityHistoryResponse>();
            CreateMap<ContractActivityHistoryResponse, ContractActivity>();

            CreateMap<EmployeeProject, ClientProject>();
            CreateMap<ClientProject, EmployeeProject>();

            // CreateMap<EmployeeProject, MateProjectTimeRecordListResponse>();
            // CreateMap<MateProjectTimeRecordListResponse, EmployeeProject>();

            CreateMap<MateProjectTimeRecord, MateTimeRecordDetailResponse>();
            CreateMap<MateTimeRecordDetailResponse, MateProjectTimeRecord>();

            CreateMap<ProjectContract, ProjectContractAddUpdateRequest>();
            CreateMap<ProjectContractAddUpdateRequest, ProjectContract>();

            CreateMap<ProjectContract, ProjectContractAddUpdateResponse>();
            CreateMap<ProjectContractAddUpdateResponse, ProjectContract>();

            CreateMap<ContractArticle, ServiceArticleIdsAddContractRequest>();
            CreateMap<ServiceArticleIdsAddContractRequest, ContractArticle>();

            //invoice
            CreateMap<Client, CustomTaskClient>();
            CreateMap<CustomTaskClient, Client>();

            CreateMap<CustomTaskContract, Contract>();
            CreateMap<Contract, CustomTaskContract>();
            //invoice
            CreateMap<ClientVM, Client>();
            CreateMap<Client, ClientVM>();

            CreateMap<EmployeeSubTask, ClientSubTask>();
            CreateMap<ClientSubTask, EmployeeSubTask>();

            CreateMap<EmployeeChildTask, ClientChildTask>();
            CreateMap<ClientChildTask, EmployeeChildTask>();

            CreateMap<MateComment, MateCommentAddUpdateRequest>();
            CreateMap<MateCommentAddUpdateRequest, MateComment>();

            CreateMap<MateComment, MateCommentAddUpdateResponse>();
            CreateMap<MateCommentAddUpdateResponse, MateComment>();

            CreateMap<MateCommentAttachment, MateCommentAttachmentGetAllResponse>();
            CreateMap<MateCommentAttachmentGetAllResponse, MateCommentAttachment>();

            CreateMap<EmployeeSubTask, EmployeeSubTaskListResponse>();
            CreateMap<EmployeeSubTaskListResponse, EmployeeSubTask>();

            CreateMap<EmployeeChildTask, EmployeeChildTaskListResponse>();
            CreateMap<EmployeeChildTaskListResponse, EmployeeChildTask>();

            CreateMap<EmployeeTaskUser, EmployeeProjectTaskUserListResponse>();
            CreateMap<EmployeeProjectTaskUserListResponse, EmployeeTaskUser>();

            CreateMap<EmployeeTaskListRequest, EmployeeGroupTaskListRequest>();
            CreateMap<EmployeeGroupTaskListRequest, EmployeeTaskListRequest>();

            CreateMap<EmployeeGroupTask, EmployeeTask>();
            CreateMap<EmployeeTask, EmployeeGroupTask>();

            CreateMap<EmployeeTask, EmployeeTaskDetailResponse>();
            CreateMap<EmployeeTaskDetailResponse, EmployeeTask>();

            CreateMap<EmployeeTaskUser, EmployeeTaskUserDetailResponse>();
            CreateMap<EmployeeTaskUserDetailResponse, EmployeeTaskUser>();

            CreateMap<EmployeeSubTask, EmployeeTaskDetailSubTask>();
            CreateMap<EmployeeTaskDetailSubTask, EmployeeSubTask>();

            CreateMap<EmployeeTaskActivity, EmployeeTaskDetailActivityResponse>();
            CreateMap<EmployeeTaskDetailActivityResponse, EmployeeTaskActivity>();

            CreateMap<ClientSocialMedia, ClientSocialMediaAddUpdateRequest>();
            CreateMap<ClientSocialMediaAddUpdateRequest, ClientSocialMedia>();

            CreateMap<ClientSocialMedia, ClientSocialMediaAddUpdateResponse>();
            CreateMap<ClientSocialMediaAddUpdateResponse, ClientSocialMedia>();

            CreateMap<AssetsManufacturer, AssetsManufacturerAddRequest>();
            CreateMap<AssetsManufacturerAddRequest, AssetsManufacturer>();

            CreateMap<AssetsManufacturer, AssetsManufacturerAddResponse>();
            CreateMap<AssetsManufacturerAddResponse, AssetsManufacturer>();

            CreateMap<AssetsManufacturer, AssetsManufacturerListResponse>();
            CreateMap<AssetsManufacturerListResponse, AssetsManufacturer>();

            CreateMap<AssetsManufacturer, AssetsManufacturerDetailResponse>();
            CreateMap<AssetsManufacturerDetailResponse, AssetsManufacturer>();

            CreateMap<ContractAsset, ContractAssetAddUpdateRequest>();
            CreateMap<ContractAssetAddUpdateRequest, ContractAsset>();

            CreateMap<ContractAsset, ContractAssetAddUpdateResponse>();
            CreateMap<ContractAssetAddUpdateResponse, ContractAsset>();

            CreateMap<ContractAsset, ContractAssetDetailResponse>();
            CreateMap<ContractAssetDetailResponse, ContractAsset>();

            CreateMap<Department, DepartmentAddUpdateRequest>();
            CreateMap<DepartmentAddUpdateRequest, Department>();

            CreateMap<Department, DepartmentAddUpdateResponse>();
            CreateMap<DepartmentAddUpdateResponse, Department>();

            CreateMap<Department, DepartmentListResponse>();
            CreateMap<DepartmentListResponse, Department>();

            CreateMap<Department, DepartmentDetailResponse>();
            CreateMap<DepartmentDetailResponse, Department>();

            CreateMap<SatisficationLevel, SatisficationLevelAddUpdateRequest>();
            CreateMap<SatisficationLevelAddUpdateRequest, SatisficationLevel>();

            CreateMap<SatisficationLevel, SatisficationLevelAddUpdateResponse>();
            CreateMap<SatisficationLevelAddUpdateResponse, SatisficationLevel>();

            CreateMap<SatisficationLevel, SatisficationLevelListResponse>();
            CreateMap<SatisficationLevelListResponse, SatisficationLevel>();

            CreateMap<SatisficationLevel, SatisficationLevelDetailResponse>();
            CreateMap<SatisficationLevelDetailResponse, SatisficationLevel>();

            CreateMap<ClientUserRole, ClientUserRoleAddUpdateRequest>();
            CreateMap<ClientUserRoleAddUpdateRequest, ClientUserRole>();

            CreateMap<ClientUserRole, ClientUserRoleAddUpdateResponse>();
            CreateMap<ClientUserRoleAddUpdateResponse, ClientUserRole>();

            CreateMap<ClientUserRole, ClientUserRoleListResponse>();
            CreateMap<ClientUserRoleListResponse, ClientUserRole>();

            CreateMap<ClientUserRole, ClientUserRoleDetailResponse>();
            CreateMap<ClientUserRoleDetailResponse, ClientUserRole>();

            CreateMap<ClientUser, ClientUserAddUpdateRequest>();
            CreateMap<ClientUserAddUpdateRequest, ClientUser>();

            CreateMap<ClientUser, ClientUserAddUpdateResponse>();
            CreateMap<ClientUserAddUpdateResponse, ClientUser>();

            CreateMap<ClientUser, ClientUserDetailResponse>();
            CreateMap<ClientUserDetailResponse, ClientUser>();

            CreateMap<CRMNotes, CRMNotesAddUpdateRequest>();
            CreateMap<CRMNotesAddUpdateRequest, CRMNotes>();

            CreateMap<CRMNotes, CRMNotesAddUpdateResponse>();
            CreateMap<CRMNotesAddUpdateResponse, CRMNotes>();

            CreateMap<ClientAppointment, ClientAppointmentAddRequest>();
            CreateMap<ClientAppointmentAddRequest, ClientAppointment>();

            CreateMap<ClientAppointment, ClientAppointmentAddResponse>();
            CreateMap<ClientAppointmentAddResponse, ClientAppointment>();

            CreateMap<ClientAppointment, ClientAppointmentDetailResponse>();
            CreateMap<ClientAppointmentDetailResponse, ClientAppointment>();

            CreateMap<ClientSocialMedia, ClientSocialMediaDetailResponse>();
            CreateMap<ClientSocialMediaDetailResponse, ClientSocialMedia>();

            CreateMap<ClientSocialMedia, ClientSocialMediaListResponse>();
            CreateMap<ClientSocialMediaListResponse, ClientSocialMedia>();

            CreateMap<ContactTokenVM, ClientUserContactTokenRequest>();
            CreateMap<ClientUserContactTokenRequest, ContactTokenVM>();

            CreateMap<ContactTokenVM, ClientUserContactTokenResponse>();
            CreateMap<ClientUserContactTokenResponse, ContactTokenVM>();

            CreateMap<CommonContactTokenVM, ClientUserContactTokenRequest>();
            CreateMap<ClientUserContactTokenRequest, CommonContactTokenVM>();

            CreateMap<ClientUserContactTokenRequest, ClientUserContactTokenResponse>();
            CreateMap<ClientUserContactTokenResponse, ClientUserContactTokenRequest>();

            CreateMap<ClientUserCreateContactResponse, ClientCreateContactVM>();
            CreateMap<ClientCreateContactVM, ClientUserCreateContactResponse>();

            CreateMap<Salutation, SalutationListResponse>();
            CreateMap<SalutationListResponse, Salutation>();

            CreateMap<Contract, ContractDetailResponse>();
            CreateMap<ContractDetailResponse, Contract>();

            CreateMap<Contract, ContractDropdownListResponse>();
            CreateMap<ContractDropdownListResponse, Contract>();

            CreateMap<Contract, AddUpdateContractResponse>();
            CreateMap<AddUpdateContractResponse, Contract>();

            CreateMap<ProjectCategory, ProjectCategoryAddUpdateRequest>();
            CreateMap<ProjectCategoryAddUpdateRequest, ProjectCategory>();

            CreateMap<ProjectCategory, ProjectCategoryAddUpdateResponse>();
            CreateMap<ProjectCategoryAddUpdateResponse, ProjectCategory>();

            CreateMap<ProjectCategory, ProjectCategoryListResponse>();
            CreateMap<ProjectCategoryListResponse, ProjectCategory>();

            CreateMap<ProjectCategory, ProjectCategoryDetailResponse>();
            CreateMap<ProjectCategoryDetailResponse, ProjectCategory>();

            CreateMap<Client, ClientDetailResponse>();
            CreateMap<ClientDetailResponse, Client>();

            CreateMap<InboxThreads, ClientConversationEmailListResponse>();
            CreateMap<ClientConversationEmailListResponse, InboxThreads>();

            CreateMap<CRMNotes, CRMNotesDetailResponse>();
            CreateMap<CRMNotesDetailResponse, CRMNotes>();

            CreateMap<ServiceArticlePrice, ServiceArticlePriceAddRequest>();
            CreateMap<ServiceArticlePriceAddRequest, ServiceArticlePrice>();

            CreateMap<ServiceArticlePrice, ServiceArticlePriceAddResponse>();
            CreateMap<ServiceArticlePriceAddResponse, ServiceArticlePrice>();

            CreateMap<ServiceArticlePrice, ServiceArticlePriceListResponse>();
            CreateMap<ServiceArticlePriceListResponse, ServiceArticlePrice>();

            CreateMap<ServiceArticlePrice, ServiceArticlePriceDetailResponse>();
            CreateMap<ServiceArticlePriceDetailResponse, ServiceArticlePrice>();

            CreateMap<AssetsManufacturer, AssetsManufacturerDropDownListResponse>();
            CreateMap<AssetsManufacturerDropDownListResponse, AssetsManufacturer>();

            CreateMap<ContractType, ContractTypeDropDownListResponse>();
            CreateMap<ContractTypeDropDownListResponse, ContractType>();

            CreateMap<Department, DepartmentDropDownListResponse>();
            CreateMap<DepartmentDropDownListResponse, Department>();

            CreateMap<SatisficationLevel, SatisficationLevelDropDownListResponse>();
            CreateMap<SatisficationLevelDropDownListResponse, SatisficationLevel>();

            CreateMap<ClientUserRole, ClientUserRoleDropDownListResponse>();
            CreateMap<ClientUserRoleDropDownListResponse, ClientUserRole>();

            CreateMap<InvoiceInterval, InvoiceIntervalDropDownListResponse>();
            CreateMap<InvoiceIntervalDropDownListResponse, InvoiceInterval>();

            CreateMap<MateCategoryDto, MateCategoryAddRequest>();
            CreateMap<MateCategoryAddRequest, MateCategoryDto>();

            CreateMap<MateCategory, MateCategoryAddResponse>();
            CreateMap<MateCategoryAddResponse, MateCategory>();

            CreateMap<MateCategoryDto, MateCategoryUpdateRequest>();
            CreateMap<MateCategoryUpdateRequest, MateCategoryDto>();

            CreateMap<MateCategory, MateCategoryUpdateResponse>();
            CreateMap<MateCategoryUpdateResponse, MateCategory>();

            CreateMap<MateCategory, MateCategoryDto>();
            CreateMap<MateCategoryDto, MateCategory>();

            CreateMap<MateCategory, MateCategoryDetailResponse>();
            CreateMap<MateCategoryDetailResponse, MateCategory>();

            CreateMap<MateCategory, MateCategoryListResponse>();
            CreateMap<MateCategoryListResponse, MateCategory>();

            CreateMap<MateCategory, MateCategoryListByTableResponse>();
            CreateMap<MateCategoryListByTableResponse, MateCategory>();

            CreateMap<MatePriority, MatePriorityDto>();
            CreateMap<MatePriorityDto, MatePriority>();

            CreateMap<MatePriorityDto, MatePriorityAddRequest>();
            CreateMap<MatePriorityAddRequest, MatePriorityDto>();

            CreateMap<MatePriority, MatePriorityAddResponse>();
            CreateMap<MatePriorityAddResponse, MatePriority>();

            CreateMap<MatePriorityDto, MatePriorityUpdateRequest>();
            CreateMap<MatePriorityUpdateRequest, MatePriorityDto>();

            CreateMap<MatePriority, MatePriorityUpdateResponse>();
            CreateMap<MatePriorityUpdateResponse, MatePriority>();

            CreateMap<MatePriority, MatePriorityDetailResponse>();
            CreateMap<MatePriorityDetailResponse, MatePriority>();

            CreateMap<MatePriority, MatePriorityListResponse>();
            CreateMap<MatePriorityListResponse, MatePriority>();

            CreateMap<MatePriority, MatePriorityListByTableResponse>();
            CreateMap<MatePriorityListByTableResponse, MatePriority>();

            CreateMap<MateTicket, MateTicketAddUpdateRequest>();
            CreateMap<MateTicketAddUpdateRequest, MateTicket>();

            CreateMap<MateTicket, MateTicketAddUpdateResponse>();
            CreateMap<MateTicketAddUpdateResponse, MateTicket>();

            CreateMap<MateTicket, MateTicketDetailResponse>();
            CreateMap<MateTicketDetailResponse, MateTicket>();

            CreateMap<MateTicketTask, MateTicketTaskAddUpdateResponse>();
            CreateMap<MateTicketTaskAddUpdateResponse, MateTicketTask>();

            CreateMap<MateTicketUser, MateTicketAssignUserResponse>();
            CreateMap<MateTicketAssignUserResponse, MateTicketUser>();

            CreateMap<MateTicket, MateTicketDropDownListResponse>();
            CreateMap<MateTicketDropDownListResponse, MateTicket>();

            CreateMap<EmployeeTask, EmployeeTaskDropDownListResponse>();
            CreateMap<EmployeeTaskDropDownListResponse, EmployeeTask>();

            CreateMap<EmployeeProject, ProjectDropDownListResponse>();
            CreateMap<ProjectDropDownListResponse, EmployeeProject>();

            CreateMap<MateTicket, MateTicketTimeRecordListResponse>();
            CreateMap<MateTicketTimeRecordListResponse, MateTicket>();
            // End


        }
    }
}