using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CheckListUserController : Controller
    {
        private readonly ICheckListService _checkListService;
        private readonly ICheckListUserService _checkListUserService;
        public IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public CheckListUserController(
            ICheckListService checkListService,
             ICheckListUserService checkListUserService,
             IMapper mapper
        )
        {
            _checkListService = checkListService;
            _checkListUserService = checkListUserService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CheckListUserDto>> Add([FromBody] CheckListUserDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            model.UserId = UserId;
            model.TenantId = TenantId;
            var checkListUserObj = await _checkListUserService.CheckInsertOrUpdate(model);

            model = _mapper.Map<CheckListUserDto>(checkListUserObj);
            if (model.Id > 0)
                return new OperationResult<CheckListUserDto>(true, System.Net.HttpStatusCode.OK,"Updated successfully", model);
            return new OperationResult<CheckListUserDto>(false, System.Net.HttpStatusCode.OK,"Added successfully.", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CheckListUserDto>> Update([FromBody] CheckListUserDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            model.UserId = UserId;
            model.TenantId = TenantId;
            var checkListUserObj = await _checkListUserService.CheckInsertOrUpdate(model);

            model = _mapper.Map<CheckListUserDto>(checkListUserObj);
            if (model.Id > 0)
                return new OperationResult<CheckListUserDto>(true, System.Net.HttpStatusCode.OK,"Updated successfully", model);
            return new OperationResult<CheckListUserDto>(false, System.Net.HttpStatusCode.OK,"Added successfully.", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]        
        [HttpDelete]
        public async Task<OperationResult<CheckListUserDto>> Remove([FromBody] CheckListUserDto model)
        {
            if (model.Id != null)
            {
                var checklistUserObj = _checkListUserService.DeleteCheckListUser(model.Id.Value);
                model = _mapper.Map<CheckListUserDto>(checklistUserObj);
                return new OperationResult<CheckListUserDto>(true, System.Net.HttpStatusCode.OK,"Check list user deleted successfully.", model);
            }
            else
            {
                return new OperationResult<CheckListUserDto>(false, System.Net.HttpStatusCode.OK,"Please provide check list user id.", model);
            }
        }
    }
}