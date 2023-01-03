using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;

namespace matcrm.service.BusinessLogic
{
    public class CustomFieldLogic
    {
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        // private readonly ICustomerService _customerService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private IMapper _mapper;
        public CustomFieldLogic(
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            // ICustomerService customerService,
            ICustomFieldService customFieldService,
            ICustomModuleService customModuleService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            IMapper mapper
        )
        {
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            // _customerService = customerService;
            _customFieldService = customFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _tenantModuleService = tenantModuleService;
            _customTenantFieldService = customTenantFieldService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _mapper = mapper;
        }

        public async Task<List<CustomFieldDto>> GetCustomField(CustomModuleDto Model)
        {
            List<CustomFieldDto> customFieldDtoList = new List<CustomFieldDto>();
            var customModuleList = _customModuleService.GetAll();
            var customTableList = _customTableService.GetAll();
            CustomModuleDto customModuleDtoObj = new CustomModuleDto();
            if (Model.Id != null)
            {
                var customModuleObj = _customModuleService.GetById(Model.Id.Value);
                if (customModuleObj != null)
                {
                    customModuleDtoObj = _mapper.Map<CustomModuleDto>(customModuleObj);
                    var tenantModuleObj = _tenantModuleService.GetTenantModule(Model.Id.Value, Model.TenantId.Value);
                    if (tenantModuleObj != null)
                    {
                        customModuleDtoObj.TenantModule = _mapper.Map<TenantModuleDto>(tenantModuleObj);
                    }
                    var moduleFields = _moduleFieldService.GetAllModuleField(Model.Id.Value);

                    if (moduleFields != null)
                    {
                        customModuleDtoObj.ModuleFields = _mapper.Map<List<ModuleFieldDto>>(moduleFields);
                    }
                    if (customModuleObj.MasterTableId != null)
                    {
                        var customTableObj = _customTableService.GetById(customModuleObj.MasterTableId.Value);

                        if (customTableObj != null)
                        {
                            customModuleDtoObj.CustomTable = _mapper.Map<CustomTableDto>(customTableObj);
                        }
                    }

                    var customTenantFieldList = new List<CustomTenantField>();
                    var updatedModuleFieldList = new List<ModuleFieldDto>();
                    foreach (var item in moduleFields)
                    {
                        if (item.FieldId != null && Model.TenantId != null)
                        {
                            var customTenantFieldObj = _customTenantFieldService.GetCustomTenantField(item.FieldId.Value, Model.TenantId.Value);
                            if (customTenantFieldObj != null)
                            {
                                customTenantFieldList.Add(customTenantFieldObj);
                                // var Obj = customModuleDto.ModuleFields.Where (t => t.FieldId == item.FieldId.Value).FirstOrDefault ();
                                // updatedModuleFields.Add (Obj);
                            }
                        }
                    }

                    // customModuleDto.ModuleFields = updatedModuleFields;
                    foreach (var item in customTenantFieldList)
                    {
                        if (item.FieldId != null)
                        {
                            var customField = _customFieldService.GetById(item.FieldId.Value);
                            if (customModuleDtoObj.CustomFields == null)
                            {
                                customModuleDtoObj.CustomFields = new List<CustomFieldDto>();
                            }
                            var customfieldDtoObj = _mapper.Map<CustomFieldDto>(customField);
                            var customControlOptions = _customControlOptionService.GetAllControlOption(item.FieldId.Value);
                            customfieldDtoObj.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(customControlOptions);

                            // var customControl = _customControlService.GetControl (customfield.ControlId.Value);
                            // customfieldDto.CustomControl = _mapper.Map<CustomControlDto> (customControl);

                            customfieldDtoObj.CustomControl = _mapper.Map<CustomControlDto>(customField.CustomControl);
                            List<CustomFieldValue> customFieldValueList = new List<CustomFieldValue>();
                            if (Model.Id != null && Model.TenantId != null && Model.RecordId != null)
                            {
                                customFieldValueList = _customFieldValueService.GetAllValues(customField.Id, Model.TenantId.Value, Model.Id.Value, Model.RecordId.Value);
                            }

                            customfieldDtoObj.CustomFieldValues = _mapper.Map<List<CustomFieldValueDto>>(customFieldValueList);

                            List<CustomControlOption> customControlOptionList = _customControlOptionService.GetAllControlOption(customField.Id);
                            foreach (var item1 in customfieldDtoObj.CustomFieldValues)
                            {
                                if (item1.OptionId != null)
                                {
                                    var optionObj = customControlOptionList.Where(t => t.Id == item1.OptionId.Value).FirstOrDefault();
                                    if (optionObj != null)
                                    {
                                        item1.Option = optionObj.Option;
                                        item1.Value = optionObj.Option;
                                    }
                                }
                            }

                            // Start logic for get linked entities
                            List<CustomTableDto> customTableDtoList = new List<CustomTableDto>();
                            var moduleFieldList = _moduleFieldService.GetAllByField(item.FieldId.Value);
                            if (moduleFieldList.Count() > 0)
                            {
                                foreach (var moduleFieldObj in moduleFieldList)
                                {
                                    var moduleId = moduleFieldObj.ModuleId;
                                    var moduleObj = customModuleList.Where(t => t.MasterTableId == moduleId).FirstOrDefault();
                                    if (moduleObj != null)
                                    {
                                        var tableObj = customTableList.Where(t => t.Id == moduleObj.MasterTableId).FirstOrDefault();
                                        var tableDto = _mapper.Map<CustomTableDto>(tableObj);
                                        customTableDtoList.Add(tableDto);
                                    }

                                    if (moduleFieldObj.FieldId == customfieldDtoObj.Id)
                                    {
                                        customfieldDtoObj.IsHide = moduleFieldObj.IsHide;
                                    }
                                }
                            }
                            customfieldDtoObj.LinkedEntities = customTableDtoList;
                            // End logic for get linked entities

                            customModuleDtoObj.CustomFields.Add(customfieldDtoObj);
                            customFieldDtoList.Add(customfieldDtoObj);
                        }
                    }
                }
            }
            // return new OperationResult<List<CustomFieldDto>> (true, "", customFieldDtos);
            return customFieldDtoList;
        }
    }
}