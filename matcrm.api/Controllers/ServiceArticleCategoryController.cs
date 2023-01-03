using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.service.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using matcrm.data.Models.Tables;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ServiceArticleCategoryController : Controller
    {
        private readonly IServiceArticleCategoryService _serviceArticleCategoryService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public ServiceArticleCategoryController(
            IServiceArticleCategoryService serviceArticleCategoryService,
            IMapper mapper)
        {
            _serviceArticleCategoryService = serviceArticleCategoryService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ServiceArticleCategoryAddResponse>> Add([FromBody] ServiceArticleCategoryAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<ServiceArticleCategory>(requestmodel);
            if (model.Id == null || model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            model.TenantId = TenantId;

            var serviceArticleCategoryObj = await _serviceArticleCategoryService.CheckInsertOrUpdate(model);
            ServiceArticleCategoryAddResponse serviceArticleCategoryAddResponseObj = new ServiceArticleCategoryAddResponse();
            serviceArticleCategoryAddResponseObj = _mapper.Map<ServiceArticleCategoryAddResponse>(serviceArticleCategoryObj);

            return new OperationResult<ServiceArticleCategoryAddResponse>(true, System.Net.HttpStatusCode.OK, "Service article category added successfully", serviceArticleCategoryAddResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<ServiceArticleCategoryAddResponse>> Update([FromBody] ServiceArticleCategoryAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<ServiceArticleCategory>(requestmodel);
            if (model.Id != null && model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;

            var serviceArticleCategoryObj = await _serviceArticleCategoryService.CheckInsertOrUpdate(model);
            ServiceArticleCategoryAddResponse serviceArticleCategoryAddResponseObj = new ServiceArticleCategoryAddResponse();
            serviceArticleCategoryAddResponseObj = _mapper.Map<ServiceArticleCategoryAddResponse>(serviceArticleCategoryObj);

            return new OperationResult<ServiceArticleCategoryAddResponse>(true, System.Net.HttpStatusCode.OK, "Service article category updated successfully", serviceArticleCategoryAddResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<ServiceArticleCategoryListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var serviceArticleCategoryList = _serviceArticleCategoryService.GetByTenant(TenantId);
            var serviceArticleCategoryListResponses = _mapper.Map<List<ServiceArticleCategoryListResponse>>(serviceArticleCategoryList);
            return new OperationResult<List<ServiceArticleCategoryListResponse>>(true, System.Net.HttpStatusCode.OK, "", serviceArticleCategoryListResponses);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<ServiceArticleCategoryDetailResponse>> Detail(int Id)
        {
            ServiceArticleCategory serviceArticleCategoryObj = new ServiceArticleCategory();
            ServiceArticleCategoryDetailResponse serviceArticleCategoryDetailResponseObj = new ServiceArticleCategoryDetailResponse();
            serviceArticleCategoryObj = _serviceArticleCategoryService.GetById(Id);
            serviceArticleCategoryDetailResponseObj = _mapper.Map<ServiceArticleCategoryDetailResponse>(serviceArticleCategoryObj);
            return new OperationResult<ServiceArticleCategoryDetailResponse>(true, System.Net.HttpStatusCode.OK, "", serviceArticleCategoryDetailResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null && Id > 0)
            {
                int deletedby = UserId;
                var serviceArticleCategoryObj = await _serviceArticleCategoryService.DeleteServiceArticleCategory(Id,deletedby);
                
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }
    }
}