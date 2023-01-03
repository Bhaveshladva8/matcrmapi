using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using matcrm.data;
using matcrm.data.Context;
using matcrm.service.Services;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;
using System.Globalization;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
// using FluentEmail.Graph;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using matcrm.api.ActionFilter;
using matcrm.api.Extensions;
using matcrm.api.Middleware;
using matcrm.api.SignalR;
using matcrm.authentication.jwt;
using matcrm.data.Infrastructure;
using matcrm.service.BusinessLogic;
using matcrm.service.Services.ERP;
using matcrm.service.Services.Mollie.Client;
using matcrm.service.Services.Mollie.Customer;
using matcrm.service.Services.Mollie.Mandate;
using matcrm.service.Services.Mollie.Payment.Refund;
using matcrm.service.Services.Mollie.Payment;
using matcrm.service.Services.Mollie.PaymentMethod;
using matcrm.service.Services.Mollie.Subscription;
using Hangfire.PostgreSql;

namespace matcrm.api
{
    public class StartupTest
    {
        public IContainer ApplicationContainer { get; private set; }
        private IHostingEnvironment HostingEnvironment { get; set; }
        private readonly SendEmail sendEmail;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly IMollieSubscriptionService _mollieSubscriptionService;
        private readonly IMollieCustomerService _mollieCustomerService;
        private readonly ISubscriptionStorageClient _subscriptionStorageClient;
        private readonly PaymentNotificationLogic paymentNotificationLogic;
        private readonly IInvoiceMollieSubscriptionService _invoiceMollieSubscriptionService;
        private readonly IPaymentStorageClient _paymentStorageClient;
        private readonly IClientService _clientService;
        private readonly IClientInvoiceService _clientInvoiceService;
        private readonly IContractService _contractService;
        private IMapper _mapper;
        private string CurrentURL { get; set; }
        private readonly string AllowCors = "_allowCors";

        public IConfiguration Configuration { get; }
        public StartupTest(IConfiguration configuration)
        {
            Configuration = configuration;
            sendEmail = new SendEmail(_emailTemplateService, _emailLogService, _emailConfigService, _emailProviderService, _mapper);
            paymentNotificationLogic = new PaymentNotificationLogic(_userSubscriptionService, _subscriptionTypeService, _emailTemplateService, _emailLogService, _emailProviderService, _emailConfigService, _mollieSubscriptionService, _mollieCustomerService, _subscriptionStorageClient, _invoiceMollieSubscriptionService, _paymentStorageClient, _clientInvoiceService, _clientService, _contractService, _mapper);
        }
        private string ConnectionString
        {
            get
            {
                return this.HostingEnvironment.IsDevelopment() ? Configuration.GetConnectionString("DefaultConnection") : Configuration.GetConnectionString("DefaultConnection");
            }
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            SetContext();
            services.AddSingleton<IConfiguration>(Configuration);
            var con = Configuration["ConnectionString:DefaultConnection"];
            // services.AddDbContext<OneClappContext> (options =>
            //     options.UseSqlServer (con)
            // );

            services.AddDbContext<OneClappContext>(options =>
            {
                // options.UseSqlServer(con);
                options.UseNpgsql(con
                , b => b.MigrationsAssembly("matcrm.api"));
            }, ServiceLifetime.Transient);

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowCors,
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });
            services.AddMvc(o => o.Filters.Add(typeof(ValidateModelStateFilter)))
                .AddJsonOptions(o =>
                {
                    // code commented for json without camelcase
                    // o.JsonSerializerOptions.PropertyNamingPolicy = null;
                    o.JsonSerializerOptions.DictionaryKeyPolicy = null;
                });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSession();

            // services.AddLocalization (options => options.ResourcesPath = "Resources");

            // services.Configure<RequestLocalizationOptions> (options => {
            //     var supportedCultures = new List<CultureInfo> {
            //     new CultureInfo ("en-US"),
            //     new CultureInfo ("gu-IN"),
            //     new CultureInfo ("de-DE")
            //     };

            //     options.DefaultRequestCulture = new RequestCulture (culture: "en-US", uiCulture: "en-US");
            //     options.SupportedCultures = supportedCultures;
            //     options.SupportedUICultures = supportedCultures;
            //     options.RequestCultureProviders = new [] { new RouteDataRequestCultureProviderExtension { IndexOfCulture = 1, IndexofUICulture = 1 } };
            // });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
            });

            services.AddControllers(options => options.EnableEndpointRouting = false);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            /// Swagger START
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = OneClappContext.ProjectName + ".api", Version = "v1" });
            });
            /// Swagger END

            /// Authentication START
            // services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme,)
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration.GetSection("JwtConfig:ValidIssuer").Value,
                        ValidAudience = Configuration.GetSection("JwtConfig:ValidAudience").Value,
                        IssuerSigningKey = JwtSecurityKey.Create(Configuration.GetSection("JwtConfig:SecretKey").Value)
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            Console.WriteLine("OnTokenValidated: " + context.SecurityToken);
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = OneClappContext.GoogleClientId;
                    options.ClientSecret = OneClappContext.GoogleSecretKey;
                });
            /// Authentication END

            // services.AddSignalR();
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
                hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(5);
            });


            // Add Hangfire services.
            services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                   .UseSimpleAssemblyNameTypeSerializer()
                   .UseRecommendedSerializerSettings()
                   .UsePostgreSqlStorage(con
                   //    , new PostgreSqlStorageOptions
                   //    {
                   //        QueuePollInterval = TimeSpan.Zero
                   //    }
                   ));
            //    .UseSqlServerStorage(con, new SqlServerStorageOptions
            //    {
            //        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            //        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            //        QueuePollInterval = TimeSpan.Zero,
            //        UseRecommendedIsolationLevel = true,
            //        DisableGlobalLocks = true
            //    }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            // var AppId = "695c100b-0030-4574-8cd4-7a276e274368";
            var AppId = "762d7faf-71fc-4519-b42a-f96eddd4994b";
            var TenantId = "f8cdef31-a31e-4b4a-93e4-5f571e91255a";
            // var Secret = "5e2643fd-c24e-457e-a435-1e1fd4df3092";
            var Secret = "cf101461-3d65-43c1-a542-e1a141398b50";
            var Sender = "shraddha.prajapati@techavidus.com";
            // var graphSenderOptions = new GraphSenderOptions
            // {
            //     ClientId = AppId,
            //     Secret = Secret,
            //     TenantId = TenantId,
            //     SaveSentItems = true
            // };

            // Reference link for fluent graph email
            // https://goforgoldman.com/2021/02/13/fluent-email.html

            // services.AddFluentEmail("shraddha.prof21@gmail.com", "GoForGoldman Mail Service")
            //         .AddRazorRenderer()
            //         .AddGraphSender(graphSenderOptions);

            // Add framework services.
            services.AddMvc();

            /// add services
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDbFactory, DbFactory>();
            services.AddTransient<IMapper, Mapper>();
            // services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<IEmailTemplateService, EmailTemplateService>();
            services.AddTransient<IErrorLogService, ErrorLogService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<ITenantService, TenantService>();
            services.AddTransient<IVerificationCodeService, VerificationCodeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITenantConfigService, TenantConfigService>();
            services.AddTransient<ITenantActivityService, TenantActivityService>();
            services.AddTransient<IEmailProviderService, EmailProviderService>();
            services.AddTransient<IEmailConfigService, EmailConfigService>();
            services.AddTransient<IEmailLogService, EmailLogService>();
            services.AddTransient<IERPSystemService, ERPSystemService>();
            services.AddTransient<IUserERPSystemService, UserERPSystemService>();

            // Start Custom field Services

            services.AddTransient<ICustomControlService, CustomControlService>();
            services.AddTransient<ICustomControlOptionService, CustomControlOptionService>();
            services.AddTransient<ICustomFieldService, CustomFieldService>();
            services.AddTransient<ICustomModuleService, CustomModuleService>();
            services.AddTransient<IModuleFieldService, ModuleFieldService>();
            services.AddTransient<ITenantModuleService, TenantModuleService>();
            services.AddTransient<ICustomTenantFieldService, CustomTenantFieldService>();
            services.AddTransient<ICustomTableService, CustomTableService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<ICustomFieldValueService, CustomFieldValueService>();

            // End Custom field Services

            services.AddTransient<ICustomerTypeService, CustomerTypeService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<IEmailPhoneNoTypeService, EmailPhoneNoTypeService>();
            services.AddTransient<IOrganizationLabelService, OrganizationLabelService>();
            services.AddTransient<ICustomerLabelService, CustomerLabelService>();
            services.AddTransient<ICustomerEmailService, CustomerEmailService>();
            services.AddTransient<ICustomerPhoneService, CustomerPhoneService>();
            services.AddTransient<ICustomerNoteService, CustomerNoteService>();
            services.AddTransient<IOrganizationService, OrganizationService>();
            services.AddTransient<ICustomerAttachmentService, CustomerAttachmentService>();
            services.AddTransient<IOrganizationNoteService, OrganizationNoteService>();
            services.AddTransient<IOrganizationAttachmentService, OrganizationAttachmentService>();
            services.AddTransient<ICustomerNotesCommentService, CustomerNotesCommentService>();
            services.AddTransient<IOrganizationNotesCommentService, OrganizationNotesCommentService>();
            services.AddTransient<ICustomTableColumnService, CustomTableColumnService>();
            services.AddTransient<ITableColumnUserService, TableColumnUserService>();
            services.AddTransient<ILeadLabelService, LeadLabelService>();
            services.AddTransient<ILeadService, LeadService>();
            services.AddTransient<ILeadNoteService, LeadNoteService>();
            services.AddTransient<ILabelCategoryService, LabelCategoryService>();
            services.AddTransient<ILabelService, LabelService>();

            services.AddTransient<IActivityTypeService, ActivityTypeService>();
            services.AddTransient<IActivityAvailabilityService, ActivityAvailabilityService>();
            services.AddTransient<ICustomerActivityService, CustomerActivityService>();
            services.AddTransient<ICustomerActivityMemberService, CustomerActivityMemberService>();
            services.AddTransient<IOrganizationActivityService, OrganizationActivityService>();
            services.AddTransient<IOrganizationActivityMemberService, OrganizationActivityMemberService>();
            services.AddTransient<ILeadActivityService, LeadActivityService>();
            services.AddTransient<ILeadActivityMemberService, LeadActivityMemberService>();
            services.AddTransient<ICalendarListService, CalendarListService>();
            services.AddTransient<ICalendarTaskService, CalendarTaskService>();
            services.AddTransient<ICalendarSubTaskService, CalendarSubTaskService>();
            services.AddTransient<ICalendarRepeatTypeService, CalendarRepeatTypeService>();
            services.AddTransient<IOneClappLatestThemeService, OneClappLatestThemeService>();
            services.AddTransient<IOneClappLatestThemeSchemeService, OneClappLatestThemeSchemeService>();
            services.AddTransient<IOneClappLatestThemeLayoutService, OneClappLatestThemeLayoutService>();
            services.AddTransient<IOneClappLatestThemeConfigService, OneClappLatestThemeConfigService>();
            services.AddTransient<IGoogleCalendarService, GoogleCalendarService>();
            services.AddTransient<IIntProviderService, IntProviderService>();
            services.AddTransient<IIntProviderAppService, IntProviderAppService>();
            services.AddTransient<IIntProviderAppSecretService, IntProviderAppSecretService>();
            services.AddTransient<ICheckListService, CheckListService>();
            services.AddTransient<ICheckListUserService, CheckListUserService>();
            services.AddTransient<ICalendarSyncActivityService, CalendarSyncActivityService>();
            services.AddTransient<IOneClappModuleService, OneClappModuleService>();
            services.AddTransient<IOneClappFormTypeService, OneClappFormTypeService>();
            services.AddTransient<IOneClappFormService, OneClappFormService>();
            services.AddTransient<IOneClappFormFieldService, OneClappFormFieldService>();
            services.AddTransient<ICheckListAssignUserService, CheckListAssignUserService>();
            services.AddTransient<IOneClappFormStatusService, OneClappFormStatusService>();
            services.AddTransient<IOneClappRequestFormService, OneClappRequestFormService>();
            services.AddTransient<IOneClappFormFieldValueService, OneClappFormFieldValueService>();
            services.AddTransient<IOneClappFormActionService, OneClappFormActionService>();
            services.AddTransient<IWeClappUserService, WeClappUserService>();
            services.AddTransient<IWeClappService, WeClappService>();
            services.AddTransient<IERPSystemColumnMapService, ERPSystemColumnMapService>();
            services.AddTransient<IModuleRecordCustomFieldService, ModuleRecordCustomFieldService>();

            services.AddTransient<ISubscriptionPlanService, SubscriptionPlanService>();
            services.AddTransient<ISubscriptionPlanDetailService, SubscriptionPlanDetailService>();
            services.AddTransient<ISubscriptionTypeService, SubscriptionTypeService>();
            services.AddTransient<IUserSubscriptionService, UserSubscriptionService>();
            services.AddTransient<IMollieCustomerService, MollieCustomerService>();
            services.AddTransient<IMollieSubscriptionService, MollieSubscriptionService>();
            services.AddTransient<ISalutationService, SalutationService>();
            services.AddTransient<IImportContactAttachmentService, ImportContactAttachmentService>();
            services.AddTransient<IExternalUserService, ExternalUserService>();

            // Start dynamic form design services
            services.AddTransient<IBorderStyleService, BorderStyleService>();
            services.AddTransient<IOneClappFormHeaderService, OneClappFormHeaderService>();
            services.AddTransient<IOneClappFormLayoutBackgroundService, OneClappFormLayoutBackgroundService>();
            services.AddTransient<IOneClappFormLayoutService, OneClappFormLayoutService>();
            // End dynamic form design services

            services.AddTransient<IDiscussionService, DiscussionService>();
            services.AddTransient<IDiscussionReadService, DiscussionReadService>();
            services.AddTransient<IDiscussionParticipantService, DiscussionParticipantService>();
            services.AddTransient<IMailBoxTeamService, MailBoxTeamService>();
            services.AddTransient<IMailCommentService, MailCommentService>();
            services.AddTransient<IMailAssignUserService, MailAssignUserService>();
            services.AddTransient<IMailReadService, MailReadService>();
            services.AddTransient<IDiscussionCommentAttachmentService, DiscussionCommentAttachmentService>();
            services.AddTransient<IDiscussionCommentService, DiscussionCommentService>();
            services.AddTransient<ITeamInboxService, TeamInboxService>();
            services.AddTransient<IMailCommentAttachmentService, MailCommentAttachmentService>();
            services.AddTransient<IMailParticipantService, MailParticipantService>();
            services.AddTransient<IMailAssignCustomerService, MailAssignCustomerService>();
            services.AddTransient<ITeamInboxAccessService, TeamInboxAccessService>();
            services.AddTransient<ICustomDomainEmailConfigService, CustomDomainEmailConfigService>();

            services.AddTransient<IOneClappTaskService, OneClappTaskService>();
            services.AddTransient<IOneClappSubTaskService, OneClappSubTaskService>();
            services.AddTransient<IOneClappChildTaskService, OneClappChildTaskService>();
            services.AddTransient<IOneClappTaskUserSerivce, OneClappTaskUserSerivce>();
            services.AddTransient<ITaskStatusService, TaskStatusService>();
            services.AddTransient<ITaskCommentService, TaskCommentService>();
            services.AddTransient<ITaskActivityService, TaskActivityService>();
            services.AddTransient<ITaskTimeRecordService, TaskTimeRecordService>();
            services.AddTransient<ISectionService, SectionService>();
            services.AddTransient<ISectionActivityService, SectionActivityService>();
            services.AddTransient<ISubTaskActivityService, SubTaskActivityService>();
            services.AddTransient<ISubTaskAttachmentService, SubTaskAttachmentService>();
            services.AddTransient<ISubTaskCommentService, SubTaskCommentService>();
            services.AddTransient<IOneClappSubTaskUserService, OneClappSubTaskUserService>();
            services.AddTransient<ISubTaskTimeRecordService, SubTaskTimeRecordService>();
            services.AddTransient<IChildTaskCommentService, ChildTaskCommentService>();
            services.AddTransient<IChildTaskActivityService, ChildTaskActivityService>();
            services.AddTransient<IChildTaskTimeRecordService, ChildTaskTimeRecordService>();
            services.AddTransient<IOneClappChildTaskUserService, OneClappChildTaskUserService>();
            services.AddTransient<ITaskAttachmentService, TaskAttachmentService>();
            services.AddTransient<IChildTaskAttachmentService, ChildTaskAttachmentService>();
            services.AddTransient<ITaskWeclappUserService, TaskWeclappUserService>();

            services.AddTransient<IEmployeeProjectActivityService, EmployeeProjectActivityService>();
            services.AddTransient<IEmployeeProjectService, EmployeeProjectService>();
            services.AddTransient<IEmployeeProjectStatusService, EmployeeProjectStatusService>();
            services.AddTransient<IEmployeeTaskActivityService, EmployeeTaskActivityService>();
            services.AddTransient<IEmployeeTaskService, EmployeeTaskService>();
            services.AddTransient<IEmployeeTaskAttachmentService, EmployeeTaskAttachmentService>();
            services.AddTransient<IEmployeeTaskCommentService, EmployeeTaskCommentService>();
            services.AddTransient<IEmployeeTaskStatusService, EmployeeTaskStatusService>();
            services.AddTransient<IEmployeeTaskTimeRecordService, EmployeeTaskTimeRecordService>();
            services.AddTransient<IEmployeeTaskUserSerivce, EmployeeTaskUserSerivce>();

            services.AddTransient<IEmployeeSubTaskService, EmployeeSubTaskService>();
            services.AddTransient<IEmployeeSubTaskAttachmentService, EmployeeSubTaskAttachmentService>();
            services.AddTransient<IEmployeeSubTaskCommentService, EmployeeSubTaskCommentService>();
            services.AddTransient<IEmployeeSubTaskUserService, EmployeeSubTaskUserService>();
            services.AddTransient<IEmployeeSubTaskTimeRecordService, EmployeeSubTaskTimeRecordService>();
            services.AddTransient<IEmployeeSubTaskActivityService, EmployeeSubTaskActivityService>();
            services.AddTransient<IEmployeeChildTaskService, EmployeeChildTaskService>();
            services.AddTransient<IEmployeeChildTaskAttachmentService, EmployeeChildTaskAttachmentService>();
            services.AddTransient<IEmployeeChildTaskCommentService, EmployeeChildTaskCommentService>();
            services.AddTransient<IEmployeeChildTaskUserService, EmployeeChildTaskUserService>();
            services.AddTransient<IEmployeeChildTaskTimeRecordService, EmployeeChildTaskTimeRecordService>();
            services.AddTransient<IEmployeeChildTaskActivityService, EmployeeChildTaskActivityService>();


            // Start Mollie Services

            services.AddTransient<IPaymentOverviewClient, PaymentOverviewClient>();
            services.AddTransient<ICustomerOverviewClient, CustomerOverviewClient>();
            services.AddTransient<ISubscriptionOverviewClient, SubscriptionOverviewClient>();
            services.AddTransient<IMandateOverviewClient, MandateOverviewClient>();
            services.AddTransient<IPaymentMethodOverviewClient, PaymentMethodOverviewClient>();
            services.AddTransient<IPaymentStorageClient, PaymentStorageClient>();
            services.AddTransient<ICustomerStorageClient, CustomerStorageClient>();
            services.AddTransient<ISubscriptionStorageClient, SubscriptionStorageClient>();
            services.AddTransient<IMandateStorageClient, MandateStorageClient>();
            services.AddScoped<IRefundPaymentClient, RefundPaymentClient>();

            services.AddScoped<IPaymentClient, PaymentClient>(x => new PaymentClient(OneClappContext.MollieApiKey));
            services.AddScoped<ICustomerClient, CustomerClient>(x => new CustomerClient(OneClappContext.MollieApiKey));
            services.AddScoped<IRefundClient, RefundClient>(x => new RefundClient(OneClappContext.MollieApiKey));
            services.AddScoped<IPaymentMethodClient, PaymentMethodClient>(x => new PaymentMethodClient(OneClappContext.MollieApiKey));
            services.AddScoped<ISubscriptionClient, SubscriptionClient>(x => new SubscriptionClient(OneClappContext.MollieApiKey));
            services.AddScoped<IMandateClient, MandateClient>(x => new MandateClient(OneClappContext.MollieApiKey));
            services.AddScoped<IInvoicesClient, InvoicesClient>(x => new InvoicesClient(OneClappContext.MollieApiKey));
            services.AddScoped<IShipmentClient, ShipmentClient>(x => new ShipmentClient(OneClappContext.MollieApiKey));
            services.AddScoped<ISettlementsClient, SettlementsClient>(x => new SettlementsClient(OneClappContext.MollieApiKey));
            services.AddScoped<IProfileClient, ProfileClient>(x => new ProfileClient(OneClappContext.MollieApiKey));
            services.AddScoped<IPermissionsClient, PermissionsClient>(x => new PermissionsClient(OneClappContext.MollieApiKey));
            services.AddScoped<IPaymentLinkClient, PaymentLinkClient>(x => new PaymentLinkClient(OneClappContext.MollieApiKey));
            services.AddScoped<IOrganizationsClient, OrganizationsClient>(x => new OrganizationsClient(OneClappContext.MollieApiKey));
            services.AddScoped<IOrderClient, OrderClient>(x => new OrderClient(OneClappContext.MollieApiKey));
            services.AddScoped<IOnboardingClient, OnboardingClient>(x => new OnboardingClient(OneClappContext.MollieApiKey));
            services.AddScoped<ICaptureClient, CaptureClient>(x => new CaptureClient(OneClappContext.MollieApiKey));
            services.AddScoped<IChargebacksClient, ChargebacksClient>(x => new ChargebacksClient(OneClappContext.MollieApiKey));

            // End

            services.AddTransient<IMailBoxService, MailBoxService>();







            // BL Change
            services.AddTransient<IProxyService, ProxyService>();

            // services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            // {
            //     builder
            //         .AllowAnyMethod()
            //         .AllowAnyHeader()
            //         .AllowCredentials()
            //         .WithOrigins(OneClappContext.AppURL);
            //     // .WithOrigins (OneClappContext.AppURL, "http://localhost:4200");
            //     // .WithOrigins("http://localhost:4200");  
            // }));

            // create a Autofac container builder
            // services.AddScoped (typeof (IRepository<>), typeof (Repository<>));
            // services.AddScoped (typeof (IService<>), typeof (Service<>));

            //var builder = new ContainerBuilder();
            //builder.Register(ctx => GetCaller()).As<ICallerService>().InstancePerDependency();
            //builder.Populate(services);
            //AutofacModule.RegisterType(builder);
            //ApplicationContainer = builder.Build();
        }
        //public void ConfigureContainer(ContainerBuilder builder)
        //{
        //    builder.RegisterModule(new AutofacModule());
        //}

        public void SetContext()
        {
            OneClappContext.ConnectionString = Configuration["ConnectionString:DefaultConnection"];
            OneClappContext.SecretKey = Configuration["JwtConfig:SecretKey"];
            OneClappContext.ProjectName = Configuration["ProjectName"];
            OneClappContext.CurrentURL = Configuration["JwtConfig:ValidIssuer"];
            OneClappContext.AppURL = Configuration["JwtConfig:ValidAudience"];
            OneClappContext.TokenExpireMinute = Configuration["JwtConfig:TokenExpireMinute"];
            OneClappContext.GoogleClientId = Configuration["GoogleAuthentication:ClientId"];
            OneClappContext.GoogleSecretKey = Configuration["GoogleAuthentication:SecretId"];
            OneClappContext.GoogleCalendarClientId = Configuration["GoogleCalendar:ClientId"];
            OneClappContext.GoogleCalendarSecretKey = Configuration["GoogleCalendar:SecretId"];
            OneClappContext.GoogleCalendarApiKey = Configuration["GoogleCalendar:APIKey"];
            OneClappContext.ValidAudience = Configuration["JwtConfig:ValidAudience"];
            OneClappContext.ValidIssuer = Configuration["JwtConfig:ValidIssuer"];
            OneClappContext.SubmitUrl = Configuration["JwtConfig:SubmitUrl"];
            OneClappContext.MollieApiKey = Configuration["Mollie:APIKey"];
            OneClappContext.MollieClientId = Configuration["Mollie:ClientId"];
            OneClappContext.MollieSecretKey = Configuration["Mollie:ClientSecret"];
            OneClappContext.MollieDefaultRedirectUrl = Configuration["Mollie:DefaultRedirectUrl"];
            OneClappContext.HcaptchaSiteKey = Configuration["Hcaptcha:SiteKey"];
            OneClappContext.HcaptchaSiteSecret = Configuration["Hcaptcha:SiteSecret"];
            OneClappContext.HcaptchaVerifyUrl = Configuration["Hcaptcha:VerifyUrl"];
            OneClappContext.MicroSoftClientId = Configuration["MicroSoft:ClientId"];
            OneClappContext.MicroSecretKey = Configuration["MicroSoft:ClientSecret"];
            OneClappContext.MicroSoftTenantId = Configuration["MicroSoft:TenantId"];
            OneClappContext.MicroSoftRedirectUrl = Configuration["MicroSoft:DefaultRedirectUrl"];
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, OneClappContext context, IBackgroundJobClient backgroundJobs)
        {

            // using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            // {
            //     serviceScope.ServiceProvider.GetService<OneClappContext>()
            //         .Database.Migrate();
            //     DataSeeder.Seed(context);
            // }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            // app.UseHangfireDashboard();
            app.UseSwaggerUI(c =>
            {
                // c.SwaggerEndpoint ("/swagger/v1/swagger.json", "matcrm.api v1");

                c.SwaggerEndpoint("swagger/v1/swagger.json", OneClappContext.ProjectName + ".api v1");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
            });

            var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizeOptions.Value);

            //Cors
            // app.UseCors (builder => {
            //     builder.AllowAnyHeader ();
            //     builder.AllowAnyMethod ();
            //     // builder.AllowCredentials ();
            //     builder.AllowAnyOrigin (); // For anyone access.
            // });

            app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) // allow any origin
               .AllowCredentials()); // allow credentials

            // app.UseCors("CorsPolicy");

            app.Use(async (ctx, next) =>
            {
                //IErrorLogService iErrorLogService = ctx.RequestServices.GetService<IErrorLogService>();
                //await ctx.Response.WriteAsync(iErrorLogService.get());
                await next();
            });

            // RecurringJob.AddOrUpdate (
            //     () => sendEmail.ResendEmail (),
            //     Cron.MinuteInterval (5));

            // Run every day at 00:00 AM
            RecurringJob.AddOrUpdate("ResendFailedMailJob",
                () => sendEmail.ResendFailedEmails(),
                Cron.Daily(23, 59));

            // RecurringJob.AddOrUpdate (
            //     () => paymentNotificationLogic.PaymentNotification (),
            //     Cron.MinuteInterval (5));

            RecurringJob.AddOrUpdate("PaymentNotificationJob",
          () => paymentNotificationLogic.PaymentNotification(),
          Cron.Daily(23, 59));

            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSession();
            // app.UseMvc ();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<BroadcastHub>("/notify");
            });


            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHangfireDashboard();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // app.ConfigureCustomExceptionMiddleware();
            // app.UseExceptionHandler();
        }

        //private static ICallerService GetCaller()
        //{
        //    if (_httpContextAccessor == null)
        //    {
        //        return (ICallerService)new AnonymousCaller();
        //    }

        //    var principal = _httpContextAccessor.HttpContext?.User;
        //    var caller = principal?.Identity == null || !principal.Identity.IsAuthenticated
        //        ? (ICallerService)new AnonymousCaller()
        //        : (ICallerService)new AuthenticatedCaller(principal);

        //    return caller;
        //}
    }
}