using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using AutoMapper;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class UserERPSystemController : Controller
    {

        private readonly IUserERPSystemService _userERPSystemService;
        private IMapper _mapper;
        private int UserId = 0;
        public UserERPSystemController(IUserERPSystemService userERPSystemService, IMapper mapper)
        {
            _userERPSystemService = userERPSystemService;
            _mapper = mapper;
        }

        // GetAll Email Provider Method
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<OperationResult<List<UserERPSystemDto>>> List()
        {
            var userErpSystemObj = _userERPSystemService.GetAllByAdmin();

            List<UserERPSystemDto> userErpSystemDetailList = new List<UserERPSystemDto>();

            if (userErpSystemObj != null)
            {
                userErpSystemDetailList = _mapper.Map<List<UserERPSystemDto>>(userErpSystemObj);

                return new OperationResult<List<UserERPSystemDto>>(true, System.Net.HttpStatusCode.OK,"User ERP system found successfully", userErpSystemDetailList);
            }

            return new OperationResult<List<UserERPSystemDto>>(false, System.Net.HttpStatusCode.OK,"User ERP system not found ");

        }


        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{UserId}")]
        public async Task<OperationResult<List<UserERPSystemDto>>> ListByTenantAdmin(long UserId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var userErpSystemObj = _userERPSystemService.GetAllByTenantAdmin(UserId);

            List<UserERPSystemDto> userErpSystemDetailList = new List<UserERPSystemDto>();

            if (userErpSystemObj != null)
            {
                userErpSystemDetailList = _mapper.Map<List<UserERPSystemDto>>(userErpSystemObj);

                return new OperationResult<List<UserERPSystemDto>>(true, System.Net.HttpStatusCode.OK,"User ERP system found successfully", userErpSystemDetailList);
            }

            return new OperationResult<List<UserERPSystemDto>>(false, System.Net.HttpStatusCode.OK,"User ERP system not found ");

        }

        // GetAll Email Provider Method
        [Authorize(Roles = "Admin")]
        [HttpGet("{UserErpSystemById}")]
        public async Task<OperationResult<UserERPSystemDto>> Detail(int UserErpSystemById)
        {
            var userERPSystemObj = _userERPSystemService.GetUserERPSystemById(UserErpSystemById);

            if (userERPSystemObj != null)
            {
                var userErpSystemDetailVMObj = _mapper.Map<UserERPSystemDto>(userERPSystemObj);
                return new OperationResult<UserERPSystemDto>(true, System.Net.HttpStatusCode.OK,"User ERP system found successfully", userErpSystemDetailVMObj);
            }
            else
            {
                return new OperationResult<UserERPSystemDto>(false, System.Net.HttpStatusCode.OK,"User ERP system not found ");
            }

        }

        // Add Updated Email Provider Method
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin,TenantUser")]
        [HttpPost]
        public async Task<OperationResult<UserERPSystemDto>> AddUpdate([FromBody] UserERPSystemDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            model.UserId = UserId;
            var validateUserERPSystemPresent = _userERPSystemService.ValidateUserERPSystem(model);
            if (validateUserERPSystemPresent != null)
            {
                return new OperationResult<UserERPSystemDto>(false, System.Net.HttpStatusCode.OK,"User ERP system found with same credential");
            }
            else
            {
                var userErpSystemObj = await _userERPSystemService.CheckInsertOrUpdate(model);
                if (userErpSystemObj != null)
                {
                    var userErpSystemDtoObj = _mapper.Map<UserERPSystemDto>(userErpSystemObj);
                    return new OperationResult<UserERPSystemDto>(true, System.Net.HttpStatusCode.OK,"User ERP system found successfully", userErpSystemDtoObj);
                }
                else
                {
                    return new OperationResult<UserERPSystemDto>(false, System.Net.HttpStatusCode.OK,"Somehting went wrong");
                }
            }

        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin,TenantUser")]
        [HttpDelete("{erpSystemId}")]
        public async Task<OperationResult<UserERPSystemDto>> Remove(int erpSystemId)
        {
            var deleteUserErpSystemObj = _userERPSystemService.DeleteUserERPSystem(erpSystemId);

            if (deleteUserErpSystemObj != null)
            {
                var deleteUserErpSystemDtoObj = _mapper.Map<UserERPSystemDto>(deleteUserErpSystemObj);
                return new OperationResult<UserERPSystemDto>(true, System.Net.HttpStatusCode.OK,"User ERP system deleted successfully", deleteUserErpSystemDtoObj);
            }
            else
            {
                return new OperationResult<UserERPSystemDto>(false, System.Net.HttpStatusCode.OK,"Somehting went wrong");
            }

        }

    }

}