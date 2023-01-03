using System.Threading.Tasks;
using matcrm.data.Models.Tables;

namespace matcrm.data
{
    public interface IUnitOfWork
    {
        IRepository<ErrorLog> ErrorLogRepository { get; }
        IRepository<Language> LanguageRepository { get; }
        IRepository<Role> RoleRepository { get; }
        IRepository<Tenant> TenantRepository { get; }
        IRepository<User> UserRepository { get; }
        IRepository<EmailTemplate> EmailTemplateRepository { get; }
        IRepository<VerificationCode> VerificationCodeRepository { get; }
        IRepository<OneClappTask> OneClappTaskRepository { get; }
        IRepository<OneClappSubTask> OneClappSubTaskRepository { get; }
        IRepository<OneClappChildTask> OneClappChildTaskRepository { get; }
        IRepository<Models.Tables.TaskStatus> TaskStatusRepository { get; }
        IRepository<OneClappTaskUser> OneClappTaskUserRepository { get; }
        IRepository<OneClappSubTaskUser> OneClappSubTaskUserRepository { get; }
        IRepository<OneClappChildTaskUser> OneClappChildTaskUserRepository { get; }
        IRepository<TaskTimeRecord> TaskTimeRecordRepository { get; }
        IRepository<SubTaskTimeRecord> SubTaskTimeRecordRepository { get; }
        IRepository<ChildTaskTimeRecord> ChildTaskTimeRecordRepository { get; }
        IRepository<TaskAttachment> TaskAttachmentRepository { get; }
        IRepository<SubTaskAttachment> SubTaskAttachmentRepository { get; }
        IRepository<ChildTaskAttachment> ChildTaskAttachmentRepository { get; }
        IRepository<TaskComment> TaskCommentRepository { get; }
        IRepository<SubTaskComment> SubTaskCommentRepository { get; }
        IRepository<ChildTaskComment> ChildTaskCommentRepository { get; }
        IRepository<TaskActivity> TaskActivityRepository { get; }
        IRepository<SubTaskActivity> SubTaskActivityRepository { get; }
        IRepository<ChildTaskActivity> ChildTaskActivityRepository { get; }
        IRepository<TenantConfig> TenantConfigRepository { get; }
        IRepository<TenantActivity> TenantActivityRepository { get; }
        IRepository<Section> SectionRepository { get; }
        IRepository<SectionActivity> SectionActivityRepository { get; }
        IRepository<PdfTemplate> PdfTemplateRepository { get; }
        IRepository<EmailProvider> EmailProviderRepository { get; }
        IRepository<EmailConfig> EmailConfigRepository { get; }
        IRepository<EmailLog> EmailLogRepository { get; }
        IRepository<ERPSystem> ERPSystemRepository { get; }
        IRepository<UserERPSystem> UserERPSystemRepository { get; }
        IRepository<CalendarList> CalendarListRepository { get; }
        IRepository<CalendarTask> CalendarTaskRepository { get; }
        IRepository<CalendarSubTask> CalendarSubTaskRepository { get; }
        IRepository<CalendarRepeatType> CalendarRepeatTypeRepository { get; }
        IRepository<OneClappLatestTheme> OneClappLatestThemeRepository { get; }
        IRepository<OneClappLatestThemeLayout> OneClappLatestThemeLayoutRepository { get; }
        IRepository<OneClappLatestThemeScheme> OneClappLatestThemeSchemeRepository { get; }
        IRepository<OneClappLatestThemeConfig> OneClappLatestThemeConfigRepository { get; }
        IRepository<CustomControl> CustomControlRepository { get; }
        IRepository<CustomControlOption> CustomControlOptionRepository { get; }
        IRepository<CustomField> CustomFieldRepository { get; }
        IRepository<CustomModule> CustomModuleRepository { get; }
        IRepository<ModuleField> ModuleFieldRepository { get; }
        IRepository<TenantModule> TenantModuleRepository { get; }
        IRepository<CustomTenantField> CustomTenantFieldRepository { get; }
        IRepository<Customer> CustomerRepository { get; }
        IRepository<CustomTable> CustomTableRepository { get; }
        IRepository<CustomFieldValue> CustomFieldValueRepository { get; }
        IRepository<CustomerType> CustomerTypeRepository { get; }
        IRepository<TicketPriority> TicketPriorityRepository { get; }
        IRepository<TicketStatus> TicketStatusRepository { get; }
        IRepository<TicketChannel> TicketChannelRepository { get; }
        IRepository<Tag> TagRepository { get; }
        IRepository<Ticket> TicketRepository { get; }
        IRepository<TicketTag> TicketTagRepository { get; }
        IRepository<TicketType> TicketTypeRepository { get; }
        IRepository<TicketCategory> TicketCategoryRepository { get; }
        IRepository<ActivityType> ActivityTypeRepository { get; }
        IRepository<EmailPhoneNoType> EmailPhoneNoTypeRepository { get; }
        IRepository<OrganizationLabel> OrganizationLabelRepository { get; }
        IRepository<CustomerLabel> CustomerLabelRepository { get; }
        IRepository<CustomerEmail> CustomerEmailRepository { get; }
        IRepository<CustomerPhone> CustomerPhoneRepository { get; }
        IRepository<CustomerNote> CustomerNoteRepository { get; }
        IRepository<CustomerActivity> CustomerActivityRepository { get; }
        IRepository<Organization> OrganizationRepository { get; }

        IRepository<EmployeeTaskStatus> EmployeeTaskStatusRepository { get; }
        IRepository<EmployeeProject> EmployeeProjectRepository { get; }
        IRepository<EmployeeProjectUser> EmployeeProjectUserRepository { get; }
        IRepository<EmployeeProjectStatus> EmployeeProjectStatusRepository { get; }
        IRepository<EmployeeTask> EmployeeTaskRepository { get; }
        IRepository<EmployeeTaskAttachment> EmployeeTaskAttachmentRepository { get; }
        IRepository<EmployeeTaskComment> EmployeeTaskCommentRepository { get; }
        IRepository<EmployeeTaskUser> EmployeeTaskUserRepository { get; }
        IRepository<EmployeeTaskTimeRecord> EmployeeTaskTimeRecordRepository { get; }
        IRepository<EmployeeTaskActivity> EmployeeTaskActivityRepository { get; }
        IRepository<EmployeeProjectActivity> EmployeeProjectActivityRepository { get; }
        IRepository<EmployeeSubTask> EmployeeSubTaskRepository { get; }
        IRepository<EmployeeSubTaskAttachment> EmployeeSubTaskAttachmentRepository { get; }
        IRepository<EmployeeSubTaskComment> EmployeeSubTaskCommentRepository { get; }
        IRepository<EmployeeSubTaskUser> EmployeeSubTaskUserRepository { get; }
        IRepository<EmployeeSubTaskTimeRecord> EmployeeSubTaskTimeRecordRepository { get; }
        IRepository<EmployeeSubTaskActivity> EmployeeSubTaskActivityRepository { get; }
        IRepository<EmployeeChildTask> EmployeeChildTaskRepository { get; }
        IRepository<EmployeeChildTaskAttachment> EmployeeChildTaskAttachmentRepository { get; }
        IRepository<EmployeeChildTaskComment> EmployeeChildTaskCommentRepository { get; }
        IRepository<EmployeeChildTaskUser> EmployeeChildTaskUserRepository { get; }
        IRepository<EmployeeChildTaskTimeRecord> EmployeeChildTaskTimeRecordRepository { get; }
        IRepository<EmployeeChildTaskActivity> EmployeeChildTaskActivityRepository { get; }
        IRepository<CustomerAttachment> CustomerAttachmentRepository { get; }
        IRepository<OrganizationNote> OrganizationNoteRepository { get; }
        IRepository<OrganizationAttachment> OrganizationAttachmentRepository { get; }
        IRepository<CustomerNotesComment> CustomerNotesCommentRepository { get; }
        IRepository<OrganizationNotesComment> OrganizationNotesCommentRepository { get; }
        IRepository<CustomTableColumn> CustomTableColumnRepository { get; }
        IRepository<TableColumnUser> TableColumnUserRepository { get; }
        IRepository<LeadLabel> LeadLabelRepository { get; }
        IRepository<Lead> LeadRepository { get; }
        IRepository<LeadNote> LeadNoteRepository { get; }
        IRepository<LabelCategory> LabelCategoryRepository { get; }
        IRepository<Label> LabelRepository { get; }
        IRepository<Status> StatusRepository { get; }
        IRepository<ActivityAvailability> ActivityAvailabilityRepository { get; }
        IRepository<CustomerActivityMember> CustomerActivityMemberRepository { get; }
        IRepository<OrganizationActivity> OrganizationActivityRepository { get; }
        IRepository<OrganizationActivityMember> OrganizationActivityMemberRepository { get; }
        IRepository<LeadActivity> LeadActivityRepository { get; }
        IRepository<LeadActivityMember> LeadActivityMemberRepository { get; }
        IRepository<IntProvider> IntProviderRepository { get; }
        IRepository<IntProviderApp> IntProviderAppRepository { get; }
        IRepository<IntProviderAppSecret> IntProviderAppSecretRepository { get; }
        IRepository<CheckList> CheckListRepository { get; }
        IRepository<CheckListUser> CheckListUserRepository { get; }
        IRepository<CalendarSyncActivity> CalendarSyncActivityRepository { get; }
        IRepository<OneClappModule> OneClappModuleRepository { get; }
        IRepository<OneClappFormType> OneClappFormTypeRepository { get; }
        IRepository<OneClappForm> OneClappFormRepository { get; }
        IRepository<OneClappFormField> OneClappFormFieldRepository { get; }
        IRepository<CheckListAssignUser> CheckListAssignUserRepository { get; }
        IRepository<OneClappFormStatus> OneClappFormStatusRepository { get; }
        IRepository<OneClappFormFieldValue> OneClappFormFieldValueRepository { get; }
        IRepository<OneClappRequestForm> OneClappRequestFormRepository { get; }
        IRepository<OneClappFormAction> OneClappFormActionRepository { get; }
        IRepository<WeClappUser> WeClappUserRepository { get; }
        IRepository<ERPSystemColumnMap> ERPSystemColumnMapRepository { get; }
        IRepository<ModuleRecordCustomField> ModuleRecordCustomFieldRepository { get; }
        IRepository<SubscriptionPlan> SubscriptionPlanRepository { get; }
        IRepository<SubscriptionPlanDetail> SubscriptionPlanDetailRepository { get; }
        IRepository<UserSubscription> UserSubscriptionRepository { get; }
        IRepository<SubscriptionType> SubscriptionTypeRepository { get; }
        IRepository<MollieCustomer> MollieCustomerRepository { get; }
        IRepository<MollieSubscription> MollieSubscriptionRepository { get; }
        IRepository<Salutation> SalutationRepository { get; }
        IRepository<ImportContactAttachment> ImportContactAttachmentRepository { get; }

        // Start form dynamic design table repository
        IRepository<BorderStyle> BorderStyleRepository { get; }
        IRepository<OneClappFormHeader> OneClappFormHeaderRepository { get; }
        IRepository<OneClappFormLayoutBackground> OneClappFormLayoutBackgroundRepository { get; }
        IRepository<OneClappFormLayout> OneClappFormLayoutRepository { get; }
        IRepository<ExternalUser> ExternalUserRepository { get; }
        IRepository<MailBoxTeam> MailBoxTeamRepository { get; }
        IRepository<MailComment> MailCommentRepository { get; }
        IRepository<MailAssignUser> MailAssignUserRepository { get; }
        IRepository<MailAssignCustomer> MailAssignCustomerRepository { get; }
        IRepository<MailRead> MailReadRepository { get; }
        IRepository<Discussion> DiscussionRepository { get; }
        IRepository<DiscussionParticipant> DiscussionParticipantRepository { get; }
        IRepository<DiscussionCommentAttachment> DiscussionCommentAttachmentRepository { get; }
        IRepository<DiscussionRead> DiscussionReadRepository { get; }
        IRepository<TeamInbox> TeamInboxRepository { get; }
        IRepository<DiscussionComment> DiscussionCommentRepository { get; }
        IRepository<MailCommentAttachment> MailCommentAttachmentRepository { get; }
        IRepository<MailParticipant> MailParticipantRepository { get; }
        IRepository<TeamInboxAccess> TeamInboxAccessRepository { get; }
        IRepository<CustomDomainEmailConfig> CustomDomainEmailConfigRepository { get; }
        IRepository<TaskWeclappUser> TaskWeclappUserRepository { get; }
        IRepository<ServiceArticleCategory> ServiceArticleCategoryRepository { get; }
        IRepository<Currency> CurrencyRepository { get; }
        IRepository<Tax> TaxRepository { get; }
        IRepository<TaxRate> TaxRateRepository { get; }
        IRepository<ServiceArticle> ServiceArticleRepository { get; }
        IRepository<ServiceArticleHour> ServiceArticleHourRepository { get; }
        IRepository<Country> CountryRepository { get; }
        IRepository<State> StateRepository { get; }
        IRepository<City> CityRepository { get; }
        IRepository<StandardTimeZone> StandardTimeZoneRepository { get; }
        IRepository<Client> ClientRepository { get; }
        IRepository<ClientEmail> ClientEmailRepository { get; }
        IRepository<ClientPhone> ClientPhoneRepository { get; }
        IRepository<ContractType> ContractTypeRepository { get; }
        IRepository<Contract> ContractRepository { get; }
        IRepository<ContractArticle> ContractArticleRepository { get; }
        IRepository<InvoiceInterval> InvoiceIntervalRepository { get; }
        IRepository<ClientInvoice> ClientInvoiceRepository { get; }
        IRepository<InvoiceMollieCustomer> InvoiceMollieCustomerRepository { get; }
        IRepository<InvoiceMollieSubscription> InvoiceMollieSubscriptionRepository { get; }
        IRepository<MateTimeRecord> MateTimeRecordRepository { get; }
        IRepository<MateProjectTimeRecord> MateProjectTimeRecordRepository { get; }
        IRepository<MateTaskTimeRecord> MateTaskTimeRecordRepository { get; }
        IRepository<ContractActivity> ContractActivityRepository { get; }
        IRepository<ContractInvoice> ContractInvoiceRepository { get; }
        IRepository<ProjectContract> ProjectContractRepository { get; }
        IRepository<MateSubTaskTimeRecord> MateSubTaskTimeRecordRepository { get; }
        IRepository<MateChildTaskTimeRecord> MateChildTaskTimeRecordRepository { get; }
        IRepository<MateComment> MateCommentRepository { get; }
        IRepository<MateCommentAttachment> MateCommentAttachmentRepository { get; }
        IRepository<MateTaskComment> MateTaskCommentRepository { get; }
        IRepository<MateSubTaskComment> MateSubTaskCommentRepository { get; }
        IRepository<MateChildTaskComment> MateChildTaskCommentRepository { get; }
        IRepository<ClientSocialMedia> ClientSocialMediaRepository { get; }
        IRepository<AssetsManufacturer> AssetsManufacturerRepository { get; }
        IRepository<ContractAsset> ContractAssetRepository { get; }
        IRepository<Department> DepartmentRepository { get; }
        IRepository<SatisficationLevel> SatisficationLevelRepository { get; }
        IRepository<ClientUserRole> ClientUserRoleRepository { get; }
        IRepository<ClientUser> ClientUserRepository { get; }
        IRepository<CRMNotes> CRMNotesRepository { get; }
        IRepository<ClientAppointment> ClientAppointmentRepository { get; }
        IRepository<ClientIntProviderAppSecret> ClientIntProviderAppSecretRepository { get; }
        IRepository<IntProviderContact> IntProviderContactRepository { get; }
        IRepository<ProjectCategory> ProjectCategoryRepository { get; }
        IRepository<ServiceArticlePrice> ServiceArticlePriceRepository { get; }
        IRepository<MateCategory> MateCategoryRepository { get; }
        IRepository<MatePriority> MatePriorityRepository { get; }
        IRepository<EmployeeProjectTask> EmployeeProjectTaskRepository { get; }
        IRepository<EmployeeClientTask> EmployeeClientTaskRepository { get; }
        IRepository<MateTicket> MateTicketRepository { get; }
        IRepository<MateTicketActivity> MateTicketActivityRepository { get; }
        IRepository<MateTicketUser> MateTicketUserRepository { get; }
        IRepository<MateClientTicket> MateClientTicketRepository { get; }
        IRepository<MateProjectTicket> MateProjectTicketRepository { get; }
        IRepository<MateTicketTimeRecord> MateTicketTimeRecordRepository { get; }
        IRepository<MateTicketTask> MateTicketTaskRepository { get; }
        IRepository<MateTicketComment> MateTicketCommentRepository { get; }

        // End form dynamic design table repository

        Task Commit();

        Task CommitAsync();

        void BeginTransaction();

        void Rollback();

        //DbRawSqlQuery<T> SQLQuery<T>(string sql, params object[] parameters);
    }
}