using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using matcrm.data.Context;
using matcrm.data.Infrastructure;
using matcrm.data.Models.Tables;

namespace matcrm.data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbFactory dbFactory;
        private OneClappContext dbContext;
        private IDbContextTransaction _transaction;

        #region 'Repository Section'
        public IRepository<ErrorLog> ErrorLogRepository => new Repository<ErrorLog>(dbFactory);
        public IRepository<Language> LanguageRepository => new Repository<Language>(dbFactory);
        public IRepository<Tenant> TenantRepository => new Repository<Tenant>(dbFactory);
        public IRepository<Role> RoleRepository => new Repository<Role>(dbFactory);
        public IRepository<User> UserRepository => new Repository<User>(dbFactory);
        public IRepository<VerificationCode> VerificationCodeRepository => new Repository<VerificationCode>(dbFactory);
        public IRepository<EmailTemplate> EmailTemplateRepository => new Repository<EmailTemplate>(dbFactory);
        public IRepository<OneClappTask> OneClappTaskRepository => new Repository<OneClappTask>(dbFactory);
        public IRepository<OneClappSubTask> OneClappSubTaskRepository => new Repository<OneClappSubTask>(dbFactory);
        public IRepository<Models.Tables.TaskStatus> TaskStatusRepository => new Repository<Models.Tables.TaskStatus>(dbFactory);
        public IRepository<OneClappChildTask> OneClappChildTaskRepository => new Repository<OneClappChildTask>(dbFactory);
        public IRepository<OneClappTaskUser> OneClappTaskUserRepository => new Repository<OneClappTaskUser>(dbFactory);
        public IRepository<OneClappSubTaskUser> OneClappSubTaskUserRepository => new Repository<OneClappSubTaskUser>(dbFactory);
        public IRepository<OneClappChildTaskUser> OneClappChildTaskUserRepository => new Repository<OneClappChildTaskUser>(dbFactory);
        public IRepository<TaskTimeRecord> TaskTimeRecordRepository => new Repository<TaskTimeRecord>(dbFactory);
        public IRepository<SubTaskTimeRecord> SubTaskTimeRecordRepository => new Repository<SubTaskTimeRecord>(dbFactory);
        public IRepository<ChildTaskTimeRecord> ChildTaskTimeRecordRepository => new Repository<ChildTaskTimeRecord>(dbFactory);
        public IRepository<TaskAttachment> TaskAttachmentRepository => new Repository<TaskAttachment>(dbFactory);
        public IRepository<SubTaskAttachment> SubTaskAttachmentRepository => new Repository<SubTaskAttachment>(dbFactory);
        public IRepository<ChildTaskAttachment> ChildTaskAttachmentRepository => new Repository<ChildTaskAttachment>(dbFactory);
        public IRepository<TaskComment> TaskCommentRepository => new Repository<TaskComment>(dbFactory);
        public IRepository<SubTaskComment> SubTaskCommentRepository => new Repository<SubTaskComment>(dbFactory);
        public IRepository<ChildTaskComment> ChildTaskCommentRepository => new Repository<ChildTaskComment>(dbFactory);
        public IRepository<TaskActivity> TaskActivityRepository => new Repository<TaskActivity>(dbFactory);
        public IRepository<SubTaskActivity> SubTaskActivityRepository => new Repository<SubTaskActivity>(dbFactory);
        public IRepository<ChildTaskActivity> ChildTaskActivityRepository => new Repository<ChildTaskActivity>(dbFactory);
        public IRepository<TenantConfig> TenantConfigRepository => new Repository<TenantConfig>(dbFactory);
        public IRepository<TenantActivity> TenantActivityRepository => new Repository<TenantActivity>(dbFactory);
        public IRepository<Section> SectionRepository => new Repository<Section>(dbFactory);
        public IRepository<SectionActivity> SectionActivityRepository => new Repository<SectionActivity>(dbFactory);
        public IRepository<PdfTemplate> PdfTemplateRepository => new Repository<PdfTemplate>(dbFactory);
        public IRepository<EmailProvider> EmailProviderRepository => new Repository<EmailProvider>(dbFactory);
        public IRepository<EmailConfig> EmailConfigRepository => new Repository<EmailConfig>(dbFactory);
        public IRepository<EmailLog> EmailLogRepository => new Repository<EmailLog>(dbFactory);
        public IRepository<ERPSystem> ERPSystemRepository => new Repository<ERPSystem>(dbFactory);
        public IRepository<UserERPSystem> UserERPSystemRepository => new Repository<UserERPSystem>(dbFactory);
        public IRepository<CalendarList> CalendarListRepository => new Repository<CalendarList>(dbFactory);
        public IRepository<CalendarTask> CalendarTaskRepository => new Repository<CalendarTask>(dbFactory);
        public IRepository<CalendarSubTask> CalendarSubTaskRepository => new Repository<CalendarSubTask>(dbFactory);
        public IRepository<CalendarRepeatType> CalendarRepeatTypeRepository => new Repository<CalendarRepeatType>(dbFactory);
        public IRepository<OneClappLatestTheme> OneClappLatestThemeRepository => new Repository<OneClappLatestTheme>(dbFactory);

        public IRepository<OneClappLatestThemeLayout> OneClappLatestThemeLayoutRepository => new Repository<OneClappLatestThemeLayout>(dbFactory);
        public IRepository<OneClappLatestThemeScheme> OneClappLatestThemeSchemeRepository => new Repository<OneClappLatestThemeScheme>(dbFactory);
        public IRepository<OneClappLatestThemeConfig> OneClappLatestThemeConfigRepository => new Repository<OneClappLatestThemeConfig>(dbFactory);

        // Start Custom Field Repository
        public IRepository<CustomControl> CustomControlRepository => new Repository<CustomControl>(dbFactory);
        public IRepository<CustomControlOption> CustomControlOptionRepository => new Repository<CustomControlOption>(dbFactory);
        public IRepository<CustomField> CustomFieldRepository => new Repository<CustomField>(dbFactory);
        public IRepository<CustomModule> CustomModuleRepository => new Repository<CustomModule>(dbFactory);
        public IRepository<ModuleField> ModuleFieldRepository => new Repository<ModuleField>(dbFactory);
        public IRepository<TenantModule> TenantModuleRepository => new Repository<TenantModule>(dbFactory);
        public IRepository<CustomTenantField> CustomTenantFieldRepository => new Repository<CustomTenantField>(dbFactory);
        public IRepository<CustomTable> CustomTableRepository => new Repository<CustomTable>(dbFactory);
        public IRepository<CustomFieldValue> CustomFieldValueRepository => new Repository<CustomFieldValue>(dbFactory);

        // End Custom Field Repository
        public IRepository<Customer> CustomerRepository => new Repository<Customer>(dbFactory);
        public IRepository<CustomerType> CustomerTypeRepository => new Repository<CustomerType>(dbFactory);
        public IRepository<TicketPriority> TicketPriorityRepository => new Repository<TicketPriority>(dbFactory);
        public IRepository<TicketStatus> TicketStatusRepository => new Repository<TicketStatus>(dbFactory);
        public IRepository<TicketChannel> TicketChannelRepository => new Repository<TicketChannel>(dbFactory);
        public IRepository<Tag> TagRepository => new Repository<Tag>(dbFactory);
        public IRepository<Ticket> TicketRepository => new Repository<Ticket>(dbFactory);
        public IRepository<TicketTag> TicketTagRepository => new Repository<TicketTag>(dbFactory);
        public IRepository<TicketType> TicketTypeRepository => new Repository<TicketType>(dbFactory);
        public IRepository<TicketCategory> TicketCategoryRepository => new Repository<TicketCategory>(dbFactory);

        public IRepository<ActivityType> ActivityTypeRepository => new Repository<ActivityType>(dbFactory);
        public IRepository<EmailPhoneNoType> EmailPhoneNoTypeRepository => new Repository<EmailPhoneNoType>(dbFactory);
        public IRepository<OrganizationLabel> OrganizationLabelRepository => new Repository<OrganizationLabel>(dbFactory);
        public IRepository<CustomerLabel> CustomerLabelRepository => new Repository<CustomerLabel>(dbFactory);
        public IRepository<CustomerEmail> CustomerEmailRepository => new Repository<CustomerEmail>(dbFactory);
        public IRepository<CustomerPhone> CustomerPhoneRepository => new Repository<CustomerPhone>(dbFactory);
        public IRepository<CustomerNote> CustomerNoteRepository => new Repository<CustomerNote>(dbFactory);
        public IRepository<CustomerActivity> CustomerActivityRepository => new Repository<CustomerActivity>(dbFactory);
        public IRepository<Organization> OrganizationRepository => new Repository<Organization>(dbFactory);

        public IRepository<EmployeeTaskStatus> EmployeeTaskStatusRepository => new Repository<EmployeeTaskStatus>(dbFactory);
        public IRepository<EmployeeProject> EmployeeProjectRepository => new Repository<EmployeeProject>(dbFactory);
        public IRepository<EmployeeProjectStatus> EmployeeProjectStatusRepository => new Repository<EmployeeProjectStatus>(dbFactory);
        public IRepository<EmployeeTask> EmployeeTaskRepository => new Repository<EmployeeTask>(dbFactory);
        public IRepository<EmployeeTaskAttachment> EmployeeTaskAttachmentRepository => new Repository<EmployeeTaskAttachment>(dbFactory);
        public IRepository<EmployeeTaskComment> EmployeeTaskCommentRepository => new Repository<EmployeeTaskComment>(dbFactory);
        public IRepository<EmployeeTaskUser> EmployeeTaskUserRepository => new Repository<EmployeeTaskUser>(dbFactory);
        public IRepository<EmployeeTaskTimeRecord> EmployeeTaskTimeRecordRepository => new Repository<EmployeeTaskTimeRecord>(dbFactory);
        public IRepository<EmployeeTaskActivity> EmployeeTaskActivityRepository => new Repository<EmployeeTaskActivity>(dbFactory);
        public IRepository<EmployeeProjectActivity> EmployeeProjectActivityRepository => new Repository<EmployeeProjectActivity>(dbFactory);

        public IRepository<EmployeeSubTask> EmployeeSubTaskRepository => new Repository<EmployeeSubTask>(dbFactory);
        public IRepository<EmployeeSubTaskAttachment> EmployeeSubTaskAttachmentRepository => new Repository<EmployeeSubTaskAttachment>(dbFactory);
        public IRepository<EmployeeSubTaskComment> EmployeeSubTaskCommentRepository => new Repository<EmployeeSubTaskComment>(dbFactory);
        public IRepository<EmployeeSubTaskUser> EmployeeSubTaskUserRepository => new Repository<EmployeeSubTaskUser>(dbFactory);
        public IRepository<EmployeeSubTaskTimeRecord> EmployeeSubTaskTimeRecordRepository => new Repository<EmployeeSubTaskTimeRecord>(dbFactory);
        public IRepository<EmployeeSubTaskActivity> EmployeeSubTaskActivityRepository => new Repository<EmployeeSubTaskActivity>(dbFactory);
        public IRepository<EmployeeChildTask> EmployeeChildTaskRepository => new Repository<EmployeeChildTask>(dbFactory);
        public IRepository<EmployeeChildTaskAttachment> EmployeeChildTaskAttachmentRepository => new Repository<EmployeeChildTaskAttachment>(dbFactory);
        public IRepository<EmployeeChildTaskComment> EmployeeChildTaskCommentRepository => new Repository<EmployeeChildTaskComment>(dbFactory);
        public IRepository<EmployeeChildTaskUser> EmployeeChildTaskUserRepository => new Repository<EmployeeChildTaskUser>(dbFactory);
        public IRepository<EmployeeChildTaskTimeRecord> EmployeeChildTaskTimeRecordRepository => new Repository<EmployeeChildTaskTimeRecord>(dbFactory);
        public IRepository<EmployeeChildTaskActivity> EmployeeChildTaskActivityRepository => new Repository<EmployeeChildTaskActivity>(dbFactory);
        public IRepository<EmployeeProjectUser> EmployeeProjectUserRepository => new Repository<EmployeeProjectUser>(dbFactory);






        public IRepository<CustomerAttachment> CustomerAttachmentRepository => new Repository<CustomerAttachment>(dbFactory);
        public IRepository<OrganizationNote> OrganizationNoteRepository => new Repository<OrganizationNote>(dbFactory);
        public IRepository<OrganizationAttachment> OrganizationAttachmentRepository => new Repository<OrganizationAttachment>(dbFactory);

        public IRepository<CustomerNotesComment> CustomerNotesCommentRepository => new Repository<CustomerNotesComment>(dbFactory);
        public IRepository<OrganizationNotesComment> OrganizationNotesCommentRepository => new Repository<OrganizationNotesComment>(dbFactory);
        public IRepository<CustomTableColumn> CustomTableColumnRepository => new Repository<CustomTableColumn>(dbFactory);
        public IRepository<TableColumnUser> TableColumnUserRepository => new Repository<TableColumnUser>(dbFactory);
        public IRepository<LeadLabel> LeadLabelRepository => new Repository<LeadLabel>(dbFactory);
        public IRepository<Lead> LeadRepository => new Repository<Lead>(dbFactory);
        public IRepository<LeadNote> LeadNoteRepository => new Repository<LeadNote>(dbFactory);

        public IRepository<LabelCategory> LabelCategoryRepository => new Repository<LabelCategory>(dbFactory);

        public IRepository<ActivityAvailability> ActivityAvailabilityRepository => new Repository<ActivityAvailability>(dbFactory);
        public IRepository<CustomerActivityMember> CustomerActivityMemberRepository => new Repository<CustomerActivityMember>(dbFactory);
        public IRepository<Label> LabelRepository => new Repository<Label>(dbFactory);
        public IRepository<Status> StatusRepository => new Repository<Status>(dbFactory);
        public IRepository<OrganizationActivity> OrganizationActivityRepository => new Repository<OrganizationActivity>(dbFactory);
        public IRepository<OrganizationActivityMember> OrganizationActivityMemberRepository => new Repository<OrganizationActivityMember>(dbFactory);
        public IRepository<LeadActivity> LeadActivityRepository => new Repository<LeadActivity>(dbFactory);
        public IRepository<LeadActivityMember> LeadActivityMemberRepository => new Repository<LeadActivityMember>(dbFactory);

        public IRepository<IntProvider> IntProviderRepository => new Repository<IntProvider>(dbFactory);
        public IRepository<IntProviderApp> IntProviderAppRepository => new Repository<IntProviderApp>(dbFactory);
        public IRepository<IntProviderAppSecret> IntProviderAppSecretRepository => new Repository<IntProviderAppSecret>(dbFactory);
        public IRepository<CheckList> CheckListRepository => new Repository<CheckList>(dbFactory);
        public IRepository<CheckListUser> CheckListUserRepository => new Repository<CheckListUser>(dbFactory);
        public IRepository<CalendarSyncActivity> CalendarSyncActivityRepository => new Repository<CalendarSyncActivity>(dbFactory);
        public IRepository<OneClappModule> OneClappModuleRepository => new Repository<OneClappModule>(dbFactory);
        public IRepository<OneClappFormType> OneClappFormTypeRepository => new Repository<OneClappFormType>(dbFactory);
        public IRepository<OneClappForm> OneClappFormRepository => new Repository<OneClappForm>(dbFactory);
        public IRepository<OneClappFormField> OneClappFormFieldRepository => new Repository<OneClappFormField>(dbFactory);
        public IRepository<CheckListAssignUser> CheckListAssignUserRepository => new Repository<CheckListAssignUser>(dbFactory);
        public IRepository<OneClappFormStatus> OneClappFormStatusRepository => new Repository<OneClappFormStatus>(dbFactory);
        public IRepository<OneClappRequestForm> OneClappRequestFormRepository => new Repository<OneClappRequestForm>(dbFactory);
        public IRepository<OneClappFormFieldValue> OneClappFormFieldValueRepository => new Repository<OneClappFormFieldValue>(dbFactory);
        public IRepository<OneClappFormAction> OneClappFormActionRepository => new Repository<OneClappFormAction>(dbFactory);
        public IRepository<WeClappUser> WeClappUserRepository => new Repository<WeClappUser>(dbFactory);
        public IRepository<ERPSystemColumnMap> ERPSystemColumnMapRepository => new Repository<ERPSystemColumnMap>(dbFactory);
        public IRepository<ModuleRecordCustomField> ModuleRecordCustomFieldRepository => new Repository<ModuleRecordCustomField>(dbFactory);
        public IRepository<SubscriptionPlan> SubscriptionPlanRepository => new Repository<SubscriptionPlan>(dbFactory);
        public IRepository<SubscriptionPlanDetail> SubscriptionPlanDetailRepository => new Repository<SubscriptionPlanDetail>(dbFactory);
        public IRepository<SubscriptionType> SubscriptionTypeRepository => new Repository<SubscriptionType>(dbFactory);
        public IRepository<UserSubscription> UserSubscriptionRepository => new Repository<UserSubscription>(dbFactory);
        public IRepository<MollieCustomer> MollieCustomerRepository => new Repository<MollieCustomer>(dbFactory);
        public IRepository<MollieSubscription> MollieSubscriptionRepository => new Repository<MollieSubscription>(dbFactory);
        public IRepository<Salutation> SalutationRepository => new Repository<Salutation>(dbFactory);
        public IRepository<ImportContactAttachment> ImportContactAttachmentRepository => new Repository<ImportContactAttachment>(dbFactory);

        // Start form dynamic design table repository

        public IRepository<BorderStyle> BorderStyleRepository => new Repository<BorderStyle>(dbFactory);
        public IRepository<OneClappFormHeader> OneClappFormHeaderRepository => new Repository<OneClappFormHeader>(dbFactory);
        public IRepository<OneClappFormLayoutBackground> OneClappFormLayoutBackgroundRepository => new Repository<OneClappFormLayoutBackground>(dbFactory);
        public IRepository<OneClappFormLayout> OneClappFormLayoutRepository => new Repository<OneClappFormLayout>(dbFactory);

        // End form dynamic design table repository

        public IRepository<ExternalUser> ExternalUserRepository => new Repository<ExternalUser>(dbFactory);

        public IRepository<MailBoxTeam> MailBoxTeamRepository => new Repository<MailBoxTeam>(dbFactory);
        public IRepository<MailComment> MailCommentRepository => new Repository<MailComment>(dbFactory);
        public IRepository<MailAssignUser> MailAssignUserRepository => new Repository<MailAssignUser>(dbFactory);
        public IRepository<MailRead> MailReadRepository => new Repository<MailRead>(dbFactory);
        public IRepository<Discussion> DiscussionRepository => new Repository<Discussion>(dbFactory);
        public IRepository<DiscussionParticipant> DiscussionParticipantRepository => new Repository<DiscussionParticipant>(dbFactory);
        public IRepository<DiscussionCommentAttachment> DiscussionCommentAttachmentRepository => new Repository<DiscussionCommentAttachment>(dbFactory);
        public IRepository<DiscussionRead> DiscussionReadRepository => new Repository<DiscussionRead>(dbFactory);
        public IRepository<DiscussionComment> DiscussionCommentRepository => new Repository<DiscussionComment>(dbFactory);
        public IRepository<MailCommentAttachment> MailCommentAttachmentRepository => new Repository<MailCommentAttachment>(dbFactory);

        public IRepository<MailParticipant> MailParticipantRepository => new Repository<MailParticipant>(dbFactory);

        public IRepository<TeamInbox> TeamInboxRepository => new Repository<TeamInbox>(dbFactory);
        public IRepository<TeamInboxAccess> TeamInboxAccessRepository => new Repository<TeamInboxAccess>(dbFactory);
        public IRepository<MailAssignCustomer> MailAssignCustomerRepository => new Repository<MailAssignCustomer>(dbFactory);
        public IRepository<CustomDomainEmailConfig> CustomDomainEmailConfigRepository => new Repository<CustomDomainEmailConfig>(dbFactory);
        public IRepository<TaskWeclappUser> TaskWeclappUserRepository => new Repository<TaskWeclappUser>(dbFactory);
        public IRepository<ServiceArticleCategory> ServiceArticleCategoryRepository => new Repository<ServiceArticleCategory>(dbFactory);
        public IRepository<Currency> CurrencyRepository => new Repository<Currency>(dbFactory);
        public IRepository<Tax> TaxRepository => new Repository<Tax>(dbFactory);
        public IRepository<TaxRate> TaxRateRepository => new Repository<TaxRate>(dbFactory);
        public IRepository<ServiceArticle> ServiceArticleRepository => new Repository<ServiceArticle>(dbFactory);
        public IRepository<ServiceArticleHour> ServiceArticleHourRepository => new Repository<ServiceArticleHour>(dbFactory);
        public IRepository<Country> CountryRepository => new Repository<Country>(dbFactory);
        public IRepository<State> StateRepository => new Repository<State>(dbFactory);
        public IRepository<City> CityRepository => new Repository<City>(dbFactory);
        public IRepository<StandardTimeZone> StandardTimeZoneRepository => new Repository<StandardTimeZone>(dbFactory);
        public IRepository<Client> ClientRepository => new Repository<Client>(dbFactory);
        public IRepository<ClientEmail> ClientEmailRepository => new Repository<ClientEmail>(dbFactory);
        public IRepository<ClientPhone> ClientPhoneRepository => new Repository<ClientPhone>(dbFactory);
        public IRepository<ContractType> ContractTypeRepository => new Repository<ContractType>(dbFactory);
        public IRepository<Contract> ContractRepository => new Repository<Contract>(dbFactory);
        public IRepository<ContractArticle> ContractArticleRepository => new Repository<ContractArticle>(dbFactory);
        public IRepository<InvoiceInterval> InvoiceIntervalRepository => new Repository<InvoiceInterval>(dbFactory);
        public IRepository<ClientInvoice> ClientInvoiceRepository => new Repository<ClientInvoice>(dbFactory);
        public IRepository<InvoiceMollieCustomer> InvoiceMollieCustomerRepository => new Repository<InvoiceMollieCustomer>(dbFactory);
        public IRepository<InvoiceMollieSubscription> InvoiceMollieSubscriptionRepository => new Repository<InvoiceMollieSubscription>(dbFactory);
        public IRepository<MateTimeRecord> MateTimeRecordRepository => new Repository<MateTimeRecord>(dbFactory);
        public IRepository<MateProjectTimeRecord> MateProjectTimeRecordRepository => new Repository<MateProjectTimeRecord>(dbFactory);
        public IRepository<MateTaskTimeRecord> MateTaskTimeRecordRepository => new Repository<MateTaskTimeRecord>(dbFactory);
        public IRepository<ContractActivity> ContractActivityRepository => new Repository<ContractActivity>(dbFactory);
        public IRepository<ContractInvoice> ContractInvoiceRepository => new Repository<ContractInvoice>(dbFactory);
        public IRepository<ProjectContract> ProjectContractRepository => new Repository<ProjectContract>(dbFactory);
        public IRepository<MateSubTaskTimeRecord> MateSubTaskTimeRecordRepository => new Repository<MateSubTaskTimeRecord>(dbFactory);
        public IRepository<MateChildTaskTimeRecord> MateChildTaskTimeRecordRepository => new Repository<MateChildTaskTimeRecord>(dbFactory);
        public IRepository<MateComment> MateCommentRepository => new Repository<MateComment>(dbFactory);
        public IRepository<MateCommentAttachment> MateCommentAttachmentRepository => new Repository<MateCommentAttachment>(dbFactory);
        public IRepository<MateTaskComment> MateTaskCommentRepository => new Repository<MateTaskComment>(dbFactory);
        public IRepository<MateSubTaskComment> MateSubTaskCommentRepository => new Repository<MateSubTaskComment>(dbFactory);
        public IRepository<MateChildTaskComment> MateChildTaskCommentRepository => new Repository<MateChildTaskComment>(dbFactory);
        public IRepository<ClientSocialMedia> ClientSocialMediaRepository => new Repository<ClientSocialMedia>(dbFactory);
        public IRepository<AssetsManufacturer> AssetsManufacturerRepository => new Repository<AssetsManufacturer>(dbFactory);
        public IRepository<ContractAsset> ContractAssetRepository => new Repository<ContractAsset>(dbFactory);
        public IRepository<Department> DepartmentRepository => new Repository<Department>(dbFactory);
        public IRepository<SatisficationLevel> SatisficationLevelRepository => new Repository<SatisficationLevel>(dbFactory);
        public IRepository<ClientUserRole> ClientUserRoleRepository => new Repository<ClientUserRole>(dbFactory);
        public IRepository<ClientUser> ClientUserRepository => new Repository<ClientUser>(dbFactory);
        public IRepository<CRMNotes> CRMNotesRepository => new Repository<CRMNotes>(dbFactory);
        public IRepository<ClientAppointment> ClientAppointmentRepository => new Repository<ClientAppointment>(dbFactory);
        public IRepository<ClientIntProviderAppSecret> ClientIntProviderAppSecretRepository => new Repository<ClientIntProviderAppSecret>(dbFactory);
        public IRepository<IntProviderContact> IntProviderContactRepository => new Repository<IntProviderContact>(dbFactory);
        public IRepository<ProjectCategory> ProjectCategoryRepository => new Repository<ProjectCategory>(dbFactory);
        public IRepository<ServiceArticlePrice> ServiceArticlePriceRepository => new Repository<ServiceArticlePrice>(dbFactory);
        public IRepository<MateCategory> MateCategoryRepository => new Repository<MateCategory>(dbFactory);
        public IRepository<MatePriority> MatePriorityRepository => new Repository<MatePriority>(dbFactory);
        public IRepository<EmployeeProjectTask> EmployeeProjectTaskRepository => new Repository<EmployeeProjectTask>(dbFactory);
        public IRepository<EmployeeClientTask> EmployeeClientTaskRepository => new Repository<EmployeeClientTask>(dbFactory);
        public IRepository<MateTicket> MateTicketRepository => new Repository<MateTicket>(dbFactory);
        public IRepository<MateTicketActivity> MateTicketActivityRepository => new Repository<MateTicketActivity>(dbFactory);
        public IRepository<MateTicketUser> MateTicketUserRepository => new Repository<MateTicketUser>(dbFactory);
        public IRepository<MateClientTicket> MateClientTicketRepository => new Repository<MateClientTicket>(dbFactory);
        public IRepository<MateProjectTicket> MateProjectTicketRepository => new Repository<MateProjectTicket>(dbFactory);
        public IRepository<MateTicketTimeRecord> MateTicketTimeRecordRepository => new Repository<MateTicketTimeRecord>(dbFactory);
        public IRepository<MateTicketTask> MateTicketTaskRepository => new Repository<MateTicketTask>(dbFactory);
        public IRepository<MateTicketComment> MateTicketCommentRepository => new Repository<MateTicketComment>(dbFactory);

        #endregion
        public UnitOfWork(IDbFactory dbFactory)
        {
            this.dbFactory = dbFactory;
        }

        public OneClappContext DbContext
        {
            get { return dbContext ?? (dbContext = dbFactory.Init()); }
        }

        public void BeginTransaction()
        {
            _transaction = DbContext.Database.BeginTransaction();
        }

        public void Rollback()
        {
            if (_transaction != null)
                _transaction.Rollback();
        }

        public async Task Commit()
        {
            try
            {
                await DbContext.SaveChangesAsync();
                if (_transaction != null)
                    _transaction.Commit();
            }
            catch (Exception ex)
            {
                if (_transaction != null)
                    _transaction.Rollback();
            }
        }

        public async Task CommitAsync()
        {
            await DbContext.SaveChangesAsync();
        }
    }
}