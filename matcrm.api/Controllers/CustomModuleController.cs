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
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CustomModuleController : Controller
    {
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly ITableColumnUserService _tableColumnUserService;
        private readonly IModuleRecordCustomFieldService _moduleRecordCustomFieldService;
        private IMapper _mapper;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private CustomFieldLogic customFieldLogic;
        private int UserId = 0;
        private int TenantId = 0;
        public CustomModuleController(
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomFieldService customFieldService,
            ICustomModuleService customModuleService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            ICustomTableColumnService customTableColumnService,
            ITableColumnUserService tableColumnUserService,
            IModuleRecordCustomFieldService moduleRecordCustomFieldService,
            IMapper mapper,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            _customFieldService = customFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _tenantModuleService = tenantModuleService;
            _customTenantFieldService = customTenantFieldService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _customTableColumnService = customTableColumnService;
            _tableColumnUserService = tableColumnUserService;
            _moduleRecordCustomFieldService = moduleRecordCustomFieldService;
            _mapper = mapper;
            _hubContext = hubContext;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
                customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }

        // Assign child task to users
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomModuleDto>> Detail([FromBody] CustomModuleDto Model)
        {
            CustomModuleDto customModuleDto = new CustomModuleDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (Model.Id != null)
            {
                var customModuleObj = _customModuleService.GetById(Model.Id.Value);
                if (customModuleObj != null)
                {
                    customModuleDto = _mapper.Map<CustomModuleDto>(customModuleObj);
                    var tenantModuleObj = _tenantModuleService.GetTenantModule(Model.Id.Value, TenantId);
                    if (tenantModuleObj != null)
                    {
                        customModuleDto.TenantModule = _mapper.Map<TenantModuleDto>(tenantModuleObj);
                    }
                    var moduleFields = _moduleFieldService.GetAllModuleField(Model.Id.Value);
                    customModuleDto.ModuleFields = _mapper.Map<List<ModuleFieldDto>>(moduleFields);

                    if (customModuleObj.MasterTableId != null)
                    {
                        var customTableObj = _customTableService.GetById(customModuleObj.MasterTableId.Value);
                        customModuleDto.CustomTable = _mapper.Map<CustomTableDto>(customTableObj);
                    }

                    var customTenantFieldList = new List<CustomTenantField>();
                    var updatedModuleFieldList = new List<ModuleFieldDto>();

                    if (moduleFields != null && moduleFields.Count() > 0)
                    {
                        foreach (var item in moduleFields)
                        {
                            if (item.FieldId != null)
                            {
                                var tenantFieldObj = _customTenantFieldService.GetCustomTenantField(item.FieldId.Value, TenantId);
                                if (tenantFieldObj != null)
                                {
                                    customTenantFieldList.Add(tenantFieldObj);
                                    var Obj = customModuleDto.ModuleFields.Where(t => t.FieldId == item.FieldId.Value).FirstOrDefault();
                                    updatedModuleFieldList.Add(Obj);
                                }
                            }
                        }
                    }

                    customModuleDto.ModuleFields = updatedModuleFieldList;
                    foreach (var item in customTenantFieldList)
                    {
                        if (item.FieldId != null)
                        {
                            var customfield = _customFieldService.GetById(item.FieldId.Value);
                            if (customModuleDto.CustomFields == null)
                            {
                                customModuleDto.CustomFields = new List<CustomFieldDto>();
                            }
                            var customfieldDto = _mapper.Map<CustomFieldDto>(customfield);
                            var customControlOptions = _customControlOptionService.GetAllControlOption(item.FieldId.Value);
                            customfieldDto.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(customControlOptions);

                            if (customfield.ControlId != null)
                            {
                                var customControl = _customControlService.GetControl(customfield.ControlId.Value);
                                customfieldDto.CustomControl = _mapper.Map<CustomControlDto>(customControl);
                                customModuleDto.CustomFields.Add(customfieldDto);
                            }
                        }
                    }
                }
                return new OperationResult<CustomModuleDto>(true, System.Net.HttpStatusCode.OK, "", customModuleDto);
            }
            else
            {
                return new OperationResult<CustomModuleDto>(false, System.Net.HttpStatusCode.OK, "Id can not pass null", customModuleDto);
            }

        }
        //get all control
        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<CustomModuleGetControlResponse>>> Controls()
        {
            List<CustomControlDto> customControlDtos = new List<CustomControlDto>();
            var customControls = _customControlService.GetAllControl();
            customControlDtos = _mapper.Map<List<CustomControlDto>>(customControls);
            var responseControlDtoList = _mapper.Map<List<CustomModuleGetControlResponse>>(customControlDtos);
            return new OperationResult<List<CustomModuleGetControlResponse>>(true, System.Net.HttpStatusCode.OK, "", responseControlDtoList);
        }

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<CustomModuleGetEntityResponse>>> Entities()
        {
            List<CustomTableDto> customTableDtos = new List<CustomTableDto>();
            var customTables = _customTableService.GetAll();
            customTableDtos = _mapper.Map<List<CustomTableDto>>(customTables);
            var responseControlTableList = _mapper.Map<List<CustomModuleGetEntityResponse>>(customTableDtos);
            return new OperationResult<List<CustomModuleGetEntityResponse>>(true, System.Net.HttpStatusCode.OK, "", responseControlTableList);
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<CustomModuleSaveFieldResponse>> SaveCustomField([FromBody] CustomModuleSaveFieldRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<CustomFieldDto>(Model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            requestmodel.Key = requestmodel.Name.ToLower();
            requestmodel.TenantId = TenantId;
            requestmodel.Key = requestmodel.Name.ToLower();
            CustomFieldDto customFieldDto = new CustomFieldDto();
            var customFieldObj = await _customFieldService.CheckInsertOrUpdate(requestmodel);

            // var tableObj = _customTableService.GetByName(Model.TableName);
            // CustomTableColumnDto columnDto = new CustomTableColumnDto();
            // columnDto.Name = Model.Name;
            // columnDto.ControlId = Model.ControlId;
            // columnDto.IsDefault = false;
            // columnDto.IsHide = true;
            // columnDto.CustomFieldId = customFieldObj.Id;
            // columnDto.TenantId = Convert.ToInt32(Model.TenantId);
            // if (tableObj != null)
            // {
            //     columnDto.MasterTableId = tableObj.Id;
            // }

            // var customTableColumn = await _customTableColumnService.CheckInsertOrUpdate(columnDto);

            if (customFieldObj != null)
            {
                customFieldDto.Id = customFieldObj.Id;
                var FieldId = customFieldObj.Id;
                List<CustomControlOptionDto> customControlOptionDtoList = new List<CustomControlOptionDto>();
                foreach (var item in requestmodel.CustomControlOptions)
                {
                    CustomControlOptionDto customControlOptionDto = new CustomControlOptionDto();
                    customControlOptionDto.Option = item.Option;
                    customControlOptionDto.CustomFieldId = customFieldObj.Id;
                    customControlOptionDto.CreatedBy = UserId;
                    customControlOptionDto.Id = item.Id;
                    var optionModel = await _customControlOptionService.CheckInsertOrUpdate(customControlOptionDto);
                    customControlOptionDto.Id = optionModel.Id;
                    customControlOptionDtoList.Add(customControlOptionDto);
                }
                customFieldDto.CustomControlOptions = customControlOptionDtoList;
                customFieldDto.Name = customFieldObj.Name;
                customFieldDto.Description = customFieldObj.Description;
                customFieldDto.IsRequired = customFieldObj.IsRequired;
                customFieldDto.ControlId = customFieldObj.ControlId;

                if (requestmodel.ControlId != null)
                {
                    var customControl = _customControlService.GetControl(requestmodel.ControlId.Value);
                    customFieldDto.CustomControl = _mapper.Map<CustomControlDto>(customControl);
                }


                if (!string.IsNullOrEmpty(requestmodel.TableName))
                {
                    // var customTableObj = _customTableService.GetByName (Model.TableName);
                    // var customModuleObj = _customModuleService.GetByName (Model.TableName);

                    // Add update Custom Tenant field
                    var customTenantFieldDto = new CustomTenantFieldDto();
                    customTenantFieldDto.TenantId = TenantId;
                    customTenantFieldDto.FieldId = FieldId;
                    customTenantFieldDto.CreatedBy = UserId;
                    customTenantFieldDto.TenantId = TenantId;
                    var customtenantFieldObj = await _customTenantFieldService.CheckInsertOrUpdate(customTenantFieldDto);

                    List<ModuleField> ModuleFieldList = new List<ModuleField>();
                    if (requestmodel.LinkedEntities != null && requestmodel.LinkedEntities.Count() > 0)
                    {
                        foreach (var item in requestmodel.LinkedEntities)
                        {
                            // var customTableObj1 = _customTableService.GetByName (item.Name);
                            //var customModuleObj1 = _customModuleService.GetByName(item.Name);

                            CustomModule? customModuleObj1 = null;
                            var customTable = _customTableService.GetByName(item.Name);
                            if (customTable != null)
                            {
                                customModuleObj1 = _customModuleService.GetByCustomTable(customTable.Id);
                            }

                            var linkTableObj = _customTableService.GetByName(item.Name);
                            if (linkTableObj != null && customModuleObj1 == null)
                            {
                                customModuleObj1 = _customModuleService.GetByCustomTable(linkTableObj.Id);
                            }
                            // Add Update tenant module
                            var tenantModuleDto = new TenantModuleDto();
                            tenantModuleDto.TenantId = TenantId;
                            if (customModuleObj1 != null)
                            {
                                tenantModuleDto.ModuleId = customModuleObj1.Id;
                            }
                            tenantModuleDto.CreatedBy = UserId;
                            var tenantModuleObj = await _tenantModuleService.CheckInsertOrUpdate(tenantModuleDto);

                            // Add Update Module field
                            // Start logic for Add link entity
                            var moduleFieldDto = new ModuleFieldDto();
                            moduleFieldDto.FieldId = FieldId;
                            moduleFieldDto.IsHide = false;
                            if (customModuleObj1 != null)
                            {
                                moduleFieldDto.ModuleId = customModuleObj1.Id;
                            }
                            moduleFieldDto.CreatedBy = UserId;
                            var moduleFieldObj = await _moduleFieldService.CheckInsertOrUpdate(moduleFieldDto);
                            ModuleFieldList.Add(moduleFieldObj);
                            // End logic for Add link entity

                            CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                            customTableColumnDto.Name = customFieldObj.Name;
                             customTableColumnDto.Key = customFieldObj.Name.ToLower();
                            customTableColumnDto.ControlId = customFieldObj.ControlId;
                            customTableColumnDto.MasterTableId = linkTableObj.Id;
                            customTableColumnDto.TenantId = TenantId;
                            customTableColumnDto.CustomFieldId = FieldId;
                            customTableColumnDto.UserId = UserId;
                            var customTableAddUpdate = await _customTableColumnService.CheckInsertOrUpdate(customTableColumnDto);
                        }
                    }

                    // Start logic for remove link entity
                    var savedModuleList = _moduleFieldService.GetAllByField(customFieldObj.Id);
                    foreach (var item in savedModuleList)
                    {
                        var Obj = ModuleFieldList.Where(t => t.ModuleId == item.ModuleId).FirstOrDefault();
                        if (Obj == null)
                        {
                            var deleted = _moduleFieldService.Delete(item.Id);
                            if (item.ModuleId != null)
                            {
                                var ModuleData = _customModuleService.GetById(item.ModuleId.Value);
                                if (ModuleData != null && item.FieldId != null)
                                {
                                    CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                                    customTableColumnDto.MasterTableId = ModuleData.MasterTableId;
                                    customTableColumnDto.CustomFieldId = item.FieldId;
                                    customTableColumnDto.TenantId = TenantId;
                                    var DeleteCustomColumn = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);
                                }
                            }
                        }
                    }

                    if (requestmodel.RecordId != null)
                    {
                        if (savedModuleList != null)
                        {
                            foreach (var item in savedModuleList)
                            {
                                ModuleRecordCustomFieldDto moduleRecordCustomFieldDto = new ModuleRecordCustomFieldDto();
                                moduleRecordCustomFieldDto.ModuleFieldId = item.Id;
                                moduleRecordCustomFieldDto.RecordId = requestmodel.RecordId;
                                moduleRecordCustomFieldDto.CreatedBy = UserId;

                                var AddUpdateRecord = _moduleRecordCustomFieldService.CheckInsertOrUpdate(moduleRecordCustomFieldDto);
                            }
                        }
                    }
                    // End logic for remove link entity

                }
                await _hubContext.Clients.All.OnCustomFieldAddUpdateEmit(TenantId, requestmodel.TableName);
            }
            var responseCustomFieldDto = _mapper.Map<CustomModuleSaveFieldResponse>(customFieldDto);
            responseCustomFieldDto.LinkedEntities = Model.LinkedEntities;
            return new OperationResult<CustomModuleSaveFieldResponse>(true, System.Net.HttpStatusCode.OK, "", responseCustomFieldDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<List<CustomModuleGetCustomFieldResponse>>> CustomFieldDetail([FromBody] CustomModuleGetCustomFieldRequest Model)
        {
            var requestmodel = _mapper.Map<CustomFieldDto>(Model);
            List<CustomFieldDto> customFields = new List<CustomFieldDto>();
            var customTableObj = _customTableService.GetByName(requestmodel.TableName);
            CustomModule customModuleObj = new CustomModule();
            if (customTableObj != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTableObj.Id);
            }
            // var moduleObj = _customModuleService.GetByName(Model.TableName);

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (customModuleObj != null)
            {
                CustomModuleDto customModuleDto = new CustomModuleDto();
                customModuleDto.TenantId = TenantId;
                customModuleDto.MasterTableId = customModuleObj.MasterTableId;
                customModuleDto.Id = customModuleObj.Id;
                customFields = await customFieldLogic.GetCustomField(customModuleDto);

                // start logic for remove record custom field
                var moduleFieldList = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                var moduleFieldIds = moduleFieldList.Select(x => x.Id).ToList();

                var moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIds);

                if (requestmodel.RecordId != null)
                {
                    if (moduleRecordFieldList != null)
                    {
                        var CurrentModuleFieldList = moduleRecordFieldList.Where(t => t.RecordId == requestmodel.RecordId && t.ModuleField.ModuleId == customModuleObj.Id).ToList();
                        if (CurrentModuleFieldList != null && CurrentModuleFieldList.Count() > 0)
                        {
                            foreach (var ModuleFieldItem in CurrentModuleFieldList)
                            {
                                var customFieldObj = customFields.Where(t => t.Id == ModuleFieldItem.ModuleField.CustomField.Id).FirstOrDefault();
                                if (customFieldObj != null)
                                {
                                    customFieldObj.IsRecordField = true;
                                }
                            }
                        }
                        moduleRecordFieldList = moduleRecordFieldList.Where(t => t.RecordId != requestmodel.RecordId || t.ModuleField.ModuleId != customModuleObj.Id).ToList();
                    }
                }

                if (moduleRecordFieldList.Count() > 0)
                {
                    foreach (var moduleRecordCustomField in moduleRecordFieldList)
                    {
                        var isExistData = customFields.Where(t => t.Id == moduleRecordCustomField.ModuleField.FieldId).FirstOrDefault();
                        if (isExistData != null)
                        {
                            customFields.Remove(isExistData);
                        }
                    }
                }
                // End logic for remove record custom field
            }
            var resposneCustomFields = _mapper.Map<List<CustomModuleGetCustomFieldResponse>>(customFields);
            return new OperationResult<List<CustomModuleGetCustomFieldResponse>>(true, System.Net.HttpStatusCode.OK, "", resposneCustomFields);
        }

        [Authorize]
        [HttpGet("{FieldId}")]
        public async Task<OperationResult<CustomFieldDto>> CustomFieldDetail(long FieldId)
        {
            CustomFieldDto customFieldDto = new CustomFieldDto();
            var customFieldObj = _customFieldService.GetById(FieldId);
            customFieldDto = _mapper.Map<CustomFieldDto>(customFieldObj);
            var customOptions = _customControlOptionService.GetAllControlOption(FieldId);
            customFieldDto.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(customOptions);
            var AllTables = _customTableService.GetAll();
            var CustomModules = _customModuleService.GetAll();
            var ModuleFields = _moduleFieldService.GetAllByField(FieldId);
            List<CustomTableDto> customTableDtoList = new List<CustomTableDto>();

            if (ModuleFields != null && ModuleFields.Count() > 0)
            {
                foreach (var item in ModuleFields)
                {
                    var ModuleId = item.ModuleId;
                    var moduleObj = CustomModules.Where(t => t.Id == ModuleId).FirstOrDefault();
                    if (moduleObj != null)
                    {
                        var tableId = moduleObj.MasterTableId;
                        var isExistObj = customTableDtoList.Where(t => t.Id == tableId).FirstOrDefault();
                        if (isExistObj == null)
                        {
                            var TableObj = AllTables.Where(t => t.Id == tableId).FirstOrDefault();
                            var customTableDto = _mapper.Map<CustomTableDto>(TableObj);
                            customTableDtoList.Add(customTableDto);
                        }
                    }
                }
            }
            customFieldDto.LinkedEntities = customTableDtoList;
            return new OperationResult<CustomFieldDto>(true, System.Net.HttpStatusCode.OK, "", customFieldDto);
        }

        [Authorize]
        [HttpDelete]
        public async Task<OperationResult<CustomModuleDeleteFieldResponse>> RemoveCustomField([FromBody] CustomModuleDeleteFieldRequest Model)
        {
            var requestmodel = _mapper.Map<CustomFieldDto>(Model);
            if (requestmodel.Id != null)
            {
                var savedModuleList = _moduleFieldService.GetAllByField(requestmodel.Id.Value);
                var customTable = _customTableService.GetByName(requestmodel.TableName);

                CustomModule? customModule = null;

                if (customTable != null)
                {
                    customModule = _customModuleService.GetByCustomTable(customTable.Id);
                }
                //var customModule = _customModuleService.GetByName(Model.TableName);

                ClaimsPrincipal user = this.User as ClaimsPrincipal;
                TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

                if (savedModuleList.Count > 1 && customModule != null)
                {
                    var deleteObj = savedModuleList.Where(t => t.ModuleId == customModule.Id).FirstOrDefault();
                    if (requestmodel.TenantId != null)
                    {
                        var deleteCustomFieldValues = await _customFieldValueService.DeleteFieldValueList(requestmodel.Id.Value, customModule.Id, requestmodel.TenantId.Value);
                        var deleted = _moduleFieldService.Delete(deleteObj.Id);
                        CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                        customTableColumnDto.Name = requestmodel.Name;
                        customTableColumnDto.ControlId = requestmodel.ControlId;
                        customTableColumnDto.IsDefault = false;
                        customTableColumnDto.TenantId = TenantId;
                        customTableColumnDto.CustomFieldId = requestmodel.Id.Value;
                        var tableObj = _customTableService.GetByName(requestmodel.TableName);
                        if (tableObj != null)
                        {
                            customTableColumnDto.MasterTableId = tableObj.Id;
                        }

                        var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);
                    }
                    await _hubContext.Clients.All.OnCustomFieldAddUpdateEmit(TenantId, requestmodel.TableName);
                }
                else if (savedModuleList.Count == 1 && customModule != null)
                {
                    var deletedTenantField = await _customTenantFieldService.DeleteTenantField(requestmodel.Id.Value, TenantId);
                    var deleteCustomOptions = await _customControlOptionService.DeleteOptions(requestmodel.Id.Value);
                    var deleteCustomFieldValues = await _customFieldValueService.DeleteFieldValueList(requestmodel.Id.Value, customModule.Id, TenantId);
                    var deletedModuleField = _moduleFieldService.DeleteByField(requestmodel.Id.Value, customModule.Id);

                    CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                    customTableColumnDto.Name = requestmodel.Name;
                    customTableColumnDto.ControlId = requestmodel.ControlId;
                    customTableColumnDto.IsDefault = false;
                    customTableColumnDto.TenantId = Convert.ToInt32(TenantId);
                    customTableColumnDto.CustomFieldId = requestmodel.Id.Value;
                    var tableObj = _customTableService.GetByName(requestmodel.TableName);
                    if (tableObj != null)
                    {
                        customTableColumnDto.MasterTableId = tableObj.Id;
                    }
                    var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);
                    var deletedField = _customFieldService.DeleteById(requestmodel.Id.Value);
                    await _hubContext.Clients.All.OnCustomFieldAddUpdateEmit(TenantId, requestmodel.TableName);
                }
                var responsemodel = _mapper.Map<CustomModuleDeleteFieldResponse>(requestmodel);
                return new OperationResult<CustomModuleDeleteFieldResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);

            }
            else
            {
                var responsemodel = _mapper.Map<CustomModuleDeleteFieldResponse>(requestmodel);
                return new OperationResult<CustomModuleDeleteFieldResponse>(false, System.Net.HttpStatusCode.OK, "Id can not pass null", responsemodel);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<OperationResult<ModuleFieldDto>> UpdateFieldColumn([FromBody] ModuleFieldDto Model)
        {
            if (Model.Id != null)
            {
                var savedModuleList = _moduleFieldService.GetAllByField(Model.Id.Value);

                return new OperationResult<ModuleFieldDto>(true, System.Net.HttpStatusCode.OK, "", Model);
            }
            else
            {
                return new OperationResult<ModuleFieldDto>(false, System.Net.HttpStatusCode.OK, "Please provice id", Model);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<CustomModuleGetAllColumnResponse>> ColumnList([FromBody] CustomModuleGetAllColumnRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<CustomTableColumnDto>(Model);
            CustomTableDto customTableDto = new CustomTableDto();

            var selectedTable = _customTableService.GetByName(requestmodel.TableName);
            // var allTables = _customTableService.GetAll ();

            var allTableColumns = _customTableColumnService.GetAll();

            var defaultColumns = allTableColumns.Where(t => t.MasterTableId == selectedTable.Id && t.IsDefault == true).ToList();
            var customFields = allTableColumns.Where(t => t.MasterTableId == selectedTable.Id && t.IsDefault == false && t.TenantId == TenantId).ToList();
            var selectedTableColumnUsers = _tableColumnUserService.GetAllByTable(selectedTable.Id, UserId);

            List<ModuleRecordCustomField> moduleRecordFieldList = new List<ModuleRecordCustomField>();

            //var moduleObj = _customModuleService.GetByName(Model.TableName);
            CustomModule? customModuleObj = null;
            if (selectedTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(selectedTable.Id);
            }
            if (customModuleObj != null)
            {
                var moduleFieldList = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                var moduleFieldIds = moduleFieldList.Select(x => x.Id).ToList();

                moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIds);
            }

            if (moduleRecordFieldList != null && moduleRecordFieldList.Count() > 0)
            {
                foreach (var moduleRecordCustomField in moduleRecordFieldList)
                {
                    var isExistData = customFields.Where(t => t.CustomFieldId == moduleRecordCustomField.ModuleField.FieldId).FirstOrDefault();
                    if (isExistData != null)
                    {
                        customFields.Remove(isExistData);
                    }
                }
            }

            if (defaultColumns != null && defaultColumns.Count() > 0)
            {
                foreach (var item in defaultColumns)
                {
                    var Obj = selectedTableColumnUsers.Where(t => t.MasterTableId == item.MasterTableId && t.TableColumnId == item.Id).FirstOrDefault();
                    var customTableColumnDto = _mapper.Map<CustomTableColumnDto>(item);
                    if (Obj != null)
                    {
                        customTableColumnDto.IsHide = Obj.IsHide;
                        customTableColumnDto.Priority = Obj.Priority;
                    }
                    else
                    {
                        customTableColumnDto.IsHide = false;
                        customTableColumnDto.Priority = null;
                    }
                    if (item.CustomControl != null)
                    {
                        customTableColumnDto.ControlType = item.CustomControl.Name;
                    }
                    if (customTableColumnDto.IsHide)
                    {
                        customTableDto.HideColumns.Add(customTableColumnDto);
                    }
                    else
                    {
                        customTableDto.ShowColumns.Add(customTableColumnDto);
                    }
                }
            }

            if (customFields != null && customFields.Count() > 0)
            {
                foreach (var item in customFields)
                {
                    var Obj = selectedTableColumnUsers.Where(t => t.MasterTableId == item.MasterTableId && t.TableColumnId == item.Id).FirstOrDefault();
                    var customTableColumnDto = _mapper.Map<CustomTableColumnDto>(item);
                    if (Obj != null)
                    {
                        customTableColumnDto.IsHide = Obj.IsHide;
                        customTableColumnDto.Priority = Obj.Priority;
                    }
                    else
                    {
                        customTableColumnDto.IsHide = false;
                        customTableColumnDto.Priority = null;
                    }
                    if (item.CustomControl != null)
                    {
                        customTableColumnDto.ControlType = item.CustomControl.Name;
                    }
                    if (customTableColumnDto.IsHide)
                    {
                        customTableDto.HideColumns.Add(customTableColumnDto);
                    }
                    else
                    {
                        customTableDto.ShowColumns.Add(customTableColumnDto);
                    }
                }
            }
            // ColumnDto.ShowColumns = ColumnDto.ShowColumns.Where(t => t.TenantId == Model.TenantId).OrderBy(t => t.Priority).ToList();

            var nullPriorityColumns = customTableDto.ShowColumns.Where(t => t.Priority == null).ToList();
            customTableDto.ShowColumns = customTableDto.ShowColumns.Where(t => t.Priority != null).OrderBy(t => t.Priority).ToList();

            if (nullPriorityColumns != null && nullPriorityColumns.Count() > 0)
            {
                foreach (var item in nullPriorityColumns)
                {
                    customTableDto.ShowColumns.Add(item);
                }
            }
            var responseCustomTableDto = _mapper.Map<CustomModuleGetAllColumnResponse>(customTableDto);
            return new OperationResult<CustomModuleGetAllColumnResponse>(true, System.Net.HttpStatusCode.OK, "", responseCustomTableDto);
        }

        [HttpPost]
        public async Task<OperationResult<CustomTableDto>> DefaultColumns([FromBody] CustomTableColumnDto Model)
        {
            CustomTableDto ColumnDto = new CustomTableDto();

            var selectedTable = _customTableService.GetByName(Model.TableName);

            var allTableColumns = _customTableColumnService.GetAll();
            if (allTableColumns != null)
            {
                var defaultColumns = allTableColumns.Where(t => t.MasterTableId == selectedTable.Id && t.IsDefault == true).ToList();
                if (defaultColumns != null && defaultColumns.Count() > 0)
                {
                    foreach (var item in defaultColumns)
                    {
                        var customFieldDto = _mapper.Map<CustomTableColumnDto>(item);
                        ColumnDto.ShowColumns.Add(customFieldDto);
                    }
                }
            }

            return new OperationResult<CustomTableDto>(true, System.Net.HttpStatusCode.OK, "", ColumnDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<OperationResult<CustomModuleSaveColumnResponse>> SaveColumn([FromBody] CustomModuleSaveColumnRequest Model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<ModuleColumnDto>(Model);
            ModuleColumnDto moduleColumnDto = new ModuleColumnDto();
            List<TableColumnUserDto> tableColumnUserDtoList = new List<TableColumnUserDto>();
            if (requestmodel.DisplayColumns != null && requestmodel.DisplayColumns.Count() > 0)
            {
                foreach (var item in requestmodel.DisplayColumns)
                {
                    TableColumnUserDto tableColumnUserDto = new TableColumnUserDto();
                    tableColumnUserDto.MasterTableId = item.MasterTableId;
                    tableColumnUserDto.UserId = UserId;
                    tableColumnUserDto.TenantId = TenantId;
                    tableColumnUserDto.TableColumnId = item.Id;
                    tableColumnUserDto.IsHide = item.IsHide;
                    tableColumnUserDto.Priority = item.Priority;
                    tableColumnUserDtoList.Add(tableColumnUserDto);
                    var AddUpdate = await _tableColumnUserService.CheckInsertOrUpdate(tableColumnUserDto);
                    tableColumnUserDto.Id = AddUpdate.Id;
                    moduleColumnDto.ColumnUsers.Add(tableColumnUserDto);
                }
            }
            // var list = await _tableColumnUserService.CheckInsertOrUpdateList(columnUserList);
            // ColumnDto.ColumnUsers = _mapper.Map<List<TableColumnUserDto>>(list);
            var resposneModuleColumnDto = _mapper.Map<CustomModuleSaveColumnResponse>(moduleColumnDto);
            return new OperationResult<CustomModuleSaveColumnResponse>(true, System.Net.HttpStatusCode.OK, "", resposneModuleColumnDto);
        }

        [HttpPost]
        public async Task<OperationResult<CustomModuleIsExistResponse>> IsExist([FromBody] CustomModuleIsExistRequest Model)
        {
            var requestmodel = _mapper.Map<CustomFieldDto>(Model);
            CustomFieldDto fieldDto = new CustomFieldDto();
            CustomModuleIsExistResponse responsefieldDto = new CustomModuleIsExistResponse();
            if (!string.IsNullOrEmpty(requestmodel.Name))
            {
                var isExistData = _customFieldService.GetByName(requestmodel.Name);
                if (isExistData != null)
                {
                    responsefieldDto = _mapper.Map<CustomModuleIsExistResponse>(fieldDto);
                    return new OperationResult<CustomModuleIsExistResponse>(false, System.Net.HttpStatusCode.OK, "Field name already exist", responsefieldDto);
                }
                else
                {
                    responsefieldDto = _mapper.Map<CustomModuleIsExistResponse>(fieldDto);
                    return new OperationResult<CustomModuleIsExistResponse>(true, System.Net.HttpStatusCode.OK, "", responsefieldDto);
                }
            }
            else
            {
                responsefieldDto = _mapper.Map<CustomModuleIsExistResponse>(fieldDto);
                return new OperationResult<CustomModuleIsExistResponse>(false, System.Net.HttpStatusCode.OK, "Please provide field name.", responsefieldDto);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<OperationResult<List<CustomModuleGetModuleFieldResponse>>> ModuleFields()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<CustomModuleDto> customModuleDtoList = new List<CustomModuleDto>();
            var allModules = _customModuleService.GetAll();

            // Start logic for find Record CustomField ID
            var moduleRecordFields = _moduleRecordCustomFieldService.GetAll();
            var customfieldIdList = moduleRecordFields.Where(t => t.ModuleField != null && t.ModuleField.CustomField != null).Select(t => t.ModuleField.CustomField.Id).ToList();
            // End

            if (allModules != null && allModules.Count() > 0)
            {
                foreach (var moduleItem in allModules)
                {
                    CustomModuleDto customModuleDto = new CustomModuleDto();
                    customModuleDto.TenantId = TenantId;
                    customModuleDto.MasterTableId = moduleItem.MasterTableId;
                    customModuleDto.Id = moduleItem.Id;
                    customModuleDto.Name = moduleItem.Name;

                    // Model.RecordId = item.Id;
                    customModuleDto.CustomFields = await customFieldLogic.GetCustomField(customModuleDto);

                    // Start logic for remove record custom field
                    for (int i = customModuleDto.CustomFields.Count() - 1; i >= 0; i--)
                    {
                        var item = customModuleDto.CustomFields[i];
                        if (item.Id != null)
                        {
                            if (customfieldIdList.Contains(item.Id.Value))
                            {
                                customModuleDto.CustomFields.Remove(item);
                            }
                        }
                    }
                    // End

                    if (moduleItem.MasterTableId != null)
                    {
                        var tableColumns = _customTableColumnService.GetAllDefaultByTable(moduleItem.MasterTableId.Value);
                        if (tableColumns != null)
                        {
                            foreach (var columnItem in tableColumns)
                            {
                                CustomFieldDto customFieldDto = new CustomFieldDto();
                                customFieldDto.CustomTableColumnId = columnItem.Id;
                                customFieldDto.ControlId = columnItem.ControlId;
                                customFieldDto.Name = columnItem.Name;
                                customFieldDto.Id = columnItem.CustomFieldId;
                                if (columnItem.CustomControl != null)
                                {
                                    customFieldDto.CustomControl = _mapper.Map<CustomControlDto>(columnItem.CustomControl);
                                }
                                customModuleDto.CustomFields.Add(customFieldDto);
                            }
                        }
                    }

                    customModuleDtoList.Add(customModuleDto);
                }
            }
            var resposnemoduleDtos = _mapper.Map<List<CustomModuleGetModuleFieldResponse>>(customModuleDtoList);
            return new OperationResult<List<CustomModuleGetModuleFieldResponse>>(true, System.Net.HttpStatusCode.OK, "", resposnemoduleDtos);
        }

    }
}