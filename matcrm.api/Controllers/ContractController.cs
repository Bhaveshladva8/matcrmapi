using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Context;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IContractArticleService _contractArticleService;
        private readonly IContractTypeService _contractTypeService;
        private readonly IContractActivityService _contractActivityService;
        private readonly IClientService _clientService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IInvoiceIntervalService _invoiceIntervalService;
        private readonly IContractInvoiceService _contractInvoiceService;
        private readonly IClientInvoiceService _clientInvoiceService;
        private readonly IProjectContractService _projectContractService;
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IEmployeeProjectActivityService _employeeProjectActivityService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IContractAssetService _contractAssetService;
        private readonly IEmployeeProjectTaskService _employeeProjectTaskService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeChildTaskAttachmentService _employeeChildTaskAttachmentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserSerivce;
        private readonly IEmployeeChildTaskTimeRecordService _employeeChildTaskTimeRecordService;
        private readonly IEmployeeChildTaskUserService _employeeChildTaskUserService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IEmployeeSubTaskAttachmentService _employeeSubTaskAttachmentService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeSubTaskTimeRecordService _employeeSubTaskTimeRecordService;
        private readonly IEmployeeSubTaskUserService _employeeSubTaskUserService;
        private readonly IEmployeeTaskAttachmentService _employeeTaskAttachmentService;
        private readonly IEmployeeClientTaskService IEmployeeClientTaskService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ContractController(IContractService contractService,
        IContractTypeService contractTypeService,
        IContractArticleService contractArticleService,
        IContractActivityService contractActivityService,
        IClientService clientService,
        IUserService userService,
        IRoleService roleService,
        IInvoiceIntervalService invoiceIntervalService,
        IContractInvoiceService contractInvoiceService,
        IClientInvoiceService clientInvoiceService,
        IProjectContractService projectContractService,
        IEmployeeProjectService employeeProjectService,
        IEmployeeProjectActivityService employeeProjectActivityService,
        IEmployeeTaskService employeeTaskService,
        IEmployeeTaskActivityService employeeTaskActivityService,
        IContractAssetService contractAssetService,
        IEmployeeProjectTaskService employeeProjectTaskService,
        IEmployeeSubTaskService employeeSubTaskService,
        IEmployeeChildTaskService employeeChildTaskService,
        IEmployeeChildTaskAttachmentService employeeChildTaskAttachmentService,
        IEmployeeChildTaskCommentService employeeChildTaskCommentService,
        IEmployeeSubTaskActivityService employeeSubTaskActivityService,
        IHostingEnvironment hostingEnvironment,
        IEmployeeTaskUserSerivce employeeTaskUserSerivce,
        IEmployeeChildTaskTimeRecordService employeeChildTaskTimeRecordService,
        IEmployeeChildTaskUserService employeeChildTaskUserService,
        IEmployeeChildTaskActivityService employeeChildTaskActivityService,
        IEmployeeSubTaskAttachmentService employeeSubTaskAttachmentService,
        IEmployeeSubTaskCommentService employeeSubTaskCommentService,
        IEmployeeSubTaskTimeRecordService employeeSubTaskTimeRecordService,
        IEmployeeSubTaskUserService employeeSubTaskUserService,
        IEmployeeTaskAttachmentService employeeTaskAttachmentService,
        IMapper mapper)
        {
            _contractService = contractService;
            _contractTypeService = contractTypeService;
            _contractArticleService = contractArticleService;
            _contractActivityService = contractActivityService;
            _clientService = clientService;
            _userService = userService;
            _roleService = roleService;
            _invoiceIntervalService = invoiceIntervalService;
            _contractInvoiceService = contractInvoiceService;
            _clientInvoiceService = clientInvoiceService;
            _projectContractService = projectContractService;
            _employeeProjectService = employeeProjectService;
            _employeeProjectActivityService = employeeProjectActivityService;
            _employeeTaskService = employeeTaskService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _contractAssetService = contractAssetService;
            _employeeProjectTaskService = employeeProjectTaskService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeChildTaskAttachmentService = employeeChildTaskAttachmentService;
            _hostingEnvironment = hostingEnvironment;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeTaskUserSerivce = employeeTaskUserSerivce;
            _employeeChildTaskTimeRecordService = employeeChildTaskTimeRecordService;
            _employeeChildTaskUserService = employeeChildTaskUserService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _employeeSubTaskAttachmentService = employeeSubTaskAttachmentService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeSubTaskTimeRecordService = employeeSubTaskTimeRecordService;
            _employeeSubTaskUserService = employeeSubTaskUserService;
            _employeeTaskAttachmentService = employeeTaskAttachmentService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateContractResponse>> Add([FromBody] AddUpdateContractRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            AddUpdateContractResponse addUpdateContractResponseObj = new AddUpdateContractResponse();

            Contract contractObj = _mapper.Map<Contract>(Model);
            if (contractObj != null)
            {
                if (Model.ClientId != null)
                {
                    bool IsExist = _contractService.IsExistOrNot(contractObj.Name, Model.ClientId.Value);
                    if (IsExist)
                    {
                        return new OperationResult<AddUpdateContractResponse>(false, System.Net.HttpStatusCode.OK, "Contract already exist");
                    }
                }
            }
            contractObj.CreatedBy = UserId;

            //invoice interval
            if (Model.InvoiceIntervalId != null && Model.Interval != null)
            {
                //check for user id tenant admin or not
                var checkUser = _userService.GetUserById(UserId);
                if (checkUser.RoleId != null)
                {
                    var roleObj = _roleService.GetRoleById(checkUser.RoleId.Value);
                    if (roleObj.RoleName == "TenantAdmin")
                    {
                        var invoiceIntervalObj = _invoiceIntervalService.GetById(Model.InvoiceIntervalId.Value);
                        if (invoiceIntervalObj.Interval != Model.Interval)
                        {
                            InvoiceInterval intervalObj = new InvoiceInterval();
                            intervalObj.Name = invoiceIntervalObj.Name;
                            intervalObj.Interval = Model.Interval;
                            intervalObj.CreatedBy = UserId;
                            var AddUpdateInvoiceInterval = await _invoiceIntervalService.CheckInsertOrUpdate(intervalObj, TenantId);
                            contractObj.InvoiceIntervalId = AddUpdateInvoiceInterval.Id;
                        }
                    }
                }
            }
            var InsetedContractObj = await _contractService.CheckInsertOrUpdate(contractObj);
            if (InsetedContractObj != null)
            {
                Model.Id = InsetedContractObj.Id;

                ContractActivity contractActivityObj = new ContractActivity();
                contractActivityObj.ContractId = Model.Id;
                contractActivityObj.ClientId = InsetedContractObj.ClientId;
                contractActivityObj.Activity = Enums.ContractActivityEnum.Contract_created.ToString().Replace("_", " ");
                var AddUpdateContractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);
            }

            addUpdateContractResponseObj = _mapper.Map<AddUpdateContractResponse>(InsetedContractObj);

            foreach (var ServiceArticle in Model.ServiceArticles)
            {
                var contractArticle = _mapper.Map<ContractArticle>(ServiceArticle);

                //ContractArticle contractArticle = new ContractArticle();
                contractArticle.ContractId = Model.Id;
                // contractArticle.ServiceArticleId = contractArticleObj.ServiceArticleId;
                // contractArticle.IsContractUnitPrice = contractArticleObj.IsContractUnitPrice;
                var InsertedContractArticle = await _contractArticleService.CheckInsertOrUpdate(contractArticle);
                if (InsertedContractArticle != null)
                {
                    ContractActivity ContractArticleActivityObj = new ContractActivity();
                    ContractArticleActivityObj.ContractId = Model.Id;
                    ContractArticleActivityObj.ClientId = InsetedContractObj.ClientId;
                    ContractArticleActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_created.ToString().Replace("_", " ");
                    var AddUpdateSubscriptionActivity = await _contractActivityService.CheckInsertOrUpdate(ContractArticleActivityObj);
                }
                // var contractArticleObj = _contractArticleService.GetById(InsertedContractArticle.Id);
                // if (contractArticleObj != null)
                // {
                //     if (contractArticleObj.ContractId == addUpdateContractResponseObj.Id)
                //     {
                //         addUpdateContractResponseObj.CurrencyName = contractArticleObj?.Contract?.Currency?.Name;
                //     }
                //     ContractServiceArticleResponse articleResponse = new ContractServiceArticleResponse();
                //     articleResponse = _mapper.Map<ContractServiceArticleResponse>(contractArticleObj?.ServiceArticle);
                //     articleResponse.CurrencyName = contractArticleObj?.ServiceArticle?.Currency?.Name;
                //     addUpdateContractResponseObj.Subscriptions.Add(articleResponse);
                // }

            }
            return new OperationResult<AddUpdateContractResponse>(true, System.Net.HttpStatusCode.OK, "", addUpdateContractResponseObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<AddUpdateContractResponse>> Update([FromBody] AddUpdateContractRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            AddUpdateContractResponse addUpdateContractResponseObj = new AddUpdateContractResponse();

            Contract contractObj = _mapper.Map<Contract>(Model);
            contractObj.UpdatedBy = UserId;

            //invoice interval
            if (Model.InvoiceIntervalId != null && Model.Interval != null)
            {
                //check for user id tenant admin or not
                var checkUser = _userService.GetUserById(UserId);
                if (checkUser.RoleId != null)
                {
                    var roleObj = _roleService.GetRoleById(checkUser.RoleId.Value);
                    if (roleObj.RoleName == "TenantAdmin")
                    {
                        var invoiceIntervalObj = _invoiceIntervalService.GetById(Model.InvoiceIntervalId.Value);
                        if (invoiceIntervalObj.Interval != Model.Interval)
                        {
                            InvoiceInterval intervalObj = new InvoiceInterval();
                            intervalObj.Name = invoiceIntervalObj.Name;
                            intervalObj.Interval = Model.Interval;
                            intervalObj.CreatedBy = UserId;
                            var AddUpdateInvoiceInterval = await _invoiceIntervalService.CheckInsertOrUpdate(intervalObj, TenantId);
                            contractObj.InvoiceIntervalId = AddUpdateInvoiceInterval.Id;
                        }
                    }
                }
            }

            var InsetedContractObj = await _contractService.CheckInsertOrUpdate(contractObj);
            if (InsetedContractObj != null)
            {
                Model.Id = InsetedContractObj.Id;

                ContractActivity contractActivityObj = new ContractActivity();
                contractActivityObj.ContractId = Model.Id;
                contractActivityObj.ClientId = InsetedContractObj.ClientId;
                contractActivityObj.Activity = Enums.ContractActivityEnum.Contract_updated.ToString().Replace("_", " ");
                var AddUpdateContractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);
            }

            addUpdateContractResponseObj = _mapper.Map<AddUpdateContractResponse>(InsetedContractObj);

            //var ContractArticleList = _contractArticleService.GetByContract(Model.Id.Value).Select(t => t.ServiceArticleId).ToList();

            if (InsetedContractObj != null && Model.ServiceArticles != null && Model.ServiceArticles.Count > 0)
            {
                var contractArticleList = _contractArticleService.GetByContract(Model.Id.Value);
                foreach (var item in contractArticleList)
                {
                    var requestModelId = Model.ServiceArticles.Where(t => t.Id == item.Id).Select(t => t.Id).FirstOrDefault();
                    if (item.Id == requestModelId)
                    {
                        var contractArticleModelObj = Model.ServiceArticles.Where(t => t.Id == item.Id).FirstOrDefault();
                        var contractArticleObj = _mapper.Map<ContractArticle>(contractArticleModelObj);
                        if (contractArticleObj != null)
                        {
                            contractArticleObj.ContractId = InsetedContractObj.Id;
                            // if (contractArticleObj.Id == 0)
                            // {
                            //     clientEmailObj.CreatedBy = UserId;
                            // }
                        }
                        var AddUpdateContractArticle = await _contractArticleService.CheckInsertOrUpdate(contractArticleObj);
                    }
                    else
                    {
                        var DeleteContractArticle = await _contractArticleService.DeletebyId(item.Id);
                    }
                }
                //Console.WriteLine(requestModel.Emails);
                foreach (var itememail in Model.ServiceArticles.Where(t => t.Id == null || t.Id == 0).ToList())
                {
                    var ContractArticleObj = _mapper.Map<ContractArticle>(itememail);
                    if (ContractArticleObj != null)
                    {
                        ContractArticleObj.ContractId = InsetedContractObj.Id;
                    }
                    var AddUpdateContractArticle = await _contractArticleService.CheckInsertOrUpdate(ContractArticleObj);
                }
            }

            // if (Model.ServiceArticles != null && Model.ServiceArticles.Count > 0)
            // {
            //     //var contractArticleList = await _contractArticleService.DeleteByContract(Model.Id.Value);
            //     foreach (var ServiceArticle in Model.ServiceArticles)
            //     {
            //         var contractArticle = _mapper.Map<ContractArticle>(ServiceArticle);
            //         contractArticle.ContractId = Model.Id;
            //         var InsertedContractArticle = await _contractArticleService.CheckInsertOrUpdate(contractArticle);
            //         if (InsertedContractArticle != null)
            //         {
            //             ContractActivity ContractArticleActivityObj = new ContractActivity();
            //             ContractArticleActivityObj.ContractId = Model.Id;
            //             ContractArticleActivityObj.ClientId = InsetedContractObj.ClientId;
            //             if (contractArticle.Id == 0)
            //             {
            //                 ContractArticleActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_created.ToString().Replace("_", " ");
            //             }
            //             else
            //             {
            //                 ContractArticleActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_updated.ToString().Replace("_", " ");
            //             }
            //             var AddUpdateSubscriptionActivity = await _contractActivityService.CheckInsertOrUpdate(ContractArticleActivityObj);
            //         }
            //         // var contractArticle = _mapper.Map<ContractArticle>(ServiceArticle);

            //         // //ContractArticle contractArticle = new ContractArticle();
            //         // contractArticle.ContractId = Model.Id;

            //         // if (ContractArticleList.Contains(contractArticle.ServiceArticleId))
            //         // {
            //         //     var InsertedContractSubscription = await _contractArticleService.CheckInsertOrUpdate(contractArticle);
            //         //     if (InsertedContractSubscription != null)
            //         //     {
            //         //         contractArticle.Id = InsertedContractSubscription.Id;

            //         //         ContractActivity contractSubscriptionActivityObj = new ContractActivity();
            //         //         contractSubscriptionActivityObj.ContractId = Model.Id;
            //         //         contractSubscriptionActivityObj.ClientId = InsetedContractObj.ClientId;
            //         //         contractSubscriptionActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_updated.ToString().Replace("_", " ");
            //         //         var AddUpdateSubscriptionActivity = await _contractActivityService.CheckInsertOrUpdate(contractSubscriptionActivityObj);
            //         //     }
            //         // }
            //         // else
            //         // {
            //         //     if (contractArticle != null && contractArticle.ServiceArticleId != null)
            //         //     {
            //         //         await _contractArticleService.Delete(Model.Id.Value, contractArticle.ServiceArticleId.Value);

            //         //         ContractActivity ActivityObj = new ContractActivity();
            //         //         ActivityObj.ContractId = Model.Id.Value;
            //         //         ActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_updated.ToString().Replace("_", " ");
            //         //         var AddUpdateActivity = await _contractActivityService.CheckInsertOrUpdate(ActivityObj);
            //         //     }

            //         // }
            //         var contractSubscriptionObj = _contractArticleService.GetById(InsertedContractArticle.Id);
            //         if (contractSubscriptionObj != null)
            //         {
            //             if (contractSubscriptionObj.ContractId == addUpdateContractResponseObj.Id)
            //             {
            //                 addUpdateContractResponseObj.CurrencyName = contractSubscriptionObj?.Contract?.Currency?.Name;
            //             }
            //             ContractServiceArticleResponse articleResponse = new ContractServiceArticleResponse();
            //             articleResponse = _mapper.Map<ContractServiceArticleResponse>(contractSubscriptionObj?.ServiceArticle);
            //             articleResponse.CurrencyName = contractSubscriptionObj?.ServiceArticle?.Currency?.Name;
            //             addUpdateContractResponseObj.Subscriptions.Add(articleResponse);
            //         }

            //     }
            // }
            return new OperationResult<AddUpdateContractResponse>(true, System.Net.HttpStatusCode.OK, "", addUpdateContractResponseObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<ContractListResponse>>> List([FromBody] ContractListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<ContractListResponse> contractListResponses = new List<ContractListResponse>();

            List<Contract> contractList = _contractService.GetByClient(requestModel.ClientId);
            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            foreach (var contractObj in contractList)
            {
                if (contractObj != null)
                {
                    ContractListResponse contractListObj = new ContractListResponse();
                    contractListObj.Id = contractObj.Id;
                    contractListObj.ContractType = contractObj?.ContractType?.Name;
                    contractListObj.StartDate = contractObj.StartDate;
                    contractListObj.EndDate = contractObj.EndDate;
                    contractListObj.Status = contractObj?.Status?.Name;
                    contractListObj.BillingPeriod = contractObj?.InvoiceInterval?.Name;
                    contractListObj.ClientId = contractObj.ClientId;
                    //next invoice date

                    var contractInvoiceList = _contractInvoiceService.GetAllByContract(contractObj.Id);
                    DateTime? StartDate = null;
                    DateTime? EndDate = null;
                    if (contractInvoiceList != null && contractInvoiceList.Count > 0)
                    {
                        var ClientInvoiceIds = contractInvoiceList.Select(t => t.ClientInvoiceId.Value).ToList();//get clientinvoice id fron contract invoice
                        var clientInvoiceList = _clientInvoiceService.GetListByIdList(ClientInvoiceIds).OrderBy(t => t.EndDate).ToList();
                        if (clientInvoiceList != null && clientInvoiceList.Count() > 0)
                        {
                            var ClientInvoiceLastRecord = clientInvoiceList.LastOrDefault(); //last record of client invoice
                            if (ClientInvoiceLastRecord != null && ClientInvoiceLastRecord.EndDate != null)
                            {
                                StartDate = ClientInvoiceLastRecord.EndDate.Value.AddDays(1).AddHours(0).AddMinutes(0).AddSeconds(0);
                                //add invoice interval from contract in start date and get enddate
                                if (contractObj.InvoiceInterval != null && contractObj.InvoiceInterval.Interval != null)
                                {
                                    EndDate = StartDate.Value.AddDays(contractObj.InvoiceInterval.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
                                }
                            }
                        }
                        else
                        {
                            if (contractObj.IsBillingFromStartDate)
                            {
                                StartDate = contractObj.StartDate;
                            }
                            if (StartDate != null)
                            {
                                StartDate = StartDate.Value.AddHours(0).AddMinutes(0).AddSeconds(0);
                                if (contractObj.InvoiceInterval != null && contractObj.InvoiceInterval.Interval != null)
                                {
                                    EndDate = StartDate.Value.AddDays(contractObj.InvoiceInterval.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (contractObj.IsBillingFromStartDate)
                        {
                            StartDate = contractObj.StartDate;
                        }
                        if (StartDate != null)
                        {
                            StartDate = StartDate.Value.AddHours(0).AddMinutes(0).AddSeconds(0);
                            if (contractObj.InvoiceInterval != null && contractObj.InvoiceInterval.Interval != null)
                            {
                                EndDate = StartDate.Value.AddDays(contractObj.InvoiceInterval.Interval.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
                            }
                        }
                    }
                    contractListObj.NextInvoiceDate = EndDate;
                    contractListResponses.Add(contractListObj);
                }
            }
            int totalCount = 0;
            totalCount = contractListResponses.Count();
            if (!string.IsNullOrEmpty(requestModel.SearchString))
            {
                contractListResponses = contractListResponses.Where(t => (!string.IsNullOrEmpty(t.ContractType) && t.ContractType.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.Status) && t.Status.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.BillingPeriod) && t.BillingPeriod.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                contractListResponses = contractListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            else
            {
                contractListResponses = contractListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            return new OperationResult<List<ContractListResponse>>(true, System.Net.HttpStatusCode.OK, "", contractListResponses, totalCount);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<ContractDetailResponse>> Detail(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            ContractDetailResponse contractDetailResponseObj = new ContractDetailResponse();

            Contract ContractObj = _contractService.GetById(Id);
            if (ContractObj != null)
            {
                contractDetailResponseObj = _mapper.Map<ContractDetailResponse>(ContractObj);
                contractDetailResponseObj.StatusName = ContractObj.Status?.Name;
                contractDetailResponseObj.InvoiceInterval = ContractObj.InvoiceInterval?.Name;
                contractDetailResponseObj.Interval = ContractObj.InvoiceInterval?.Interval;
                contractDetailResponseObj.CurrencyName = ContractObj.Currency?.Symbol + "" + ContractObj.Currency?.Code; ;
                contractDetailResponseObj.ContractTypeName = ContractObj.ContractType?.Name;
                contractDetailResponseObj.AllowedHours = ContractObj.AllowedHours;
                var contractArticleList = _contractArticleService.GetByContract(ContractObj.Id);
                if (contractArticleList != null && contractArticleList.Count() > 0)
                {
                    foreach (var articleObj in contractArticleList)
                    {
                        ContractDetailServiceArticleResponse serviceArticleResponseObj = new ContractDetailServiceArticleResponse();
                        serviceArticleResponseObj.Id = articleObj.Id;
                        serviceArticleResponseObj.ServiceArticleId = articleObj.ServiceArticle.Id;
                        serviceArticleResponseObj.Name = articleObj.ServiceArticle.Name;
                        serviceArticleResponseObj.Description = articleObj.ServiceArticle.Description;
                        serviceArticleResponseObj.StartDate = articleObj.StartDate;
                        serviceArticleResponseObj.EndDate = articleObj.EndDate;
                        serviceArticleResponseObj.IsBillable = articleObj.IsBillable;
                        serviceArticleResponseObj.IsContractUnitPrice = articleObj.IsContractUnitPrice;
                        contractDetailResponseObj.ServiceArticles.Add(serviceArticleResponseObj);
                    }
                }
                // contractDetailResponseObj.ServiceArticles = contractSubscriptionList;
            }

            return new OperationResult<ContractDetailResponse>(true, System.Net.HttpStatusCode.OK, "", contractDetailResponseObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (Id != null && Id > 0)
            {
                var contractObj = _contractService.GetById(Id);
                //contract article
                var contractArticleList = await _contractArticleService.DeleteByContract(Id);
                if (contractArticleList != null && contractArticleList.Count > 0)
                {
                    ContractActivity contractSubscriptionActivityObj = new ContractActivity();
                    contractSubscriptionActivityObj.ContractId = Id;
                    contractSubscriptionActivityObj.ClientId = contractObj?.ClientId;
                    contractSubscriptionActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_removed.ToString().Replace("_", " ");
                    var AddUpdateActivity = await _contractActivityService.CheckInsertOrUpdate(contractSubscriptionActivityObj);
                }

                //contract invoice
                var contractInvoiceList = _contractInvoiceService.GetAllByContract(Id);
                if (contractInvoiceList != null && contractInvoiceList.Count > 0)
                {
                    foreach (var contractInvoice in contractInvoiceList)
                    {
                        if (contractInvoice != null && contractInvoice.ClientInvoiceId != null)
                        {
                            var contractInvoiceObj = await _contractInvoiceService.DeleteContractInvoice(contractInvoice.Id);
                            if (contractInvoiceObj != null)
                            {
                                ContractActivity contractInvoiceActivityObj = new ContractActivity();
                                contractInvoiceActivityObj.ContractId = contractInvoice.ContractId;
                                contractInvoiceActivityObj.ClientId = contractObj?.ClientId;
                                contractInvoiceActivityObj.Activity = Enums.ContractActivityEnum.Contract_invoice_removed.ToString().Replace("_", " ");
                                var AddUpdateActivity = await _contractActivityService.CheckInsertOrUpdate(contractInvoiceActivityObj);
                            }
                            var clientInvoiceObj = await _clientInvoiceService.DeleteClientInvoice(contractInvoice.ClientInvoiceId.Value);
                        }
                    }
                }
                //contract invoice

                //start for ProjectContract
                var projectContractList = _projectContractService.GetByContractId(Id);
                if (projectContractList != null && projectContractList.Count > 0)
                {
                    foreach (var item in projectContractList)
                    {
                        if (item.ProjectId != null)
                        {
                            var tasks = _employeeProjectTaskService.GetAllTaskByProjectId(item.ProjectId.Value);
                            if (tasks != null && tasks.Count > 0)
                            {
                                return new OperationResult(true, System.Net.HttpStatusCode.OK, "This contract has multiple projects and tasks", Id);
                            }
                        }
                    }
                    return new OperationResult(true, System.Net.HttpStatusCode.OK, "This contract has multiple projects and tasks", Id);
                }
                // var projectContractList = await _projectContractService.DeleteByContract(Id);
                // if (projectContractList != null && projectContractList.Count > 0)
                // {
                //     //contract activity
                //     ContractActivity contractActivityObj = new ContractActivity();
                //     contractActivityObj.ContractId = Id;
                //     contractActivityObj.ClientId = contractObj.ClientId;
                //     contractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_removed_from_contract.ToString().Replace("_", " ");
                //     var contractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);

                //     // foreach (var item in projectContractList)
                //     // {
                //     //     if (item != null && item.ProjectId != null)
                //     //     {
                //     //         //var tasks = _employeeProjectTaskService.GetAllTaskByProjectId(item.ProjectId.Value);

                //     //         // if (requestmodel.IsKeepTasks == true)
                //     //         // {
                //     //         // if (tasks != null && tasks.Count() > 0)
                //     //         // {
                //     //         //     foreach (var taskObj in tasks)
                //     //         //     {
                //     //         //         var employeeProjectTaskObj = await _employeeProjectTaskService.DeleteByTaskId(taskObj.Id);

                //     //         //         EmployeeTaskActivity EmployeeTaskActivityObj = new EmployeeTaskActivity();
                //     //         //         EmployeeTaskActivityObj.EmployeeTaskId = taskObj.EmployeeTaskId;
                //     //         //         EmployeeTaskActivityObj.UserId = UserId;
                //     //         //         EmployeeTaskActivityObj.Activity = "Removed this task from Project";
                //     //         //         var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(EmployeeTaskActivityObj);
                //     //         //     }
                //     //         // }
                //     //         // }
                //     //         // else
                //     //         // {
                //     //         //     if (tasks != null && tasks.Count() > 0)
                //     //         //     {
                //     //         //         foreach (var taskObj in tasks)
                //     //         //         {
                //     //         //             var employeeTaskId = taskObj.Id;

                //     //         //             var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(employeeTaskId);

                //     //         //             if (subTasks != null && subTasks.Count() > 0)
                //     //         //             {
                //     //         //                 foreach (var subTask in subTasks)
                //     //         //                 {
                //     //         //                     var subTaskId = subTask.Id;

                //     //         //                     var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);

                //     //         //                     if (childTasks != null && childTasks.Count() > 0)
                //     //         //                     {
                //     //         //                         foreach (var childItem in childTasks)
                //     //         //                         {
                //     //         //                             var childTaskId = childItem.Id;

                //     //         //                             var childDocuments = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                //     //         //                             // Remove child task documents from folder
                //     //         //                             if (childDocuments != null && childDocuments.Count() > 0)
                //     //         //                             {
                //     //         //                                 foreach (var childTaskDoc in childDocuments)
                //     //         //                                 {

                //     //         //                                     //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                //     //         //                                     var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                //     //         //                                     var filePath = dirPath + "\\" + childTaskDoc.Name;

                //     //         //                                     if (System.IO.File.Exists(filePath))
                //     //         //                                     {
                //     //         //                                         System.IO.File.Delete(Path.Combine(filePath));
                //     //         //                                     }
                //     //         //                                 }
                //     //         //                             }

                //     //         //                             var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                //     //         //                             var childTimeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(childTaskId);

                //     //         //                             var childTaskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(childTaskId);

                //     //         //                             EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                //     //         //                             employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
                //     //         //                             employeeChildTaskActivityObj.UserId = UserId;
                //     //         //                             employeeChildTaskActivityObj.Activity = "Removed the task";
                //     //         //                             var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

                //     //         //                             var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

                //     //         //                             var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);
                //     //         //                         }
                //     //         //                     }

                //     //         //                     var subDocuments = await _employeeSubTaskAttachmentService.DeleteAttachmentByEmployeeSubTaskId(subTaskId);

                //     //         //                     // Remove sub task documents from folder
                //     //         //                     if (subDocuments != null && subDocuments.Count() > 0)
                //     //         //                     {
                //     //         //                         foreach (var subTaskDoc in subDocuments)
                //     //         //                         {

                //     //         //                             //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                //     //         //                             var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
                //     //         //                             var filePath = dirPath + "\\" + subTaskDoc.Name;

                //     //         //                             if (System.IO.File.Exists(filePath))
                //     //         //                             {
                //     //         //                                 System.IO.File.Delete(Path.Combine(filePath));
                //     //         //                             }
                //     //         //                         }
                //     //         //                     }

                //     //         //                     var subComments = await _employeeSubTaskCommentService.DeleteCommentByEmployeeSubTaskId(subTaskId);

                //     //         //                     var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                //     //         //                     var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(subTaskId);

                //     //         //                     EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                //     //         //                     employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskId;
                //     //         //                     employeeSubTaskActivityObj.UserId = UserId;
                //     //         //                     employeeSubTaskActivityObj.Activity = "Removed the task";
                //     //         //                     var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                //     //         //                     var subTaskActivities = await _employeeSubTaskActivityService.DeleteByEmployeeSubTaskId(subTaskId);

                //     //         //                     var subTaskToDelete = await _employeeSubTaskService.Delete(subTaskId);
                //     //         //                 }
                //     //         //             }

                //     //         //             var documents = await _employeeTaskAttachmentService.DeleteAttachmentByTaskId(employeeTaskId);

                //     //         //             // Remove task documents from folder
                //     //         //             if (documents != null && documents.Count() > 0)
                //     //         //             {
                //     //         //                 foreach (var taskDoc in documents)
                //     //         //                 {

                //     //         //                     //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
                //     //         //                     var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                //     //         //                     var filePath = dirPath + "\\" + taskDoc.Name;

                //     //         //                     if (System.IO.File.Exists(filePath))
                //     //         //                     {
                //     //         //                         System.IO.File.Delete(Path.Combine(filePath));
                //     //         //                     }
                //     //         //                 }
                //     //         //             }

                //     //         //             var comments = await _employeeTaskCommentService.DeleteCommentByEmployeeTaskId(employeeTaskId);

                //     //         //             var timeRecords = await _employeeTaskTimeRecordService.DeleteTimeRecordByTaskId(employeeTaskId);

                //     //         //             //for EmployeeClientTask
                //     //         //             var employeeClientTaskObj = await _employeeClientTaskService.DeleteByTaskId(employeeTaskId);
                //     //         //             if (employeeClientTaskObj != null)
                //     //         //             {
                //     //         //                 EmployeeTaskActivity clientTaskActivityObj = new EmployeeTaskActivity();
                //     //         //                 clientTaskActivityObj.EmployeeTaskId = employeeTaskId;
                //     //         //                 clientTaskActivityObj.UserId = UserId;
                //     //         //                 clientTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Unassign_client_from_task.ToString().Replace("_", " ");
                //     //         //                 var AddUpdateClientTask = await _employeeTaskActivityService.CheckInsertOrUpdate(clientTaskActivityObj);
                //     //         //             }

                //     //         //             var taskUsers = await _employeeTaskUserService.DeleteByEmployeeTaskId(employeeTaskId);
                //     //         //             EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                //     //         //             employeeTaskActivityObj.EmployeeTaskId = employeeTaskId;
                //     //         //             employeeTaskActivityObj.UserId = UserId;
                //     //         //             employeeTaskActivityObj.Activity = "Removed this task";
                //     //         //             var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

                //     //         //             //var taskActivities = await _employeeTaskActivityService.DeleteByEmployeeTaskId(employeeTaskId);

                //     //         //             var taskToDelete = await _employeeTaskService.Delete(employeeTaskId);
                //     //         //         }
                //     //         //     }
                //     //         // }

                //     //         var employeeProjectObj = await _employeeProjectService.DeleteEmployeeProject(item.ProjectId.Value);
                //     //         if (employeeProjectObj != null)
                //     //         {
                //     //             EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                //     //             employeeProjectActivityObj.ProjectId = item.ProjectId;
                //     //             employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_Removed.ToString().Replace("_", " ");
                //     //             employeeProjectActivityObj.UserId = UserId;
                //     //             var projectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                //     //         }
                //     //     }
                //     // }
                // }
                //end for ProjectContract

                //start contract asset
                var contractAssetList = await _contractAssetService.DeleteByContractId(Id);
                //end contract asset
                var removedContract = await _contractService.DeleteContract(Id, UserId);
                if (removedContract != null)
                {
                    ContractActivity contractActivityObj = new ContractActivity();
                    contractActivityObj.ContractId = Id;
                    contractActivityObj.ClientId = removedContract.ClientId;
                    contractActivityObj.Activity = Enums.ContractActivityEnum.Contract_removed.ToString().Replace("_", " ");
                    var AddUpdateContractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);
                }

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{ContractId}")]
        public async Task<OperationResult<List<ContractActivityHistoryResponse>>> History(long ContractId)
        {
            List<ContractActivityHistoryResponse> contractActivityHistoryResponseList = new List<ContractActivityHistoryResponse>();
            var AllClient = _clientService.GetAll();
            var contractActivityList = _contractActivityService.GetAllByContractId(ContractId);
            contractActivityHistoryResponseList = _mapper.Map<List<ContractActivityHistoryResponse>>(contractActivityList);
            if (contractActivityHistoryResponseList != null && contractActivityHistoryResponseList.Count > 0)
            {
                foreach (var item in contractActivityHistoryResponseList)
                {
                    var clientObj = AllClient.Where(t => t.Id == item.ClientId).FirstOrDefault();
                    if (clientObj != null)
                    {
                        item.FirstName = clientObj.FirstName;
                        item.LastName = clientObj.LastName;
                        item.Name = clientObj.Name;
                    }
                }
            }
            return new OperationResult<List<ContractActivityHistoryResponse>>(true, System.Net.HttpStatusCode.OK, "", contractActivityHistoryResponseList);
        }

        //for client drop down
        [SwaggerOperation(Description = "Use this api for getting contract list for drop down")]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{ClientId}")]
        public async Task<OperationResult<List<ContractDropdownListResponse>>> DropdownList(long ClientId)
        {
            // ClaimsPrincipal user = this.User as ClaimsPrincipal;
            // TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            //List<ContractDropdownListResponse> dropdownListResponses = new List<ContractDropdownListResponse>();
            var contractist = _contractService.GetByClient(ClientId);
            var dropdownListResponses = _mapper.Map<List<ContractDropdownListResponse>>(contractist);

            return new OperationResult<List<ContractDropdownListResponse>>(true, System.Net.HttpStatusCode.OK, "", dropdownListResponses);
        }
    }
}