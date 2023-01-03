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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Route("[controller]")]
    public class SectionNewController : Controller
    {
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly ISectionService _sectionService;
        private readonly IUserService _userService;
        private readonly IEmployeeProjectActivityService _projectActivityService;
        private readonly IEmployeeProjectStatusService _employeeProjectStatusService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IEmployeeTaskAttachmentService _employeeTaskAttachmentService;

        private readonly IEmployeeTaskCommentService _employeeTaskCommentService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserSerivce;
        private readonly IEmployeeSubTaskUserService _employeeSubTaskUserService;
        private readonly IEmployeeChildTaskUserService _employeeChildTaskUserService;
        private readonly IEmployeeSubTaskTimeRecordService _employeeSubTaskTimeRecordService;
        private readonly IEmployeeChildTaskTimeRecordService _employeeChildTaskTimeRecordService;
        private readonly IWeClappService _weClappService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmployeeSubTaskAttachmentService _employeeSubTaskAttachmentService;
        private readonly IEmployeeChildTaskAttachmentService _employeeChildTaskAttachmentService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IEmployeeTaskStatusService _employeeTaskStatusService;
        private readonly ISectionActivityService _sectionActivityService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public SectionNewController(
            IEmployeeProjectService employeeProjectService,
            ISectionService sectionService,
            IEmployeeProjectStatusService employeeProjectStatusService,
            IUserService userService,
            IEmployeeProjectActivityService projectActivityService,
            IEmployeeTaskService employeeTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserService,
            IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
            IEmployeeTaskAttachmentService employeeTaskAttachmentService,
            IEmployeeTaskCommentService employeeTaskCommentService,
            IEmployeeTaskActivityService employeeTaskActivityService,
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeChildTaskService employeeChildTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserSerivce,
            IEmployeeSubTaskUserService employeeSubTaskUserService,
            IEmployeeChildTaskUserService employeeChildTaskUserService,
            IEmployeeSubTaskTimeRecordService employeeSubTaskTimeRecordService,
            IEmployeeChildTaskTimeRecordService employeeChildTaskTimeRecordService,
            IWeClappService weClappService,
            IHostingEnvironment hostingEnvironment,
            IEmployeeSubTaskAttachmentService employeeSubTaskAttachmentService,
            IEmployeeChildTaskAttachmentService employeeChildTaskAttachmentService,
            IEmployeeSubTaskActivityService employeeSubTaskActivityService,
            IEmployeeChildTaskActivityService employeeChildTaskActivityService,
            IEmployeeSubTaskCommentService employeeSubTaskCommentService,
            IEmployeeChildTaskCommentService employeeChildTaskCommentService,
            IEmployeeTaskStatusService employeeTaskStatusService,
            ISectionActivityService sectionActivityService,
            IMapper mapper
        )
        {
            _employeeProjectService = employeeProjectService;
            _sectionService = sectionService;
            _employeeProjectStatusService = employeeProjectStatusService;
            _userService = userService;
            _projectActivityService = projectActivityService;
            _employeeTaskService = employeeTaskService;
            _employeeTaskUserService = employeeTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeTaskAttachmentService = employeeTaskAttachmentService;
            _employeeTaskCommentService = employeeTaskCommentService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeTaskUserSerivce = employeeTaskUserSerivce;
            _employeeSubTaskUserService = employeeSubTaskUserService;
            _employeeChildTaskUserService = employeeChildTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeSubTaskTimeRecordService = employeeSubTaskTimeRecordService;
            _employeeChildTaskTimeRecordService = employeeChildTaskTimeRecordService;
            _weClappService = weClappService;
            _hostingEnvironment = hostingEnvironment;
            _employeeSubTaskAttachmentService = employeeSubTaskAttachmentService;
            _employeeChildTaskAttachmentService = employeeChildTaskAttachmentService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _userService = userService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeTaskStatusService = employeeTaskStatusService;
            _sectionActivityService = sectionActivityService;
            _mapper = mapper;
        }

        // For add update section
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<SectionDto>> AddUpdate([FromBody] SectionDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (model.Id == null)
            {
                model.CreatedBy = UserId;
            }
            else
            {
                model.UpdatedBy = UserId;
            }

            model.TenantId = TenantId;
            var AddUpdate = await _sectionService.CheckInsertOrUpdate(model);

            SectionActivity sectionActivityObj = new SectionActivity();
            sectionActivityObj.SectionId = AddUpdate.Id;
            if (model.Id == null)
            {
                sectionActivityObj.Activity = "Created section";
            }
            else
            {
                sectionActivityObj.Activity = "Updated section";
            }

            sectionActivityObj.UserId = UserId;
            var sectionActivityAddUpdateObj = await _sectionActivityService.CheckInsertOrUpdate(sectionActivityObj);
            model.Id = AddUpdate.Id;
            return new OperationResult<SectionDto>(true, System.Net.HttpStatusCode.OK, "Project add successfully", model);
        }

        // // Return section and tasklist based on project
        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost("List")]
        // public async Task<OperationResult<EmployeeSectionTaskListVM>> GetAllByProject([FromBody] SectionTaskDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     EmployeeSectionTaskListVM employeeSectionTaskListVMObj = new EmployeeSectionTaskListVM();

        //     List<EmployeeSectionVM> employeeSectionVMList = new List<EmployeeSectionVM>();
        //     var y = 60 * 60 * 1000;
        //     List<Section> SectionList = new List<Section>();
        //     if (model.ProjectId != null)
        //     {
        //         SectionList = _sectionService.GetByProject(model.ProjectId.Value);
        //     }
        //     else
        //     {
        //         SectionList = _sectionService.GetAllByTenant(TenantId);
        //     }

        //     employeeSectionVMList = _mapper.Map<List<EmployeeSectionVM>>(SectionList);

        //     // Start logic for Task with Project
        //     if (employeeSectionVMList != null && employeeSectionVMList.Count() > 0)
        //     {
        //         var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
        //         var taskIdList = tasks.Select(t => t.Id).ToList();
        //         var subTasks = _employeeSubTaskService.GetAllActiveByTaskIds(taskIdList);
        //         var subTaskIdList = subTasks.Select(t => t.Id).ToList();
        //         var childTasks = _employeeChildTaskService.GetAllActiveBySubTaskIds(subTaskIdList);

        //         foreach (var item in employeeSectionVMList)
        //         {
        //             if (item.Id != null)
        //             {
        //                 item.SectionId = item.Id;
        //                 var taskList = _employeeTaskService.GetAllTaskBySection(item.Id.Value);
        //                 if (taskList != null && taskList.Count() > 0)
        //                 {
        //                     if (model.FilterTaskDescription != null)
        //                     {
        //                         taskList = taskList.Where(t => t.Description.ToLower().Contains(model.FilterTaskDescription.ToLower())).ToList();
        //                     }
        //                     if (model.StatusId != null)
        //                     {
        //                         taskList = taskList.Where(t => t.StatusId == model.StatusId).ToList();
        //                     }
        //                     // var taskWeClappIdList = taskList.Select (t => t.WeClappTimeRecordId).ToList ();

        //                     // var taskRecordExist = timeRecords.Where (t => taskWeClappIdList.Contains (Convert.ToInt64 (t.id))).Select (t => t.id).ToList ();

        //                     var FinalTaskList = taskList.ToList();

        //                     if (FinalTaskList != null && FinalTaskList.Count() > 0)
        //                     {
        //                         item.Tasks = new List<EmployeeTaskVM>();
        //                         var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(FinalTaskList);
        //                         if (taskVMList != null && taskVMList.Count() > 0)
        //                         {
        //                             foreach (var taskObj in taskVMList)
        //                             {
        //                                 // taskObj.SectionId = item.SectionId;
        //                                 if (taskObj.Id != null)
        //                                 {
        //                                     var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
        //                                    taskObj.Duration = taskTotalDuration;

        //                                     // var h = taskTotalDuration / y;
        //                                     // var m = (taskTotalDuration - (h * y)) / (y / 60);
        //                                     // var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
        //                                     // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                                     // taskObj.Seconds = s;
        //                                     // taskObj.Minutes = m;
        //                                     // taskObj.Hours = h;
        //                                     var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
        //                                     if (assignTaskUsers.Count > 0)
        //                                     {
        //                                         taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
        //                                         var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        //                                         taskObj.AssignedUsers = assignUsersVMList;
        //                                     }
        //                                     //taskObj.AssignUserCount = assignTaskUsers.Count();
        //                                 }
        //                                 if (subTasks != null && subTasks.Count() > 0)
        //                                 {
        //                                     var subTaskList = subTasks.Where(t => t.EmployeeTaskId == taskObj.Id).ToList();
        //                                     taskObj.SubTasks = new List<EmployeeSubTaskVM>();
        //                                     if (subTaskList != null && subTaskList.Count() > 0)
        //                                     {
        //                                         var subTaskVMList = _mapper.Map<List<EmployeeSubTaskVM>>(subTaskList);
        //                                         if (subTaskVMList != null && subTaskVMList.Count() > 0)
        //                                         {
        //                                             foreach (var subTaskVM in subTaskVMList)
        //                                             {
        //                                                 var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(subTaskVM.Id);
        //                                                 subTaskVM.Duration = subTaskTotalDuration;

        //                                                 // var hh = subTaskTotalDuration / y;
        //                                                 // var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
        //                                                 // var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
        //                                                 // var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

        //                                                 // subTaskVM.Seconds = ss;
        //                                                 // subTaskVM.Minutes = mm;
        //                                                 // subTaskVM.Hours = hh;

        //                                                 var assignSubTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
        //                                                 if (assignSubTaskUsers.Count > 0)
        //                                                 {
        //                                                     subTaskVM.AssignedUsers = new List<EmployeeSubTaskUserDto>();
        //                                                     var assignUsersVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignSubTaskUsers);
        //                                                     subTaskVM.AssignedUsers = assignUsersVMList;
        //                                                 }
        //                                                 if (childTasks != null && childTasks.Count() > 0)
        //                                                 {
        //                                                     var childTaskList = childTasks.Where(t => t.EmployeeSubTaskId == subTaskVM.Id).ToList();

        //                                                     subTaskVM.ChildTasks = new List<EmployeeChildTaskVM>();
        //                                                     if (childTaskList != null && childTaskList.Count() > 0)
        //                                                     {
        //                                                         var childTaskVMList = _mapper.Map<List<EmployeeChildTaskVM>>(childTaskList);
        //                                                         if (childTaskVMList != null && childTaskVMList.Count() > 0)
        //                                                         {
        //                                                             foreach (var childTaskVM in childTaskVMList)
        //                                                             {
        //                                                                 var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskVM.Id);
        //                                                                 childTaskVM.Duration = childTaskTotalDuration;

        //                                                                 // var h3 = childTaskTotalDuration / y;
        //                                                                 // var m3 = (childTaskTotalDuration - (h3 * y)) / (y / 60);
        //                                                                 // var s3 = (childTaskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
        //                                                                 // // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                                                                 // childTaskVM.Seconds = s3;
        //                                                                 // childTaskVM.Minutes = m3;
        //                                                                 // childTaskVM.Hours = h3;

        //                                                                 var assignChildTaskUsers = _employeeChildTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
        //                                                                 if (assignChildTaskUsers.Count > 0)
        //                                                                 {
        //                                                                     childTaskVM.AssignedUsers = new List<EmployeeChildTaskUserDto>();
        //                                                                     var assignUsersVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
        //                                                                     childTaskVM.AssignedUsers = assignUsersVMList;
        //                                                                 }
        //                                                             }
        //                                                         }
        //                                                         subTaskVM.ChildTasks = childTaskVMList;
        //                                                     }
        //                                                 }
        //                                             }
        //                                         }

        //                                         taskObj.SubTasks = subTaskVMList;
        //                                     }
        //                                 }

        //                             }
        //                         }
        //                         var taskSorting = taskVMList.OrderBy(t => t.Priority).ToList();
        //                         item.Tasks = taskSorting;
        //                         if (model.ShortColumn != null && model.ShortColumn != "")
        //                         {
        //                             var taskTempList = ShortTaskByColumn(model.ShortColumn, model.ShortOrder, item.Tasks);
        //                             item.Tasks = taskTempList;
        //                         }

        //                         // item.Tasks = taskVMList;
        //                     }
        //                     else
        //                     {
        //                         item.Tasks = new List<EmployeeTaskVM>();
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     // End Logic

        //     // Start Logic for Tasks without Project
        //     List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();
        //     if (model.ProjectId != null)
        //     {
        //         employeeTaskList = _employeeTaskService.GetAllTaskByTenantWithOutSection(TenantId, model.ProjectId.Value);
        //     }
        //     else
        //     {
        //         employeeTaskList = _employeeTaskService.GetAllTaskByTenantWithOutProject(TenantId);
        //     }

        //     var taskIdList1 = employeeTaskList.Select(t => t.Id).ToList();

        //     var subTasks1 = _employeeSubTaskService.GetAllActiveByTaskIds(taskIdList1);
        //     var subTaskIdList1 = subTasks1.Select(t => t.Id).ToList();
        //     var childTasks1 = _employeeChildTaskService.GetAllActiveBySubTaskIds(subTaskIdList1);
        //     // var taskWeClappIdList1 = taskWithOutProjectList.Select(t => t.WeClappTimeRecordId).ToList();
        //     // var taskRecordExist1 = timeRecords.Where(t => taskWeClappIdList1.Contains(Convert.ToInt64(t.id))).Select(t => t.id).ToList();

        //     var FinalTaskList1 = employeeTaskList.ToList();
        //     if (FinalTaskList1 != null && FinalTaskList1.Count() > 0)
        //     {
        //         employeeSectionTaskListVMObj.Tasks = new List<EmployeeTaskVM>();
        //         var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(FinalTaskList1);
        //         if (taskVMList != null && taskVMList.Count() > 0)
        //         {
        //             if (model.FilterTaskDescription != null)
        //             {
        //                 taskVMList = taskVMList.Where(t => t.Description.ToLower().Contains(model.FilterTaskDescription.ToLower())).ToList();
        //             }
        //             if (model.StatusId != null)
        //             {
        //                 taskVMList = taskVMList.Where(t => t.StatusId == model.StatusId).ToList();
        //             }
        //             foreach (var taskObj in taskVMList)
        //             {
        //                 if (taskObj.Id != null)
        //                 {
        //                     var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
        //                     taskObj.Duration = taskTotalDuration;

        //                     // var h = taskTotalDuration / y;
        //                     // var m = (taskTotalDuration - (h * y)) / (y / 60);
        //                     // var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
        //                     // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                     // taskObj.Seconds = s;
        //                     // taskObj.Minutes = m;
        //                     // taskObj.Hours = h;
        //                     var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
        //                     if (assignTaskUsers.Count > 0)
        //                     {
        //                         taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
        //                         var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        //                         taskObj.AssignedUsers = assignUsersVMList;
        //                     }

        //                     //taskObj.AssignUserCount = assignTaskUsers.Count();
        //                 }
        //                 if (subTasks1 != null && subTasks1.Count() > 0)
        //                 {
        //                     var subTaskList = subTasks1.Where(t => t.EmployeeTaskId == taskObj.Id).ToList();
        //                     taskObj.SubTasks = new List<EmployeeSubTaskVM>();
        //                     if (subTaskList != null && subTaskList.Count() > 0)
        //                     {
        //                         var subTaskVMList = _mapper.Map<List<EmployeeSubTaskVM>>(subTaskList);
        //                         foreach (var subTaskVM in subTaskVMList)
        //                         {
        //                             var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(subTaskVM.Id);
        //                             subTaskVM.Duration = subTaskTotalDuration;

        //                             // var hh = subTaskTotalDuration / y;
        //                             // var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
        //                             // var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
        //                             // var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

        //                             // subTaskVM.Seconds = ss;
        //                             // subTaskVM.Minutes = mm;
        //                             // subTaskVM.Hours = hh;

        //                             var assignSubTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
        //                             if (assignSubTaskUsers.Count > 0)
        //                             {
        //                                 subTaskVM.AssignedUsers = new List<EmployeeSubTaskUserDto>();
        //                                 var assignUsersVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignSubTaskUsers);
        //                                 subTaskVM.AssignedUsers = assignUsersVMList;
        //                             }
        //                             if (childTasks1 != null && childTasks1.Count() > 0)
        //                             {
        //                                 var childTaskList = childTasks1.Where(t => t.EmployeeSubTaskId == subTaskVM.Id).ToList();

        //                                 subTaskVM.ChildTasks = new List<EmployeeChildTaskVM>();
        //                                 if (childTaskList != null && childTaskList.Count() > 0)
        //                                 {

        //                                     var childTaskVMList = _mapper.Map<List<EmployeeChildTaskVM>>(childTaskList);

        //                                     foreach (var childTaskVM in childTaskVMList)
        //                                     {
        //                                         var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskVM.Id);
        //                                         childTaskVM.Duration = childTaskTotalDuration;

        //                                         // var h3 = childTaskTotalDuration / y;
        //                                         // var m3 = (childTaskTotalDuration - (h3 * y)) / (y / 60);
        //                                         // var s3 = (childTaskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
        //                                         // // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                                         // childTaskVM.Seconds = s3;
        //                                         // childTaskVM.Minutes = m3;
        //                                         // childTaskVM.Hours = h3;

        //                                         var assignChildTaskUsers = _employeeChildTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
        //                                         if (assignChildTaskUsers.Count > 0)
        //                                         {
        //                                             childTaskVM.AssignedUsers = new List<EmployeeChildTaskUserDto>();
        //                                             var assignUsersVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
        //                                             childTaskVM.AssignedUsers = assignUsersVMList;
        //                                         }
        //                                     }
        //                                     subTaskVM.ChildTasks = childTaskVMList;
        //                                 }
        //                             }
        //                         }

        //                         taskObj.SubTasks = subTaskVMList;
        //                     }
        //                 }

        //             }
        //             var taskSorting = taskVMList.OrderBy(t => t.Priority).ToList();
        //             employeeSectionTaskListVMObj.Tasks = taskSorting;

        //             // Sort Task based on column
        //             if (model.ShortColumn != "" && model.ShortColumn != null)
        //             {
        //                 var taskTempList = ShortTaskByColumn(model.ShortColumn, model.ShortOrder, employeeSectionTaskListVMObj.Tasks);
        //                 employeeSectionTaskListVMObj.Tasks = taskTempList;
        //             }
        //         }
        //         // taskListVM.Tasks = taskVMList;
        //     }
        //     // End Logic

        //     // if (model.FilterTaskDescription != null) {
        //     //     ProjectList = ProjectList.Where (t => t.Tasks.Count () > 0).ToList ();
        //     // }
        //     // ProjectList = ProjectList.Where (t => t.Tasks.Count () > 0).ToList ();
        //     employeeSectionVMList = employeeSectionVMList.OrderBy(t => t.Priority).ToList();
        //     employeeSectionTaskListVMObj.Sections = employeeSectionVMList;

        //     return new OperationResult<EmployeeSectionTaskListVM>(true, System.Net.HttpStatusCode.OK, "", employeeSectionTaskListVMObj);
        // }

        // Return section and tasklistwithout project
        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost("All")]
        // public async Task<OperationResult<EmployeeSectionTaskListVM>> GetAllWithoutProject([FromBody] SectionTaskWithoutProjectRequestDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     EmployeeSectionTaskListVM employeeSectionTaskListVMObj = new EmployeeSectionTaskListVM();

        //     List<EmployeeSectionVM> employeeSectionVMList = new List<EmployeeSectionVM>();
        //     var y = 60 * 60 * 1000;
        //     List<Section> SectionList = new List<Section>();

        //     SectionList = _sectionService.GetAllByTenantWithoutProject(TenantId);


        //     employeeSectionVMList = _mapper.Map<List<EmployeeSectionVM>>(SectionList);

        //     // Start logic for Task with Project
        //     if (employeeSectionVMList != null && employeeSectionVMList.Count() > 0)
        //     {
        //         var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
        //         var taskIdList = tasks.Select(t => t.Id).ToList();
        //         var subTasks = _employeeSubTaskService.GetAllActiveByTaskIds(taskIdList);
        //         var subTaskIdList = subTasks.Select(t => t.Id).ToList();
        //         var childTasks = _employeeChildTaskService.GetAllActiveBySubTaskIds(subTaskIdList);

        //         foreach (var item in employeeSectionVMList)
        //         {
        //             if (item.Id != null)
        //             {
        //                 var taskList = _employeeTaskService.GetAllTaskBySection(item.Id.Value);
        //                 if (taskList != null && taskList.Count() > 0)
        //                 {
        //                     if (model.FilterTaskDescription != null)
        //                     {
        //                         taskList = taskList.Where(t => t.Description.ToLower().Contains(model.FilterTaskDescription.ToLower())).ToList();
        //                     }
        //                     if (model.StatusId != null)
        //                     {
        //                         taskList = taskList.Where(t => t.StatusId == model.StatusId).ToList();
        //                     }
        //                     // var taskWeClappIdList = taskList.Select (t => t.WeClappTimeRecordId).ToList ();

        //                     // var taskRecordExist = timeRecords.Where (t => taskWeClappIdList.Contains (Convert.ToInt64 (t.id))).Select (t => t.id).ToList ();

        //                     var FinalTaskList = taskList.ToList();

        //                     if (FinalTaskList != null && FinalTaskList.Count() > 0)
        //                     {
        //                         item.Tasks = new List<EmployeeTaskVM>();
        //                         var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(FinalTaskList);
        //                         foreach (var taskObj in taskVMList)
        //                         {
        //                             if (taskObj.Id != null)
        //                             {
        //                                 var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
        //                                 //taskObj.Duration = taskTotalDuration;

        //                                 // var h = taskTotalDuration / y;
        //                                 // var m = (taskTotalDuration - (h * y)) / (y / 60);
        //                                 // var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
        //                                 // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                                 // taskObj.Seconds = s;
        //                                 // taskObj.Minutes = m;
        //                                 // taskObj.Hours = h;
        //                                 var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
        //                                 if (assignTaskUsers.Count > 0)
        //                                 {
        //                                     taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
        //                                     var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        //                                     taskObj.AssignedUsers = assignUsersVMList;
        //                                 }
        //                                 //taskObj.AssignUserCount = assignTaskUsers.Count();
        //                             }
        //                             if (subTasks != null && subTasks.Count() > 0)
        //                             {
        //                                 var subTaskList = subTasks.Where(t => t.EmployeeTaskId == taskObj.Id).ToList();
        //                                 taskObj.SubTasks = new List<EmployeeSubTaskVM>();
        //                                 if (subTaskList != null && subTaskList.Count() > 0)
        //                                 {
        //                                     var subTaskVMList = _mapper.Map<List<EmployeeSubTaskVM>>(subTaskList);
        //                                     foreach (var subTaskVM in subTaskVMList)
        //                                     {
        //                                         var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(subTaskVM.Id);
        //                                         //subTaskVM.Duration = subTaskTotalDuration;

        //                                         // var hh = subTaskTotalDuration / y;
        //                                         // var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
        //                                         // var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
        //                                         // var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

        //                                         // subTaskVM.Seconds = ss;
        //                                         // subTaskVM.Minutes = mm;
        //                                         // subTaskVM.Hours = hh;

        //                                         var assignSubTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
        //                                         if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
        //                                         {
        //                                             subTaskVM.AssignedUsers = new List<EmployeeSubTaskUserDto>();
        //                                             var assignUsersVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignSubTaskUsers);
        //                                             subTaskVM.AssignedUsers = assignUsersVMList;
        //                                         }
        //                                         if (childTasks != null && childTasks.Count() > 0)
        //                                         {
        //                                             var childTaskList = childTasks.Where(t => t.EmployeeSubTaskId == subTaskVM.Id).ToList();

        //                                             subTaskVM.ChildTasks = new List<EmployeeChildTaskVM>();
        //                                             if (childTaskList != null && childTaskList.Count() > 0)
        //                                             {

        //                                                 var childTaskVMList = _mapper.Map<List<EmployeeChildTaskVM>>(childTaskList);

        //                                                 foreach (var childTaskVM in childTaskVMList)
        //                                                 {
        //                                                     var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskVM.Id);
        //                                                     //childTaskVM.Duration = childTaskTotalDuration;

        //                                                     // var h3 = childTaskTotalDuration / y;
        //                                                     // var m3 = (childTaskTotalDuration - (h3 * y)) / (y / 60);
        //                                                     // var s3 = (childTaskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
        //                                                     // // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                                                     // childTaskVM.Seconds = s3;
        //                                                     // childTaskVM.Minutes = m3;
        //                                                     // childTaskVM.Hours = h3;

        //                                                     var assignChildTaskUsers = _employeeChildTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
        //                                                     if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
        //                                                     {
        //                                                         childTaskVM.AssignedUsers = new List<EmployeeChildTaskUserDto>();
        //                                                         var assignUsersVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
        //                                                         childTaskVM.AssignedUsers = assignUsersVMList;
        //                                                     }
        //                                                 }
        //                                                 subTaskVM.ChildTasks = childTaskVMList;
        //                                             }
        //                                         }
        //                                     }
        //                                     taskObj.SubTasks = subTaskVMList;
        //                                 }
        //                             }

        //                         }
        //                         var taskSorting = taskVMList.OrderBy(t => t.Priority).ToList();
        //                         item.Tasks = taskSorting;
        //                         if (model.ShortColumn != null && model.ShortColumn != "")
        //                         {
        //                             var taskTempList = ShortTaskByColumn(model.ShortColumn, model.ShortOrder, item.Tasks);
        //                             item.Tasks = taskTempList;
        //                         }

        //                         // item.Tasks = taskVMList;
        //                     }
        //                     else
        //                     {
        //                         item.Tasks = new List<EmployeeTaskVM>();
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     // End Logic

        //     // Start Logic for Tasks without Project
        //     List<EmployeeTask> employeeTaskList = new List<EmployeeTask>();

        //     employeeTaskList = _employeeTaskService.GetAllTaskWithOutProjectAndSection(TenantId);


        //     var taskIdList1 = employeeTaskList.Select(t => t.Id).ToList();

        //     var subTasks1 = _employeeSubTaskService.GetAllActiveByTaskIds(taskIdList1);
        //     var subTaskIdList1 = subTasks1.Select(t => t.Id).ToList();
        //     var childTasks1 = _employeeChildTaskService.GetAllActiveBySubTaskIds(subTaskIdList1);
        //     // var taskWeClappIdList1 = taskWithOutProjectList.Select(t => t.WeClappTimeRecordId).ToList();
        //     // var taskRecordExist1 = timeRecords.Where(t => taskWeClappIdList1.Contains(Convert.ToInt64(t.id))).Select(t => t.id).ToList();

        //     var FinalTaskList1 = employeeTaskList.ToList();
        //     if (FinalTaskList1 != null && FinalTaskList1.Count() > 0)
        //     {
        //         employeeSectionTaskListVMObj.Tasks = new List<EmployeeTaskVM>();
        //         var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(FinalTaskList1);
        //         if (taskVMList != null && taskVMList.Count() > 0)
        //         {
        //             if (model.FilterTaskDescription != null)
        //             {
        //                 taskVMList = taskVMList.Where(t => t.Description.ToLower().Contains(model.FilterTaskDescription.ToLower())).ToList();
        //             }
        //             if (model.StatusId != null)
        //             {
        //                 taskVMList = taskVMList.Where(t => t.StatusId == model.StatusId).ToList();
        //             }
        //             foreach (var taskObj in taskVMList)
        //             {
        //                 if (taskObj.Id != null)
        //                 {
        //                     var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
        //                     //taskObj.Duration = taskTotalDuration;

        //                     // var h = taskTotalDuration / y;
        //                     // var m = (taskTotalDuration - (h * y)) / (y / 60);
        //                     // var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
        //                     // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                     // taskObj.Seconds = s;
        //                     // taskObj.Minutes = m;
        //                     // taskObj.Hours = h;
        //                     var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
        //                     if (assignTaskUsers != null && assignTaskUsers.Count > 0)
        //                     {
        //                         taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
        //                         var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        //                         taskObj.AssignedUsers = assignUsersVMList;
        //                     }

        //                     //taskObj.AssignUserCount = assignTaskUsers.Count();
        //                 }
        //                 if (subTasks1 != null && subTasks1.Count() > 0)
        //                 {
        //                     var subTaskList = subTasks1.Where(t => t.EmployeeTaskId == taskObj.Id).ToList();
        //                     taskObj.SubTasks = new List<EmployeeSubTaskVM>();
        //                     if (subTaskList != null && subTaskList.Count() > 0)
        //                     {
        //                         var subTaskVMList = _mapper.Map<List<EmployeeSubTaskVM>>(subTaskList);
        //                         foreach (var subTaskVM in subTaskVMList)
        //                         {
        //                             var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(subTaskVM.Id);
        //                             //subTaskVM.Duration = subTaskTotalDuration;

        //                             // var hh = subTaskTotalDuration / y;
        //                             // var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
        //                             // var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
        //                             // var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

        //                             // subTaskVM.Seconds = ss;
        //                             // subTaskVM.Minutes = mm;
        //                             // subTaskVM.Hours = hh;

        //                             var assignSubTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
        //                             if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
        //                             {
        //                                 subTaskVM.AssignedUsers = new List<EmployeeSubTaskUserDto>();
        //                                 var assignUsersVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignSubTaskUsers);
        //                                 subTaskVM.AssignedUsers = assignUsersVMList;
        //                             }
        //                             if (childTasks1 != null && childTasks1.Count() > 0)
        //                             {
        //                                 var childTaskList = childTasks1.Where(t => t.EmployeeSubTaskId == subTaskVM.Id).ToList();

        //                                 subTaskVM.ChildTasks = new List<EmployeeChildTaskVM>();
        //                                 if (childTaskList != null && childTaskList.Count() > 0)
        //                                 {

        //                                     var childTaskVMList = _mapper.Map<List<EmployeeChildTaskVM>>(childTaskList);

        //                                     foreach (var childTaskVM in childTaskVMList)
        //                                     {
        //                                         var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskVM.Id);
        //                                         //childTaskVM.Duration = childTaskTotalDuration;

        //                                         // var h3 = childTaskTotalDuration / y;
        //                                         // var m3 = (childTaskTotalDuration - (h3 * y)) / (y / 60);
        //                                         // var s3 = (childTaskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
        //                                         // // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                                         // childTaskVM.Seconds = s3;
        //                                         // childTaskVM.Minutes = m3;
        //                                         // childTaskVM.Hours = h3;

        //                                         var assignChildTaskUsers = _employeeChildTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
        //                                         if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
        //                                         {
        //                                             childTaskVM.AssignedUsers = new List<EmployeeChildTaskUserDto>();
        //                                             var assignUsersVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
        //                                             childTaskVM.AssignedUsers = assignUsersVMList;
        //                                         }
        //                                     }
        //                                     subTaskVM.ChildTasks = childTaskVMList;
        //                                 }
        //                             }
        //                         }

        //                         taskObj.SubTasks = subTaskVMList;
        //                     }
        //                 }

        //             }
        //             var taskSorting = taskVMList.OrderBy(t => t.Priority).ToList();
        //             employeeSectionTaskListVMObj.Tasks = taskSorting;

        //             // Sort Task based on column
        //             if (model.ShortColumn != "" && model.ShortColumn != null)
        //             {
        //                 var taskTempList = ShortTaskByColumn(model.ShortColumn, model.ShortOrder, employeeSectionTaskListVMObj.Tasks);
        //                 employeeSectionTaskListVMObj.Tasks = taskTempList;
        //             }
        //         }
        //         // taskListVM.Tasks = taskVMList;
        //     }
        //     // End Logic

        //     // if (model.FilterTaskDescription != null) {
        //     //     ProjectList = ProjectList.Where (t => t.Tasks.Count () > 0).ToList ();
        //     // }
        //     // ProjectList = ProjectList.Where (t => t.Tasks.Count () > 0).ToList ();
        //     employeeSectionVMList = employeeSectionVMList.OrderBy(t => t.Priority).ToList();
        //     employeeSectionTaskListVMObj.Sections = employeeSectionVMList;

        //     return new OperationResult<EmployeeSectionTaskListVM>(true, System.Net.HttpStatusCode.OK, "", employeeSectionTaskListVMObj);
        // }

        // Delete section with task delete or not option
        // Pass IsKeepTasks = true for section delete and task as it is
        // Pass IsKeepTasks = false for section and task both delete

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpDelete]
        // public async Task<OperationResult<Section>> Remove([FromBody] SectionDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     Section sectionObj = new Section();
        //     if (model.Id != null)
        //     {
        //         var SectionId = model.Id.Value;
        //         var tasks = _employeeTaskService.GetAllTaskBySection(SectionId);
        //         var tasksWithOutSection = _employeeTaskService.GetAllTaskByTenantWithOutSectionId(TenantId);
        //         var taskWithOutSectionCount = 0;
        //         if (tasksWithOutSection != null)
        //         {
        //             taskWithOutSectionCount = tasksWithOutSection.Count();
        //         }

        //         if (model.IsKeepTasks == true)
        //         {
        //             if (tasks != null && tasks.Count() > 0)
        //             {
        //                 foreach (var taskObj in tasks)
        //                 {
        //                     taskWithOutSectionCount = taskWithOutSectionCount + 1;
        //                     taskObj.SectionId = null;
        //                     taskObj.Priority = taskWithOutSectionCount;
        //                     taskObj.UpdatedBy = UserId;
        //                     var UpdatedTask = await _employeeTaskService.UpdateTask(taskObj, taskObj.Id);

        //                     EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
        //                     employeeTaskActivityObj.EmployeeTaskId = UpdatedTask.Id;
        //                     employeeTaskActivityObj.UserId = UserId;
        //                     // EmployeeTaskActivity.ProjectId = ProjectId;
        //                     employeeTaskActivityObj.Activity = "Removed this task from Section";
        //                     var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             if (tasks != null && tasks.Count() > 0)
        //             {
        //                 foreach (var taskObj in tasks)
        //                 {
        //                     var employeeTaskId = taskObj.Id;

        //                     var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(employeeTaskId);

        //                     if (subTasks != null && subTasks.Count() > 0)
        //                     {

        //                         foreach (var subTask in subTasks)
        //                         {
        //                             var subTaskId = subTask.Id;

        //                             var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);

        //                             if (childTasks != null && childTasks.Count() > 0)
        //                             {

        //                                 foreach (var item in childTasks)
        //                                 {
        //                                     var childTaskId = item.Id;

        //                                     var childDocuments = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

        //                                     // Remove child task documents from folder
        //                                     if (childDocuments != null && childDocuments.Count() > 0)
        //                                     {
        //                                         foreach (var childTaskDoc in childDocuments)
        //                                         {

        //                                             //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
        //                                             var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
        //                                             var filePath = dirPath + "\\" + childTaskDoc.Name;

        //                                             if (System.IO.File.Exists(filePath))
        //                                             {
        //                                                 System.IO.File.Delete(Path.Combine(filePath));
        //                                             }
        //                                         }
        //                                     }

        //                                     var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

        //                                     var childTimeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(childTaskId);

        //                                     var childTaskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(childTaskId);

        //                                     EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
        //                                     employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
        //                                     employeeChildTaskActivityObj.UserId = UserId;
        //                                     employeeChildTaskActivityObj.Activity = "Removed the task";
        //                                     var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

        //                                     var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

        //                                     var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);


        //                                 }
        //                             }

        //                             var subDocuments = await _employeeSubTaskAttachmentService.DeleteAttachmentByEmployeeSubTaskId(subTaskId);

        //                             // Remove sub task documents from folder
        //                             if (subDocuments != null && subDocuments.Count() > 0)
        //                             {
        //                                 foreach (var subTaskDoc in subDocuments)
        //                                 {

        //                                     //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
        //                                     var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
        //                                     var filePath = dirPath + "\\" + subTaskDoc.Name;

        //                                     if (System.IO.File.Exists(filePath))
        //                                     {
        //                                         System.IO.File.Delete(Path.Combine(filePath));
        //                                     }
        //                                 }
        //                             }

        //                             var subComments = await _employeeSubTaskCommentService.DeleteCommentByEmployeeSubTaskId(subTaskId);

        //                             var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

        //                             var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(subTaskId);

        //                             EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
        //                             employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskId;
        //                             employeeSubTaskActivityObj.UserId = UserId;
        //                             employeeSubTaskActivityObj.Activity = "Removed the task";
        //                             var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

        //                             var subTaskActivities = await _employeeSubTaskActivityService.DeleteByEmployeeSubTaskId(subTaskId);

        //                             var subTaskToDelete = await _employeeSubTaskService.Delete(subTaskId);

        //                         }
        //                     }

        //                     var documents = await _employeeTaskAttachmentService.DeleteAttachmentByTaskId(employeeTaskId);

        //                     // Remove task documents from folder
        //                     if (documents != null && documents.Count() > 0)
        //                     {
        //                         foreach (var taskDoc in documents)
        //                         {

        //                             //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
        //                             var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
        //                             var filePath = dirPath + "\\" + taskDoc.Name;

        //                             if (System.IO.File.Exists(filePath))
        //                             {
        //                                 System.IO.File.Delete(Path.Combine(filePath));
        //                             }
        //                         }
        //                     }

        //                     var comments = await _employeeTaskCommentService.DeleteCommentByEmployeeTaskId(employeeTaskId);

        //                     var timeRecords = await _employeeTaskTimeRecordService.DeleteTimeRecordByTaskId(employeeTaskId);

        //                     var taskUsers = await _employeeTaskUserService.DeleteByEmployeeTaskId(employeeTaskId);
        //                     EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
        //                     employeeTaskActivityObj.EmployeeTaskId = employeeTaskId;
        //                     employeeTaskActivityObj.UserId = UserId;
        //                     employeeTaskActivityObj.Activity = "Removed this task";
        //                     var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);

        //                     var taskActivities = await _employeeTaskActivityService.DeleteByEmployeeTaskId(employeeTaskId);

        //                     var taskToDelete = await _employeeTaskService.Delete(employeeTaskId);



        //                 }
        //             }
        //         }

        //         var AddUpdate = await _sectionService.DeleteSection(model.Id.Value);

        //         SectionActivity sectionActivityObj = new SectionActivity();
        //         sectionActivityObj.SectionId = AddUpdate.Id;
        //         sectionActivityObj.Activity = "Removed this Section";
        //         sectionActivityObj.UserId = UserId;
        //         var SectionActivityAddUpdateObj = await _sectionActivityService.CheckInsertOrUpdate(sectionActivityObj);

        //         return new OperationResult<Section>(true, System.Net.HttpStatusCode.OK, "Section delete successfully", AddUpdate);
        //     }
        //     else
        //     {
        //         return new OperationResult<Section>(false, System.Net.HttpStatusCode.OK, "Section id null", sectionObj);
        //     }
        // }

        // get section using section id
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<EmployeeSectionVM>> GetSectionById(long Id)
        {
            EmployeeSectionVM employeeSectionVMObj = new EmployeeSectionVM();
            var sectionObj = _sectionService.GetSectionById(Id);
            if (sectionObj != null)
            {
                employeeSectionVMObj = _mapper.Map<EmployeeSectionVM>(sectionObj);

                var y = 60 * 60 * 1000;
                if (employeeSectionVMObj.Id != null)
                {
                    var SectionId = employeeSectionVMObj.Id.Value;

                    var taskList = _employeeTaskService.GetAllTaskBySection(SectionId);

                    if (taskList != null && taskList.Count() > 0)
                    {
                        employeeSectionVMObj.Tasks = new List<EmployeeTaskVM>();
                        var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(taskList);
                        foreach (var taskObj in taskVMList)
                        {
                            if (taskObj.Id != null)
                            {
                                var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
                                //taskObj.Duration = taskTotalDuration;

                                // var h = taskTotalDuration / y;
                                // var m = (taskTotalDuration - (h * y)) / (y / 60);
                                // var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;

                                // taskObj.Seconds = s;
                                // taskObj.Minutes = m;
                                // taskObj.Hours = h;
                                var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
                                if (assignTaskUsers.Count > 0)
                                {
                                    taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
                                    var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
                                    taskObj.AssignedUsers = assignUsersVMList;
                                }
                            }
                        }
                        employeeSectionVMObj.Tasks = taskVMList;
                    }
                }
                return new OperationResult<EmployeeSectionVM>(true, System.Net.HttpStatusCode.OK, "", employeeSectionVMObj);
            }
            else
            {
                return new OperationResult<EmployeeSectionVM>(false, System.Net.HttpStatusCode.OK, "Section not found", employeeSectionVMObj);
            }
        }

        private List<EmployeeTaskVM> ShortTaskByColumn(string ShortColumn, string ShortOrder, List<EmployeeTaskVM> taskList)
        {
            List<EmployeeTaskVM> employeeTaskVMList = new List<EmployeeTaskVM>();
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn == "Description")
                {
                    if (ShortOrder == "Asc")
                    {
                        employeeTaskVMList = taskList.OrderBy(t => t.Description).ToList();
                    }
                    else
                    {
                        employeeTaskVMList = taskList.OrderByDescending(t => t.Description).ToList();
                    }
                }
                else if (ShortColumn == "Duration")
                {
                    // if (ShortOrder == "Asc")
                    // {
                    //     employeeTaskVMList = taskList.OrderBy(t => t.Duration).ToList();
                    // }
                    // else
                    // {
                    //     employeeTaskVMList = taskList.OrderByDescending(t => t.Duration).ToList();
                    // }
                }
                else if (ShortColumn == "Due Date")
                {
                    if (ShortOrder == "Asc")
                    {
                        employeeTaskVMList = taskList.OrderBy(t => t.EndDate).ToList();
                    }
                    else
                    {
                        employeeTaskVMList = taskList.OrderByDescending(t => t.EndDate).ToList();
                    }
                }
                else if (ShortColumn == "Assignee")
                {
                    // if (ShortOrder == "Asc")
                    // {
                    //     employeeTaskVMList = taskList.OrderBy(t => t.AssignUserCount).ToList();
                    // }
                    // else
                    // {
                    //     employeeTaskVMList = taskList.OrderByDescending(t => t.AssignUserCount).ToList();
                    // }
                }
            }

            return employeeTaskVMList;
        }


        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut("Priority")]
        public async Task<OperationResult<SectionDto>> UpdatePriority([FromBody] SectionDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            SectionDto sectionDtoObj = new SectionDto();
            SectionActivity sectionActivityObj = new SectionActivity();
            if (model.Id != null)
            {
                // start logic for Update Current task with priority
                var sectionObj = _sectionService.GetSectionById(model.Id.Value);
                sectionObj.Priority = model.CurrentPriority;
                sectionObj.UpdatedBy = UserId;

                sectionActivityObj.Activity = "Priority changed for this section. ";

                var sectionAddUpdate = await _sectionService.UpdateSection(sectionObj, sectionObj.Id);

                sectionActivityObj.SectionId = model.Id;
                sectionActivityObj.UserId = UserId;

                var AddUpdate = await _sectionActivityService.CheckInsertOrUpdate(sectionActivityObj);
                // End Logic


                // start logic for task move in with out section list
                var SectionList = _sectionService.GetAllByTenant(TenantId);

                if (model.CurrentPriority < SectionList.Count())
                {
                    if (model.CurrentPriority != model.PreviousPriority)
                    {
                        if (model.PreviousPriority < model.CurrentPriority)
                        {
                            if (SectionList != null && SectionList.Count() > 0)
                            {
                                var sections = SectionList.Where(t => t.Priority > model.PreviousPriority && t.Priority <= model.CurrentPriority && t.Id != model.Id).ToList();
                                if (sections != null && sections.Count() > 0)
                                {
                                    foreach (var item in sections)
                                    {
                                        item.Priority = item.Priority - 1;
                                        await _sectionService.UpdateSection(item, item.Id);
                                    }
                                }
                            }
                        }
                        else if (model.PreviousPriority > model.CurrentPriority)
                        {
                            var sections = SectionList.Where(t => t.Priority < model.PreviousPriority && t.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
                            if (sections != null && sections.Count() > 0)
                            {
                                foreach (var item in sections)
                                {
                                    item.Priority = item.Priority + 1;
                                    await _sectionService.UpdateSection(item, item.Id);
                                }
                            }
                        }
                    }

                    // end
                }
            }
            return new OperationResult<SectionDto>(true, System.Net.HttpStatusCode.OK, "", sectionDtoObj);
        }
    }
}