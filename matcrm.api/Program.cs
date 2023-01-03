// using System;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Infrastructure;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using matcrm.api;
// using matcrm.data.Context;

// namespace JobCRM
// {
//     public class Program
//     {
//         private static string _env = "dev";
//         public static void Main(string[] args)
//         {

//             setEnvironment();
//             CreateHostBuilder(args)
//                 .Build()
//                 .MigrateDatabase()
//                 .Run();
//         }

//         private static void setEnvironment()
//         {
//             try
//             {
//                 var config = new ConfigurationBuilder()
//               .AddJsonFile("appsettings.json", false)
//               .Build();
//                 _env = config.GetSection("Environment").Value;
//             }
//             catch (Exception ex)
//             {

//                 throw ex;
//             }
//         }

//         public static IHostBuilder CreateHostBuilder(string[] args) =>
//             Host.CreateDefaultBuilder(args)
//              .ConfigureAppConfiguration((hostingContext, config) =>
//             {
//                 config.AddJsonFile("appsettings.json");
//                 config.AddJsonFile($"appsettings.{_env}.json", optional: true);
//             })
//                 .ConfigureWebHostDefaults(webBuilder =>
//                 {
//                     webBuilder.UseStartup<Startup>();
//                     webBuilder.UseUrls("http://localhost:4000/");
//                 });
//     }

//     public static class MigrationManager
//     {


//         public static IHost MigrateDatabase(this IHost host)
//         {
//             using (var scope = host.Services.CreateScope())
//             {
//                 using (var appContext = scope.ServiceProvider.GetRequiredService<OneClappContext>())
//                 {
//                     try
//                     {
//                         if (!appContext.GetService<OneClappContext>().AllMigrationsApplied())
//                         {
//                             appContext.GetService<OneClappContext>().Database.Migrate();
//                             DataSeeder.Seed(appContext);
//                         }
//                     }
//                     catch (Exception ex)
//                     {
//                         //Log errors or do anything you think it's needed
//                         throw ex;
//                     }
//                 }
//             }

//             return host;
//         }
//     }
// }

using System;
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
using System.Threading.Tasks;
// using Autofac;
using AutoMapper;
// using FluentEmail.Graph;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
using matcrm.data.Helpers;

string AllowCors = "_allowCors";
//a variable to hold configuration
IConfiguration Configuration;
SendEmail sendEmail;
PaymentNotificationLogic paymentNotificationLogic;

IMapper mapper;
//Testmodel _appconfig;
string _env = "dev";
var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:4000/");
Configuration = builder.Configuration;

var config1 = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false)
              .Build();

_env = config1.GetSection("Environment").Value;


Configuration = new ConfigurationBuilder()
      //    .SetBasePath(env.ContentRootPath)
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile($"appsettings.{_env}.json", optional: true)
      .AddEnvironmentVariables()
      .Build();

// builder.Services.Configure<RouteOptions>(options =>
//             {
//                 options.ConstraintMap.Add("culture", typeof(LanguageRouteConstraint));
//             });

// Add services to the container.
//builder.Configuration.GetSection("Testmodel").Get<Testmodel>();

builder.Services.AddControllers(options => options.EnableEndpointRouting = false);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = OneClappContext.ProjectName + ".api", Version = "v1" });
                option.EnableAnnotations();
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });



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
OneClappContext.GoogleScopes = Configuration["GoogleCalendar:Scopes"];
OneClappContext.ValidAudience = Configuration["JwtConfig:ValidAudience"];
OneClappContext.ValidIssuer = Configuration["JwtConfig:ValidIssuer"];
OneClappContext.SubmitUrl = Configuration["JwtConfig:SubmitUrl"];
OneClappContext.MollieApiKey = Configuration["Mollie:APIKey"];
OneClappContext.MollieClientId = Configuration["Mollie:ClientId"];
OneClappContext.MollieSecretKey = Configuration["Mollie:ClientSecret"];
OneClappContext.PaidPaymentStatus = Configuration["Mollie:PaidPaymentStatus"];
OneClappContext.MollieDefaultRedirectUrl = Configuration["Mollie:DefaultRedirectUrl"];
OneClappContext.HcaptchaSiteKey = Configuration["Hcaptcha:SiteKey"];
OneClappContext.HcaptchaSiteSecret = Configuration["Hcaptcha:SiteSecret"];
OneClappContext.HcaptchaVerifyUrl = Configuration["Hcaptcha:VerifyUrl"];
OneClappContext.MicroSoftClientId = Configuration["MicroSoft:ClientId"];
OneClappContext.MicroSecretKey = Configuration["MicroSoft:ClientSecret"];
OneClappContext.MicroSoftTenantId = Configuration["MicroSoft:TenantId"];
OneClappContext.MicroSoftRedirectUrl = Configuration["MicroSoft:DefaultRedirectUrl"];
OneClappContext.MicrosoftUserScopes = Configuration["MicroSoft:UserScopes"];
OneClappContext.MicrosoftClientScopes = Configuration["MicroSoft:ClientScopes"];
OneClappContext.OriginalUserProfileDirPath = Configuration["ImagePath:OriginalUserProfileDirPath"];
OneClappContext.ReSizedUserProfileDirPath = Configuration["ImagePath:ReSizedUserProfileDirPath"];
OneClappContext.CustomerFileUploadDirPath = Configuration["ImagePath:CustomerFileUploadDirPath"];
OneClappContext.DiscussionCommentUploadDirPath = Configuration["ImagePath:DiscussionCommentUploadDirPath"];
OneClappContext.EmployeeChildTaskUploadDirPath = Configuration["ImagePath:EmployeeChildTaskUploadDirPath"];
OneClappContext.EmployeeSubTaskUploadDirPath = Configuration["ImagePath:EmployeeSubTaskUploadDirPath"];
OneClappContext.SubTaskUploadDirPath = Configuration["ImagePath:SubTaskUploadDirPath"];
OneClappContext.ChildTaskUploadDirPath = Configuration["ImagePath:ChildTaskUploadDirPath"];
OneClappContext.EmployeeTaskUploadDirPath = Configuration["ImagePath:EmployeeTaskUploadDirPath"];
OneClappContext.DynamicFormHeaderDirPath = Configuration["ImagePath:DynamicFormHeaderDirPath"];
OneClappContext.ImportContactDirPath = Configuration["ImagePath:ImportContactDirPath"];
OneClappContext.DynamicFormLayoutDirPath = Configuration["ImagePath:DynamicFormLayoutDirPath"];
OneClappContext.DefaultLayoutDirPath = Configuration["ImagePath:DefaultLayoutDirPath"];
OneClappContext.MailCommentUploadDirPath = Configuration["ImagePath:MailCommentUploadDirPath"];
OneClappContext.OrganizationUploadDirPath = Configuration["ImagePath:OrganizationUploadDirPath"];
OneClappContext.ProjectLogoDirPath = Configuration["ImagePath:ProjectLogoDirPath"];
OneClappContext.TaskUploadDirPath = Configuration["ImagePath:TaskUploadDirPath"];
OneClappContext.BgImageDirPath = Configuration["ImagePath:BgImageDirPath"];
OneClappContext.LogoImageDirPath = Configuration["ImagePath:LogoImageDirPath"];
OneClappContext.FormsJSUploadDirPath = Configuration["JSPath:FormsJSUploadDirPath"];
OneClappContext.ModalFormsJSUploadDirPath = Configuration["JSPath:ModalFormsJSUploadDirPath"];
OneClappContext.SlidingFormJSUploadDirPath = Configuration["JSPath:SlidingFormJSUploadDirPath"];
OneClappContext.ClamAVServerURL = Configuration["ClamAVServer:URL"];
OneClappContext.ClamAVServerPort = Configuration["ClamAVServer:Port"];
OneClappContext.ClamAVServerIsLive = Convert.ToBoolean(Configuration["ClamAVServer:IsLive"]);
OneClappContext.ClientLogoDirPath = Configuration["ImagePath:ClientLogoDirPath"];
OneClappContext.MailServer = Configuration["MailConfiguration:mailServer"];
OneClappContext.MailFrom = Configuration["MailConfiguration:mailFrom"];
OneClappContext.MailPassword = Configuration["MailConfiguration:mailPassword"];
OneClappContext.MateCommentUploadDirPath = Configuration["ImagePath:MateCommentUploadDirPath"];
OneClappContext.ClientUserRootRole = Configuration["ClientUserRole:RootRole"];
OneClappContext.ClientUserLogoDirPath = Configuration["ImagePath:ClientUserLogoDirPath"];
OneClappContext.ProjectCategoryIconDirPath = Configuration["ImagePath:ProjectCategoryIconDirPath"];
OneClappContext.MateCategoryIconDirPath = Configuration["ImagePath:MateCategoryIconDirPath"];
OneClappContext.ProjectMailUrl = Configuration["AssignMail:Project"];
OneClappContext.TaskMailUrl = Configuration["AssignMail:Task"];
OneClappContext.TicketMailUrl = Configuration["AssignMail:Ticket"];
// builder.Services.AddSingleton<IConfiguration>(Configuration);
var con = Configuration["ConnectionString:DefaultConnection"];

builder.Services.AddDbContext<OneClappContext>(options =>
           {
               // options.UseSqlServer(con);
               options.UseNpgsql(con);
           }, ServiceLifetime.Transient);

builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: AllowCors,
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });
builder.Services.AddMvc(o => o.Filters.Add(typeof(ValidateModelStateFilter)))
    .AddJsonOptions(o =>
    {
        // code commented for json without camelcase
        // o.JsonSerializerOptions.PropertyNamingPolicy = null;
        o.JsonSerializerOptions.DictionaryKeyPolicy = null;
    });
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// builder.Services.AddAutoMapper(config =>
// {
//     config.AddProfile(AutoMapperProfile);
// });

builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var Key = System.Text.Encoding.UTF8.GetBytes(OneClappContext.SecretKey);
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
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                            }
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
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(5);
});




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

// builder.Services.AddFluentEmail("shraddha.prof21@gmail.com", "GoForGoldman Mail Service")
//         .AddRazorRenderer()
//         .AddGraphSender(graphSenderOptions);

// Add framework services.
builder.Services.AddMvc();

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IDbFactory, DbFactory>();
builder.Services.AddTransient<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddTransient<IEmailLogService, EmailLogService>();
builder.Services.AddTransient<IEmailConfigService, EmailConfigService>();
builder.Services.AddTransient<IErrorLogService, ErrorLogService>();
builder.Services.AddTransient<IMapper, Mapper>();
builder.Services.AddTransient<IEmailProviderService, EmailProviderService>();
builder.Services.AddTransient<ILanguageService, LanguageService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<ITenantService, TenantService>();
builder.Services.AddTransient<IVerificationCodeService, VerificationCodeService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<ITenantConfigService, TenantConfigService>();
builder.Services.AddTransient<ITenantActivityService, TenantActivityService>();
builder.Services.AddTransient<IEmailProviderService, EmailProviderService>();
builder.Services.AddTransient<IERPSystemService, ERPSystemService>();
builder.Services.AddTransient<IUserERPSystemService, UserERPSystemService>();

// Start Custom field Services

builder.Services.AddTransient<ICustomControlService, CustomControlService>();
builder.Services.AddTransient<ICustomControlOptionService, CustomControlOptionService>();
builder.Services.AddTransient<ICustomFieldService, CustomFieldService>();
builder.Services.AddTransient<ICustomModuleService, CustomModuleService>();
builder.Services.AddTransient<IModuleFieldService, ModuleFieldService>();
builder.Services.AddTransient<ITenantModuleService, TenantModuleService>();
builder.Services.AddTransient<ICustomTenantFieldService, CustomTenantFieldService>();
builder.Services.AddTransient<ICustomTableService, CustomTableService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<ICustomFieldValueService, CustomFieldValueService>();

// End Custom field Services

builder.Services.AddTransient<ICustomerTypeService, CustomerTypeService>();
builder.Services.AddTransient<ITagService, TagService>();
builder.Services.AddTransient<IEmailPhoneNoTypeService, EmailPhoneNoTypeService>();
builder.Services.AddTransient<IOrganizationLabelService, OrganizationLabelService>();
builder.Services.AddTransient<ICustomerLabelService, CustomerLabelService>();
builder.Services.AddTransient<ICustomerEmailService, CustomerEmailService>();
builder.Services.AddTransient<ICustomerPhoneService, CustomerPhoneService>();
builder.Services.AddTransient<ICustomerNoteService, CustomerNoteService>();
builder.Services.AddTransient<IOrganizationService, OrganizationService>();
builder.Services.AddTransient<ICustomerAttachmentService, CustomerAttachmentService>();
builder.Services.AddTransient<IOrganizationNoteService, OrganizationNoteService>();
builder.Services.AddTransient<IOrganizationAttachmentService, OrganizationAttachmentService>();
builder.Services.AddTransient<ICustomerNotesCommentService, CustomerNotesCommentService>();
builder.Services.AddTransient<IOrganizationNotesCommentService, OrganizationNotesCommentService>();
builder.Services.AddTransient<ICustomTableColumnService, CustomTableColumnService>();
builder.Services.AddTransient<ITableColumnUserService, TableColumnUserService>();
builder.Services.AddTransient<ILeadLabelService, LeadLabelService>();
builder.Services.AddTransient<ILeadService, LeadService>();
builder.Services.AddTransient<ILeadNoteService, LeadNoteService>();
builder.Services.AddTransient<ILabelCategoryService, LabelCategoryService>();
builder.Services.AddTransient<ILabelService, LabelService>();

builder.Services.AddTransient<IActivityTypeService, ActivityTypeService>();
builder.Services.AddTransient<IActivityAvailabilityService, ActivityAvailabilityService>();
builder.Services.AddTransient<ICustomerActivityService, CustomerActivityService>();
builder.Services.AddTransient<ICustomerActivityMemberService, CustomerActivityMemberService>();
builder.Services.AddTransient<IOrganizationActivityService, OrganizationActivityService>();
builder.Services.AddTransient<IOrganizationActivityMemberService, OrganizationActivityMemberService>();
builder.Services.AddTransient<ILeadActivityService, LeadActivityService>();
builder.Services.AddTransient<ILeadActivityMemberService, LeadActivityMemberService>();
builder.Services.AddTransient<ICalendarListService, CalendarListService>();
builder.Services.AddTransient<ICalendarTaskService, CalendarTaskService>();
builder.Services.AddTransient<ICalendarSubTaskService, CalendarSubTaskService>();
builder.Services.AddTransient<ICalendarRepeatTypeService, CalendarRepeatTypeService>();
builder.Services.AddTransient<IOneClappLatestThemeService, OneClappLatestThemeService>();
builder.Services.AddTransient<IOneClappLatestThemeSchemeService, OneClappLatestThemeSchemeService>();
builder.Services.AddTransient<IOneClappLatestThemeLayoutService, OneClappLatestThemeLayoutService>();
builder.Services.AddTransient<IOneClappLatestThemeConfigService, OneClappLatestThemeConfigService>();
builder.Services.AddTransient<IGoogleCalendarService, GoogleCalendarService>();
builder.Services.AddTransient<IIntProviderService, IntProviderService>();
builder.Services.AddTransient<IIntProviderAppService, IntProviderAppService>();
builder.Services.AddTransient<IIntProviderAppSecretService, IntProviderAppSecretService>();
builder.Services.AddTransient<ICheckListService, CheckListService>();
builder.Services.AddTransient<ICheckListUserService, CheckListUserService>();
builder.Services.AddTransient<ICalendarSyncActivityService, CalendarSyncActivityService>();
builder.Services.AddTransient<IOneClappModuleService, OneClappModuleService>();
builder.Services.AddTransient<IOneClappFormTypeService, OneClappFormTypeService>();
builder.Services.AddTransient<IOneClappFormService, OneClappFormService>();
builder.Services.AddTransient<IOneClappFormFieldService, OneClappFormFieldService>();
builder.Services.AddTransient<ICheckListAssignUserService, CheckListAssignUserService>();
builder.Services.AddTransient<IOneClappFormStatusService, OneClappFormStatusService>();
builder.Services.AddTransient<IOneClappRequestFormService, OneClappRequestFormService>();
builder.Services.AddTransient<IOneClappFormFieldValueService, OneClappFormFieldValueService>();
builder.Services.AddTransient<IOneClappFormActionService, OneClappFormActionService>();
builder.Services.AddTransient<IWeClappUserService, WeClappUserService>();
builder.Services.AddTransient<IWeClappService, WeClappService>();
builder.Services.AddTransient<IERPSystemColumnMapService, ERPSystemColumnMapService>();
builder.Services.AddTransient<IModuleRecordCustomFieldService, ModuleRecordCustomFieldService>();

builder.Services.AddTransient<ISubscriptionPlanService, SubscriptionPlanService>();
builder.Services.AddTransient<ISubscriptionPlanDetailService, SubscriptionPlanDetailService>();
builder.Services.AddTransient<ISubscriptionTypeService, SubscriptionTypeService>();
builder.Services.AddTransient<IUserSubscriptionService, UserSubscriptionService>();
builder.Services.AddTransient<IMollieCustomerService, MollieCustomerService>();
builder.Services.AddTransient<IMollieSubscriptionService, MollieSubscriptionService>();
builder.Services.AddTransient<ISalutationService, SalutationService>();
builder.Services.AddTransient<IImportContactAttachmentService, ImportContactAttachmentService>();
builder.Services.AddTransient<IExternalUserService, ExternalUserService>();

// Start dynamic form design services
builder.Services.AddTransient<IBorderStyleService, BorderStyleService>();
builder.Services.AddTransient<IOneClappFormHeaderService, OneClappFormHeaderService>();
builder.Services.AddTransient<IOneClappFormLayoutBackgroundService, OneClappFormLayoutBackgroundService>();
builder.Services.AddTransient<IOneClappFormLayoutService, OneClappFormLayoutService>();
// End dynamic form design services

builder.Services.AddTransient<IDiscussionService, DiscussionService>();
builder.Services.AddTransient<IDiscussionReadService, DiscussionReadService>();
builder.Services.AddTransient<IDiscussionParticipantService, DiscussionParticipantService>();
builder.Services.AddTransient<IMailBoxTeamService, MailBoxTeamService>();
builder.Services.AddTransient<IMailCommentService, MailCommentService>();
builder.Services.AddTransient<IMailAssignUserService, MailAssignUserService>();
builder.Services.AddTransient<IMailReadService, MailReadService>();
builder.Services.AddTransient<IDiscussionCommentAttachmentService, DiscussionCommentAttachmentService>();
builder.Services.AddTransient<IDiscussionCommentService, DiscussionCommentService>();
builder.Services.AddTransient<ITeamInboxService, TeamInboxService>();
builder.Services.AddTransient<IMailCommentAttachmentService, MailCommentAttachmentService>();
builder.Services.AddTransient<IMailParticipantService, MailParticipantService>();
builder.Services.AddTransient<IMailAssignCustomerService, MailAssignCustomerService>();
builder.Services.AddTransient<ITeamInboxAccessService, TeamInboxAccessService>();
builder.Services.AddTransient<ICustomDomainEmailConfigService, CustomDomainEmailConfigService>();

builder.Services.AddTransient<IOneClappTaskService, OneClappTaskService>();
builder.Services.AddTransient<IOneClappSubTaskService, OneClappSubTaskService>();
builder.Services.AddTransient<IOneClappChildTaskService, OneClappChildTaskService>();
builder.Services.AddTransient<IOneClappTaskUserSerivce, OneClappTaskUserSerivce>();
builder.Services.AddTransient<ITaskStatusService, TaskStatusService>();
builder.Services.AddTransient<ITaskCommentService, TaskCommentService>();
builder.Services.AddTransient<ITaskActivityService, TaskActivityService>();
builder.Services.AddTransient<ITaskTimeRecordService, TaskTimeRecordService>();
builder.Services.AddTransient<ISectionService, SectionService>();
builder.Services.AddTransient<ISectionActivityService, SectionActivityService>();
builder.Services.AddTransient<ISubTaskActivityService, SubTaskActivityService>();
builder.Services.AddTransient<ISubTaskAttachmentService, SubTaskAttachmentService>();
builder.Services.AddTransient<ISubTaskCommentService, SubTaskCommentService>();
builder.Services.AddTransient<IOneClappSubTaskUserService, OneClappSubTaskUserService>();
builder.Services.AddTransient<ISubTaskTimeRecordService, SubTaskTimeRecordService>();
builder.Services.AddTransient<IChildTaskCommentService, ChildTaskCommentService>();
builder.Services.AddTransient<IChildTaskActivityService, ChildTaskActivityService>();
builder.Services.AddTransient<IChildTaskTimeRecordService, ChildTaskTimeRecordService>();
builder.Services.AddTransient<IOneClappChildTaskUserService, OneClappChildTaskUserService>();
builder.Services.AddTransient<ITaskAttachmentService, TaskAttachmentService>();
builder.Services.AddTransient<IChildTaskAttachmentService, ChildTaskAttachmentService>();
builder.Services.AddTransient<ITaskWeclappUserService, TaskWeclappUserService>();

builder.Services.AddTransient<IEmployeeProjectActivityService, EmployeeProjectActivityService>();
builder.Services.AddTransient<IEmployeeProjectService, EmployeeProjectService>();
builder.Services.AddTransient<IEmployeeProjectStatusService, EmployeeProjectStatusService>();
builder.Services.AddTransient<IEmployeeProjectUserService, EmployeeProjectUserService>();
builder.Services.AddTransient<IEmployeeTaskActivityService, EmployeeTaskActivityService>();
builder.Services.AddTransient<IEmployeeTaskService, EmployeeTaskService>();
builder.Services.AddTransient<IEmployeeTaskAttachmentService, EmployeeTaskAttachmentService>();
builder.Services.AddTransient<IEmployeeTaskCommentService, EmployeeTaskCommentService>();
builder.Services.AddTransient<IEmployeeTaskStatusService, EmployeeTaskStatusService>();
builder.Services.AddTransient<IEmployeeTaskTimeRecordService, EmployeeTaskTimeRecordService>();
builder.Services.AddTransient<IEmployeeTaskUserSerivce, EmployeeTaskUserSerivce>();

builder.Services.AddTransient<IEmployeeSubTaskService, EmployeeSubTaskService>();
builder.Services.AddTransient<IEmployeeSubTaskAttachmentService, EmployeeSubTaskAttachmentService>();
builder.Services.AddTransient<IEmployeeSubTaskCommentService, EmployeeSubTaskCommentService>();
builder.Services.AddTransient<IEmployeeSubTaskUserService, EmployeeSubTaskUserService>();
builder.Services.AddTransient<IEmployeeSubTaskTimeRecordService, EmployeeSubTaskTimeRecordService>();
builder.Services.AddTransient<IEmployeeSubTaskActivityService, EmployeeSubTaskActivityService>();
builder.Services.AddTransient<IEmployeeChildTaskService, EmployeeChildTaskService>();
builder.Services.AddTransient<IEmployeeChildTaskAttachmentService, EmployeeChildTaskAttachmentService>();
builder.Services.AddTransient<IEmployeeChildTaskCommentService, EmployeeChildTaskCommentService>();
builder.Services.AddTransient<IEmployeeChildTaskUserService, EmployeeChildTaskUserService>();
builder.Services.AddTransient<IEmployeeChildTaskTimeRecordService, EmployeeChildTaskTimeRecordService>();
builder.Services.AddTransient<IEmployeeChildTaskActivityService, EmployeeChildTaskActivityService>();
builder.Services.AddTransient<IMailBoxService, MailBoxService>();
builder.Services.AddTransient<IStatusService, StatusService>();
builder.Services.AddTransient<IServiceArticleCategoryService, ServiceArticleCategoryService>();
builder.Services.AddTransient<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<ITaxService, TaxService>();
builder.Services.AddTransient<ITaxRateService, TaxRateService>();
builder.Services.AddTransient<IServiceArticleService, ServiceArticleService>();
builder.Services.AddTransient<IServiceArticleHourService, ServiceArticleHourService>();
builder.Services.AddTransient<ICountryService, CountryService>();
builder.Services.AddTransient<IStateService, StateService>();
builder.Services.AddTransient<ICityService, CityService>();
builder.Services.AddTransient<IStandardTimeZoneService, StandardTimeZoneService>();
builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IClientEmailService, ClientEmailService>();
builder.Services.AddTransient<IClientPhoneService, ClientPhoneService>();
builder.Services.AddTransient<IContractTypeService, ContractTypeService>();
builder.Services.AddTransient<IContractService, ContractService>();
builder.Services.AddTransient<IContractArticleService, ContractArticleService>();
builder.Services.AddTransient<IInvoiceIntervalService, InvoiceIntervalService>();
builder.Services.AddTransient<IClientInvoiceService, ClientInvoiceService>();
builder.Services.AddTransient<IInvoiceMollieCustomerService, InvoiceMollieCustomerService>();
builder.Services.AddTransient<IInvoiceMollieSubscriptionService, InvoiceMollieSubscriptionService>();
builder.Services.AddTransient<IMateTimeRecordService, MateTimeRecordService>();
builder.Services.AddTransient<IMateProjectTimeRecordService, MateProjectTimeRecordService>();
builder.Services.AddTransient<IMateTaskTimeRecordService, MateTaskTimeRecordService>();
builder.Services.AddTransient<IContractActivityService, ContractActivityService>();
builder.Services.AddTransient<IContractInvoiceService, ContractInvoiceService>();
builder.Services.AddTransient<IProjectContractService, ProjectContractService>();
builder.Services.AddTransient<IMateSubTaskTimeRecordService, MateSubTaskTimeRecordService>();
builder.Services.AddTransient<IMateChildTaskTimeRecordService, MateChildTaskTimeRecordService>();
builder.Services.AddTransient<IMateCommentService, MateCommentService>();
builder.Services.AddTransient<IMateCommentAttachmentService, MateCommentAttachmentService>();
builder.Services.AddTransient<IMateTaskCommentService, MateTaskCommentService>();
builder.Services.AddTransient<IMateSubTaskCommentService, MateSubTaskCommentService>();
builder.Services.AddTransient<IMateChildTaskCommentService, MateChildTaskCommentService>();
builder.Services.AddTransient<IClientSocialMediaService, ClientSocialMediaService>();
builder.Services.AddTransient<IAssetsManufacturerService, AssetsManufacturerService>();
builder.Services.AddTransient<IContractAssetService, ContractAssetService>();
builder.Services.AddTransient<IDepartmentService, DepartmentService>();
builder.Services.AddTransient<ISatisficationLevelService, SatisficationLevelService>();
builder.Services.AddTransient<IClientUserRoleService, ClientUserRoleService>();
builder.Services.AddTransient<IClientUserService, ClientUserService>();
builder.Services.AddTransient<ICRMNotesService, CRMNotesService>();
builder.Services.AddTransient<IClientAppointmentService, ClientAppointmentService>();
builder.Services.AddTransient<IContactService, ContactService>();
builder.Services.AddTransient<IClientIntProviderAppSecretService, ClientIntProviderAppSecretService>();
builder.Services.AddTransient<IIntProviderContactService, IntProviderContactService>();
builder.Services.AddTransient<IProjectCategoryService, ProjectCategoryService>();
builder.Services.AddTransient<IServiceArticlePriceService, ServiceArticlePriceService>();
builder.Services.AddTransient<IMateCategoryService, MateCategoryService>();
builder.Services.AddTransient<IMatePriorityService, MatePriorityService>();
builder.Services.AddTransient<IEmployeeProjectTaskService, EmployeeProjectTaskService>();
builder.Services.AddTransient<IEmployeeClientTaskService, EmployeeClientTaskService>();
builder.Services.AddTransient<IMateTicketService, MateTicketService>();
builder.Services.AddTransient<IMateTicketActivityService, MateTicketActivityService>();
builder.Services.AddTransient<IMateTicketUserService, MateTicketUserService>();
builder.Services.AddTransient<IMateClientTicketService, MateClientTicketService>();
builder.Services.AddTransient<IMateProjectTicketService, MateProjectTicketService>();
builder.Services.AddTransient<IMateTicketTimeRecordService, MateTicketTimeRecordService>();
builder.Services.AddTransient<IMateTicketTaskService, MateTicketTaskService>();
builder.Services.AddTransient<IMateTicketCommentService, MateTicketCommentService>();
builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddTransient(typeof(IService<>), typeof(Service<>));

// BL Change
builder.Services.AddTransient<IProxyService, ProxyService>();

// Start Mollie Services

builder.Services.AddTransient<IPaymentOverviewClient, PaymentOverviewClient>();
builder.Services.AddTransient<ICustomerOverviewClient, CustomerOverviewClient>();
builder.Services.AddTransient<ISubscriptionOverviewClient, SubscriptionOverviewClient>();
builder.Services.AddTransient<IMandateOverviewClient, MandateOverviewClient>();
builder.Services.AddTransient<IPaymentMethodOverviewClient, PaymentMethodOverviewClient>();
builder.Services.AddTransient<IPaymentStorageClient, PaymentStorageClient>();
builder.Services.AddTransient<ICustomerStorageClient, CustomerStorageClient>();
builder.Services.AddTransient<ISubscriptionStorageClient, SubscriptionStorageClient>();
builder.Services.AddTransient<IMandateStorageClient, MandateStorageClient>();
builder.Services.AddTransient<IRefundPaymentClient, RefundPaymentClient>();
builder.Services.AddTransient<IPaymentClient, PaymentClient>(x => new PaymentClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<ICustomerClient, CustomerClient>(x => new CustomerClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IRefundClient, RefundClient>(x => new RefundClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IPaymentMethodClient, PaymentMethodClient>(x => new PaymentMethodClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<ISubscriptionClient, SubscriptionClient>(x => new SubscriptionClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IMandateClient, MandateClient>(x => new MandateClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IInvoicesClient, InvoicesClient>(x => new InvoicesClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IShipmentClient, ShipmentClient>(x => new ShipmentClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<ISettlementsClient, SettlementsClient>(x => new SettlementsClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IProfileClient, ProfileClient>(x => new ProfileClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IPermissionsClient, PermissionsClient>(x => new PermissionsClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IPaymentLinkClient, PaymentLinkClient>(x => new PaymentLinkClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IOrganizationsClient, OrganizationsClient>(x => new OrganizationsClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IOrderClient, OrderClient>(x => new OrderClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IOnboardingClient, OnboardingClient>(x => new OnboardingClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<ICaptureClient, CaptureClient>(x => new CaptureClient(OneClappContext.MollieApiKey));
builder.Services.AddTransient<IChargebacksClient, ChargebacksClient>(x => new ChargebacksClient(OneClappContext.MollieApiKey));

// End

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
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
builder.Services.AddHangfireServer();

var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<OneClappContext>();
    dataContext.Database.Migrate();
    DataSeeder.Seed(dataContext);
}


// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
// app.UseSwaggerUI();
app.UseSwaggerUI(c =>
           {
               c.SwaggerEndpoint("/swagger/v1/swagger.json", OneClappContext.ProjectName + ".api v1");
           });
app.UseDeveloperExceptionPage();
// }


app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});
// var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
// app.UseRequestLocalization(localizeOptions.Value);

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

// https://stackoverflow.com/questions/70802713/test-net6-minimal-api-with-automapper
// https://www.kafle.io/tutorials/asp-dot-net/automapper


var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
mapper = new Mapper(configuration);
// Get the Automapper, we can share this too



var emailTemplateService = app.Services.GetService<IEmailTemplateService>();
var emailLogService = app.Services.GetService<IEmailLogService>();
var emailConfigService = app.Services.GetService<IEmailConfigService>();
var emailProviderService = app.Services.GetService<IEmailProviderService>();

var userSubScriptionService = app.Services.GetService<IUserSubscriptionService>();
var subscriptionTypeService = app.Services.GetService<ISubscriptionTypeService>();
var mollieSubscriptionService = app.Services.GetService<IMollieSubscriptionService>();
var mollieCustomerService = app.Services.GetService<IMollieCustomerService>();
var subscriptionStorageClient = app.Services.GetService<ISubscriptionStorageClient>();
var invoiceMollieSubscriptionService = app.Services.GetService<IInvoiceMollieSubscriptionService>();
var paymentStorageClient = app.Services.GetService<IPaymentStorageClient>();
var clientService = app.Services.GetService<IClientService>();
var clientInvoiceService = app.Services.GetService<IClientInvoiceService>();
var contractService = app.Services.GetService<IContractService>();


sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);

paymentNotificationLogic = new PaymentNotificationLogic(userSubScriptionService, subscriptionTypeService, emailTemplateService,
emailLogService, emailProviderService, emailConfigService, mollieSubscriptionService, mollieCustomerService, subscriptionStorageClient,
invoiceMollieSubscriptionService, paymentStorageClient, clientInvoiceService, clientService, contractService, mapper);

// IMapper mapper = mappingConfig.CreateMapper();
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
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<BroadcastHub>("/notify");
});


// backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

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


// app.UseHttpsRedirection();

// app.UseAuthorization();

// app.MapControllers();

app.Run();


// public static class MigrationManager
// {


//     public static IHost MigrateDatabase(this IHost host)
//     {
//         using (var scope = host.Services.CreateScope())
//         {
//             using (var appContext = scope.ServiceProvider.GetRequiredService<OneClappContext>())
//             {
//                 try
//                 {
//                     if (!appContext.GetService<OneClappContext>().AllMigrationsApplied())
//                     {
//                         appContext.GetService<OneClappContext>().Database.Migrate();
//                         DataSeeder.Seed(appContext);
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     //Log errors or do anything you think it's needed
//                     throw ex;
//                 }
//             }
//         }

//         return host;
//     }
// }
