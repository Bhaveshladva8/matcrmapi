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
using matcrm.service.BusinessLogic;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class ProjectOldController : Controller
    {
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IUserService _userService;
        private readonly IEmployeeProjectActivityService _projectActivityService;
        private readonly IEmployeeProjectStatusService _employeeProjectStatusService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly ISectionService _sectionService;
        private readonly ISectionActivityService _sectionActivityService;
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
        private readonly ICustomModuleService _customModuleService;
        private CustomFieldLogic customFieldLogic;
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomFieldService _customFieldService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly IModuleRecordCustomFieldService _moduleRecordCustomFieldService;
        private readonly ICustomTableColumnService _customTableColumnService;

        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ProjectOldController(
            IEmployeeProjectService employeeProjectService,
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
            IMapper mapper,
            ICustomModuleService customModuleService,
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomFieldService customFieldService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            IModuleRecordCustomFieldService moduleRecordCustomFieldService,
            ICustomTableColumnService customTableColumnService,
            ISectionService sectionService,
            ISectionActivityService sectionActivityService
        )
        {
            _employeeProjectService = employeeProjectService;
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
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _customModuleService = customModuleService;
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            _customFieldService = customFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _tenantModuleService = tenantModuleService;
            _customTenantFieldService = customTenantFieldService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _moduleRecordCustomFieldService = moduleRecordCustomFieldService;
            _customTableColumnService = customTableColumnService;
            _sectionActivityService = sectionActivityService;
            _sectionService = sectionService;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
                customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost("Add")]
        // public async Task<OperationResult<EmployeeProject>> Add([FromForm] EmployeeProjectDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     if (!string.IsNullOrEmpty(model.Duration))
        //     {
        //         var data = model.Duration.Split(":");
        //         var count = data.Count();
        //         if (count == 3)
        //         {
        //             var hours = Convert.ToInt16(data[0]);
        //             var minutes = Convert.ToInt16(data[1]);
        //             var seconds = Convert.ToInt16(data[2]);
        //             model.EstimatedTime = new TimeSpan(hours, minutes, seconds);
        //         }
        //     }
            
        //     model.CreatedBy = UserId;
        //     model.TenantId = TenantId;
        //     if (model.File != null)
        //     {
        //         var dirPath = _hostingEnvironment.WebRootPath + "\\ProjectLogo";

        //         if (!Directory.Exists(dirPath))
        //         {
        //             Directory.CreateDirectory(dirPath);
        //         }

        //         var fileName = string.Concat(
        //                         Path.GetFileNameWithoutExtension(model.File.FileName),
        //                         DateTime.Now.ToString("yyyyMMdd_HHmmss"),
        //                         Path.GetExtension(model.File.FileName)
        //                     );
        //         var filePath = dirPath + "\\" + fileName;


        //         if (System.IO.File.Exists(filePath))
        //         {
        //             System.IO.File.Delete(Path.Combine(filePath));
        //         }

        //         EmployeeProject employeeProjectObj = new EmployeeProject();
        //         var employeeProject = _mapper.Map<EmployeeProject>(model);

        //         model.Logo = fileName;

        //         using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
        //         {
        //             await model.File.CopyToAsync(oStream);
        //         }
        //     }

        //     var AddUpdate = await _employeeProjectService.CheckInsertOrUpdate(model);
        //     EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
        //     employeeProjectActivityObj.ProjectId = AddUpdate.Id;
        //     employeeProjectActivityObj.Activity = "Created Project";
        //     employeeProjectActivityObj.UserId = model.CreatedBy;
        //     var projectActivityObj = _projectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);

        //     CustomModule? customModuleObj = null;
        //     var employeeProjectTableObj = _customTableService.GetByName("Project");
        //     if (employeeProjectTableObj != null)
        //     {
        //         customModuleObj = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
        //     }


        //     if (model.CustomFields != null && model.CustomFields.Count() > 0)
        //     {
        //         foreach (var item in model.CustomFields)
        //         {
        //             if (item != null)
        //             {
        //                 CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
        //                 customFieldValueDto.FieldId = item.Id;
        //                 customFieldValueDto.ModuleId = customModuleObj.Id;
        //                 customFieldValueDto.RecordId = AddUpdate.Id;
        //                 var controlType = "";
        //                 if (item.CustomControl != null)
        //                 {
        //                     controlType = item.CustomControl.Name;
        //                     customFieldValueDto.ControlType = controlType;
        //                 }
        //                 customFieldValueDto.Value = item.Value;
        //                 customFieldValueDto.CreatedBy = UserId;
        //                 customFieldValueDto.TenantId = TenantId;
        //                 if (item.CustomControlOptions != null && item.CustomControlOptions.Count() > 0)
        //                 {

        //                     var selectedOptionList = item.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
        //                     if (controlType == "Checkbox")
        //                     {
        //                         var deletedList = await _customFieldValueService.DeleteList(customFieldValueDto);
        //                     }
        //                     if (selectedOptionList != null && selectedOptionList.Count() > 0)
        //                     {
        //                         foreach (var option in selectedOptionList)
        //                         {
        //                             customFieldValueDto.OptionId = option.Id;
        //                             var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
        //                         }
        //                     }
        //                 }
        //                 else
        //                 {
        //                     var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
        //                 }
        //             }

        //         }
        //     }

        //     return new OperationResult<EmployeeProject>(true, System.Net.HttpStatusCode.OK, "Project add successfully", AddUpdate);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPut("Update")]
        // public async Task<OperationResult<EmployeeProject>> Update([FromBody] EmployeeProjectDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     if (model.Duration != null)
        //     {
        //         var data = model.Duration.Split(":");
        //         var count = data.Count();
        //         if (count == 3)
        //         {
        //             var hours = Convert.ToInt16(data[0]);
        //             var minutes = Convert.ToInt16(data[1]);
        //             var seconds = Convert.ToInt16(data[2]);
        //             model.EstimatedTime = new TimeSpan(hours, minutes, seconds);
        //         }
        //     }

        //     model.UpdatedBy = UserId;
        //     model.TenantId = TenantId;
        //     EmployeeProject employeeProjectObj = new EmployeeProject();
        //     if (model.Id != null)
        //     {
        //         var existingItem = _employeeProjectService.GetEmployeeProjectById(model.Id.Value);
        //         if (existingItem != null)
        //         {
        //             existingItem.Name = model.Name;
        //             existingItem.UpdatedBy = model.UpdatedBy;
        //             var AddUpdate = await _employeeProjectService.CheckInsertOrUpdate(model);

        //             EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
        //             employeeProjectActivityObj.ProjectId = AddUpdate.Id;
        //             employeeProjectActivityObj.Activity = "Updated Project";
        //             employeeProjectActivityObj.UserId = model.UpdatedBy;
        //             var ProjectActivityObj = _projectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);

        //             CustomModule? customModuleObj = null;
        //             var employeeProjectTableObj = _customTableService.GetByName("Project");
        //             if (employeeProjectTableObj != null)
        //             {
        //                 customModuleObj = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
        //             }

        //             if (model.CustomFields != null && model.CustomFields.Count() > 0)
        //             {
        //                 foreach (var item in model.CustomFields)
        //                 {
        //                     if (item != null)
        //                     {
        //                         CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
        //                         customFieldValueDto.FieldId = item.Id;
        //                         customFieldValueDto.ModuleId = customModuleObj.Id;
        //                         customFieldValueDto.RecordId = AddUpdate.Id;
        //                         var controlType = "";
        //                         if (item.CustomControl != null)
        //                         {
        //                             controlType = item.CustomControl.Name;
        //                             customFieldValueDto.ControlType = controlType;
        //                         }
        //                         customFieldValueDto.Value = item.Value;
        //                         customFieldValueDto.CreatedBy = UserId;
        //                         customFieldValueDto.TenantId = TenantId;
        //                         if (item.CustomControlOptions != null && item.CustomControlOptions.Count() > 0)
        //                         {

        //                             var selectedOptionList = item.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
        //                             if (controlType == "Checkbox")
        //                             {
        //                                 var deletedList = await _customFieldValueService.DeleteList(customFieldValueDto);
        //                             }
        //                             if (selectedOptionList != null && selectedOptionList.Count() > 0)
        //                             {
        //                                 foreach (var option in selectedOptionList)
        //                                 {
        //                                     customFieldValueDto.OptionId = option.Id;
        //                                     var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
        //                                 }
        //                             }
        //                         }
        //                         else
        //                         {
        //                             var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
        //                         }
        //                     }

        //                 }
        //             }

        //             return new OperationResult<EmployeeProject>(true, System.Net.HttpStatusCode.OK, "Project Updated successfully", AddUpdate);
        //         }
        //     }
        //     return new OperationResult<EmployeeProject>(false, System.Net.HttpStatusCode.OK, "Error", employeeProjectObj);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost("List")]
        // public async Task<OperationResult<ProjectTaskListVM>> GetAll([FromBody] ProjectTaskDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     ProjectTaskListVM projectTaskListVMObj = new ProjectTaskListVM();

        //     List<ProjectVM> projectVMList = new List<ProjectVM>();
        //     var y = 60 * 60 * 1000;

        //     var projects = _employeeProjectService.GetAllByTenant(TenantId);
        //     projectVMList = _mapper.Map<List<ProjectVM>>(projects);

        //     // Start logic for Task with Project
        //     if (projectVMList != null && projectVMList.Count() > 0)
        //     {
        //         var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
        //         var taskIdList = tasks.Select(t => t.Id).ToList();
        //         var subTasks = _employeeSubTaskService.GetAllActiveByTaskIds(taskIdList);
        //         var subTaskIdList = subTasks.Select(t => t.Id).ToList();
        //         var childTasks = _employeeChildTaskService.GetAllActiveBySubTaskIds(subTaskIdList);

        //         foreach (var item in projectVMList)
        //         {
        //             if (item.Id != null)
        //             {
        //                 var taskList = _employeeTaskService.GetAllTaskByProjectId(item.Id.Value);
        //                 if (taskList != null)
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

        //                     var finalTaskList = taskList.ToList();

        //                     if (finalTaskList.Count() > 0)
        //                     {
        //                         item.Tasks = new List<EmployeeTaskVM>();
        //                         var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(finalTaskList);
        //                         if (taskVMList != null)
        //                         {
        //                             foreach (var taskObj in taskVMList)
        //                             {
        //                                 if (taskObj.Id != null)
        //                                 {
        //                                     // var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
        //                                     // taskObj.Duration = taskTotalDuration;

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
        //                                         var assignUsersVM = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        //                                         taskObj.AssignedUsers = assignUsersVM;
        //                                     }
        //                                     //taskObj.AssignUserCount = assignTaskUsers.Count();
        //                                 }
        //                                 if (subTasks != null)
        //                                 {
        //                                     var subTaskList = subTasks.Where(t => t.EmployeeTaskId == taskObj.Id).ToList();
        //                                     taskObj.SubTasks = new List<EmployeeSubTaskVM>();
        //                                     if (subTaskList.Count() > 0)
        //                                     {
        //                                         var subTaskVMList = _mapper.Map<List<EmployeeSubTaskVM>>(subTaskList);
        //                                         if (subTaskVMList != null && subTaskVMList.Count() > 0)
        //                                         {
        //                                             foreach (var subTaskVM in subTaskVMList)
        //                                             {
        //                                                 var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(subTaskVM.Id);
        //                                                 // subTaskVM.Duration = subTaskTotalDuration;

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
        //                                                                 // childTaskVM.Duration = childTaskTotalDuration;

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

        //                         item.Tasks = taskVMList;
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
        //     var taskWithOutProjectList = _employeeTaskService.GetAllTaskByTenantWithOutProject(TenantId);
        //     var taskIdList1 = taskWithOutProjectList.Select(t => t.Id).ToList();

        //     var subTasks1 = _employeeSubTaskService.GetAllActiveByTaskIds(taskIdList1);
        //     var subTaskIdList1 = subTasks1.Select(t => t.Id).ToList();
        //     var childTasks1 = _employeeChildTaskService.GetAllActiveBySubTaskIds(subTaskIdList1);
        //     // var taskWeClappIdList1 = taskWithOutProjectList.Select(t => t.WeClappTimeRecordId).ToList();
        //     // var taskRecordExist1 = timeRecords.Where(t => taskWeClappIdList1.Contains(Convert.ToInt64(t.id))).Select(t => t.id).ToList();

        //     var finalTaskList1 = taskWithOutProjectList.ToList();
        //     if (finalTaskList1 != null && finalTaskList1.Count() > 0)
        //     {
        //         projectTaskListVMObj.Tasks = new List<EmployeeTaskVM>();
        //         var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(finalTaskList1);
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
        //                     // taskObj.Duration = taskTotalDuration;

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
        //                 if (subTasks1 != null)
        //                 {
        //                     var subTaskList = subTasks1.Where(t => t.EmployeeTaskId == taskObj.Id).ToList();
        //                     taskObj.SubTasks = new List<EmployeeSubTaskVM>();
        //                     if (subTaskList != null && subTaskList.Count() > 0)
        //                     {
        //                         var subTaskVMList = _mapper.Map<List<EmployeeSubTaskVM>>(subTaskList);
        //                         foreach (var subTaskVM in subTaskVMList)
        //                         {
        //                             var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(subTaskVM.Id);
        //                             // subTaskVM.Duration = subTaskTotalDuration;

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
        //                             if (childTasks1 != null)
        //                             {
        //                                 var childTaskList = childTasks1.Where(t => t.EmployeeSubTaskId == subTaskVM.Id).ToList();

        //                                 subTaskVM.ChildTasks = new List<EmployeeChildTaskVM>();
        //                                 if (childTaskList != null && childTaskList.Count() > 0)
        //                                 {
        //                                     var childTaskVMList = _mapper.Map<List<EmployeeChildTaskVM>>(childTaskList);

        //                                     foreach (var childTaskVM in childTaskVMList)
        //                                     {
        //                                         var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskVM.Id);
        //                                         // childTaskVM.Duration = childTaskTotalDuration;

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
        //         }
        //         // var taskSorting = taskVMList.OrderBy(t => t.Priority).ToList();
        //         // projectTaskListVMObj.Tasks = taskSorting;

        //         // Sort Task based on column
        //         if (model.ShortColumn != "" && model.ShortColumn != null)
        //         {
        //             var taskTempList = ShortTaskByColumn(model.ShortColumn, model.ShortOrder, projectTaskListVMObj.Tasks);
        //             projectTaskListVMObj.Tasks = taskTempList;
        //         }
        //         // taskListVM.Tasks = taskVMList;
        //     }
        //     // End Logic

        //     // if (model.FilterTaskDescription != null) {
        //     //     ProjectList = ProjectList.Where (t => t.Tasks.Count () > 0).ToList ();
        //     // }
        //     // ProjectList = ProjectList.Where (t => t.Tasks.Count () > 0).ToList ();
        //     projectVMList = projectVMList.OrderBy(t => t.Priority).ToList();
        //     projectTaskListVMObj.Projects = projectVMList;

        //     return new OperationResult<ProjectTaskListVM>(true, System.Net.HttpStatusCode.OK, "", projectTaskListVMObj);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpGet("AllWithProject")]
        // public async Task<OperationResult<List<ProjectVM>>> GetAllWithProject()
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     List<ProjectVM> projectVMList = new List<ProjectVM>();
        //     var y = 60 * 60 * 1000;
        //     var projects = _employeeProjectService.GetAllByTenant(TenantId);
        //     projectVMList = _mapper.Map<List<ProjectVM>>(projects);

        //     if (projectVMList != null && projectVMList.Count() > 0)
        //     {
        //         var tasks = _employeeTaskService.GetAllActiveByTenant(TenantId);
        //         var taskIdList = tasks.Select(t => t.Id).ToList();
        //         var subTasks = _employeeSubTaskService.GetAllActiveByTaskIds(taskIdList);
        //         var subTaskIdList = subTasks.Select(t => t.Id).ToList();
        //         var childTasks = _employeeChildTaskService.GetAllActiveBySubTaskIds(subTaskIdList);

        //         foreach (var item in projectVMList)
        //         {
        //             var taskList = _employeeTaskService.GetAllTaskByProjectId(item.Id.Value);

        //             var FinalTaskList = taskList.ToList();

        //             if (FinalTaskList != null && FinalTaskList.Count() > 0)
        //             {
        //                 item.Tasks = new List<EmployeeTaskVM>();
        //                 var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(FinalTaskList);
        //                 if (taskVMList != null && taskVMList.Count() > 0)
        //                 {
        //                     foreach (var taskObj in taskVMList)
        //                     {
        //                         if (taskObj.Id != null)
        //                         {
        //                             var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
        //                             // taskObj.Duration = taskTotalDuration;

        //                             // var h = taskTotalDuration / y;
        //                             // var m = (taskTotalDuration - (h * y)) / (y / 60);
        //                             // var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;
        //                             // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                             // taskObj.Seconds = s;
        //                             // taskObj.Minutes = m;
        //                             // taskObj.Hours = h;
        //                             var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
        //                             if (assignTaskUsers.Count > 0)
        //                             {
        //                                 taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
        //                                 var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        //                                 taskObj.AssignedUsers = assignUsersVMList;
        //                             }
        //                         }
        //                         if (subTasks != null)
        //                         {
        //                             var subTaskList = subTasks.Where(t => t.EmployeeTaskId == taskObj.Id).ToList();
        //                             taskObj.SubTasks = new List<EmployeeSubTaskVM>();
        //                             if (subTaskList != null && subTaskList.Count() > 0)
        //                             {
        //                                 var subTaskVMList = _mapper.Map<List<EmployeeSubTaskVM>>(subTaskList);
        //                                 foreach (var subTaskVM in subTaskVMList)
        //                                 {
        //                                     var subTaskTotalDuration = _employeeSubTaskTimeRecordService.GetTotalEmployeeSubTaskTimeRecord(subTaskVM.Id);
        //                                     // subTaskVM.Duration = subTaskTotalDuration;

        //                                     // var hh = subTaskTotalDuration / y;
        //                                     // var mm = (subTaskTotalDuration - (hh * y)) / (y / 60);
        //                                     // var ss = (subTaskTotalDuration - (hh * y) - (mm * (y / 60))) / 1000;
        //                                     // var mmi = subTaskTotalDuration - (hh * y) - (mm * (y / 60)) - (ss * 1000);

        //                                     // subTaskVM.Seconds = ss;
        //                                     // subTaskVM.Minutes = mm;
        //                                     // subTaskVM.Hours = hh;

        //                                     var assignSubTaskUsers = _employeeSubTaskUserService.GetAssignUsersBySubTask(subTaskVM.Id);
        //                                     if (assignSubTaskUsers.Count > 0)
        //                                     {
        //                                         subTaskVM.AssignedUsers = new List<EmployeeSubTaskUserDto>();
        //                                         var assignUsersVMList = _mapper.Map<List<EmployeeSubTaskUserDto>>(assignSubTaskUsers);
        //                                         subTaskVM.AssignedUsers = assignUsersVMList;
        //                                     }

        //                                     if (childTasks != null)
        //                                     {
        //                                         var childTaskList = childTasks.Where(t => t.EmployeeSubTaskId == subTaskVM.Id).ToList();

        //                                         subTaskVM.ChildTasks = new List<EmployeeChildTaskVM>();
        //                                         if (childTaskList != null && childTaskList.Count() > 0)
        //                                         {
        //                                             var childTaskVMList = _mapper.Map<List<EmployeeChildTaskVM>>(childTaskList);

        //                                             foreach (var childTaskVM in childTaskVMList)
        //                                             {
        //                                                 var childTaskTotalDuration = _employeeChildTaskTimeRecordService.GetTotalEmployeeChildTaskTimeRecord(childTaskVM.Id);
        //                                                 // childTaskVM.Duration = childTaskTotalDuration;

        //                                                 // var h3 = childTaskTotalDuration / y;
        //                                                 // var m3 = (childTaskTotalDuration - (h3 * y)) / (y / 60);
        //                                                 // var s3 = (childTaskTotalDuration - (h3 * y) - (m3 * (y / 60))) / 1000;
        //                                                 // // var mi = taskTotalDuration - (h * y) - (m * (y / 60)) - (s * 1000);

        //                                                 // childTaskVM.Seconds = s3;
        //                                                 // childTaskVM.Minutes = m3;
        //                                                 // childTaskVM.Hours = h3;

        //                                                 var assignChildTaskUsers = _employeeChildTaskUserService.GetAssignUsersByChildTask(childTaskVM.Id);
        //                                                 if (assignChildTaskUsers.Count > 0)
        //                                                 {
        //                                                     childTaskVM.AssignedUsers = new List<EmployeeChildTaskUserDto>();
        //                                                     var assignUsersVMList = _mapper.Map<List<EmployeeChildTaskUserDto>>(assignChildTaskUsers);
        //                                                     childTaskVM.AssignedUsers = assignUsersVMList;
        //                                                 }
        //                                             }
        //                                             subTaskVM.ChildTasks = childTaskVMList;
        //                                         }
        //                                     }
        //                                 }

        //                                 taskObj.SubTasks = subTaskVMList;
        //                             }
        //                         }
        //                     }
        //                 }
        //                 item.Tasks = taskVMList;
        //             }
        //         }
        //     }
        //     return new OperationResult<List<ProjectVM>>(true, System.Net.HttpStatusCode.OK, "", projectVMList);
        // }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpDelete]
        // public async Task<OperationResult<EmployeeProject>> Remove([FromBody] EmployeeProjectDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     EmployeeProject employeeProjectObj = new EmployeeProject();
        //     if (model.Id != null)
        //     {
        //         var projectId = model.Id.Value;
        //         var tasks = _employeeTaskService.GetAllTaskByProjectId(projectId);
        //         var sections = _sectionService.GetByProject(projectId);
        //         var tasksWithOutProject = _employeeTaskService.GetAllTaskByTenantWithOutProject(TenantId);
        //         var sectionWithOutProject = _sectionService.GetAllByTenantWithoutProject(TenantId);

        //         var taskWithOutProjectCount = 0;
        //         if (tasksWithOutProject != null)
        //         {
        //             taskWithOutProjectCount = tasksWithOutProject.Count();
        //         }
        //         var sectionWithOutProjectCount = 0;
        //         if (sectionWithOutProject != null)
        //         {
        //             sectionWithOutProjectCount = sectionWithOutProject.Count();
        //         }

        //         if (model.IsKeepTasks == true)
        //         {
        //             if (tasks != null && tasks.Count() > 0)
        //             {
        //                 foreach (var taskObj in tasks)
        //                 {
        //                     taskWithOutProjectCount = taskWithOutProjectCount + 1;
        //                     taskObj.ProjectId = null;
        //                     taskObj.Priority = taskWithOutProjectCount;
        //                     taskObj.UpdatedBy = UserId;
        //                     var UpdatedTask = await _employeeTaskService.UpdateTask(taskObj, taskObj.Id);

        //                     EmployeeTaskActivity EmployeeTaskActivityObj = new EmployeeTaskActivity();
        //                     EmployeeTaskActivityObj.EmployeeTaskId = UpdatedTask.Id;
        //                     EmployeeTaskActivityObj.UserId = UserId;
        //                     // EmployeeTaskActivity.ProjectId = ProjectId;
        //                     EmployeeTaskActivityObj.Activity = "Removed this task from Project";
        //                     var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(EmployeeTaskActivityObj);
        //                 }
        //             }
        //             if (sections != null && sections.Count() > 0)
        //             {
        //                 foreach (var sectionObj in sections)
        //                 {
        //                     sectionWithOutProjectCount = sectionWithOutProjectCount + 1;
        //                     sectionObj.ProjectId = null;
        //                     sectionObj.Priority = taskWithOutProjectCount;
        //                     sectionObj.UpdatedBy = UserId;
        //                     var UpdatedSection = await _sectionService.UpdateSection(sectionObj, sectionObj.Id);

        //                     SectionActivity sectionActivityObj = new SectionActivity();
        //                     sectionActivityObj.SectionId = UpdatedSection.Id;
        //                     sectionActivityObj.UserId = UserId;
        //                     // EmployeeTaskActivity.ProjectId = ProjectId;
        //                     sectionActivityObj.Activity = "Removed this section from Project";
        //                     var AddUpdateActivity = await _sectionActivityService.CheckInsertOrUpdate(sectionActivityObj);
        //                 }
        //             }

        //             foreach (var sectionObj in sections)
        //             {
        //                 sectionWithOutProjectCount = sectionWithOutProjectCount + 1;
        //                 sectionObj.ProjectId = null;
        //                 sectionObj.Priority = taskWithOutProjectCount;
        //                 sectionObj.UpdatedBy = UserId;
        //                 var UpdatedSection = await _sectionService.UpdateSection(sectionObj, sectionObj.Id);

        //                 SectionActivity SectionActivityObj = new SectionActivity();
        //                 SectionActivityObj.SectionId = UpdatedSection.Id;
        //                 SectionActivityObj.UserId = UserId;
        //                 // EmployeeTaskActivity.ProjectId = ProjectId;
        //                 SectionActivityObj.Activity = "Removed this section from Project";
        //                 var AddUpdateActivity = await _sectionActivityService.CheckInsertOrUpdate(SectionActivityObj);
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

        //                                             var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
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

        //                                     var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
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

        //                             var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
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
        //             var DeletedSection = await _sectionService.DeleteSection(projectId);
        //         }

        //         var AddUpdate = await _employeeProjectService.DeleteEmployeeProject(model.Id.Value);

        //         EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
        //         employeeProjectActivityObj.ProjectId = AddUpdate.Id;
        //         employeeProjectActivityObj.Activity = "Removed this Project";
        //         employeeProjectActivityObj.UserId = UserId;
        //         var ProjectActivityObj = _projectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);

        //         CustomModule? customModuleObj = null;
        //         var employeeProjectTableObj = _customTableService.GetByName("Project");
        //         if (employeeProjectTableObj != null)
        //         {
        //             customModuleObj = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
        //         }

        //         if (customModuleObj != null)
        //         {

        //             var moduleFields = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

        //             var moduleFieldIdList = moduleFields.Select(t => t.Id).ToList();

        //             var moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIdList);
        //             if (moduleRecordFieldList != null && moduleRecordFieldList.Count() > 0)
        //             {
        //                 foreach (var moduleRecordField in moduleRecordFieldList)
        //                 {
        //                     if (moduleRecordField.RecordId == model.Id)
        //                     {
        //                         var DeletedModuleRecordField = await _moduleRecordCustomFieldService.DeleteById(moduleRecordField.Id);

        //                         var moduleFieldId = moduleRecordField.ModuleFieldId;
        //                         long? CustomFieldId1 = null;
        //                         if (moduleRecordField.ModuleField.CustomField != null)
        //                         {
        //                             CustomFieldId1 = moduleRecordField.ModuleField.CustomField.Id;
        //                         }

        //                         if (moduleFieldId != null)
        //                         {
        //                             var DeleteModuleField = _moduleFieldService.Delete(moduleFieldId.Value);
        //                         }

        //                         if (CustomFieldId1 != null && AddUpdate.TenantId != null)
        //                         {
        //                             var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(CustomFieldId1.Value, AddUpdate.TenantId.Value);
        //                         }

        //                         CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
        //                         customTableColumnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
        //                         customTableColumnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
        //                         customTableColumnDto.IsDefault = false;
        //                         customTableColumnDto.TenantId = AddUpdate.TenantId;
        //                         if (CustomFieldId1 != null)
        //                         {
        //                             customTableColumnDto.CustomFieldId = CustomFieldId1;
        //                         }

        //                         customTableColumnDto.MasterTableId = customModuleObj.Id;

        //                         var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);

        //                         if (CustomFieldId1 != null)
        //                         {
        //                             var deleteTableColumns1 = _customFieldService.DeleteById(CustomFieldId1.Value);
        //                         }

        //                     }
        //                 }
        //             }
        //         }

        //         return new OperationResult<EmployeeProject>(true, System.Net.HttpStatusCode.OK, "Project delete successfully", AddUpdate);
        //     }
        //     else
        //     {
        //         return new OperationResult<EmployeeProject>(false, System.Net.HttpStatusCode.OK, "Project id null", employeeProjectObj);
        //     }
        // }

        // // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // // [HttpGet]
        // // public async Task<OperationResult<ProjectVM>> GetProjectById(long Id)
        // // {
        // //     ProjectVM projectVMObj = new ProjectVM();
        // //     var projectObj = _employeeProjectService.GetEmployeeProjectById(Id);
        // //     if (projectObj != null)
        // //     {
        // //         projectVMObj = _mapper.Map<ProjectVM>(projectObj);

        // //         var y = 60 * 60 * 1000;
        // //         if (projectVMObj.Id != null)
        // //         {
        // //             var projectId = projectVMObj.Id.Value;

        // //             var taskList = _employeeTaskService.GetAllTaskByProjectId(projectId);

        // //             if (taskList != null && taskList.Count() > 0)
        // //             {
        // //                 projectVMObj.Tasks = new List<EmployeeTaskVM>();
        // //                 var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(taskList);
        // //                 foreach (var taskObj in taskVMList)
        // //                 {
        // //                     if (taskObj.Id != null)
        // //                     {
        // //                         var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
        // //                         // taskObj.Duration = taskTotalDuration;

        // //                         // var h = taskTotalDuration / y;
        // //                         // var m = (taskTotalDuration - (h * y)) / (y / 60);
        // //                         // var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;

        // //                         // taskObj.Seconds = s;
        // //                         // taskObj.Minutes = m;
        // //                         // taskObj.Hours = h;
        // //                         var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
        // //                         if (assignTaskUsers.Count > 0)
        // //                         {
        // //                             taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
        // //                             var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
        // //                             taskObj.AssignedUsers = assignUsersVMList;
        // //                         }
        // //                     }
        // //                 }
        // //                 projectVMObj.Tasks = taskVMList;
        // //             }
        // //         }
        // //         return new OperationResult<ProjectVM>(true, System.Net.HttpStatusCode.OK, "", projectVMObj);
        // //     }
        // //     else
        // //     {
        // //         return new OperationResult<ProjectVM>(false, System.Net.HttpStatusCode.OK, "Project not found", projectVMObj);
        // //     }
        // // }

        // private List<EmployeeTaskVM> ShortTaskByColumn(string ShortColumn, string ShortOrder, List<EmployeeTaskVM> taskList)
        // {
        //     List<EmployeeTaskVM> employeeTaskVMList = new List<EmployeeTaskVM>();
        //     if (ShortColumn != "" && ShortColumn != null)
        //     {
        //         if (ShortColumn == "Description")
        //         {
        //             if (ShortOrder == "Asc")
        //             {
        //                 employeeTaskVMList = taskList.OrderBy(t => t.Description).ToList();
        //             }
        //             else
        //             {
        //                 employeeTaskVMList = taskList.OrderByDescending(t => t.Description).ToList();
        //             }
        //         }
        //         else if (ShortColumn == "Duration")
        //         {
        //             // if (ShortOrder == "Asc")
        //             // {
        //             //     employeeTaskVMList = taskList.OrderBy(t => t.Duration).ToList();
        //             // }
        //             // else
        //             // {
        //             //     employeeTaskVMList = taskList.OrderByDescending(t => t.Duration).ToList();
        //             // }
        //         }
        //         else if (ShortColumn == "Due Date")
        //         {
        //             if (ShortOrder == "Asc")
        //             {
        //                 employeeTaskVMList = taskList.OrderBy(t => t.EndDate).ToList();
        //             }
        //             else
        //             {
        //                 employeeTaskVMList = taskList.OrderByDescending(t => t.EndDate).ToList();
        //             }
        //         }
        //         else if (ShortColumn == "Assignee")
        //         {
        //             // if (ShortOrder == "Asc")
        //             // {
        //             //     employeeTaskVMList = taskList.OrderBy(t => t.AssignUserCount).ToList();
        //             // }
        //             // else
        //             // {
        //             //     employeeTaskVMList = taskList.OrderByDescending(t => t.AssignUserCount).ToList();
        //             // }
        //         }
        //     }

        //     return employeeTaskVMList;
        // }


        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPut("Priority")]
        // public async Task<OperationResult<EmployeeProjectDto>> UpdatePriority([FromBody] EmployeeProjectDto model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     EmployeeProjectDto employeeProjectDtoObj = new EmployeeProjectDto();
        //     EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
        //     if (model.Id != null)
        //     {
        //         // start logic for Update Current task with priority
        //         var projectObj = _employeeProjectService.GetEmployeeProjectById(model.Id.Value);
        //         projectObj.Priority = model.CurrentPriority;
        //         projectObj.UpdatedBy = UserId;

        //         employeeProjectActivityObj.Activity = "Priority changed for this project. ";

        //         var projectAddUpdate = await _employeeProjectService.UpdateEmployeeProject(projectObj, projectObj.Id);

        //         employeeProjectActivityObj.ProjectId = model.Id;
        //         employeeProjectActivityObj.UserId = UserId;

        //         var AddUpdate = await _projectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
        //         // End Logic


        //         // start logic for task move in with out section list
        //         var employeeProjectLsit = _employeeProjectService.GetAllByTenant(TenantId);

        //         if (model.CurrentPriority < employeeProjectLsit.Count())
        //         {
        //             if (model.CurrentPriority != model.PreviousPriority)
        //             {
        //                 if (model.PreviousPriority < model.CurrentPriority)
        //                 {
        //                     var projects = employeeProjectLsit.Where(t => t.Priority > model.PreviousPriority && t.Priority <= model.CurrentPriority && t.Id != model.Id).ToList();
        //                     if (projects != null && projects.Count() > 0)
        //                     {
        //                         foreach (var item in projects)
        //                         {
        //                             item.Priority = item.Priority - 1;
        //                             await _employeeProjectService.UpdateEmployeeProject(item, item.Id);
        //                         }
        //                     }
        //                 }
        //                 else if (model.PreviousPriority > model.CurrentPriority)
        //                 {
        //                     var projects = employeeProjectLsit.Where(t => t.Priority < model.PreviousPriority && t.Priority >= model.CurrentPriority && t.Id != model.Id).ToList();
        //                     if (projects != null && projects.Count() > 0)
        //                     {
        //                         foreach (var item in projects)
        //                         {
        //                             item.Priority = item.Priority + 1;
        //                             await _employeeProjectService.UpdateEmployeeProject(item, item.Id);
        //                         }
        //                     }
        //                 }
        //             }

        //             // end
        //         }
        //     }
        //     return new OperationResult<EmployeeProjectDto>(true, System.Net.HttpStatusCode.OK, "", employeeProjectDtoObj);
        // }


        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]        
        // [HttpGet("Projects")]
        // public async Task<OperationResult<List<EmployeeProjectDto>>> GetAllProject(string searchString)
        // {
        //     List<EmployeeProjectDto> employeeProjectDtoList = new List<EmployeeProjectDto>();

        //     EmployeeProjectDto employeeProjectObj = new EmployeeProjectDto();

        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

        //     var employeeProjectList = _employeeProjectService.GetAll(searchString, employeeProjectObj);
        //     employeeProjectDtoList = _mapper.Map<List<EmployeeProjectDto>>(employeeProjectList);
        //     if (employeeProjectDtoList != null && employeeProjectDtoList.Count() > 0)
        //     {
        //         foreach (var item in employeeProjectDtoList)
        //         {
        //             if (item.Id != null)
        //             {
        //                 var projectStatus = _employeeProjectStatusService.GetEmployeeProjectStatusById(item.Id.Value);
        //                 if (projectStatus != null)
        //                 {
        //                     item.StatusId = projectStatus.Id;
        //                 }
        //             }
        //         }
        //     }
        //     CustomModule? customModuleObj = null;
        //     var employeeProjectTableObj = _customTableService.GetByName("Project");
        //     if (employeeProjectTableObj != null)
        //     {
        //         customModuleObj = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
        //     }
        //     // var employeeProjectModule = _customModuleService.GetByName("Project");
        //     if (customModuleObj != null)
        //     {
        //         CustomModuleDto customModuleDto = new CustomModuleDto();
        //         customModuleDto.TenantId = TenantId;
        //         customModuleDto.MasterTableId = customModuleObj.MasterTableId;
        //         customModuleDto.Id = customModuleObj.Id;
        //         if (employeeProjectDtoList != null && employeeProjectDtoList.Count() > 0)
        //         {
        //             foreach (var item in employeeProjectDtoList)
        //             {
        //                 customModuleDto.RecordId = item.Id;
        //                 item.CustomFields = await customFieldLogic.GetCustomField(customModuleDto);
        //             }
        //         }
        //     }
        //     return new OperationResult<List<EmployeeProjectDto>>(true, System.Net.HttpStatusCode.OK, "", employeeProjectDtoList);

        // }


        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        // [HttpGet("GetAllProject")]
        // public async Task<OperationResult<List<EmployeeProjectDto>>> GetAllProject()
        // {
        //     List<EmployeeProjectDto> EmployeeProjectDtos = new List<EmployeeProjectDto>();

        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     var EmployeeProjectList = _employeeProjectService.GetAll();

        //     EmployeeProjectDtos = _mapper.Map<List<EmployeeProjectDto>>(EmployeeProjectList);

        //     foreach (var item in EmployeeProjectDtos)
        //     {
        //         var projectStatus = _employeeProjectStatusService.GetEmployeeProjectStatusById(item.Id.Value);
        //         if (projectStatus != null)
        //         {
        //             item.StatusId = projectStatus.Id;
        //         }
        //     }
        //     CustomModule? employeeProjectModule = null;
        //     var employeeProjectTableObj = _customTableService.GetByName("Project");
        //     if (employeeProjectTableObj != null)
        //     {
        //         employeeProjectModule = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
        //     }
        //     // var employeeProjectModule = _customModuleService.GetByName("Project");
        //     if (employeeProjectModule != null)
        //     {
        //         CustomModuleDto Model = new CustomModuleDto();
        //         Model.TenantId = TenantId;
        //         Model.MasterTableId = employeeProjectModule.MasterTableId;
        //         Model.Id = employeeProjectModule.Id;

        //         foreach (var item in EmployeeProjectDtos)
        //         {
        //             Model.RecordId = item.Id;
        //             item.CustomFields = await customFieldLogic.GetCustomField(Model);
        //         }
        //     }

        //     // var ImagesPath = Path.Combine(_hostingEnvironment.WebRootPath, "ProjectLogo");
        //     // DirectoryInfo dir = new DirectoryInfo(ImagesPath);
        //     // FileInfo[] files = dir.GetFiles();
        //     // EmployeeProjectDtos.fi = files;

        //     return new OperationResult<List<EmployeeProjectDto>>(true, System.Net.HttpStatusCode.OK,"", EmployeeProjectDtos);


        // }

    }
}