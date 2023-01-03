using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Tables;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ServiceArticleController : Controller
    {
        private IMapper _mapper;
        private readonly IServiceArticleService _serviceArticleService;
        private readonly IServiceArticleHourService _serviceArticleHourService;
        private readonly IServiceArticleCategoryService _serviceArticleCategoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IMateProjectTimeRecordService _mateProjectTimeRecordService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IEmployeeProjectActivityService _employeeProjectActivityService;
        private readonly IMateTaskTimeRecordService _mateTaskTimeRecordService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IContractArticleService _contractArticleService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IContractService _contractService;
        private readonly IContractActivityService _contractActivityService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;
        private int TenantId = 0;
        public ServiceArticleController(IMapper mapper,
        IServiceArticleService serviceArticleService,
        IServiceArticleHourService serviceArticleHourService,
        IServiceArticleCategoryService serviceArticleCategoryService,
        IMateProjectTimeRecordService mateProjectTimeRecordService,
        IMateTimeRecordService mateTimeRecordService,
        ICurrencyService currencyService,
        IEmployeeProjectActivityService employeeProjectActivityService,
        IMateTaskTimeRecordService mateTaskTimeRecordService,
        IEmployeeTaskActivityService employeeTaskActivityService,
        IContractArticleService contractArticleService,
        IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
        IContractService contractService,
        IContractActivityService contractActivityService,
        IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _mapper = mapper;
            _serviceArticleService = serviceArticleService;
            _serviceArticleHourService = serviceArticleHourService;
            _serviceArticleCategoryService = serviceArticleCategoryService;
            _currencyService = currencyService;
            _mateProjectTimeRecordService = mateProjectTimeRecordService;
            _mateTimeRecordService = mateTimeRecordService;
            _employeeProjectActivityService = employeeProjectActivityService;
            _mateTaskTimeRecordService = mateTaskTimeRecordService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _contractArticleService = contractArticleService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _contractService = contractService;
            _contractActivityService = contractActivityService;
            _hubContext = hubContext;
        }


        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ServiceArticleAddResponse>> Add([FromBody] ServiceArticleAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<ServiceArticle>(requestmodel);
            if (model.Id == null || model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            // model.TenantId = TenantId;
            var serviceArticleObj = await _serviceArticleService.CheckInsertOrUpdate(model);
            // if (serviceArticleObj != null && requestmodel.UnitPrice != null)
            // {
            //     ServiceArticleHour serviceArticleHourObj = new ServiceArticleHour();
            //     serviceArticleHourObj.UnitPrice = requestmodel.UnitPrice;
            //     serviceArticleHourObj.ServiceArticleId = serviceArticleObj.Id;
            //     var serviceArticleHourAddUpdateObj = await _serviceArticleHourService.CheckInsertOrUpdate(serviceArticleHourObj);
            // }
            ServiceArticleAddResponse serviceArticleAddResponseObj = new ServiceArticleAddResponse();
            serviceArticleAddResponseObj = _mapper.Map<ServiceArticleAddResponse>(serviceArticleObj);
            // if (requestmodel.CategoryId != null)
            // {
            //     var categoryObj = _serviceArticleCategoryService.GetById(requestmodel.CategoryId.Value);
            //     serviceArticleAddResponseObj.CategoryName = categoryObj.Name;
            // }
            if (requestmodel.CurrencyId != null)
            {
                var currencyObj = _currencyService.GetById(requestmodel.CurrencyId.Value);
                if (currencyObj != null)
                {
                    serviceArticleAddResponseObj.UnitPrice = currencyObj.Symbol + "" + requestmodel.UnitPrice;
                }
                else
                {
                    serviceArticleAddResponseObj.UnitPrice = Convert.ToString(requestmodel.UnitPrice);
                }
            }
            //serviceArticleAddResponseObj.UnitPrice = requestmodel.UnitPrice;
            return new OperationResult<ServiceArticleAddResponse>(true, System.Net.HttpStatusCode.OK, "Service article added successfully", serviceArticleAddResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<ServiceArticleAddResponse>> Update([FromBody] ServiceArticleAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<ServiceArticle>(requestmodel);

            // model.TenantId = TenantId;
            var serviceArticleObj = await _serviceArticleService.CheckInsertOrUpdate(model);
            // if (serviceArticleObj != null && requestmodel.UnitPrice != null)
            // {
            //     ServiceArticleHour serviceArticleHourObj = new ServiceArticleHour();
            //     serviceArticleHourObj = _serviceArticleHourService.GetByServiceArticleId(serviceArticleObj.Id);
            //     serviceArticleHourObj.Id = serviceArticleHourObj.Id;
            //     serviceArticleHourObj.UnitPrice = requestmodel.UnitPrice;
            //     serviceArticleHourObj.ServiceArticleId = serviceArticleObj.Id;
            //     var serviceArticleHourAddUpdateObj = await _serviceArticleHourService.CheckInsertOrUpdate(serviceArticleHourObj);
            // }
            ServiceArticleAddResponse serviceArticleAddResponseObj = new ServiceArticleAddResponse();
            serviceArticleAddResponseObj = _mapper.Map<ServiceArticleAddResponse>(serviceArticleObj);
            // if (requestmodel.CategoryId != null)
            // {
            //     var categoryObj = _serviceArticleCategoryService.GetById(requestmodel.CategoryId.Value);
            //     serviceArticleAddResponseObj.CategoryName = categoryObj.Name;
            // }
            if (requestmodel.CurrencyId != null)
            {
                var currencyObj = _currencyService.GetById(requestmodel.CurrencyId.Value);
                if (currencyObj != null)
                {
                    serviceArticleAddResponseObj.UnitPrice = currencyObj.Symbol + "" + requestmodel.UnitPrice;
                }
                else
                {
                    serviceArticleAddResponseObj.UnitPrice = Convert.ToString(requestmodel.UnitPrice);
                }
            }
            return new OperationResult<ServiceArticleAddResponse>(true, System.Net.HttpStatusCode.OK, "Service article updated successfully", serviceArticleAddResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<ServiceArticleListResponse>>> List([FromBody] CommonListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<ServiceArticleListResponse> serviceArticleListResponseList = new List<ServiceArticleListResponse>();
            var totalCount = 0;
            var serviceArticleList = _serviceArticleService.GetByTenant(TenantId);
            serviceArticleListResponseList = serviceArticleList.Where(t => t.CurrencyId != null).Select(t => new ServiceArticleListResponse
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                UnitPrice = t.UnitPrice + t.Currency.Symbol.ToString(),
                CurrencyId = t.CurrencyId.Value,
                CreatedBy = t.CreatedBy,
                CreatedOn = t.CreatedOn
            }).Take(requestModel.PageSize).Skip(requestModel.PageSize * (requestModel.PageNumber - 1)).ToList();

            serviceArticleListResponseList = ShortListByColumn(requestModel.ShortColumnName, requestModel.SortType, serviceArticleListResponseList);
            // serviceArticleListResponseList = _mapper.Map<List<ServiceArticleListResponse>>(serviceArticleList);
            // if (serviceArticleListResponseList != null && serviceArticleListResponseList.Count() > 0)
            // {
            //     foreach (var item in serviceArticleListResponseList)
            //     {
            //         if (item != null && item.CategoryId != null)
            //         {
            //             var categoryObj = serviceArticleList.Where(t => t.ServiceArticleCategory.Id == item.CategoryId).FirstOrDefault();
            //             if (categoryObj != null)
            //             {
            //                 item.CategoryName = categoryObj.ServiceArticleCategory.Name;
            //             }
            //             var serviceArticleHourObj = _serviceArticleHourService.GetByServiceArticleId(item.Id);
            //             if (serviceArticleHourObj != null)
            //             {
            //                 item.UnitPrice = Convert.ToString(serviceArticleHourObj.UnitPrice);
            //             }
            //             var currencyObj = serviceArticleList.Where(t => t.Currency.Id == item.CurrencyId).FirstOrDefault();
            //             if (currencyObj != null)
            //             {
            //                 item.UnitPrice = currencyObj.Currency.Symbol + "" + item.UnitPrice;
            //             }
            //         }
            //     }
            // }
            if (serviceArticleListResponseList != null)
            {
                totalCount = serviceArticleListResponseList.Count();
            }
            return new OperationResult<List<ServiceArticleListResponse>>(true, System.Net.HttpStatusCode.OK, "", serviceArticleListResponseList, totalCount);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<ServiceArticleDetailResponse>> Detail(int Id)
        {
            ServiceArticle serviceArticleObj = new ServiceArticle();

            ServiceArticleDetailResponse serviceArticleDetailResponseObj = new ServiceArticleDetailResponse();

            serviceArticleObj = _serviceArticleService.GetById(Id);
            var serviceArticleHourObj = _serviceArticleHourService.GetByServiceArticleId(Id);
            serviceArticleDetailResponseObj = _mapper.Map<ServiceArticleDetailResponse>(serviceArticleObj);

            if (serviceArticleHourObj != null && serviceArticleHourObj.UnitPrice != null)
            {
                serviceArticleDetailResponseObj.UnitPrice = serviceArticleHourObj.UnitPrice.Value;
            }

            return new OperationResult<ServiceArticleDetailResponse>(true, System.Net.HttpStatusCode.OK, "", serviceArticleDetailResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult> Remove([FromBody] ServiceArticleRemoveRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (model.Id != null && model.Id > 0)
            {
                //contract subscription
                var contractArticleList = _contractArticleService.GetByServiceArticle(model.Id);
                if (contractArticleList != null && contractArticleList.Count > 0)
                {
                    foreach (var article in contractArticleList)
                    {
                        if (article.ContractId != null)
                        {
                            var contractObj = _contractService.GetById(article.ContractId.Value);
                            var contractArticleObj = await _contractArticleService.DeletebyId(article.Id);
                            if (contractArticleObj != null)
                            {
                                ContractActivity contractArticleActivityObj = new ContractActivity();
                                contractArticleActivityObj.ContractId = article.ContractId;
                                contractArticleActivityObj.ClientId = contractObj.ClientId;
                                contractArticleActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_removed.ToString().Replace("_", " ");
                                var AddUpdateActivity = await _contractActivityService.CheckInsertOrUpdate(contractArticleActivityObj);
                            }
                        }
                    }
                }

                //for project time record by service article
                var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetTimeRecordByServiceArticleId(model.Id);
                if (mateProjectTimeRecordList != null && mateProjectTimeRecordList.Count() > 0)
                {
                    foreach (var projectTimeRecord in mateProjectTimeRecordList)
                    {
                        if (projectTimeRecord != null)
                        {
                            if (model.IsKeepTimeRecords == true)
                            {
                                var timeRecordObj = _mapper.Map<MateTimeRecord>(projectTimeRecord.MateTimeRecord);
                                timeRecordObj.ServiceArticleId = null;
                                var AddUpdateProject = await _mateTimeRecordService.CheckInsertOrUpdate(timeRecordObj);

                                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                                employeeProjectActivityObj.ProjectId = projectTimeRecord.ProjectId;
                                employeeProjectActivityObj.UserId = UserId;
                                employeeProjectActivityObj.Activity = "Removed project time record from service article";
                                var AddUpdateActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                                await _hubContext.Clients.All.OnProjectModuleEvent(projectTimeRecord.ProjectId, TenantId);
                            }
                            else
                            {
                                if (projectTimeRecord.MateTimeRecordId != null)
                                {
                                    var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(projectTimeRecord.MateTimeRecordId.Value);
                                    if (mateTimeRecordObj != null)
                                    {
                                        EmployeeProjectActivity projectActivityObj = new EmployeeProjectActivity();
                                        projectActivityObj.ProjectId = projectTimeRecord.ProjectId;
                                        projectActivityObj.UserId = UserId;
                                        projectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_time_record_removed.ToString().Replace("_", " ");
                                        var AddUpdateProjectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(projectActivityObj);
                                        await _hubContext.Clients.All.OnProjectModuleEvent(projectTimeRecord.ProjectId, TenantId);
                                    }
                                }
                            }
                        }
                    }
                }
                //for project time record by service article

                //for task time record by service article
                var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetTimeRecordByServiceArticleId(model.Id);
                if (mateTaskTimeRecordList != null && mateTaskTimeRecordList.Count() > 0)
                {
                    foreach (var taskTimeRecord in mateTaskTimeRecordList)
                    {
                        if (taskTimeRecord != null)
                        {
                            if (model.IsKeepTimeRecords == true)
                            {
                                var timeRecordObj = _mapper.Map<MateTimeRecord>(taskTimeRecord.MateTimeRecord);
                                timeRecordObj.ServiceArticleId = null;
                                var AddUpdateTask = await _mateTimeRecordService.CheckInsertOrUpdate(timeRecordObj);

                                EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                                employeeTaskActivityObj.EmployeeTaskId = taskTimeRecord.TaskId;
                                employeeTaskActivityObj.UserId = UserId;
                                employeeTaskActivityObj.Activity = "Removed task time record from service article";
                                var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                            }
                            else
                            {
                                if (taskTimeRecord.MateTimeRecordId != null)
                                {
                                    var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(taskTimeRecord.MateTimeRecordId.Value);
                                    if (mateTimeRecordObj != null)
                                    {
                                        EmployeeTaskActivity taskActivityObj = new EmployeeTaskActivity();
                                        taskActivityObj.EmployeeTaskId = taskTimeRecord.TaskId;
                                        taskActivityObj.UserId = UserId;
                                        taskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Task_time_record_removed.ToString().Replace("_", " ");
                                        var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(taskActivityObj);
                                    }
                                }
                            }
                            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(taskTimeRecord.TaskId, TenantId);
                        }
                    }
                }
                //for task time record by service article

                //Delete EmployeeTaskTimeRecord
                var employeeTaskTimeRecordList = await _employeeTaskTimeRecordService.DeleteTimeRecordByServiceArticleId(model.Id);

                var serviceArticleObj = await _serviceArticleService.DeleteServiceArticle(model.Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", model.Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", model.Id);
            }
        }

        private List<ServiceArticleListResponse> ShortListByColumn(string ShortColumn, string ShortOrder, List<ServiceArticleListResponse> ArticleList)
        {
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn.ToLower() == "description")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ArticleList = ArticleList.OrderBy(t => t.Description).ToList();
                    }
                    else
                    {
                        ArticleList = ArticleList.OrderByDescending(t => t.Description).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "unitPrice")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ArticleList = ArticleList.OrderBy(t => t.UnitPrice).ToList();
                    }
                    else
                    {
                        ArticleList = ArticleList.OrderByDescending(t => t.UnitPrice).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "name")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ArticleList = ArticleList.OrderBy(t => t.Name).ToList();
                    }
                    else
                    {
                        ArticleList = ArticleList.OrderByDescending(t => t.Name).ToList();
                    }
                }
                else
                {
                    ArticleList = ArticleList.OrderByDescending(t => t.CreatedOn).ToList();
                }
            }

            return ArticleList;
        }
    }
}