using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using matcrm.service.Services;
using matcrm.service.Common;
using matcrm.data.Models.Response;
using matcrm.data.Models.Request;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Tables;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ServiceArticlePriceController : Controller
    {
        private readonly IServiceArticlePriceService _serviceArticlePriceService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ServiceArticlePriceController(IServiceArticlePriceService serviceArticlePriceService,
        IMapper mapper)
        {
            _serviceArticlePriceService = serviceArticlePriceService;
            _mapper = mapper;
        }
        //All api for this is userd for client service article..so show side of contract tab
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ServiceArticlePriceAddResponse>> Add([FromBody] ServiceArticlePriceAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ServiceArticlePrice>(requestmodel);
            if (model.Id == null || model.Id == 0)
            {
                model.LoggedInUserId = UserId;
            }
            var AddUpdateObj = await _serviceArticlePriceService.CheckInsertOrUpdate(model);
            ServiceArticlePriceAddResponse serviceArticlePriceAddResponseObj = new ServiceArticlePriceAddResponse();
            serviceArticlePriceAddResponseObj = _mapper.Map<ServiceArticlePriceAddResponse>(AddUpdateObj);
            return new OperationResult<ServiceArticlePriceAddResponse>(true, System.Net.HttpStatusCode.OK, "Price added successfully", serviceArticlePriceAddResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<ServiceArticlePriceAddResponse>> Update([FromBody] ServiceArticlePriceAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ServiceArticlePrice>(requestmodel);
            if (model.Id == null || model.Id == 0)
            {
                model.LoggedInUserId = UserId;
            }
            var AddUpdateObj = await _serviceArticlePriceService.CheckInsertOrUpdate(model);
            ServiceArticlePriceAddResponse serviceArticlePriceAddResponseObj = new ServiceArticlePriceAddResponse();
            serviceArticlePriceAddResponseObj = _mapper.Map<ServiceArticlePriceAddResponse>(AddUpdateObj);
            return new OperationResult<ServiceArticlePriceAddResponse>(true, System.Net.HttpStatusCode.OK, "Price updated successfully", serviceArticlePriceAddResponseObj);
        }

        [HttpPost]
        public async Task<OperationResult<List<ServiceArticlePriceListResponse>>> List([FromBody] ServiceArticlePriceListRequest requestModel)
        {
            var PriceList = _serviceArticlePriceService.GetAllByClientId(requestModel.ClientId);
            List<ServiceArticlePriceListResponse> ResponsePriceList = new List<ServiceArticlePriceListResponse>();
            if (PriceList != null && PriceList.Count > 0)
            {
                foreach (var item in PriceList)
                {
                    ServiceArticlePriceListResponse ResponseObj = new ServiceArticlePriceListResponse();
                    ResponseObj = _mapper.Map<ServiceArticlePriceListResponse>(item);
                    if (item.ServiceArticleId != null)
                    {
                        ResponseObj.ServiceArticleName = item.ServiceArticle.Name;                       
                    }                    
                    ResponsePriceList.Add(ResponseObj);
                }
            }
            int totalCount = 0;
            totalCount = ResponsePriceList.Count();
            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            if (!string.IsNullOrEmpty(requestModel.SearchString))
            {
                ResponsePriceList = ResponsePriceList.Where(t => (!string.IsNullOrEmpty(t.ServiceArticleName) && t.ServiceArticleName.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                ResponsePriceList = ResponsePriceList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            else
            {
                ResponsePriceList = ResponsePriceList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            ResponsePriceList = ShortByColumn(requestModel.ShortColumnName, requestModel.SortType, ResponsePriceList);
            return new OperationResult<List<ServiceArticlePriceListResponse>>(true, System.Net.HttpStatusCode.OK, "", ResponsePriceList, totalCount);
        }

        private List<ServiceArticlePriceListResponse> ShortByColumn(string ShortColumn, string ShortOrder, List<ServiceArticlePriceListResponse> PriceList)
        {
            List<ServiceArticlePriceListResponse> PriceListVMList = new List<ServiceArticlePriceListResponse>();
            PriceListVMList = PriceList;
            if (ShortColumn.ToLower() == "servicearticlename")
            {
                if (ShortOrder.ToLower() == "asc")
                {
                    PriceListVMList = PriceList.OrderBy(t => t.ServiceArticleName).ToList();
                }
                else
                {
                    PriceListVMList = PriceList.OrderByDescending(t => t.ServiceArticleName).ToList();
                }
            }
            else if (ShortColumn.ToLower() == "price")
            {
                if (ShortOrder.ToLower() == "asc")
                {
                    PriceListVMList = PriceList.OrderBy(t => t.Price).ToList();
                }
                else
                {
                    PriceListVMList = PriceList.OrderByDescending(t => t.Price).ToList();
                }
            }
            else if (ShortColumn.ToLower() == "startdate")
            {
                if (ShortOrder.ToLower() == "asc")
                {
                    PriceListVMList = PriceList.OrderBy(t => t.StartDate).ToList();
                }
                else
                {
                    PriceListVMList = PriceList.OrderByDescending(t => t.StartDate).ToList();
                }
            }
            else if (ShortColumn.ToLower() == "enddate")
            {
                if (ShortOrder.ToLower() == "asc")
                {
                    PriceListVMList = PriceList.OrderBy(t => t.EndDate).ToList();
                }
                else
                {
                    PriceListVMList = PriceList.OrderByDescending(t => t.EndDate).ToList();
                }
            }
            else
            {
                PriceListVMList = PriceList.OrderByDescending(t => t.Id).ToList();
            }
            return PriceListVMList;
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<ServiceArticlePriceDetailResponse>> Detail(long Id)
        {
            var PriceObj = _serviceArticlePriceService.GetById(Id);
            var ResponseObj = _mapper.Map<ServiceArticlePriceDetailResponse>(PriceObj);
            if (PriceObj.ServiceArticleId != null)
            {
                ResponseObj.ServiceArticleName = PriceObj.ServiceArticle.Name;                                
            }            
            return new OperationResult<ServiceArticlePriceDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            if (Id > 0)
            {
                var serviceArticlePriceObj = await _serviceArticlePriceService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }
    }
}