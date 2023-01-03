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
    [Authorize]
    [Route("[controller]/[action]")]
    public class SectionController : Controller
    {
        private readonly ISectionService _sectionService;
        private readonly IUserService _userService;
        private readonly ISectionActivityService _sectionActivityService;
        private readonly IOneClappTaskService _taskService;
        private readonly IOneClappSubTaskService _subTaskService;
        private readonly IOneClappChildTaskService _childTaskService;
        private readonly IOneClappTaskUserSerivce _taskUserService;
        private readonly IOneClappSubTaskUserService _subTaskUserService;
        private readonly IOneClappChildTaskUserService _childTaskUserService;
        private readonly ITaskTimeRecordService _taskTimeRecordService;
        private readonly ISubTaskTimeRecordService _subTaskTimeRecordService;
        private readonly IChildTaskTimeRecordService _childTaskTimeRecordService;
        private readonly ITaskAttachmentService _taskAttachmentService;
        private readonly ISubTaskAttachmentService _subTaskAttachmentService;
        private readonly IChildTaskAttachmentService _childTaskAttachmentService;
        private readonly ITaskCommentService _taskCommentService;
        private readonly ISubTaskCommentService _subTaskCommentService;
        private readonly IChildTaskCommentService _childTaskCommentService;
        private readonly ITaskActivityService _taskActivityService;
        private readonly ISubTaskActivityService _subTaskActivityService;
        private readonly IChildTaskActivityService _childTaskActivityService;
        private readonly IWeClappService _weClappService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public SectionController(
            ISectionService sectionService,
            IUserService userService,
            ISectionActivityService sectionActivityService,
            IOneClappTaskService taskService,
            IOneClappSubTaskService subTaskService,
            IOneClappChildTaskService childTaskService,
            IOneClappTaskUserSerivce taskUserSerivce,
            IOneClappSubTaskUserService subTaskUserSerivce,
            IOneClappChildTaskUserService childTaskUserService,
            ITaskTimeRecordService taskTimeRecordService,
            ISubTaskTimeRecordService subTaskTimeRecordService,
            IChildTaskTimeRecordService childTaskTimeRecordService,
            ITaskAttachmentService taskAttachmentService,
            ISubTaskAttachmentService subTaskAttachmentService,
            IChildTaskAttachmentService childTaskAttachmentService,
            ITaskCommentService taskCommentService,
            ISubTaskCommentService subTaskCommentService,
            IChildTaskCommentService childTaskCommentService,
            ITaskActivityService taskActivityService,
            ISubTaskActivityService subTaskActivityService,
            IChildTaskActivityService childTaskActivityService,
            IWeClappService weClappService,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper,
            OneClappContext context
        )
        {
            _sectionService = sectionService;
            _userService = userService;
            _sectionActivityService = sectionActivityService;
            _taskService = taskService;
            _subTaskService = subTaskService;
            _childTaskService = childTaskService;
            _taskUserService = taskUserSerivce;
            _subTaskUserService = subTaskUserSerivce;
            _childTaskUserService = childTaskUserService;
            _taskTimeRecordService = taskTimeRecordService;
            _subTaskTimeRecordService = subTaskTimeRecordService;
            _childTaskTimeRecordService = childTaskTimeRecordService;
            _taskAttachmentService = taskAttachmentService;
            _subTaskAttachmentService = subTaskAttachmentService;
            _childTaskAttachmentService = childTaskAttachmentService;
            _taskCommentService = taskCommentService;
            _subTaskCommentService = subTaskCommentService;
            _childTaskCommentService = childTaskCommentService;
            _taskActivityService = taskActivityService;
            _subTaskActivityService = subTaskActivityService;
            _childTaskActivityService = childTaskActivityService;
            _weClappService = weClappService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _context = context;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<Section>> Add([FromBody] SectionDto model)
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
            sectionActivityObj.Activity = "Created section";
            sectionActivityObj.UserId = UserId;
            var sectionActivityObj1 = _sectionActivityService.CheckInsertOrUpdate(sectionActivityObj);
            return new OperationResult<Section>(true, System.Net.HttpStatusCode.OK, "Section add successfully", AddUpdate);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<Section>> Update([FromBody] SectionDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            Section sectionObj = new Section();
            var existingItem = _sectionService.GetSectionById(model.Id.Value);
            if (existingItem != null)
            {
                existingItem.Name = model.Name;
                existingItem.UpdatedBy = UserId;
                var AddUpdate = await _sectionService.UpdateSection(existingItem, existingItem.Id);

                SectionActivity sectionActivityObj = new SectionActivity();
                sectionActivityObj.SectionId = AddUpdate.Id;
                sectionActivityObj.Activity = "Updated section";
                sectionActivityObj.UserId = UserId;
                var sectionActivityAddUpdateObj = _sectionActivityService.CheckInsertOrUpdate(sectionActivityObj);

                return new OperationResult<Section>(true, System.Net.HttpStatusCode.OK, "Section Updated successfully", AddUpdate);
            }
            return new OperationResult<Section>(false, System.Net.HttpStatusCode.OK, "Error", sectionObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<TaskListVM>> List([FromBody] TicketDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            TaskListVM taskListVMObj = new TaskListVM();

            List<SectionVM> sectionVMList = new List<SectionVM>();
            var y = 60 * 60 * 1000;
            // var sections = _sectionService.GetAllByTenant (TenantId);
            var ticketNumber = Convert.ToInt64(model.TicketNumber);
            var sections = _sectionService.GetAllByTenantAndTicket(TenantId, ticketNumber);
            sectionVMList = _mapper.Map<List<SectionVM>>(sections);

            List<TimeRecord> timeRecordList = await _weClappService.GetTimeRecords(model.ApiKey, model.Tenant);
            if (timeRecordList != null && timeRecordList.Count() > 0)
            {
                timeRecordList = timeRecordList.Where(t => t.ticketNumber == model.TicketNumber).ToList();
            }

            // Start logic for Task with Section
            if (sectionVMList != null && sectionVMList.Count() > 0)
            {
                var tasks = _taskService.GetAllActiveByTenant(TenantId);
                var taskIdList = tasks.Select(t => t.Id).ToList();
                var subTasks = _subTaskService.GetAllActiveByTaskIds(taskIdList);
                var subTaskIdList = subTasks.Select(t => t.Id).ToList();
                var childTasks = _childTaskService.GetAllActiveBySubTaskIds(subTaskIdList);

                foreach (var item in sectionVMList)
                {
                    if (item.Id != null)
                    {
                        var taskList = _taskService.GetAllTaskBySectionId(item.Id.Value);
                        if (taskList != null && taskList.Count() > 0)
                        {
                            if (model.FilterTaskDescription != null)
                            {
                                taskList = taskList.Where(t => t.Description.ToLower().Contains(model.FilterTaskDescription.ToLower())).ToList();
                            }
                            if (model.StatusId != null)
                            {
                                taskList = taskList.Where(t => t.StatusId == model.StatusId).ToList();
                            }
                            var taskWeClappIdList = taskList.Select(t => t.WeClappTimeRecordId).ToList();

                            var taskRecordExist = timeRecordList.Where(t => taskWeClappIdList.Contains(Convert.ToInt64(t.id))).Select(t => t.id).ToList();

                            var FinalTaskList = taskList.Where(t => taskRecordExist.Contains(t.WeClappTimeRecordId.ToString())).ToList();

                            if (FinalTaskList != null && FinalTaskList.Count() > 0)
                            {
                                item.Tasks = new List<OneClappTaskVM>();
                                var taskVMList = _mapper.Map<List<OneClappTaskVM>>(FinalTaskList);
                                foreach (var taskObj in taskVMList)
                                {
                                    var taskTotalDuration = _taskTimeRecordService.GetTotalTaskTimeRecord(taskObj.Id);
                                    taskObj.Duration = taskTotalDuration;

                                    var h = taskTotalDuration / y;
                                    var m = (taskTotalDuration - (h * y)) / (y / 60);
                                    var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
                                    var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                    taskObj.Seconds = s;
                                    taskObj.Minutes = m;
                                    taskObj.Hours = h;
                                    var assignTaskUsers = _taskUserService.GetAssignUsersByTask(taskObj.Id);
                                    if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                                    {
                                        taskObj.AssignedUsers = new List<OneClappTaskUserDto>();
                                        var assignUsersVMList = _mapper.Map<List<OneClappTaskUserDto>>(assignTaskUsers);
                                        taskObj.AssignedUsers = assignUsersVMList;
                                    }
                                    taskObj.AssignUserCount = assignTaskUsers.Count();
                                    if (subTasks != null && subTasks.Count() > 0)
                                    {
                                        var subTaskList = subTasks.Where(t => t.OneClappTaskId == taskObj.Id).ToList();
                                        taskObj.SubTasks = new List<OneClappSubTaskVM>();
                                        if (subTaskList != null && subTaskList.Count() > 0)
                                        {
                                            var subTaskVMList = _mapper.Map<List<OneClappSubTaskVM>>(subTaskList);
                                            foreach (var subTaskVM in subTaskVMList)
                                            {
                                                var subTaskTotalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(subTaskVM.Id);
                                                subTaskVM.Duration = subTaskTotalDuration;

                                                var hh = subTaskTotalDuration / y;
                                                var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
                                                var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
                                                var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

                                                subTaskVM.Seconds = ss;
                                                subTaskVM.Minutes = mm;
                                                subTaskVM.Hours = hh;

                                                var assignSubTaskUsers = _subTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
                                                if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
                                                {
                                                    subTaskVM.AssignedUsers = new List<OneClappSubTaskUserDto>();
                                                    var assignUsersVMList = _mapper.Map<List<OneClappSubTaskUserDto>>(assignSubTaskUsers);
                                                    subTaskVM.AssignedUsers = assignUsersVMList;
                                                }
                                                if (childTasks != null && childTasks.Count() > 0)
                                                {
                                                    var childTaskList = childTasks.Where(t => t.OneClappSubTaskId == subTaskVM.Id).ToList();

                                                    subTaskVM.ChildTasks = new List<OneClappChildTaskVM>();
                                                    if (childTaskList != null && childTaskList.Count() > 0)
                                                    {

                                                        var childTaskVMList = _mapper.Map<List<OneClappChildTaskVM>>(childTaskList);

                                                        foreach (var childTaskVM in childTaskVMList)
                                                        {
                                                            var childTaskTotalDuration = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(childTaskVM.Id);
                                                            childTaskVM.Duration = childTaskTotalDuration;

                                                            var h3 = childTaskTotalDuration / y;
                                                            var m3 = (childTaskTotalDuration - (h3 * y)) / (y / 60);
                                                            var s3 = (childTaskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
                                                            // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                                            childTaskVM.Seconds = s3;
                                                            childTaskVM.Minutes = m3;
                                                            childTaskVM.Hours = h3;

                                                            var assignChildTaskUsers = _childTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
                                                            if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
                                                            {
                                                                childTaskVM.AssignedUsers = new List<OneClappChildTaskUserDto>();
                                                                var assignUsersVMList = _mapper.Map<List<OneClappChildTaskUserDto>>(assignChildTaskUsers);
                                                                childTaskVM.AssignedUsers = assignUsersVMList;
                                                            }
                                                        }
                                                        subTaskVM.ChildTasks = childTaskVMList;
                                                    }
                                                }
                                            }

                                            taskObj.SubTasks = subTaskVMList;
                                        }
                                    }
                                }
                                var taskSorting = taskVMList.OrderBy(t => t.Priority.Value).ToList();
                                item.Tasks = taskSorting;
                                if (model.ShortColumn != null && model.ShortColumn != "")
                                {
                                    var taskTempList = ShortTaskByColumn(model.ShortColumn, model.ShortOrder, item.Tasks);
                                    item.Tasks = taskTempList;
                                }

                                // item.Tasks = taskVMList;
                            }
                            else
                            {
                                item.Tasks = new List<OneClappTaskVM>();
                            }
                        }
                    }
                }
            }
            // End Logic

            // Start Logic for Tasks without section
            var taskWithOutSectionList = _taskService.GetAllTaskByTenantWithOutSection(TenantId);
            var taskIdList1 = taskWithOutSectionList.Select(t => t.Id).ToList();
            var subTasks1 = _subTaskService.GetAllActiveByTaskIds(taskIdList1);
            var subTaskIdList1 = subTasks1.Select(t => t.Id).ToList();
            var childTasks1 = _childTaskService.GetAllActiveBySubTaskIds(subTaskIdList1);
            var taskWeClappIdList1 = taskWithOutSectionList.Select(t => t.WeClappTimeRecordId).ToList();
            var taskRecordExist1 = timeRecordList.Where(t => taskWeClappIdList1.Contains(Convert.ToInt64(t.id))).Select(t => t.id).ToList();

            var FinalTaskList1 = taskWithOutSectionList.Where(t => taskRecordExist1.Contains(t.WeClappTimeRecordId.ToString())).ToList();
            if (FinalTaskList1 != null && FinalTaskList1.Count() > 0)
            {
                taskListVMObj.Tasks = new List<OneClappTaskVM>();
                var taskVMList = _mapper.Map<List<OneClappTaskVM>>(FinalTaskList1);
                if (taskVMList != null && taskVMList.Count() > 0)
                {
                    if (model.FilterTaskDescription != null)
                    {
                        taskVMList = taskVMList.Where(t => t.Description.ToLower().Contains(model.FilterTaskDescription.ToLower())).ToList();
                    }
                    if (model.StatusId != null)
                    {
                        taskVMList = taskVMList.Where(t => t.StatusId == model.StatusId).ToList();
                    }
                    foreach (var taskObj in taskVMList)
                    {
                        var taskTotalDuration = _taskTimeRecordService.GetTotalTaskTimeRecord(taskObj.Id);
                        taskObj.Duration = taskTotalDuration;

                        var h = taskTotalDuration / y;
                        var m = (taskTotalDuration - (h * y)) / (y / 60);
                        var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
                        var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                        taskObj.Seconds = s;
                        taskObj.Minutes = m;
                        taskObj.Hours = h;
                        var assignTaskUsers = _taskUserService.GetAssignUsersByTask(taskObj.Id);
                        if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                        {
                            taskObj.AssignedUsers = new List<OneClappTaskUserDto>();
                            var assignUsersVMList = _mapper.Map<List<OneClappTaskUserDto>>(assignTaskUsers);
                            taskObj.AssignedUsers = assignUsersVMList;
                        }

                        taskObj.AssignUserCount = assignTaskUsers.Count();
                        if (subTasks1 != null && subTasks1.Count() > 0)
                        {
                            var subTaskList = subTasks1.Where(t => t.OneClappTaskId == taskObj.Id).ToList();
                            taskObj.SubTasks = new List<OneClappSubTaskVM>();
                            if (subTaskList != null && subTaskList.Count() > 0)
                            {
                                var subTaskVMList = _mapper.Map<List<OneClappSubTaskVM>>(subTaskList);
                                foreach (var subTaskVM in subTaskVMList)
                                {
                                    var subTaskTotalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(subTaskVM.Id);
                                    subTaskVM.Duration = subTaskTotalDuration;

                                    var hh = subTaskTotalDuration / y;
                                    var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
                                    var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
                                    var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

                                    subTaskVM.Seconds = ss;
                                    subTaskVM.Minutes = mm;
                                    subTaskVM.Hours = hh;

                                    var assignSubTaskUsers = _subTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
                                    if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
                                    {
                                        subTaskVM.AssignedUsers = new List<OneClappSubTaskUserDto>();
                                        var assignUsersVMList = _mapper.Map<List<OneClappSubTaskUserDto>>(assignSubTaskUsers);
                                        subTaskVM.AssignedUsers = assignUsersVMList;
                                    }

                                    if (childTasks1 != null && childTasks1.Count() > 0)
                                    {
                                        var childTaskList = childTasks1.Where(t => t.OneClappSubTaskId == subTaskVM.Id).ToList();

                                        subTaskVM.ChildTasks = new List<OneClappChildTaskVM>();
                                        if (childTaskList != null && childTaskList.Count() > 0)
                                        {

                                            var childTaskVMList = _mapper.Map<List<OneClappChildTaskVM>>(childTaskList);

                                            foreach (var childTaskVM in childTaskVMList)
                                            {
                                                var childTaskTotalDuration = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(childTaskVM.Id);
                                                childTaskVM.Duration = childTaskTotalDuration;

                                                var h3 = childTaskTotalDuration / y;
                                                var m3 = (childTaskTotalDuration - (h3 * y)) / (y / 60);
                                                var s3 = (childTaskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
                                                // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                                childTaskVM.Seconds = s3;
                                                childTaskVM.Minutes = m3;
                                                childTaskVM.Hours = h3;

                                                var assignChildTaskUsers = _childTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
                                                if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
                                                {
                                                    childTaskVM.AssignedUsers = new List<OneClappChildTaskUserDto>();
                                                    var assignUsersVMList = _mapper.Map<List<OneClappChildTaskUserDto>>(assignChildTaskUsers);
                                                    childTaskVM.AssignedUsers = assignUsersVMList;
                                                }
                                            }
                                            subTaskVM.ChildTasks = childTaskVMList;
                                        }
                                    }
                                }

                                taskObj.SubTasks = subTaskVMList;
                            }
                        }
                    }
                    var taskSorting = taskVMList.OrderBy(t => t.Priority.Value).ToList();
                    taskListVMObj.Tasks = taskSorting;

                    // Sort Task based on column
                    if (model.ShortColumn != "" && model.ShortColumn != null)
                    {
                        var taskTempList = ShortTaskByColumn(model.ShortColumn, model.ShortOrder, taskListVMObj.Tasks);
                        taskListVMObj.Tasks = taskTempList;
                    }
                    // taskListVM.Tasks = taskVMList;
                }
            }
            // End Logic

            // if (model.FilterTaskDescription != null) {
            //     SectionList = SectionList.Where (t => t.Tasks.Count () > 0).ToList ();
            // }
            sectionVMList = sectionVMList.Where(t => t.Tasks != null && t.Tasks.Count() > 0).ToList();
            taskListVMObj.Sections = sectionVMList;

            return new OperationResult<TaskListVM>(true, System.Net.HttpStatusCode.OK, "", taskListVMObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<SectionVM>>> ListWithSection([FromBody] TicketDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<SectionVM> sectionVMList = new List<SectionVM>();
            var y = 60 * 60 * 1000;
            var sections = _sectionService.GetAllByTenant(TenantId);
            sectionVMList = _mapper.Map<List<SectionVM>>(sections);

            if (sectionVMList != null && sectionVMList.Count() > 0)
            {
                var tasks = _taskService.GetAllActiveByTenant(TenantId);
                var taskIdList = tasks.Select(t => t.Id).ToList();
                var subTasks = _subTaskService.GetAllActiveByTaskIds(taskIdList);
                var subTaskIdList = subTasks.Select(t => t.Id).ToList();
                var childTasks = _childTaskService.GetAllActiveBySubTaskIds(subTaskIdList);

                List<TimeRecord> timeRecordList = await _weClappService.GetTimeRecords(model.ApiKey, model.Tenant);
                if (timeRecordList != null && timeRecordList.Count() > 0)
                {
                    timeRecordList = timeRecordList.Where(t => t.ticketNumber == model.TicketNumber).ToList();
                }

                foreach (var item in sectionVMList)
                {
                    if (item.Id != null)
                    {
                        var taskList = _taskService.GetAllTaskBySectionId(item.Id.Value);
                        var taskWeClappIdList = taskList.Select(t => t.WeClappTimeRecordId).ToList();

                        var taskRecordExist = timeRecordList.Where(t => taskWeClappIdList.Contains(Convert.ToInt64(t.id))).Select(t => t.id).ToList();

                        var FinalTaskList = taskList.Where(t => taskRecordExist.Contains(t.WeClappTimeRecordId.ToString())).ToList();

                        // foreach (var timeRecordObj in timeRecords) {
                        //     foreach (var taskWeClappId in taskWeClappIdList) {
                        //         if (Convert.ToInt64 (timeRecordObj.id) == taskWeClappId) {
                        //             var data = true;
                        //         }
                        //     }
                        // }

                        if (FinalTaskList != null && FinalTaskList.Count() > 0)
                        {
                            item.Tasks = new List<OneClappTaskVM>();
                            var taskVMList = _mapper.Map<List<OneClappTaskVM>>(FinalTaskList);
                            foreach (var taskObj in taskVMList)
                            {
                                var taskTotalDuration = _taskTimeRecordService.GetTotalTaskTimeRecord(taskObj.Id);
                                taskObj.Duration = taskTotalDuration;

                                var h = taskTotalDuration / y;
                                var m = (taskTotalDuration - (h * y)) / (y / 60);
                                var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
                                var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                taskObj.Seconds = s;
                                taskObj.Minutes = m;
                                taskObj.Hours = h;
                                var assignTaskUsers = _taskUserService.GetAssignUsersByTask(taskObj.Id);
                                if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                                {
                                    taskObj.AssignedUsers = new List<OneClappTaskUserDto>();
                                    var assignUsersVMList = _mapper.Map<List<OneClappTaskUserDto>>(assignTaskUsers);
                                    taskObj.AssignedUsers = assignUsersVMList;
                                }
                                if (subTasks != null && subTasks.Count() > 0)
                                {
                                    var subTaskList = subTasks.Where(t => t.OneClappTaskId == taskObj.Id).ToList();
                                    taskObj.SubTasks = new List<OneClappSubTaskVM>();
                                    if (subTaskList != null && subTaskList.Count() > 0)
                                    {
                                        var subTaskVMList = _mapper.Map<List<OneClappSubTaskVM>>(subTaskList);
                                        foreach (var subTaskVM in subTaskVMList)
                                        {
                                            var subTaskTotalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(subTaskVM.Id);
                                            subTaskVM.Duration = subTaskTotalDuration;

                                            var hh = subTaskTotalDuration / y;
                                            var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
                                            var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
                                            var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

                                            subTaskVM.Seconds = ss;
                                            subTaskVM.Minutes = mm;
                                            subTaskVM.Hours = hh;

                                            var assignSubTaskUsers = _subTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
                                            if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
                                            {
                                                subTaskVM.AssignedUsers = new List<OneClappSubTaskUserDto>();
                                                var assignUsersVMList = _mapper.Map<List<OneClappSubTaskUserDto>>(assignSubTaskUsers);
                                                subTaskVM.AssignedUsers = assignUsersVMList;
                                            }

                                            if (childTasks != null)
                                            {
                                                var childTaskList = childTasks.Where(t => t.OneClappSubTaskId == subTaskVM.Id).ToList();

                                                subTaskVM.ChildTasks = new List<OneClappChildTaskVM>();
                                                if (childTaskList != null && childTaskList.Count() > 0)
                                                {

                                                    var childTaskVMList = _mapper.Map<List<OneClappChildTaskVM>>(childTaskList);

                                                    foreach (var childTaskVM in childTaskVMList)
                                                    {
                                                        var childTaskTotalDuration = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(childTaskVM.Id);
                                                        childTaskVM.Duration = childTaskTotalDuration;

                                                        var h3 = taskTotalDuration / y;
                                                        var m3 = (taskTotalDuration - (h3 * y)) / (y / 60);
                                                        var s3 = (taskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
                                                        // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                                        childTaskVM.Seconds = s3;
                                                        childTaskVM.Minutes = m3;
                                                        childTaskVM.Hours = h3;

                                                        var assignChildTaskUsers = _childTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
                                                        if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
                                                        {
                                                            childTaskVM.AssignedUsers = new List<OneClappChildTaskUserDto>();
                                                            var assignUsersVMList = _mapper.Map<List<OneClappChildTaskUserDto>>(assignChildTaskUsers);
                                                            childTaskVM.AssignedUsers = assignUsersVMList;
                                                        }
                                                    }
                                                    subTaskVM.ChildTasks = childTaskVMList;
                                                }
                                            }
                                        }

                                        taskObj.SubTasks = subTaskVMList;
                                    }
                                }
                            }
                            item.Tasks = taskVMList;
                        }
                    }
                }
            }
            return new OperationResult<List<SectionVM>>(true, System.Net.HttpStatusCode.OK, "", sectionVMList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<Section>> Remove([FromBody] SectionDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var sectionObj = new Section();
            if (model.Id != null)
            {
                var sectionId = model.Id.Value;
                var tasks = _taskService.GetAllTaskBySectionId(sectionId);
                var tasksWithOutSection = _taskService.GetAllTaskByTenantWithOutSection(TenantId);
                var taskWithOutSectionCount = 0;
                if (tasksWithOutSection != null)
                {
                    taskWithOutSectionCount = tasksWithOutSection.Count();
                }

                if (model.IsKeepTasks == true)
                {
                    if (tasks != null && tasks.Count() > 0)
                    {
                        foreach (var taskObj in tasks)
                        {
                            taskWithOutSectionCount = taskWithOutSectionCount + 1;
                            taskObj.SectionId = null;
                            taskObj.Priority = taskWithOutSectionCount;
                            taskObj.UpdatedBy = model.UpdatedBy;
                            var UpdatedTask = _taskService.UpdateTask(taskObj, taskObj.Id);

                            TaskActivity taskActivityObj = new TaskActivity();
                            taskActivityObj.TaskId = UpdatedTask.Id;
                            taskActivityObj.UserId = model.UpdatedBy;
                            taskActivityObj.SectionId = sectionId;
                            taskActivityObj.Activity = "Removed this task from section";
                            var AddUpdateActivity = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);
                        }
                    }
                }
                else
                {
                    if (tasks != null && tasks.Count() > 0)
                    {
                        foreach (var taskObj in tasks)
                        {
                            var taskId = taskObj.Id;

                            var subTasks = _subTaskService.GetAllSubTaskByTask(taskId);
                            if (subTasks != null && subTasks.Count() > 0)
                            {
                                foreach (var subTask in subTasks)
                                {
                                    var subTaskId = subTask.Id;

                                    var childTasks = _childTaskService.GetAllChildTaskBySubTask(subTaskId);

                                    if (childTasks != null && childTasks.Count() > 0)
                                    {
                                        foreach (var item in childTasks)
                                        {
                                            var childTaskId = item.Id;

                                            var childDocuments = await _childTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                                            // Remove child task documents from folder
                                            if (childDocuments != null && childDocuments.Count() > 0)
                                            {
                                                foreach (var childTaskDoc in childDocuments)
                                                {

                                                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                                                    var filePath = dirPath + "\\" + childTaskDoc.Name;

                                                    if (System.IO.File.Exists(filePath))
                                                    {
                                                        System.IO.File.Delete(Path.Combine(filePath));
                                                    }
                                                }
                                            }

                                            var childComments = _childTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                                            var childTimeRecords = _childTaskTimeRecordService.DeleteTimeRecordByChildTaskId(childTaskId);

                                            var childTaskUsers = _childTaskUserService.DeleteByChildTaskId(childTaskId);

                                            var childTaskActivities = _childTaskActivityService.DeleteChildTaskActivityByChildTaskId(childTaskId);

                                            var childTaskToDelete = _childTaskService.Delete(childTaskId);

                                            ChildTaskActivity childTaskActivityObj = new ChildTaskActivity();
                                            childTaskActivityObj.ChildTaskId = childTaskId;
                                            childTaskActivityObj.UserId = UserId;
                                            childTaskActivityObj.Activity = "Removed this child task";
                                            var AddUpdate1 = _childTaskActivityService.CheckInsertOrUpdate(childTaskActivityObj);
                                        }
                                    }

                                    var subDocuments = _subTaskAttachmentService.DeleteAttachmentBySubTaskId(subTaskId);

                                    // Remove sub task documents from folder
                                    if (subDocuments != null && subDocuments.Count() > 0)
                                    {
                                        foreach (var subTaskDoc in subDocuments)
                                        {

                                            //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
                                            var filePath = dirPath + "\\" + subTaskDoc.Name;

                                            if (System.IO.File.Exists(filePath))
                                            {
                                                System.IO.File.Delete(Path.Combine(filePath));
                                            }
                                        }
                                    }

                                    var subComments = _subTaskCommentService.DeleteCommentBySubTaskId(subTaskId);

                                    var subTimeRecords = _subTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                                    var subTaskUsers = _subTaskUserService.DeleteBySubTaskId(subTaskId);

                                    // var subTaskActivities = _subTaskActivityService.DeleteSubTaskActivityBySubTaskId (subTaskId);

                                    var subTaskToDelete = _subTaskService.Delete(subTaskId);

                                    SubTaskActivity subTaskActivityObj = new SubTaskActivity();
                                    subTaskActivityObj.SubTaskId = subTaskId;
                                    subTaskActivityObj.UserId = UserId;
                                    subTaskActivityObj.Activity = "Removed this sub task";
                                    var AddUpdate2 = _subTaskActivityService.CheckInsertOrUpdate(subTaskActivityObj);
                                }
                            }

                            var documents = _taskAttachmentService.DeleteAttachmentByTaskId(taskId);

                            // Remove task documents from folder
                            if (documents != null && documents.Count() > 0)
                            {
                                foreach (var taskDoc in documents)
                                {

                                    //var dirPath = _hostingEnvironment.WebRootPath + "\\TaskUpload";
                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.TaskUploadDirPath;
                                    var filePath = dirPath + "\\" + taskDoc.Name;

                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(Path.Combine(filePath));
                                    }
                                }
                            }

                            var comments = _taskCommentService.DeleteCommentByTaskId(taskId);

                            var timeRecords = _taskTimeRecordService.DeleteTimeRecordByTaskId(taskId);

                            var taskUsers = _taskUserService.DeleteByTaskId(taskId);

                            // var taskActivities = _taskActivityService.DeleteTaskActivityByTaskId (taskId);

                            var taskToDelete = _taskService.Delete(taskId);

                            TaskActivity taskActivityObj = new TaskActivity();
                            taskActivityObj.TaskId = taskId;
                            taskActivityObj.UserId = UserId;
                            taskActivityObj.Activity = "Removed this task";
                            var AddUpdateActivity = _taskActivityService.CheckInsertOrUpdate(taskActivityObj);

                        }
                    }
                }

                var AddUpdate = await _sectionService.DeleteSection(model.Id.Value);

                SectionActivity sectionActivityObj = new SectionActivity();
                sectionActivityObj.SectionId = AddUpdate.Id;
                sectionActivityObj.Activity = "Removed this section";
                sectionActivityObj.UserId = UserId;
                var sectionActivityObj1 = _sectionActivityService.CheckInsertOrUpdate(sectionActivityObj);

                return new OperationResult<Section>(true, System.Net.HttpStatusCode.OK, "Section delete successfully", AddUpdate);
            }
            else
            {
                return new OperationResult<Section>(false, System.Net.HttpStatusCode.OK, "Section id null", sectionObj);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<SectionVM>> Detail(long Id)
        {
            SectionVM sectionVMObj = new SectionVM();
            var sectionObj = _sectionService.GetSectionById(Id);

            sectionVMObj = _mapper.Map<SectionVM>(sectionObj);

            var y = 60 * 60 * 1000;
            if (sectionVMObj.Id != null)
            {
                var sectionId = sectionVMObj.Id.Value;

                var taskList = _taskService.GetAllTaskBySectionId(sectionId);
                var taskIdList = taskList.Select(t => t.Id).ToList();
                var subTasks = _subTaskService.GetAllActiveByTaskIds(taskIdList);
                var subTaskIdList = subTasks.Select(t => t.Id).ToList();
                var childTasks = _childTaskService.GetAllActiveBySubTaskIds(subTaskIdList);

                if (taskList != null && taskList.Count() > 0)
                {
                    sectionVMObj.Tasks = new List<OneClappTaskVM>();
                    var taskVMList = _mapper.Map<List<OneClappTaskVM>>(taskList);
                    foreach (var taskObj in taskVMList)
                    {
                        var taskTotalDuration = _taskTimeRecordService.GetTotalTaskTimeRecord(taskObj.Id);
                        taskObj.Duration = taskTotalDuration;

                        var h = taskTotalDuration / y;
                        var m = (taskTotalDuration - (h * y)) / (y / 60);
                        var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;

                        taskObj.Seconds = s;
                        taskObj.Minutes = m;
                        taskObj.Hours = h;
                        var assignTaskUsers = _taskUserService.GetAssignUsersByTask(taskObj.Id);
                        if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                        {
                            taskObj.AssignedUsers = new List<OneClappTaskUserDto>();
                            var assignUsersVMList = _mapper.Map<List<OneClappTaskUserDto>>(assignTaskUsers);
                            taskObj.AssignedUsers = assignUsersVMList;
                        }
                        if (subTasks != null && subTasks.Count() > 0)
                        {
                            var subTaskList = subTasks.Where(t => t.OneClappTaskId == taskObj.Id).ToList();
                            taskObj.SubTasks = new List<OneClappSubTaskVM>();
                            if (subTaskList != null && subTaskList.Count() > 0)
                            {
                                var subTaskVMList = _mapper.Map<List<OneClappSubTaskVM>>(subTaskList);
                                foreach (var subTaskVM in subTaskVMList)
                                {
                                    var subTaskTotalDuration = _subTaskTimeRecordService.GetTotalSubTaskTimeRecord(subTaskVM.Id);
                                    subTaskVM.Duration = subTaskTotalDuration;

                                    var hh = subTaskTotalDuration / y;
                                    var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
                                    var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;

                                    subTaskVM.Seconds = ss;
                                    subTaskVM.Minutes = mm;
                                    subTaskVM.Hours = hh;

                                    var assignSubTaskUsers = _subTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
                                    if (assignSubTaskUsers != null && assignSubTaskUsers.Count > 0)
                                    {
                                        subTaskVM.AssignedUsers = new List<OneClappSubTaskUserDto>();
                                        var assignUsersVMList = _mapper.Map<List<OneClappSubTaskUserDto>>(assignSubTaskUsers);
                                        subTaskVM.AssignedUsers = assignUsersVMList;
                                    }

                                    if (childTasks != null && childTasks.Count() > 0)
                                    {
                                        var childTaskList = childTasks.Where(t => t.OneClappSubTaskId == subTaskVM.Id).ToList();

                                        subTaskVM.ChildTasks = new List<OneClappChildTaskVM>();
                                        if (childTaskList != null && childTaskList.Count() > 0)
                                        {

                                            var childTaskVMList = _mapper.Map<List<OneClappChildTaskVM>>(childTaskList);

                                            foreach (var childTaskVM in childTaskVMList)
                                            {
                                                var childTaskTotalDuration = _childTaskTimeRecordService.GetTotalChildTaskTimeRecord(childTaskVM.Id);
                                                childTaskVM.Duration = childTaskTotalDuration;

                                                var h3 = taskTotalDuration / y;
                                                var m3 = (taskTotalDuration - (h3 * y)) / (y / 60);
                                                var s3 = (taskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
                                                // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

                                                childTaskVM.Seconds = s3;
                                                childTaskVM.Minutes = m3;
                                                childTaskVM.Hours = h3;

                                                var assignChildTaskUsers = _childTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
                                                if (assignChildTaskUsers != null && assignChildTaskUsers.Count > 0)
                                                {
                                                    childTaskVM.AssignedUsers = new List<OneClappChildTaskUserDto>();
                                                    var assignUsersVMList = _mapper.Map<List<OneClappChildTaskUserDto>>(assignChildTaskUsers);
                                                    childTaskVM.AssignedUsers = assignUsersVMList;
                                                }
                                            }
                                            subTaskVM.ChildTasks = childTaskVMList;
                                        }
                                    }
                                }

                                taskObj.SubTasks = subTaskVMList;
                            }
                        }
                    }
                    sectionVMObj.Tasks = taskVMList;
                }
            }

            return new OperationResult<SectionVM>(true, System.Net.HttpStatusCode.OK, "", sectionVMObj);
        }

        private List<OneClappTaskVM> ShortTaskByColumn(string ShortColumn, string ShortOrder, List<OneClappTaskVM> taskList)
        {
            List<OneClappTaskVM> oneClappTaskVMList = new List<OneClappTaskVM>();
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn == "Description")
                {
                    if (ShortOrder == "Asc")
                    {
                        oneClappTaskVMList = taskList.OrderBy(t => t.Description).ToList();
                    }
                    else
                    {
                        oneClappTaskVMList = taskList.OrderByDescending(t => t.Description).ToList();
                    }
                }
                else if (ShortColumn == "Duration")
                {
                    if (ShortOrder == "Asc")
                    {
                        oneClappTaskVMList = taskList.OrderBy(t => t.Duration).ToList();
                    }
                    else
                    {
                        oneClappTaskVMList = taskList.OrderByDescending(t => t.Duration).ToList();
                    }
                }
                else if (ShortColumn == "Due Date")
                {
                    if (ShortOrder == "Asc")
                    {
                        oneClappTaskVMList = taskList.OrderBy(t => t.EndDate).ToList();
                    }
                    else
                    {
                        oneClappTaskVMList = taskList.OrderByDescending(t => t.EndDate).ToList();
                    }
                }
                else if (ShortColumn == "Assignee")
                {
                    if (ShortOrder == "Asc")
                    {
                        oneClappTaskVMList = taskList.OrderBy(t => t.AssignUserCount).ToList();
                    }
                    else
                    {
                        oneClappTaskVMList = taskList.OrderByDescending(t => t.AssignUserCount).ToList();
                    }
                }
            }

            return oneClappTaskVMList;
        }

    }
}