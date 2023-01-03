using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TaskWeclappUserController : Controller
    {
        private readonly ITaskWeclappUserService _taskWeclappUserService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public TaskWeclappUserController(ITaskWeclappUserService taskWeclappUserService,
        IMapper mapper)
        {
            _taskWeclappUserService = taskWeclappUserService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<TaskWeclappUserDto>> Credential()
        {
            TaskWeclappUserDto taskWeclappUserDto = new TaskWeclappUserDto();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var taskweclappUserObj = _taskWeclappUserService.GetByTenantId(TenantId);

            taskWeclappUserDto = _mapper.Map<TaskWeclappUserDto>(taskweclappUserObj);

            return new OperationResult<TaskWeclappUserDto>(true, System.Net.HttpStatusCode.OK,"", taskWeclappUserDto);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<TaskWeclappUserDto>> AddUpdate([FromBody] TaskWeclappUserDto taskWeclappUserDto)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            taskWeclappUserDto.TenantId = TenantId;
            taskWeclappUserDto.UserId = UserId;
            var taskweclappUserObj = await _taskWeclappUserService.CheckInsertOrUpdate(taskWeclappUserDto);

            taskWeclappUserDto = _mapper.Map<TaskWeclappUserDto>(taskweclappUserObj);

            return new OperationResult<TaskWeclappUserDto>(true, System.Net.HttpStatusCode.OK,"", taskWeclappUserDto);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            if (Id != null)
            {
                var taskweclappUserObj = _taskWeclappUserService.DeleteById(Id);
                return new OperationResult(true, "");
            }
            else
            {
                return new OperationResult(false, "Please provide id");
            }
        }
    }
}