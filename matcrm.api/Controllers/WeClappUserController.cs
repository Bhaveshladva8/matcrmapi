using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.ERP;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class WeClappUserController : Controller
    {
        private readonly IWeClappUserService _weClappUserService;
        private readonly IWeClappService _weClappService;
        private IMapper _mapper;
        private int UserId = 0;
        public WeClappUserController(
            IWeClappUserService weClappUserService,
            IWeClappService weClappService,
             IMapper mapper
        )
        {
            _weClappUserService = weClappUserService;
            _weClappService = weClappService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<WeClappUserResponse>> BasedOnUser()
        {
            WeClappUserDto weClappUserDtoObj = new WeClappUserDto();
            WeClappUserResponse ResponseWeClappUser = new WeClappUserResponse();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (UserId != null)
            {
                var weClappUserObj = _weClappUserService.GetByUser(UserId);
                weClappUserDtoObj = _mapper.Map<WeClappUserDto>(weClappUserObj);
                ResponseWeClappUser = _mapper.Map<WeClappUserResponse>(weClappUserDtoObj);
                return new OperationResult<WeClappUserResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseWeClappUser);
            }
            else
            {
                ResponseWeClappUser = _mapper.Map<WeClappUserResponse>(weClappUserDtoObj);
                return new OperationResult<WeClappUserResponse>(false, System.Net.HttpStatusCode.OK, "Please provide userid", ResponseWeClappUser);
            }
        }

        [HttpPost]
        public async Task<OperationResult<WeClappUserAddUpdateResponse>> AddUpdate([FromBody] WeClappUserAddUpdateRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<WeClappUserDto>(Model);
            if (!string.IsNullOrEmpty(requestmodel.TenantName) && !string.IsNullOrEmpty(requestmodel.ApiKey) && UserId != null)
            {
                var customers = await _weClappService.GetCustomers(requestmodel.ApiKey, requestmodel.TenantName);
                if (customers != null)
                {
                    requestmodel.UserId = UserId;
                    var AddUpdate = await _weClappUserService.CheckInsertOrUpdate(requestmodel);
                    requestmodel = _mapper.Map<WeClappUserDto>(AddUpdate);
                    var responsemodel = _mapper.Map<WeClappUserAddUpdateResponse>(requestmodel);
                    return new OperationResult<WeClappUserAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
                }
                else
                {
                    var responsemodel = _mapper.Map<WeClappUserAddUpdateResponse>(requestmodel);
                    return new OperationResult<WeClappUserAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Invalid credential", responsemodel);
                }
            }
            else
            {
                var responsemodel = _mapper.Map<WeClappUserAddUpdateResponse>(requestmodel);
                return new OperationResult<WeClappUserAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Please provide tenant name, apikey and userId.", responsemodel);
            }
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            if (Id != null && Id > 0)
            {
                var DeletedObj = await _weClappUserService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

    }
}