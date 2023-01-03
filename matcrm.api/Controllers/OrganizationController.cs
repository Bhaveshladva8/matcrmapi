using System;
using System.Collections.Generic;
using System.Dynamic;
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
using matcrm.data.Context;


namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class OrganizationController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly IOrganizationNoteService _organizationNoteService;
        private readonly IOrganizationAttachmentService _organizationAttachmentService;
        private readonly IUserService _userService;
        private readonly IOrganizationNotesCommentService _organizationNotesCommentService;
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly IOrganizationLabelService _organizationLabelService;
        private readonly IOrganizationActivityService _organizationActivityService;
        private readonly IActivityTypeService _organizationActivityTypeService;
        private readonly IActivityAvailabilityService _organizationActivityAvailabilityService;
        private readonly IOrganizationActivityMemberService _organizationActivityMemberService;
        private readonly IModuleRecordCustomFieldService _moduleRecordCustomFieldService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IERPSystemColumnMapService _eRPSystemColumnMapService;
        private readonly IWeClappService _weClappService;
        private readonly IWeClappUserService _weClappUserService;
        private readonly ILabelCategoryService _labelCategoryService;
        private readonly ILabelService _labelService;
        private CustomFieldLogic customFieldLogic;
        private IMapper _mapper;
        private readonly OneClappContext _context;
        private int UserId = 0;
        private int TenantId = 0;
        public OrganizationController(
            IOrganizationService organizationService,
            IOrganizationNoteService organizationNoteService,
            IOrganizationAttachmentService organizationAttachmentService,
            IUserService userService,
            IOrganizationNotesCommentService organizationNotesCommentService,
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomFieldService customFieldService,
            ICustomModuleService customModuleService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            IOrganizationLabelService organizationLabelService,
            IOrganizationActivityService organizationActivityService,
            IActivityTypeService organizationActivityTypeService,
            IActivityAvailabilityService organizationActivityAvailabilityService,
            IOrganizationActivityMemberService organizationActivityMemberService,
            IModuleRecordCustomFieldService moduleRecordCustomFieldService,
            ICustomTableColumnService customTableColumnService,
            IERPSystemColumnMapService eRPSystemColumnMapService,
            IWeClappService weClappService,
            IWeClappUserService weClappUserService,
            ILabelCategoryService labelCategoryService,
            ILabelService labelService,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper,
            OneClappContext context)
        {
            _organizationService = organizationService;
            _organizationNoteService = organizationNoteService;
            _organizationAttachmentService = organizationAttachmentService;
            _userService = userService;
            _organizationNotesCommentService = organizationNotesCommentService;
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            _customFieldService = customFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _tenantModuleService = tenantModuleService;
            _customTenantFieldService = customTenantFieldService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _organizationLabelService = organizationLabelService;
            _organizationActivityService = organizationActivityService;
            _organizationActivityTypeService = organizationActivityTypeService;
            _organizationActivityMemberService = organizationActivityMemberService;
            _organizationActivityAvailabilityService = organizationActivityAvailabilityService;
            _moduleRecordCustomFieldService = moduleRecordCustomFieldService;
            _customTableColumnService = customTableColumnService;
            _hostingEnvironment = hostingEnvironment;
            _eRPSystemColumnMapService = eRPSystemColumnMapService;
            _weClappService = weClappService;
            _weClappUserService = weClappUserService;
            _labelCategoryService = labelCategoryService;
            _labelService = labelService;
            _mapper = mapper;
            _context = context;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
                customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<GetAllOrganizationResponse>>> List()
        {
            List<OrganizationDto> OrganizationDtos = new List<OrganizationDto>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var OrganizationList = _organizationService.GetAllByTenant(TenantId);
            OrganizationDtos = _mapper.Map<List<OrganizationDto>>(OrganizationList);
            foreach (var item in OrganizationDtos)
            {
                var organizationLabel = _organizationLabelService.GetByOrganization(item.Id.Value);
                if (organizationLabel != null)
                {
                    item.LabelId = organizationLabel.LabelId;
                }
                var notes = _organizationNoteService.GetByOrganization(item.Id.Value);
                var documents = _organizationAttachmentService.GetAllByOrganizationId(item.Id.Value);

                item.Notes = _mapper.Map<List<OrganizationNoteDto>>(notes);
                item.Documents = _mapper.Map<List<OrganizationAttachmentDto>>(documents);
            }

            var organizationModule = _customModuleService.GetByName("Organization");
            if (organizationModule != null)
            {
                CustomModuleDto Model = new CustomModuleDto();
                Model.TenantId = TenantId;
                Model.MasterTableId = organizationModule.MasterTableId;
                Model.Id = organizationModule.Id;

                foreach (var item in OrganizationDtos)
                {
                    Model.RecordId = item.Id;
                    item.CustomFields = await customFieldLogic.GetCustomField(Model);
                }
            }
            var responseDtos = _mapper.Map<List<GetAllOrganizationResponse>>(OrganizationDtos);
            return new OperationResult<List<GetAllOrganizationResponse>>(true, System.Net.HttpStatusCode.OK, "", responseDtos);
        }


        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OrganizationAddUpdateResponse>> AddUpdate([FromBody] OrganizationAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<OrganizationDto>(model);

            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;

            var OrganizationObj = await _organizationService.CheckInsertOrUpdate(requestmodel);

            var taskStatusModule = _customModuleService.GetByName("Organization");

            OrganizationLabelDto labelDto = new OrganizationLabelDto();
            if (requestmodel.LabelId != null)
            {
                labelDto.LabelId = requestmodel.LabelId;
                labelDto.CreatedBy = UserId;
                labelDto.OrganizationId = OrganizationObj.Id;
                labelDto.TenantId = TenantId;
                var labelObj = await _organizationLabelService.CheckInsertOrUpdate(labelDto);
                labelDto.Id = labelObj.Id;
            }

            if (requestmodel.CustomFields.Count() > 0)
            {
                foreach (var item in requestmodel.CustomFields)
                {
                    CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                    customFieldValueDto.FieldId = item.Id;
                    customFieldValueDto.ModuleId = taskStatusModule.Id;
                    customFieldValueDto.RecordId = OrganizationObj.Id;
                    var controlType = "";
                    if (item.CustomControl != null)
                    {
                        controlType = item.CustomControl.Name;
                        customFieldValueDto.ControlType = controlType;
                    }
                    customFieldValueDto.Value = item.Value;
                    customFieldValueDto.CreatedBy = UserId;
                    customFieldValueDto.TenantId = TenantId;
                    if (item.CustomControlOptions.Count() > 0)
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
            var responseObj = _mapper.Map<OrganizationAddUpdateResponse>(OrganizationObj);
            return new OperationResult<OrganizationAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }

        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        // [HttpDelete]
        // public async Task<OperationResult<DeleteOrganizationResponse>> Remove(DeleteOrganizationRequest model)
        // {
        //     var requestmodel = _mapper.Map<OrganizationDto>(model);
        //     DeleteOrganizationResponse responsemodel = new DeleteOrganizationResponse();
        //     if (requestmodel.Id != null)
        //     {
        //         var organizationId = requestmodel.Id.Value;
        //         var documents = await _organizationAttachmentService.DeleteAttachmentByOrganizationId(organizationId);

        //         // Remove organization documents from folder

        //         foreach (var OrganizationDoc in documents)
        //         {

        //             var dirPath = _hostingEnvironment.WebRootPath + "\\OrganizationUpload";
        //             var filePath = dirPath + "\\" + OrganizationDoc.FileName;

        //             if (System.IO.File.Exists(filePath))
        //             {
        //                 System.IO.File.Delete(Path.Combine(filePath));
        //             }
        //         }

        //         var notes = await _organizationNoteService.DeleteByOrganization(organizationId);
        //         var OrganizationObj = _organizationService.DeleteOrganization(requestmodel);

        //         // Start logic for Record wise delete custom field
        //         var moduleObj = _customModuleService.GetByName("Organization");
        //         if (moduleObj != null)
        //         {

        //             var moduleFields = _moduleFieldService.GetAllModuleField(moduleObj.Id);

        //             var moduleFieldIdList = moduleFields.Select(t => t.Id).ToList();

        //             var moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIdList);

        //             foreach (var moduleRecordField in moduleRecordFieldList)
        //             {
        //                 if (moduleRecordField.RecordId == requestmodel.Id)
        //                 {
        //                     var DeletedModuleRecordField = await _moduleRecordCustomFieldService.DeleteById(moduleRecordField.Id);

        //                     var moduleFieldId = moduleRecordField.ModuleFieldId;
        //                     long? CustomFieldId1 = null;
        //                     if (moduleRecordField.ModuleField.CustomField != null)
        //                     {
        //                         CustomFieldId1 = moduleRecordField.ModuleField.CustomField.Id;
        //                     }

        //                     if (moduleFieldId != null)
        //                     {
        //                         var DeleteModuleField = _moduleFieldService.Delete(moduleFieldId.Value);
        //                     }

        //                     if (CustomFieldId1 != null)
        //                     {
        //                         var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(CustomFieldId1.Value, OrganizationObj.TenantId.Value);
        //                     }

        //                     CustomTableColumnDto columnDto = new CustomTableColumnDto();
        //                     columnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
        //                     columnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
        //                     columnDto.IsDefault = false;
        //                     columnDto.TenantId = OrganizationObj.TenantId;
        //                     if (CustomFieldId1 != null)
        //                     {
        //                         columnDto.CustomFieldId = CustomFieldId1;
        //                     }
        //                     // var tableObj = _customTableService.GetByName("Person");
        //                     // if (tableObj != null)
        //                     // {
        //                     columnDto.MasterTableId = moduleObj.Id;
        //                     // }
        //                     var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(columnDto);

        //                     if (CustomFieldId1 != null)
        //                     {
        //                         var deleteTableColumns1 = _customFieldService.DeleteById(CustomFieldId1.Value);
        //                     }

        //                 }
        //             }
        //         }
        //         // End logic for Record wise delete custom field
        //         responsemodel = _mapper.Map<DeleteOrganizationResponse>(requestmodel);
        //         return new OperationResult<DeleteOrganizationResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        //     }
        //     else
        //     {
        //         responsemodel = _mapper.Map<DeleteOrganizationResponse>(requestmodel);
        //         return new OperationResult<DeleteOrganizationResponse>(false, System.Net.HttpStatusCode.OK, "Id can not be null", responsemodel);
        //     }
        // }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            DeleteOrganizationResponse responsemodel = new DeleteOrganizationResponse();
            if (Id != null)
            {
                var organizationId = Id;
                var documents = await _organizationAttachmentService.DeleteAttachmentByOrganizationId(organizationId);

                // Remove organization documents from folder

                foreach (var OrganizationDoc in documents)
                {

                    //var dirPath = _hostingEnvironment.WebRootPath + "\\OrganizationUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.OrganizationUploadDirPath;
                    var filePath = dirPath + "\\" + OrganizationDoc.FileName;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }
                }

                var notes = await _organizationNoteService.DeleteByOrganization(organizationId);
                var allActivity = _organizationActivityService.GetByOrganization(organizationId);

                foreach (var OrganizationActivityObj in allActivity)
                {
                    var deletedActivityMembers = await _organizationActivityMemberService.DeleteByActivityId(OrganizationActivityObj.Id);
                }

                var deletedAllActivities = await _organizationActivityService.DeleteByOrganization(organizationId);
                var OrganizationObj = await _organizationService.DeleteOrganization(Id);

                if (OrganizationObj != null)
                {
                    CustomModule? customModuleObj = null;
                    var customTable = _customTableService.GetByName("Organization");
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
                            customFieldValueDtoObj.RecordId = OrganizationObj.Id;
                            var deletedRecord = await _customFieldValueService.DeleteList(customFieldValueDtoObj);
                        }
                    }
                    // Start logic for Record wise delete custom field
                    //var moduleObj = _customModuleService.GetByName("Organization");
                    if (customModuleObj != null)
                    {

                        var moduleFields = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                        var moduleFieldIdList = moduleFields.Select(t => t.Id).ToList();

                        var moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIdList);

                        foreach (var moduleRecordField in moduleRecordFieldList)
                        {
                            if (moduleRecordField.RecordId == Id)
                            {
                                var DeletedModuleRecordField = await _moduleRecordCustomFieldService.DeleteById(moduleRecordField.Id);

                                var moduleFieldId = moduleRecordField.ModuleFieldId;
                                long? CustomFieldId1 = null;
                                if (moduleRecordField.ModuleField.CustomField != null)
                                {
                                    CustomFieldId1 = moduleRecordField.ModuleField.CustomField.Id;
                                }

                                if (moduleFieldId != null)
                                {
                                    var DeleteModuleField = _moduleFieldService.Delete(moduleFieldId.Value);
                                }

                                if (CustomFieldId1 != null)
                                {
                                    var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(CustomFieldId1.Value, OrganizationObj.TenantId.Value);
                                }

                                CustomTableColumnDto columnDto = new CustomTableColumnDto();
                                columnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
                                columnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
                                columnDto.IsDefault = false;
                                columnDto.TenantId = OrganizationObj.TenantId;
                                if (CustomFieldId1 != null)
                                {
                                    columnDto.CustomFieldId = CustomFieldId1;
                                }
                                // var tableObj = _customTableService.GetByName("Person");
                                // if (tableObj != null)
                                // {
                                columnDto.MasterTableId = customModuleObj.Id;
                                // }
                                var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(columnDto);

                                if (CustomFieldId1 != null)
                                {
                                    var deleteTableColumns1 = _customFieldService.DeleteById(CustomFieldId1.Value);
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
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide organization id", Id);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<GetOrganizationResponse>> Detail(int Id)
        {
            Organization OrganizationObj = new Organization();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            int tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            OrganizationObj = _organizationService.GetById(Id);
            var organizatioDto = _mapper.Map<OrganizationDto>(OrganizationObj);

            var notes = _organizationNoteService.GetByOrganization(Id);
            var documents = _organizationAttachmentService.GetAllByOrganizationId(Id);

            organizatioDto.Notes = _mapper.Map<List<OrganizationNoteDto>>(notes);
            organizatioDto.Documents = _mapper.Map<List<OrganizationAttachmentDto>>(documents);

            var activityTypes = _organizationActivityTypeService.GetAll();
            var activityAvailabilities = _organizationActivityAvailabilityService.GetAll();

            var allActivity = _organizationActivityService.GetByOrganization(Id);

            var plannedActivities = allActivity.Where(t => t.IsDone == false).ToList();
            var completedActivities = allActivity.Where(t => t.IsDone == true).ToList();

            organizatioDto.PlannedActivities = _mapper.Map<List<OrganizationActivityDto>>(plannedActivities);
            organizatioDto.CompletedActivities = _mapper.Map<List<OrganizationActivityDto>>(completedActivities);

            var users = _userService.GetAll();

            foreach (var item in organizatioDto.Notes)
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

                    var comments = _organizationNotesCommentService.GetAllByNoteId(item.Id.Value);
                    var commentsDto = _mapper.Map<List<OrganizationNotesCommentDto>>(comments);
                    foreach (var CommentObj in commentsDto)
                    {
                        var userObj1 = users.Where(t => t.Id == CommentObj.CreatedBy).FirstOrDefault();
                        if (userObj1 != null)
                        {
                            CommentObj.FirstName = userObj1.FirstName;
                            CommentObj.LastName = userObj1.LastName;
                            CommentObj.Email = userObj1.Email;
                            if (CommentObj.FirstName != null)
                            {
                                CommentObj.ShortName = CommentObj.FirstName.Substring(0, 1);
                            }
                            if (item.LastName != null)
                            {
                                CommentObj.ShortName = CommentObj.ShortName + CommentObj.LastName.Substring(0, 1);
                            }
                        }
                    }
                    item.Comments = commentsDto;
                }
            }

            foreach (var item in organizatioDto.PlannedActivities)
            {
                var members = _organizationActivityMemberService.GetAllByActivity(item.Id.Value);
                if (item.OrganizationActivityTypeId != null)
                {
                    var activityTypeObj = activityTypes.Where(t => t.Id == item.OrganizationActivityTypeId).FirstOrDefault();
                    if (activityTypeObj != null)
                    {
                        item.OrganizationActivityType = activityTypeObj.Name;
                    }
                }

                if (item.OrganizationActivityAvailabilityId != null)
                {
                    var activityAvailabilityObj = activityAvailabilities.Where(t => t.Id == item.OrganizationActivityAvailabilityId).FirstOrDefault();
                    if (activityAvailabilityObj != null)
                    {
                        item.OrganizationActivityAvailability = activityAvailabilityObj.Name;
                    }
                }

                item.Members = _mapper.Map<List<OrganizationActivityMemberDto>>(members);

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

            foreach (var completeActivityObj in organizatioDto.CompletedActivities)
            {

                var members = _organizationActivityMemberService.GetAllByActivity(completeActivityObj.Id.Value);

                if (completeActivityObj.OrganizationActivityTypeId != null)
                {
                    var activityTypeObj = activityTypes.Where(t => t.Id == completeActivityObj.OrganizationActivityTypeId).FirstOrDefault();
                    if (activityTypeObj != null)
                    {
                        completeActivityObj.OrganizationActivityType = activityTypeObj.Name;
                    }
                }

                if (completeActivityObj.OrganizationActivityAvailabilityId != null)
                {
                    var activityAvailabilityObj = activityAvailabilities.Where(t => t.Id == completeActivityObj.OrganizationActivityAvailabilityId).FirstOrDefault();
                    if (activityAvailabilityObj != null)
                    {
                        completeActivityObj.OrganizationActivityAvailability = activityAvailabilityObj.Name;
                    }
                }

                completeActivityObj.Members = _mapper.Map<List<OrganizationActivityMemberDto>>(members);

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

            var organizationModule = _customModuleService.GetByName("Organization");
            if (organizationModule != null)
            {
                CustomModuleDto Model = new CustomModuleDto();
                Model.TenantId = tenantId;
                Model.MasterTableId = organizationModule.MasterTableId;
                Model.Id = organizationModule.Id;
                Model.RecordId = Id;
                // organizatioDto.CustomFields = await customFieldLogic.GetCustomField(Model);

                var customFields = await customFieldLogic.GetCustomField(Model);

                List<ModuleRecordCustomField> moduleRecordFieldList = new List<ModuleRecordCustomField>();

                var moduleFieldList = _moduleFieldService.GetAllModuleField(organizationModule.Id);

                var moduleFieldIds = moduleFieldList.Select(x => x.Id).ToList();

                moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIds);


                if (moduleRecordFieldList.Count() > 0)
                {
                    foreach (var moduleRecordCustomField in moduleRecordFieldList)
                    {
                        var isExistData = customFields.Where(t => t.Id == moduleRecordCustomField.ModuleField.FieldId).FirstOrDefault();
                        if (isExistData != null && moduleRecordCustomField.RecordId != Id)
                        {
                            customFields.Remove(isExistData);
                        }
                    }
                }

                organizatioDto.CustomFields = customFields;
            }
            var responseDtos = _mapper.Map<GetOrganizationResponse>(organizatioDto);
            return new OperationResult<GetOrganizationResponse>(true, System.Net.HttpStatusCode.OK, "", responseDtos);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<SyncWeClappToBemateResponse>> SyncWeClappToBemate([FromBody] SyncWeClappToBemateResquest syncContactModel)
        {
            var dataList = new List<KeyValuePair<string, string>>();
            var organizationDto = new OrganizationDto();
            var LabelCategoryObj = _labelCategoryService.GetByName("Organization");
            var organizationModule = _customModuleService.GetByName("Organization");

            var allTableColumns = _customTableColumnService.GetAll();

            var requestmodel = _mapper.Map<SyncContactDto>(syncContactModel);
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            int tenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            List<CustomTableColumnDto> customFields = new List<CustomTableColumnDto>();

            foreach (var model in requestmodel.ContactPropertyList)
            {
                OrganizationDto organizationModel = new OrganizationDto();
                foreach (var item in model.ContactProperties)
                {
                    var FieldName = item.Key;
                    switch (FieldName)
                    {
                        case "Name":
                            organizationModel.Name = item.Value;
                            break;
                        case "Address":
                            organizationModel.Address = item.Value;
                            break;
                        case "WeClappOrganizationId":
                            organizationModel.WeClappOrganizationId = Convert.ToInt64(item.Value);
                            break;
                            // default:
                    }
                }

                organizationModel.CreatedBy = UserId;
                organizationModel.UserId = UserId;
                organizationModel.TenantId = tenantId;
                var organizationObj = await _organizationService.CheckInsertOrUpdate(organizationModel);
                organizationDto = _mapper.Map<OrganizationDto>(organizationObj);

                CustomModuleDto Model = new CustomModuleDto();
                List<CustomerDto> customerDtos = new List<CustomerDto>();
                Model.TenantId = tenantId;
                Model.MasterTableId = organizationModule.MasterTableId;
                Model.Id = organizationModule.Id;
                Model.RecordId = organizationDto.Id;
                organizationDto.CustomFields = await customFieldLogic.GetCustomField(Model);

                foreach (var item in model.ContactProperties)
                {
                    if (item.Key != "Name" && item.Key != "Address" && item.Key != "WeClappOrganizationId")
                    {
                        if (item.Key == "Label")
                        {
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                LabelDto labelDto1 = new LabelDto();
                                labelDto1.Name = item.Value;
                                if (LabelCategoryObj != null)
                                {
                                    labelDto1.LabelCategoryId = LabelCategoryObj.Id;
                                }
                                labelDto1.Color = "gray";
                                labelDto1.TenantId = tenantId;
                                labelDto1.CreatedBy = UserId;
                                var AddUpdateLabel = await _labelService.CheckInsertOrUpdate(labelDto1);

                                OrganizationLabelDto organizationLabelDto = new OrganizationLabelDto();
                                organizationLabelDto.LabelId = AddUpdateLabel.Id;
                                organizationLabelDto.OrganizationId = organizationObj.Id;
                                organizationLabelDto.TenantId = tenantId;
                                organizationLabelDto.CreatedBy = UserId;
                                var AddUpdateOrganizationLabel = await _organizationLabelService.CheckInsertOrUpdate(organizationLabelDto);

                            }
                        }

                        // Add logic for custom field

                        if (organizationModule != null)
                        {
                            if (organizationDto.CustomFields.Count() > 0)
                            {
                                foreach (var item1 in organizationDto.CustomFields)
                                {
                                    if (item.Key == item1.Name)
                                    {
                                        if (!string.IsNullOrEmpty(item.Value))
                                        {
                                            CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                                            customFieldValueDto.FieldId = item1.Id;
                                            customFieldValueDto.ModuleId = organizationModule.Id;
                                            customFieldValueDto.RecordId = organizationDto.Id;
                                            var controlType = "";
                                            if (item1.CustomControl != null)
                                            {
                                                controlType = item1.CustomControl.Name;
                                                customFieldValueDto.ControlType = controlType;
                                            }
                                            customFieldValueDto.Value = item.Value;
                                            customFieldValueDto.CreatedBy = UserId;
                                            customFieldValueDto.TenantId = tenantId;
                                            if (item1.CustomControlOptions.Count() > 0)
                                            {

                                                var selectedOptionList = item1.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
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
                                                if (!string.IsNullOrEmpty(customFieldValueDto.Value))
                                                {
                                                    var AddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var responsemodel = _mapper.Map<SyncWeClappToBemateResponse>(organizationDto);
            return new OperationResult<SyncWeClappToBemateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }


        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<SyncbeMateToWeClappResponse>> SyncbeMateToWeClapp()
        {

            List<OrganizationDto> organizationDtos = new List<OrganizationDto>();
            OrganizationDto organizationDto = new OrganizationDto();
            SyncbeMateToWeClappResponse responseDtos = new SyncbeMateToWeClappResponse();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);


            var organizationModule = _customModuleService.GetByName("Organization");
            var ERPColumnMapData = _eRPSystemColumnMapService.GetByUserAndModule(UserId, organizationModule.Id);
            var weClappUserObj = _weClappUserService.GetByUser(UserId);
            if (ERPColumnMapData.Count() > 0 && weClappUserObj != null)
            {
                List<CustomTableColumnDto> OrganizationColumns = new List<CustomTableColumnDto>();
                if (organizationModule != null)
                {
                    var customerTableColumnList = _customTableColumnService.GetAllByTable(organizationModule.MasterTableId.Value);
                    OrganizationColumns = _mapper.Map<List<CustomTableColumnDto>>(customerTableColumnList);
                }

                if (organizationModule != null)
                {
                    CustomModuleDto Model = new CustomModuleDto();
                    Model.TenantId = TenantId;
                    Model.MasterTableId = organizationModule.MasterTableId;
                    Model.Id = organizationModule.Id;
                    // var customerList = _customerService.GetAllByTenant(TenantId);
                    var organizationList = _organizationService.GetByUser(UserId);

                    organizationList = organizationList.Where(t => t.WeClappOrganizationId == null && t.Name != null).ToList();
                    organizationDtos = _mapper.Map<List<OrganizationDto>>(organizationList);

                    foreach (var organizationDtoItem in organizationDtos)
                    {
                        dynamic MyDynamic = new ExpandoObject();
                        MyDynamic.partyType = "ORGANIZATION";

                        foreach (var item in ERPColumnMapData)
                        {
                            var organizationColumnObj = OrganizationColumns.Where(t => t.Name == item.DestinationColumnName).FirstOrDefault();
                            if (organizationColumnObj.CustomFieldId != null)
                            {
                                var CustomFieldValueList = _customFieldValueService.GetAllValues(organizationColumnObj.CustomFieldId.Value, organizationDtoItem.TenantId.Value, organizationModule.Id, organizationDtoItem.Id.Value);
                                if (CustomFieldValueList.Count() > 0)
                                {
                                    var fieldValue = "";
                                    foreach (var CustomFieldValueItem in CustomFieldValueList)
                                    {
                                        if (CustomFieldValueList.Count() == 1)
                                        {
                                            fieldValue = CustomFieldValueItem.Value;
                                        }
                                        else
                                        {
                                            if (CustomFieldValueItem.OptionId != null)
                                            {
                                                var customControlOptionObj = _customControlOptionService.GetById(CustomFieldValueItem.OptionId.Value);
                                                if (customControlOptionObj != null)
                                                {
                                                    if (fieldValue == "")
                                                    {
                                                        fieldValue = customControlOptionObj.Option;
                                                    }
                                                    else
                                                    {
                                                        fieldValue = fieldValue + ", " + customControlOptionObj.Option;
                                                    }
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(fieldValue))
                                        {
                                            ((IDictionary<String, Object>)MyDynamic)[item.SourceColumnName] = fieldValue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ((IDictionary<String, Object>)MyDynamic)[item.SourceColumnName] = organizationDtoItem.GetType().GetProperty(item.DestinationColumnName).GetValue(organizationDtoItem, null);
                            }

                        }

                        var isExist = Common.HasProperty(MyDynamic, "company");

                        if (isExist == true)
                        {
                            var companyValue = MyDynamic.company;

                            if (string.IsNullOrEmpty(companyValue))
                            {
                                MyDynamic.company = organizationDtoItem.Name;
                            }
                        }
                        else
                        {
                            MyDynamic.company = organizationDtoItem.Name;
                        }

                        var data = await _weClappService.SaveCustomer(weClappUserObj.ApiKey, weClappUserObj.TenantName, MyDynamic);
                        if (!string.IsNullOrEmpty(data.Error))
                        {
                            var ErrorMessage = data.Error;
                            if (data.Error == "unauthorized")
                            {
                                ErrorMessage = "Please provide valid weclapp credential";
                            }
                            responseDtos = _mapper.Map<SyncbeMateToWeClappResponse>(organizationDtoItem);
                            return new OperationResult<SyncbeMateToWeClappResponse>(false, System.Net.HttpStatusCode.OK, ErrorMessage, responseDtos);
                        }
                        else
                        {
                            organizationDtoItem.WeClappOrganizationId = data.Id;
                            var AddUpdate = await _organizationService.CheckInsertOrUpdate(organizationDtoItem);
                        }
                    }



                    #region One Customer SyncBemate To WeClapp For Testing
                    // Start logic for one customer for testing
                    // customerDto = customerDtos.Where(t => t.LastName != null && t.FirstName != null).FirstOrDefault();
                    // dynamic MyDynamic = new ExpandoObject();
                    // MyDynamic.partyType = "PERSON";
                    // if (ERPColumnMapData.Count() > 0)
                    // {
                    //     foreach (var item in ERPColumnMapData)
                    //     {
                    //         ((IDictionary<String, Object>)MyDynamic)[item.SourceColumnName] = customerDto.GetType().GetProperty(item.DestinationColumnName).GetValue(customerDto, null);
                    //     }
                    // }

                    // var LatNameProperty = MyDynamic.GetType().GetProperty("lastName");
                    // if (LatNameProperty == null)
                    // {
                    //     MyDynamic.lastName = customerDto.LastName;
                    // }

                    // var data = await _weClappService.SaveCustomer("7a970695-5f65-4056-ab0e-9c6fd40ad7e6", "testit", MyDynamic);
                    // if (!string.IsNullOrEmpty(data.Error))
                    // {
                    //     return new OperationResult<CustomerDto>(false, data.Error, customerDto);
                    // }
                    // else
                    // {
                    //     customerDto.WeClappCustomerId = data.Id;
                    //     var AddUpdate = _customerService.CheckInsertOrUpdate(customerDto);
                    // }

                    #endregion
                }
                responseDtos = _mapper.Map<SyncbeMateToWeClappResponse>(organizationDto);

                return new OperationResult<SyncbeMateToWeClappResponse>(true, System.Net.HttpStatusCode.OK, "", responseDtos);
            }
            else
            {
                responseDtos = _mapper.Map<SyncbeMateToWeClappResponse>(organizationDto);

                return new OperationResult<SyncbeMateToWeClappResponse>(false, System.Net.HttpStatusCode.OK, "Please add column mapping", responseDtos);
            }
        }
    }
}