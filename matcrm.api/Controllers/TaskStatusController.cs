using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;


namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TaskStatusController : Controller
    {
        private readonly ITaskStatusService _taskStatusService;
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomerService _customerService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private IMapper _mapper;
        private CustomFieldLogic customFieldLogic;
        private int UserId = 0;
        private int TenantId = 0;
        public TaskStatusController(ITaskStatusService taskStatusService,
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomerService customerService,
            ICustomFieldService customFieldService,
            ICustomModuleService customModuleService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            IMapper mapper)
        {
            _taskStatusService = taskStatusService;
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            _customerService = customerService;
            _customFieldService = customFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _tenantModuleService = tenantModuleService;
            _customTenantFieldService = customTenantFieldService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _mapper = mapper;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
                 customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }

        // [Authorize (Roles = "Manager,TenantAdmin")]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<TaskStatusDto>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<TaskStatusDto> taskStatusDtoList = new List<TaskStatusDto>();
            var taskStatusList = _taskStatusService.GetStatusByUser(UserId);
            taskStatusDtoList = _mapper.Map<List<TaskStatusDto>>(taskStatusList);
            //var taskStatusModule = _customModuleService.GetByName("TaskStatus");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("TaskStatus");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            if (customModuleObj != null)
            {
                CustomModuleDto customModuleDtoObj = new CustomModuleDto();
                customModuleDtoObj.TenantId = TenantId;
                customModuleDtoObj.MasterTableId = customModuleObj.MasterTableId;
                customModuleDtoObj.Id = customModuleObj.Id;

                foreach (var item in taskStatusDtoList)
                {
                    customModuleDtoObj.RecordId = item.Id;
                    item.CustomFields = await customFieldLogic.GetCustomField(customModuleDtoObj);
                }
            }
            return new OperationResult<List<TaskStatusDto>>(true, System.Net.HttpStatusCode.OK, "", taskStatusDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<matcrm.data.Models.Tables.TaskStatus>>> ByTenant()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var taskStatusList = _taskStatusService.GetStatusByTenant(TenantId);
            return new OperationResult<List<matcrm.data.Models.Tables.TaskStatus>>(true, System.Net.HttpStatusCode.OK, "", taskStatusList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<matcrm.data.Models.Tables.TaskStatus>> Add([FromBody] AddUpdateTaskStatusRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var taskStatusDto = _mapper.Map<TaskStatusDto>(requestModel);
            taskStatusDto.UserId = UserId;
            taskStatusDto.TenantId = TenantId;
            var taskStatusObj = _taskStatusService.CheckInsertOrUpdate(taskStatusDto);
            //var taskStatusModule = _customModuleService.GetByName("TaskStatus");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("TaskStatus");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }
           

            if (taskStatusDto.CustomFields != null && taskStatusDto.CustomFields.Count() > 0 && customModuleObj != null)
            {
                foreach (var item in taskStatusDto.CustomFields)
                {
                    if (item != null)
                    {
                        CustomFieldValueDto customFieldValueDtoObj = new CustomFieldValueDto();
                        customFieldValueDtoObj.FieldId = item.Id;
                        customFieldValueDtoObj.ModuleId = customModuleObj.Id;
                        customFieldValueDtoObj.RecordId = taskStatusObj.Id;
                        var controlType = "";
                        if (item.CustomControl != null)
                        {
                            controlType = item.CustomControl.Name;
                            customFieldValueDtoObj.ControlType = controlType;
                        }
                        customFieldValueDtoObj.Value = item.Value;
                        customFieldValueDtoObj.CreatedBy = UserId;
                        customFieldValueDtoObj.TenantId = taskStatusDto.TenantId;
                        if (item.CustomControlOptions != null && item.CustomControlOptions.Count() > 0)
                        {

                            var selectedOptionList = item.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
                            if (controlType == "Checkbox")
                            {
                                var deletedList = _customFieldValueService.DeleteList(customFieldValueDtoObj);
                            }
                            foreach (var option in selectedOptionList)
                            {
                                customFieldValueDtoObj.OptionId = option.Id;
                                var AddUpdate = _customFieldValueService.CheckInsertOrUpdate(customFieldValueDtoObj);
                            }
                        }
                        else
                        {
                            var AddUpdate = _customFieldValueService.CheckInsertOrUpdate(customFieldValueDtoObj);
                        }
                    }

                }
            }
            return new OperationResult<matcrm.data.Models.Tables.TaskStatus>(true, System.Net.HttpStatusCode.OK, "", taskStatusObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<matcrm.data.Models.Tables.TaskStatus>> Update([FromBody] AddUpdateTaskStatusRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var taskStatusDto = _mapper.Map<TaskStatusDto>(requestModel);
            taskStatusDto.UserId = UserId;
            taskStatusDto.TenantId = TenantId;
            var taskStatusObj = _taskStatusService.CheckInsertOrUpdate(taskStatusDto);
            //var taskStatusModule = _customModuleService.GetByName("TaskStatus");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("TaskStatus");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            if (taskStatusDto.CustomFields != null && taskStatusDto.CustomFields.Count() > 0 && customModuleObj != null)
            {
                foreach (var item in taskStatusDto.CustomFields)
                {
                    if (item != null)
                    {
                        CustomFieldValueDto customFieldValueDtoObj = new CustomFieldValueDto();
                        customFieldValueDtoObj.FieldId = item.Id;
                        customFieldValueDtoObj.ModuleId = customModuleObj.Id;
                        customFieldValueDtoObj.RecordId = taskStatusObj.Id;
                        var controlType = "";
                        if (item.CustomControl != null)
                        {
                            controlType = item.CustomControl.Name;
                            customFieldValueDtoObj.ControlType = controlType;
                        }
                        customFieldValueDtoObj.Value = item.Value;
                        customFieldValueDtoObj.CreatedBy = UserId;
                        customFieldValueDtoObj.TenantId = taskStatusDto.TenantId;
                        if (item.CustomControlOptions != null && item.CustomControlOptions.Count() > 0)
                        {

                            var selectedOptionList = item.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
                            if (controlType == "Checkbox")
                            {
                                var deletedList = _customFieldValueService.DeleteList(customFieldValueDtoObj);
                            }
                            foreach (var option in selectedOptionList)
                            {
                                customFieldValueDtoObj.OptionId = option.Id;
                                var AddUpdate = _customFieldValueService.CheckInsertOrUpdate(customFieldValueDtoObj);
                            }
                        }
                        else
                        {
                            var AddUpdate = _customFieldValueService.CheckInsertOrUpdate(customFieldValueDtoObj);
                        }
                    }

                }
            }
            return new OperationResult<matcrm.data.Models.Tables.TaskStatus>(true, System.Net.HttpStatusCode.OK, "", taskStatusObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{TaskStatusId}")]
        public async Task<OperationResult<matcrm.data.Models.Tables.TaskStatus>> Remove(int TaskStatusId)
        {
            var taskStatusObj = _taskStatusService.DeleteTaskStatus(TaskStatusId);
            //var taskStatusModule = _customModuleService.GetByName("TaskStatus");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("TaskStatus");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

             List<CustomField> customFieldList = new List<CustomField>();
            if (customModuleObj != null)
            {
                var moduleFieldList = _moduleFieldService.GetAllModuleField(customModuleObj.Id);


                foreach (var item in moduleFieldList)
                {
                    if (item.CustomField != null)
                    {
                        customFieldList.Add(item.CustomField);
                    }
                }
            }

             if (customFieldList != null && customFieldList.Count() > 0 && customModuleObj != null)
            {
                foreach (var item in customFieldList)
                {
                    CustomFieldValueDto customFieldValueDtoObj = new CustomFieldValueDto();
                    customFieldValueDtoObj.FieldId = item.Id;
                    customFieldValueDtoObj.ModuleId = customModuleObj.Id;
                    customFieldValueDtoObj.RecordId = taskStatusObj.Id;
                    var deletedRecord = _customFieldValueService.DeleteList(customFieldValueDtoObj);
                }
            }

            // if (model.CustomFields != null && model.CustomFields.Count() > 0 && customModuleObj != null)
            // {
            //     foreach (var item in model.CustomFields)
            //     {
            //         CustomFieldValueDto customFieldValueDtoObj = new CustomFieldValueDto();
            //         customFieldValueDtoObj.FieldId = item.Id;
            //         customFieldValueDtoObj.ModuleId = customModuleObj.Id;
            //         customFieldValueDtoObj.RecordId = taskStatusObj.Id;
            //         var deletedRecord = _customFieldValueService.DeleteList(customFieldValueDtoObj);
            //     }
            // }

            return new OperationResult<matcrm.data.Models.Tables.TaskStatus>(true, System.Net.HttpStatusCode.OK, "", taskStatusObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{StatusId}")]
        public async Task<OperationResult<matcrm.data.Models.Tables.TaskStatus>> Detail(int StatusId)
        {
            matcrm.data.Models.Tables.TaskStatus taskStatusObj = new matcrm.data.Models.Tables.TaskStatus();
            taskStatusObj = _taskStatusService.GetTaskStatusById(StatusId);
            return new OperationResult<matcrm.data.Models.Tables.TaskStatus>(true, System.Net.HttpStatusCode.OK, "", taskStatusObj);
        }
    }
}