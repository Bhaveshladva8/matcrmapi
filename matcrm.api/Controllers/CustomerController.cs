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
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CustomerController : Controller
    {
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
        private readonly ICustomerNoteService _customerNoteService;
        private readonly ICustomerAttachmentService _customerAttachmentService;
        private readonly IEmailPhoneNoTypeService _emailPhoneNoTypeService;
        private readonly ICustomerEmailService _customerEmailService;
        private readonly ICustomerPhoneService _customerPhoneService;
        private readonly ICustomerNotesCommentService _customerNotesCommentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IUserService _userSerVice;
        private readonly ICustomerLabelService _customerLabelService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IActivityTypeService _customerActivityTypeService;
        private readonly IActivityAvailabilityService _customerActivityAvailabilityService;
        private readonly ICustomerActivityMemberService _customerActivityMemberService;
        private readonly IModuleRecordCustomFieldService _moduleRecordCustomFieldService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly ILabelCategoryService _labelCategoryService;
        private readonly ILabelService _labelService;
        private readonly IOrganizationService _organizationService;
        private readonly IERPSystemColumnMapService _eRPSystemColumnMapService;
        private readonly IWeClappService _weClappService;
        private readonly ISalutationService _salutationService;
        private readonly IWeClappUserService _weClappUserService;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private CustomFieldLogic customFieldLogic;
        private int UserId = 0;
        private int TenantId = 0;

        public CustomerController(
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
            ICustomerNoteService customerNoteService,
            ICustomerAttachmentService customerAttachmentService,
            IEmailPhoneNoTypeService emailPhoneNoTypeService,
            ICustomerEmailService customerEmailService,
            ICustomerPhoneService customerPhoneService,
            ICustomerNotesCommentService customerNotesCommentService,
            IUserService userSerVice,
            ICustomerLabelService customerLabelService,
            ICustomerActivityService customerActivityService,
            IActivityTypeService customerActivityTypeService,
            IActivityAvailabilityService customerActivityAvailabilityService,
            ICustomerActivityMemberService customerActivityMemberService,
            IModuleRecordCustomFieldService moduleRecordCustomFieldService,
            ICustomTableColumnService customTableColumnService,
            ILabelCategoryService labelCategoryService,
            ILabelService labelService,
            IOrganizationService organizationService,
            IHostingEnvironment hostingEnvironment,
            IERPSystemColumnMapService eRPSystemColumnMapService,
            ISalutationService salutationService,
            IWeClappService weClappService,
            IWeClappUserService weClappUserService,
            OneClappContext context,
            IMapper mapper
        )
        {
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
            _customerNoteService = customerNoteService;
            _customerAttachmentService = customerAttachmentService;
            _emailPhoneNoTypeService = emailPhoneNoTypeService;
            _customerEmailService = customerEmailService;
            _customerPhoneService = customerPhoneService;
            _customerNotesCommentService = customerNotesCommentService;
            _userSerVice = userSerVice;
            _customerLabelService = customerLabelService;
            _customerActivityService = customerActivityService;
            _customerActivityTypeService = customerActivityTypeService;
            _customerActivityAvailabilityService = customerActivityAvailabilityService;
            _customerActivityMemberService = customerActivityMemberService;
            _hostingEnvironment = hostingEnvironment;
            _moduleRecordCustomFieldService = moduleRecordCustomFieldService;
            _customTableColumnService = customTableColumnService;
            _labelCategoryService = labelCategoryService;
            _labelService = labelService;
            _organizationService = organizationService;
            _eRPSystemColumnMapService = eRPSystemColumnMapService;
            _weClappService = weClappService;
            _weClappUserService = weClappUserService;
            _salutationService = salutationService;
            _context = context;
            _mapper = mapper;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
                customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }
        //get all customer
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CustomerNewGetAllResponse>>> List()
        {

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            List<CustomerDto> customerDtoList = new List<CustomerDto>();
            //var customerModule = _customModuleService.GetByName("Person");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("Person");
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
                var customerList = _customerService.GetAllByTenant(TenantId);
                customerDtoList = _mapper.Map<List<CustomerDto>>(customerList);
                if (customerDtoList != null && customerDtoList.Count() > 0)
                {
                    foreach (var item in customerDtoList)
                    {
                        customModuleDto.RecordId = item.Id;
                        item.CustomFields = await customFieldLogic.GetCustomField(customModuleDto);
                        if (item.Id != null)
                        {
                            var customerLabel = _customerLabelService.GetByCustomer(item.Id.Value);
                            if (customerLabel != null)
                            {
                                item.LabelId = customerLabel.LabelId;
                            }
                        }
                        if (item.Id != null)
                        {
                            var notes = _customerNoteService.GetByCustomer(item.Id.Value);
                            var documents = _customerAttachmentService.GetAllByCustomerId(item.Id.Value);

                            var phones = _customerPhoneService.GetByCustomer(item.Id.Value);
                            var emails = _customerEmailService.GetByCustomer(item.Id.Value);

                            item.Notes = _mapper.Map<List<CustomerNoteDto>>(notes);
                            item.Documents = _mapper.Map<List<CustomerAttachmentDto>>(documents);

                            item.Phones = _mapper.Map<List<CustomerPhoneDto>>(phones);
                            item.Emails = _mapper.Map<List<CustomerEmailDto>>(emails);
                        }
                    }
                }
            }
            var responseCustomerDtoList = _mapper.Map<List<CustomerNewGetAllResponse>>(customerDtoList);
            return new OperationResult<List<CustomerNewGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseCustomerDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomerAddUpdateResponse>> AddUpdate([FromBody] CustomerAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<CustomerDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var customerObj = await _customerService.CheckInsertOrUpdate(requestmodel);
            var customerDto = _mapper.Map<CustomerDto>(customerObj);
            CustomerLabelDto customerLabelDto = new CustomerLabelDto();
            if (requestmodel.LabelId != null)
            {
                customerLabelDto.LabelId = requestmodel.LabelId;
                customerLabelDto.CreatedBy = UserId;
                customerLabelDto.CustomerId = customerObj.Id;
                customerLabelDto.TenantId = TenantId;
                var labelObj = await _customerLabelService.CheckInsertOrUpdate(customerLabelDto);
                customerLabelDto.Id = labelObj.Id;
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
            //var customerModule = _customModuleService.GetByName("Person");
            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("Person");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            if (requestmodel.CustomFields != null && requestmodel.CustomFields.Count() > 0 && customModuleObj != null)
            {
                foreach (var item in requestmodel.CustomFields)
                {
                    if (item != null)
                    {
                        CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                        customFieldValueDto.FieldId = item.Id;
                        customFieldValueDto.ModuleId = customModuleObj.Id;
                        customFieldValueDto.RecordId = customerObj.Id;
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
                            if (selectedOptionList != null)
                            {
                                foreach (var option in selectedOptionList)
                                {
                                    customFieldValueDto.OptionId = option.Id;
                                    var AddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                                }
                            }
                        }
                        else
                        {
                            var AddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                        }
                    }

                }
            }

            var emails = _customerEmailService.GetByCustomer(customerObj.Id);
            var phones = _customerPhoneService.GetByCustomer(customerObj.Id);

            customerDto.Emails = _mapper.Map<List<CustomerEmailDto>>(emails);
            customerDto.Phones = _mapper.Map<List<CustomerPhoneDto>>(phones);
            var responseCustomerDto = _mapper.Map<CustomerAddUpdateResponse>(customerDto);
            return new OperationResult<CustomerAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseCustomerDto);
        }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]        
        // [HttpDelete]
        // public async Task<OperationResult<DeleteCustomerResponse>> Remove([FromBody] DeleteCustomerRequest model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

        //     var requestmodel = _mapper.Map<CustomerDto>(model);
        //     DeleteCustomerResponse responsemodel = new DeleteCustomerResponse();

        //     if (requestmodel.Id != null)
        //     {
        //         var customerId = requestmodel.Id.Value;

        //         var documents = _customerAttachmentService.DeleteAttachmentByCustomerId(customerId);

        //         // Remove customer documents from folder
        //         if (documents != null && documents.Count() > 0)
        //         {
        //             foreach (var CustomerDoc in documents)
        //             {

        //                 var dirPath = _hostingEnvironment.WebRootPath + "\\CustomerUpload";
        //                 var filePath = dirPath + "\\" + CustomerDoc.FileName;

        //                 if (System.IO.File.Exists(filePath))
        //                 {
        //                     System.IO.File.Delete(Path.Combine(filePath));
        //                 }
        //             }
        //         }

        //         var notes = await _customerNoteService.DeleteByCustomer(customerId);

        //         var phones = await _customerPhoneService.DeleteByCustomer(customerId);

        //         var email = _customerEmailService.DeleteByCustomer(customerId);

        //         //var customerObj = _customerService.DeleteCustomer(requestmodel);
        //         var customerObj = await _customerService.DeleteCustomer(requestmodel.Id.Value);
        //         if (customerObj != null)
        //         {
        //             // model = _mapper.Map<CustomTicketDto> (ticketObj);
        //             //var customerModule = _customModuleService.GetByName("Person");

        //             CustomModule? customModuleObj = null;
        //             var customTable = _customTableService.GetByName("Person");
        //             if (customTable != null)
        //             {
        //                 customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
        //             }
        //             List<CustomField> customFieldList = new List<CustomField>();
        //             if (customModuleObj != null)
        //             {
        //                 var moduleFieldList = _moduleFieldService.GetAllModuleField(customModuleObj.Id);


        //                 foreach (var item in moduleFieldList)
        //                 {
        //                     if (item.CustomField != null)
        //                     {
        //                         customFieldList.Add(item.CustomField);
        //                     }
        //                 }
        //             }

        //             if (customFieldList != null && customFieldList.Count() > 0 && customModuleObj != null)
        //             {
        //                 foreach (var item in customFieldList)
        //                 {
        //                     CustomFieldValueDto customFieldValueDtoObj = new CustomFieldValueDto();
        //                     customFieldValueDtoObj.FieldId = item.Id;
        //                     customFieldValueDtoObj.ModuleId = customModuleObj.Id;
        //                     customFieldValueDtoObj.RecordId = customerObj.Id;
        //                     var deletedRecord = await _customFieldValueService.DeleteList(customFieldValueDtoObj);
        //                 }
        //             }
        //             // if (requestmodel.CustomFields != null && requestmodel.CustomFields.Count() > 0 && customModuleObj != null)
        //             // {
        //             //     foreach (var item in requestmodel.CustomFields)
        //             //     {
        //             //         CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
        //             //         customFieldValueDto.FieldId = item.Id;
        //             //         customFieldValueDto.ModuleId = customModuleObj.Id;
        //             //         customFieldValueDto.RecordId = customerObj.Id;
        //             //         var deletedRecord = await _customFieldValueService.DeleteList(customFieldValueDto);
        //             //     }
        //             // }

        //             // Start logic for Record wise delete custom field
        //             if (customModuleObj != null)
        //             {
        //                 var moduleFields = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

        //                 var moduleFieldIdList = moduleFields.Select(t => t.Id).ToList();

        //                 var moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIdList);
        //                 if (moduleRecordFieldList != null && moduleRecordFieldList.Count() > 0)
        //                 {
        //                     foreach (var moduleRecordField in moduleRecordFieldList)
        //                     {
        //                         if (moduleRecordField.RecordId == requestmodel.Id)
        //                         {
        //                             var DeletedModuleRecordField = await _moduleRecordCustomFieldService.DeleteById(moduleRecordField.Id);

        //                             var moduleFieldId = moduleRecordField.ModuleFieldId;
        //                             long? CustomFieldId1 = null;
        //                             if (moduleRecordField.ModuleField.CustomField != null)
        //                             {
        //                                 CustomFieldId1 = moduleRecordField.ModuleField.CustomField.Id;
        //                             }

        //                             if (moduleFieldId != null)
        //                             {
        //                                 var DeleteModuleField = _moduleFieldService.Delete(moduleFieldId.Value);
        //                             }

        //                             if (CustomFieldId1 != null)
        //                             {
        //                                 var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(CustomFieldId1.Value, TenantId);
        //                             }

        //                             CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
        //                             customTableColumnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
        //                             customTableColumnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
        //                             customTableColumnDto.IsDefault = false;
        //                             customTableColumnDto.TenantId = TenantId;
        //                             if (CustomFieldId1 != null)
        //                             {
        //                                 customTableColumnDto.CustomFieldId = CustomFieldId1;
        //                             }
        //                             // var tableObj = _customTableService.GetByName("Person");
        //                             // if (tableObj != null)
        //                             // {
        //                             // columnDto.MasterTableId = tableObj.Id;
        //                             // }
        //                             customTableColumnDto.MasterTableId = customModuleObj.MasterTableId;
        //                             var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);

        //                             if (CustomFieldId1 != null)
        //                             {
        //                                 var deleteTableColumns1 = _customFieldService.DeleteById(CustomFieldId1.Value);
        //                             }

        //                         }
        //                     }
        //                 }
        //             }
        //             // End logic for Record wise delete custom field
        //         }
        //         responsemodel = _mapper.Map<DeleteCustomerResponse>(requestmodel);
        //         return new OperationResult<DeleteCustomerResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        //     }
        //     else
        //     {
        //         responsemodel = _mapper.Map<DeleteCustomerResponse>(requestmodel);
        //         return new OperationResult<DeleteCustomerResponse>(false, System.Net.HttpStatusCode.OK, "Id can not pass null", responsemodel);
        //     }
        // }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (Id != null && Id > 0)
            {
                var customerId = Id;

                var documents = _customerAttachmentService.DeleteAttachmentByCustomerId(customerId);

                // Remove customer documents from folder
                if (documents != null && documents.Count() > 0)
                {
                    foreach (var CustomerDoc in documents)
                    {

                        //var dirPath = _hostingEnvironment.WebRootPath + "\\CustomerUpload";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.CustomerFileUploadDirPath;
                        var filePath = dirPath + "\\" + CustomerDoc.FileName;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                    }
                }

                var notes = await _customerNoteService.DeleteByCustomer(customerId);

                var phones = await _customerPhoneService.DeleteByCustomer(customerId);

                var email = _customerEmailService.DeleteByCustomer(customerId);

                var allActivity = _customerActivityService.GetByCustomer(customerId);

                foreach (var CustomerActivityObj in allActivity)
                {
                    var deletedActivityMembers = await _customerActivityMemberService.DeleteByActivityId(CustomerActivityObj.Id);
                }

                var deletedAllActivities = await _customerActivityService.DeleteByCustomer(customerId);

                //var customerObj = _customerService.DeleteCustomer(requestmodel);
                var customerObj = await _customerService.DeleteCustomer(Id);
                if (customerObj != null)
                {
                    // model = _mapper.Map<CustomTicketDto> (ticketObj);
                    //var customerModule = _customModuleService.GetByName("Person");

                    CustomModule? customModuleObj = null;
                    var customTable = _customTableService.GetByName("Person");
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
                            customFieldValueDtoObj.RecordId = customerObj.Id;
                            var deletedRecord = await _customFieldValueService.DeleteList(customFieldValueDtoObj);
                        }
                    }
                    // if (requestmodel.CustomFields != null && requestmodel.CustomFields.Count() > 0 && customModuleObj != null)
                    // {
                    //     foreach (var item in requestmodel.CustomFields)
                    //     {
                    //         CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                    //         customFieldValueDto.FieldId = item.Id;
                    //         customFieldValueDto.ModuleId = customModuleObj.Id;
                    //         customFieldValueDto.RecordId = customerObj.Id;
                    //         var deletedRecord = await _customFieldValueService.DeleteList(customFieldValueDto);
                    //     }
                    // }

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
                                        var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(CustomFieldId1.Value, TenantId);
                                    }

                                    CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                                    customTableColumnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
                                    customTableColumnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
                                    customTableColumnDto.IsDefault = false;
                                    customTableColumnDto.TenantId = TenantId;
                                    if (CustomFieldId1 != null)
                                    {
                                        customTableColumnDto.CustomFieldId = CustomFieldId1;
                                    }
                                    // var tableObj = _customTableService.GetByName("Person");
                                    // if (tableObj != null)
                                    // {
                                    // columnDto.MasterTableId = tableObj.Id;
                                    // }
                                    customTableColumnDto.MasterTableId = customModuleObj.MasterTableId;
                                    var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);

                                    if (CustomFieldId1 != null)
                                    {
                                        var deleteTableColumns1 = _customFieldService.DeleteById(CustomFieldId1.Value);
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
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide customer id", Id);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CustomerPhoneResponse>> RemovePhone([FromBody] CustomerPhoneRequest model)
        {
            var requestmodel = _mapper.Map<CustomerPhoneDto>(model);
            var RemovedData = _customerPhoneService.DeleteCustomerPhone(requestmodel);
            var responsemodel = _mapper.Map<CustomerPhoneResponse>(requestmodel);
            return new OperationResult<CustomerPhoneResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CustomerDeleteEmailResponse>> RemoveEmail([FromBody] CustomerDeleteEmailRequest model)
        {
            var requestmodel = _mapper.Map<CustomerEmailDto>(model);
            var RemovedData = _customerEmailService.DeleteCustomerEmail(requestmodel);
            var responsemodel = _mapper.Map<CustomerDeleteEmailResponse>(requestmodel);
            return new OperationResult<CustomerDeleteEmailResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        // [Authorize]
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CustomerGetAllEmailPhoneTypeResponse>>> EmailPhoneTypes()
        {
            List<EmailPhoneNoTypeDto> emailPhoneNoTypeDtoList = new List<EmailPhoneNoTypeDto>();
            var emailPhoneNoTypes = _emailPhoneNoTypeService.GetAll();
            emailPhoneNoTypeDtoList = _mapper.Map<List<EmailPhoneNoTypeDto>>(emailPhoneNoTypes);
            var responseEmailPhoneNoTypeDtoList = _mapper.Map<List<CustomerGetAllEmailPhoneTypeResponse>>(emailPhoneNoTypeDtoList);
            return new OperationResult<List<CustomerGetAllEmailPhoneTypeResponse>>(true, System.Net.HttpStatusCode.OK, "", responseEmailPhoneNoTypeDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{CustomerId}")]
        public async Task<OperationResult<CustomerGetDetailResponse>> Detail(long CustomerId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var customerDto = new CustomerDto();
            CustomerGetDetailResponse responseCustomerDto = new CustomerGetDetailResponse();
            var users = _userSerVice.GetAll();
            var customer = _customerService.GetById(CustomerId);
            if (customer != null)
            {
                customerDto = _mapper.Map<CustomerDto>(customer);
                var notes = _customerNoteService.GetByCustomer(CustomerId);
                var files = _customerAttachmentService.GetAllByCustomerId(CustomerId);
                var emails = _customerEmailService.GetByCustomer(CustomerId);
                var phones = _customerPhoneService.GetByCustomer(CustomerId);

                var activityTypes = _customerActivityTypeService.GetAll();
                var activityAvailabilities = _customerActivityAvailabilityService.GetAll();

                var allActivity = _customerActivityService.GetByCustomer(CustomerId);

                var plannedActivities = allActivity.Where(t => t.IsDone == false).ToList();
                var completedActivities = allActivity.Where(t => t.IsDone == true).ToList();

                customerDto.PlannedActivities = _mapper.Map<List<CustomerActivityDto>>(plannedActivities);
                customerDto.CompletedActivities = _mapper.Map<List<CustomerActivityDto>>(completedActivities);

                customerDto.Documents = _mapper.Map<List<CustomerAttachmentDto>>(files);
                customerDto.Notes = _mapper.Map<List<CustomerNoteDto>>(notes);
                customerDto.Emails = _mapper.Map<List<CustomerEmailDto>>(emails);
                customerDto.Phones = _mapper.Map<List<CustomerPhoneDto>>(phones);
                if (customerDto.Notes != null && customerDto.Notes.Count() > 0)
                {
                    foreach (var item in customerDto.Notes)
                    {
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
                                if (item.Id != null)
                                {
                                    var comments = _customerNotesCommentService.GetAllByNoteId(item.Id.Value);
                                    var commentsDto = _mapper.Map<List<CustomerNotesCommentDto>>(comments);
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
                        }
                    }
                }
                if (customerDto.Documents != null && customerDto.Documents.Count() > 0)
                {
                    foreach (var docObj in customerDto.Documents)
                    {
                        if (users != null)
                        {
                            var userObj = users.Where(t => t.Id == docObj.CreatedBy).FirstOrDefault();
                            if (userObj != null)
                            {
                                docObj.FirstName = userObj.FirstName;
                                docObj.LastName = userObj.LastName;
                                docObj.Email = userObj.Email;
                                if (docObj.FirstName != null)
                                {
                                    docObj.ShortName = docObj.FirstName.Substring(0, 1);
                                }
                                if (docObj.LastName != null)
                                {
                                    docObj.ShortName = docObj.ShortName + docObj.LastName.Substring(0, 1);
                                }
                            }
                        }
                    }
                }
                if (customerDto.PlannedActivities != null && customerDto.PlannedActivities.Count() > 0)
                {
                    foreach (var item in customerDto.PlannedActivities)
                    {

                        if (item.CustomerActivityTypeId != null)
                        {
                            if (activityTypes != null)
                            {
                                var activityTypeObj = activityTypes.Where(t => t.Id == item.CustomerActivityTypeId).FirstOrDefault();
                                if (activityTypeObj != null)
                                {
                                    item.CustomerActivityType = activityTypeObj.Name;
                                }
                            }
                        }

                        if (item.CustomerActivityAvailabilityId != null)
                        {
                            if (activityAvailabilities != null)
                            {
                                var activityAvailabilityObj = activityAvailabilities.Where(t => t.Id == item.CustomerActivityAvailabilityId).FirstOrDefault();
                                if (activityAvailabilityObj != null)
                                {
                                    item.CustomerActivityAvailability = activityAvailabilityObj.Name;
                                }
                            }
                        }
                        if (item.Id != null)
                        {
                            var members = _customerActivityMemberService.GetAllByActivity(item.Id.Value);
                            item.Members = _mapper.Map<List<CustomerActivityMemberDto>>(members);
                        }

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
                if (customerDto.CompletedActivities != null && customerDto.CompletedActivities.Count() > 0)
                {
                    foreach (var completeActivityObj in customerDto.CompletedActivities)
                    {

                        var members = _customerActivityMemberService.GetAllByActivity(completeActivityObj.Id.Value);
                        if (completeActivityObj.CustomerActivityTypeId != null)
                        {
                            if (activityTypes != null)
                            {
                                var activityTypeObj = activityTypes.Where(t => t.Id == completeActivityObj.CustomerActivityTypeId).FirstOrDefault();
                                if (activityTypeObj != null)
                                {
                                    completeActivityObj.CustomerActivityType = activityTypeObj.Name;
                                }
                            }
                        }

                        if (completeActivityObj.CustomerActivityAvailabilityId != null)
                        {
                            if (activityAvailabilities != null)
                            {
                                var activityAvailabilityObj = activityAvailabilities.Where(t => t.Id == completeActivityObj.CustomerActivityAvailabilityId).FirstOrDefault();
                                if (activityAvailabilityObj != null)
                                {
                                    completeActivityObj.CustomerActivityAvailability = activityAvailabilityObj.Name;
                                }
                            }
                        }

                        completeActivityObj.Members = _mapper.Map<List<CustomerActivityMemberDto>>(members);
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

                //var customerModule = _customModuleService.GetByName("Person");

                CustomModule? customModuleObj = null;
                var customTable = _customTableService.GetByName("Person");
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
                    customModuleDto.RecordId = CustomerId;
                    // customerDetail.CustomFields = await customFieldLogic.GetCustomField(Model);

                    var customFields = await customFieldLogic.GetCustomField(customModuleDto);

                    List<ModuleRecordCustomField> moduleRecordFieldList = new List<ModuleRecordCustomField>();

                    var moduleFieldList = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                    var moduleFieldIds = moduleFieldList.Select(x => x.Id).ToList();

                    moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIds);


                    if (moduleRecordFieldList != null && moduleRecordFieldList.Count() > 0)
                    {
                        foreach (var moduleRecordCustomField in moduleRecordFieldList)
                        {
                            var isExistData = customFields.Where(t => t.Id == moduleRecordCustomField.ModuleField.FieldId).FirstOrDefault();
                            if (isExistData != null && moduleRecordCustomField.RecordId != CustomerId)
                            {
                                customFields.Remove(isExistData);
                            }
                            else if (isExistData != null && moduleRecordCustomField.RecordId == CustomerId)
                            {
                                isExistData.IsRecordField = true;
                            }
                        }
                    }

                    customerDto.CustomFields = customFields;
                }
                responseCustomerDto = _mapper.Map<CustomerGetDetailResponse>(customerDto);
                return new OperationResult<CustomerGetDetailResponse>(true, System.Net.HttpStatusCode.OK, "", responseCustomerDto);
            }
            else
            {
                responseCustomerDto = _mapper.Map<CustomerGetDetailResponse>(customerDto);
                return new OperationResult<CustomerGetDetailResponse>(true, System.Net.HttpStatusCode.OK, "", responseCustomerDto);
            }

        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<SyncCustomerResponse>> SyncCustomer([FromBody] SyncCustomerRequest syncContactModel)
        {
            var dataList = new List<KeyValuePair<string, string>>();
            var customerDto = new CustomerDto();
            var labelCategoryObj = _labelCategoryService.GetByName("Person");
            //var customerModule = _customModuleService.GetByName("Person");

            CustomModule? customModuleObj = null;
            var customTable = _customTableService.GetByName("Person");
            if (customTable != null)
            {
                customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
            }

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var allTableColumns = _customTableColumnService.GetAll();

            List<CustomTableColumnDto> customFieldList = new List<CustomTableColumnDto>();
            var requestmodel = _mapper.Map<SyncContactDto>(syncContactModel);
            if (requestmodel.ContactPropertyList != null && requestmodel.ContactPropertyList.Count() > 0)
            {
                foreach (var model in requestmodel.ContactPropertyList)
                {
                    CustomerDto customerDto1 = new CustomerDto();
                    foreach (var item in model.ContactProperties)
                    {
                        if (item != null)
                        {
                            if (item.Key == "OrganizationName")
                            {
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    OrganizationDto organizationDto = new OrganizationDto();
                                    organizationDto.Name = item.Value;
                                    organizationDto.TenantId = TenantId;
                                    organizationDto.CreatedBy = UserId;
                                    var AddUpdateOrganization = await _organizationService.CheckInsertOrUpdate(organizationDto);
                                    customerDto1.OrganizationId = AddUpdateOrganization.Id;
                                }
                            }
                            var FieldName = item.Key;
                            switch (FieldName)
                            {
                                case "FirstName":
                                    customerDto1.FirstName = item.Value;
                                    break;
                                case "Name":
                                    customerDto1.Name = item.Value;
                                    break;
                                case "LastName":
                                    customerDto1.LastName = item.Value;
                                    break;
                                case "WeClappCustomerId":
                                    customerDto1.WeClappCustomerId = Convert.ToInt64(item.Value);
                                    break;
                                    // default:
                            }
                        }
                    }

                    customerDto1.CreatedBy = UserId;
                    customerDto1.UserId = UserId;
                    customerDto1.TenantId = TenantId;
                    var customerObj = await _customerService.CheckInsertOrUpdate(customerDto1);
                    customerDto = _mapper.Map<CustomerDto>(customerObj);

                    if (customModuleObj != null)
                    {
                        CustomModuleDto customModuleDto = new CustomModuleDto();
                        List<CustomerDto> customerDtoList = new List<CustomerDto>();
                        customModuleDto.TenantId = TenantId;
                        customModuleDto.MasterTableId = customModuleObj.MasterTableId;
                        customModuleDto.Id = customModuleObj.Id;
                        customModuleDto.RecordId = customerDto.Id;
                        customerDto.CustomFields = await customFieldLogic.GetCustomField(customModuleDto);
                    }

                    foreach (var item in model.ContactProperties)
                    {
                        if (item != null)
                        {
                            if (item.Key != "OrganizationName" && item.Key != "FirstName" && item.Key != "Name" && item.Key != "WeClappCustomerId")
                            {
                                if (item.Key == "Label")
                                {
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        LabelDto labelDto1 = new LabelDto();
                                        labelDto1.Name = item.Value;
                                        if (labelCategoryObj != null)
                                        {
                                            labelDto1.LabelCategoryId = labelCategoryObj.Id;
                                        }
                                        labelDto1.Color = "gray";
                                        labelDto1.TenantId = TenantId;
                                        labelDto1.CreatedBy = UserId;
                                        var AddUpdateLabel = await _labelService.CheckInsertOrUpdate(labelDto1);

                                        CustomerLabelDto customerLabelDto = new CustomerLabelDto();
                                        customerLabelDto.LabelId = AddUpdateLabel.Id;
                                        customerLabelDto.CustomerId = customerObj.Id;
                                        customerLabelDto.TenantId = TenantId;
                                        customerLabelDto.CreatedBy = UserId;
                                        var AddUpdateCustomerLabel = await _customerLabelService.CheckInsertOrUpdate(customerLabelDto);

                                    }
                                }
                                else if (item.Key == "Email")
                                {
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        CustomerEmailDto customerEmailDto = new CustomerEmailDto();
                                        customerEmailDto.Email = item.Value;
                                        customerEmailDto.CustomerId = customerObj.Id;
                                        customerEmailDto.TenantId = TenantId;
                                        customerEmailDto.CreatedBy = UserId;
                                        var AddUpdateEmail = _customerEmailService.CheckInsertOrUpdate(customerEmailDto);
                                    }
                                }
                                else if (item.Key == "Phone")
                                {
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        CustomerPhoneDto customerPhoneDto = new CustomerPhoneDto();
                                        customerPhoneDto.PhoneNo = item.Value;
                                        customerPhoneDto.CustomerId = customerObj.Id;
                                        customerPhoneDto.TenantId = TenantId;
                                        customerPhoneDto.CreatedBy = UserId;
                                        var AddUpdateEmail = await _customerPhoneService.CheckInsertOrUpdate(customerPhoneDto);
                                    }
                                }

                                // Add logic for custom field

                                if (customModuleObj != null)
                                {
                                    if (customerDto.CustomFields != null && customerDto.CustomFields.Count() > 0)
                                    {
                                        foreach (var item1 in customerDto.CustomFields)
                                        {
                                            if (item.Key == item1.Name)
                                            {
                                                if (!string.IsNullOrEmpty(item.Value))
                                                {
                                                    CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                                                    customFieldValueDto.FieldId = item1.Id;
                                                    customFieldValueDto.ModuleId = customModuleObj.Id;
                                                    customFieldValueDto.RecordId = customerDto.Id;
                                                    var controlType = "";
                                                    if (item1.CustomControl != null)
                                                    {
                                                        controlType = item1.CustomControl.Name;
                                                        customFieldValueDto.ControlType = controlType;
                                                    }
                                                    customFieldValueDto.Value = item.Value;
                                                    customFieldValueDto.CreatedBy = UserId;
                                                    customFieldValueDto.TenantId = TenantId;
                                                    if (item1.CustomControlOptions != null && item1.CustomControlOptions.Count() > 0)
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
                }
            }


            // foreach (var model in syncContactModel.CustomerList)
            // {
            //     if (!string.IsNullOrEmpty(model.OrganizationName))
            //     {
            //         OrganizationDto organizationDto = new OrganizationDto();
            //         organizationDto.Name = model.OrganizationName;
            //         organizationDto.TenantId = model.TenantId;
            //         organizationDto.CreatedBy = model.UserId;
            //         var AddUpdateOrganization = await _organizationService.CheckInsertOrUpdate(organizationDto);
            //         model.OrganizationId = AddUpdateOrganization.Id;
            //     }
            //     model.CreatedBy = model.UserId;
            //     var customerObj = await _customerService.CheckInsertOrUpdate(model);
            //     customerDto = _mapper.Map<CustomerDto>(customerObj);
            //     if (!string.IsNullOrEmpty(model.Label))
            //     {
            //         LabelDto labelDto1 = new LabelDto();
            //         labelDto1.Name = model.Label;
            //         if (LabelCategoryObj != null)
            //         {
            //             labelDto1.LabelCategoryId = LabelCategoryObj.Id;
            //         }
            //         labelDto1.Color = "gray";
            //         labelDto1.TenantId = model.TenantId;
            //         labelDto1.CreatedBy = model.UserId;
            //         var AddUpdateLabel = await _labelService.CheckInsertOrUpdate(labelDto1);

            //         CustomerLabelDto customerLabelDto = new CustomerLabelDto();
            //         customerLabelDto.LabelId = AddUpdateLabel.Id;
            //         customerLabelDto.CustomerId = customerObj.Id;
            //         customerLabelDto.TenantId = model.TenantId;
            //         customerLabelDto.CreatedBy = model.UserId;
            //         var AddUpdateCustomerLabel = await _customerLabelService.CheckInsertOrUpdate(customerLabelDto);

            //     }

            //     if (!string.IsNullOrEmpty(model.Email))
            //     {
            //         CustomerEmailDto emailDto = new CustomerEmailDto();
            //         emailDto.Email = model.Email;
            //         emailDto.CustomerId = customerObj.Id;
            //         emailDto.TenantId = model.TenantId;
            //         emailDto.CreatedBy = model.UserId;
            //         var AddUpdateEmail = _customerEmailService.CheckInsertOrUpdate(emailDto);
            //     }

            //     if (!string.IsNullOrEmpty(model.Phone))
            //     {
            //         CustomerPhoneDto phoneDto = new CustomerPhoneDto();
            //         phoneDto.PhoneNo = model.Phone;
            //         phoneDto.CustomerId = customerObj.Id;
            //         phoneDto.TenantId = model.TenantId;
            //         phoneDto.CreatedBy = model.UserId;
            //         var AddUpdateEmail = await _customerPhoneService.CheckInsertOrUpdate(phoneDto);
            //     }

            //     var emails = _customerEmailService.GetByCustomer(customerObj.Id);
            //     var phones = _customerPhoneService.GetByCustomer(customerObj.Id);

            //     customerDto.Emails = _mapper.Map<List<CustomerEmailDto>>(emails);
            //     model.Phones = _mapper.Map<List<CustomerPhoneDto>>(phones);
            // }
            var responseCustomerDto = _mapper.Map<SyncCustomerResponse>(customerDto);
            return new OperationResult<SyncCustomerResponse>(true, System.Net.HttpStatusCode.OK, "", responseCustomerDto);
        }


        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<CustomerSyncToWeClappResponse>> SyncToWeClapp()
        {
            List<CustomerDto> customerDtoList = new List<CustomerDto>();
            CustomerDto customerDto = new CustomerDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var eRPColumnMapData = _eRPSystemColumnMapService.GetByUser(UserId);
            var weClappUserObj = _weClappUserService.GetByUser(UserId);
            CustomerSyncToWeClappResponse responseDto = new CustomerSyncToWeClappResponse();
            if (eRPColumnMapData != null && eRPColumnMapData.Count() > 0 && weClappUserObj != null)
            {
                //var customerModule = _customModuleService.GetByName("Person");

                CustomModule? customModuleObj = null;
                var customTable = _customTableService.GetByName("Person");
                if (customTable != null)
                {
                    customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                }
                List<CustomTableColumnDto> customTableColumnDtoList = new List<CustomTableColumnDto>();
                if (customModuleObj != null)
                {
                    if (customModuleObj.MasterTableId != null)
                    {
                        var customerTableColumnList = _customTableColumnService.GetAllByTable(customModuleObj.MasterTableId.Value);
                        customTableColumnDtoList = _mapper.Map<List<CustomTableColumnDto>>(customerTableColumnList);
                    }
                }

                if (customModuleObj != null)
                {
                    CustomModuleDto customModuleDto = new CustomModuleDto();
                    customModuleDto.TenantId = TenantId;
                    customModuleDto.MasterTableId = customModuleObj.MasterTableId;
                    customModuleDto.Id = customModuleObj.Id;
                    // var customerList = _customerService.GetAllByTenant(TenantId);
                    var customerList = _customerService.GetByUser(UserId);

                    customerList = customerList.Where(t => t.WeClappCustomerId == null && t.LastName != null).ToList();
                    customerDtoList = _mapper.Map<List<CustomerDto>>(customerList);
                    if (customerDtoList != null && customerDtoList.Count() > 0)
                    {
                        foreach (var customerDtoItem in customerDtoList)
                        {
                            dynamic MyDynamic = new ExpandoObject();
                            MyDynamic.partyType = "PERSON";


                            if (eRPColumnMapData != null && eRPColumnMapData.Count() > 0)
                            {
                                foreach (var item in eRPColumnMapData)
                                {
                                    if (customTableColumnDtoList != null && customTableColumnDtoList.Count() > 0)
                                    {
                                        var customerColumnObj = customTableColumnDtoList.Where(t => t.Name == item.DestinationColumnName).FirstOrDefault();
                                        if (customerColumnObj != null && customerColumnObj.CustomFieldId != null && customerDtoItem.TenantId != null && customerDtoItem.Id != null)
                                        {
                                            var CustomFieldValueList = _customFieldValueService.GetAllValues(customerColumnObj.CustomFieldId.Value, customerDtoItem.TenantId.Value, customModuleObj.Id, customerDtoItem.Id.Value);
                                            if (CustomFieldValueList != null && CustomFieldValueList.Count() > 0)
                                            {
                                                var fieldValue = "";
                                                foreach (var CustomFieldValueItem in CustomFieldValueList)
                                                {
                                                    if (CustomFieldValueList.Count() == 1)
                                                    {
                                                        if (CustomFieldValueItem != null)
                                                        {
                                                            fieldValue = CustomFieldValueItem.Value;
                                                        }
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
                                            ((IDictionary<String, Object>)MyDynamic)[item.SourceColumnName] = customerDtoItem.GetType().GetProperty(item.DestinationColumnName).GetValue(customerDtoItem, null);
                                        }
                                    }

                                }
                            }

                            var isExist = Common.HasProperty(MyDynamic, "lastName");
                            // var LatNameProperty = MyDynamic.GetType().GetProperty("lastName");
                            // if (LatNameProperty == null)
                            // {
                            //     MyDynamic.lastName = customerDtoItem.LastName;
                            // }
                            if (isExist == true)
                            {
                                var lastNameValue = MyDynamic.lastName;

                                if (string.IsNullOrEmpty(lastNameValue))
                                {
                                    MyDynamic.lastName = customerDtoItem.LastName;
                                }
                            }
                            else
                            {
                                MyDynamic.lastName = customerDtoItem.LastName;
                            }

                            var data = await _weClappService.SaveCustomer(weClappUserObj.ApiKey, weClappUserObj.TenantName, MyDynamic);
                            if (!string.IsNullOrEmpty(data.Error))
                            {
                                var ErrorMessage = data.Error;
                                if (data.Error == "unauthorized")
                                {
                                    ErrorMessage = "Please provide valid weclapp credential";
                                }
                                responseDto = _mapper.Map<CustomerSyncToWeClappResponse>(customerDtoItem);
                                return new OperationResult<CustomerSyncToWeClappResponse>(false, System.Net.HttpStatusCode.InternalServerError, ErrorMessage, responseDto);

                            }
                            else
                            {
                                customerDtoItem.WeClappCustomerId = data.Id;
                                var AddUpdate = await _customerService.CheckInsertOrUpdate(customerDtoItem);
                            }
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
                responseDto = _mapper.Map<CustomerSyncToWeClappResponse>(customerDto);
                return new OperationResult<CustomerSyncToWeClappResponse>(true, System.Net.HttpStatusCode.OK, "", responseDto);

            }
            else
            {
                responseDto = _mapper.Map<CustomerSyncToWeClappResponse>(customerDto);
                return new OperationResult<CustomerSyncToWeClappResponse>(false, System.Net.HttpStatusCode.OK, "Please add column mapping", responseDto);
            }
        }


        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CustomerGetAllSalutationResponse>>> Salutations()
        {
            List<SalutationDto> salutationDtoList = new List<SalutationDto>();
            var salutations = _salutationService.GetAll();
            salutationDtoList = _mapper.Map<List<SalutationDto>>(salutations);
            var responseSalutationDtoList = _mapper.Map<List<CustomerGetAllSalutationResponse>>(salutationDtoList);
            return new OperationResult<List<CustomerGetAllSalutationResponse>>(true, System.Net.HttpStatusCode.OK, "", responseSalutationDtoList);
        }

    }
}