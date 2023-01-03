using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using matcrm.data.Models.Response;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.Request;
using matcrm.data.Models.MollieModel.Subscription;
using matcrm.data.Models.Dto;
using matcrm.service.Services.Mollie.Customer;
using matcrm.service.Services.Mollie.Payment;
using matcrm.service.Services.Mollie.Subscription;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.MollieModel;
using AutoMapper;
using matcrm.service.BusinessLogic;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    public class InvoiceController : Controller
    {
        private readonly ILogger<InvoiceController> _logger;
        private readonly IClientInvoiceService _clientInvoiceService;
        private readonly IClientService _clientService;
        private readonly IClientEmailService _clientEmailService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IContractService _contractService;
        private readonly IContractArticleService _contractArticleService;
        private readonly IServiceArticleService _serviceArticleService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IInvoiceMollieCustomerService _invoiceMollieCustomerService;
        private readonly IInvoiceMollieSubscriptionService _invoiceMollieSubscriptionService;
        private readonly ICustomerStorageClient _customerStorageClient;
        private readonly IPaymentOverviewClient _paymentOverviewClient;
        private readonly IPaymentStorageClient _paymentStorageClient;
        private readonly ISubscriptionOverviewClient _subscriptionOverviewClient;
        private readonly ISubscriptionStorageClient _subscriptionStorageClient;
        private readonly IMateTaskTimeRecordService _mateTaskTimeRecordService;
        private readonly IMateProjectTimeRecordService _mateProjectTimeRecordService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly ITaxRateService _taxRateService;
        private readonly IContractInvoiceService _contractInvoiceService;
        private readonly IMateSubTaskTimeRecordService _mateSubTaskTimeRecordService;
        private readonly IMateChildTaskTimeRecordService _mateChildTaskTimeRecordService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private SendEmail sendEmail;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public InvoiceController(
            IClientInvoiceService clientInvoiceService,
            IClientService clientService,
            IClientEmailService clientEmailService,
            IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
            IContractService contractService,
            IContractArticleService contractArticleService,
            IServiceArticleService serviceArticleService,
            IEmployeeTaskService employeeTaskService,
            IInvoiceMollieCustomerService invoiceMollieCustomerService,
            IInvoiceMollieSubscriptionService invoiceMollieSubscriptionService,
             ICustomerOverviewClient customerOverviewClient,
            ICustomerStorageClient customerStorageClient,
            IPaymentOverviewClient paymentOverviewClient,
            IPaymentStorageClient paymentStorageClient,
            ISubscriptionOverviewClient subscriptionOverviewClient,
            ISubscriptionStorageClient subscriptionStorageClient,
            IMateTaskTimeRecordService mateTaskTimeRecordService,
            IMateTimeRecordService mateTimeRecordService,
            IEmployeeProjectService employeeProjectService,
            ITaxRateService taxRateService,
            IMateProjectTimeRecordService mateProjectTimeRecordService,
            IContractInvoiceService contractInvoiceService,
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeChildTaskService employeeChildTaskService,
            IMateSubTaskTimeRecordService mateSubTaskTimeRecordService,
            IMateChildTaskTimeRecordService mateChildTaskTimeRecordService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            IMapper mapper)
        {
            _clientInvoiceService = clientInvoiceService;
            _clientService = clientService;
            _clientEmailService = clientEmailService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _contractService = contractService;
            _contractArticleService = contractArticleService;
            _serviceArticleService = serviceArticleService;
            _employeeTaskService = employeeTaskService;
            _invoiceMollieCustomerService = invoiceMollieCustomerService;
            _invoiceMollieSubscriptionService = invoiceMollieSubscriptionService;
            _customerStorageClient = customerStorageClient;
            _paymentOverviewClient = paymentOverviewClient;
            _paymentStorageClient = paymentStorageClient;
            _subscriptionOverviewClient = subscriptionOverviewClient;
            _subscriptionStorageClient = subscriptionStorageClient;
            _mateTaskTimeRecordService = mateTaskTimeRecordService;
            _mateTimeRecordService = mateTimeRecordService;
            _employeeProjectService = employeeProjectService;
            _taxRateService = taxRateService;
            _mateProjectTimeRecordService = mateProjectTimeRecordService;
            _contractInvoiceService = contractInvoiceService;
            _employeeSubTaskService = employeeSubTaskService;
            _mateSubTaskTimeRecordService = mateSubTaskTimeRecordService;
            _mateChildTaskTimeRecordService = mateChildTaskTimeRecordService;
            _employeeChildTaskService = employeeChildTaskService;
            _mapper = mapper;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProvider = emailProvider;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        // [HttpPost]
        // public async Task<OperationResult<List<AddUpdateTaskTimeTrackeResponse>>> Create([FromBody] AddInvoiceRequest requestModel)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

        //     var EmployeeTaskIdList = _employeeTaskService.GetTaskIdByClient(requestModel.ClientId);
        //     var TaskTimeRecordList = _employeeTaskTimeRecordService.GetListByTaskIdList(EmployeeTaskIdList);

        //     var employeeTaskTimeTrackResponseList = TaskTimeRecordList.Where(t => t.CreatedOn != null && (t.CreatedOn.Value.Date >= requestModel.StartDate.Date && t.CreatedOn.Value <= requestModel.EndDate.Date)).Select(t => new AddUpdateTaskTimeTrackeResponse
        //     {
        //         Id = t.Id,
        //         Duration = t.Duration,
        //         ServiceArticleName = t?.ServiceArticle?.Name,
        //         EmployeeTaskId = t?.EmployeeTaskId,
        //         Price = t?.ServiceArticle?.UnitPrice,
        //         TotalAmount = t?.IsBillable == true ? ((t?.ServiceArticle?.UnitPrice) * t?.Duration) + t?.ServiceArticle?.Currency?.Symbol : null
        //     }).ToList();

        //     long? TotalAmount = 0;

        //     TotalAmount = employeeTaskTimeTrackResponseList.Where(t => t.Price != null && t.Duration != null).Select(t => (t.Price * t.Duration)).Sum();

        //     ClientInvoice clientInvoice = new ClientInvoice();
        //     clientInvoice.TotalAmount = TotalAmount;
        //     clientInvoice.ClientId = requestModel.ClientId;
        //     clientInvoice.InvoiceDate = DateTime.UtcNow;
        //     clientInvoice.StartDate = requestModel.StartDate;
        //     clientInvoice.EndDate = requestModel.EndDate;
        //     clientInvoice.InvoiceNo = DateTime.UtcNow.ToString("ddMMyyhhmmss");
        //     clientInvoice.CreatedBy = UserId;
        //     var addedClientInvoice = await _clientInvoiceService.CheckInsertOrUpdate(clientInvoice);

        //     return new OperationResult<List<AddUpdateTaskTimeTrackeResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeTaskTimeTrackResponseList);
        // }

        // // Start logic for payment
        // [HttpPost]
        // public async Task<OperationResult<PaymentResponse>> Payment(InvoicePaymentRequest model)
        // {
        //     SubscriptionResponse subscriptionResponseObj = new SubscriptionResponse();
        //     PaymentResponse paymentResponseObj = new PaymentResponse();
        //     MollieCustomer? mollieCustomerObj = null;
        //     UserSubscription? userSubscriptionObj = null;

        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);


        //     var clientObj = _clientService.GetById(model.ClientId);
        //     var clientContractList = _contractService.GetByClient(model.ClientId);
        //     // foreach (var itemObj in clientContractList)
        //     // {
        //     //     var contractArticles = _contractSubscriptionService.GetByContract(itemObj.Id);
        //     //     var contractServiceArticleList = contractArticles.Select(t => t.ServiceArticleId).ToList();


        //     // }


        //     var PrimaryClientEmailObj = _clientEmailService.GetByClientIdWithPrimary(model.ClientId);

        //     if (PrimaryClientEmailObj != null)
        //     {

        //         // InvoiceMollieCustomer invoiceMollieCustomer = new InvoiceMollieCustomer();
        //         // invoiceMollieCustomer.ClientId = model.ClientId;
        //         // invoiceMollieCustomer.CustomerId = PrimaryClientEmailObj.Email;

        //         var invoiceMollieCustomerObj = _invoiceMollieCustomerService.GetByClient(model.ClientId);

        //         CustomerResponse customerResponseObj = new CustomerResponse();
        //         CreateCustomerModel createCustomerModelObj = new CreateCustomerModel();
        //         // mollieCustomerObj = _mollieCustomerService.GetByUser(UserId);
        //         if (invoiceMollieCustomerObj != null)
        //         {
        //             createCustomerModelObj.CustomerId = invoiceMollieCustomerObj.CustomerId;
        //             createCustomerModelObj.Email = invoiceMollieCustomerObj.CustomerId;
        //             createCustomerModelObj.Name = invoiceMollieCustomerObj?.Client?.FirstName;
        //         }
        //         else
        //         {
        //             createCustomerModelObj.Email = PrimaryClientEmailObj.Email;
        //             createCustomerModelObj.CustomerId = PrimaryClientEmailObj.Email;
        //             createCustomerModelObj.Name = PrimaryClientEmailObj?.Client?.FirstName;
        //             customerResponseObj = await this._customerStorageClient.Create(createCustomerModelObj);
        //             InvoiceMollieCustomer invoiceMollieCustomerObj1 = new InvoiceMollieCustomer();
        //             invoiceMollieCustomerObj1.ClientId = model.ClientId;
        //             invoiceMollieCustomerObj1.CustomerId = customerResponseObj.Id;
        //             invoiceMollieCustomerObj = await _invoiceMollieCustomerService.CheckInsertOrUpdate(invoiceMollieCustomerObj1);

        //         }

        //         if (!string.IsNullOrEmpty(model.InvoiceNo))
        //         {
        //             var ClientInvoiceObj = _clientInvoiceService.GetByInvoiceNo(model.InvoiceNo);
        //             if (ClientInvoiceObj != null)
        //             {
        //                 var InvoiceMollieSubscriptionObj = _invoiceMollieSubscriptionService.GetByInvoiceId(ClientInvoiceObj.Id);
        //                 if (InvoiceMollieSubscriptionObj == null)
        //                 {
        //                     decimal DEBITAMT = Convert.ToDecimal(string.Format("{0:F2}", ClientInvoiceObj.TotalAmount.Value));
        //                     CreatePaymentModel createPaymentModelObj = new CreatePaymentModel()
        //                     {
        //                         Amount = DEBITAMT,
        //                         Currency = PaymentCurrency.EUR,
        //                         CustomerId = invoiceMollieCustomerObj.CustomerId,
        //                         SequenceType = "first",
        //                         Description = "first payment from api"
        //                     };
        //                     paymentResponseObj = await this._paymentStorageClient.Create(createPaymentModelObj);
        //                     InvoiceMollieSubscription mollieSubscriptionDto = new InvoiceMollieSubscription();
        //                     // mollieSubscriptionDto.SubscriptionId = subscriptionResponseObj.Id;
        //                     mollieSubscriptionDto.PaymentId = paymentResponseObj.Id;
        //                     mollieSubscriptionDto.ClientId = model.ClientId;
        //                     mollieSubscriptionDto.ClientInvoiceId = ClientInvoiceObj.Id;
        //                     var mollieSubscriptionObj = await _invoiceMollieSubscriptionService.CheckInsertOrUpdate(mollieSubscriptionDto);
        //                     // CreateSubscriptionModel createSubscriptionModelObj = new CreateSubscriptionModel();
        //                     // createSubscriptionModelObj.CustomerId = invoiceMollieCustomerObj.CustomerId;
        //                     // createSubscriptionModelObj.Amount.value = DEBITAMT;
        //                     // createSubscriptionModelObj.Amount.Currency = PaymentCurrency.EUR;
        //                     // // subscriptionModel.IntervalAmount = 1;
        //                     // InvoiceType invoiceTypeObj = new InvoiceType();
        //                     // invoiceTypeObj.Name = "Monthly";
        //                     // // subscriptionModel.Times = 12;
        //                     // if (invoiceTypeObj.Name == "Monthly")
        //                     // {
        //                     //     createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Months;
        //                     //     createSubscriptionModelObj.IntervalAmount = 1;
        //                     //     // subscriptionModel.Times = 12;
        //                     // }
        //                     // else if (invoiceTypeObj.Name == "YearlyWithMonthly")
        //                     // {
        //                     //     createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Months;
        //                     //     createSubscriptionModelObj.IntervalAmount = 12;
        //                     //     // subscriptionModel.Times = 1;
        //                     // }
        //                     // else if (invoiceTypeObj.Name == "Weekly")
        //                     // {
        //                     //     createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Months;
        //                     //     createSubscriptionModelObj.IntervalAmount = 12;
        //                     //     // subscriptionModel.Times = 1;
        //                     // }

        //                     // Random generator = new Random();
        //                     // String r = generator.Next(0, 1000000).ToString("D6");
        //                     // // subscriptionModel.IntervalPeriod = IntervalPeriod.Days;
        //                     // createSubscriptionModelObj.Description = "Test 1 subscription from api " + r;
        //                     // var bbb = DateTime.Now.ToString("YYYY-MM-DD");
        //                     // createSubscriptionModelObj.startDate = DateTime.Now.ToString("yyyy-MM-dd");

        //                     // subscriptionResponseObj = await this._subscriptionStorageClient.Create(createSubscriptionModelObj);
        //                 }
        //             }

        //         }

        //         return new OperationResult<PaymentResponse>(true, System.Net.HttpStatusCode.OK, "", paymentResponseObj);
        //     }
        //     else
        //     {
        //         return new OperationResult<PaymentResponse>(true, System.Net.HttpStatusCode.OK, "Please add client email", paymentResponseObj);
        //     }

        // }


        // // End logic for payment



        // // List<Client> ClientList = _clientService.GetByTenant(TenantId);
        // // List<Contract> ContractList = _contractService.GetAll();

        // // foreach (var ClientObj in ClientList)
        // // {
        // //     long interval = 30;
        // //     var EmployeeTaskIdList = _employeeTaskService.GetTaskIdByClient(ClientObj.Id);
        // //     var TaskTimeRecordList = _employeeTaskTimeRecordService.GetListByTaskIdList(EmployeeTaskIdList);

        // //     var ClientContracts = ContractList.Where(t => t.ClientId == ClientObj.Id).ToList();

        // //     if (ClientContracts != null && ClientContracts.Count() > 0)
        // //     {
        // //         if (ClientContracts.Count() == 1)
        // //         {
        // //             var ContractObj = ClientContracts.FirstOrDefault();
        // //             var InvoiceTypeObj = ContractObj?.InvoiceType;
        // //             interval = (InvoiceTypeObj != null && InvoiceTypeObj.Interval != null) ? InvoiceTypeObj.Interval.Value : Convert.ToInt64(30);

        // //             var ContractCreatedDate = ContractObj.CreatedOn.Value.AddDays(interval);
        // //             DateTime startDate = DateTime.Today.AddDays(-interval);
        // //             startDate = startDate.AddHours(0).AddMinutes(0).AddSeconds(0);
        // //             var ClientInvoiceList = _clientInvoiceService.GetAllByClient(ClientObj.Id).Where(t => t.CreatedOn >= startDate && t.CreatedOn <= DateTime.Today.Date).ToList();
        // //             if (ClientInvoiceList == null)
        // //             {
        // //                 if (ContractCreatedDate.Date == DateTime.Today.Date)
        // //                 {
        // //                     var employeeTaskTimeTrackResponseList = TaskTimeRecordList.Select(t => new AddUpdateTaskTimeTrackeResponse
        // //                     {
        // //                         Id = t.Id,
        // //                         Duration = t.Duration,
        // //                         ServiceArticleName = t?.ServiceArticle?.Name,
        // //                         EmployeeTaskId = t?.EmployeeTaskId,
        // //                         Price = t?.ServiceArticle.UnitPrice,
        // //                         TotalAmount = t?.IsBillable == true ? ((t?.ServiceArticle?.UnitPrice) * t?.Duration) + t?.ServiceArticle?.Currency?.Name : null
        // //                     }).ToList();
        // //                 }
        // //             }
        // //         }
        // //     }
        // // }

        // [HttpGet]
        // public async Task<List<ClientVM>> GenerateClientBaseInterval()
        // {
        //     List<ClientVM> clientVMs = new List<ClientVM>();
        //     List<ClientInvoice> clientInvoices = new List<ClientInvoice>();
        //     List<Client> ClientList = _clientService.GetAll();

        //     List<MateTimeRecord> timeRecords = _mateTimeRecordService.GetAll();
        //     List<ClientContractInvoiceResponse> clientContractInvoiceResponses = new List<ClientContractInvoiceResponse>();
        //     List<ContractInvoice> contractInvoiceList = _contractInvoiceService.GetAll();
        //     List<long> clientInvoiceIdListForContract = contractInvoiceList.Where(t => t.ClientInvoiceId != null).Select(t => t.ClientInvoiceId.Value).ToList();

        //     foreach (var clientObj in ClientList)
        //     {
        //         ClientVM clientVM = _mapper.Map<ClientVM>(clientObj);
        //         var clientTasks = _employeeTaskService.GetTaskByClient(clientObj.Id);
        //         var clientProjects = _employeeProjectService.GetAllByClientId(clientObj.Id);
        //         List<long> ProjectIds = clientProjects.Select(t => t.Id).ToList();
        //         // var projectTasks = _employeeTaskService.GetAllTaskByProjectIdList(ProjectIds);
        //         // clientTasks.AddRange(projectTasks);
        //         List<long> TaskIds = clientTasks.Select(t => t.Id).ToList();
        //         var subTasks = _employeeSubTaskService.GetAllActiveByTaskIds(TaskIds);
        //         List<long> SubTaskIds = subTasks.Select(t => t.Id).ToList();
        //         var childTasks = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIds);

        //         if (clientObj.IsContractBaseInvoice == false)
        //         {
        //             InvoiceInterval clientInvoiceInternal = clientObj.InvoiceInterval;
        //             ClientContractInvoiceResponse clientResponseObj = new ClientContractInvoiceResponse();
        //             clientResponseObj.Id = clientObj.Id;
        //             clientResponseObj.Name = clientObj.Name;
        //             var clientInvoiceList = _clientInvoiceService.GetAllByClient(clientObj.Id);

        //             clientInvoiceList = clientInvoiceList.Where(t => t.DeletedOn == null && !clientInvoiceIdListForContract.Any(b => t.Id == b)).OrderBy(t => t.StartDate).ToList();
        //             DateTime? StartDate = null;
        //             DateTime? EndDate = null;

        //             // start logic for get startdate from last existing invoice enddate
        //             if (clientInvoiceList != null && clientInvoiceList.Count() > 0)
        //             {
        //                 var lastClientInvoice = clientInvoiceList.LastOrDefault();
        //                 if (lastClientInvoice != null && lastClientInvoice.EndDate != null)
        //                 {
        //                     StartDate = lastClientInvoice?.EndDate.Value.Date.AddDays(1).AddHours(0).AddMinutes(0).AddSeconds(0);
        //                     EndDate = StartDate.Value.Date.AddDays(clientInvoiceInternal.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                 }
        //             }
        //             else
        //             {
        //                 // start logic for get startdate from first time start timerecord date for generate first invoice startDate
        //                 TaskIds = clientTasks.Select(t => t.Id).ToList();
        //                 var taskStartDate = _mateTaskTimeRecordService.GetTaskTimeRecordStartDate(TaskIds);
        //                 var projectStartDate = _mateProjectTimeRecordService.GetProjectTimeRecordStartDate(ProjectIds);
        //                 if (taskStartDate != null && projectStartDate == null)
        //                 {
        //                     StartDate = taskStartDate;
        //                 }
        //                 else if (taskStartDate == null && projectStartDate != null)
        //                 {
        //                     StartDate = projectStartDate;
        //                 }
        //                 else if (projectStartDate != null && taskStartDate != null)
        //                 {
        //                     StartDate = taskStartDate > projectStartDate ? projectStartDate : StartDate;
        //                 }
        //                 if (StartDate != null)
        //                 {
        //                     StartDate = StartDate.Value.Date.AddHours(0).AddMinutes(0).AddSeconds(0);

        //                     EndDate = StartDate.Value.Date.AddDays(clientInvoiceInternal.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                 }
        //             }

        //             // if (StartDate != null && EndDate != null && EndDate.Value.Date == DateTime.Today.Date)
        //             if (StartDate != null && EndDate != null)
        //             {
        //                 var clientContracts = _contractService.GetByClient(clientObj.Id);
        //                 var contractIds = clientContracts.Select(t => t.Id).ToList();
        //                 var contractArticles = _contractArticleService.GetByContractIds(contractIds);

        //                 // var now = DateTime.Now;
        //                 // var CurrentMonthFirstDate = new DateTime(now.Year, now.Month, 1);
        //                 // var CurrentMonthLastDate = CurrentMonthFirstDate.AddMonths(1).AddDays(-1);
        //                 // foreach (var clientContractObj in clientContracts)
        //                 // {
        //                 // var contractObj = _mapper.Map<ClientContract>(clientContractObj);
        //                 if (clientObj.InvoiceInterval != null)
        //                 {
        //                     clientVM.InvoiceIntervalName = clientObj.InvoiceInterval?.Name;
        //                     clientVM.Interval = clientObj.InvoiceInterval?.Interval;
        //                 }
        //                 // var contractServiceArticleList = _contractArticleService.GetByContract(clientContractObj.Id);
        //                 // var contrctServiceArticleIdList = contractServiceArticleList.Select(t => t.ServiceArticleId).ToList();

        //                 // long ContractTotalAmount = 0;
        //                 // var calculateDiscount = Convert.ToInt32(discount / 100);

        //                 // var totalDiscount = calculateDiscount;


        //                 #region Start logic for Global task
        //                 // -------------------------------start logic for global task--------------------------------------------//
        //                 foreach (var taskObj in clientTasks)
        //                 {
        //                     var taskTimeRecords = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj.Id, StartDate.Value, EndDate.Value)
        //                     .Where(t => t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true).ToList();

        //                     ClientTask clientTask = new ClientTask();
        //                     clientTask = _mapper.Map<ClientTask>(taskObj);
        //                     clientTask.PayableAmount = 0;
        //                     long? TaskTotalAmount = 0;

        //                     foreach (var taskTimeRecordObj in taskTimeRecords)
        //                     {
        //                         var t = taskTimeRecordObj;
        //                         if (taskTimeRecordObj.MateTimeRecord != null)
        //                         {
        //                             CustomTimeRecord customTaskTimeRecord = new CustomTimeRecord();
        //                             customTaskTimeRecord.Id = taskTimeRecordObj.MateTimeRecord.Id;
        //                             customTaskTimeRecord.UserId = t.MateTimeRecord?.UserId;
        //                             customTaskTimeRecord.Duration = t.MateTimeRecord?.Duration;
        //                             customTaskTimeRecord.Comment = t.MateTimeRecord?.Comment;
        //                             customTaskTimeRecord.IsBillable = t.MateTimeRecord?.IsBillable;
        //                             customTaskTimeRecord.CreatedOn = t.MateTimeRecord?.CreatedOn;
        //                             customTaskTimeRecord.CurrencyName = t.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                             customTaskTimeRecord.ServiceArticleId = t.MateTimeRecord?.ServiceArticleId;
        //                             if (taskTimeRecordObj.MateTimeRecord.ServiceArticle != null)
        //                             {
        //                                 var currentContractArticle = contractArticles.Where(t => t.ServiceArticleId == customTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                 if (currentContractArticle != null)
        //                                 {
        //                                     var contractObj = currentContractArticle.Contract;
        //                                     if (currentContractArticle != null && currentContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                     {
        //                                         customTaskTimeRecord.UnitPrice = contractObj?.DefaultUnitPrice;
        //                                         customTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (t.MateTimeRecord?.Duration);
        //                                     }
        //                                     else
        //                                     {
        //                                         customTaskTimeRecord.UnitPrice = t.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                                         customTaskTimeRecord.TotalAmount = (t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration);
        //                                     }
        //                                 }
        //                             }

        //                             // customTaskTimeRecord.ServiceArticle = t.MateTimeRecord?.ServiceArticle;

        //                             // customTaskTimeRecord.TotalDiscount = (((t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                             if (t.MateTimeRecord != null && t.MateTimeRecord != null && t.MateTimeRecord?.ServiceArticle != null && t.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                             {
        //                                 var taxId = t.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                 if (taxId != null)
        //                                 {
        //                                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                     customTaskTimeRecord.TotalTaxAmount = (((t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 customTaskTimeRecord.TotalTaxAmount = 0;
        //                             }
        //                             customTaskTimeRecord.PayableAmount = customTaskTimeRecord.TotalAmount + customTaskTimeRecord.TotalTaxAmount;
        //                             TaskTotalAmount = TaskTotalAmount + customTaskTimeRecord.PayableAmount;
        //                             // ContractTotalAmount = ContractTotalAmount + customTaskTimeRecord.PayableAmount.Value;
        //                             clientTask.TaskTrackedTime.Add(customTaskTimeRecord);
        //                         }

        //                         // contractObj.ContractSubscriptions = contrctServiceArticleList;
        //                     }

        //                     /////////////////////////////--------------SubTask Start---------------///////////////////////////////
        //                     var subTaskList = _employeeSubTaskService.GetAllSubTaskByTask(taskObj.Id);
        //                     foreach (var employeeSubTaskObj in subTaskList)
        //                     {
        //                         var subTaskTimeRecords = _mateSubTaskTimeRecordService.GetMateSubTaskTimeRecordBySubTaskId(employeeSubTaskObj.Id, StartDate.Value, EndDate.Value)
        //                         .Where(t => t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true).ToList();

        //                         ClientSubTask clientSubTask = new ClientSubTask();
        //                         clientSubTask = _mapper.Map<ClientSubTask>(employeeSubTaskObj);
        //                         clientSubTask.PayableAmount = 0;

        //                         foreach (var subTaskTimeRecordObj in subTaskTimeRecords)
        //                         {
        //                             if (subTaskTimeRecordObj.MateTimeRecord != null)
        //                             {
        //                                 var s = subTaskTimeRecordObj;
        //                                 CustomTimeRecord customSubTaskTimeRecord = new CustomTimeRecord();
        //                                 customSubTaskTimeRecord.Id = s.MateTimeRecord.Id;
        //                                 customSubTaskTimeRecord.UserId = s.MateTimeRecord?.UserId;
        //                                 customSubTaskTimeRecord.Duration = s.MateTimeRecord?.Duration;
        //                                 customSubTaskTimeRecord.Comment = s.MateTimeRecord?.Comment;
        //                                 customSubTaskTimeRecord.IsBillable = s.MateTimeRecord?.IsBillable;
        //                                 customSubTaskTimeRecord.CreatedOn = s.MateTimeRecord?.CreatedOn;
        //                                 customSubTaskTimeRecord.CurrencyName = s.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                 customSubTaskTimeRecord.ServiceArticleId = s.MateTimeRecord?.ServiceArticleId;

        //                                 if (subTaskTimeRecordObj.MateTimeRecord.ServiceArticle != null)
        //                                 {
        //                                     var currentSubTaskContractArticle = contractArticles.Where(t => t.ServiceArticleId == customSubTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                     if (currentSubTaskContractArticle != null)
        //                                     {
        //                                         var contractObj = currentSubTaskContractArticle.Contract;
        //                                         if (currentSubTaskContractArticle != null && currentSubTaskContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                         {
        //                                             customSubTaskTimeRecord.UnitPrice = contractObj?.DefaultUnitPrice;
        //                                             customSubTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (s.MateTimeRecord?.Duration);
        //                                         }
        //                                         else
        //                                         {
        //                                             customSubTaskTimeRecord.UnitPrice = s.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                                             customSubTaskTimeRecord.TotalAmount = (s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration);
        //                                         }
        //                                     }
        //                                 }
        //                                 // customTaskTimeRecord.ServiceArticle = s.MateTimeRecord?.ServiceArticle;

        //                                 // customTaskTimeRecord.TotalDiscount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                 if (s.MateTimeRecord != null && s.MateTimeRecord != null && s.MateTimeRecord?.ServiceArticle != null && s.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                 {
        //                                     var taxId = s.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                     if (taxId != null)
        //                                     {
        //                                         var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                         var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                         customSubTaskTimeRecord.TotalTaxAmount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                     }
        //                                 }
        //                                 else
        //                                 {
        //                                     customSubTaskTimeRecord.TotalTaxAmount = 0;
        //                                 }
        //                                 customSubTaskTimeRecord.PayableAmount = customSubTaskTimeRecord.TotalAmount + customSubTaskTimeRecord.TotalTaxAmount;
        //                                 clientSubTask.PayableAmount = clientSubTask.PayableAmount + customSubTaskTimeRecord.PayableAmount;
        //                                 TaskTotalAmount = TaskTotalAmount + customSubTaskTimeRecord.PayableAmount;
        //                                 // ContractTotalAmount = ContractTotalAmount + customSubTaskTimeRecord.PayableAmount.Value;
        //                                 clientSubTask.TaskTrackedTime.Add(customSubTaskTimeRecord);
        //                             }

        //                         }
        //                         //////////////////////////////--------------SubTask end------------------///////////////////////////////


        //                         ////////////////////////////-----------------ChildTask start----------------------//////////////////////////////////////

        //                         var childTaskList = _employeeChildTaskService.GetAllChildTaskBySubTask(employeeSubTaskObj.Id);
        //                         foreach (var employeeChildTaskObj in childTaskList)
        //                         {
        //                             var childTaskTimeRecords = _mateChildTaskTimeRecordService.GetMateChildTaskTimeRecordByChildTaskId(employeeChildTaskObj.Id, StartDate.Value, EndDate.Value)
        //                             .Where(t => t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true);

        //                             ClientChildTask clientChildTask = new ClientChildTask();
        //                             clientChildTask = _mapper.Map<ClientChildTask>(employeeChildTaskObj);
        //                             clientChildTask.PayableAmount = 0;
        //                             foreach (var childTaskTimeRecordObj in childTaskTimeRecords)
        //                             {
        //                                 if (childTaskTimeRecordObj.MateTimeRecord != null)
        //                                 {
        //                                     var c = childTaskTimeRecordObj;
        //                                     CustomTimeRecord customChildTaskTimeRecord = new CustomTimeRecord();
        //                                     customChildTaskTimeRecord.Id = c.MateTimeRecord.Id;
        //                                     customChildTaskTimeRecord.UserId = c.MateTimeRecord?.UserId;
        //                                     customChildTaskTimeRecord.Duration = c.MateTimeRecord?.Duration;
        //                                     customChildTaskTimeRecord.Comment = c.MateTimeRecord?.Comment;
        //                                     customChildTaskTimeRecord.IsBillable = c.MateTimeRecord?.IsBillable;
        //                                     customChildTaskTimeRecord.CreatedOn = c.MateTimeRecord?.CreatedOn;
        //                                     customChildTaskTimeRecord.CurrencyName = c.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                     customChildTaskTimeRecord.ServiceArticleId = c.MateTimeRecord?.ServiceArticleId;
        //                                     if (childTaskTimeRecordObj.MateTimeRecord.ServiceArticle != null)
        //                                     {
        //                                         var currentChildTaskContractArticle = contractArticles.Where(t => t.ServiceArticleId == customChildTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                         if (currentChildTaskContractArticle != null)
        //                                         {
        //                                             var contractObj = currentChildTaskContractArticle.Contract;
        //                                             if (currentChildTaskContractArticle != null && currentChildTaskContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                             {
        //                                                 customChildTaskTimeRecord.UnitPrice = contractObj?.DefaultUnitPrice;
        //                                                 customChildTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (c.MateTimeRecord?.Duration);
        //                                             }
        //                                             else
        //                                             {
        //                                                 customChildTaskTimeRecord.UnitPrice = c.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                                                 customChildTaskTimeRecord.TotalAmount = (c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration);
        //                                             }
        //                                         }
        //                                     }
        //                                     // customTaskTimeRecord.ServiceArticle = c.MateTimeRecord?.ServiceArticle;

        //                                     // customTaskTimeRecord.TotalDiscount = (((c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                     if (c.MateTimeRecord != null && c.MateTimeRecord != null && c.MateTimeRecord?.ServiceArticle != null && c.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                     {
        //                                         var taxId = c.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                         if (taxId != null)
        //                                         {
        //                                             var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                             var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                             customChildTaskTimeRecord.TotalTaxAmount = (((c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                         }
        //                                     }
        //                                     else
        //                                     {
        //                                         customChildTaskTimeRecord.TotalTaxAmount = 0;
        //                                     }


        //                                     customChildTaskTimeRecord.PayableAmount = customChildTaskTimeRecord.TotalAmount + customChildTaskTimeRecord.TotalTaxAmount;
        //                                     clientChildTask.PayableAmount = clientChildTask.PayableAmount + customChildTaskTimeRecord.PayableAmount;
        //                                     TaskTotalAmount = TaskTotalAmount + customChildTaskTimeRecord.PayableAmount;
        //                                     // ContractTotalAmount = ContractTotalAmount + customChildTaskTimeRecord.PayableAmount.Value;
        //                                     clientChildTask.TaskTrackedTime.Add(customChildTaskTimeRecord);
        //                                 }
        //                             }
        //                             clientSubTask.ChildTasks.Add(clientChildTask);
        //                         }
        //                         ////////////////////////////-----------------ChildTask end----------------------/////////////////////////////////////
        //                         clientTask.SubTasks.Add(clientSubTask);
        //                     }
        //                     clientTask.PayableAmount = TaskTotalAmount;
        //                     clientVM.Tasks.Add(clientTask);
        //                 }
        //                 #endregion

        //                 //--------------------------------------------------End logic for global task ----------------------------------------------------//

        //                 // -------------------------------------------------Start logic for Client projects-------------------------------------------------


        //                 foreach (var projectObj in clientProjects)
        //                 {
        //                     var projectTimeRecords = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj.Id, StartDate.Value, EndDate.Value)
        //                     .Where(t => t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true).ToList();

        //                     ClientProject clientProject = new ClientProject();
        //                     clientProject = _mapper.Map<ClientProject>(projectObj);
        //                     long? projectTotalAmount = 0;

        //                     foreach (var projectTimeRecordObj in projectTimeRecords)
        //                     {
        //                         if (projectTimeRecordObj.MateTimeRecord != null)
        //                         {
        //                             CustomTimeRecord customProjectTimeRecord = new CustomTimeRecord();
        //                             customProjectTimeRecord.Id = projectTimeRecordObj.MateTimeRecord.Id;
        //                             customProjectTimeRecord.UserId = projectTimeRecordObj.MateTimeRecord?.UserId;
        //                             customProjectTimeRecord.Duration = projectTimeRecordObj.MateTimeRecord?.Duration;
        //                             customProjectTimeRecord.Comment = projectTimeRecordObj.MateTimeRecord?.Comment;
        //                             customProjectTimeRecord.IsBillable = projectTimeRecordObj.MateTimeRecord?.IsBillable;
        //                             customProjectTimeRecord.ServiceArticleId = projectTimeRecordObj.MateTimeRecord?.ServiceArticleId;
        //                             customProjectTimeRecord.CreatedOn = projectTimeRecordObj.MateTimeRecord?.CreatedOn;
        //                             customProjectTimeRecord.CurrencyName = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                             var currentProjectContractArticle = contractArticles.Where(t => t.ServiceArticleId == customProjectTimeRecord.ServiceArticleId).FirstOrDefault();
        //                             if (currentProjectContractArticle != null)
        //                             {
        //                                 var contractObj = currentProjectContractArticle.Contract;
        //                                 if (contractObj != null && currentProjectContractArticle != null && currentProjectContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                 {
        //                                     customProjectTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                                 }
        //                                 else
        //                                 {
        //                                     customProjectTimeRecord.TotalAmount = (projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                                 }
        //                             }

        //                             // customProjectTimeRecord.TotalDiscount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                             if (projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                             {
        //                                 var taxId = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                 if (taxId != null)
        //                                 {
        //                                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                     customProjectTimeRecord.TotalTaxAmount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 customProjectTimeRecord.TotalTaxAmount = 0;
        //                             }
        //                             customProjectTimeRecord.PayableAmount = customProjectTimeRecord.TotalAmount + customProjectTimeRecord.TotalTaxAmount;
        //                             // ContractTotalAmount = ContractTotalAmount + customProjectTimeRecord.PayableAmount.Value;
        //                             clientProject.TaskTrackedTime.Add(customProjectTimeRecord);

        //                             projectTotalAmount = projectTotalAmount + customProjectTimeRecord.PayableAmount;
        //                         }
        //                     }

        //                     #region Logic for project Task

        //                     /////////////////////////////////////////////////////////////Add logic for project task //////////////////////////////////////////
        //                     var projectTasks = _employeeTaskService.GetAllTaskByProjectId(projectObj.Id);

        //                     foreach (var projectTaskObj in projectTasks)
        //                     {
        //                         var projectTaskTimeRecords = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(projectTaskObj.Id, StartDate.Value, EndDate.Value)
        //                         .Where(t => t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true).ToList();

        //                         ClientTask projectTask = new ClientTask();
        //                         projectTask = _mapper.Map<ClientTask>(projectTaskObj);
        //                         foreach (var projectTaskTimeRecordObj in projectTaskTimeRecords)
        //                         {
        //                             if (projectTaskTimeRecordObj.MateTimeRecord != null)
        //                             {
        //                                 var s = projectTaskTimeRecordObj;
        //                                 CustomTimeRecord customProjectTaskTimeRecord = new CustomTimeRecord();
        //                                 customProjectTaskTimeRecord.Id = s.MateTimeRecord.Id;
        //                                 customProjectTaskTimeRecord.UserId = s.MateTimeRecord?.UserId;
        //                                 customProjectTaskTimeRecord.Duration = s.MateTimeRecord?.Duration;
        //                                 customProjectTaskTimeRecord.Comment = s.MateTimeRecord?.Comment;
        //                                 customProjectTaskTimeRecord.IsBillable = s.MateTimeRecord?.IsBillable;
        //                                 customProjectTaskTimeRecord.CreatedOn = s.MateTimeRecord?.CreatedOn;
        //                                 customProjectTaskTimeRecord.CurrencyName = s.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                 customProjectTaskTimeRecord.ServiceArticleId = s.MateTimeRecord?.ServiceArticleId;

        //                                 if (projectTaskTimeRecordObj.MateTimeRecord.ServiceArticle != null)
        //                                 {
        //                                     var currentProjectTaskContractArticle = contractArticles.Where(t => t.ServiceArticleId == customProjectTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                     if (currentProjectTaskContractArticle != null)
        //                                     {
        //                                         var contractObj = currentProjectTaskContractArticle.Contract;
        //                                         if (currentProjectTaskContractArticle != null && currentProjectTaskContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                         {
        //                                             customProjectTaskTimeRecord.UnitPrice = contractObj?.DefaultUnitPrice;
        //                                             customProjectTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (s.MateTimeRecord?.Duration);
        //                                         }
        //                                         else
        //                                         {
        //                                             customProjectTaskTimeRecord.UnitPrice = s.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                                             customProjectTaskTimeRecord.TotalAmount = (s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration);
        //                                         }
        //                                     }
        //                                 }
        //                                 // customTaskTimeRecord.ServiceArticle = s.MateTimeRecord?.ServiceArticle;

        //                                 // customTaskTimeRecord.TotalDiscount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                 if (s.MateTimeRecord != null && s.MateTimeRecord != null && s.MateTimeRecord?.ServiceArticle != null && s.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                 {
        //                                     var taxId = s.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                     if (taxId != null)
        //                                     {
        //                                         var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                         var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                         customProjectTaskTimeRecord.TotalTaxAmount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                     }
        //                                 }
        //                                 else
        //                                 {
        //                                     customProjectTaskTimeRecord.TotalTaxAmount = 0;
        //                                 }
        //                                 customProjectTaskTimeRecord.PayableAmount = customProjectTaskTimeRecord.TotalAmount + customProjectTaskTimeRecord.TotalTaxAmount;
        //                                 projectTotalAmount = projectTotalAmount + customProjectTaskTimeRecord.PayableAmount;
        //                                 // ContractTotalAmount = ContractTotalAmount + customSubTaskTimeRecord.PayableAmount.Value;
        //                                 projectTask.TaskTrackedTime.Add(customProjectTaskTimeRecord);
        //                             }

        //                         }


        //                         /////////////////////////////--------------SubTask Start---------------///////////////////////////////
        //                         var subTaskList = _employeeSubTaskService.GetAllSubTaskByTask(projectTaskObj.Id);
        //                         foreach (var employeeSubTaskObj in subTaskList)
        //                         {
        //                             var subTaskTimeRecords = _mateSubTaskTimeRecordService.GetMateSubTaskTimeRecordBySubTaskId(employeeSubTaskObj.Id, StartDate.Value, EndDate.Value)
        //                             .Where(t => t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true).ToList();

        //                             ClientSubTask clientSubTask = new ClientSubTask();
        //                             clientSubTask = _mapper.Map<ClientSubTask>(employeeSubTaskObj);

        //                             foreach (var subTaskTimeRecordObj in subTaskTimeRecords)
        //                             {
        //                                 if (subTaskTimeRecordObj.MateTimeRecord != null)
        //                                 {
        //                                     var s = subTaskTimeRecordObj;
        //                                     CustomTimeRecord customProjectSubTaskTimeRecord = new CustomTimeRecord();
        //                                     customProjectSubTaskTimeRecord.Id = s.MateTimeRecord.Id;
        //                                     customProjectSubTaskTimeRecord.UserId = s.MateTimeRecord?.UserId;
        //                                     customProjectSubTaskTimeRecord.Duration = s.MateTimeRecord?.Duration;
        //                                     customProjectSubTaskTimeRecord.Comment = s.MateTimeRecord?.Comment;
        //                                     customProjectSubTaskTimeRecord.IsBillable = s.MateTimeRecord?.IsBillable;
        //                                     customProjectSubTaskTimeRecord.CreatedOn = s.MateTimeRecord?.CreatedOn;
        //                                     customProjectSubTaskTimeRecord.CurrencyName = s.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                     customProjectSubTaskTimeRecord.ServiceArticleId = s.MateTimeRecord?.ServiceArticleId;

        //                                     if (subTaskTimeRecordObj.MateTimeRecord.ServiceArticle != null)
        //                                     {
        //                                         var currentSubTaskContractArticle = contractArticles.Where(t => t.ServiceArticleId == customProjectSubTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                         if (currentSubTaskContractArticle != null)
        //                                         {
        //                                             var contractObj = currentSubTaskContractArticle.Contract;
        //                                             if (currentSubTaskContractArticle != null && currentSubTaskContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                             {
        //                                                 customProjectSubTaskTimeRecord.UnitPrice = contractObj?.DefaultUnitPrice;
        //                                                 customProjectSubTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (s.MateTimeRecord?.Duration);
        //                                             }
        //                                             else
        //                                             {
        //                                                 customProjectSubTaskTimeRecord.UnitPrice = s.MateTimeRecord?.ServiceArticle?.UnitPrice;
        //                                                 customProjectSubTaskTimeRecord.TotalAmount = (s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration);
        //                                             }
        //                                         }
        //                                     }
        //                                     // customTaskTimeRecord.ServiceArticle = s.MateTimeRecord?.ServiceArticle;

        //                                     // customTaskTimeRecord.TotalDiscount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                     if (s.MateTimeRecord != null && s.MateTimeRecord != null && s.MateTimeRecord?.ServiceArticle != null && s.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                     {
        //                                         var taxId = s.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                         if (taxId != null)
        //                                         {
        //                                             var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                             var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                             customProjectSubTaskTimeRecord.TotalTaxAmount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                         }
        //                                     }
        //                                     else
        //                                     {
        //                                         customProjectSubTaskTimeRecord.TotalTaxAmount = 0;
        //                                     }
        //                                     customProjectSubTaskTimeRecord.PayableAmount = customProjectSubTaskTimeRecord.TotalAmount + customProjectSubTaskTimeRecord.TotalTaxAmount;
        //                                     projectTotalAmount = projectTotalAmount + customProjectSubTaskTimeRecord.PayableAmount;
        //                                     // ContractTotalAmount = ContractTotalAmount + customSubTaskTimeRecord.PayableAmount.Value;
        //                                     clientSubTask.TaskTrackedTime.Add(customProjectSubTaskTimeRecord);
        //                                 }

        //                             }
        //                             //////////////////////////////--------------SubTask end------------------///////////////////////////////


        //                             ////////////////////////////-----------------ChildTask start----------------------//////////////////////////////////////

        //                             var childTaskList = _employeeChildTaskService.GetAllChildTaskBySubTask(employeeSubTaskObj.Id);
        //                             foreach (var employeeChildTaskObj in childTaskList)
        //                             {
        //                                 var childTaskTimeRecords = _mateChildTaskTimeRecordService.GetMateChildTaskTimeRecordByChildTaskId(employeeChildTaskObj.Id, StartDate.Value, EndDate.Value)
        //                                 .Where(t => t.MateTimeRecord != null && t.MateTimeRecord.IsBillable == true).ToList();

        //                                 ClientChildTask clientChildTask = new ClientChildTask();
        //                                 clientChildTask = _mapper.Map<ClientChildTask>(employeeChildTaskObj);

        //                                 foreach (var childTaskTimeRecordObj in childTaskTimeRecords)
        //                                 {
        //                                     if (childTaskTimeRecordObj.MateTimeRecord != null)
        //                                     {
        //                                         var c = childTaskTimeRecordObj;
        //                                         CustomTimeRecord customProjectChildTaskTimeRecord = new CustomTimeRecord();
        //                                         customProjectChildTaskTimeRecord.Id = c.MateTimeRecord.Id;
        //                                         customProjectChildTaskTimeRecord.UserId = c.MateTimeRecord?.UserId;
        //                                         customProjectChildTaskTimeRecord.Duration = c.MateTimeRecord?.Duration;
        //                                         customProjectChildTaskTimeRecord.Comment = c.MateTimeRecord?.Comment;
        //                                         customProjectChildTaskTimeRecord.IsBillable = c.MateTimeRecord?.IsBillable;
        //                                         customProjectChildTaskTimeRecord.CreatedOn = c.MateTimeRecord?.CreatedOn;
        //                                         customProjectChildTaskTimeRecord.CurrencyName = c.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                         customProjectChildTaskTimeRecord.ServiceArticleId = c.MateTimeRecord?.ServiceArticleId;


        //                                         if (childTaskTimeRecordObj.MateTimeRecord.ServiceArticle != null)
        //                                         {
        //                                             var currentChildTaskContractArticle = contractArticles.Where(t => t.ServiceArticleId == customProjectChildTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                             if (currentChildTaskContractArticle != null)
        //                                             {
        //                                                 var contractObj = currentChildTaskContractArticle.Contract;
        //                                                 if (currentChildTaskContractArticle != null && currentChildTaskContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                                 {
        //                                                     customProjectChildTaskTimeRecord.UnitPrice = contractObj?.DefaultUnitPrice;
        //                                                     customProjectChildTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (c.MateTimeRecord?.Duration);
        //                                                 }
        //                                                 else
        //                                                 {
        //                                                     customProjectChildTaskTimeRecord.UnitPrice = contractObj?.DefaultUnitPrice;
        //                                                     customProjectChildTaskTimeRecord.TotalAmount = (c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration);
        //                                                 }
        //                                             }
        //                                         }
        //                                         // customTaskTimeRecord.ServiceArticle = c.MateTimeRecord?.ServiceArticle;

        //                                         // customTaskTimeRecord.TotalDiscount = (((c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                         if (c.MateTimeRecord != null && c.MateTimeRecord != null && c.MateTimeRecord?.ServiceArticle != null && c.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                         {
        //                                             var taxId = c.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                             if (taxId != null)
        //                                             {
        //                                                 var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                                 var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                                 customProjectChildTaskTimeRecord.TotalTaxAmount = (((c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                             }
        //                                         }
        //                                         else
        //                                         {
        //                                             customProjectChildTaskTimeRecord.TotalTaxAmount = 0;
        //                                         }


        //                                         customProjectChildTaskTimeRecord.PayableAmount = customProjectChildTaskTimeRecord.TotalAmount + customProjectChildTaskTimeRecord.TotalTaxAmount;
        //                                         projectTotalAmount = projectTotalAmount + customProjectChildTaskTimeRecord.PayableAmount;
        //                                         // ContractTotalAmount = ContractTotalAmount + customChildTaskTimeRecord.PayableAmount.Value;
        //                                         clientChildTask.TaskTrackedTime.Add(customProjectChildTaskTimeRecord);
        //                                     }
        //                                 }
        //                                 clientSubTask.ChildTasks.Add(clientChildTask);
        //                             }
        //                             ////////////////////////////-----------------ChildTask end----------------------/////////////////////////////////////

        //                             projectTask.SubTasks.Add(clientSubTask);
        //                         }
        //                         // contractObj.Tasks.Add(clientTask);

        //                         clientProject.Tasks.Add(projectTask);
        //                     }

        //                     //////////////////////////////////////////////////////////// End logic for project task //////////////////////////////////////////
        //                     #endregion

        //                     clientProject.PayableAmount = projectTotalAmount;
        //                     clientVM.Projects.Add(clientProject);
        //                 }

        //                 // -------------------------------------------------End logic for client projects --------------------------------------------------


        //                 // foreach (var projectObj in clientProjects)
        //                 // {
        //                 //     var projectTimeRecords = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj.Id, StartDate.Value, EndDate.Value)
        //                 //     .Where(t => t.MateTimeRecord != null && contrctServiceArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                 //     ClientProject clientProject = new ClientProject();
        //                 //     clientProject = _mapper.Map<ClientProject>(projectObj);

        //                 //     foreach (var projectTimeRecordObj in projectTimeRecords)
        //                 //     {
        //                 //         if (projectTimeRecordObj.MateTimeRecord != null)
        //                 //         {
        //                 //             CustomTimeRecord customProjectTimeRecord = new CustomTimeRecord();
        //                 //             customProjectTimeRecord.Id = projectTimeRecordObj.MateTimeRecord.Id;
        //                 //             customProjectTimeRecord.UserId = projectTimeRecordObj.MateTimeRecord?.UserId;
        //                 //             customProjectTimeRecord.Duration = projectTimeRecordObj.MateTimeRecord?.Duration;
        //                 //             customProjectTimeRecord.Comment = projectTimeRecordObj.MateTimeRecord?.Comment;
        //                 //             customProjectTimeRecord.IsBillable = projectTimeRecordObj.MateTimeRecord?.IsBillable;
        //                 //             customProjectTimeRecord.ServiceArticleId = projectTimeRecordObj.MateTimeRecord?.ServiceArticleId;
        //                 //             customProjectTimeRecord.CreatedOn = projectTimeRecordObj.MateTimeRecord?.CreatedOn;
        //                 //             customProjectTimeRecord.CurrencyName = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                 //             var currentContractArticle = contractServiceArticleList.Where(t => t.ServiceArticleId == customProjectTimeRecord.ServiceArticleId).FirstOrDefault();
        //                 //             if (contractObj != null && currentContractArticle != null && currentContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                 //             {
        //                 //                 customProjectTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                 //             }
        //                 //             else
        //                 //             {
        //                 //                 customProjectTimeRecord.TotalAmount = (projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                 //             }
        //                 //             // customProjectTimeRecord.TotalDiscount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                 //             if (projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                 //             {
        //                 //                 var taxId = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.TaxId;
        //                 //                 if (taxId != null)
        //                 //                 {
        //                 //                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                 //                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                 //                     customProjectTimeRecord.TotalTaxAmount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                 //                 }
        //                 //             }
        //                 //             else
        //                 //             {
        //                 //                 customProjectTimeRecord.TotalTaxAmount = 0;
        //                 //             }
        //                 //             customProjectTimeRecord.PayableAmount = customProjectTimeRecord.TotalAmount + customProjectTimeRecord.TotalTaxAmount;
        //                 //             ContractTotalAmount = ContractTotalAmount + customProjectTimeRecord.PayableAmount.Value;
        //                 //             clientProject.TaskTrackedTime.Add(customProjectTimeRecord);
        //                 //         }
        //                 //     }
        //                 //     contractObj.Projects.Add(clientProject);
        //                 // }
        //                 // if (contractObj.Discount != null)
        //                 // {
        //                 //     contractObj.PayableAmount = ContractTotalAmount - (ContractTotalAmount * (contractObj.Discount) / 100);
        //                 // }
        //                 // else
        //                 // {
        //                 //     contractObj.PayableAmount = ContractTotalAmount;
        //                 // }

        //                 // clientResponseObj.Contracts.Add(contractObj);
        //                 // }
        //                 clientResponseObj.TotalAmount = clientResponseObj.Contracts.Sum(t => t.PayableAmount);
        //                 // var TaskList = _employeetas
        //                 clientContractInvoiceResponses.Add(clientResponseObj);
        //                 clientVM.TotalAmount = clientVM.Projects.Sum(t => t.PayableAmount) + clientVM.Tasks.Sum(t => t.PayableAmount);
        //                 clientVMs.Add(clientVM);

        //                 //---Start for client invoice entry---
        //                 ClientInvoice clientInvoiceObj = new ClientInvoice();
        //                 clientInvoiceObj.TotalAmount = clientVM.TotalAmount;
        //                 clientInvoiceObj.ClientId = clientObj.Id;
        //                 clientInvoiceObj.InvoiceDate = DateTime.UtcNow;
        //                 clientInvoiceObj.StartDate = StartDate;
        //                 clientInvoiceObj.EndDate = EndDate;
        //                 clientInvoiceObj.InvoiceNo = DateTime.UtcNow.ToString("ddMMyyhhmmss");
        //                 // clientInvoiceObj.CreatedBy = UserId;
        //                 var addedClientInvoice = await _clientInvoiceService.CheckInsertOrUpdate(clientInvoiceObj);
        //                 //---End for client invoice entry---
        //                 if (addedClientInvoice != null)
        //                 {
        //                     //For contract wise email notification
        //                     if (clientObj.Id != 0)
        //                     {
        //                         var PrimaryClientEmailObj = _clientEmailService.GetByClientIdWithPrimary(clientObj.Id);
        //                         if (PrimaryClientEmailObj != null)
        //                         {
        //                             string ClientName = clientObj.FirstName + ' ' + clientObj.LastName;
        //                             await sendEmail.SendEmailClientBasedInvoiceNotification(PrimaryClientEmailObj.Email, ClientName, addedClientInvoice.StartDate, addedClientInvoice.EndDate, clientVM.TotalAmount);
        //                         }
        //                     }
        //                 }

        //             }
        //         }
        //     }
        //     return new List<ClientVM>(clientVMs);
        // }

        // [HttpGet]
        // public async Task<List<ClientContractInvoiceResponse>> GenerateContractOrClientBaseInterval()
        // {
        //     List<ClientInvoice> clientInvoices = new List<ClientInvoice>();
        //     List<Client> ClientList = _clientService.GetAll();

        //     List<MateTimeRecord> timeRecords = _mateTimeRecordService.GetAll();
        //     List<ClientContractInvoiceResponse> clientContractInvoiceResponses = new List<ClientContractInvoiceResponse>();
        //     List<ContractInvoice> contractInvoiceList = _contractInvoiceService.GetAll();
        //     List<long> clientInvoiceIdListForContract = contractInvoiceList.Where(t => t.ClientInvoiceId != null).Select(t => t.ClientInvoiceId.Value).ToList();

        //     foreach (var clientObj in ClientList)
        //     {
        //         var clientTasks = _employeeTaskService.GetTaskByClient(clientObj.Id);
        //         var clientProjects = _employeeProjectService.GetAllByClientId(clientObj.Id);
        //         List<long> ProjectIds = clientProjects.Select(t => t.Id).ToList();
        //         var projectTasks = _employeeTaskService.GetAllTaskByProjectIdList(ProjectIds);
        //         clientTasks.AddRange(projectTasks);
        //         List<long> TaskIds = clientTasks.Select(t => t.Id).ToList();
        //         var subTasks = _employeeSubTaskService.GetAllActiveByTaskIds(TaskIds);
        //         List<long> SubTaskIds = subTasks.Select(t => t.Id).ToList();
        //         var childTasks = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIds);

        //         if (clientObj.IsContractBaseInvoice == false)
        //         {
        //             InvoiceInterval clientInvoiceInternal = clientObj.InvoiceInterval;
        //             ClientContractInvoiceResponse clientResponseObj = new ClientContractInvoiceResponse();
        //             clientResponseObj.Id = clientObj.Id;
        //             clientResponseObj.Name = clientObj.Name;
        //             var clientInvoiceList = _clientInvoiceService.GetAllByClient(clientObj.Id);

        //             clientInvoiceList = clientInvoiceList.Where(t => t.DeletedOn == null && clientInvoiceIdListForContract.Any(b => t.Id != b)).OrderBy(t => t.StartDate).ToList();
        //             DateTime? StartDate = null;
        //             DateTime? EndDate = null;

        //             // start logic for get startdate from last existing invoice enddate
        //             if (clientInvoiceList != null && clientInvoiceList.Count() > 0)
        //             {
        //                 var lastClientInvoice = clientInvoiceList.LastOrDefault();
        //                 if (lastClientInvoice != null && lastClientInvoice.EndDate != null)
        //                 {
        //                     StartDate = lastClientInvoice?.EndDate.Value.AddDays(1).AddHours(0).AddMinutes(0).AddSeconds(0);
        //                     EndDate = StartDate.Value.AddDays(clientInvoiceInternal.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                 }
        //             }
        //             else
        //             {
        //                 // start logic for get startdate from first time start timerecord date for generate first invoice startDate
        //                 TaskIds = clientTasks.Select(t => t.Id).ToList();

        //                 var taskStartDate = _mateTaskTimeRecordService.GetTaskTimeRecordStartDate(TaskIds);
        //                 var projectStartDate = _mateProjectTimeRecordService.GetProjectTimeRecordStartDate(ProjectIds);
        //                 if (taskStartDate != null && projectStartDate == null)
        //                 {
        //                     StartDate = taskStartDate;
        //                 }
        //                 else if (taskStartDate == null && projectStartDate != null)
        //                 {
        //                     StartDate = projectStartDate;
        //                 }
        //                 else if (projectStartDate != null && taskStartDate != null)
        //                 {
        //                     StartDate = taskStartDate > projectStartDate ? projectStartDate : StartDate;
        //                 }
        //                 if (StartDate != null)
        //                 {
        //                     StartDate = StartDate.Value.AddHours(0).AddMinutes(0).AddSeconds(0);
        //                     EndDate = StartDate.Value.AddDays(clientInvoiceInternal.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
        //                 }
        //                 // }
        //             }
        //             if (StartDate != null && EndDate != null && EndDate.Value.Date == DateTime.Today.Date)
        //             {

        //                 var clientContracts = _contractService.GetByClient(clientObj.Id);

        //                 // var now = DateTime.Now;
        //                 // var CurrentMonthFirstDate = new DateTime(now.Year, now.Month, 1);
        //                 // var CurrentMonthLastDate = CurrentMonthFirstDate.AddMonths(1).AddDays(-1);
        //                 foreach (var clientContractObj in clientContracts)
        //                 {
        //                     var contractObj = _mapper.Map<ClientContract>(clientContractObj);
        //                     if (clientContractObj.InvoiceInterval != null)
        //                     {
        //                         contractObj.InvoiceIntervalName = clientContractObj.InvoiceInterval?.Name;
        //                         contractObj.Interval = clientContractObj.InvoiceInterval?.Interval;
        //                     }
        //                     var contractServiceArticleList = _contractArticleService.GetByContract(clientContractObj.Id);
        //                     var contrctServiceArticleIdList = contractServiceArticleList.Select(t => t.ServiceArticleId).ToList();

        //                     long ContractTotalAmount = 0;
        //                     // var calculateDiscount = Convert.ToInt32(discount / 100);

        //                     // var totalDiscount = calculateDiscount;

        //                     foreach (var taskObj in clientTasks)
        //                     {
        //                         var taskTimeRecords = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj.Id, StartDate.Value, EndDate.Value)
        //                         .Where(t => t.MateTimeRecord != null && contrctServiceArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                         ClientTask clientTask = new ClientTask();
        //                         clientTask = _mapper.Map<ClientTask>(taskObj);

        //                         foreach (var taskTimeRecordObj in taskTimeRecords)
        //                         {
        //                             var t = taskTimeRecordObj;
        //                             CustomTimeRecord customTaskTimeRecord = new CustomTimeRecord();
        //                             customTaskTimeRecord.Id = taskTimeRecordObj.MateTimeRecord.Id;
        //                             customTaskTimeRecord.UserId = t.MateTimeRecord?.UserId;
        //                             customTaskTimeRecord.Duration = t.MateTimeRecord?.Duration;
        //                             customTaskTimeRecord.Comment = t.MateTimeRecord?.Comment;
        //                             customTaskTimeRecord.IsBillable = t.MateTimeRecord?.IsBillable;
        //                             customTaskTimeRecord.CreatedOn = t.MateTimeRecord?.CreatedOn;
        //                             customTaskTimeRecord.CurrencyName = t.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                             customTaskTimeRecord.ServiceArticleId = t.MateTimeRecord?.ServiceArticleId;
        //                             var currentContractArticle = contractServiceArticleList.Where(t => t.ServiceArticleId == customTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                             if (contractObj != null && currentContractArticle != null && currentContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                             {
        //                                 customTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (t.MateTimeRecord?.Duration);
        //                             }
        //                             else
        //                             {
        //                                 customTaskTimeRecord.TotalAmount = (t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration);
        //                             }
        //                             // customTaskTimeRecord.ServiceArticle = t.MateTimeRecord?.ServiceArticle;

        //                             // customTaskTimeRecord.TotalDiscount = (((t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                             if (t.MateTimeRecord != null && t.MateTimeRecord != null && t.MateTimeRecord?.ServiceArticle != null && t.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                             {
        //                                 var taxId = t.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                 if (taxId != null)
        //                                 {
        //                                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                     customTaskTimeRecord.TotalTaxAmount = (((t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                 }
        //                             }
        //                             else
        //                             {
        //                                 customTaskTimeRecord.TotalTaxAmount = 0;
        //                             }
        //                             customTaskTimeRecord.PayableAmount = customTaskTimeRecord.TotalAmount + customTaskTimeRecord.TotalTaxAmount;

        //                             ContractTotalAmount = ContractTotalAmount + customTaskTimeRecord.PayableAmount.Value;
        //                             clientTask.TaskTrackedTime.Add(customTaskTimeRecord);
        //                             // contractObj.ContractSubscriptions = contrctServiceArticleList;
        //                         }

        //                         /////////////////////////////--------------SubTask Start---------------///////////////////////////////
        //                         var subTaskList = _employeeSubTaskService.GetAllSubTaskByTask(taskObj.Id);
        //                         foreach (var employeeSubTaskObj in subTaskList)
        //                         {
        //                             var subTaskTimeRecords = _mateSubTaskTimeRecordService.GetMateSubTaskTimeRecordBySubTaskId(employeeSubTaskObj.Id, StartDate.Value, EndDate.Value)
        //                                                             .Where(t => t.MateTimeRecord != null && contrctServiceArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                             ClientSubTask clientSubTask = new ClientSubTask();
        //                             clientSubTask = _mapper.Map<ClientSubTask>(employeeSubTaskObj);

        //                             foreach (var subTaskTimeRecordObj in subTaskTimeRecords)
        //                             {
        //                                 var s = subTaskTimeRecordObj;
        //                                 CustomTimeRecord customSubTaskTimeRecord = new CustomTimeRecord();
        //                                 customSubTaskTimeRecord.Id = s.MateTimeRecord.Id;
        //                                 customSubTaskTimeRecord.UserId = s.MateTimeRecord?.UserId;
        //                                 customSubTaskTimeRecord.Duration = s.MateTimeRecord?.Duration;
        //                                 customSubTaskTimeRecord.Comment = s.MateTimeRecord?.Comment;
        //                                 customSubTaskTimeRecord.IsBillable = s.MateTimeRecord?.IsBillable;
        //                                 customSubTaskTimeRecord.CreatedOn = s.MateTimeRecord?.CreatedOn;
        //                                 customSubTaskTimeRecord.CurrencyName = s.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                 customSubTaskTimeRecord.ServiceArticleId = s.MateTimeRecord?.ServiceArticleId;
        //                                 var currentSubTaskContractArticle = contractServiceArticleList.Where(t => t.ServiceArticleId == customSubTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                 if (contractObj != null && currentSubTaskContractArticle != null && currentSubTaskContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                 {
        //                                     customSubTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (s.MateTimeRecord?.Duration);
        //                                 }
        //                                 else
        //                                 {
        //                                     customSubTaskTimeRecord.TotalAmount = (s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration);
        //                                 }
        //                                 // customTaskTimeRecord.ServiceArticle = s.MateTimeRecord?.ServiceArticle;

        //                                 // customTaskTimeRecord.TotalDiscount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                 if (s.MateTimeRecord != null && s.MateTimeRecord != null && s.MateTimeRecord?.ServiceArticle != null && s.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                 {
        //                                     var taxId = s.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                     if (taxId != null)
        //                                     {
        //                                         var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                         var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                         customSubTaskTimeRecord.TotalTaxAmount = (((s.MateTimeRecord?.ServiceArticle?.UnitPrice) * (s.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                     }
        //                                 }
        //                                 else
        //                                 {
        //                                     customSubTaskTimeRecord.TotalTaxAmount = 0;
        //                                 }


        //                                 customSubTaskTimeRecord.PayableAmount = customSubTaskTimeRecord.TotalAmount + customSubTaskTimeRecord.TotalTaxAmount;

        //                                 ContractTotalAmount = ContractTotalAmount + customSubTaskTimeRecord.PayableAmount.Value;
        //                                 clientSubTask.TaskTrackedTime.Add(customSubTaskTimeRecord);
        //                             }
        //                             //////////////////////////////--------------SubTask end------------------///////////////////////////////


        //                             ////////////////////////////-----------------ChildTask start----------------------//////////////////////////////////////

        //                             var childTaskList = _employeeChildTaskService.GetAllChildTaskBySubTask(employeeSubTaskObj.Id);
        //                             foreach (var employeeChildTaskObj in childTaskList)
        //                             {
        //                                 var childTaskTimeRecords = _mateChildTaskTimeRecordService.GetMateChildTaskTimeRecordByChildTaskId(employeeChildTaskObj.Id, StartDate.Value, EndDate.Value)
        //                                                                 .Where(t => t.MateTimeRecord != null && contrctServiceArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                                 ClientChildTask clientChildTask = new ClientChildTask();
        //                                 clientChildTask = _mapper.Map<ClientChildTask>(employeeChildTaskObj);

        //                                 foreach (var childTaskTimeRecordObj in childTaskTimeRecords)
        //                                 {
        //                                     var c = childTaskTimeRecordObj;
        //                                     CustomTimeRecord customChildTaskTimeRecord = new CustomTimeRecord();
        //                                     customChildTaskTimeRecord.Id = c.MateTimeRecord.Id;
        //                                     customChildTaskTimeRecord.UserId = c.MateTimeRecord?.UserId;
        //                                     customChildTaskTimeRecord.Duration = c.MateTimeRecord?.Duration;
        //                                     customChildTaskTimeRecord.Comment = c.MateTimeRecord?.Comment;
        //                                     customChildTaskTimeRecord.IsBillable = c.MateTimeRecord?.IsBillable;
        //                                     customChildTaskTimeRecord.CreatedOn = c.MateTimeRecord?.CreatedOn;
        //                                     customChildTaskTimeRecord.CurrencyName = c.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                     customChildTaskTimeRecord.ServiceArticleId = c.MateTimeRecord?.ServiceArticleId;
        //                                     var currentChildTaskContractArticle = contractServiceArticleList.Where(t => t.ServiceArticleId == customChildTaskTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                     if (contractObj != null && currentChildTaskContractArticle != null && currentChildTaskContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                     {
        //                                         customChildTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (c.MateTimeRecord?.Duration);
        //                                     }
        //                                     else
        //                                     {
        //                                         customChildTaskTimeRecord.TotalAmount = (c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration);
        //                                     }
        //                                     // customTaskTimeRecord.ServiceArticle = c.MateTimeRecord?.ServiceArticle;

        //                                     // customTaskTimeRecord.TotalDiscount = (((c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                     if (c.MateTimeRecord != null && c.MateTimeRecord != null && c.MateTimeRecord?.ServiceArticle != null && c.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                     {
        //                                         var taxId = c.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                         if (taxId != null)
        //                                         {
        //                                             var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                             var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                             customChildTaskTimeRecord.TotalTaxAmount = (((c.MateTimeRecord?.ServiceArticle?.UnitPrice) * (c.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                         }
        //                                     }
        //                                     else
        //                                     {
        //                                         customChildTaskTimeRecord.TotalTaxAmount = 0;
        //                                     }


        //                                     customChildTaskTimeRecord.PayableAmount = customChildTaskTimeRecord.TotalAmount + customChildTaskTimeRecord.TotalTaxAmount;

        //                                     ContractTotalAmount = ContractTotalAmount + customChildTaskTimeRecord.PayableAmount.Value;
        //                                     clientChildTask.TaskTrackedTime.Add(customChildTaskTimeRecord);
        //                                 }
        //                                 clientSubTask.ChildTasks.Add(clientChildTask);
        //                             }
        //                             ////////////////////////////-----------------ChildTask end----------------------/////////////////////////////////////

        //                             clientTask.SubTasks.Add(clientSubTask);
        //                         }
        //                         contractObj.Tasks.Add(clientTask);
        //                     }

        //                     foreach (var projectObj in clientProjects)
        //                     {
        //                         var projectTimeRecords = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj.Id, StartDate.Value, EndDate.Value)
        //                         .Where(t => t.MateTimeRecord != null && contrctServiceArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                         ClientProject clientProject = new ClientProject();
        //                         clientProject = _mapper.Map<ClientProject>(projectObj);

        //                         foreach (var projectTimeRecordObj in projectTimeRecords)
        //                         {
        //                             if (projectTimeRecordObj.MateTimeRecord != null)
        //                             {
        //                                 CustomTimeRecord customProjectTimeRecord = new CustomTimeRecord();
        //                                 customProjectTimeRecord.Id = projectTimeRecordObj.MateTimeRecord.Id;
        //                                 customProjectTimeRecord.UserId = projectTimeRecordObj.MateTimeRecord?.UserId;
        //                                 customProjectTimeRecord.Duration = projectTimeRecordObj.MateTimeRecord?.Duration;
        //                                 customProjectTimeRecord.Comment = projectTimeRecordObj.MateTimeRecord?.Comment;
        //                                 customProjectTimeRecord.IsBillable = projectTimeRecordObj.MateTimeRecord?.IsBillable;
        //                                 customProjectTimeRecord.ServiceArticleId = projectTimeRecordObj.MateTimeRecord?.ServiceArticleId;
        //                                 customProjectTimeRecord.CreatedOn = projectTimeRecordObj.MateTimeRecord?.CreatedOn;
        //                                 customProjectTimeRecord.CurrencyName = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                                 var currentContractArticle = contractServiceArticleList.Where(t => t.ServiceArticleId == customProjectTimeRecord.ServiceArticleId).FirstOrDefault();
        //                                 if (contractObj != null && currentContractArticle != null && currentContractArticle.IsContractUnitPrice == true && contractObj.DefaultUnitPrice != null)
        //                                 {
        //                                     customProjectTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                                 }
        //                                 else
        //                                 {
        //                                     customProjectTimeRecord.TotalAmount = (projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                                 }
        //                                 // customProjectTimeRecord.TotalDiscount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                                 if (projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                                 {
        //                                     var taxId = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.TaxId;
        //                                     if (taxId != null)
        //                                     {
        //                                         var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                         var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                         customProjectTimeRecord.TotalTaxAmount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                                     }
        //                                 }
        //                                 else
        //                                 {
        //                                     customProjectTimeRecord.TotalTaxAmount = 0;
        //                                 }
        //                                 customProjectTimeRecord.PayableAmount = customProjectTimeRecord.TotalAmount + customProjectTimeRecord.TotalTaxAmount;
        //                                 ContractTotalAmount = ContractTotalAmount + customProjectTimeRecord.PayableAmount.Value;
        //                                 clientProject.TaskTrackedTime.Add(customProjectTimeRecord);
        //                             }
        //                         }
        //                         contractObj.Projects.Add(clientProject);
        //                     }
        //                     if (contractObj.Discount != null)
        //                     {
        //                         contractObj.PayableAmount = ContractTotalAmount - (ContractTotalAmount * (contractObj.Discount) / 100);
        //                     }
        //                     else
        //                     {
        //                         contractObj.PayableAmount = ContractTotalAmount;
        //                     }

        //                     clientResponseObj.Contracts.Add(contractObj);
        //                 }
        //                 clientResponseObj.TotalAmount = clientResponseObj.Contracts.Sum(t => t.PayableAmount);
        //                 // var TaskList = _employeetas
        //                 clientContractInvoiceResponses.Add(clientResponseObj);
        //             }
        //         }
        //     }
        //     return new List<ClientContractInvoiceResponse>(clientContractInvoiceResponses);
        // }


        // [HttpGet]
        // public async Task<List<ClientContractInvoiceResponse>> GenerateMonthlyInvoice()
        // {
        //     List<ClientInvoice> clientInvoices = new List<ClientInvoice>();
        //     List<Client> ClientList = _clientService.GetAll();

        //     List<MateTimeRecord> timeRecords = _mateTimeRecordService.GetAll();
        //     List<ClientContractInvoiceResponse> clientContractInvoiceResponses = new List<ClientContractInvoiceResponse>();
        //     foreach (var clientObj in ClientList)
        //     {
        //         ClientContractInvoiceResponse clientResponseObj = new ClientContractInvoiceResponse();
        //         clientResponseObj.Id = clientObj.Id;
        //         clientResponseObj.Name = clientObj.Name;
        //         var clientInvoiceList = _clientInvoiceService.GetAllByClient(clientObj.Id);
        //         var clientContracts = _contractService.GetByClient(clientObj.Id);
        //         var clientTasks = _employeeTaskService.GetTaskByClient(clientObj.Id);
        //         var clientProjects = _employeeProjectService.GetAllByClientId(clientObj.Id);

        //         var now = DateTime.Now;
        //         var CurrentMonthFirstDate = new DateTime(now.Year, now.Month, 1);
        //         var CurrentMonthLastDate = CurrentMonthFirstDate.AddMonths(1).AddDays(-1);
        //         foreach (var clientContractObj in clientContracts)
        //         {
        //             var contractObj = _mapper.Map<ClientContract>(clientContractObj);
        //             if (clientContractObj.InvoiceInterval != null)
        //             {
        //                 contractObj.InvoiceIntervalName = clientContractObj.InvoiceInterval?.Name;
        //                 contractObj.Interval = clientContractObj.InvoiceInterval?.Interval;
        //             }
        //             var contrctServiceArticleList = _contractArticleService.GetByContract(clientContractObj.Id);
        //             var contrctServiceArticleIdList = contrctServiceArticleList.Select(t => t.ServiceArticleId).ToList();

        //             long ContractTotalAmount = 0;
        //             // var calculateDiscount = Convert.ToInt32(discount / 100);

        //             // var totalDiscount = calculateDiscount;

        //             foreach (var taskObj in clientTasks)
        //             {
        //                 var taskTimeRecords = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj.Id, CurrentMonthFirstDate, CurrentMonthLastDate)
        //                 .Where(t => t.MateTimeRecord != null && contrctServiceArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                 ClientTask clientTask = new ClientTask();
        //                 clientTask = _mapper.Map<ClientTask>(taskObj);

        //                 foreach (var taskTimeRecordObj in taskTimeRecords)
        //                 {
        //                     var t = taskTimeRecordObj;
        //                     CustomTimeRecord customTaskTimeRecord = new CustomTimeRecord();
        //                     customTaskTimeRecord.Id = taskTimeRecordObj.MateTimeRecord.Id;
        //                     customTaskTimeRecord.UserId = t.MateTimeRecord?.UserId;
        //                     customTaskTimeRecord.Duration = t.MateTimeRecord?.Duration;
        //                     customTaskTimeRecord.Comment = t.MateTimeRecord?.Comment;
        //                     customTaskTimeRecord.IsBillable = t.MateTimeRecord?.IsBillable;
        //                     customTaskTimeRecord.CreatedOn = t.MateTimeRecord?.CreatedOn;
        //                     customTaskTimeRecord.CurrencyName = t.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                     customTaskTimeRecord.ServiceArticleId = t.MateTimeRecord?.ServiceArticleId;

        //                     if (contractObj != null && contractObj.IsContractUnitPrice && contractObj.DefaultUnitPrice != null)
        //                     {
        //                         customTaskTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (t.MateTimeRecord?.Duration);
        //                     }
        //                     else
        //                     {
        //                         customTaskTimeRecord.TotalAmount = (t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration);
        //                     }
        //                     // customTaskTimeRecord.ServiceArticle = t.MateTimeRecord?.ServiceArticle;

        //                     // customTaskTimeRecord.TotalDiscount = (((t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                     if (t.MateTimeRecord != null && t.MateTimeRecord != null && t.MateTimeRecord?.ServiceArticle != null && t.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                     {
        //                         var taxId = t.MateTimeRecord?.ServiceArticle?.TaxId;
        //                         if (taxId != null)
        //                         {
        //                             var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                             var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                             customTaskTimeRecord.TotalTaxAmount = (((t.MateTimeRecord?.ServiceArticle?.UnitPrice) * (t.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                         }
        //                     }
        //                     else
        //                     {
        //                         customTaskTimeRecord.TotalTaxAmount = 0;
        //                     }
        //                     customTaskTimeRecord.PayableAmount = customTaskTimeRecord.TotalAmount + customTaskTimeRecord.TotalTaxAmount;

        //                     ContractTotalAmount = ContractTotalAmount + customTaskTimeRecord.PayableAmount.Value;

        //                     clientTask.TaskTrackedTime.Add(customTaskTimeRecord);
        //                 }
        //                 contractObj.Tasks.Add(clientTask);
        //                 // contractObj.ContractSubscriptions = contrctServiceArticleList;
        //             }

        //             foreach (var projectObj in clientProjects)
        //             {
        //                 var projectTimeRecords = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj.Id, CurrentMonthFirstDate, CurrentMonthLastDate)
        //                 .Where(t => t.MateTimeRecord != null && contrctServiceArticleIdList.Contains(t.MateTimeRecord.ServiceArticleId)).ToList();

        //                 ClientProject clientProject = new ClientProject();
        //                 clientProject = _mapper.Map<ClientProject>(projectObj);

        //                 foreach (var projectTimeRecordObj in projectTimeRecords)
        //                 {
        //                     if (projectTimeRecordObj.MateTimeRecord != null)
        //                     {
        //                         CustomTimeRecord customProjectTimeRecord = new CustomTimeRecord();
        //                         customProjectTimeRecord.Id = projectTimeRecordObj.MateTimeRecord.Id;
        //                         customProjectTimeRecord.UserId = projectTimeRecordObj.MateTimeRecord?.UserId;
        //                         customProjectTimeRecord.Duration = projectTimeRecordObj.MateTimeRecord?.Duration;
        //                         customProjectTimeRecord.Comment = projectTimeRecordObj.MateTimeRecord?.Comment;
        //                         customProjectTimeRecord.IsBillable = projectTimeRecordObj.MateTimeRecord?.IsBillable;
        //                         customProjectTimeRecord.ServiceArticleId = projectTimeRecordObj.MateTimeRecord?.ServiceArticleId;
        //                         customProjectTimeRecord.CreatedOn = projectTimeRecordObj.MateTimeRecord?.CreatedOn;
        //                         customProjectTimeRecord.CurrencyName = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //                         if (contractObj != null && contractObj.IsContractUnitPrice && contractObj.DefaultUnitPrice != null)
        //                         {
        //                             customProjectTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                         }
        //                         else
        //                         {
        //                             customProjectTimeRecord.TotalAmount = (projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //                         }
        //                         // customProjectTimeRecord.TotalDiscount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //                         if (projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //                         {
        //                             var taxId = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.TaxId;
        //                             if (taxId != null)
        //                             {
        //                                 var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                                 var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                                 customProjectTimeRecord.TotalTaxAmount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                             }
        //                         }
        //                         else
        //                         {
        //                             customProjectTimeRecord.TotalTaxAmount = 0;
        //                         }
        //                         customProjectTimeRecord.PayableAmount = customProjectTimeRecord.TotalAmount + customProjectTimeRecord.TotalTaxAmount;
        //                         ContractTotalAmount = ContractTotalAmount + customProjectTimeRecord.PayableAmount.Value;
        //                         clientProject.TaskTrackedTime.Add(customProjectTimeRecord);
        //                     }
        //                 }
        //                 contractObj.Projects.Add(clientProject);
        //             }
        //             if (contractObj.Discount != null)
        //             {
        //                 contractObj.PayableAmount = ContractTotalAmount - (ContractTotalAmount * (contractObj.Discount) / 100);
        //             }
        //             else
        //             {
        //                 contractObj.PayableAmount = ContractTotalAmount;
        //             }

        //             clientResponseObj.Contracts.Add(contractObj);
        //         }
        //         clientResponseObj.TotalAmount = clientResponseObj.Contracts.Sum(t => t.PayableAmount);
        //         // var TaskList = _employeetas
        //         clientContractInvoiceResponses.Add(clientResponseObj);
        //     }
        //     return new List<ClientContractInvoiceResponse>(clientContractInvoiceResponses);
        // }

        // private List<CustomTimeRecord> GetChildTimeRecordList(long SubTaskId, )
        // {
        //     var ChildTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(SubTaskId);
        //     List<long> ChildTaskIds = ChildTasks.Select(t => t.Id).ToList();
        //     var childTimeRecords = _mateChildTaskTimeRecordService.GetListByChildTaskIdList(ChildTaskIds);
        //     if (childTimeRecords != null && childTimeRecords.Count() > 0)
        //     {
        //         foreach (var childTaskTimeRecordObj in childTimeRecords)
        //         {
        //             CustomTimeRecord customProjectTimeRecord = new CustomTimeRecord();
        //             customProjectTimeRecord.Id = childTaskTimeRecordObj.MateTimeRecord.Id;
        //             customProjectTimeRecord.UserId = childTaskTimeRecordObj.MateTimeRecord?.UserId;
        //             customProjectTimeRecord.Duration = childTaskTimeRecordObj.MateTimeRecord?.Duration;
        //             customProjectTimeRecord.Comment = childTaskTimeRecordObj.MateTimeRecord?.Comment;
        //             customProjectTimeRecord.IsBillable = childTaskTimeRecordObj.MateTimeRecord?.IsBillable;
        //             customProjectTimeRecord.ServiceArticleId = childTaskTimeRecordObj.MateTimeRecord?.ServiceArticleId;
        //             customProjectTimeRecord.CreatedOn = childTaskTimeRecordObj.MateTimeRecord?.CreatedOn;
        //             customProjectTimeRecord.CurrencyName = childTaskTimeRecordObj.MateTimeRecord?.ServiceArticle?.Currency?.Code;
        //             if (contractObj != null && contractObj.IsContractUnitPrice && contractObj.DefaultUnitPrice != null)
        //             {
        //                 customProjectTimeRecord.TotalAmount = (contractObj?.DefaultUnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //             }
        //             else
        //             {
        //                 customProjectTimeRecord.TotalAmount = (projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration);
        //             }
        //             // customProjectTimeRecord.TotalDiscount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * contractObj.Discount / 100);
        //             if (projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle != null && projectTimeRecordObj.MateTimeRecord?.ServiceArticle.IsTaxable == true)
        //             {
        //                 var taxId = projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.TaxId;
        //                 if (taxId != null)
        //                 {
        //                     var taxRates = _taxRateService.GetByTaxId(taxId.Value);
        //                     var taxPercentage = taxRates.Where(t => t.Percentage != null).Sum(t => t.Percentage);
        //                     customProjectTimeRecord.TotalTaxAmount = (((projectTimeRecordObj.MateTimeRecord?.ServiceArticle?.UnitPrice) * (projectTimeRecordObj.MateTimeRecord?.Duration)) * taxPercentage / 100);
        //                 }
        //             }
        //             else
        //             {
        //                 customProjectTimeRecord.TotalTaxAmount = 0;
        //             }
        //             customProjectTimeRecord.PayableAmount = customProjectTimeRecord.TotalAmount + customProjectTimeRecord.TotalTaxAmount;
        //             ContractTotalAmount = ContractTotalAmount + customProjectTimeRecord.PayableAmount.Value;
        //         }
        //     }

        // }
    }


}