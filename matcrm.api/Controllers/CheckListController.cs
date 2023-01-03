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
    public class CheckListController : Controller
    {
        private readonly ICheckListService _checkListService;
        private readonly IOneClappModuleService _oneClappModuleService;
        private readonly ICheckListUserService _checkListUserService;
        private readonly ICheckListAssignUserService _checkListAssignUserService;
        public IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public CheckListController(
            ICheckListService checkListService,
            IOneClappModuleService oneClappModuleService,
            ICheckListUserService checkListUserService,
            ICheckListAssignUserService checkListAssignUserService,
            IMapper mapper
        )
        {
            _checkListService = checkListService;
            _oneClappModuleService = oneClappModuleService;
            _checkListUserService = checkListUserService;
            _checkListAssignUserService = checkListAssignUserService;
            _mapper = mapper;
        }

        #region CheckList
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CheckListDto>> Add([FromBody] CheckListDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if(model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;
            if (!string.IsNullOrEmpty(model.ModuleName))
            {
                var moduleObj = _oneClappModuleService.GetByName(model.ModuleName);
                if (moduleObj != null)
                {
                    model.ModuleId = moduleObj.Id;
                }
            }

            var checkListObj = await _checkListService.CheckInsertOrUpdate(model);

            if (model.AssignUsers != null && model.AssignUsers.Count() > 0)
            {
                foreach (var checklistAssignUserModel in model.AssignUsers)
                {
                    if (checklistAssignUserModel.AssignUserId != null)
                    {
                        checklistAssignUserModel.CheckListId = checkListObj.Id;
                        checklistAssignUserModel.TenantId = TenantId;
                        checklistAssignUserModel.CreatedBy = UserId;
                        var checklistAssignUser = await _checkListAssignUserService.CheckInsertOrUpdate(checklistAssignUserModel);
                    }
                }
            }
            model = _mapper.Map<CheckListDto>(checkListObj);
            if (model.Id > 0)
                return new OperationResult<CheckListDto>(true, System.Net.HttpStatusCode.OK, "Updated successfully", model);
            return new OperationResult<CheckListDto>(false, System.Net.HttpStatusCode.OK, "Added successfully.", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CheckListDto>> Update([FromBody] CheckListDto model)
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if(model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;
            if (!string.IsNullOrEmpty(model.ModuleName))
            {
                var moduleObj = _oneClappModuleService.GetByName(model.ModuleName);
                if (moduleObj != null)
                {
                    model.ModuleId = moduleObj.Id;
                }
            }

            var checkListObj = await _checkListService.CheckInsertOrUpdate(model);

            if (model.AssignUsers != null && model.AssignUsers.Count() > 0)
            {
                foreach (var checklistAssignUserModel in model.AssignUsers)
                {
                    if (checklistAssignUserModel.AssignUserId != null)
                    {
                        checklistAssignUserModel.CheckListId = checkListObj.Id;
                        checklistAssignUserModel.TenantId = TenantId;
                        checklistAssignUserModel.CreatedBy = UserId;
                        var checklistAssignUser = await _checkListAssignUserService.CheckInsertOrUpdate(checklistAssignUserModel);
                    }
                }
            }
            model = _mapper.Map<CheckListDto>(checkListObj);
            if (model.Id > 0)
                return new OperationResult<CheckListDto>(true, System.Net.HttpStatusCode.OK, "Updated successfully", model);
            return new OperationResult<CheckListDto>(false, System.Net.HttpStatusCode.OK, "Added successfully.", model);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<CheckListDto>>> AssignUser([FromBody] List<CheckListDto> checkListDtoList)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

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

                    }
                }
            }
            // if (!string.IsNullOrEmpty(model.ModuleName))
            // {
            //     var moduleObj = _oneClappModuleService.GetByName(model.ModuleName);
            //     if (moduleObj != null)
            //     {
            //         model.ModuleId = moduleObj.Id;
            //     }
            // }

            // var checkListObj = _checkListService.CheckInsertOrUpdate(model);

            // if (model.AssignUsers.Count() > 0)
            // {
            //     foreach (var checklistAssignUserModel in model.AssignUsers)
            //     {
            //         if (checklistAssignUserModel.AssignUserId != null)
            //         {
            //             checklistAssignUserModel.CheckListId = checkListObj.Id;
            //             checklistAssignUserModel.TenantId = model.TenantId;
            //             var checklistAssignUser = _checkListAssignUserService.CheckInsertOrUpdate(checklistAssignUserModel);
            //         }
            //     }
            // }
            // model = _mapper.Map<CheckListDto>(checkListObj);

            return new OperationResult<List<CheckListDto>>(true, System.Net.HttpStatusCode.OK, "", checkListDtoList);
        }



        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]        
        [HttpDelete]
        public async Task<OperationResult<CheckListDto>> Remove([FromBody] CheckListDto model)
        {
            if (model.Id != null)
            {
                var checkListUsers = _checkListUserService.DeleteByCheckListId(model.Id.Value);
                var checkListAssignUsers = _checkListAssignUserService.DeleteByCheckListId(model.Id.Value);
                var checklistObj = await _checkListService.DeleteCheckList(model.Id.Value);
                model = _mapper.Map<CheckListDto>(checklistObj);
                return new OperationResult<CheckListDto>(true, System.Net.HttpStatusCode.OK, "Checklist deleted successfully.", model);
            }
            else
            {
                return new OperationResult<CheckListDto>(false, System.Net.HttpStatusCode.OK, "Please provide checklist id.", model);
            }
        }


        // For get all checklist by admin, tenant admin and tenant manager
        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<CheckListDto>>> List([FromBody] CheckListDto model)
        {
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
            List<CheckListDto> checkListDtos = new List<CheckListDto>();
            if (model.ModuleId != null)
            {
                var checkLists = _checkListService.GetAllByModule(model.ModuleId.Value, TenantId);

                // if (model.UserId != null)
                // {
                //     for (int i = checklists.Count() - 1; i >= 0; i--)
                //     {
                //         var item = checklists[i];
                //         var checklistUserArray = _checkListUserService.GetCheckListId(item.Id);

                //         if (checklistUserArray.Count() > 0)
                //         {
                //             var isExit = checklistUserArray.Where(t => t.UserId == model.UserId).FirstOrDefault();
                //             if (isExit == null)
                //             {
                //                 checklists.Remove(item);
                //             }
                //         }
                //     }
                // }

                checkListDtos = _mapper.Map<List<CheckListDto>>(checkLists);
            }

            return new OperationResult<List<CheckListDto>>(true, System.Net.HttpStatusCode.OK, "", checkListDtos);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<CheckListDto>>> BasedOnUser([FromBody] CheckListDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
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

            if (model.ModuleId != null)
            {
                var checklists = _checkListService.GetAllByModule(model.ModuleId.Value, TenantId);
                checkListDtoList = _mapper.Map<List<CheckListDto>>(checklists);
                var checkListUsers = _checkListUserService.GetByUser(UserId);
                if (UserId != null)
                {
                    for (int i = checkListDtoList.Count() - 1; i >= 0; i--)
                    {
                        var item = checkListDtoList[i];
                        if (item.Id != null)
                        {
                            var checklistAssignUserArray = _checkListAssignUserService.GetCheckListId(item.Id.Value);

                            if (checklistAssignUserArray != null && checklistAssignUserArray.Count() > 0)
                            {
                                var isExitAssignUser = checklistAssignUserArray.Where(t => t.AssignUserId == UserId).FirstOrDefault();
                                if (isExitAssignUser == null)
                                {
                                    checkListDtoList.Remove(item);
                                }

                                var checkListUserObj = _checkListUserService.GetByUserAndCheckListId(UserId, item.Id.Value);
                                if (checkListUserObj != null)
                                {
                                    model.IsChecked = checkListUserObj.IsChecked;
                                }
                                else
                                {
                                    model.IsChecked = false;
                                }
                            }
                        }
                    }
                }
            }

            // foreach (var item in checkListDtos)
            // {
            //     var ExistDataObj = checkListUsers.Where(t => t.CheckListId == item.Id).FirstOrDefault();
            //     if (ExistDataObj != null)
            //     {
            //         item.IsChecked = ExistDataObj.IsChecked;
            //     }
            //     else
            //     {
            //         item.IsChecked = false;
            //     }

            // }

            // List<CheckListDto> checkListDtos = new List<CheckListDto>();
            // var checklists = _checkListService.GetAllByModule(model.ModuleId.Value, model.TenantId);

            // if (model.UserId != null)
            // {
            //     for (int i = checklists.Count() - 1; i >= 0; i--)
            //     {
            //         var item = checklists[i];
            //         var checklistUserArray = _checkListUserService.GetCheckListId(item.Id);

            //         if (checklistUserArray.Count() > 0)
            //         {
            //             var isExit = checklistUserArray.Where(t => t.UserId == model.UserId).FirstOrDefault();
            //             if (isExit == null)
            //             {
            //                 checklists.Remove(item);
            //             }
            //         }
            //     }
            // }

            // checkListDtos = _mapper.Map<List<CheckListDto>>(checklists);

            return new OperationResult<List<CheckListDto>>(true, System.Net.HttpStatusCode.OK, "", checkListDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CheckListDto>> Detail(long Id)
        {
            CheckListDto checkListDto = new CheckListDto();
            var checkListObj = _checkListService.GetCheckListById(Id);
            checkListDto = _mapper.Map<CheckListDto>(checkListObj);

            var assignUsers = _checkListAssignUserService.GetCheckListId(Id);

            checkListDto.AssignUsers = _mapper.Map<List<CheckListAssignUserDto>>(assignUsers);

            return new OperationResult<CheckListDto>(true, System.Net.HttpStatusCode.OK, "", checkListDto);
        }
        #endregion
    }
}