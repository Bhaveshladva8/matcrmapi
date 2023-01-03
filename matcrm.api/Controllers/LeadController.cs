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
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class LeadController : Controller
    {

        private readonly ILeadService _leadService;
        private readonly ILeadNoteService _leadNoteService;
        private readonly IUserService _userService;
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly ILeadLabelService _leadLabelService;
        private readonly ICustomerService _customerService;
        private readonly IOrganizationService _organizationService;
        private readonly ICustomerPhoneService _customerPhoneService;
        private readonly ICustomerEmailService _customerEmailService;
        private readonly ILabelCategoryService _labelCategoryService;
        private readonly ILabelService _labelService;
        private readonly ILeadActivityService _leadActivityService;
        private readonly IActivityTypeService _leadActivityTypeService;
        private readonly IActivityAvailabilityService _leadActivityAvailabilityService;
        private readonly ILeadActivityMemberService _leadActivityMemberService;
        private readonly IModuleRecordCustomFieldService _moduleRecordCustomFieldService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private CustomFieldLogic customFieldLogic;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public LeadController(
            ILeadService leadService,
            ILeadNoteService leadNoteService,
            IUserService userService,
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomFieldService customFieldService,
            ICustomModuleService customModuleService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            ILeadLabelService leadLabelService,
            ICustomerService customerService,
            IOrganizationService organizationService,
            ICustomerPhoneService customerPhoneService,
            ICustomerEmailService customerEmailService,
            ILabelCategoryService labelCategoryService,
            ILabelService labelService,
            ILeadActivityService leadActivityService,
            IActivityTypeService leadActivityTypeService,
            IActivityAvailabilityService leadActivityAvailabilityService,
            ILeadActivityMemberService leadActivityMemberService,
            IModuleRecordCustomFieldService moduleRecordCustomFieldService,
            ICustomTableColumnService customTableColumnService,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper)
        {
            _leadService = leadService;
            _leadNoteService = leadNoteService;
            _userService = userService;
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            _customFieldService = customFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _tenantModuleService = tenantModuleService;
            _customTenantFieldService = customTenantFieldService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _leadLabelService = leadLabelService;
            _customerService = customerService;
            _organizationService = organizationService;
            _customerPhoneService = customerPhoneService;
            _customerEmailService = customerEmailService;
            _labelCategoryService = labelCategoryService;
            _labelService = labelService;
            _leadActivityService = leadActivityService;
            _leadActivityTypeService = leadActivityTypeService;
            _leadActivityAvailabilityService = leadActivityAvailabilityService;
            _leadActivityMemberService = leadActivityMemberService;
            _moduleRecordCustomFieldService = moduleRecordCustomFieldService;
            _customTableColumnService = customTableColumnService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
                customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<LeadResponse>>> List()
        {
            List<LeadDto> leadDtoList = new List<LeadDto>();


            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);


            var leadList = _leadService.GetAllByTenant(TenantId);
            leadDtoList = _mapper.Map<List<LeadDto>>(leadList);
            var leadCategoryObj = _labelCategoryService.GetByName("Lead");

            var labels = _labelService.GetAllByCategory(leadCategoryObj.Id);

            var leadLabels = _leadLabelService.GetAll();
            if (leadDtoList != null && leadDtoList.Count() > 0)
            {
                foreach (var item in leadDtoList)
                {
                    if (item.Id != null)
                    {
                        var notes = _leadNoteService.GetByLead(item.Id.Value);
                        item.Notes = _mapper.Map<List<LeadNoteDto>>(notes);
                        if (leadLabels != null)
                        {
                            var labelList1 = leadLabels.Where(t => t.LeadId == item.Id).ToList();
                            if (labelList1 != null && labelList1.Count() > 0)
                            {
                                foreach (var item1 in labelList1)
                                {
                                    LeadLabelDto leadLabelDto = new LeadLabelDto();
                                    leadLabelDto.Id = item1.Id;
                                    leadLabelDto.LeadId = item.Id;
                                    leadLabelDto.LabelId = item1.LabelId;
                                    if (labels != null)
                                    {
                                        var labelObj = labels.Where(t => t.Id == item1.LabelId).FirstOrDefault();
                                        if (labelObj != null)
                                        {
                                            leadLabelDto.Name = labelObj.Name;
                                            leadLabelDto.Color = labelObj.Color;
                                            item.Labels.Add(leadLabelDto);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //var LeadModule = _customModuleService.GetByName("Lead");
            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("Lead");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            if (customModuleObj != null)
            {
                CustomModuleDto customModuleDto = new CustomModuleDto();
                customModuleDto.TenantId = TenantId;
                customModuleDto.MasterTableId = customModuleObj.MasterTableId;
                customModuleDto.Id = customModuleObj.Id;
                if (leadDtoList != null && leadDtoList.Count() > 0)
                {
                    foreach (var item in leadDtoList)
                    {
                        customModuleDto.RecordId = item.Id;
                        item.CustomFields = await customFieldLogic.GetCustomField(customModuleDto);
                    }
                }
            }
            var responseLeadDtoList = _mapper.Map<List<LeadResponse>>(leadDtoList);
            return new OperationResult<List<LeadResponse>>(true, System.Net.HttpStatusCode.OK, "", responseLeadDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<LeadLabelDto>>> Labels()
        {
            List<LeadLabelDto> leadLabelDtoList = new List<LeadLabelDto>();
            var labelList = _leadLabelService.GetAll();
            leadLabelDtoList = _mapper.Map<List<LeadLabelDto>>(labelList);

            return new OperationResult<List<LeadLabelDto>>(true, System.Net.HttpStatusCode.OK, "", leadLabelDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<LeadDto>>> BasedOnTenant()
        {
            List<LeadDto> leadDtoList = new List<LeadDto>();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var leadList = _leadService.GetAllByTenant(TenantId);
            leadDtoList = _mapper.Map<List<LeadDto>>(leadList);
            return new OperationResult<List<LeadDto>>(true, System.Net.HttpStatusCode.OK, "", leadDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<LeadAddUpdateResponse>> AddUpdate([FromBody] LeadAddUpdateRequest model)
        {
            LeadDto leadDto = new LeadDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<LeadDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            if (requestmodel.OrganizationId == null && !string.IsNullOrEmpty(requestmodel.OrganizationName))
            {
                OrganizationDto organizationDto = new OrganizationDto();
                organizationDto.Name = requestmodel.OrganizationName;
                organizationDto.TenantId = TenantId;
                organizationDto.CreatedBy = UserId;
                var OrganizationObj = await _organizationService.CheckInsertOrUpdate(organizationDto);
                requestmodel.OrganizationId = OrganizationObj.Id;
            }

            if (requestmodel.CustomerId == null && !string.IsNullOrEmpty(requestmodel.CustomerName))
            {
                CustomerDto customerDto = new CustomerDto();
                customerDto.Name = requestmodel.CustomerName;
                customerDto.OrganizationId = requestmodel.OrganizationId;
                customerDto.TenantId = TenantId;
                customerDto.CreatedBy = UserId;
                var customerObj = await _customerService.CheckInsertOrUpdate(customerDto);

                requestmodel.CustomerId = customerObj.Id;

                if (customerObj != null && requestmodel.Phones != null && requestmodel.Phones.Count() > 0)
                {
                    foreach (var item in requestmodel.Phones)
                    {
                        item.CustomerId = customerObj.Id;
                        item.TenantId = TenantId;

                        if (item.Id != null)
                        {
                            item.UpdatedBy = Convert.ToInt16(UserId);
                        }
                        else
                        {
                            item.CreatedBy = Convert.ToInt16(UserId);
                        }
                        var AddUpdatePhone = await _customerPhoneService.CheckInsertOrUpdate(item);
                    }
                }

                if (customerObj != null && requestmodel.Emails != null && requestmodel.Emails.Count() > 0)
                {
                    foreach (var item in requestmodel.Emails)
                    {
                        item.CustomerId = customerObj.Id;
                        item.TenantId = TenantId;

                        if (item.Id != null)
                        {
                            item.UpdatedBy = Convert.ToInt16(UserId);
                        }
                        else
                        {
                            item.CreatedBy = Convert.ToInt16(UserId);
                        }
                        var AddUpdateEmail = _customerEmailService.CheckInsertOrUpdate(item);
                    }
                }
            }

            var leadObj = _leadService.CheckInsertOrUpdate(requestmodel);

            leadDto = _mapper.Map<LeadDto>(leadObj);

            var labels = _leadLabelService.GetAllByLeadId(leadObj.Id);

            List<LeadLabel> removeLabelList = new List<LeadLabel>();
            if (labels != null && labels.Count() > 0)
            {
                foreach (var item in labels)
                {
                    if (requestmodel.LabelIds != null)
                    {
                        int? isExistData = requestmodel.LabelIds.Where(t => t == item.LabelId).FirstOrDefault();
                        if (isExistData == null || isExistData == 0)
                        {
                            removeLabelList.Add(item);
                        }
                    }
                }
            }

            if (removeLabelList != null && removeLabelList.Count() > 0)
            {
                foreach (var LeadLabelItem in removeLabelList)
                {
                    var deleted = await _leadLabelService.DeleteLeadLabel(LeadLabelItem.Id);
                }
            }

            if (requestmodel.LabelIds != null && requestmodel.LabelIds.Count() > 0)
            {
                foreach (var item in requestmodel.LabelIds)
                {
                    LeadLabelDto leadLabelDto = new LeadLabelDto();
                    leadLabelDto.LabelId = item;
                    leadLabelDto.LeadId = leadObj.Id;
                    leadLabelDto.CreatedBy = UserId;
                    leadLabelDto.TenantId = TenantId;
                    var LeadLabelAddUpdate = await _leadLabelService.CheckInsertOrUpdate(leadLabelDto);
                }
            }

            labels = _leadLabelService.GetAllByLeadId(leadObj.Id);
            leadDto.Labels = _mapper.Map<List<LeadLabelDto>>(labels);

            //var taskStatusModule = _customModuleService.GetByName("Lead");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("Lead");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            if (requestmodel.CustomFields != null && requestmodel.CustomFields.Count() > 0 && customModuleObj != null)
            {
                foreach (var item in requestmodel.CustomFields)
                {
                    CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                    customFieldValueDto.FieldId = item.Id;
                    customFieldValueDto.ModuleId = customModuleObj.Id;
                    customFieldValueDto.RecordId = leadObj.Id;
                    var controlType = "";
                    if (item.CustomControl != null)
                    {
                        controlType = item.CustomControl.Name;
                        customFieldValueDto.ControlType = controlType;
                    }
                    customFieldValueDto.Value = item.Value;
                    customFieldValueDto.CreatedBy = UserId;
                    customFieldValueDto.TenantId = TenantId;
                    if (item.CustomControlOptions != null && item.CustomControlOptions.Count() > 0)
                    {

                        var selectedOptionList = item.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
                        if (controlType == "Checkbox")
                        {
                            var deletedList = await _customFieldValueService.DeleteList(customFieldValueDto);
                        }
                        foreach (var option in selectedOptionList)
                        {
                            customFieldValueDto.OptionId = option.Id;
                            var AddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                        }
                    }
                    else
                    {
                        var AddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                    }

                }
            }
            var responseLeadDto = _mapper.Map<LeadAddUpdateResponse>(leadDto);
            return new OperationResult<LeadAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseLeadDto);
        }

        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]        
        // [HttpDelete]
        // public async Task<OperationResult<DeleteLeadResponse>> Remove(DeleteLeadRequest model)
        // {
        //     var requestmodel = _mapper.Map<LeadDto>(model);
        //     DeleteLeadResponse responsemodel  = new DeleteLeadResponse();
        //     if (requestmodel.Id != null)
        //     {
        //         var LeadId = requestmodel.Id.Value;

        //         var notes = _leadNoteService.DeleteByLead(LeadId);
        //         var leadObj = _leadService.DeleteLead(requestmodel);

        //         // Start logic for Record wise delete custom field
        //         //var moduleObj = _customModuleService.GetByName("Lead");
        //         CustomModule? customModuleObj = null;
        //         var customTable = _customTableService.GetByName("Lead");
        //         if (customTable != null)
        //         {
        //             customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
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
        //                     if (moduleRecordField.RecordId == requestmodel.Id)
        //                     {
        //                         var deletedModuleRecordField = await _moduleRecordCustomFieldService.DeleteById(moduleRecordField.Id);

        //                         var moduleFieldId = moduleRecordField.ModuleFieldId;
        //                         long? customFieldId1 = null;
        //                         if (moduleRecordField.ModuleField.CustomField != null)
        //                         {
        //                             customFieldId1 = moduleRecordField.ModuleField.CustomField.Id;
        //                         }

        //                         if (moduleFieldId != null)
        //                         {
        //                             var deleteModuleField = _moduleFieldService.Delete(moduleFieldId.Value);
        //                         }

        //                         if (customFieldId1 != null && leadObj.TenantId != null)
        //                         {
        //                             var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(customFieldId1.Value, leadObj.TenantId.Value);
        //                         }

        //                         CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
        //                         customTableColumnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
        //                         customTableColumnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
        //                         customTableColumnDto.IsDefault = false;
        //                         customTableColumnDto.TenantId = leadObj.TenantId;
        //                         if (customFieldId1 != null)
        //                         {
        //                             customTableColumnDto.CustomFieldId = customFieldId1;
        //                         }
        //                         // var tableObj = _customTableService.GetByName("Person");
        //                         // if (tableObj != null)
        //                         // {
        //                         customTableColumnDto.MasterTableId = customModuleObj.MasterTableId;
        //                         // }
        //                         var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);

        //                         if (customFieldId1 != null)
        //                         {
        //                             var deleteTableColumns1 = _customFieldService.DeleteById(customFieldId1.Value);
        //                         }

        //                     }
        //                 }
        //             }
        //         }
        //         // End logic for Record wise delete custom field
        //         responsemodel = _mapper.Map<DeleteLeadResponse>(requestmodel);
        //         return new OperationResult<DeleteLeadResponse>(true, System.Net.HttpStatusCode.OK,"", responsemodel);                
        //     }
        //     else
        //     {
        //         responsemodel = _mapper.Map<DeleteLeadResponse>(requestmodel);
        //         return new OperationResult<DeleteLeadResponse>(false, System.Net.HttpStatusCode.OK,"Id can not be null", responsemodel);
        //     }
        // }


        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            DeleteLeadResponse responsemodel = new DeleteLeadResponse();
            if (Id != null && Id > 0)
            {
                var LeadId = Id;

                var notes = _leadNoteService.DeleteByLead(LeadId);
                 var allActivity = _leadActivityService.GetByLead(LeadId);

                foreach (var LeadActivityObj in allActivity)
                {
                    var deletedActivityMembers = await _leadActivityMemberService.DeleteByActivityId(LeadActivityObj.Id);
                }

                var deletedAllActivities = await _leadActivityService.DeleteByLead(LeadId);
                var leadObj = await _leadService.DeleteLead(Id);

                if (leadObj != null)
                {
                    CustomModule? customModuleObj = null;
                    var customTable = _customTableService.GetByName("Lead");
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
                            customFieldValueDtoObj.RecordId = leadObj.Id;
                            var deletedRecord = await _customFieldValueService.DeleteList(customFieldValueDtoObj);
                        }
                    }

                    // Start logic for Record wise delete custom field

                    if (customModuleObj != null)
                    {
                        var moduleFields = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                        var moduleFieldIdList = moduleFields.Select(t => t.Id).ToList();

                        var moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIdList);
                        if (moduleRecordFieldList != null && moduleRecordFieldList.Count() > 0)
                        {
                            foreach (var moduleRecordField in moduleRecordFieldList)
                            {
                                if (moduleRecordField.RecordId == Id)
                                {
                                    var deletedModuleRecordField = await _moduleRecordCustomFieldService.DeleteById(moduleRecordField.Id);

                                    var moduleFieldId = moduleRecordField.ModuleFieldId;
                                    long? customFieldId1 = null;
                                    if (moduleRecordField.ModuleField.CustomField != null)
                                    {
                                        customFieldId1 = moduleRecordField.ModuleField.CustomField.Id;
                                    }

                                    if (moduleFieldId != null)
                                    {
                                        var deleteModuleField = _moduleFieldService.Delete(moduleFieldId.Value);
                                    }

                                    if (customFieldId1 != null && leadObj.TenantId != null)
                                    {
                                        var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(customFieldId1.Value, leadObj.TenantId.Value);
                                    }

                                    CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                                    customTableColumnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
                                    customTableColumnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
                                    customTableColumnDto.IsDefault = false;
                                    customTableColumnDto.TenantId = leadObj.TenantId;
                                    if (customFieldId1 != null)
                                    {
                                        customTableColumnDto.CustomFieldId = customFieldId1;
                                    }
                                    // var tableObj = _customTableService.GetByName("Person");
                                    // if (tableObj != null)
                                    // {
                                    customTableColumnDto.MasterTableId = customModuleObj.MasterTableId;
                                    // }
                                    var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);

                                    if (customFieldId1 != null)
                                    {
                                        var deleteTableColumns1 = _customFieldService.DeleteById(customFieldId1.Value);
                                    }

                                }
                            }
                        }
                    }
                    // End logic for Record wise delete custom field
                }
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide lead id", Id);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<GetLeadResponse>> Detail(int Id)
        {
            Lead leadObj = new Lead();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            leadObj = _leadService.GetById(Id);
            var leadDto = _mapper.Map<LeadDto>(leadObj);

            if (leadObj.CustomerId != null)
            {
                leadDto.CustomerDto = _mapper.Map<CustomerDto>(leadObj.Customer);
            }
            if (leadObj.OrganizationId != null)
            {
                leadDto.OrganizationDto = _mapper.Map<OrganizationDto>(leadObj.Organization);
            }

            var notes = _leadNoteService.GetByLead(Id);

            leadDto.Notes = _mapper.Map<List<LeadNoteDto>>(notes);

            var users = _userService.GetAll();

            var leadCategoryObj = _labelCategoryService.GetByName("Lead");

            var labels = _labelService.GetAllByCategory(leadCategoryObj.Id);

            var leadLabels = _leadLabelService.GetAll();

            var activityTypes = _leadActivityTypeService.GetAll();
            var activityAvailabilities = _leadActivityAvailabilityService.GetAll();

            var allActivity = _leadActivityService.GetByLead(Id);

            if (allActivity != null && allActivity.Count() > 0)
            {
                var plannedActivities = allActivity.Where(t => t.IsDone == false).ToList();
                var completedActivities = allActivity.Where(t => t.IsDone == true).ToList();

                leadDto.PlannedActivities = _mapper.Map<List<LeadActivityDto>>(plannedActivities);
                leadDto.CompletedActivities = _mapper.Map<List<LeadActivityDto>>(completedActivities);
            }

            if (leadLabels != null && leadLabels.Count() > 0)
            {
                var labelList1 = leadLabels.Where(t => t.LeadId == Id).ToList();
                if (labelList1 != null && labelList1.Count() > 0)
                {
                    foreach (var item1 in labelList1)
                    {
                        LeadLabelDto leadLabelDto = new LeadLabelDto();
                        leadLabelDto.Id = item1.Id;
                        leadLabelDto.LeadId = Id;
                        leadLabelDto.LabelId = item1.LabelId;

                        var labelObj = labels.Where(t => t.Id == item1.LabelId).FirstOrDefault();
                        leadLabelDto.Name = labelObj.Name;
                        leadLabelDto.Color = labelObj.Color;
                        leadDto.Labels.Add(leadLabelDto);
                    }
                }
            }

            if (leadDto.Notes != null && leadDto.Notes.Count() > 0)
            {
                foreach (var item in leadDto.Notes)
                {
                    if (users != null && users.Count() > 0)
                    {
                        var userObj = users.Where(t => t.Id == item.CreatedBy).FirstOrDefault();
                        if (userObj != null)
                        {
                            item.FirstName = userObj.FirstName;
                            item.LastName = userObj.LastName;
                            item.Email = userObj.Email;
                            if (item.FirstName != null)
                            {
                                item.ShortName = item.FirstName.Substring(0, 1);
                            }
                            if (item.LastName != null)
                            {
                                item.ShortName = item.ShortName + item.LastName.Substring(0, 1);
                            }
                        }
                    }
                }
            }

            if (leadDto.PlannedActivities != null && leadDto.PlannedActivities.Count() > 0)
            {
                foreach (var item in leadDto.PlannedActivities)
                {
                    if (item.Id != null)
                    {
                        var members = _leadActivityMemberService.GetAllByActivity(item.Id.Value);

                        if (item.LeadActivityTypeId != null)
                        {
                            if (activityTypes != null && activityTypes.Count() > 0)
                            {
                                var activityTypeObj = activityTypes.Where(t => t.Id == item.LeadActivityTypeId).FirstOrDefault();
                                if (activityTypeObj != null)
                                {
                                    item.LeadActivityType = activityTypeObj.Name;
                                }
                            }
                        }

                        if (item.LeadActivityAvailabilityId != null)
                        {
                            if (activityAvailabilities != null && activityAvailabilities.Count() > 0)
                            {
                                var activityAvailabilityObj = activityAvailabilities.Where(t => t.Id == item.LeadActivityAvailabilityId).FirstOrDefault();
                                if (activityAvailabilityObj != null)
                                {
                                    item.LeadActivityAvailability = activityAvailabilityObj.Name;
                                }
                            }
                        }

                        item.Members = _mapper.Map<List<LeadActivityMemberDto>>(members);
                        if (users != null)
                        {
                            var userObj = users.Where(t => t.Id == item.CreatedBy).FirstOrDefault();
                            if (userObj != null)
                            {
                                item.FirstName = userObj.FirstName;
                                item.LastName = userObj.LastName;
                                item.Email = userObj.Email;
                                if (item.FirstName != null)
                                {
                                    item.ShortName = item.FirstName.Substring(0, 1);
                                }
                                if (item.LastName != null)
                                {
                                    item.ShortName = item.ShortName + item.LastName.Substring(0, 1);
                                }
                            }
                        }
                    }
                }
            }

            if (leadDto.CompletedActivities != null && leadDto.CompletedActivities.Count() > 0)
            {
                foreach (var completeActivityObj in leadDto.CompletedActivities)
                {
                    if (completeActivityObj.Id != null)
                    {
                        var members = _leadActivityMemberService.GetAllByActivity(completeActivityObj.Id.Value);

                        if (completeActivityObj.LeadActivityTypeId != null)
                        {
                            if (activityTypes != null)
                            {
                                var activityTypeObj = activityTypes.Where(t => t.Id == completeActivityObj.LeadActivityTypeId).FirstOrDefault();
                                if (activityTypeObj != null)
                                {
                                    completeActivityObj.LeadActivityType = activityTypeObj.Name;
                                }
                            }
                        }

                        if (completeActivityObj.LeadActivityAvailabilityId != null)
                        {
                            if (activityAvailabilities != null)
                            {
                                var activityAvailabilityObj = activityAvailabilities.Where(t => t.Id == completeActivityObj.LeadActivityAvailabilityId).FirstOrDefault();
                                if (activityAvailabilityObj != null)
                                {
                                    completeActivityObj.LeadActivityAvailability = activityAvailabilityObj.Name;
                                }
                            }
                        }

                        completeActivityObj.Members = _mapper.Map<List<LeadActivityMemberDto>>(members);
                        if (users != null)
                        {
                            var userObj = users.Where(t => t.Id == completeActivityObj.CreatedBy).FirstOrDefault();
                            if (userObj != null)
                            {
                                completeActivityObj.FirstName = userObj.FirstName;
                                completeActivityObj.LastName = userObj.LastName;
                                completeActivityObj.Email = userObj.Email;
                                if (completeActivityObj.FirstName != null)
                                {
                                    completeActivityObj.ShortName = completeActivityObj.FirstName.Substring(0, 1);
                                }
                                if (completeActivityObj.LastName != null)
                                {
                                    completeActivityObj.ShortName = completeActivityObj.ShortName + completeActivityObj.LastName.Substring(0, 1);
                                }
                            }
                        }
                    }
                }
            }

            //var leadModule = _customModuleService.GetByName("Lead");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("Lead");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            if (customModuleObj != null)
            {
                CustomModuleDto customModuleDto = new CustomModuleDto();
                customModuleDto.TenantId = TenantId;
                customModuleDto.MasterTableId = customModuleObj.MasterTableId;
                customModuleDto.Id = customModuleObj.Id;
                customModuleDto.RecordId = Id;

                var customFields = await customFieldLogic.GetCustomField(customModuleDto);

                List<ModuleRecordCustomField> moduleRecordCustomFieldList = new List<ModuleRecordCustomField>();

                var moduleFieldList = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                var moduleFieldIds = moduleFieldList.Select(x => x.Id).ToList();

                moduleRecordCustomFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIds);


                if (moduleRecordCustomFieldList != null && moduleRecordCustomFieldList.Count() > 0)
                {
                    foreach (var moduleRecordCustomField in moduleRecordCustomFieldList)
                    {
                        var isExistData = customFields.Where(t => t.Id == moduleRecordCustomField.ModuleField.FieldId).FirstOrDefault();
                        if (isExistData != null && moduleRecordCustomField.RecordId != Id)
                        {
                            customFields.Remove(isExistData);
                        }
                    }
                }

                leadDto.CustomFields = customFields;
            }
            var responsemodel = _mapper.Map<GetLeadResponse>(leadDto);
            return new OperationResult<GetLeadResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }
    }
}