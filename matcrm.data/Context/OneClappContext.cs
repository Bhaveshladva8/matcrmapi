using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using matcrm.data.Models.Tables;
using static matcrm.data.Helpers.DataUtility;

namespace matcrm.data.Context
{
    public class OneClappContext : DbContext
    {
        public static string ConnectionString { get; set; }
        public static string CurrentURL { get; set; }
        public static string ProjectName { get; set; }
        public static string SecretKey { get; set; }
        public static string AppURL { get; set; }
        public static string TokenExpireMinute { get; set; }
        public static string GoogleClientId { get; set; }
        public static string GoogleSecretKey { get; set; }

        public static string GoogleCalendarClientId { get; set; }
        public static string GoogleCalendarSecretKey { get; set; }
        public static string GoogleCalendarApiKey { get; set; }
        public static string GoogleScopes { get; set; }
        public static string GoogleCalendarId { get; set; }
        public static string ValidIssuer { get; set; }
        public static string ValidAudience { get; set; }
        public static string SubmitUrl { get; set; }

        public static string MollieClientId { get; set; }
        public static string MollieSecretKey { get; set; }
        public static string MollieApiKey { get; set; }
        public static string MollieDefaultRedirectUrl { get; set; }

        public static string HcaptchaSiteKey { get; set; }
        public static string HcaptchaSiteSecret { get; set; }
        public static string HcaptchaVerifyUrl { get; set; }

        public static string MicroSoftClientId { get; set; }
        public static string MicroSecretKey { get; set; }
        public static string MicroSoftTenantId { get; set; }
        public static string MicroSoftRedirectUrl { get; set; }
        public static string MicrosoftUserScopes { get; set; }
        public static string MicrosoftClientScopes { get; set; }
        public static string OriginalUserProfileDirPath { get; set; }
        public static string ReSizedUserProfileDirPath { get; set; }
        public static string CustomerFileUploadDirPath { get; set; }
        public static string DiscussionCommentUploadDirPath { get; set; }
        public static string EmployeeChildTaskUploadDirPath { get; set; }
        public static string EmployeeSubTaskUploadDirPath { get; set; }
        public static string SubTaskUploadDirPath { get; set; }
        public static string ChildTaskUploadDirPath { get; set; }
        public static string EmployeeTaskUploadDirPath { get; set; }
        public static string DynamicFormHeaderDirPath { get; set; }
        public static string ImportContactDirPath { get; set; }
        public static string DynamicFormLayoutDirPath { get; set; }
        public static string DefaultLayoutDirPath { get; set; }
        public static string MailCommentUploadDirPath { get; set; }
        public static string FormsJSUploadDirPath { get; set; }
        public static string ModalFormsJSUploadDirPath { get; set; }
        public static string SlidingFormJSUploadDirPath { get; set; }
        public static string OrganizationUploadDirPath { get; set; }
        public static string ProjectLogoDirPath { get; set; }
        public static string TaskUploadDirPath { get; set; }
        public static string BgImageDirPath { get; set; }
        public static string LogoImageDirPath { get; set; }
        public static string ClamAVServerURL { get; set; }
        public static string ClamAVServerPort { get; set; }
        public static bool ClamAVServerIsLive { get; set; }
        public static string ClientLogoDirPath { get; set; }
        public static string PaidPaymentStatus { get; set; }
        public static string MailServer { get; set; }
        public static string MailFrom { get; set; }
        public static string MailPassword { get; set; }
        public static string MateCommentUploadDirPath { get; set; }
        public static string ClientUserRootRole { get; set; }
        public static string ClientUserLogoDirPath { get; set; }
        public static string ProjectCategoryIconDirPath { get; set; }
        public static string MateCategoryIconDirPath { get; set; }
        public static string ProjectMailUrl { get; set; }
        public static string TaskMailUrl { get; set; }
        public static string TicketMailUrl { get; set; }
        public OneClappContext() { }

        public OneClappContext(DbContextOptions<OneClappContext> options) : base(options)
        {
            Database.SetCommandTimeout(150000);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //  optionsBuilder.UseSqlServer ("Data Source=tcp:113.212.87.157\\TA4,1438;Initial Catalog=OneClappDB;User Id=sa;Password=Admin@123;");
            // optionsBuilder.UseSqlServer("Data Source=tcp:TA4\\TA4,1438,1438;Initial Catalog=OneClappDB_Shakti;User Id=sa;Password=Admin@123;MultipleActiveResultSets=True;");
            // optionsBuilder.UseSqlServer ("Data Source=tcp:113.212.87.157\\TA4,1438;Initial Catalog=OneClappDB_Theme;User Id=sa;Password=Admin@123;");
            // optionsBuilder.UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=beMate_DB;Trusted_Connection=True;MultipleActiveResultSets=True;");
            // optionsBuilder.UseSqlServer(ConnectionString);
            optionsBuilder.UseNpgsql(ConnectionString);
            //optionsBuilder.UseNpgsql("User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=MatCrm_test;Pooling=true;");
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        // public DbSet<Phrases> Phrases { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<VerificationCode> VerificationCode { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<Language> Language { get; set; }
        public DbSet<Role> Role { get; set; }

        // Start OneClapp theme tables
        public DbSet<OneClappLatestTheme> OneClappLatestTheme { get; set; }
        public DbSet<OneClappLatestThemeScheme> OneClappLatestThemeScheme { get; set; }
        public DbSet<OneClappLatestThemeLayout> OneClappLatestThemeLayout { get; set; }
        public DbSet<OneClappLatestThemeConfig> OneClappLatestThemeConfig { get; set; }
        // End OneClapp theme tables

        // Start OneClapp task tables
        public DbSet<OneClappTask> OneClappTask { get; set; }
        public DbSet<OneClappSubTask> OneClappSubTask { get; set; }
        public DbSet<Models.Tables.TaskStatus> TaskStatus { get; set; }
        public DbSet<OneClappChildTask> OneClappChildTask { get; set; }
        public DbSet<OneClappTaskUser> OneClappTaskUser { get; set; }
        public DbSet<TaskWeclappUser> TaskWeclappUser { get; set; }
        public DbSet<OneClappSubTaskUser> OneClappSubTaskUser { get; set; }
        public DbSet<OneClappChildTaskUser> OneClappChildTaskUser { get; set; }
        public DbSet<TaskTimeRecord> TaskTimeRecord { get; set; }
        public DbSet<SubTaskTimeRecord> SubTaskTimeRecord { get; set; }
        public DbSet<ChildTaskTimeRecord> ChildTaskTimeRecord { get; set; }
        public DbSet<TaskAttachment> TaskAttachment { get; set; }
        public DbSet<SubTaskAttachment> SubTaskAttachment { get; set; }
        public DbSet<ChildTaskAttachment> ChildTaskAttachment { get; set; }
        public DbSet<TaskComment> TaskComment { get; set; }
        public DbSet<SubTaskComment> SubTaskComment { get; set; }
        public DbSet<ChildTaskComment> ChildTaskComment { get; set; }
        public DbSet<TenantConfig> TenantConfig { get; set; }
        public DbSet<TaskActivity> TaskActivity { get; set; }
        public DbSet<SubTaskActivity> SubTaskActivity { get; set; }
        public DbSet<ChildTaskActivity> ChildTaskActivity { get; set; }
        public DbSet<TenantActivity> TenantActivity { get; set; }
        public DbSet<Section> Section { get; set; }
        public DbSet<SectionActivity> SectionActivity { get; set; }
        public DbSet<PdfTemplate> PdfTemplate { get; set; }

        // End Oneclapp task

        // Start Custom field tables

        public DbSet<CustomControl> CustomControl { get; set; }
        public DbSet<CustomControlOption> CustomControlOption { get; set; }
        public DbSet<CustomField> CustomField { get; set; }
        public DbSet<CustomModule> CustomModule { get; set; }
        public DbSet<ModuleField> ModuleField { get; set; }
        public DbSet<TenantModule> TenantModule { get; set; }
        public DbSet<CustomTenantField> CustomTenantField { get; set; }
        public DbSet<CustomTable> CustomTable { get; set; }
        public DbSet<CustomFieldValue> CustomFieldValue { get; set; }
        public DbSet<CustomTableColumn> CustomTableColumn { get; set; }
        public DbSet<TableColumnUser> TableColumnUser { get; set; }
        // End Custom field tables

        //Start Customers Tables

        public DbSet<LabelCategory> LabelCategory { get; set; }
        public DbSet<Label> Label { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<ActivityType> ActivityType { get; set; }
        public DbSet<ActivityAvailability> ActivityAvailability { get; set; }


        public DbSet<Organization> Organization { get; set; }
        public DbSet<OrganizationNote> OrganizationNote { get; set; }
        public DbSet<OrganizationAttachment> OrganizationAttachment { get; set; }
        public DbSet<OrganizationNotesComment> OrganizationNotesComment { get; set; }
        public DbSet<OrganizationActivity> OrganizationActivity { get; set; }
        public DbSet<OrganizationActivityMember> OrganizationActivityMember { get; set; }
        public DbSet<OrganizationLabel> OrganizationLabel { get; set; }

        // public DbSet<CustomerTopic> CustomerTopic { get; set; }
        public DbSet<CustomerType> CustomerType { get; set; }
        public DbSet<TicketPriority> TicketPriority { get; set; }
        public DbSet<TicketStatus> TicketStatus { get; set; }
        public DbSet<TicketChannel> TicketChannel { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<Ticket> Ticket { get; set; }
        public DbSet<TicketTag> TicketTag { get; set; }
        public DbSet<TicketType> TicketType { get; set; }
        public DbSet<Salutation> Salutation { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<TicketCategory> TicketCategory { get; set; }
        public DbSet<EmailPhoneNoType> EmailPhoneNoType { get; set; }
        public DbSet<CustomerEmail> CustomerEmail { get; set; }
        public DbSet<CustomerPhone> CustomerPhone { get; set; }
        public DbSet<CustomerNote> CustomerNote { get; set; }
        public DbSet<CustomerAttachment> CustomerAttachment { get; set; }
        public DbSet<CustomerNotesComment> CustomerNotesComment { get; set; }
        public DbSet<CustomerLabel> CustomerLabel { get; set; }

        public DbSet<CustomerActivity> CustomerActivity { get; set; }
        public DbSet<CustomerActivityMember> CustomerActivityMember { get; set; }


        public DbSet<Lead> Lead { get; set; }
        public DbSet<LeadNote> LeadNote { get; set; }
        public DbSet<LeadLabel> LeadLabel { get; set; }

        public DbSet<LeadActivity> LeadActivity { get; set; }
        public DbSet<LeadActivityMember> LeadActivityMember { get; set; }

        public DbSet<WeClappUser> WeClappUser { get; set; }
        public DbSet<ModuleRecordCustomField> ModuleRecordCustomField { get; set; }

        // End Customer Tables

        // start email tables
        public DbSet<EmailProvider> EmailProvider { get; set; }
        public DbSet<EmailConfig> EmailConfig { get; set; }
        public DbSet<EmailLog> EmailLog { get; set; }
        // end email tables

        // Start ERP system tables
        public DbSet<ERPSystem> ERPSystem { get; set; }
        public DbSet<UserERPSystem> UserERPSystem { get; set; }
        public DbSet<ERPSystemColumnMap> ERPSystemColumnMap { get; set; }
        // End ERP system tables

        // // Start Employee Task Tables
        public DbSet<EmployeeTaskStatus> EmployeeTaskStatus { get; set; }
        public DbSet<EmployeeProject> EmployeeProject { get; set; }
        public DbSet<EmployeeProjectUser> EmployeeProjectUser { get; set; }
        public DbSet<EmployeeProjectStatus> EmployeeProjectStatus { get; set; }
        public DbSet<EmployeeTask> EmployeeTask { get; set; }
        public DbSet<EmployeeTaskAttachment> EmployeeTaskAttachment { get; set; }
        public DbSet<EmployeeTaskComment> EmployeeTaskComment { get; set; }
        public DbSet<EmployeeTaskUser> EmployeeTaskUser { get; set; }
        public DbSet<EmployeeTaskTimeRecord> EmployeeTaskTimeRecord { get; set; }
        public DbSet<EmployeeTaskActivity> EmployeeTaskActivity { get; set; }
        public DbSet<EmployeeProjectActivity> EmployeeProjectActivity { get; set; }

        public DbSet<EmployeeSubTask> EmployeeSubTask { get; set; }
        public DbSet<EmployeeSubTaskAttachment> EmployeeSubTaskAttachment { get; set; }
        public DbSet<EmployeeSubTaskComment> EmployeeSubTaskComment { get; set; }
        public DbSet<EmployeeSubTaskUser> EmployeeSubTaskUser { get; set; }
        public DbSet<EmployeeSubTaskTimeRecord> EmployeeSubTaskTimeRecord { get; set; }
        public DbSet<EmployeeSubTaskActivity> EmployeeSubTaskActivity { get; set; }

        public DbSet<EmployeeChildTask> EmployeeChildTask { get; set; }
        public DbSet<EmployeeChildTaskAttachment> EmployeeChildTaskAttachment { get; set; }
        public DbSet<EmployeeChildTaskComment> EmployeeChildTaskComment { get; set; }
        public DbSet<EmployeeChildTaskUser> EmployeeChildTaskUser { get; set; }
        public DbSet<EmployeeChildTaskTimeRecord> EmployeeChildTaskTimeRecord { get; set; }
        public DbSet<EmployeeChildTaskActivity> EmployeeChildTaskActivity { get; set; }

        // // End Employee Task Tables


        // Start Calendar task tables
        public DbSet<CalendarRepeatType> CalendarRepeatType { get; set; }
        public DbSet<CalendarList> CalendarList { get; set; }

        public DbSet<CalendarTask> CalendarTask { get; set; }
        public DbSet<CalendarSubTask> CalendarSubTask { get; set; }
        // End Calendar task tables

        // Start IntProvider tables
        public DbSet<IntProvider> IntProvider { get; set; }
        public DbSet<IntProviderApp> IntProviderApp { get; set; }
        public DbSet<IntProviderAppSecret> IntProviderAppSecret { get; set; }
        public DbSet<CustomDomainEmailConfig> CustomDomainEmailConfig { get; set; }
        public DbSet<CalendarSyncActivity> CalendarSyncActivity { get; set; }
        // End IntProvider tables

        // Start checklist tables
        public DbSet<OneClappModule> OneClappModule { get; set; }
        public DbSet<CheckList> CheckList { get; set; }
        public DbSet<CheckListUser> CheckListUser { get; set; }
        public DbSet<CheckListAssignUser> CheckListAssignUser { get; set; }
        // End checklist tables

        // Start Form builder module tables
        public DbSet<OneClappFormType> OneClappFormType { get; set; }
        public DbSet<OneClappForm> OneClappForm { get; set; }
        public DbSet<OneClappFormField> OneClappFormField { get; set; }
        public DbSet<OneClappFormStatus> OneClappFormStatus { get; set; }
        public DbSet<OneClappRequestForm> OneClappRequestForm { get; set; }
        public DbSet<OneClappFormFieldValue> OneClappFormFieldValue { get; set; }
        public DbSet<OneClappFormAction> OneClappFormAction { get; set; }

        public DbSet<BorderStyle> BorderStyle { get; set; }
        // public DbSet<Border> Border { get; set; }
        // public DbSet<Typography> Typography { get; set; }
        // public DbSet<OneClappFormFieldStyle> OneClappFormFieldStyle { get; set; }
        // public DbSet<OneClappFormStyle> OneClappFormStyle { get; set; }
        public DbSet<OneClappFormHeader> OneClappFormHeader { get; set; }
        public DbSet<OneClappFormLayout> OneClappFormLayout { get; set; }
        public DbSet<OneClappFormLayoutBackground> OneClappFormLayoutBackground { get; set; }
        // public DbSet<BoxShadow> BoxShadow { get; set; }

        // End Form builder module

        // Start Subscription tables
        public DbSet<SubscriptionPlan> SubscriptionPlan { get; set; }
        public DbSet<SubscriptionType> SubscriptionType { get; set; }
        public DbSet<SubscriptionPlanDetail> SubscriptionPlanDetail { get; set; }
        public DbSet<UserSubscription> UserSubscription { get; set; }
        public DbSet<MollieCustomer> MollieCustomer { get; set; }
        public DbSet<MollieSubscription> MollieSubscription { get; set; }

        // End Subscription tables

        public DbSet<ImportContactAttachment> ImportContactAttachment { get; set; }

        public DbSet<ExternalUser> ExternalUser { get; set; }

        // Start mailbox module tables

        public DbSet<MailBoxTeam> MailBoxTeam { get; set; }
        public DbSet<MailComment> MailBoxComment { get; set; }
        public DbSet<MailAssignUser> MailAssignUser { get; set; }
        public DbSet<MailRead> MailRead { get; set; }
        public DbSet<Discussion> Discussion { get; set; }
        public DbSet<DiscussionRead> DiscussionRead { get; set; }
        public DbSet<DiscussionCommentAttachment> DiscussionCommentAttachment { get; set; }
        public DbSet<DiscussionParticipant> DiscussionParticipant { get; set; }
        public DbSet<DiscussionComment> DiscussionComment { get; set; }
        public DbSet<TeamInbox> TeamInbox { get; set; }
        public DbSet<TeamInboxAccess> TeamInboxAccess { get; set; }
        public DbSet<MailCommentAttachment> MailCommentAttachment { get; set; }
        public DbSet<MailParticipant> MailParticipant { get; set; }
        public DbSet<MailAssignCustomer> MailAssignCustomer { get; set; }
        // End mailbox module tables
        public DbSet<Tax> Tax { get; set; }
        public DbSet<TaxRate> TaxRate { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<ServiceArticleCategory> ServiceArticleCategory { get; set; }
        public DbSet<ServiceArticle> ServiceArticle { get; set; }
        public DbSet<ServiceArticleHour> ServiceArticleHour { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<StandardTimeZone> StandardTimeZone { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientEmail> ClientEmail { get; set; }
        public DbSet<ClientPhone> ClientPhone { get; set; }
        public DbSet<ContractType> ContractType { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<ContractArticle> ContractArticle { get; set; }
        public DbSet<InvoiceInterval> InvoiceInterval { get; set; }
        public DbSet<ClientInvoice> ClientInvoice { get; set; }
        public DbSet<InvoiceMollieCustomer> InvoiceMollieCustomer { get; set; }
        public DbSet<InvoiceMollieSubscription> InvoiceMollieSubscription { get; set; }
        public DbSet<MateTimeRecord> MateTimeRecord { get; set; }
        public DbSet<MateProjectTimeRecord> MateProjectTimeRecord { get; set; }
        public DbSet<MateTaskTimeRecord> MateTaskTimeRecord { get; set; }
        public DbSet<MateSubTaskTimeRecord> MateSubTaskTimeRecord { get; set; }
        public DbSet<MateChildTaskTimeRecord> MateChildTaskTimeRecord { get; set; }
        public DbSet<ContractActivity> ContractActivity { get; set; }
        public DbSet<ContractInvoice> ContractInvoice { get; set; }
        public DbSet<ProjectContract> ProjectContract { get; set; }
        public DbSet<MateComment> MateComment { get; set; }
        public DbSet<MateCommentAttachment> MateCommentAttachment { get; set; }
        public DbSet<MateTaskComment> MateTaskComment { get; set; }
        public DbSet<MateSubTaskComment> MateSubTaskComment { get; set; }
        public DbSet<MateChildTaskComment> MateChildTaskComment { get; set; }
        public DbSet<SocialMedia> SocialMedia { get; set; }
        public DbSet<ClientSocialMedia> ClientSocialMedia { get; set; }
        public DbSet<ClientAppointment> ClientAppointment { get; set; }
        public DbSet<AssetsManufacturer> AssetsManufacturer { get; set; }
        public DbSet<ContractAsset> ContractAsset { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<ClientUserRole> ClientUserRole { get; set; }
        public DbSet<ClientUser> ClientUser { get; set; }
        public DbSet<SatisficationLevel> SatisficationLevel { get; set; }
        public DbSet<CRMNotes> CRMNotes { get; set; }
        public DbSet<ClientIntProviderAppSecret> ClientIntProviderAppSecret { get; set; }
        public DbSet<IntProviderContact> IntProviderContact { get; set; }
        public DbSet<ProjectCategory> ProjectCategory { get; set; }
        public DbSet<ServiceArticlePrice> ServiceArticlePrice { get; set; }
        public DbSet<MatePriority> MatePriority { get; set; }
        public DbSet<MateCategory> MateCategory { get; set; }
        public DbSet<EmployeeProjectTask> EmployeeProjectTask { get; set; }
        public DbSet<EmployeeClientTask> EmployeeClientTask { get; set; }
        public DbSet<MateTicket> MateTicket { get; set; }
        public DbSet<MateClientTicket> MateClientTicket { get; set; }
        public DbSet<MateProjectTicket> MateProjectTicket { get; set; }
        public DbSet<MateTicketTask> MateTicketTask { get; set; }
        public DbSet<MateTicketUser> MateTicketUser { get; set; }
        public DbSet<MateTicketActivity> MateTicketActivity { get; set; }
        public DbSet<MateTicketTimeRecord> MateTicketTimeRecord { get; set; }
        public DbSet<MateTicketComment> MateTicketComment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            // AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                 .SelectMany(t => t.GetProperties())
                 .Where
                 (p
                   => p.ClrType == typeof(DateTime)
                      || p.ClrType == typeof(DateTime?)
                 )
        )
            {
                property.SetColumnType("timestamp without time zone");
            }
        }
    }

    public static class DataSeeder
    {
        public static bool AllMigrationsApplied(this OneClappContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static void Seed(OneClappContext _context)
        {
            try
            {
                SeedRoles(_context);
                SeedDefaultTenant(_context);
                SeedSuperAdminUser(_context);
                SeedLanguages(_context);
                // SeedThemeColor(_context);
                // SeedLayoutStyle(_context);
                // SeedLayoutWidth(_context);
                // SeedLayoutNavBarPosition(_context);
                // SeedLayoutNavBarVariant(_context);
                // SeedLayoutSidePanelPosition(_context);
                // SeedToolBarFooterPosition(_context);
                SeedCustomControl(_context);
                SeedCustomTable(_context);
                SeedCustomModule(_context);
                SeedCustomTableColumn(_context);
                SeedCustomerType(_context);
                SeedActivityType(_context);
                // SeedOrganizationLabel (_context);
                SeedEmailPhoneNoType(_context);
                SeedLabelCateGory(_context);
                SeedLabel(_context);
                SeedActivityAvailability(_context);
                SeedCalendarRepeatTypes(_context);
                SeedCalendarList(_context);
                SeedOneClappLatestThemeLayout(_context);
                SeedOneClappLatestThemeScheme(_context);
                SeedOneClappLatestTheme(_context);
                SeedIntProviders(_context);
                SeedIntProviderApps(_context);
                SeedOneClappModules(_context);
                SeedOneClappFormTypes(_context);
                SeedOneClappFormStatus(_context);
                SeedOneClappFormAction(_context);
                SeedSubscriptionPlan(_context);
                SeedSubscriptionPlanDetail(_context);
                SeedSubscriptionType(_context);
                SeedSalutation(_context);
                SeedBorderStyle(_context);
                SeedCountry(_context);
                SeedState(_context);
                SeedCity(_context);
                SeedTimeZone(_context);
                SeedCurrency(_context);
                SeedInvoiceInterval(_context);
                SeedSocialMedias(_context);
                SeedClientUserRoles(_context);
                SeedContractType(_context);
                SeedDepartment(_context);
                SeedSatisficationLevel(_context);
                SeedAssetsManufacturer(_context);
                SeedMatePriority(_context);
                SeedStatus(_context);
                // SeedCustomerLabel (_context);
                // SeedLeadLabel(_context);
                // // _context.SaveChanges ();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SeedDefaultTenant(OneClappContext _context)
        {
            if (_context.Tenants != null && !_context.Tenants.Any())
            {
                _context.Tenants.Add(
                    new Tenant()
                    {
                        Username = "Admin",
                        TenantName = "Admin",
                        CreatedOn = DateTime.UtcNow,
                        BlockedOn = null,
                        IsBlocked = false,
                        Token = "Admin@123",
                        IsAdmin = true
                    }
                );

                _context.Tenants.Add(
                    new Tenant()
                    {
                        Username = "Testit",
                        TenantName = "testit",
                        CreatedOn = DateTime.UtcNow,
                        BlockedOn = null,
                        IsBlocked = false,
                        Token = "1beaa048-43c0-47b4-a6d7-2263c6953b5b",
                        IsAdmin = true
                    }
                );

                _context.SaveChanges();
            }
        }

        public static void SeedSuperAdminUser(OneClappContext _context)
        {
            var password = "Admin@123";
            byte[] passwordSalt;
            byte[] passwordHash;
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            if (_context.Users != null && !_context.Users.Any())
            {

                // Super admin
                _context.Users.Add(
                    new User()
                    {
                        // TenantId = (int) OneclappDefaultTenants.Admin,
                        TenantId = _context.Tenants.Where(t => t.TenantName == "Admin").FirstOrDefault().TenantId,
                        FirstName = "Super",
                        LastName = "Admin",
                        UserName = "Super Admin",
                        Email = "super.admin@oneclapp.com",
                        WeClappToken = "1beaa048-43c0-47b4-a6d7-2263c6953b5b",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        IsEmailVerified = true,
                        VerifiedOn = DateTime.UtcNow,
                        IsBlocked = false,
                        IsDeleted = false,
                        // RoleId = (int) OneclappRoles.Admin,
                        RoleId = _context.Role.Where(t => t.RoleName == "Admin").FirstOrDefault().RoleId,
                        TempGuid = Convert.ToString(Guid.NewGuid()),
                        CreatedOn = DateTime.UtcNow
                    }
                );

                // Tenant admin
                _context.Users.Add(
                    new User()
                    {
                        // TenantId = (int) OneclappDefaultTenants.TestIT,
                        TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId,
                        FirstName = "Tenant",
                        LastName = "Admin",
                        UserName = "TenantAdmin",
                        Email = "tenant.admin@oneclapp.com",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        IsEmailVerified = true,
                        VerifiedOn = DateTime.UtcNow,
                        IsBlocked = false,
                        IsDeleted = false,
                        // RoleId = (int) OneclappRoles.TenantAdmin,
                        RoleId = _context.Role.Where(t => t.RoleName == "TenantAdmin").FirstOrDefault().RoleId,
                        TempGuid = Convert.ToString(Guid.NewGuid()),
                        CreatedOn = DateTime.UtcNow
                    }
                );

                // Tenant Manager
                _context.Users.Add(
                    new User()
                    {
                        // TenantId = (int) OneclappDefaultTenants.TestIT,
                        TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId,
                        FirstName = "Tenant",
                        LastName = "Manager",
                        UserName = "TenantManager",
                        Email = "tenant.manager@oneclapp.com",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        IsEmailVerified = true,
                        VerifiedOn = DateTime.UtcNow,
                        IsBlocked = false,
                        IsDeleted = false,
                        // RoleId = (int) OneclappRoles.TenantManager,
                        RoleId = _context.Role.Where(t => t.RoleName == "TenantManager").FirstOrDefault().RoleId,
                        TempGuid = Convert.ToString(Guid.NewGuid()),
                        CreatedOn = DateTime.UtcNow
                    }
                );

                // Tenant User
                _context.Users.Add(
                    new User()
                    {
                        // TenantId = (int) OneclappDefaultTenants.TestIT,
                        TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId,
                        FirstName = "Tenant",
                        LastName = "User",
                        UserName = "TenantUser",
                        Email = "tenant.user@oneclapp.com",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        IsEmailVerified = true,
                        VerifiedOn = DateTime.UtcNow,
                        IsBlocked = false,
                        IsDeleted = false,
                        // RoleId = (int) OneclappRoles.TenantUser,
                        RoleId = _context.Role.Where(t => t.RoleName == "TenantUser").FirstOrDefault().RoleId,
                        TempGuid = Convert.ToString(Guid.NewGuid()),
                        CreatedOn = DateTime.UtcNow
                    }
                );

                // External User
                _context.Users.Add(
                    new User()
                    {
                        // TenantId = (int) OneclappDefaultTenants.TestIT,
                        TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId,
                        FirstName = "Tenant",
                        LastName = "External User",
                        UserName = "TenantExternalUser",
                        Email = "tenant.externaluser@oneclapp.com",
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        IsEmailVerified = true,
                        VerifiedOn = DateTime.UtcNow,
                        IsBlocked = false,
                        IsDeleted = false,
                        // RoleId = (int) OneclappRoles.TenantExternalUser,
                        RoleId = _context.Role.Where(t => t.RoleName == "ExternalUser").FirstOrDefault().RoleId,
                        TempGuid = Convert.ToString(Guid.NewGuid()),
                        CreatedOn = DateTime.UtcNow
                    }
                );

                _context.SaveChanges();

            }
        }

        public static void SeedLanguages(OneClappContext _context)
        {

            if (_context.Language != null && !_context.Language.Any())
            {
                List<Language> languageList = new List<Language>() {
                new Language () { LanguageCode = "en-US", LanguageName = "English", IsActive = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new Language () { LanguageCode = "de-DE", LanguageName = "German", IsActive = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new Language () { LanguageCode = "gu-IN", LanguageName = "Gujarati", IsActive = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null }
                };
                languageList.AddRange(languageList);
                _context.Language.AddRange(languageList);
                _context.SaveChanges();
            }
        }

        public static void SeedRoles(OneClappContext _context)
        {

            if (_context.Role != null && !_context.Role.Any())
            {
                List<Role> roleList = new List<Role>() {
                new Role () { RoleName = "Admin", IsActive = true, TenantId = null, CreatedOn = DateTime.UtcNow, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Role () { RoleName = "TenantAdmin", IsActive = true, TenantId = null, CreatedOn = DateTime.UtcNow, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Role () { RoleName = "TenantManager", IsActive = true, TenantId = null, CreatedOn = DateTime.UtcNow, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Role () { RoleName = "TenantUser", IsActive = true, TenantId = null, CreatedOn = DateTime.UtcNow, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Role () { RoleName = "ExternalUser", IsActive = true, TenantId = null, CreatedOn = DateTime.UtcNow, UpdatedOn = null, IsDeleted = false, DeletedOn = null }
                };
                roleList.AddRange(roleList);
                _context.Role.AddRange(roleList);
                _context.SaveChanges();
            }
        }

        public static void SeedCustomControl(OneClappContext _context)
        {

            if (_context.CustomControl != null && !_context.CustomControl.Any())
            {
                List<CustomControl> customControlList = new List<CustomControl>() {
                new CustomControl () { Name = "TextBox", CreatedBy = null, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomControl () { Name = "TextArea", CreatedBy = null, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomControl () { Name = "DropDown", CreatedBy = null, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomControl () { Name = "Radio", CreatedBy = null, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomControl () { Name = "Checkbox", CreatedBy = null, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomControl () { Name = "Date", CreatedBy = null, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomControl () { Name = "Heading", CreatedBy = null, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null }
                };
                customControlList.AddRange(customControlList);
                _context.CustomControl.AddRange(customControlList);
                _context.SaveChanges();
            }
        }

        public static void SeedCustomTable(OneClappContext _context)
        {
            if (_context.CustomTable != null)
            {
                List<CustomTable> customTableList = new List<CustomTable>() {
                new CustomTable () { Name = "Person", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTable () { Name = "Ticket", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTable () { Name = "TaskStatus", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "User", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Organization", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Lead", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Form", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Project", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Task", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "EmployeeTask", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Contract", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Client", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTable () { Name = "Ticket", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null }
                };
                foreach (var c in customTableList)
                {
                    var check = _context.CustomTable.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.CustomTable.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedCustomModule(OneClappContext _context)
        {
            if (_context.CustomModule != null)
            {
                List<CustomModule> customModuleList = new List<CustomModule>() {
                new CustomModule () { Name = "Person", MasterTableId = _context.CustomTable.Where (t => t.Name == "Person").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomModule () { Name = "Ticket", MasterTableId = _context.CustomTable.Where (t => t.Name == "Ticket").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomModule () { Name = "TaskStatus", MasterTableId = _context.CustomTable.Where (t => t.Name == "TaskStatus").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "User", MasterTableId = _context.CustomTable.Where (t => t.Name == "User").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "Organization", MasterTableId = _context.CustomTable.Where (t => t.Name == "Organization").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "Lead", MasterTableId = _context.CustomTable.Where (t => t.Name == "Lead").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "Form", MasterTableId = _context.CustomTable.Where (t => t.Name == "Form").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "TimeTrack", MasterTableId = _context.CustomTable.Where (t => t.Name == "Project").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "Task", MasterTableId = _context.CustomTable.Where (t => t.Name == "Task").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "EmployeeTask", MasterTableId = _context.CustomTable.Where (t => t.Name == "EmployeeTask").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "Contract", MasterTableId = _context.CustomTable.Where (t => t.Name == "Contract").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "Client", MasterTableId = _context.CustomTable.Where (t => t.Name == "Client").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomModule () { Name = "Ticket", MasterTableId = _context.CustomTable.Where (t => t.Name == "Ticket").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null }
                };
                foreach (var c in customModuleList)
                {
                    var check = _context.CustomModule.FirstOrDefault(p => p.Name == c.Name && p.MasterTableId == c.MasterTableId);
                    if (check == null)
                    {
                        _context.CustomModule.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedCustomTableColumn(OneClappContext _context)
        {

            if (_context.CustomTableColumn != null)
            {

                // List<CustomTableColumn> tables = new List<CustomTableColumn>() {
                // new CustomTableColumn () { Name = "Name", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Label", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Organization", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Email", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Phone", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Name", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Label", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Address", ControlId = _context.CustomControl.Where (t => t.Name == "TextArea").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // // new CustomTableColumn () { Name = "People", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Title", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Label", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Organization", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Person", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, TenantId = _context.Tenants.Where(t => t.TenantName == "testit").FirstOrDefault().TenantId, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // };

                List<CustomTableColumn> customTableColumnList = new List<CustomTableColumn>() {
                new CustomTableColumn () { Name = "Name", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Label", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Organization", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Email", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Phone", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "FirstName", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "LastName", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Salutation", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Person").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Name", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Label", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Address", ControlId = _context.CustomControl.Where (t => t.Name == "TextArea").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "People", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Organization").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Title", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Label", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Organization", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Person", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Lead").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },


                new CustomTableColumn () { Name = "Name", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Form").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "SubmissionCount", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Form").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "CreatedOn", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Form").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Status", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Form").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },


                new CustomTableColumn () { Name = "Name", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Project").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Description", ControlId = _context.CustomControl.Where (t => t.Name == "TextArea").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Project").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Due Date", ControlId = _context.CustomControl.Where (t => t.Name == "Date").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Project").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Status", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Project").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Logo", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Project").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "EstimatedTime", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Project").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },


                new CustomTableColumn () { Name = "Description", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "EmployeeTask").FirstOrDefault().Id,Key="description", IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                // new CustomTableColumn () { Name = "Due Date", ControlId = _context.CustomControl.Where (t => t.Name == "Date").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "EmployeeTask").FirstOrDefault().Id, IsDefault = true,CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Start Date", ControlId = _context.CustomControl.Where (t => t.Name == "Date").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "EmployeeTask").FirstOrDefault().Id,Key="startDate", IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "End Date", ControlId = _context.CustomControl.Where (t => t.Name == "Date").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "EmployeeTask").FirstOrDefault().Id, Key="endDate", IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Status", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "EmployeeTask").FirstOrDefault().Id,Key="statusId", IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Assignee", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "EmployeeTask").FirstOrDefault().Id,Key="assignee", IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },

                //client
                new CustomTableColumn () { Name = "Name", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "FirstName", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "LastName", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Organization", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "SiteName", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "SiteContactNo", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "SiteAddress1", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "SiteAddress2", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "SiteAddress3", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "PostalCode", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Country", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "State", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "City", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "StandardTimeZone", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                //new CustomTableColumn () { Name = "Active", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "Logo", ControlId = _context.CustomControl.Where (t => t.Name == "TextBox").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new CustomTableColumn () { Name = "InvoiceInterval", ControlId = _context.CustomControl.Where (t => t.Name == "DropDown").FirstOrDefault ().Id, MasterTableId = _context.CustomTable.Where(t => t.Name == "Client").FirstOrDefault().Id, IsDefault = true, CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                };
                foreach (var c in customTableColumnList)
                {
                    var check = _context.CustomTableColumn.FirstOrDefault(p => p.Name == c.Name && p.ControlId == c.ControlId && p.MasterTableId == c.MasterTableId);
                    if (check == null)
                    {
                        _context.CustomTableColumn.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedCustomerType(OneClappContext _context)
        {

            if (_context.CustomerType != null && !_context.CustomerType.Any())
            {
                List<CustomerType> customerTypeList = new List<CustomerType>() {
                new CustomerType () { Name = "Company", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new CustomerType () { Name = "Person", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null }
                };
                customerTypeList.AddRange(customerTypeList);
                _context.CustomerType.AddRange(customerTypeList);
                _context.SaveChanges();
            }
        }

        public static void SeedActivityType(OneClappContext _context)
        {

            if (_context.ActivityType != null && !_context.ActivityType.Any())
            {
                List<ActivityType> activityTypeList = new List<ActivityType>() {
                new ActivityType () { Name = "Call", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new ActivityType () { Name = "Meeting", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new ActivityType () { Name = "Task", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new ActivityType () { Name = "DeadLine", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new ActivityType () { Name = "Email", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new ActivityType () { Name = "Lunch", CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null }
                };
                activityTypeList.AddRange(activityTypeList);
                _context.ActivityType.AddRange(activityTypeList);
                _context.SaveChanges();
            }
        }

        // public static void SeedOrganizationLabel (OneClappContext _context) {

        //     if (_context.OrganizationLabel != null && !_context.OrganizationLabel.Any ()) {
        //         List<OrganizationLabel> tables = new List<OrganizationLabel> () {
        //         new OrganizationLabel () { Name = "CUSTOMER", Color = "green", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new OrganizationLabel () { Name = "HOT LEAD", Color = "red", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new OrganizationLabel () { Name = "WARM LEAD", Color = "#f8cf07", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new OrganizationLabel () { Name = "COLD LEAD", Color = "#13b4ff", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new OrganizationLabel () { Name = "SUPPLIER", Color = "#ab3fdd", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null }
        //         };
        //         tables.AddRange (tables);
        //         _context.OrganizationLabel.AddRange (tables);
        //     }
        // }

        // public static void SeedCustomerLabel (OneClappContext _context) {

        //     if (_context.OrganizationLabel != null && !_context.OrganizationLabel.Any ()) {
        //         List<CustomerLabel> tables = new List<CustomerLabel> () {
        //         new CustomerLabel () { Name = "CUSTOMER", Color = "green", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new CustomerLabel () { Name = "HOT LEAD", Color = "red", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new CustomerLabel () { Name = "WARM LEAD", Color = "#f8cf07", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new CustomerLabel () { Name = "COLD LEAD", Color = "#13b4ff", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
        //         new CustomerLabel () { Name = "SUPPLIER", Color = "#ab3fdd", TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null }
        //         };
        //         tables.AddRange (tables);
        //         _context.CustomerLabel.AddRange (tables);
        //     }
        // }

        public static void SeedEmailPhoneNoType(OneClappContext _context)
        {

            if (_context.EmailPhoneNoType != null && !_context.EmailPhoneNoType.Any())
            {
                List<EmailPhoneNoType> emailPhoneNoTypeList = new List<EmailPhoneNoType>() {
                new EmailPhoneNoType () { Name = "Work", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new EmailPhoneNoType () { Name = "Home", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new EmailPhoneNoType () { Name = "Mobile", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new EmailPhoneNoType () { Name = "Other", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null }
                };
                emailPhoneNoTypeList.AddRange(emailPhoneNoTypeList);
                _context.EmailPhoneNoType.AddRange(emailPhoneNoTypeList);
                _context.SaveChanges();
            }
        }

        // public static void SeedLeadLabel (OneClappContext _context) {

        //     if (_context.LeadLabel != null && !_context.LeadLabel.Any ()) {
        //         List<LeadLabel> tables = new List<LeadLabel> () {
        //         new LeadLabel () { Name = "Hot", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
        //         new LeadLabel () { Name = "Warm", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
        //         new LeadLabel () { Name = "Cold", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null }
        //         };
        //         tables.AddRange (tables);
        //         _context.LeadLabel.AddRange (tables);
        //     }
        // }

        public static void SeedLabelCateGory(OneClappContext _context)
        {

            if (_context.LabelCategory != null && !_context.LabelCategory.Any())
            {
                List<LabelCategory> labelCategoryList = new List<LabelCategory>() {
                new LabelCategory () { Name = "Person", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new LabelCategory () { Name = "Organization", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null },
                new LabelCategory () { Name = "Lead", CreatedOn = DateTime.UtcNow, IsDeleted = false, DeletedOn = null }
                };
                labelCategoryList.AddRange(labelCategoryList);
                _context.LabelCategory.AddRange(labelCategoryList);
                _context.SaveChanges();
            }
        }

        public static void SeedLabel(OneClappContext _context)
        {

            if (_context.Label != null && !_context.Label.Any())
            {
                List<Label> labelList = new List<Label>() {
                new Label () { Name = "CUSTOMER", Color = "green", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Person").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "HOT LEAD", Color = "red", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Person").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "WARM LEAD", Color = "#f8cf07", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Person").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "COLD LEAD", Color = "#13b4ff", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Person").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "SUPPLIER", Color = "#ab3fdd", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Person").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },

                new Label () { Name = "CUSTOMER", Color = "green", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Organization").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "HOT LEAD", Color = "red", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Organization").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "WARM LEAD", Color = "#f8cf07", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Organization").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "COLD LEAD", Color = "#13b4ff", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Organization").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "SUPPLIER", Color = "#ab3fdd", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Organization").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },

                new Label () { Name = "Hot", Color = "red", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Lead").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "Warm", Color = "#f8cf07", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Lead").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },
                new Label () { Name = "Cold", Color = "#13b4ff", LabelCategoryId = _context.LabelCategory.Where (t => t.Name == "Lead").FirstOrDefault ().Id, TenantId = null, CreatedBy = null, CreatedOn = DateTime.UtcNow, UpdatedBy = null, UpdatedOn = null, IsDeleted = false, DeletedOn = null },

                };
                labelList.AddRange(labelList);
                _context.Label.AddRange(labelList);
                _context.SaveChanges();
            }
        }

        public static void SeedActivityAvailability(OneClappContext _context)
        {
            if (_context.ActivityAvailability != null && !_context.ActivityAvailability.Any())
            {
                List<ActivityAvailability> activityAvailabilityList = new List<ActivityAvailability>() {
                new ActivityAvailability () { Name = "Busy" },
                new ActivityAvailability () { Name = "Free" }
                };
                activityAvailabilityList.AddRange(activityAvailabilityList);
                _context.ActivityAvailability.AddRange(activityAvailabilityList);
                _context.SaveChanges();
            }
        }

        public static void SeedCalendarRepeatTypes(OneClappContext _context)
        {
            if (_context.CalendarRepeatType != null && !_context.CalendarRepeatType.Any())
            {
                List<CalendarRepeatType> calendarRepeatTypeList = new List<CalendarRepeatType>() {
                new CalendarRepeatType () { Name = "day" },
                new CalendarRepeatType () { Name = "week" },
                new CalendarRepeatType () { Name = "month" },
                new CalendarRepeatType () { Name = "year" }
                };
                calendarRepeatTypeList.AddRange(calendarRepeatTypeList);
                _context.CalendarRepeatType.AddRange(calendarRepeatTypeList);
                _context.SaveChanges();
            }
        }

        public static void SeedCalendarList(OneClappContext _context)
        {
            if (_context.CalendarList != null && !_context.CalendarList.Any())
            {
                List<CalendarList> calendarList = new List<CalendarList>() {
                new CalendarList () { Name = "My Tasks" }
                };
                calendarList.AddRange(calendarList);
                _context.CalendarList.AddRange(calendarList);
                _context.SaveChanges();
            }
        }

        public static void SeedOneClappLatestThemeLayout(OneClappContext _context)
        {
            if (_context.OneClappLatestThemeLayout != null && !_context.OneClappLatestThemeLayout.Any())
            {
                List<OneClappLatestThemeLayout> oneClappLatestThemeLayoutList = new List<OneClappLatestThemeLayout>() {
                // new OneClappLatestThemeLayout () { Name = "empty", TemplateHtml= "<div class='flex flex-col cursor-pointer' (click)='setLayout('empty')'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80' [class.border-primary]='config.layout === 'empty''><div class='flex flex-col flex-auto bg-gray-50 dark:bg - gray - 900'></div></ div >< div class='mt-2 text-md font-medium text-center text-secondary' [class.text-primary]='config.layout === 'empty''> Empty </div></div>" },
                // new OneClappLatestThemeLayout() { Name = "centered", TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded-md overflow-hidden'><div class='flex items-center h-3 bg-gray-100 dark:bg-gray-800'><div class='flex ml-1.5'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "enterprise" , TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex items-center h-3 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-3 px-2 border-t border-b space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "material", TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex items-center h-4 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-2 px-2 space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "modern", TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex items-center h-4 px-2 border-b bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center h-3 ml-2 space-x-1'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "classic", TemplateHtml="<div class='flex flex-col cursor-pointer'> <div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='mt-3 mx-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div><div class='mt-2 text-md font-medium text-center text-secondary'></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "classy", TemplateHtml="<div class='flex flex-col cursor-pointer' > <div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex items-center mt-1 mx-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-0.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='w-4 h-4 mt-2.5 mx-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='mt-2 mx-1 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:confibg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-2'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "compact", TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-5 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-3 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "dense", TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-4 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "futuristic", TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex flex-col flex-auto h-full py-3 px-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex-auto'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "thin", TemplateHtml="<div lass='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-3 bg-gray-100 dark:bg-gray-800'><div class='w-1.5 h-1.5 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div>div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" }


                // new OneClappLatestThemeLayout() { Name = "modern", CreatedOn = DateTime.UtcNow ,TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex items-center h-4 px-2 border-b bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center h-3 ml-2 space-x-1'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "classic", CreatedOn = DateTime.UtcNow,TemplateHtml="<div class='flex flex-col cursor-pointer'> <div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='mt-3 mx-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div><div class='mt-2 text-md font-medium text-center text-secondary'></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "classy",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer' > <div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex items-center mt-1 mx-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-0.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='w-4 h-4 mt-2.5 mx-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='mt-2 mx-1 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:confibg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-2'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "compact",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-5 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-3 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "dense",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-4 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "futuristic",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex flex-col flex-auto h-full py-3 px-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex-auto'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "thin",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='w-3 bg-gray-100 dark:bg-gray-800'><div class='w-1.5 h-1.5 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout () { Name = "empty",CreatedOn = DateTime.UtcNow, TemplateHtml= "<div class='flex flex-col cursor-pointer' (click)='setLayout('empty')'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80' [class.border-primary]='config.layout === 'empty''><div class='flex flex-col flex-auto bg-gray-50 dark:bg - gray - 900'></div></ div >< div class='mt-2 text-md font-medium text-center text-secondary' [class.text-primary]='config.layout === 'empty''> Empty </div></div>" ,IsDeleted = true},
                // new OneClappLatestThemeLayout() { Name = "centered",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded-md overflow-hidden'><div class='flex items-center h-3 bg-gray-100 dark:bg-gray-800'><div class='flex ml-1.5'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" ,IsDeleted = true},
                // new OneClappLatestThemeLayout() { Name = "enterprise" ,CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex items-center h-3 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-3 px-2 border-t border-b space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div></div>" ,IsDeleted = true },
                // new OneClappLatestThemeLayout() { Name = "material",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden border-2 hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex items-center h-4 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-2 px-2 space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" ,IsDeleted = true},

                // update on date:29-09-2021
                // new OneClappLatestThemeLayout() { Name = "modern", CreatedOn = DateTime.UtcNow ,TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex items-center h-4 px-2 border-b bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center h-3 ml-2 space-x-1'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "classic", CreatedOn = DateTime.UtcNow,TemplateHtml=" <div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden   hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='mt-3 mx-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "classy",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer' > <div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex items-center mt-1 mx-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-0.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='w-4 h-4 mt-2.5 mx-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='mt-2 mx-1 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:confibg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-2'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "compact",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-5 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-3 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "dense",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-4 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "futuristic",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex flex-col flex-auto h-full py-3 px-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex-auto'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "thin",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-3 bg-gray-100 dark:bg-gray-800'><div class='w-1.5 h-1.5 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout () { Name = "empty",CreatedOn = DateTime.UtcNow, TemplateHtml= "<div class='flex flex-col cursor-pointer' (click)='setLayout('empty')'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80' [class.border-primary]='config.layout === 'empty''><div class='flex flex-col flex-auto bg-gray-50 dark:bg - gray - 900'></div></ div >< div class='mt-2 text-md font-medium text-center text-secondary' [class.text-primary]='config.layout === 'empty''> Empty </div></div>" ,IsDeleted = true},
                // new OneClappLatestThemeLayout() { Name = "centered",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded-md overflow-hidden'><div class='flex items-center h-3 bg-gray-100 dark:bg-gray-800'><div class='flex ml-1.5'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" ,IsDeleted = true},
                // new OneClappLatestThemeLayout() { Name = "enterprise" ,CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex items-center h-3 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-3 px-2 border-t border-b space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div></div>" ,IsDeleted = true },
                // new OneClappLatestThemeLayout() { Name = "material",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex items-center h-4 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-2 px-2 space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" ,IsDeleted = true},

                // update on date:30-09-2021
                new OneClappLatestThemeLayout() { Name = "modern", CreatedOn = DateTime.UtcNow ,TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex items-center h-4 px-2 border-b bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center h-3 ml-2 space-x-1'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "classic", CreatedOn = DateTime.UtcNow,TemplateHtml=" <div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden   hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='mt-3 mx-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" },
                // new OneClappLatestThemeLayout() { Name = "classy",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer' > <div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex items-center mt-1 mx-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-0.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='w-4 h-4 mt-2.5 mx-auto rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='mt-2 mx-1 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-2'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                new OneClappLatestThemeLayout() { Name = "compact",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-5 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-3 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-2.5 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                new OneClappLatestThemeLayout() { Name = "dense",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-4 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='w-2 h-2 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout() { Name = "futuristic",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-8 bg-gray-100 dark:bg-gray-800'><div class='flex flex-col flex-auto h-full py-3 px-1.5 space-y-1'><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex-auto'></div><div class='h-1 rounded-sm bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                new OneClappLatestThemeLayout() { Name = "thin",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='w-3 bg-gray-100 dark:bg-gray-800'><div class='w-1.5 h-1.5 mt-2 mx-auto rounded-sm bg-gray-300 dark:bg-gray-700'></div><div class='flex flex-col items-center w-full mt-2 space-y-1'><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1.5 h-1.5 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-col flex-auto border-l'><div class='h-3 bg-gray-100 dark:bg-gray-800'><div class='flex items-center justify-end h-full mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div></div>" },
                // new OneClappLatestThemeLayout () { Name = "empty",CreatedOn = DateTime.UtcNow, TemplateHtml= "<div class='flex flex-col cursor-pointer' (click)='setLayout('empty')'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80' [class.border-primary]='config.layout === 'empty''><div class='flex flex-col flex-auto bg-gray-50 dark:bg - gray - 900'></div></ div >< div class='mt-2 text-md font-medium text-center text-secondary' [class.text-primary]='config.layout === 'empty''> Empty </div></div>" ,IsDeleted = true},
                // new OneClappLatestThemeLayout() { Name = "centered",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded-md overflow-hidden'><div class='flex items-center h-3 bg-gray-100 dark:bg-gray-800'><div class='flex ml-1.5'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex items-center justify-end ml-auto mr-1.5'><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 ml-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" ,IsDeleted = true},
                // new OneClappLatestThemeLayout() { Name = "enterprise" ,CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex items-center h-3 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-3 px-2 border-t border-b space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex flex-auto bg-gray-50 dark:bg-gray-900'></div></div></div></div>" ,IsDeleted = true },
                // new OneClappLatestThemeLayout() { Name = "material",CreatedOn = DateTime.UtcNow, TemplateHtml="<div class='flex flex-col cursor-pointer'><div class='flex flex-col h-20 rounded-md overflow-hidden  hover:opacity-80'><div class='flex flex-col flex-auto my-1 mx-2 border rounded overflow-hidden'><div class='flex items-center h-4 px-2 bg-gray-100 dark:bg-gray-800'><div class='w-2 h-2 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='flex items-center justify-end ml-auto space-x-1'><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-1 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div></div><div class='flex items-center h-2 px-2 space-x-1 bg-gray-100 dark:bg-gray-800'><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div><div class='w-3 h-1 rounded-full bg-gray-300 dark:bg-gray-700'></div></div><div class='flex flex-auto border-t bg-gray-50 dark:bg-gray-900'></div></div></div> </div>" ,IsDeleted = true},
            };
                oneClappLatestThemeLayoutList.AddRange(oneClappLatestThemeLayoutList);
                _context.OneClappLatestThemeLayout.AddRange(oneClappLatestThemeLayoutList);
                _context.SaveChanges();
            }
        }

        public static void SeedOneClappLatestThemeScheme(OneClappContext _context)
        {
            if (_context.OneClappLatestThemeScheme != null && !_context.OneClappLatestThemeScheme.Any())
            {
                List<OneClappLatestThemeScheme> oneClappLatestThemeSchemeList = new List<OneClappLatestThemeScheme>() {
                // new OneClappLatestThemeScheme () { Name = "auto" , TemplateHtml="<div class='flex items-center py-3 pl-5 pr-6 rounded-full cursor-pointer ring-inset ring-primary bg-hover' [class.ring-2]='config.scheme === 'auto'' [matTooltip]=''Automatically sets the scheme based on user\'s operating system\'s color scheme preference using \'prefer-color-scheme\' media query.'' (click)='setScheme('auto')'> <div class='flex items-center rounded-full overflow-hidden'> <mat-icon class='icon-size-5' [svgIcon]=''heroicons_solid:lightning-bolt''></mat-icon> </div> <div class='flex items-center ml-2 font-medium leading-5' [class.text-secondary]='config.scheme !== 'auto''> Auto </div></div>"},
                // new OneClappLatestThemeScheme () { Name = "dark" , TemplateHtml="<div class='flex items-center py-3 pl-5 pr-6 rounded-full cursor-pointer ring-inset ring-primary bg-hover' [class.ring-2]='config.scheme === 'dark'' (click)='setScheme('dark')'> <div class='flex items-center rounded-full overflow-hidden'> <mat-icon class='icon-size-5' [svgIcon]=''heroicons_solid:moon''></mat-icon> </div> <div class='flex items-center ml-2 font-medium leading-5' [class.text-secondary]='config.scheme !== 'dark''> Dark </div> </div>"},
                // new OneClappLatestThemeScheme () { Name = "light", TemplateHtml="<div class='flex items-center py-3 pl-5 pr-6 rounded-full cursor-pointer ring-inset ring-primary bg-hover'[class.ring-2]='config.scheme === 'light'' (click)='setScheme('light')'><div class='flex items-center rounded-full overflow-hidden'><mat-icon class='icon-size-5' [svgIcon]=''heroicons_solid:sun''></mat-icon></div><div class='flex items-center ml-2 font-medium leading-5' [class.text-secondary]='config.scheme !== 'light''>Light</div></div>" }

                new OneClappLatestThemeScheme () { Name = "auto" , TemplateHtml="<div class='flex items-center py-3 pl-5 pr-6 rounded-full cursor-pointer ring-inset ring-primary bg-hover' [matTooltip]='Automatically sets the scheme based on user's operating system's color scheme preference using prefer-color-scheme media query.' ><div class='flex items-center rounded-full overflow-hidden'><mat-icon class='icon-size-5' [svgIcon]='heroicons_solid:lightning-bolt'></mat-icon></div> </div>"},
                new OneClappLatestThemeScheme () { Name = "dark" , TemplateHtml="<div class='flex items-center py-3 pl-5 pr-6 rounded-full cursor-pointer ring-inset ring-primary bg-hover' > <div class='flex items-center rounded-full overflow-hidden'><mat-icon class='icon-size-5' [svgIcon]='heroicons_solid:moon'></mat-icon> </div> </div>"},
                new OneClappLatestThemeScheme () { Name = "light", TemplateHtml="<div class='flex items-center py-3 pl-5 pr-6 rounded-full cursor-pointer ring-inset ring-primary bg-hover'><div class='flex items-center rounded-full overflow-hidden'><mat-icon class='icon-size-5' [svgIcon]='heroicons_solid:sun'></mat-icon></div></div>" },
                new OneClappLatestThemeScheme () { Name = "purple", TemplateHtml="<div class='flex items-center py-3 pl-5 pr-6 rounded-full cursor-pointer ring-inset ring-primary bg-hover'><div class='flex items-center rounded-full overflow-hidden'><mat-icon class='icon-size-5' [svgIcon]='heroicons_solid:fire'></mat-icon></div></div>" }
                };
                oneClappLatestThemeSchemeList.AddRange(oneClappLatestThemeSchemeList);
                _context.OneClappLatestThemeScheme.AddRange(oneClappLatestThemeSchemeList);
                _context.SaveChanges();
            }
        }

        public static void SeedOneClappLatestTheme(OneClappContext _context)
        {
            if (_context.OneClappLatestTheme != null && !_context.OneClappLatestTheme.Any())
            {
                List<OneClappLatestTheme> oneClappLatestThemeList = new List<OneClappLatestTheme>() {
                new OneClappLatestTheme () { Name = "default", Primary = "#4f46e5", Accent = "#1e293b", Warn = "#dc2626" },
                new OneClappLatestTheme () { Name = "brand", Primary = "#2196f3", Accent = "#2196f3", Warn = "#dc2626" },
                new OneClappLatestTheme () { Name = "indigo", Primary = "#0d9488", Accent = "#1e293b", Warn = "#dc2626" },
                new OneClappLatestTheme () { Name = "rose", Primary = "#f43f5e", Accent = "#1e293b", Warn = "#dc2626" },
                new OneClappLatestTheme () { Name = "purple", Primary = "#9333ea", Accent = "#1e293b", Warn = "#dc2626" },
                new OneClappLatestTheme () { Name = "amber", Primary = "#f59e0b", Accent = "#1e293b", Warn = "#dc2626" }
                };
                oneClappLatestThemeList.AddRange(oneClappLatestThemeList);
                _context.OneClappLatestTheme.AddRange(oneClappLatestThemeList);
                _context.SaveChanges();
            }
        }

        public static void SeedIntProviders(OneClappContext _context)
        {
            if (_context.IntProvider != null && !_context.IntProvider.Any())
            {
                List<IntProvider> intProviderList = new List<IntProvider>() {
                new IntProvider () { Name = "Google", CreatedOn = DateTime.UtcNow },
                new IntProvider () { Name = "Microsoft", CreatedOn = DateTime.UtcNow }
                };
                intProviderList.AddRange(intProviderList);
                _context.IntProvider.AddRange(intProviderList);
                _context.SaveChanges();
            }
        }

        public static void SeedIntProviderApps(OneClappContext _context)
        {
            var data = _context.IntProvider.Where(t => t.IsDeleted == false).ToList();
            if (_context.IntProviderApp != null)
            {
                List<IntProviderApp> intProviderAppList = new List<IntProviderApp>() {
                new IntProviderApp () { Name = "Calendar",IntProviderId=_context.IntProvider.Where(t => t.Name == "Google").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new IntProviderApp () { Name = "Calendar",IntProviderId=_context.IntProvider.Where(t => t.Name == "Microsoft").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new IntProviderApp () { Name = "Gmail",IntProviderId=_context.IntProvider.Where(t => t.Name == "Google").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new IntProviderApp () { Name = "Outlook",IntProviderId=_context.IntProvider.Where(t => t.Name == "Microsoft").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new IntProviderApp () { Name = "Contact",IntProviderId=_context.IntProvider.Where(t => t.Name == "Google").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new IntProviderApp () { Name = "Contact",IntProviderId=_context.IntProvider.Where(t => t.Name == "Microsoft").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                };
                foreach (var c in intProviderAppList)
                {
                    var check = _context.IntProviderApp.FirstOrDefault(p => p.Name == c.Name && p.IntProviderId == c.IntProviderId);
                    if (check == null)
                    {
                        _context.IntProviderApp.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedOneClappModules(OneClappContext _context)
        {
            if (_context.OneClappModule != null && !_context.OneClappModule.Any())
            {
                List<OneClappModule> oneClappModuleList = new List<OneClappModule>() {
                new OneClappModule () { Name = "CRM",CreatedBy=_context.Users.Where (t => t.Email == "super.admin@oneclapp.com").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow },
                new OneClappModule () { Name = "FormBuilder",CreatedBy=_context.Users.Where (t => t.Email == "super.admin@oneclapp.com").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow }
                };
                oneClappModuleList.AddRange(oneClappModuleList);
                _context.OneClappModule.AddRange(oneClappModuleList);
                _context.SaveChanges();
            }
        }

        public static void SeedOneClappFormTypes(OneClappContext _context)
        {
            if (_context.OneClappFormType != null && !_context.OneClappFormType.Any())
            {
                List<OneClappFormType> oneClappFormTypeList = new List<OneClappFormType>() {
                new OneClappFormType () { Name = "Simple Form", CreatedOn = DateTime.UtcNow },
                new OneClappFormType () { Name = "Custom API Source", CreatedOn = DateTime.UtcNow }
                };
                oneClappFormTypeList.AddRange(oneClappFormTypeList);
                _context.OneClappFormType.AddRange(oneClappFormTypeList);
                _context.SaveChanges();
            }
        }

        public static void SeedOneClappFormStatus(OneClappContext _context)
        {
            if (_context.OneClappFormStatus != null && !_context.OneClappFormStatus.Any())
            {
                List<OneClappFormStatus> oneClappFormStatusList = new List<OneClappFormStatus>() {
                new OneClappFormStatus () { Name = "Verify", CreatedOn = DateTime.UtcNow },
                new OneClappFormStatus () { Name = "Skip", CreatedOn = DateTime.UtcNow }
                };
                oneClappFormStatusList.AddRange(oneClappFormStatusList);
                _context.OneClappFormStatus.AddRange(oneClappFormStatusList);
                _context.SaveChanges();
            }
        }

        public static void SeedOneClappFormAction(OneClappContext _context)
        {
            if (_context.OneClappFormAction != null && !_context.OneClappFormAction.Any())
            {
                List<OneClappFormAction> oneClappFormActionList = new List<OneClappFormAction>() {
                new OneClappFormAction () { Name = "Person", CreatedOn = DateTime.UtcNow },
                new OneClappFormAction () { Name = "Organization", CreatedOn = DateTime.UtcNow },
                new OneClappFormAction () { Name = "Lead", CreatedOn = DateTime.UtcNow }
                };
                oneClappFormActionList.AddRange(oneClappFormActionList);
                _context.OneClappFormAction.AddRange(oneClappFormActionList);
                _context.SaveChanges();
            }
        }

        public static void SeedSubscriptionPlan(OneClappContext _context)
        {
            if (_context.SubscriptionPlan != null && !_context.SubscriptionPlan.Any())
            {
                List<SubscriptionPlan> subscriptionPlanList = new List<SubscriptionPlan>() {
                new SubscriptionPlan () { Name = "Basic", MonthlyPrice = 9, YearlyPrice=7, TrialPeriod=30, CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new SubscriptionPlan () { Name = "Business",  TrialPeriod=30, CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new SubscriptionPlan () { Name = "Enterprise",  TrialPeriod=30, CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow }
                };
                subscriptionPlanList.AddRange(subscriptionPlanList);
                _context.SubscriptionPlan.AddRange(subscriptionPlanList);
                _context.SaveChanges();
            }
        }

        public static void SeedSubscriptionPlanDetail(OneClappContext _context)
        {
            if (_context.SubscriptionPlanDetail != null && !_context.SubscriptionPlanDetail.Any())
            {
                List<SubscriptionPlanDetail> subscriptionPlanDetailList = new List<SubscriptionPlanDetail>() {
                new SubscriptionPlanDetail () { SubScriptionPlanId = _context.SubscriptionPlan.Where(t => t.Name == "Basic").FirstOrDefault().Id, FeatureName = "50GB storage", Description="50GB storage", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new SubscriptionPlanDetail () { SubScriptionPlanId = _context.SubscriptionPlan.Where(t => t.Name == "Basic").FirstOrDefault().Id, FeatureName = "Calendar integration", Description="Calendar integration", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow }
                // new SubscriptionPlanDetail () { SubScriptionPlanId = _context.SubscriptionPlan.Where(t => t.Name == "Business").FirstOrDefault().Id, FeatureName = "100GB storage", Description="100GB storage", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow }
                };
                subscriptionPlanDetailList.AddRange(subscriptionPlanDetailList);
                _context.SubscriptionPlanDetail.AddRange(subscriptionPlanDetailList);
                _context.SaveChanges();
            }
        }

        public static void SeedSubscriptionType(OneClappContext _context)
        {
            if (_context.SubscriptionType != null && !_context.SubscriptionType.Any())
            {
                List<SubscriptionType> subscriptionTypeList = new List<SubscriptionType>() {
                new SubscriptionType () { Name = "Yearly", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new SubscriptionType () { Name = "Monthly", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow }
                };
                subscriptionTypeList.AddRange(subscriptionTypeList);
                _context.SubscriptionType.AddRange(subscriptionTypeList);
                _context.SaveChanges();
            }
        }

        public static void SeedSalutation(OneClappContext _context)
        {
            if (_context.Salutation != null && !_context.Salutation.Any())
            {
                List<Salutation> salutationList = new List<Salutation>() {
                new Salutation () { Name = "Mr.", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new Salutation () { Name = "Mrs.", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new Salutation () { Name = "Ms.", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new Salutation () { Name = "Miss", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new Salutation () { Name = "Dr.", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow },
                new Salutation () { Name = "Professor", CreatedBy=_context.Users.Where(t => t.Email == "super.admin@oneclapp.com").FirstOrDefault().Id, CreatedOn = DateTime.UtcNow }
                };
                salutationList.AddRange(salutationList);
                _context.Salutation.AddRange(salutationList);
                _context.SaveChanges();
            }
        }

        public static void SeedBorderStyle(OneClappContext _context)
        {
            if (_context.BorderStyle != null && !_context.BorderStyle.Any())
            {
                List<BorderStyle> borderStyleList = new List<BorderStyle>() {
                new BorderStyle () { Name = "dashed",  CreatedOn = DateTime.UtcNow },
                new BorderStyle () { Name = "dotted", CreatedOn = DateTime.UtcNow },
                new BorderStyle () { Name = "solid",CreatedOn = DateTime.UtcNow },
                new BorderStyle () { Name = "none", CreatedOn = DateTime.UtcNow }
                };
                borderStyleList.AddRange(borderStyleList);
                _context.BorderStyle.AddRange(borderStyleList);
                _context.SaveChanges();
            }
        }

        public static void SeedCountry(OneClappContext _context)
        {
            if (_context.Country != null && !_context.Country.Any())
            {
                List<Country> countryList = new List<Country>() {
                new Country () { Name = "India",  CreatedOn = DateTime.UtcNow },
                new Country () { Name = "Germany", CreatedOn = DateTime.UtcNow },
                };
                countryList.AddRange(countryList);
                _context.Country.AddRange(countryList);
                _context.SaveChanges();
            }
        }

        public static void SeedState(OneClappContext _context)
        {
            if (_context.State != null && !_context.State.Any())
            {
                List<State> stateList = new List<State>() {
                new State () { Name = "Andhra Pradesh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Andaman and Nicobar Islands", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Arunachal Pradesh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Assam", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Bihar", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Chhattisgarh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Chandigarh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Delhi", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Dadra and Nagar Haveli and Daman and Diu", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Goa", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Gujarat", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Haryana", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Himachal Pradesh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Jharkhand", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Jammu and Kashmir", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Karnataka", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Kerala", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Ladakh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Lakshadweep", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Lakshadweep", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Madhya Pradesh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Maharashtra", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Manipur", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Meghalaya", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Mizoram", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Nagaland", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Odisha", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Punjab", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Puducherry", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Rajasthan", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Sikkim", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Tamil Nadu", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Telangana", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Tripura", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Uttar Pradesh", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Uttarakhand", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "West Bengal", CountryId = _context.Country.Where (t => t.Name == "India").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Baden-Württemberg", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Bavaria", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Berlin", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Brandenburg", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Bremen", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Hamburg", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Hesse", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Lower Saxony", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Mecklenburg-Vorpommern", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "North Rhine-Westphalia", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Rhineland-Palatinate", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Saarland", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Saxony", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Saxony-Anhalt", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Schleswig-Holstein", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new State () { Name = "Thuringia", CountryId = _context.Country.Where (t => t.Name == "Germany").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                };
                stateList.AddRange(stateList);
                _context.State.AddRange(stateList);
                _context.SaveChanges();
            }
        }

        public static void SeedCity(OneClappContext _context)
        {
            if (_context.City != null)
            {
                List<City> cityList = new List<City>() {
                new City () { Name = "Ahmedabad", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Surat", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Vadodara", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Rajkot", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Bhavnagar", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Jamnagar", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Gandhinagar", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Junagadh", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Gandhidham", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Anand", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Navsari", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Morbi", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Nadiad", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Surendranagar", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Bharuch", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Mehsana", StateId = _context.State.Where (t => t.Name == "Gujarat").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Abenberg", StateId = _context.State.Where (t => t.Name == "Bavaria").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Aichach", StateId = _context.State.Where (t => t.Name == "Bavaria").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Aub", StateId = _context.State.Where (t => t.Name == "Bavaria").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Nürnberg", StateId = _context.State.Where (t => t.Name == "Bavaria").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Visakhapatnam", StateId = _context.State.Where (t => t.Name == "Andhra Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Anantapur", StateId = _context.State.Where (t => t.Name == "Andhra Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Port Blair", StateId = _context.State.Where (t => t.Name == "Andaman and Nicobar Islands").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Mayabunder", StateId = _context.State.Where (t => t.Name == "Andaman and Nicobar Islands").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Changlang", StateId = _context.State.Where (t => t.Name == "Arunachal Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Tirap", StateId = _context.State.Where (t => t.Name == "Arunachal Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Guwahati", StateId = _context.State.Where (t => t.Name == "Assam").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Tezpur", StateId = _context.State.Where (t => t.Name == "Assam").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Patna", StateId = _context.State.Where (t => t.Name == "Bihar").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Muzaffarpur", StateId = _context.State.Where (t => t.Name == "Bihar").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Raipur", StateId = _context.State.Where (t => t.Name == "Chhattisgarh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Bilaspur", StateId = _context.State.Where (t => t.Name == "Chhattisgarh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Gharoli", StateId = _context.State.Where (t => t.Name == "Delhi").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Burari", StateId = _context.State.Where (t => t.Name == "Delhi").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Silvassa", StateId = _context.State.Where (t => t.Name == "Dadra and Nagar Haveli and Daman and Diu").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Panaji", StateId = _context.State.Where (t => t.Name == "Goa").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Mapusa", StateId = _context.State.Where (t => t.Name == "Goa").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Gurugram", StateId = _context.State.Where (t => t.Name == "Haryana").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Rohtak", StateId = _context.State.Where (t => t.Name == "Haryana").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Shimla", StateId = _context.State.Where (t => t.Name == "Himachal Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Manali", StateId = _context.State.Where (t => t.Name == "Himachal Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Ranchi", StateId = _context.State.Where (t => t.Name == "Jharkhand").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Jamshedpur", StateId = _context.State.Where (t => t.Name == "Jharkhand").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Srinagar", StateId = _context.State.Where (t => t.Name == "Jammu and Kashmir").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Anantnag", StateId = _context.State.Where (t => t.Name == "Jammu and Kashmir").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Mysuru", StateId = _context.State.Where (t => t.Name == "Karnataka").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kalaburagi", StateId = _context.State.Where (t => t.Name == "Karnataka").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kochi", StateId = _context.State.Where (t => t.Name == "Kerala").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Thrissur", StateId = _context.State.Where (t => t.Name == "Kerala").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Leh", StateId = _context.State.Where (t => t.Name == "Ladakh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kargil", StateId = _context.State.Where (t => t.Name == "Ladakh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kavaratti", StateId = _context.State.Where (t => t.Name == "Lakshadweep").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Minicoy Island", StateId = _context.State.Where (t => t.Name == "Lakshadweep").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Bhopal", StateId = _context.State.Where (t => t.Name == "Madhya Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Indore", StateId = _context.State.Where (t => t.Name == "Madhya Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Mumbai", StateId = _context.State.Where (t => t.Name == "Maharashtra").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Pune", StateId = _context.State.Where (t => t.Name == "Maharashtra").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Imphal", StateId = _context.State.Where (t => t.Name == "Manipur").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Thoubal", StateId = _context.State.Where (t => t.Name == "Manipur").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Shillong", StateId = _context.State.Where (t => t.Name == "Meghalaya").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Cherrapunji", StateId = _context.State.Where (t => t.Name == "Meghalaya").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Champhai", StateId = _context.State.Where (t => t.Name == "Mizoram").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Aizawl", StateId = _context.State.Where (t => t.Name == "Mizoram").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kohima", StateId = _context.State.Where (t => t.Name == "Nagaland").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Dimapur", StateId = _context.State.Where (t => t.Name == "Nagaland").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Bhubaneswar", StateId = _context.State.Where (t => t.Name == "Odisha").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Brahmapur", StateId = _context.State.Where (t => t.Name == "Odisha").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Amritsar", StateId = _context.State.Where (t => t.Name == "Punjab").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Jalandhar", StateId = _context.State.Where (t => t.Name == "Punjab").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Mahe", StateId = _context.State.Where (t => t.Name == "Puducherry").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Karaikal", StateId = _context.State.Where (t => t.Name == "Puducherry").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Jaipur", StateId = _context.State.Where (t => t.Name == "Rajasthan").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Jodhpur", StateId = _context.State.Where (t => t.Name == "Rajasthan").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Gangtok", StateId = _context.State.Where (t => t.Name == "Sikkim").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Namchi", StateId = _context.State.Where (t => t.Name == "Sikkim").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Chennai", StateId = _context.State.Where (t => t.Name == "Tamil Nadu").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Coimbatore", StateId = _context.State.Where (t => t.Name == "Tamil Nadu").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Hyderabad", StateId = _context.State.Where (t => t.Name == "Telangana").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Warangal", StateId = _context.State.Where (t => t.Name == "Telangana").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Agartala", StateId = _context.State.Where (t => t.Name == "Tripura").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Belonia", StateId = _context.State.Where (t => t.Name == "Tripura").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Lucknow", StateId = _context.State.Where (t => t.Name == "Uttar Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kanpur", StateId = _context.State.Where (t => t.Name == "Uttar Pradesh").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Dehradun", StateId = _context.State.Where (t => t.Name == "Uttarakhand").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Nainital", StateId = _context.State.Where (t => t.Name == "Uttarakhand").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kolkata", StateId = _context.State.Where (t => t.Name == "West Bengal").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Siliguri", StateId = _context.State.Where (t => t.Name == "West Bengal").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Baden-Baden", StateId = _context.State.Where (t => t.Name == "Baden-Württemberg").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Stuttgart", StateId = _context.State.Where (t => t.Name == "Baden-Württemberg").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Cottbus", StateId = _context.State.Where (t => t.Name == "Brandenburg").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Potsdam", StateId = _context.State.Where (t => t.Name == "Brandenburg").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Frankfurt", StateId = _context.State.Where (t => t.Name == "Hesse").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Hanau", StateId = _context.State.Where (t => t.Name == "Hesse").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Hanover", StateId = _context.State.Where (t => t.Name == "Lower Saxony").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Brunswick", StateId = _context.State.Where (t => t.Name == "Lower Saxony").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Schwerin", StateId = _context.State.Where (t => t.Name == "Mecklenburg-Vorpommern").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Rostock", StateId = _context.State.Where (t => t.Name == "Mecklenburg-Vorpommern").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Essen", StateId = _context.State.Where (t => t.Name == "North Rhine-Westphalia").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Bonn", StateId = _context.State.Where (t => t.Name == "North Rhine-Westphalia").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Mainz", StateId = _context.State.Where (t => t.Name == "Rhineland-Palatinate").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Koblenz", StateId = _context.State.Where (t => t.Name == "Rhineland-Palatinate").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Saarbrücken", StateId = _context.State.Where (t => t.Name == "Saarland").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Völklingen", StateId = _context.State.Where (t => t.Name == "Saarland").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Dresden", StateId = _context.State.Where (t => t.Name == "Saxony").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Leipzig", StateId = _context.State.Where (t => t.Name == "Saxony").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Magdeburg", StateId = _context.State.Where (t => t.Name == "Saxony-Anhalt").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Halle", StateId = _context.State.Where (t => t.Name == "Saxony-Anhalt").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Kiel", StateId = _context.State.Where (t => t.Name == "Schleswig-Holstein").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Lübeck", StateId = _context.State.Where (t => t.Name == "Schleswig-Holstein").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Erfurt", StateId = _context.State.Where (t => t.Name == "Thuringia").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                new City () { Name = "Weimar", StateId = _context.State.Where (t => t.Name == "Thuringia").FirstOrDefault ().Id, CreatedOn = DateTime.UtcNow, DeletedOn = null },
                };

                foreach (var c in cityList)
                {
                    var check = _context.City.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.City.Add(c);
                        _context.SaveChanges();
                    }
                }
                // cityList.AddRange(cityList);
                // _context.City.AddRange(cityList);
                // _context.SaveChanges();
            }
        }
        public static void SeedTimeZone(OneClappContext _context)
        {
            if (_context.StandardTimeZone != null && !_context.StandardTimeZone.Any())
            {
                List<StandardTimeZone> standardTimeZoneList = new List<StandardTimeZone>() {
                new StandardTimeZone () { Name = "Dateline Standard Time", Time = "(GMT-12:00) International Date Line West", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Samoa Standard Time", Time = "(GMT-11:00) Midway Island, Samoa", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Hawaiian Standard Time", Time = "(GMT-10:00) Hawaii", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Alaskan Standard Time", Time = "(GMT-09:00) Alaska", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Pacific Standard Time", Time = "(GMT-08:00) Pacific Time (US and Canada)", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Mountain Standard Time", Time = "(GMT-07:00) Mountain Time (US and Canada)", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Mexico Standard Time 2", Time = "(GMT-07:00) Chihuahua, La Paz, Mazatlan", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "U.S. Mountain Standard Time", Time = "(GMT-07:00) Arizona", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Central Standard Time", Time = "(GMT-06:00) Central Time (US and Canada)", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Canada Central Standard Time", Time = "(GMT-06:00) Saskatchewan", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Mexico Standard Time", Time = "(GMT-06:00) Guadalajara, Mexico City, Monterrey", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Central America Standard Time", Time = "(GMT-06:00) Central America", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Eastern Standard Time", Time = "(GMT-05:00) Eastern Time (US and Canada)", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "U.S. Eastern Standard Time", Time = "(GMT-05:00) Indiana (East)", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "S.A. Pacific Standard Time", Time = "(GMT-05:00) Bogota, Lima, Quito", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Atlantic Standard Time", Time = "(GMT-04:00) Atlantic Time (Canada)", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "S.A. Western Standard Time", Time = "(GMT-04:00) Caracas, La Paz", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Pacific S.A. Standard Time", Time = "(GMT-04:00) Santiago", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Newfoundland and Labrador Standard Time", Time = "(GMT-03:30) Newfoundland and Labrador", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "E. South America Standard Time", Time = "(GMT-03:00) Brasilia", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "S.A. Eastern Standard Time", Time = "(GMT-03:00) Buenos Aires, Georgetown", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Greenland Standard Time", Time = "(GMT-03:00) Greenland", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Mid-Atlantic Standard Time", Time = "(GMT-02:00) Mid-Atlantic", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Azores Standard Time", Time = "(GMT-01:00) Azores", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Cape Verde Standard Time", Time = "(GMT-01:00) Cape Verde Islands", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "GMT Standard Time", Time = "GMT) Greenwich Mean Time: Dublin, Edinburgh, Lisbon, London", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Greenwich Standard Time", Time = "(GMT) Casablanca, Monrovia", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Central Europe Standard Time", Time = "(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Central European Standard Time", Time = "(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Romance Standard Time", Time = "(GMT+01:00) Brussels, Copenhagen, Madrid, Paris", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "W. Europe Standard Time", Time = "(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "W. Central Africa Standard Time", Time = "(GMT+01:00) West Central Africa", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "E. Europe Standard Time", Time = "(GMT+02:00) Bucharest", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Egypt Standard Time", Time = "(GMT+02:00) Cairo", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "FLE Standard Time", Time = "(GMT+02:00) Helsinki, Kiev, Riga, Sofia, Tallinn, Vilnius", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "GTB Standard Time", Time = "(GMT+02:00) Athens, Istanbul, Minsk", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Israel Standard Time", Time = "(GMT+02:00) Jerusalem", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "South Africa Standard Time", Time = "(GMT+02:00) Harare, Pretoria", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Russian Standard Time", Time = "(GMT+03:00) Moscow, St. Petersburg, Volgograd", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Arab Standard Time", Time = "(GMT+03:00) Kuwait, Riyadh", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "E. Africa Standard Time", Time = "(GMT+03:00) Nairobi", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Arabic Standard Time", Time = "(GMT+03:00) Baghdad", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Iran Standard Time", Time = "(GMT+03:30) Tehran", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Arabian Standard Time", Time = "(GMT+04:00) Abu Dhabi, Muscat", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Caucasus Standard Time", Time = "s(GMT+04:00) Baku, Tbilisi, Yerevan", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Transitional Islamic State of Afghanistan Standard Time", Time = "(GMT+04:30) Kabul", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Ekaterinburg Standard Time", Time = "(GMT+05:00) Ekaterinburg", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "West Asia Standard Time", Time = "(GMT+05:00) Islamabad, Karachi, Tashkent", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "India Standard Time", Time = "(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Nepal Standard Time", Time = "(GMT+05:45) Kathmandu", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Central Asia Standard Time", Time = "(GMT+06:00) Astana, Dhaka", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Sri Lanka Standard Time", Time = "(GMT+06:00) Sri Jayawardenepura", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "N. Central Asia Standard Time", Time = "(GMT+06:00) Almaty, Novosibirsk", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Myanmar Standard Time", Time = "(GMT+06:30) Yangon Rangoon", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "S.E. Asia Standard Time", Time = "(GMT+07:00) Bangkok, Hanoi, Jakarta", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "North Asia Standard Time", Time = "(GMT+07:00) Krasnoyarsk", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "China Standard Time", Time = "(GMT+08:00) Beijing, Chongqing, Hong Kong SAR, Urumqi", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Singapore Standard Time", Time = "(GMT+08:00) Kuala Lumpur, Singapore", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Taipei Standard Time", Time = "(GMT+08:00) Taipei", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "W. Australia Standard Time", Time = "(GMT+08:00) Perth", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "North Asia East Standard Time", Time = "(GMT+08:00) Irkutsk, Ulaanbaatar", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Korea Standard Time", Time = "(GMT+09:00) Seoul", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Tokyo Standard Time", Time = "(GMT+09:00) Osaka, Sapporo, Tokyo", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Yakutsk Standard Time", Time = "(GMT+09:00) Yakutsk", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "A.U.S. Central Standard Time", Time = "(GMT+09:30) Darwin", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Cen. Australia Standard Time", Time = "(GMT+09:30) Adelaide", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "A.U.S. Eastern Standard Time", Time = "(GMT+10:00) Canberra, Melbourne, Sydney", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "E. Australia Standard Time", Time = "(GMT+10:00) Brisbane", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Tasmania Standard Time", Time = "(GMT+10:00) Hobart", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Vladivostok Standard Time", Time = "(GMT+10:00) Vladivostok", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "West Pacific Standard Time", Time = "(GMT+10:00) Guam, Port Moresby", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Central Pacific Standard Time", Time = "(GMT+11:00) Magadan, Solomon Islands, New Caledonia", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Fiji Islands Standard Time", Time = "(GMT+12:00) Fiji Islands, Kamchatka, Marshall Islands", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "New Zealand Standard Time", Time = "(GMT+12:00) Auckland, Wellington", CreatedOn = DateTime.UtcNow },
                new StandardTimeZone () { Name = "Tonga Standard Time", Time = "(GMT+13:00) Nuku'alofa", CreatedOn = DateTime.UtcNow },
                };
                standardTimeZoneList.AddRange(standardTimeZoneList);
                _context.StandardTimeZone.AddRange(standardTimeZoneList);
                _context.SaveChanges();
            }
        }

        public static void SeedCurrency(OneClappContext _context)
        {
            if (_context.Currency != null && !_context.Currency.Any())
            {
                List<Currency> currencyList = new List<Currency>() {
                new Currency () { CountryName = "United States" , Name = "United States dollar", Code = "USD" , Symbol = "$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Australia" , Name = "Australian dollar", Code = "AUD" , Symbol = "A$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Bulgaria" , Name = "Bulgarian lev", Code = "BGN" , Symbol = "Лв" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "United Arab Emirates" , Name = "United Arab Emirates dirham", Code = "AED" , Symbol = "د.إ" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Brazil" , Name = "Brazilian real", Code = "BRL" , Symbol = "R$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Canada" , Name = "Canadian dollar", Code = "CAD" , Symbol = "$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Switzerland" , Name = "Swiss franc", Code = "CHF" , Symbol = "CHf" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Czech" , Name = "Czech koruna", Code = "CZK" , Symbol = "Kč" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Denmark" , Name = "Danish krone", Code = "DKK" , Symbol = "Kr." , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Europe" , Name = "Euro", Code = "EUR" , Symbol = "€" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "United Kingdom" , Name = "British pound", Code = "GBP" , Symbol = "£" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Hong Kong" , Name = "Hong Kong dollar", Code = "HKD" , Symbol = "HK$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Croatia" , Name = "Croatian kuna", Code = "HRK" , Symbol = "kn" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Hungary" , Name = "Hungarian forint", Code = "HUF" , Symbol = "Ft" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Israel" , Name = "Israeli new shekel", Code = "ILS" , Symbol = "₪" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Iceland" , Name = "Icelandic króna", Code = "ISK" , Symbol = "kr" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Japan" , Name = "Japanese yen", Code = "JPY" , Symbol = "¥" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Mexico" , Name = "Mexican peso", Code = "MXN" , Symbol = "$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Malaysia" , Name = "Malaysian ringgit", Code = "MYR" , Symbol = "RM" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Norway" , Name = "Norwegian krone", Code = "NOK" , Symbol = "kr" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "New Zealand" , Name = "New Zealand dollar", Code = "NZD" , Symbol = "NZ$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Philippine" , Name = "Philippine piso", Code = "PHP" , Symbol = "₱" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Poland" , Name = "Polish złoty", Code = "PLN" , Symbol = "zł" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Romania" , Name = "Romanian leu", Code = "RON" , Symbol = "lei" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Russian" , Name = "Russian ruble", Code = "RUB" , Symbol = "₽" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Sweden" , Name = "Swedish krona", Code = "SEK" , Symbol = "kr" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Singapore" , Name = "Singapore dollar", Code = "SGD" , Symbol = "S$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Thailand" , Name = "Thai baht", Code = "THB" , Symbol = "฿" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "Taiwan" , Name = "New Taiwan dollar", Code = "TWD" , Symbol = "NT$" , TenantId = null , CreatedOn = DateTime.UtcNow},
                new Currency () { CountryName = "South Africa" , Name = "South African rand", Code = "ZAR" , Symbol = "R" , TenantId = null , CreatedOn = DateTime.UtcNow},
                };
                currencyList.AddRange(currencyList);
                _context.Currency.AddRange(currencyList);
                _context.SaveChanges();
            }
        }

        public static void SeedInvoiceInterval(OneClappContext _context)
        {
            if (_context.InvoiceInterval != null && !_context.InvoiceInterval.Any())
            {
                List<InvoiceInterval> invoiceIntervals = new List<InvoiceInterval>() {
                new InvoiceInterval() { Name = "Weekly",Interval = 7, CreatedOn = DateTime.UtcNow },
                new InvoiceInterval() { Name = "Monthly", Interval = 30, CreatedOn = DateTime.UtcNow },
                new InvoiceInterval() { Name = "Yearly with monthly", Interval = 30, CreatedOn = DateTime.UtcNow },
                new InvoiceInterval() { Name = "Yearly", Interval = 365, CreatedOn = DateTime.UtcNow },
                };
                invoiceIntervals.AddRange(invoiceIntervals);
                _context.InvoiceInterval.AddRange(invoiceIntervals);
                _context.SaveChanges();
            }
        }

        public static void SeedSocialMedias(OneClappContext _context)
        {
            if (_context.SocialMedia != null)
            {
                List<SocialMedia> socialMediaList = new List<SocialMedia>() {
                new SocialMedia () { Name = "Facebook", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SocialMedia () { Name = "Instagram", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SocialMedia () { Name = "Twitter", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SocialMedia () { Name = "Snapchat", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SocialMedia () { Name = "Indeed", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                };
                foreach (var c in socialMediaList)
                {
                    var check = _context.SocialMedia.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.SocialMedia.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedClientUserRoles(OneClappContext _context)
        {
            if (_context.ClientUserRole != null)
            {
                List<ClientUserRole> clientUserRoleList = new List<ClientUserRole>() {
                new ClientUserRole () { Name = "ClientUser", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new ClientUserRole () { Name = OneClappContext.ClientUserRootRole, CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null }
                };
                foreach (var c in clientUserRoleList)
                {
                    var check = _context.ClientUserRole.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.ClientUserRole.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedContractType(OneClappContext _context)
        {
            if (_context.ContractType != null)
            {
                List<ContractType> contractTypeList = new List<ContractType>() {
                new ContractType () { Name = "Fixed", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new ContractType () { Name = "Hourly", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null }
                };
                foreach (var c in contractTypeList)
                {
                    var check = _context.ContractType.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.ContractType.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedDepartment(OneClappContext _context)
        {
            if (_context.Department != null)
            {
                List<Department> departmentList = new List<Department>() {
                new Department () { Name = "Software", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new Department () { Name = "Marketing", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new Department () { Name = "Finance", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new Department () { Name = "Human Resources", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new Department () { Name = "Production", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null }
                };
                foreach (var c in departmentList)
                {
                    var check = _context.Department.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.Department.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedSatisficationLevel(OneClappContext _context)
        {
            if (_context.SatisficationLevel != null)
            {
                List<SatisficationLevel> satisficationLevelList = new List<SatisficationLevel>() {
                new SatisficationLevel () { Name = "Very Good", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SatisficationLevel () { Name = "Good", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SatisficationLevel () { Name = "Worst", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SatisficationLevel () { Name = "Bad", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new SatisficationLevel () { Name = "Very Bad", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null }
                };
                foreach (var c in satisficationLevelList)
                {
                    var check = _context.SatisficationLevel.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.SatisficationLevel.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedAssetsManufacturer(OneClappContext _context)
        {
            if (_context.AssetsManufacturer != null)
            {
                List<AssetsManufacturer> assetsManufacturerList = new List<AssetsManufacturer>() {
                new AssetsManufacturer () { Name = "Dell", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new AssetsManufacturer () { Name = "Lenovo", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new AssetsManufacturer () { Name = "LG", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new AssetsManufacturer () { Name = "Sumsung", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new AssetsManufacturer () { Name = "Intel", CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null }
                };
                foreach (var c in assetsManufacturerList)
                {
                    var check = _context.AssetsManufacturer.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.AssetsManufacturer.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedMatePriority(OneClappContext _context)
        {
            if (_context.MatePriority != null)
            {
                List<MatePriority> matePriorityList = new List<MatePriority>() {
                new MatePriority () { Name = "Urgent", Color = "Red" ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new MatePriority () { Name = "High", Color = "Yellow" ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new MatePriority () { Name = "Medium", Color = "Blue" ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new MatePriority () { Name = "Low", Color = "Grey" ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                };
                foreach (var c in matePriorityList)
                {
                    var check = _context.MatePriority.FirstOrDefault(p => p.Name == c.Name);
                    if (check == null)
                    {
                        _context.MatePriority.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public static void SeedStatus(OneClappContext _context)
        {
            if (_context.Status != null)
            {
                List<Status> statusList = new List<Status>() {
                new Status () { Name = "Overdue", Color = "Red" , IsDefault = true ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new Status () { Name = "Completed", Color = "Green" , IsDefault = true ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new Status () { Name = "In Progress", Color = "Blue" , IsDefault = true ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                new Status () { Name = "To Do", Color = "Grey" , IsDefault = true ,CustomTableId = null ,CreatedOn = DateTime.UtcNow, UpdatedOn = null, DeletedOn = null },
                };
                foreach (var c in statusList)
                {
                    var check = _context.Status.FirstOrDefault(p => p.Name == c.Name && p.CustomTableId == null);
                    if (check == null)
                    {
                        _context.Status.Add(c);
                        _context.SaveChanges();
                    }
                }
            }
        }

    }
}