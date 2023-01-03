using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
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

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CheckListAssignUserController : Controller
    {
        private readonly ICheckListService _checkListService;
        private readonly IOneClappModuleService _oneClappModuleService;
        private readonly ICheckListUserService _checkListUserService;
        private readonly ICheckListAssignUserService _checkListAssignUserService;
        private readonly IUserService _userService;
        public IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public CheckListAssignUserController(
            ICheckListService checkListService,
            IOneClappModuleService oneClappModuleService,
            ICheckListUserService checkListUserService,
            ICheckListAssignUserService checkListAssignUserService,
            IUserService userService,
            IMapper mapper
        )
        {
            _checkListService = checkListService;
            _oneClappModuleService = oneClappModuleService;
            _checkListUserService = checkListUserService;
            _checkListAssignUserService = checkListAssignUserService;
            _userService = userService;
            _mapper = mapper;
        }

        #region CheckListAssignUser


        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<CheckListDto>>> Add([FromBody] List<CheckListDto> checkListDtoList)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;

            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (checkListDtoList != null && checkListDtoList.Count() > 0)
            {
                foreach (var model in checkListDtoList)
                {
                    if (model.AssignUsers != null && model.AssignUsers.Count() > 0)
                    {
                        foreach (var checkListAssignUser in model.AssignUsers)
                        {
                            CheckListAssignUserDto checkListAssignUserDto = new CheckListAssignUserDto();
                            checkListAssignUserDto.AssignUserId = checkListAssignUser.AssignUserId;
                            checkListAssignUserDto.CheckListId = model.Id;
                            checkListAssignUserDto.TenantId = TenantId;
                            checkListAssignUserDto.CreatedBy = UserId;
                            var AddUpdate = await _checkListAssignUserService.CheckInsertOrUpdate(checkListAssignUserDto);
                            checkListAssignUser.Id = AddUpdate.Id;
                            model.CheckListAssignUserId = AddUpdate.Id;
                        }
                    }
                }
            }
            return new OperationResult<List<CheckListDto>>(true, System.Net.HttpStatusCode.OK, "", checkListDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<List<CheckListDto>>> Update([FromBody] List<CheckListDto> checkListDtoList)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;

            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (checkListDtoList != null && checkListDtoList.Count() > 0)
            {
                foreach (var model in checkListDtoList)
                {
                    if (model.AssignUsers != null && model.AssignUsers.Count() > 0)
                    {
                        foreach (var checkListAssignUser in model.AssignUsers)
                        {
                            CheckListAssignUserDto checkListAssignUserDto = new CheckListAssignUserDto();
                            checkListAssignUserDto.AssignUserId = checkListAssignUser.AssignUserId;
                            checkListAssignUserDto.CheckListId = model.Id;
                            checkListAssignUserDto.TenantId = TenantId;
                            checkListAssignUserDto.CreatedBy = UserId;
                            var AddUpdate = await _checkListAssignUserService.CheckInsertOrUpdate(checkListAssignUserDto);
                            checkListAssignUser.Id = AddUpdate.Id;
                            model.CheckListAssignUserId = AddUpdate.Id;
                        }
                    }
                }
            }
            return new OperationResult<List<CheckListDto>>(true, System.Net.HttpStatusCode.OK, "", checkListDtoList);
        }


        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CheckListAssignUserDto>> Remove([FromBody] CheckListAssignUserDto model)
        {
            if (model.Id != null)
            {
                var checkListUsers = _checkListAssignUserService.DeleteCheckListAssignUser(model.Id.Value);
                return new OperationResult<CheckListAssignUserDto>(true, System.Net.HttpStatusCode.OK, "Checklist assignuser deleted successfully.", model);
            }
            else
            {
                return new OperationResult<CheckListAssignUserDto>(false, System.Net.HttpStatusCode.OK, "Please provide checklist id.", model);
            }
        }


        // For get all checklist by admin, tenant admin and tenant manager
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<AdminCheckListDto>> List([FromBody] CheckListDto model)
        {
            AdminCheckListDto adminCheckListDto = new AdminCheckListDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (!string.IsNullOrEmpty(model.ModuleName))
            {
                var moduleObj = _oneClappModuleService.GetByName(model.ModuleName);
                if (moduleObj != null)
                {
                    model.ModuleId = moduleObj.Id;
                }
            }
            List<CheckListDto> checkListDtoList = new List<CheckListDto>();
            List<CheckList> checkLists = new List<CheckList>();
            if (model.ModuleId != null)
            {
                checkLists = _checkListService.GetAllByModule(model.ModuleId.Value, TenantId);
                checkListDtoList = _mapper.Map<List<CheckListDto>>(checkLists);
            }

            List<CheckListAssignUserDto> checkListAssignUserDtoList = new List<CheckListAssignUserDto>();
            var assignUsers = _checkListAssignUserService.GetAll();
            checkListAssignUserDtoList = _mapper.Map<List<CheckListAssignUserDto>>(assignUsers);

            List<User> userList = new List<User>();
            if (model.TenantId == null)
            {
                userList = _userService.GetAll();
            }
            else
            {
                userList = _userService.GetAllUsersByTenantAdmin(TenantId);
            }

            List<CheckListAssignUserDto> checkListAssignUserDtoList1 = new List<CheckListAssignUserDto>();
            checkListAssignUserDtoList1 = _mapper.Map<List<CheckListAssignUserDto>>(userList);

            if (checkListAssignUserDtoList1 != null && checkListAssignUserDtoList1.Count() > 0)
            {
                foreach (var userObj in checkListAssignUserDtoList1)
                {
                    userObj.AssignUserId = Convert.ToInt32(userObj.Id);
                    if (userObj.FirstName != null)
                    {
                        userObj.ShortName = userObj.FirstName.Substring(0, 1);
                    }
                    if (userObj.LastName != null)
                    {
                        userObj.ShortName = userObj.ShortName + userObj.LastName.Substring(0, 1);
                    }
                    userObj.SelectedCheckList = new List<CheckListDto>();
                    var checkListAssignUserArr = checkListAssignUserDtoList.Where(t => t.AssignUserId == userObj.Id).ToList();
                    if (checkListAssignUserArr != null && checkListAssignUserArr.Count() > 0)
                    {
                        foreach (var assignUserObj in checkListAssignUserArr)
                        {
                            CheckListDto checkListDto = new CheckListDto();
                            if (checkLists != null)
                            {
                                var checkListObj = checkLists.Where(t => t.Id == assignUserObj.CheckListId).FirstOrDefault();
                                if (checkListObj != null)
                                {
                                    checkListDto = _mapper.Map<CheckListDto>(checkListObj);
                                    checkListDto.CheckListAssignUserId = assignUserObj.Id;
                                    userObj.SelectedCheckList.Add(checkListDto);
                                }
                            }
                        }
                    }
                }
            }
            adminCheckListDto.CheckList = checkListDtoList;
            adminCheckListDto.UserList = checkListAssignUserDtoList1;

            return new OperationResult<AdminCheckListDto>(true, System.Net.HttpStatusCode.OK, "", adminCheckListDto);
        }
        #endregion
    }
}