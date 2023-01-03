using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;
using matcrm.service.Utility;
using System.Dynamic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CustomTaskController : Controller
    {

        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserSerivce;
        private readonly IEmployeeSubTaskUserService _employeeSubTaskUserService;
        private readonly IEmployeeChildTaskUserService _employeeChildTaskUserService;
        private readonly IEmployeeSubTaskTimeRecordService _employeeSubTaskTimeRecordService;
        private readonly IEmployeeChildTaskTimeRecordService _employeeChildTaskTimeRecordService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmployeeTaskAttachmentService _employeeTaskAttachmentService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IEmployeeTaskCommentService _employeeTaskCommentService;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeTaskStatusService _employeeTaskStatusService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private readonly IEmployeeSubTaskAttachmentService _employeeSubTaskAttachmentService;
        private readonly IEmployeeChildTaskAttachmentService _employeeChildTaskAttachmentService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IStatusService _statusService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly ITableColumnUserService _tableColumnUserService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IModuleRecordCustomFieldService _moduleRecordCustomFieldService;
        private readonly IModuleFieldService _moduleFieldService;

        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        //
        private readonly IClientService _clientService;
        private readonly IContractService _contractService;
        private readonly IProjectContractService _projectContractService;
        private readonly IMateProjectTimeRecordService _mateProjectTimeRecordService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IContractArticleService _contractArticleService;
        private readonly ITaxRateService _taxRateService;
        private readonly IContractInvoiceService _contractInvoiceService;
        private readonly IClientInvoiceService _clientInvoiceService;
        private readonly IInvoiceIntervalService _invoiceIntervalService;
        private readonly IMateTaskTimeRecordService _mateTaskTimeRecordService;
        private readonly IMateSubTaskTimeRecordService _mateSubTaskTimeRecordService;
        private readonly IMateChildTaskTimeRecordService _mateChildTaskTimeRecordService;
        private readonly IClientEmailService _clientEmailService;
        private SendEmail sendEmail;
        //
        private readonly OneClappContext _context;
        private CustomFieldLogic customFieldLogic;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public CustomTaskController(IEmployeeTaskService employeeTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserService,
            IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
            IHostingEnvironment hostingEnvironment,
            IEmployeeTaskAttachmentService employeeTaskAttachmentService,
            IEmployeeTaskActivityService employeeTaskActivityService,
            IUserService userService,
            ICustomerService customerService,
            IEmployeeTaskCommentService employeeTaskCommentService,
            IEmployeeTaskStatusService employeeTaskStatusService,
            // IPdfTemplateService pdfTemplateService,
            ITenantConfigService tenantConfig,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeChildTaskService employeeChildTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserSerivce,
            IEmployeeSubTaskUserService employeeSubTaskUserService,
            IEmployeeChildTaskUserService employeeChildTaskUserService,
            IEmployeeSubTaskTimeRecordService employeeSubTaskTimeRecordService,
            IEmployeeChildTaskTimeRecordService employeeChildTaskTimeRecordService,
            IWeClappService weClappService,
            IEmployeeSubTaskAttachmentService employeeSubTaskAttachmentService,
            IEmployeeChildTaskAttachmentService employeeChildTaskAttachmentService,
            IEmployeeSubTaskActivityService employeeSubTaskActivityService,
            IEmployeeChildTaskActivityService employeeChildTaskActivityService,
            IEmployeeSubTaskCommentService employeeSubTaskCommentService,
            IEmployeeChildTaskCommentService employeeChildTaskCommentService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            ICustomTableColumnService customTableColumnService,
            ITableColumnUserService tableColumnUserService,
            IModuleRecordCustomFieldService moduleRecordCustomFieldService,
            ICustomModuleService customModuleService,
            IModuleFieldService moduleFieldService,
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomFieldService customFieldService,
            ITenantModuleService tenantModuleService,
            OneClappContext context,
            IMapper mapper,
            IStatusService statusService,
            IClientService clientService,
            IContractService contractService,
            IProjectContractService projectContractService,
            IMateProjectTimeRecordService mateProjectTimeRecordService,
            IMateTimeRecordService mateTimeRecordService,
            IContractArticleService contractArticleService,
            ITaxRateService taxRateService,
            IContractInvoiceService contractInvoiceService,
            IClientInvoiceService clientInvoiceService,
            IInvoiceIntervalService invoiceIntervalService,
            IMateTaskTimeRecordService mateTaskTimeRecordService,
            IMateSubTaskTimeRecordService mateSubTaskTimeRecordService,
            IMateChildTaskTimeRecordService mateChildTaskTimeRecordService,
            IClientEmailService clientEmailService)
        {
            _employeeTaskService = employeeTaskService;
            _employeeTaskUserService = employeeTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _hostingEnvironment = hostingEnvironment;
            _employeeTaskAttachmentService = employeeTaskAttachmentService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _userService = userService;
            _customerService = customerService;
            _employeeTaskCommentService = employeeTaskCommentService;
            _employeeTaskStatusService = employeeTaskStatusService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProvider = emailProvider;
            _employeeTaskService = employeeTaskService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeTaskUserSerivce = employeeTaskUserSerivce;
            _employeeSubTaskUserService = employeeSubTaskUserService;
            _employeeChildTaskUserService = employeeChildTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeSubTaskTimeRecordService = employeeSubTaskTimeRecordService;
            _employeeChildTaskTimeRecordService = employeeChildTaskTimeRecordService;
            _hostingEnvironment = hostingEnvironment;
            _employeeSubTaskAttachmentService = employeeSubTaskAttachmentService;
            _employeeChildTaskAttachmentService = employeeChildTaskAttachmentService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeTaskStatusService = employeeTaskStatusService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _customTableColumnService = customTableColumnService;
            _tableColumnUserService = tableColumnUserService;
            _moduleRecordCustomFieldService = moduleRecordCustomFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            _customFieldService = customFieldService;
            _tenantModuleService = tenantModuleService;
            _context = context;
            _mapper = mapper;
            _statusService = statusService;
            _clientService = clientService;
            _contractService = contractService;
            _projectContractService = projectContractService;
            _mateProjectTimeRecordService = mateProjectTimeRecordService;
            _mateTimeRecordService = mateTimeRecordService;
            _contractArticleService = contractArticleService;
            _taxRateService = taxRateService;
            _contractInvoiceService = contractInvoiceService;
            _clientInvoiceService = clientInvoiceService;
            _invoiceIntervalService = invoiceIntervalService;
            _mateTaskTimeRecordService = mateTaskTimeRecordService;
            _mateSubTaskTimeRecordService = mateSubTaskTimeRecordService;
            _mateChildTaskTimeRecordService = mateChildTaskTimeRecordService;
            _clientEmailService = clientEmailService;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
               customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // [HttpGet]
        // public async Task<OperationResult<CustomTaskCreateResponse>> Create()
        // {
        //     CustomTaskCreateResponse customTaskCreateResponseObj = new CustomTaskCreateResponse();
        //     List<Client> ClientList = _clientService.GetAll().Where(t => t.IsContractBaseInvoice == true).ToList(); //get client for contract based invoice           
        //     //start for client loop    
        //     foreach (var clientObj in ClientList)
        //     {
        //         CustomTaskClient customTaskClientObj = new CustomTaskClient();
        //         customTaskClientObj = _mapper.Map<CustomTaskClient>(clientObj);
        //         customTaskCreateResponseObj.Clients.Add(customTaskClientObj);

        //         var clientContracts = _contractService.GetByClient(clientObj.Id).OrderBy(t => t.Id).ToList(); //get all contract for client id                
        //         //start for contract loop
        //         foreach (var contractObj in clientContracts)
        //         {
        //             CustomTaskContract customTaskContractObj = new CustomTaskContract();
        //             customTaskContractObj = _mapper.Map<CustomTaskContract>(contractObj);

        //             DateTime? StartDate = null;
        //             DateTime? EndDate = null;

        //             var projectContractList = _projectContractService.GetByContractId(contractObj.Id); //get project contract list by contract                    

        //             var ProjectIdList = projectContractList.Select(t => t.ProjectId.Value).ToList();

        //             var employeeTaskIdList = _employeeTaskService.GetAllByProjectIdList(ProjectIdList).Select(t => t.Id).ToList();//get task list from project ids    

        //             var contractInvoiceList = _contractInvoiceService.GetAllByContract(contractObj.Id); //get client invoice from contract invoice based on contract
        //             var ClientInvoiceIds = contractInvoiceList.Select(t => t.ClientInvoiceId.Value).ToList();//get clientinvoice id fron contract invoice
        //             var clientInvoiceList = _clientInvoiceService.GetListByIdList(ClientInvoiceIds).OrderBy(t => t.EndDate).ToList();
        //             //start logic for start and enddate
        //             if (clientInvoiceList != null && clientInvoiceList.Count() > 0)
        //             {
        //                 var ClientInvoiceLastRecord = clientInvoiceList.LastOrDefault(); //last record of client invoice
        //                 if (ClientInvoiceLastRecord != null && ClientInvoiceLastRecord.EndDate != null)
        //                 {
        //                     StartDate = ClientInvoiceLastRecord.EndDate.Value.AddDays(1).AddHours(0).AddMinutes(0).AddSeconds(0);
        //                     //add invoice interval from contract in start date and get enddate
        //                     EndDate = StartDate.Value.AddDays(contractObj.InvoiceInterval.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                 }
        //             }
        //             else
        //             {
        //                 //no invoice generate mns call first invoice
        //                 if (contractObj.IsBillingFromStartDate)
        //                 {
        //                     StartDate = contractObj.StartDate;
        //                 }
        //                 else
        //                 {
        //                     // start logic for get startdate from first time start timerecord date for generate first invoice startDate                                                                      
        //                     var taskStartDate = _mateTaskTimeRecordService.GetTaskTimeRecordStartDate(employeeTaskIdList);
        //                     var projectStartDate = _mateProjectTimeRecordService.GetProjectTimeRecordStartDate(ProjectIdList);
        //                     if (taskStartDate != null && projectStartDate == null)
        //                     {
        //                         StartDate = taskStartDate;
        //                     }
        //                     else if (taskStartDate == null && projectStartDate != null)
        //                     {
        //                         StartDate = projectStartDate;
        //                     }
        //                     else if (projectStartDate != null && taskStartDate != null)
        //                     {
        //                         StartDate = taskStartDate > projectStartDate ? projectStartDate : StartDate;
        //                     }
        //                 }
        //                 if (StartDate != null)
        //                 {
        //                     StartDate = StartDate.Value.AddHours(0).AddMinutes(0).AddSeconds(0);
        //                     EndDate = StartDate.Value.AddDays(contractObj.InvoiceInterval.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                 }

        //             }
        //             //end logic for start and enddate

        //             DateTime TodayDate = DateTime.UtcNow.Date;
        //             //DateTime TodayDate = Convert.ToDateTime("31-10-2022");

        //             long? totalInvoiceAmount = 0; //total invoice amount without discount
        //             long? FinalInvoiceAmount = 0; //with discount
        //             long? ProjectContractTotalAmount = 0; //project amount
        //             long? TaskContractTotalAmount = 0; //task amount
        //             long? SubTaskTotalAmountProject = 0; //sub task amount
        //             long? ChildTaskTotalAmountProject = 0; //child task amount

        //             //if (StartDate != null && EndDate != null)    
        //             if (StartDate != null && EndDate != null && EndDate.Value.Date == TodayDate.Date)
        //             {
        //                 var contractArticles = _contractArticleService.GetByContract(contractObj.Id);
        //                 var contractArticleIdList = contractArticles.Select(t => t.ServiceArticleId).ToList();

        //                 CustomTaskProject customTaskProjectObj = new CustomTaskProject();

        //                 //------Project------
        //                 foreach (var projectId in ProjectIdList)
        //                 {
        //                     customTaskProjectObj = new CustomTaskProject();
        //                     customTaskProjectObj.projectId = projectId;

        //                     //get project time record by project id which contain contract service article
        //                     var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectId, StartDate.Value, EndDate.Value)
        //                                                                                 .Where(t => t.MateTimeRecord != null && contractArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                     long? TotalProjectAmount = 0;
        //                     long? TotalProjectTaxAmount = 0;
        //                     long? PayableProjectAmount = 0;

        //                     foreach (var projectTimeRecord in mateProjectTimeRecordList)
        //                     {
        //                         if (projectTimeRecord.MateTimeRecord != null)
        //                         {
        //                             var ProjectContractArticleObj = contractArticles.Where(t => t.ServiceArticleId == projectTimeRecord.MateTimeRecord?.ServiceArticleId).FirstOrDefault();
        //                             //var ProjectContractArticleObj = _contractArticleService.GetByContractAndServiceArticle(contractObj.Id, projectTimeRecord.MateTimeRecord.ServiceArticleId.Value);
        //                             if (ProjectContractArticleObj != null)
        //                             {
        //                                 if (contractObj != null && ProjectContractArticleObj.IsContractUnitPrice && contractObj.DefaultUnitPrice != null)
        //                                 {
        //                                     TotalProjectAmount = (contractObj?.DefaultUnitPrice) * (projectTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                                 else
        //                                 {
        //                                     TotalProjectAmount = (projectTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                             }

        //                             if (projectTimeRecord.MateTimeRecord?.ServiceArticle != null && projectTimeRecord.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                             {
        //                                 var taxId = projectTimeRecord.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                 if (taxId != null)
        //                                 {
        //                                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                     TotalProjectTaxAmount = (((projectTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecord.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 TotalProjectTaxAmount = 0;
        //                             }
        //                             PayableProjectAmount = TotalProjectAmount + TotalProjectTaxAmount;
        //                             ProjectContractTotalAmount = ProjectContractTotalAmount + PayableProjectAmount.Value;

        //                             CustomTaskTimeRecord customProjectTimeRecordObj = new CustomTaskTimeRecord();
        //                             customProjectTimeRecordObj.Id = projectTimeRecord.Id;
        //                             customProjectTimeRecordObj.Duration = projectTimeRecord.MateTimeRecord?.Duration;
        //                             customProjectTimeRecordObj.Comment = projectTimeRecord.MateTimeRecord?.Comment;
        //                             customProjectTimeRecordObj.IsBillable = projectTimeRecord.MateTimeRecord?.IsBillable;
        //                             customProjectTimeRecordObj.ServiceArticleId = projectTimeRecord.MateTimeRecord?.ServiceArticle?.Id;
        //                             customProjectTimeRecordObj.IsContractUnitPrice = ProjectContractArticleObj.IsContractUnitPrice;
        //                             customProjectTimeRecordObj.UnitPrice = projectTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                             customProjectTimeRecordObj.TotalAmount = TotalProjectAmount;
        //                             customProjectTimeRecordObj.TotalTaxAmount = TotalProjectTaxAmount;
        //                             customProjectTimeRecordObj.PayableAmount = PayableProjectAmount;
        //                             customTaskProjectObj.ProjectTimeRecords.Add(customProjectTimeRecordObj);
        //                         }
        //                     }
        //                     customTaskProjectObj.ProjectContractTotalAmount = ProjectContractTotalAmount;
        //                     customTaskContractObj.Projects.Add(customTaskProjectObj);
        //                 }
        //                 //------Project------

        //                 //------Task------
        //                 CustomTaskTask customTaskTaskObj = new CustomTaskTask();
        //                 foreach (var taskObj in employeeTaskIdList)
        //                 {
        //                     customTaskTaskObj = new CustomTaskTask();
        //                     customTaskTaskObj.TaskId = taskObj;

        //                     var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj, StartDate.Value, EndDate.Value)
        //                                                .Where(t => t.MateTimeRecord != null && contractArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                     long? TotalTaskAmountWithoutProject = 0;
        //                     long? TotalTaskTaxAmountWithoutProject = 0;
        //                     long? PayableTaskAmountWithOutProject = 0;

        //                     foreach (var taskTimeRecord in mateTaskTimeRecordList)
        //                     {
        //                         if (taskTimeRecord.MateTimeRecord != null)
        //                         {
        //                             var TaskContractArticleObj = contractArticles.Where(t => t.ServiceArticleId == taskTimeRecord.MateTimeRecord?.ServiceArticleId).FirstOrDefault();
        //                             //var TaskContractArticleObj = _contractArticleService.GetByContractAndServiceArticle(contractObj.Id, taskTimeRecord.MateTimeRecord.ServiceArticleId.Value);
        //                             if (TaskContractArticleObj != null)
        //                             {
        //                                 if (contractObj != null && TaskContractArticleObj.IsContractUnitPrice && contractObj.DefaultUnitPrice != null)
        //                                 {
        //                                     TotalTaskAmountWithoutProject = (contractObj?.DefaultUnitPrice) * (taskTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                                 else
        //                                 {
        //                                     TotalTaskAmountWithoutProject = (taskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (taskTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                             }

        //                             if (taskTimeRecord.MateTimeRecord != null && taskTimeRecord.MateTimeRecord != null && taskTimeRecord.MateTimeRecord?.ServiceArticle != null && taskTimeRecord.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                             {
        //                                 var taxId = taskTimeRecord.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                 if (taxId != null)
        //                                 {
        //                                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                     TotalTaskTaxAmountWithoutProject = (((taskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (taskTimeRecord.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 TotalTaskTaxAmountWithoutProject = 0;
        //                             }
        //                             PayableTaskAmountWithOutProject = TotalTaskAmountWithoutProject + TotalTaskTaxAmountWithoutProject;

        //                             TaskContractTotalAmount = TaskContractTotalAmount + PayableTaskAmountWithOutProject.Value;

        //                             CustomTaskTimeRecord customTaskTimeRecordObj = new CustomTaskTimeRecord();
        //                             customTaskTimeRecordObj.Id = taskTimeRecord.Id;
        //                             customTaskTimeRecordObj.Duration = taskTimeRecord.MateTimeRecord?.Duration;
        //                             customTaskTimeRecordObj.Comment = taskTimeRecord.MateTimeRecord?.Comment;
        //                             customTaskTimeRecordObj.IsBillable = taskTimeRecord.MateTimeRecord?.IsBillable;
        //                             customTaskTimeRecordObj.ServiceArticleId = taskTimeRecord.MateTimeRecord?.ServiceArticle?.Id;
        //                             customTaskTimeRecordObj.IsContractUnitPrice = TaskContractArticleObj.IsContractUnitPrice;
        //                             customTaskTimeRecordObj.UnitPrice = taskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                             customTaskTimeRecordObj.TotalAmount = TotalTaskAmountWithoutProject;
        //                             customTaskTimeRecordObj.TotalTaxAmount = TotalTaskTaxAmountWithoutProject;
        //                             customTaskTimeRecordObj.PayableAmount = PayableTaskAmountWithOutProject;
        //                             customTaskTaskObj.TaskTimeRecords.Add(customTaskTimeRecordObj);
        //                         }
        //                     }
        //                     customTaskTaskObj.TaskContractTotalAmount = TaskContractTotalAmount;
        //                     customTaskProjectObj.Tasks.Add(customTaskTaskObj);
        //                 }
        //                 //------Task------

        //                 //------Sub Task------
        //                 CustomTaskSubTask customTaskSubTaskObj = new CustomTaskSubTask();
        //                 var SubTaskIdList = _employeeSubTaskService.GetAllActiveByTaskIds(employeeTaskIdList).Select(t => t.Id).ToList();
        //                 foreach (var subtaskObj in SubTaskIdList)
        //                 {
        //                     customTaskSubTaskObj = new CustomTaskSubTask();
        //                     customTaskSubTaskObj.SubTaskId = subtaskObj;


        //                     var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTask(subtaskObj, StartDate.Value, EndDate.Value)
        //                                                .Where(t => t.MateTimeRecord != null && contractArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                     long? TotalSubTaskAmount = 0;
        //                     long? TotalSubTaskTaxAmount = 0;
        //                     long? PayableSubTaskAmount = 0;

        //                     foreach (var subTaskTimeRecord in mateSubTaskTimeRecordList)
        //                     {
        //                         if (subTaskTimeRecord.MateTimeRecord != null)
        //                         {
        //                             var SubTaskContractArticleObj = contractArticles.Where(t => t.ServiceArticleId == subTaskTimeRecord.MateTimeRecord?.ServiceArticleId).FirstOrDefault();
        //                             //var SubTaskContractArticleObj = _contractArticleService.GetByContractAndServiceArticle(contractObj.Id, subTaskTimeRecord.MateTimeRecord.ServiceArticleId.Value);
        //                             if (SubTaskContractArticleObj != null)
        //                             {
        //                                 if (contractObj != null && SubTaskContractArticleObj.IsContractUnitPrice && contractObj.DefaultUnitPrice != null)
        //                                 {
        //                                     TotalSubTaskAmount = (contractObj?.DefaultUnitPrice) * (subTaskTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                                 else
        //                                 {
        //                                     TotalSubTaskAmount = (subTaskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (subTaskTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                             }

        //                             if (subTaskTimeRecord.MateTimeRecord != null && subTaskTimeRecord.MateTimeRecord != null && subTaskTimeRecord.MateTimeRecord?.ServiceArticle != null && subTaskTimeRecord.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                             {
        //                                 var taxId = subTaskTimeRecord.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                 if (taxId != null)
        //                                 {
        //                                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                     TotalSubTaskTaxAmount = (((subTaskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (subTaskTimeRecord.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 TotalSubTaskTaxAmount = 0;
        //                             }
        //                             PayableSubTaskAmount = TotalSubTaskAmount + TotalSubTaskTaxAmount;

        //                             SubTaskTotalAmountProject = SubTaskTotalAmountProject + PayableSubTaskAmount.Value;

        //                             CustomTaskTimeRecord customSubTaskTimeRecordObj = new CustomTaskTimeRecord();
        //                             customSubTaskTimeRecordObj.Id = subTaskTimeRecord.Id;
        //                             customSubTaskTimeRecordObj.Duration = subTaskTimeRecord.MateTimeRecord?.Duration;
        //                             customSubTaskTimeRecordObj.Comment = subTaskTimeRecord.MateTimeRecord?.Comment;
        //                             customSubTaskTimeRecordObj.IsBillable = subTaskTimeRecord.MateTimeRecord?.IsBillable;
        //                             customSubTaskTimeRecordObj.ServiceArticleId = subTaskTimeRecord.MateTimeRecord?.ServiceArticle?.Id;
        //                             customSubTaskTimeRecordObj.IsContractUnitPrice = SubTaskContractArticleObj.IsContractUnitPrice;
        //                             customSubTaskTimeRecordObj.UnitPrice = subTaskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                             customSubTaskTimeRecordObj.TotalAmount = TotalSubTaskAmount;
        //                             customSubTaskTimeRecordObj.TotalTaxAmount = TotalSubTaskTaxAmount;
        //                             customSubTaskTimeRecordObj.PayableAmount = PayableSubTaskAmount;
        //                             customTaskSubTaskObj.SubTimeRecords.Add(customSubTaskTimeRecordObj);
        //                         }
        //                     }
        //                     customTaskSubTaskObj.SubTaskTotalAmountProject = SubTaskTotalAmountProject;
        //                     customTaskTaskObj.SubTasks.Add(customTaskSubTaskObj);
        //                 }
        //                 //------Sub Task------

        //                 //------Child Task------
        //                 var ChildTaskIdList = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIdList).Select(t => t.Id).ToList();
        //                 foreach (var childTask in ChildTaskIdList)
        //                 {
        //                     CustomTaskChildTask customTaskChildTaskObj = new CustomTaskChildTask();
        //                     customTaskChildTaskObj.ChildTaskId = childTask;

        //                     var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTask(childTask, StartDate.Value, EndDate.Value)
        //                                                .Where(t => t.MateTimeRecord != null && contractArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                     long? TotalChildTaskAmount = 0;
        //                     long? TotalChildTaskTaxAmount = 0;
        //                     long? PayableChildTaskAmount = 0;

        //                     foreach (var childTaskTimeRecord in mateChildTaskTimeRecordList)
        //                     {
        //                         if (childTaskTimeRecord.MateTimeRecord != null)
        //                         {
        //                             var childTaskContractArticleObj = contractArticles.Where(t => t.ServiceArticleId == childTaskTimeRecord.MateTimeRecord?.ServiceArticleId).FirstOrDefault();
        //                             //var childTaskContractArticleObj = _contractArticleService.GetByContractAndServiceArticle(contractObj.Id, childTaskTimeRecord.MateTimeRecord.ServiceArticleId.Value);
        //                             if (childTaskContractArticleObj != null)
        //                             {
        //                                 if (contractObj != null && childTaskContractArticleObj.IsContractUnitPrice && contractObj.DefaultUnitPrice != null)
        //                                 {
        //                                     TotalChildTaskAmount = (contractObj?.DefaultUnitPrice) * (childTaskTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                                 else
        //                                 {
        //                                     TotalChildTaskAmount = (childTaskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (childTaskTimeRecord.MateTimeRecord?.Duration);
        //                                 }
        //                             }

        //                             if (childTaskTimeRecord.MateTimeRecord != null && childTaskTimeRecord.MateTimeRecord != null && childTaskTimeRecord.MateTimeRecord?.ServiceArticle != null && childTaskTimeRecord.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                             {
        //                                 var taxId = childTaskTimeRecord.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                 if (taxId != null)
        //                                 {
        //                                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                     TotalChildTaskTaxAmount = (((childTaskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice) * (childTaskTimeRecord.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 TotalChildTaskTaxAmount = 0;
        //                             }
        //                             PayableChildTaskAmount = TotalChildTaskAmount + TotalChildTaskTaxAmount;

        //                             ChildTaskTotalAmountProject = ChildTaskTotalAmountProject + PayableChildTaskAmount.Value;

        //                             CustomTaskTimeRecord customChildTimeRecordObj = new CustomTaskTimeRecord();
        //                             customChildTimeRecordObj.Id = childTaskTimeRecord.Id;
        //                             customChildTimeRecordObj.Duration = childTaskTimeRecord.MateTimeRecord?.Duration;
        //                             customChildTimeRecordObj.Comment = childTaskTimeRecord.MateTimeRecord?.Comment;
        //                             customChildTimeRecordObj.IsBillable = childTaskTimeRecord.MateTimeRecord?.IsBillable;
        //                             customChildTimeRecordObj.ServiceArticleId = childTaskTimeRecord.MateTimeRecord?.ServiceArticle?.Id;
        //                             customChildTimeRecordObj.IsContractUnitPrice = childTaskContractArticleObj.IsContractUnitPrice;
        //                             customChildTimeRecordObj.UnitPrice = childTaskTimeRecord.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                             customChildTimeRecordObj.TotalAmount = TotalChildTaskAmount;
        //                             customChildTimeRecordObj.TotalTaxAmount = TotalChildTaskTaxAmount;
        //                             customChildTimeRecordObj.PayableAmount = PayableChildTaskAmount;
        //                             customTaskChildTaskObj.ChildTimeRecords.Add(customChildTimeRecordObj);
        //                         }
        //                     }
        //                     customTaskChildTaskObj.ChildTaskTotalAmountProject = ChildTaskTotalAmountProject;
        //                     customTaskSubTaskObj.ChildTasks.Add(customTaskChildTaskObj);
        //                 }
        //                 //------Child Task------

        //                 //get invoice amount from all project,task,subtask,child task TotalAmount with out discount    
        //                 totalInvoiceAmount = ProjectContractTotalAmount + TaskContractTotalAmount + SubTaskTotalAmountProject + ChildTaskTotalAmountProject;
        //                 //for discount
        //                 if (contractObj.Discount != null)
        //                 {
        //                     FinalInvoiceAmount = totalInvoiceAmount - (totalInvoiceAmount * (contractObj.Discount) / 100);
        //                 }
        //                 else
        //                 {
        //                     FinalInvoiceAmount = totalInvoiceAmount;
        //                 }

        //                 //---Start for client invoice entry---
        //                 ClientInvoice clientInvoiceObj = new ClientInvoice();
        //                 clientInvoiceObj.TotalAmount = FinalInvoiceAmount;
        //                 clientInvoiceObj.ClientId = clientObj.Id;
        //                 clientInvoiceObj.InvoiceDate = DateTime.UtcNow;
        //                 clientInvoiceObj.StartDate = StartDate;
        //                 clientInvoiceObj.EndDate = EndDate;
        //                 clientInvoiceObj.InvoiceNo = DateTime.UtcNow.ToString("ddMMyyhhmmss");
        //                 // clientInvoiceObj.CreatedBy = UserId;
        //                 var addedClientInvoice = await _clientInvoiceService.CheckInsertOrUpdate(clientInvoiceObj);
        //                 //---Start for contract invoice entry---
        //                 if (addedClientInvoice != null)
        //                 {
        //                     ContractInvoice contractInvoiceObj = new ContractInvoice();
        //                     contractInvoiceObj.ClientInvoiceId = addedClientInvoice.Id;
        //                     contractInvoiceObj.ContractId = contractObj.Id;
        //                     var AddcontractInvoiceObj = await _contractInvoiceService.CheckInsertOrUpdate(contractInvoiceObj);
        //                     if (AddcontractInvoiceObj != null)
        //                     {
        //                         //For contract wise email notification
        //                         if (contractObj.ClientId != null)
        //                         {
        //                             var PrimaryClientEmailObj = _clientEmailService.GetByClientIdWithPrimary(contractObj.ClientId.Value);
        //                             if (PrimaryClientEmailObj != null)
        //                             {
        //                                 string ClientName = clientObj.FirstName + ' ' + clientObj.LastName;
        //                                 await sendEmail.SendEmailContractBasedInvoiceNotification(PrimaryClientEmailObj.Email, ClientName, contractObj.Name, addedClientInvoice.StartDate, addedClientInvoice.EndDate, FinalInvoiceAmount, contractObj.Discount);
        //                             }
        //                         }
        //                     }
        //                 }
        //                 //---End for contract invoice entry---
        //                 //---End for client invoice entry--- 

        //             }
        //             customTaskContractObj.Discount = contractObj.Discount;
        //             customTaskContractObj.FinalInvoiceAmount = FinalInvoiceAmount;
        //             customTaskClientObj.Contracts.Add(customTaskContractObj);
        //         }
        //         //end for contract loop
        //     }
        //     //End for client loop
        //     return new OperationResult<CustomTaskCreateResponse>(true, System.Net.HttpStatusCode.OK, "", customTaskCreateResponseObj);
        // }


        // Save Time Record [Task]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<CustomTaskListResponse>> GroupTask()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            CustomTaskListResponse TaskList = new CustomTaskListResponse();

            TaskList.GroupColumnList = await DefaultGroupColumns();

            List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();

            employeeTaskList = _employeeTaskService.GetAllActiveByTenant(TenantId);
            var TaskKeyValueData = employeeTaskList.GroupBy(t => t?.StatusId);

            foreach (var item in TaskKeyValueData)
            {
                TaskGroupBY taskGroupBY = new TaskGroupBY();
                var TaskIds = item.Select(t => t.Id).ToList();
                taskGroupBY.TaskIds = TaskIds;
                Status? statusObj = null;
                if (item.Key != null)
                {
                    taskGroupBY.Id.Add(item.Key.Value.ToString());
                    statusObj = _statusService.GetById(item.Key.Value);
                }

                taskGroupBY.Data.Add(statusObj);
                TaskList.TaskGroupBY.Add(taskGroupBY);
            }

            // Start logic for columns

            CustomTableDto customTableDto = new CustomTableDto();

            var selectedTable = _customTableService.GetByName("EmployeeTask");
            // var allTables = _customTableService.GetAll ();

            var allTableColumns = _customTableColumnService.GetAll();

            var defaultColumns = allTableColumns.Where(t => t.MasterTableId == selectedTable.Id && t.IsDefault == true).ToList();
            var customFields = allTableColumns.Where(t => t.MasterTableId == selectedTable.Id && t.IsDefault == false && t.TenantId == TenantId).ToList();
            var selectedTableColumnUsers = _tableColumnUserService.GetAllByTable(selectedTable.Id, UserId);

            List<ModuleRecordCustomField> moduleRecordFieldList = new List<ModuleRecordCustomField>();

            //var moduleObj = _customModuleService.GetByName(Model.TableName);
            CustomModule? customModuleObj = null;
            if (selectedTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(selectedTable.Id);
            }
            if (customModuleObj != null)
            {
                var moduleFieldList = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                var moduleFieldIds = moduleFieldList.Select(x => x.Id).ToList();

                moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIds);
            }

            if (moduleRecordFieldList != null && moduleRecordFieldList.Count() > 0)
            {
                foreach (var moduleRecordCustomField in moduleRecordFieldList)
                {
                    var isExistData = customFields.Where(t => t.CustomFieldId == moduleRecordCustomField.ModuleField.FieldId).FirstOrDefault();
                    if (isExistData != null)
                    {
                        customFields.Remove(isExistData);
                    }
                }
            }

            if (defaultColumns != null && defaultColumns.Count() > 0)
            {
                foreach (var item in defaultColumns)
                {
                    var Obj = selectedTableColumnUsers.Where(t => t.MasterTableId == item.MasterTableId && t.TableColumnId == item.Id).FirstOrDefault();
                    var customTableColumnDto = _mapper.Map<CustomTableColumnDto>(item);
                    if (Obj != null)
                    {
                        customTableColumnDto.IsHide = Obj.IsHide;
                        customTableColumnDto.Priority = Obj.Priority;
                    }
                    else
                    {
                        customTableColumnDto.IsHide = false;
                        customTableColumnDto.Priority = null;
                    }
                    if (item.CustomControl != null)
                    {
                        customTableColumnDto.ControlType = item.CustomControl.Name;
                    }
                    if (customTableColumnDto.IsHide)
                    {
                        customTableDto.HideColumns.Add(customTableColumnDto);
                    }
                    else
                    {
                        customTableDto.ShowColumns.Add(customTableColumnDto);
                    }
                }
            }

            if (customFields != null && customFields.Count() > 0)
            {
                foreach (var item in customFields)
                {
                    var Obj = selectedTableColumnUsers.Where(t => t.MasterTableId == item.MasterTableId && t.TableColumnId == item.Id).FirstOrDefault();
                    var customTableColumnDto = _mapper.Map<CustomTableColumnDto>(item);
                    if (Obj != null)
                    {
                        customTableColumnDto.IsHide = Obj.IsHide;
                        customTableColumnDto.Priority = Obj.Priority;
                    }
                    else
                    {
                        customTableColumnDto.IsHide = false;
                        customTableColumnDto.Priority = null;
                    }
                    if (item.CustomControl != null)
                    {
                        customTableColumnDto.ControlType = item.CustomControl.Name;
                    }
                    if (customTableColumnDto.IsHide)
                    {
                        customTableDto.HideColumns.Add(customTableColumnDto);
                    }
                    else
                    {
                        customTableDto.ShowColumns.Add(customTableColumnDto);
                    }
                }
            }
            // ColumnDto.ShowColumns = ColumnDto.ShowColumns.Where(t => t.TenantId == Model.TenantId).OrderBy(t => t.Priority).ToList();

            var nullPriorityColumns = customTableDto.ShowColumns.Where(t => t.Priority == null).ToList();
            customTableDto.ShowColumns = customTableDto.ShowColumns.Where(t => t.Priority != null).OrderBy(t => t.Priority).ToList();

            if (nullPriorityColumns != null && nullPriorityColumns.Count() > 0)
            {
                foreach (var item in nullPriorityColumns)
                {
                    customTableDto.ShowColumns.Add(item);
                }
            }

            TaskList.Columns = _mapper.Map<List<CustomTableColumnResponse>>(customTableDto.ShowColumns);
            var i = 1;
            foreach (var ColumnObj in TaskList.Columns)
            {
                if (i == 1)
                {
                    ColumnObj.Fx = "40%";
                }
                if (ColumnObj.Name.ToLower() != "duration")
                {
                    ColumnObj.IsInsert = true;
                }
                if (ColumnObj.Name.ToLower() == "Assignee")
                {
                    ColumnObj.ControlType = "MultiDropDown";
                }
                if (!string.IsNullOrEmpty(ColumnObj.Name))
                {
                    // ColumnObj.DisplayName = ColumnObj.Name;
                    if (ColumnObj.Name.ToLower() == "status")
                    {

                    }
                    switch (ColumnObj.Name.ToLower())
                    {
                        case "status":
                            ColumnObj.Key = "statusId";
                            break;
                        case "assignee":
                            ColumnObj.Key = "assignee";
                            break;
                        default:
                            var columnname = FirstCharToLowerCase(ColumnObj.Name);
                            ColumnObj.Key = columnname?.ToString().Replace(" ", "");
                            break;
                    }
                    // ColumnObj.Name = ColumnObj.Name.ToLower().ToString().Replace(" ", "");
                }
                i++;
            }
            TaskList.SelectedGroupBy = "Status";
            // var responseCustomTableDto = _mapper.Map<CustomModuleGetAllColumnResponse>(customTableDto);

            // End logic for columns

            return new OperationResult<CustomTaskListResponse>(true, System.Net.HttpStatusCode.OK, "", TaskList);


        }

        private async Task<List<CustomTaskGroupColumnResponse>> DefaultGroupColumns()
        {
            List<CustomTaskGroupColumnResponse> TaskGroupColumns = new List<CustomTaskGroupColumnResponse>();
            // var ColumnNames = new List<string> { "Name", "Status", "Assignee" };
            var ColumnNames = new List<string> { "Status" };
            var i = 1;
            foreach (var ColumnName in ColumnNames)
            {
                CustomTaskGroupColumnResponse Column1 = new CustomTaskGroupColumnResponse();
                Column1.Id = i;
                Column1.Name = ColumnName;
                TaskGroupColumns.Add(Column1);
                i++;
            }
            return TaskGroupColumns.ToList();
        }

        // Get All Tasks
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<dynamic>>> DynamicTaskList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<dynamic> dynamicTaskList = new List<dynamic>();

            List<User> UserList = _userService.GetAll();

            List<CustomTaskVM> employeeTaskListVMObj = new List<CustomTaskVM>();

            //var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
            var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId).OrderBy(t => t.Id).ToList();
            if (tasks.Count() == 0)
            {
                return new OperationResult<List<dynamic>>(true, System.Net.HttpStatusCode.OK, "", dynamicTaskList);
            }


            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("EmployeeTask");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            if (customModuleObj != null)
            {
                CustomModuleDto customModuleDto = new CustomModuleDto();
                customModuleDto.TenantId = TenantId;
                customModuleDto.MasterTableId = customModuleObj.MasterTableId;
                customModuleDto.Id = customModuleObj.Id;
                var customerList = _customerService.GetAllByTenant(TenantId);
                if (tasks != null && tasks.Count() > 0)
                {
                    foreach (var item in tasks)
                    {
                        // dynamic myDynamic = item as dynamic;
                        dynamic myDynamic = new ExpandoObject();
                        List<Dictionary<string, object>> myDynamicList = new List<Dictionary<string, object>>();
                        // IDictionary<string, object> myUnderlyingObject = myDynamic;
                        customModuleDto.RecordId = item.Id;
                        var CustomFields = await customFieldLogic.GetCustomField(customModuleDto);
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        foreach (var customFieldObj in CustomFields)
                        {

                            if (customFieldObj.CustomControl != null)
                            {
                                var controlTypeName = customFieldObj.CustomControl.Name;
                                switch (controlTypeName.ToLower())
                                {
                                    case "textbox":
                                    case "textarea":
                                    case "date":
                                    case "heading":
                                        {
                                            var answer = "";
                                            if (customFieldObj.CustomFieldValues.Count() > 0)
                                            {
                                                foreach (var valueObj in customFieldObj.CustomFieldValues)
                                                {
                                                    answer = answer + valueObj.Value;
                                                }
                                            }
                                            data[customFieldObj.Name] = answer;
                                            // myDynamicList.Add(data);
                                            break;
                                        }
                                    case "dropdown":
                                    case "radio":
                                    case "checkbox":
                                        {
                                            var answer = "";
                                            if (customFieldObj.CustomFieldValues.Count() > 0)
                                            {
                                                foreach (var valueObj in customFieldObj.CustomFieldValues)
                                                {
                                                    answer = answer + valueObj.Value;
                                                }
                                            }
                                            data[customFieldObj.Name] = answer;
                                            //   myDynamicList.Add(data);
                                            break;
                                        }

                                    default:
                                        break;
                                }
                            }

                            // myDynamic.Add(customFieldObj.Key, "");
                        }

                        foreach (var dynamicObj in data)
                        {
                            if (!string.IsNullOrEmpty(dynamicObj.Key))
                            {
                                // myDynamic[dynamicObj.Key] = dynamicObj.Value;
                                ((IDictionary<String, Object>)myDynamic)[dynamicObj.Key] = dynamicObj.Value;
                            }
                        }
                        PropertyInfo[] propInfos = typeof(CustomTaskVM).GetProperties();

                        // foreach (var p in propInfos)
                        // {
                        var jsonString = JsonConvert.SerializeObject(item);
                        JObject json = JObject.Parse(jsonString);
                        Console.WriteLine(json);

                        var objProperties = json.ToObject<Dictionary<string, object>>();
                        foreach (var obj in objProperties)
                        {
                            if (!string.IsNullOrEmpty(obj.Key))
                            {
                                // myDynamic[dynamicObj.Key] = dynamicObj.Value;
                                ((IDictionary<String, Object>)myDynamic)[obj.Key] = obj.Value;
                            }
                        }
                        //     Console.WriteLine(string.Format("Property name: {0}", p.Name));
                        // }
                        Console.WriteLine(myDynamic);
                        dynamicTaskList.Add(myDynamic);
                    }
                }
            }

            var taskIdList = tasks.Select(t => t.Id).ToList();
            // employeeTaskListVMObj.Tasks = new List<CustomSubTaskVM>();

            if (tasks != null && tasks.Count() > 0)
            {
                foreach (var taskObj in tasks)
                {
                    var employeeTaskVMObj = _mapper.Map<CustomTaskVM>(taskObj);
                    // employeeTaskVMObj.Status = taskObj.StatusId;
                    if (taskObj.Status != null)
                    {
                        employeeTaskVMObj.StatusName = taskObj.Status.Name;
                    }
                    if (employeeTaskVMObj.Id != null)
                    {
                        var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(employeeTaskVMObj.Id.Value);
                        employeeTaskVMObj.Duration = taskTotalDuration;
                    }
                    var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id);
                    if (assignTaskUsers.Count > 0)
                    {
                        employeeTaskVMObj.Assignee = new List<CustomTaskUserVM>();
                        // var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
                        // employeeTaskVMObj.Assignee = assignUsersVMList;
                        foreach (var assignUserItem in assignTaskUsers)
                        {
                            if (assignUserItem.User != null)
                            {
                                var assignObj = _mapper.Map<CustomTaskUserVM>(assignUserItem.User);
                                employeeTaskVMObj.Assignee.Add(assignObj);
                            }

                            var UserObj = UserList.Where(t => t.Id == assignUserItem.Id).FirstOrDefault();
                            if (UserObj != null)
                            {
                                if (!string.IsNullOrEmpty(employeeTaskVMObj.AssigneeName))
                                {
                                    employeeTaskVMObj.AssigneeName = employeeTaskVMObj.AssigneeName + ",";
                                }
                                if (!string.IsNullOrEmpty(UserObj.FirstName) && !string.IsNullOrEmpty(UserObj.LastName))
                                {
                                    employeeTaskVMObj.AssigneeName = employeeTaskVMObj.AssigneeName + UserObj.FirstName + " " + UserObj.LastName;
                                }
                                else
                                {
                                    employeeTaskVMObj.AssigneeName = employeeTaskVMObj.AssigneeName + UserObj.Email;
                                }
                            }
                        }
                    }

                    var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(taskObj.Id);

                    if (subTasks != null && subTasks.Count() > 0)
                    {
                        foreach (var subTask in subTasks)
                        {
                            var employeeSubTaskVMObj = _mapper.Map<CustomSubTaskVM>(subTask);
                            // employeeSubTaskVMObj.Status = subTask.StatusId;
                            if (subTask.Status != null)
                            {
                                employeeSubTaskVMObj.StatusName = subTask.Status.Name;
                            }
                            if (employeeSubTaskVMObj.Id != null)
                            {
                                var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(employeeSubTaskVMObj.Id);
                                employeeSubTaskVMObj.Duration = subTaskTotalDuration;
                            }
                            var assignSubTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(taskObj.Id);
                            if (assignSubTaskUsers.Count > 0)
                            {
                                employeeSubTaskVMObj.Assignee = new List<EmployeeSubTaskUserDto>();
                                var assignSubTaskUsersVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignSubTaskUsers);
                                employeeSubTaskVMObj.Assignee = assignSubTaskUsersVMList;
                                foreach (var assignSubTaskUserItem in assignSubTaskUsers)
                                {
                                    var UserObj = UserList.Where(t => t.Id == assignSubTaskUserItem.Id).FirstOrDefault();
                                    if (UserObj != null)
                                    {
                                        if (!string.IsNullOrEmpty(employeeSubTaskVMObj.AssigneeName))
                                        {
                                            employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + ",";
                                        }
                                        if (!string.IsNullOrEmpty(UserObj.FirstName) && !string.IsNullOrEmpty(UserObj.LastName))
                                        {
                                            employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + UserObj.FirstName + " " + UserObj.LastName;
                                        }
                                        else
                                        {
                                            employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + UserObj.Email;
                                        }
                                    }
                                }
                            }

                            var subTaskId = subTask.Id;

                            var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);

                            if (childTasks != null && childTasks.Count() > 0)
                            {
                                foreach (var childTaskObj in childTasks)
                                {
                                    var childTaskId = childTaskObj.Id;

                                    var employeeChildTaskVMObj = _mapper.Map<CustomChildTaskVM>(childTaskObj);
                                    // employeeChildTaskVMObj.Status = childTaskObj.StatusId;
                                    if (childTaskObj.Status != null)
                                    {
                                        employeeChildTaskVMObj.StatusName = childTaskObj.Status.Name;
                                    }
                                    if (employeeChildTaskVMObj.Id != null)
                                    {
                                        var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(employeeChildTaskVMObj.Id);
                                        employeeChildTaskVMObj.Duration = childTaskTotalDuration;
                                    }
                                    var assignChildTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(childTaskId);
                                    if (assignChildTaskUsers.Count > 0)
                                    {
                                        employeeChildTaskVMObj.Assignee = new List<EmployeeChildTaskUserDto>();
                                        var assignChildTaskUsersVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
                                        employeeChildTaskVMObj.Assignee = assignChildTaskUsersVMList;

                                        foreach (var assignChildTaskUserItem in assignChildTaskUsers)
                                        {
                                            var UserObj = UserList.Where(t => t.Id == assignChildTaskUserItem.Id).FirstOrDefault();
                                            if (UserObj != null)
                                            {
                                                if (!string.IsNullOrEmpty(employeeSubTaskVMObj.AssigneeName))
                                                {
                                                    employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + ",";
                                                }

                                                if (!string.IsNullOrEmpty(UserObj.FirstName) && !string.IsNullOrEmpty(UserObj.LastName))
                                                {
                                                    employeeChildTaskVMObj.AssigneeName = employeeChildTaskVMObj.AssigneeName + UserObj.FirstName + " " + UserObj.LastName;
                                                }
                                                else
                                                {
                                                    employeeChildTaskVMObj.AssigneeName = employeeChildTaskVMObj.AssigneeName + UserObj.Email;
                                                }
                                            }
                                        }
                                    }
                                    employeeSubTaskVMObj.Tasks.Add(employeeChildTaskVMObj);
                                }
                            }
                            employeeTaskVMObj.Tasks.Add(employeeSubTaskVMObj);
                        }
                    }
                    employeeTaskListVMObj.Add(employeeTaskVMObj);
                }
            }

            return new OperationResult<List<dynamic>>(true, System.Net.HttpStatusCode.OK, "", dynamicTaskList);
        }

        // Get All Tasks
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CustomTaskVM>>> TaskList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            List<User> UserList = _userService.GetAll();

            List<CustomTaskVM> employeeTaskListVMObj = new List<CustomTaskVM>();

            //var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
            var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
            if (tasks.Count() == 0)
            {
                return new OperationResult<List<CustomTaskVM>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskListVMObj);
            }

            var taskIdList = tasks.Select(t => t.Id).ToList();
            // employeeTaskListVMObj.Tasks = new List<CustomSubTaskVM>();

            if (tasks != null && tasks.Count() > 0)
            {
                foreach (var taskObj in tasks)
                {
                    var employeeTaskVMObj = _mapper.Map<CustomTaskVM>(taskObj);
                    var MainTaskId = taskObj.Id;
                    // employeeTaskVMObj.Status = taskObj.StatusId;
                    employeeTaskVMObj.MainTaskId = MainTaskId;
                    if (taskObj.Status != null)
                    {
                        employeeTaskVMObj.StatusName = taskObj.Status.Name;
                    }
                    if (employeeTaskVMObj.Id != null)
                    {
                        var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(employeeTaskVMObj.Id.Value);
                        employeeTaskVMObj.Duration = taskTotalDuration;
                    }
                    var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id);
                    if (assignTaskUsers.Count > 0)
                    {
                        employeeTaskVMObj.Assignee = new List<CustomTaskUserVM>();
                        var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
                        // employeeTaskVMObj.Assignee = assignUsersVMList;
                        foreach (var assignUserItem in assignTaskUsers)
                        {
                            if (assignUserItem.User != null)
                            {
                                var assignee = _mapper.Map<CustomTaskUserVM>(assignUserItem.User);
                                // assignee.EmployeeTaskId = assignUserItem.EmployeeTaskId;
                                employeeTaskVMObj.Assignee.Add(assignee);
                            }
                            var UserObj = UserList.Where(t => t.Id == assignUserItem.Id).FirstOrDefault();
                            if (UserObj != null)
                            {
                                if (!string.IsNullOrEmpty(employeeTaskVMObj.AssigneeName))
                                {
                                    employeeTaskVMObj.AssigneeName = employeeTaskVMObj.AssigneeName + ",";
                                }
                                if (!string.IsNullOrEmpty(UserObj.FirstName) && !string.IsNullOrEmpty(UserObj.LastName))
                                {
                                    employeeTaskVMObj.AssigneeName = employeeTaskVMObj.AssigneeName + UserObj.FirstName + " " + UserObj.LastName;
                                }
                                else
                                {
                                    employeeTaskVMObj.AssigneeName = employeeTaskVMObj.AssigneeName + UserObj.Email;
                                }
                            }
                        }
                    }

                    var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(taskObj.Id);

                    if (subTasks != null && subTasks.Count() > 0)
                    {

                        foreach (var subTask in subTasks)
                        {
                            var employeeSubTaskVMObj = _mapper.Map<CustomSubTaskVM>(subTask);
                            employeeSubTaskVMObj.MainTaskId = MainTaskId;
                            // employeeSubTaskVMObj.Status = subTask.StatusId;
                            if (subTask.Status != null)
                            {
                                employeeSubTaskVMObj.StatusName = subTask.Status.Name;
                            }
                            if (employeeSubTaskVMObj.Id != null)
                            {
                                var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(employeeSubTaskVMObj.Id);
                                employeeSubTaskVMObj.Duration = subTaskTotalDuration;
                            }
                            var assignSubTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(taskObj.Id);
                            if (assignSubTaskUsers.Count > 0)
                            {
                                employeeSubTaskVMObj.Assignee = new List<EmployeeSubTaskUserDto>();
                                var assignSubTaskUsersVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignSubTaskUsers);
                                employeeSubTaskVMObj.Assignee = assignSubTaskUsersVMList;
                                foreach (var assignSubTaskUserItem in assignSubTaskUsers)
                                {
                                    var UserObj = UserList.Where(t => t.Id == assignSubTaskUserItem.Id).FirstOrDefault();
                                    if (UserObj != null)
                                    {
                                        if (!string.IsNullOrEmpty(employeeSubTaskVMObj.AssigneeName))
                                        {
                                            employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + ",";
                                        }
                                        if (!string.IsNullOrEmpty(UserObj.FirstName) && !string.IsNullOrEmpty(UserObj.LastName))
                                        {
                                            employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + UserObj.FirstName + " " + UserObj.LastName;
                                        }
                                        else
                                        {
                                            employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + UserObj.Email;
                                        }
                                    }
                                }
                            }

                            var subTaskId = subTask.Id;

                            var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);

                            if (childTasks != null && childTasks.Count() > 0)
                            {
                                foreach (var childTaskObj in childTasks)
                                {
                                    var childTaskId = childTaskObj.Id;

                                    var employeeChildTaskVMObj = _mapper.Map<CustomChildTaskVM>(childTaskObj);
                                    employeeChildTaskVMObj.MainTaskId = MainTaskId;
                                    // employeeChildTaskVMObj.Status = childTaskObj.StatusId;
                                    if (childTaskObj.Status != null)
                                    {
                                        employeeChildTaskVMObj.StatusName = childTaskObj.Status.Name;
                                    }
                                    if (employeeChildTaskVMObj.Id != null)
                                    {
                                        var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(employeeChildTaskVMObj.Id);
                                        employeeChildTaskVMObj.Duration = childTaskTotalDuration;
                                    }
                                    var assignChildTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(childTaskId);
                                    if (assignChildTaskUsers.Count > 0)
                                    {
                                        employeeChildTaskVMObj.Assignee = new List<EmployeeChildTaskUserDto>();
                                        var assignChildTaskUsersVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
                                        employeeChildTaskVMObj.Assignee = assignChildTaskUsersVMList;

                                        foreach (var assignChildTaskUserItem in assignChildTaskUsers)
                                        {
                                            var UserObj = UserList.Where(t => t.Id == assignChildTaskUserItem.Id).FirstOrDefault();
                                            if (UserObj != null)
                                            {
                                                if (!string.IsNullOrEmpty(employeeSubTaskVMObj.AssigneeName))
                                                {
                                                    employeeSubTaskVMObj.AssigneeName = employeeSubTaskVMObj.AssigneeName + ",";
                                                }

                                                if (!string.IsNullOrEmpty(UserObj.FirstName) && !string.IsNullOrEmpty(UserObj.LastName))
                                                {
                                                    employeeChildTaskVMObj.AssigneeName = employeeChildTaskVMObj.AssigneeName + UserObj.FirstName + " " + UserObj.LastName;
                                                }
                                                else
                                                {
                                                    employeeChildTaskVMObj.AssigneeName = employeeChildTaskVMObj.AssigneeName + UserObj.Email;
                                                }
                                            }
                                        }
                                    }
                                    employeeSubTaskVMObj.Tasks.Add(employeeChildTaskVMObj);
                                }
                            }
                            employeeTaskVMObj.Tasks.Add(employeeSubTaskVMObj);
                        }
                    }
                    employeeTaskListVMObj.Add(employeeTaskVMObj);
                }
            }

            return new OperationResult<List<CustomTaskVM>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskListVMObj);
        }

        // Add Update Task
        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost]
        // public async Task<OperationResult<CustomTaskVM>> AddUpdate([FromBody] AddUpdateCustomTaskRequestResponse taskRequestModel)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     var userList = _userService.GetAll();
        //     CustomTaskVM taskDetail = new CustomTaskVM();
        //     List<CustomSubTaskVM> subTaskVMs = new List<CustomSubTaskVM>();
        //     List<CustomChildTaskVM> childTaskVMs = new List<CustomChildTaskVM>();

        //     long? TaskId = null;
        //     if (taskRequestModel.EmployeeSubTaskId != null)
        //     {
        //         var subTaskObj = _employeeSubTaskService.GetSubTaskById(taskRequestModel.EmployeeSubTaskId.Value);
        //         TaskId = subTaskObj?.EmployeeTaskId ?? null;
        //         var employeeChildTaskDto = _mapper.Map<EmployeeChildTaskDto>(taskRequestModel);
        //         employeeChildTaskDto.IsActive = true;
        //         if (taskRequestModel.Id == null)
        //         {
        //             employeeChildTaskDto.CreatedBy = UserId;
        //         }
        //         else
        //         {
        //             employeeChildTaskDto.Id = taskRequestModel.Id;
        //             employeeChildTaskDto.UpdatedBy = UserId;
        //         }

        //         var childTaskResult = await _employeeChildTaskService.CheckInsertOrUpdate(employeeChildTaskDto);
        //         if (childTaskResult != null)
        //         {
        //             employeeChildTaskDto.Id = childTaskResult.Id;
        //             taskRequestModel.Id = childTaskResult.Id;
        //         }

        //         EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
        //         employeeChildTaskActivityObj.UserId = UserId;
        //         employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskResult?.Id ?? null;
        //         if (taskRequestModel.Id == null)
        //         {
        //             employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Created.ToString().Replace("_", " ");
        //         }
        //         else
        //         {
        //             employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Updated.ToString().Replace("_", " ");
        //         }

        //         var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

        //         // if (taskRequestModel.AssignedUsers != null && taskRequestModel.AssignedUsers.Count() > 0)
        //         // {
        //         //     foreach (var userObj in taskRequestModel.AssignedUsers)
        //         //     {
        //         //         EmployeeChildTaskUserDto employeeChildTaskUserDto = new EmployeeChildTaskUserDto();
        //         //         employeeChildTaskUserDto.EmployeeChildTaskId = childTaskResult?.Id ?? null;
        //         //         employeeChildTaskUserDto.UserId = userObj.UserId;
        //         //         var isExist = _employeeTaskUserService.IsExistOrNot(employeeChildTaskUserDto);
        //         //         var employeeChildTaskUserObj = _employeeChildTaskUserService.CheckInsertOrUpdate(employeeChildTaskUserDto);
        //         //         if (employeeChildTaskUserObj != null)
        //         //         {
        //         //             userObj.Id = employeeChildTaskUserObj.Id;
        //         //         }
        //         //         if (!isExist)
        //         //         {
        //         //             if (employeeChildTaskUserDto.UserId != null)
        //         //             {
        //         //                 var userAssignDetails = _userService.GetUserById(employeeChildTaskUserDto.UserId.Value);
        //         //                 if (userAssignDetails != null)
        //         //                     await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, employeeTask.Description, TenantId);
        //         //                 EmployeeChildTaskActivity employeeAssignChildTaskActivityObj = new EmployeeChildTaskActivity();
        //         //                 employeeAssignChildTaskActivityObj.EmployeeChildTaskId = childTaskResult?.Id ?? null;
        //         //                 employeeAssignChildTaskActivityObj.UserId = UserId;
        //         //                 // employeeTaskActivityObj1.Activity = "Assigned the user";
        //         //                 employeeAssignChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_assigned_to_user.ToString().Replace("_", " ");
        //         //                 var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeAssignChildTaskActivityObj);
        //         //             }
        //         //         }
        //         //     }
        //         //     employeeTaskDto.AssignedUsers = new List<EmployeeChildTaskUser>();
        //         //     // employeeTaskDto.AssignedUsers = employeeTask.AssignedUsers;
        //         // }
        //         // var employeeTaskAddUpdateResponse = _mapper.Map<EmployeeTaskAddUpdateResponse>(employeeTaskDto);
        //         // employeeTaskAddUpdateResponse.AssignedUsers = taskRequestModel.AssignedUsers;
        //     }
        //     else if (taskRequestModel.EmployeeTaskId != null)
        //     {
        //         TaskId = taskRequestModel.EmployeeTaskId;
        //         var employeeSubTaskDto = _mapper.Map<EmployeeSubTaskDto>(taskRequestModel);
        //         employeeSubTaskDto.IsActive = true;
        //         // employeeSubTaskDto.TenantId = TenantId;
        //         if (taskRequestModel.Id == null)
        //         {
        //             employeeSubTaskDto.CreatedBy = UserId;
        //         }
        //         else
        //         {
        //             employeeSubTaskDto.Id = taskRequestModel.Id;
        //             employeeSubTaskDto.UpdatedBy = UserId;
        //         }

        //         var subTaskResult = await _employeeSubTaskService.CheckInsertOrUpdate(employeeSubTaskDto);
        //         if (subTaskResult != null)
        //         {
        //             employeeSubTaskDto.Id = subTaskResult.Id;
        //             taskRequestModel.Id = subTaskResult.Id;
        //         }

        //         EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
        //         employeeSubTaskActivityObj.UserId = UserId;
        //         employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskResult?.Id ?? null;
        //         if (taskRequestModel.Id == null)
        //         {
        //             employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Created.ToString().Replace("_", " ");
        //         }
        //         else
        //         {
        //             employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Updated.ToString().Replace("_", " ");
        //         }


        //         var AddUpdate = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

        //         // if (taskRequestModel.AssignedUsers != null && taskRequestModel.AssignedUsers.Count() > 0)
        //         // {
        //         //     foreach (var userObj in taskRequestModel.AssignedUsers)
        //         //     {
        //         //         EmployeeSubTaskUserDto employeeSubTaskUserDto = new EmployeeSubTaskUserDto();
        //         //         employeeSubTaskUserDto.EmployeeSubTaskId = subTaskResult?.Id ?? null;
        //         //         employeeSubTaskUserDto.UserId = userObj.UserId;
        //         //         var isExist = _employeeSubTaskUserService.IsExistOrNot(employeeSubTaskUserDto);
        //         //         var employeeSubTaskUserObj = _employeeSubTaskUserService.CheckInsertOrUpdate(employeeSubTaskUserDto);
        //         //         if (employeeSubTaskUserObj != null)
        //         //         {
        //         //             userObj.Id = employeeSubTaskUserObj.Id;
        //         //         }
        //         //         if (!isExist)
        //         //         {
        //         //             if (employeeSubTaskUserDto.UserId != null)
        //         //             {
        //         //                 var userAssignDetails = _userService.GetUserById(employeeSubTaskUserDto.UserId.Value);
        //         //                 if (userAssignDetails != null)
        //         //                     await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, subTaskResult?.Description, TenantId);
        //         //                 EmployeeSubTaskActivity employeeAssignSubTaskActivityObj = new EmployeeSubTaskActivity();
        //         //                 employeeAssignSubTaskActivityObj.EmployeeSubTaskId = subTaskResult?.Id ?? null;
        //         //                 employeeAssignSubTaskActivityObj.UserId = UserId;
        //         //                 employeeAssignSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_assigned_to_user.ToString().Replace("_", " ");
        //         //                 var AddUpdate1 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeAssignSubTaskActivityObj);
        //         //             }
        //         //         }
        //         //     }
        //         //     employeeSubTaskUserDto.AssignedUsers = new List<EmployeeTaskUser>();
        //         //     // employeeTaskDto.AssignedUsers = employeeTask.AssignedUsers;
        //         // }
        //         // var employeeTaskAddUpdateResponse = _mapper.Map<EmployeeTaskAddUpdateResponse>(employeeSubTaskUserDto);
        //         // employeeTaskAddUpdateResponse.AssignedUsers = taskRequestModel.AssignedUsers;
        //     }
        //     else
        //     {
        //         var employeeTaskDto = _mapper.Map<EmployeeTaskDto>(taskRequestModel);
        //         employeeTaskDto.IsActive = true;
        //         employeeTaskDto.TenantId = TenantId;
        //         if (taskRequestModel.Id == null)
        //         {
        //             employeeTaskDto.CreatedBy = UserId;
        //         }
        //         else
        //         {
        //             TaskId = taskRequestModel.Id;
        //             employeeTaskDto.Id = taskRequestModel.Id;
        //             employeeTaskDto.UpdatedBy = UserId;
        //         }

        //         var taskResult = await _employeeTaskService.CheckInsertOrUpdate(employeeTaskDto);
        //         if (taskResult != null)
        //         {
        //             employeeTaskDto.Id = taskResult.Id;
        //             taskRequestModel.Id = taskResult.Id;
        //             TaskId = taskResult.Id;
        //         }

        //         EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
        //         employeeTaskActivityObj.UserId = UserId;
        //         employeeTaskActivityObj.ProjectId = taskResult?.ProjectId ?? null;
        //         employeeTaskActivityObj.EmployeeTaskId = taskResult?.Id ?? null;
        //         if (taskRequestModel.Id == null)
        //         {
        //             employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Created.ToString().Replace("_", " ");
        //         }
        //         else
        //         {
        //             employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Updated.ToString().Replace("_", " ");
        //         }


        //         var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

        //         // if (taskRequestModel.AssignedUsers != null && taskRequestModel.AssignedUsers.Count() > 0)
        //         // {
        //         //     foreach (var userObj in taskRequestModel.AssignedUsers)
        //         //     {
        //         //         EmployeeTaskUserDto employeeTaskUserDto = new EmployeeTaskUserDto();
        //         //         employeeTaskUserDto.EmployeeTaskId = taskResult.Id;
        //         //         employeeTaskUserDto.UserId = userObj.UserId;
        //         //         employeeTaskUserDto.CreatedBy = UserId;
        //         //         var isExist = _employeeTaskUserService.IsExistOrNot(employeeTaskUserDto);
        //         //         var employeeTaskUserObj = _employeeTaskUserService.CheckInsertOrUpdate(employeeTaskUserDto);
        //         //         if (employeeTaskUserObj != null)
        //         //         {
        //         //             userObj.Id = employeeTaskUserObj.Id;
        //         //         }
        //         //         if (!isExist)
        //         //         {
        //         //             if (employeeTaskUserDto.UserId != null)
        //         //             {
        //         //                 var userAssignDetails = _userService.GetUserById(employeeTaskUserDto.UserId.Value);
        //         //                 if (userAssignDetails != null)
        //         //                     await sendEmail.SendEmailEmployeeTaskUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, employeeTask.Description, TenantId);
        //         //                 EmployeeTaskActivity employeeTaskActivityObj1 = new EmployeeTaskActivity();
        //         //                 employeeTaskActivityObj1.EmployeeTaskId = taskResult.Id;
        //         //                 employeeTaskActivityObj1.UserId = UserId;
        //         //                 employeeTaskActivityObj1.ProjectId = taskResult.ProjectId;
        //         //                 // employeeTaskActivityObj1.Activity = "Assigned the user";
        //         //                 employeeTaskActivityObj1.Activity = Enums.EmployeeTaskActivityEnum.Task_assigned_to_user.ToString().Replace("_", " ");
        //         //                 var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj1);
        //         //             }
        //         //         }
        //         //     }
        //         //     employeeTaskDto.AssignedUsers = new List<EmployeeTaskUser>();
        //         //     // employeeTaskDto.AssignedUsers = employeeTask.AssignedUsers;
        //         // }
        //         // var employeeTaskAddUpdateResponse = _mapper.Map<EmployeeTaskAddUpdateResponse>(employeeTaskDto);
        //         // employeeTaskAddUpdateResponse.AssignedUsers = taskRequestModel.AssignedUsers;
        //     }

        //     if (TaskId != null)
        //     {
        //         taskDetail = await Detail(TaskId.Value);
        //     }

        //     // For all subtask with completed status then main task automatic completed status
        //     // if (employeeTask.Id != null) {
        //     //     var taskId = employeeTask.Id.Value;
        //     //     var subTasks = _subTaskService.GetAllSubTaskByTask (taskId);
        //     //     if (employeeTask.UserId != null && subTasks.Count () > 0) {
        //     //         var userId = employeeTask.UserId.Value;
        //     //         var employeeTaskStatusList = _employeeTaskService.GetTaskByUser (userId);
        //     //         var finalStatus = employeeTaskStatusList.Where (t => t.IsFinalize == true).FirstOrDefault ();
        //     //         if (finalStatus != null) {
        //     //             var completedSubTaskCount = subTasks.Where (t => t.StatusId == finalStatus.Id).Count ();
        //     //             if (subTasks.Count () == completedSubTaskCount) {
        //     //                 taskObj.StatusId = finalStatus.Id;
        //     //             }
        //     //         }
        //     //     }
        //     // }

        //     return new OperationResult<CustomTaskVM>(true, System.Net.HttpStatusCode.OK, "Task saved successfully.", taskDetail);
        // }

        // Add Update Task
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CustomTaskVM>> Remove([FromBody] AddUpdateCustomTaskRequestResponse taskRequestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            long? TaskId = null;
            CustomTaskVM? taskDetail = new CustomTaskVM();
            List<CustomSubTaskVM> subTaskVMs = new List<CustomSubTaskVM>();
            List<CustomChildTaskVM> childTaskVMs = new List<CustomChildTaskVM>();

            if (taskRequestModel.EmployeeTaskId == null && taskRequestModel.EmployeeSubTaskId != null && taskRequestModel.Id != null)
            {
                long ChildTaskId = taskRequestModel.Id.Value;
                var subTaskObj = _employeeChildTaskService.GetChildTaskById(ChildTaskId);
                if (subTaskObj != null)
                {
                    TaskId = subTaskObj?.EmployeeSubTask?.EmployeeTaskId;
                }
                var documents = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(taskRequestModel.Id.Value);

                // Remove child task documents from folder
                if (documents != null && documents.Count() > 0)
                {
                    foreach (var childTaskDoc in documents)
                    {

                        //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeChildTaskUpload";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeChildTaskUploadDirPath;
                        var filePath = dirPath + "\\" + childTaskDoc.Name;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                    }
                }

                var comments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(ChildTaskId);

                var timeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(ChildTaskId);

                var taskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(ChildTaskId);

                EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                employeeChildTaskActivityObj.EmployeeChildTaskId = ChildTaskId;
                employeeChildTaskActivityObj.UserId = UserId;
                employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Removed.ToString().Replace("_", " ");
                var AddUpdate = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
                var childTaskToDelete = await _employeeChildTaskService.Delete(ChildTaskId);

                var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(ChildTaskId);
            }
            else if (taskRequestModel.EmployeeTaskId != null && taskRequestModel.Id != null && taskRequestModel.EmployeeSubTaskId == null)
            {
                long SubTaskId = taskRequestModel.Id.Value;
                TaskId = taskRequestModel.EmployeeTaskId.Value;
                var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(SubTaskId);

                foreach (var item in childTasks)
                {
                    var childTaskId = item.Id;

                    var childDocuments = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                    // Remove child task documents from folder

                    foreach (var childTaskDoc in childDocuments)
                    {

                        //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                        var filePath = dirPath + "\\" + childTaskDoc.Name;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                    }

                    var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                    var childTimeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(childTaskId);

                    var childTaskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(childTaskId);

                    var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

                    var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);

                    EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                    employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
                    employeeChildTaskActivityObj.UserId = UserId;
                    employeeChildTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Removed.ToString().Replace("_", " ");
                    var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);
                }

                var subDocuments = await _employeeSubTaskAttachmentService.DeleteAttachmentByEmployeeSubTaskId(SubTaskId);

                // Remove sub task documents from folder

                foreach (var subTaskDoc in subDocuments)
                {

                    //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
                    var filePath = dirPath + "\\" + subTaskDoc.Name;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }
                }

                var subComments = await _employeeSubTaskCommentService.DeleteCommentByEmployeeSubTaskId(SubTaskId);

                var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(SubTaskId);

                var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(SubTaskId);



                var subTaskToDelete = await _employeeSubTaskService.Delete(SubTaskId);

                EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                employeeSubTaskActivityObj.EmployeeSubTaskId = SubTaskId;
                employeeSubTaskActivityObj.UserId = UserId;
                employeeSubTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_Removed.ToString().Replace("_", " ");
                var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                var subTaskActivities = await _employeeSubTaskActivityService.DeleteByEmployeeSubTaskId(SubTaskId);
            }
            else
            {
                TaskId = null;
                taskDetail = null;
                long employeeTaskId = taskRequestModel.Id.Value;
                var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(employeeTaskId);

                if (subTasks != null && subTasks.Count() > 0)
                {
                    foreach (var subTask in subTasks)
                    {
                        var subTaskId = subTask.Id;

                        var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);

                        if (childTasks != null && childTasks.Count() > 0)
                        {
                            foreach (var item in childTasks)
                            {
                                var childTaskId = item.Id;

                                var childDocuments = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                                // Remove child task documents from folder

                                foreach (var childTaskDoc in childDocuments)
                                {

                                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                                    var filePath = dirPath + "\\" + childTaskDoc.Name;

                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(Path.Combine(filePath));
                                    }
                                }

                                var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                                var childTimeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(childTaskId);

                                var childTaskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(childTaskId);

                                EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                                employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
                                employeeChildTaskActivityObj.UserId = UserId;
                                employeeChildTaskActivityObj.Activity = "Removed the task";
                                var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

                                var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

                                var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);


                            }
                        }

                        var subDocuments = await _employeeSubTaskAttachmentService.DeleteAttachmentByEmployeeSubTaskId(subTaskId);

                        // Remove sub task documents from folder

                        foreach (var subTaskDoc in subDocuments)
                        {

                            //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
                            var filePath = dirPath + "\\" + subTaskDoc.Name;

                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(Path.Combine(filePath));
                            }
                        }

                        var subComments = await _employeeSubTaskCommentService.DeleteCommentByEmployeeSubTaskId(subTaskId);

                        var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                        var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(subTaskId);

                        EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                        employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskId;
                        employeeSubTaskActivityObj.UserId = UserId;
                        employeeSubTaskActivityObj.Activity = "Removed the task";
                        var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                        var subTaskActivities = await _employeeSubTaskActivityService.DeleteByEmployeeSubTaskId(subTaskId);

                        var subTaskToDelete = await _employeeSubTaskService.Delete(subTaskId);

                    }
                }

                var documents = await _employeeTaskAttachmentService.DeleteAttachmentByTaskId(employeeTaskId);

                // Remove task documents from folder

                foreach (var taskDoc in documents)
                {

                    //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                    var filePath = dirPath + "\\" + taskDoc.Name;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }
                }

                var comments = await _employeeTaskCommentService.DeleteCommentByEmployeeTaskId(employeeTaskId);

                var timeRecords = await _employeeTaskTimeRecordService.DeleteTimeRecordByTaskId(employeeTaskId);

                var taskUsers = await _employeeTaskUserService.DeleteByEmployeeTaskId(employeeTaskId);

                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                employeeTaskActivityObj.EmployeeTaskId = employeeTaskId;
                employeeTaskActivityObj.UserId = UserId;
                employeeTaskActivityObj.Activity = "Removed the task";
                var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

                var taskActivities = await _employeeTaskActivityService.DeleteByEmployeeTaskId(employeeTaskId);

                var taskToDelete = await _employeeTaskService.Delete(employeeTaskId);
            }

            if (TaskId != null)
            {
                taskDetail = await Detail(TaskId.Value);
            }

            // For all subtask with completed status then main task automatic completed status
            // if (employeeTask.Id != null) {
            //     var taskId = employeeTask.Id.Value;
            //     var subTasks = _subTaskService.GetAllSubTaskByTask (taskId);
            //     if (employeeTask.UserId != null && subTasks.Count () > 0) {
            //         var userId = employeeTask.UserId.Value;
            //         var employeeTaskStatusList = _employeeTaskService.GetTaskByUser (userId);
            //         var finalStatus = employeeTaskStatusList.Where (t => t.IsFinalize == true).FirstOrDefault ();
            //         if (finalStatus != null) {
            //             var completedSubTaskCount = subTasks.Where (t => t.StatusId == finalStatus.Id).Count ();
            //             if (subTasks.Count () == completedSubTaskCount) {
            //                 taskObj.StatusId = finalStatus.Id;
            //             }
            //         }
            //     }
            // }

            return new OperationResult<CustomTaskVM>(true, System.Net.HttpStatusCode.OK, "Task saved successfully.", taskDetail);
        }

        private async Task<CustomTaskVM> Detail(long EmployeeTaskId)
        {
            CustomTaskVM taskVMObj = new CustomTaskVM();
            var AllUsers = _userService.GetAll();
            var taskObj = _employeeTaskService.GetTaskById(EmployeeTaskId);
            taskVMObj = _mapper.Map<CustomTaskVM>(taskObj);
            taskVMObj.MainTaskId = EmployeeTaskId;

            //var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(EmployeeTaskId);
            //employeeTaskVMObj.Duration = taskTotalDuration;

            // For task assign users
            var assignUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(EmployeeTaskId);
            if (assignUsers != null && assignUsers.Count > 0)
            {
                var assignTaskUserVMList = _mapper.Map<List<CustomTaskUserVM>>(assignUsers);
                if (taskObj.StatusId != null)
                {
                    taskVMObj.StatusName = taskObj.Status?.Name;
                }
                if (taskVMObj.Assignee == null)
                {
                    taskVMObj.Assignee = new List<CustomTaskUserVM>();
                }
                if (assignTaskUserVMList != null && assignTaskUserVMList.Count() > 0)
                {
                    foreach (var assignUser in assignTaskUserVMList)
                    {
                        if (AllUsers != null)
                        {
                            var userObj = AllUsers.Where(t => t.Id == assignUser.UserId).FirstOrDefault();
                            assignUser.FirstName = userObj?.FirstName ?? "";
                            assignUser.LastName = userObj?.LastName ?? "";
                            assignUser.UserName = userObj?.UserName ?? "";
                            assignUser.Email = userObj?.Email ?? "";
                            if (userObj != null)
                            {
                                if (!string.IsNullOrEmpty(taskVMObj.AssigneeName))
                                {
                                    taskVMObj.AssigneeName = taskVMObj.AssigneeName + ",";
                                }
                                if (!string.IsNullOrEmpty(userObj.FirstName) && !string.IsNullOrEmpty(userObj.LastName))
                                {
                                    taskVMObj.AssigneeName = taskVMObj.AssigneeName + userObj.FirstName + " " + userObj.LastName;
                                }
                                else
                                {
                                    taskVMObj.AssigneeName = taskVMObj.AssigneeName + userObj.Email;
                                }
                            }
                        }
                    }
                }
                taskVMObj.Assignee = assignTaskUserVMList;
            }

            var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(EmployeeTaskId);
            taskVMObj.Tasks = _mapper.Map<List<CustomSubTaskVM>>(subTasks);
            foreach (var subTaskVMObj in taskVMObj.Tasks)
            {
                subTaskVMObj.MainTaskId = EmployeeTaskId;
                var subTask = subTasks.Where(t => t.Id == subTaskVMObj.Id).FirstOrDefault();
                if (subTask?.StatusId != null)
                {
                    subTaskVMObj.StatusName = subTask?.Status?.Name;
                }
                var subTaskAssignUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(subTaskVMObj.Id);
                subTaskVMObj.Assignee = _mapper.Map<List<EmployeeSubTaskUserDto>>(subTaskAssignUsers);
                foreach (var subTaskAssignUserObj in subTaskVMObj.Assignee)
                {
                    if (subTaskAssignUserObj.UserId != null)
                    {
                        var subTaskUserObj = AllUsers.Where(t => t.Id == subTaskAssignUserObj.UserId).FirstOrDefault();
                        if (subTaskUserObj != null)
                        {
                            if (!string.IsNullOrEmpty(subTaskVMObj.AssigneeName))
                            {
                                subTaskVMObj.AssigneeName = subTaskVMObj.AssigneeName + ",";
                            }
                            if (!string.IsNullOrEmpty(subTaskUserObj.FirstName) && !string.IsNullOrEmpty(subTaskUserObj.LastName))
                            {
                                subTaskVMObj.AssigneeName = subTaskVMObj.AssigneeName + subTaskUserObj.FirstName + " " + subTaskUserObj.LastName;
                            }
                            else
                            {
                                subTaskVMObj.AssigneeName = subTaskVMObj.AssigneeName + subTaskUserObj.Email;
                            }
                        }
                    }
                }
                var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskVMObj.Id);
                subTaskVMObj.Tasks = _mapper.Map<List<CustomChildTaskVM>>(childTasks);
                foreach (var childTaskVMObj in subTaskVMObj.Tasks)
                {
                    childTaskVMObj.MainTaskId = EmployeeTaskId;
                    var childTask = childTasks.Where(t => t.Id == childTaskVMObj.Id).FirstOrDefault();
                    if (childTask?.StatusId != null)
                    {
                        childTaskVMObj.StatusName = childTask?.Status?.Name;
                    }
                    var childTaskAssignUsers = _employeeChildTaskUserService.GetAssignUsersByChildTask(childTaskVMObj.Id);
                    childTaskVMObj.Assignee = _mapper.Map<List<EmployeeChildTaskUserDto>>(childTaskAssignUsers);
                    foreach (var childTaskAssignUserObj in childTaskVMObj.Assignee)
                    {
                        if (childTaskAssignUserObj.UserId != null)
                        {
                            var childTaskUserObj = AllUsers.Where(t => t.Id == childTaskAssignUserObj.UserId).FirstOrDefault();
                            if (childTaskUserObj != null)
                            {
                                if (!string.IsNullOrEmpty(childTaskVMObj.AssigneeName))
                                {
                                    childTaskVMObj.AssigneeName = childTaskVMObj.AssigneeName + ",";
                                }
                                if (!string.IsNullOrEmpty(childTaskUserObj.FirstName) && !string.IsNullOrEmpty(childTaskUserObj.LastName))
                                {
                                    childTaskVMObj.AssigneeName = childTaskVMObj.AssigneeName + childTaskUserObj.FirstName + " " + childTaskUserObj.LastName;
                                }
                                else
                                {
                                    subTaskVMObj.AssigneeName = subTaskVMObj.AssigneeName + childTaskUserObj.Email;
                                }
                            }
                        }
                    }
                }

            }

            return taskVMObj;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateTaskTimeTrackeResponse>> TimeRecord([FromBody] AddUpdateEmployeeTaskTimeTrackRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            EmployeeTaskTimeRecord employeeTaskTimeRecordObj = new EmployeeTaskTimeRecord();
            AddUpdateTaskTimeTrackeResponse employeeTaskTimeRecordResponse = new AddUpdateTaskTimeTrackeResponse();
            if (Model.Duration != null && Model.EmployeeTaskId != null)
            {
                var employeeTaskTimeRecordDto = _mapper.Map<EmployeeTaskTimeRecordDto>(Model);
                employeeTaskTimeRecordDto.UserId = UserId;
                employeeTaskTimeRecordObj = await _employeeTaskTimeRecordService.CheckInsertOrUpdate(employeeTaskTimeRecordDto);
                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                employeeTaskActivityObj.EmployeeTaskId = employeeTaskTimeRecordObj.EmployeeTaskId;
                employeeTaskActivityObj.UserId = UserId;
                employeeTaskActivityObj.Activity = "Created time record";
                employeeTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Time_record_added.ToString().Replace("_", " ");
                var AddUpdate1 = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

                var taskTimeRecord = _employeeTaskTimeRecordService.GetById(employeeTaskTimeRecordObj.Id);

                employeeTaskTimeRecordResponse = _mapper.Map<AddUpdateTaskTimeTrackeResponse>(taskTimeRecord);
                if (taskTimeRecord.Duration != null && taskTimeRecord != null && taskTimeRecord.ServiceArticle != null && taskTimeRecord.ServiceArticle.UnitPrice != null)
                {
                    var totalPrice = taskTimeRecord.Duration.Value * taskTimeRecord.ServiceArticle.UnitPrice.Value;
                    if (taskTimeRecord.ServiceArticle.Currency != null)
                    {
                        employeeTaskTimeRecordResponse.TotalAmount = totalPrice + taskTimeRecord.ServiceArticle.Currency.Symbol;
                    }
                }
                return new OperationResult<AddUpdateTaskTimeTrackeResponse>(true, System.Net.HttpStatusCode.OK, "", employeeTaskTimeRecordResponse);
            }
            else
            {
                var message = "EmployeeTaskId can not be null";
                if (Model.Duration == null)
                {
                    message = "Duration can not be null";
                }
                return new OperationResult<AddUpdateTaskTimeTrackeResponse>(false, System.Net.HttpStatusCode.OK, message, employeeTaskTimeRecordResponse);
            }

        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{TaskId}")]
        public async Task<OperationResult<List<AddUpdateTaskTimeTrackeResponse>>> TimeRecordList(long TaskId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<AddUpdateTaskTimeTrackeResponse> employeeTaskTimeTrackResponseList = new List<AddUpdateTaskTimeTrackeResponse>();
            var employeeTaskTimeRecordList = _employeeTaskTimeRecordService.getListByTaskId(TaskId);

            employeeTaskTimeTrackResponseList = employeeTaskTimeRecordList.Select(t => new AddUpdateTaskTimeTrackeResponse
            {
                Id = t.Id,
                Duration = t.Duration,
                ServiceArticleName = t?.ServiceArticle?.Name,
                EmployeeTaskId = t?.EmployeeTaskId,
                Price = t?.ServiceArticle.UnitPrice,
                TotalAmount = t?.IsBillable == true ? ((t?.ServiceArticle?.UnitPrice) * t?.Duration) + t?.ServiceArticle?.Currency?.Name : null
            }).ToList();

            return new OperationResult<List<AddUpdateTaskTimeTrackeResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskTimeTrackResponseList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<AddUpdateTaskTimeTrackeResponse>>> Invoice([FromBody] TaskInvoiceRequest invoiceRequest)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<AddUpdateTaskTimeTrackeResponse> employeeTaskTimeTrackResponseList = new List<AddUpdateTaskTimeTrackeResponse>();
            // TimeSpan ts = new TimeSpan(0, 0, 0);
            // invoiceRequest.StartDate = invoiceRequest.StartDate + ts;
            // invoiceRequest.EndDate = invoiceRequest.EndDate + ts;
            // invoiceRequest.EndDate.setHours(0, 0, 0);
            var employeeTaskTimeRecordList = _employeeTaskTimeRecordService.InVoice(invoiceRequest.StartDate.Value, invoiceRequest.EndDate.Value, TenantId).ToList();

            employeeTaskTimeTrackResponseList = employeeTaskTimeRecordList.Select(t => new AddUpdateTaskTimeTrackeResponse
            {
                Id = t.Id,
                Duration = t.Duration,
                ServiceArticleName = t?.ServiceArticle?.Name,
                EmployeeTaskId = t?.EmployeeTaskId,
                Price = t?.ServiceArticle.UnitPrice,
                TotalAmount = t?.IsBillable == true ? ((t?.ServiceArticle?.UnitPrice) * t?.Duration) + t?.ServiceArticle?.Currency?.Symbol : null
            }).ToList();

            return new OperationResult<List<AddUpdateTaskTimeTrackeResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskTimeTrackResponseList);
        }

        private static string? FirstCharToLowerCase(string? str)
        {
            if (!string.IsNullOrEmpty(str) && char.IsUpper(str[0]))
                return str.Length == 1 ? char.ToLower(str[0]).ToString() : char.ToLower(str[0]) + str[1..];

            return str;
        }



    }

}