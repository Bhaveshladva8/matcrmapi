using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ClientSocialMediaController : Controller
    {
        private readonly IClientSocialMediaService _clientSocialMediaService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ClientSocialMediaController(IClientSocialMediaService clientSocialMediaService,
        IMapper mapper)
        {
            _clientSocialMediaService = clientSocialMediaService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<ClientSocialMediaAddUpdateResponse>> Add([FromBody] ClientSocialMediaAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<ClientSocialMedia>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var clientSocialMediaObj = await _clientSocialMediaService.CheckInsertOrUpdate(model);
            var clientSocialMediaResponseObj = _mapper.Map<ClientSocialMediaAddUpdateResponse>(clientSocialMediaObj);
            return new OperationResult<ClientSocialMediaAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", clientSocialMediaResponseObj);
        }

        [HttpPut]
        public async Task<OperationResult<ClientSocialMediaAddUpdateResponse>> Update([FromBody] ClientSocialMediaAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<ClientSocialMedia>(requestmodel);
            if (model.Id != 0)
            {
                model.UpdatedBy = UserId;
            }
            var clientSocialMediaObj = await _clientSocialMediaService.CheckInsertOrUpdate(model);
            var clientSocialMediaResponseObj = _mapper.Map<ClientSocialMediaAddUpdateResponse>(clientSocialMediaObj);
            return new OperationResult<ClientSocialMediaAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", clientSocialMediaResponseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            if (Id > 0)
            {
                var clientSocialMediaObj = await _clientSocialMediaService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpGet("{ClientId}")]
        public async Task<OperationResult<List<ClientSocialMediaListResponse>>> List(long ClientId)
        {
            var clientSocialMediaList= _clientSocialMediaService.GetByClientId(ClientId);
            var responseList = _mapper.Map<List<ClientSocialMediaListResponse>>(clientSocialMediaList);
            return new OperationResult<List<ClientSocialMediaListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseList);
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<ClientSocialMediaDetailResponse>> Detail(int Id)
        {
            var clientSocialMediaObj = _clientSocialMediaService.GetById(Id);
            var ResponseObj = _mapper.Map<ClientSocialMediaDetailResponse>(clientSocialMediaObj);
            return new OperationResult<ClientSocialMediaDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }
        
    }
}