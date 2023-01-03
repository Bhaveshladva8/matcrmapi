using System;
using System.Collections.Generic;
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
using System.IdentityModel.Tokens.Jwt;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OneClappRequestFormController : Controller
    {
        private readonly IOneClappRequestFormService _requestFormService;
        private readonly IOneClappFormStatusService _formStatusService;
        private readonly IOneClappFormFieldValueService _formFieldValueService;
        private readonly IOneClappFormService _formService;
        private readonly IOneClappFormFieldService _formFieldService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ILeadService _leadService;
        private readonly ICustomModuleService _customModuleService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomerService _customerService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly IOrganizationService _organizationService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly IOneClappFormService _oneClappFormService;
        private readonly IOneClappFormActionService _oneClappFormActionService;
        private readonly ICustomerEmailService _customerEmailService;
        private readonly ICustomerPhoneService _customerPhoneService;
        private readonly ILabelService _labelService;
        private readonly ILabelCategoryService _labelCategoryService;
        private readonly ILeadLabelService _leadLabelService;
        private readonly ICustomerLabelService _customerLabelService;
        private readonly IOrganizationLabelService _organizationLabelService;
        private readonly IOneClappFormFieldService _oneClappFormFieldService;
        private readonly ISalutationService _salutationService;
        private IMapper _mapper;
        private CustomFieldLogic customFieldLogic;

        private int UserId = 0;
        private int TenantId = 0;

        public OneClappRequestFormController(
        IOneClappRequestFormService requestFormService,
        IOneClappFormStatusService formStatusService,
        IOneClappFormFieldValueService formFieldValueService,
        IOneClappFormService formService,
        IOneClappFormFieldService formFieldService,
        ICustomFieldService customFieldService,
        ILeadService leadService,
        ICustomModuleService customModuleService,
        ICustomFieldValueService customFieldValueService,
        ICustomControlService customControlService,
        ICustomControlOptionService customControlOptionService,
        ICustomerService customerService,
        IModuleFieldService moduleFieldService,
        ITenantModuleService tenantModuleService,
        ICustomTenantFieldService customTenantFieldService,
        ICustomTableService customTableService,
        IOrganizationService organizationService,
        ICustomTableColumnService customTableColumnService,
        IOneClappFormService oneClappFormService,
        IOneClappFormActionService oneClappFormActionService,
        ICustomerEmailService customerEmailService,
        ICustomerPhoneService customerPhoneService,
        ILabelService labelService,
        ILabelCategoryService labelCategoryService,
        ILeadLabelService leadLabelService,
        ICustomerLabelService customerLabelService,
        IOrganizationLabelService organizationLabelService,
        IOneClappFormFieldService oneClappFormFieldService,
        ISalutationService salutationService,
        IMapper mapper
       )
        {
            _requestFormService = requestFormService;
            _formStatusService = formStatusService;
            _formFieldValueService = formFieldValueService;
            _formService = formService;
            _formFieldService = formFieldService;
            _leadService = leadService;
            _customFieldValueService = customFieldValueService;
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
            _organizationService = organizationService;
            _customTableColumnService = customTableColumnService;
            _oneClappFormService = oneClappFormService;
            _oneClappFormActionService = oneClappFormActionService;
            _customerEmailService = customerEmailService;
            _customerPhoneService = customerPhoneService;
            _labelService = labelService;
            _labelCategoryService = labelCategoryService;
            _leadLabelService = leadLabelService;
            _customerLabelService = customerLabelService;
            _organizationLabelService = organizationLabelService;
            _oneClappFormFieldService = oneClappFormFieldService;
            _salutationService = salutationService;
            _mapper = mapper;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
              customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }

        #region FormRequest
        [AllowAnonymous]
        [HttpPost]
        public async Task<OperationResult<OneClappRequestFormAddUpdateResponse>> AddUpdate([FromBody] OneClappRequestFormAddUpdateRequest model)
        {
            var requestmodel = _mapper.Map<OneClappRequestFormDto>(model);
            var requestFormObj = await _requestFormService.CheckInsertOrUpdate(requestmodel);
            if (requestmodel.FormFieldValues != null && requestmodel.FormFieldValues.Count() > 0)
            {
                foreach (var item in requestmodel.FormFieldValues)
                {
                    if (item.CustomFieldId != null)
                    {
                        var customFieldObj = _customFieldService.GetById(item.CustomFieldId.Value);
                        if (customFieldObj.CustomControl != null)
                        {
                            item.ControlType = customFieldObj.CustomControl.Name;
                        }
                    }

                    if (item.OneClappFormFieldId != null)
                    {
                        var formFieldObj = _oneClappFormFieldService.GetById(item.OneClappFormFieldId.Value);
                        item.OneClappRequestFormId = requestFormObj.Id;
                        if (item.OptionId == 0)
                        {
                            item.OptionId = null;
                        }

                        if (formFieldObj != null && formFieldObj.CustomTableColumn != null)
                        {
                            if (formFieldObj.CustomTableColumn.Name == "Salutation")
                            {
                                if (item.OptionId != null)
                                {
                                    var salutationObj = _salutationService.GetById(item.OptionId.Value);
                                    if (salutationObj != null)
                                    {
                                        item.Value = salutationObj.Name;
                                        item.OptionId = null;
                                    }
                                }
                            }
                        }
                    }
                    var oneClappFormFieldValue = await _formFieldValueService.CheckInsertOrUpdate(item);

                }
            }
            requestmodel = _mapper.Map<OneClappRequestFormDto>(requestFormObj);
            var responsemodel = _mapper.Map<OneClappRequestFormAddUpdateResponse>(requestmodel);
            return new OperationResult<OneClappRequestFormAddUpdateResponse>(true, System.Net.HttpStatusCode.OK,"", responsemodel);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]        
        [HttpGet]
        public async Task<OperationResult<List<VerifyFormRequestDto>>> List()
        {
            var requestFormAll = _requestFormService.GetAll();
            var allFormAction = _oneClappFormActionService.GetAll();
            var oneClappRequestFormDtoList = _mapper.Map<List<OneClappRequestFormDto>>(requestFormAll);

            List<OneClappForm> oneClappFormList = _oneClappFormService.GetAll();
            if (oneClappRequestFormDtoList != null && oneClappRequestFormDtoList.Count() > 0)
            {
                foreach (var item in oneClappRequestFormDtoList)
                {
                    item.OneClappFormId = item.OneClappFormId;
                    if (oneClappFormList != null && oneClappFormList.Count() > 0)
                    {
                        var formObj = oneClappFormList.Where(t => t.Id == item.OneClappFormId).FirstOrDefault();
                        if (formObj.FormActionId != null)
                        {
                            var formActionObj = allFormAction.Where(t => t.Id == formObj.FormActionId).FirstOrDefault();
                            if (formActionObj != null)
                            {
                                item.SubmitFormCreatedName = formActionObj.Name;
                            }
                        }
                    }
                    if (item.OneClappFormId != null)
                    {
                        var formFields = _formFieldService.GetAllByForm(item.OneClappFormId.Value);
                        if (formFields != null && formFields.Count() > 0)
                        {
                            formFields = formFields.Where(t => t.CustomFieldId != null || t.CustomTableColumnId != null).ToList();
                            if (item.Id != null)
                            {
                                var requestFormFieldValues = _formFieldValueService.GetByRequestId(item.Id.Value);
                                var oneClappFormFieldDtoList = _mapper.Map<List<OneClappFormFieldDto>>(formFields);
                                item.FormFields = oneClappFormFieldDtoList;
                                if (oneClappFormFieldDtoList != null && oneClappFormFieldDtoList.Count() > 0)
                                {
                                    foreach (var formFieldObj in oneClappFormFieldDtoList)
                                    {
                                        if (requestFormFieldValues != null)
                                        {
                                            var formFieldValue = requestFormFieldValues.Where(t => t.OneClappFormFieldId == formFieldObj.Id).ToList();
                                            Console.WriteLine(formFieldValue);
                                            formFieldObj.FormFieldValues = _mapper.Map<List<OneClappFormFieldValueDto>>(formFieldValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Add Logic for return request form with answer
            List<VerifyFormRequestDto> verifyFormRequestDtoList = new List<VerifyFormRequestDto>();
            if (oneClappRequestFormDtoList != null && oneClappRequestFormDtoList.Count() > 0)
            {
                foreach (var item in oneClappRequestFormDtoList)
                {
                    VerifyFormRequestDto verifyFormRequestDto = new VerifyFormRequestDto();
                    verifyFormRequestDto.Id = item.Id;
                    verifyFormRequestDto.SubmitFormCreatedName = item.SubmitFormCreatedName;
                    verifyFormRequestDto.Fields = new List<FormFieldDto>();

                    if (item.FormFields != null && item.FormFields.Count() > 0)
                    {
                        foreach (var item1 in item.FormFields)
                        {
                            FormFieldDto formFieldDto = new FormFieldDto();
                            if (!string.IsNullOrEmpty(item1.LabelName))
                            {
                                formFieldDto.Label = item1.LabelName;
                            }
                            else
                            {
                                if (item1.CustomField != null)
                                {
                                    formFieldDto.Label = item1.CustomField.Name;
                                }
                                else if (item1.CustomTableColumn != null)
                                {
                                    formFieldDto.Label = item1.CustomTableColumn.Name;
                                }
                            }
                            if (item1.CustomField == null)
                            {
                                if (item1.CustomTableColumnId != null)
                                {
                                    var tableColumnObj = _customTableColumnService.GetById(item1.CustomTableColumnId.Value);
                                    if (tableColumnObj != null && tableColumnObj.CustomControl != null)
                                    {
                                        formFieldDto.ControlType = tableColumnObj.CustomControl.Name;
                                        formFieldDto.Name = tableColumnObj.Name;
                                    }
                                }
                            }
                            else
                            {
                                if (item1.CustomField.CustomControl != null)
                                {
                                    formFieldDto.ControlType = item1.CustomField.CustomControl.Name;
                                }

                            }
                            if (item1.FormFieldValues != null && item1.FormFieldValues.Count() > 0)
                            {
                                foreach (var valueObj in item1.FormFieldValues)
                                {
                                    if (formFieldDto.ControlType == "TextBox" || formFieldDto.ControlType == "TextArea" || formFieldDto.ControlType == "Date")
                                    {
                                        if (valueObj != null)
                                        {
                                            formFieldDto.Value = valueObj.Value;
                                        }
                                    }
                                    else
                                    {
                                        if (formFieldDto != null)
                                        {
                                            if (!string.IsNullOrEmpty(formFieldDto.Value))
                                            {
                                                if (valueObj.CustomControlOption != null)
                                                {
                                                    formFieldDto.Value = formFieldDto.Value + ", " + valueObj.CustomControlOption.Option;
                                                }
                                            }
                                            else
                                            {
                                                if (valueObj.CustomControlOption != null)
                                                {
                                                    formFieldDto.Value = valueObj.CustomControlOption.Option;
                                                }
                                            }
                                        }
                                    }

                                    if (formFieldDto.Name != null && formFieldDto.Name == "Salutation")
                                    {
                                        if (valueObj != null)
                                        {
                                            formFieldDto.Value = valueObj.Value;
                                        }
                                    }
                                }
                            }
                            verifyFormRequestDto.Fields.Add(formFieldDto);

                        }
                    }
                    verifyFormRequestDtoList.Add(verifyFormRequestDto);
                }
            }
            return new OperationResult<List<VerifyFormRequestDto>>(true, System.Net.HttpStatusCode.OK, "", verifyFormRequestDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OneClappRequestFormRemoveResponse>> Remove([FromBody] OneClappRequestFormRemoveRequest model)
        {
            var requestmodel = _mapper.Map<VerifyFormRequestDto>(model);
            if (requestmodel.Id != null)
            {
                var requestFormObj = _requestFormService.DeleteById(requestmodel.Id.Value);
                var requestFormDto = _mapper.Map<OneClappRequestFormDto>(requestFormObj);
                var responserequestFormDto = _mapper.Map<OneClappRequestFormRemoveResponse>(requestFormDto);
                return new OperationResult<OneClappRequestFormRemoveResponse>(true, System.Net.HttpStatusCode.OK,"", responserequestFormDto);                
            }
            else
            {
                return new OperationResult<OneClappRequestFormRemoveResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<OneClappRequestFormVerifyResponse>> Verify([FromBody] OneClappRequestFormVerifyRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
			
			TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<OneClappRequestFormDto>(model);
            if (requestmodel.Id != null)
            {                
                var requestFormObj = _requestFormService.GetById(requestmodel.Id.Value);
                OneClappForm? oneClappFormObj = null;
                if (requestFormObj != null)
                {
                    if (requestFormObj.OneClappFormId != null)
                    {
                        oneClappFormObj = _oneClappFormService.GetById(requestFormObj.OneClappFormId.Value);
                        if (oneClappFormObj != null)
                        {                            
                            if (oneClappFormObj.CreatedBy != null)
                            {
                                UserId = Convert.ToInt32(oneClappFormObj.CreatedBy.Value);
                            }
                        }
                    }
                }

                requestmodel = _mapper.Map<OneClappRequestFormDto>(requestFormObj);
                requestmodel.IsVerify = true;
                if(requestmodel.Id == null)
                {
                    requestmodel.CreatedBy = UserId;
                }
                else
                {
                    requestmodel.UpdatedBy = UserId;
                }
                requestmodel.TenantId = TenantId;
                var AddUpdate = await _requestFormService.CheckInsertOrUpdate(requestmodel);
                if (requestFormObj.OneClappFormId != null)
                {
                    var formFields = _formFieldService.GetAllByForm(requestFormObj.OneClappFormId.Value);
                    var requestFormFieldValues = _formFieldValueService.GetByRequestId(requestmodel.Id.Value);
                    var oneClappFormFieldDtoList = _mapper.Map<List<OneClappFormFieldDto>>(formFields);
                    requestmodel.FormFields = oneClappFormFieldDtoList;

                    Lead? leadObj = null;
                    Organization? organizationObj = null;
                    Customer? customerObj = null;
                    CustomModule? customModuleObj = null;
                    CustomTable? customTableObj = null;

                    var isLeadCreate = false;
                    var isOrganizationCreate = false;
                    var isCustomerCreate = false;

                    if (oneClappFormObj != null && oneClappFormObj.OneClappFormAction != null)
                    {
                        if (oneClappFormObj.OneClappFormAction.Name == "Person")
                        {
                            isCustomerCreate = true;
                            requestmodel.IsCustomerCreate = true;
                        }
                        else if (oneClappFormObj.OneClappFormAction.Name == "Organization")
                        {
                            isOrganizationCreate = true;
                            requestmodel.IsOrganizationCreate = true;
                        }
                        else if (oneClappFormObj.OneClappFormAction.Name == "Lead")
                        {
                            isLeadCreate = true;
                            requestmodel.IsLeadCreate = true;
                        }


                        if (isLeadCreate == true)
                        {
                            LeadDto leadDto = new LeadDto();
                            leadDto.Title = "Lead_" + requestFormObj.Id;
                            if (formFields != null && formFields.Count() > 0)
                            {
                                var fields = formFields.Where(t => t.CustomTableColumn != null && t.CustomTableColumn.CustomTable != null).ToList();
                                if (fields != null && fields.Count() > 0)
                                {
                                    var leadTitleObj = fields.Where(t => t.CustomTableColumn.Name == "Title" && t.CustomTableColumn.CustomTable.Name == "Lead").FirstOrDefault();
                                    if (leadTitleObj != null)
                                    {
                                        if (requestFormFieldValues != null)
                                        {
                                            var LeadTitleValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == leadTitleObj.Id).FirstOrDefault();
                                            if (LeadTitleValueObj != null)
                                            {
                                                leadDto.Title = LeadTitleValueObj.Value;
                                            }
                                        }
                                    }

                                    leadDto.TenantId = TenantId;
                                    leadDto.CreatedBy = UserId;
                                    leadObj = _leadService.CheckInsertOrUpdate(leadDto);
                                    leadDto = _mapper.Map<LeadDto>(leadObj);

                                    var leadLabelObj = fields.Where(t => t.CustomTableColumn.Name == "Label" && t.CustomTableColumn.CustomTable.Name == "Lead").FirstOrDefault();
                                    if (leadLabelObj != null)
                                    {
                                        if (requestFormFieldValues != null)
                                        {
                                            var LeadLabelValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == leadLabelObj.Id).FirstOrDefault();
                                            if (LeadLabelValueObj != null && !string.IsNullOrEmpty(LeadLabelValueObj.Value))
                                            {
                                                var leadLabelCategoryObj = _labelCategoryService.GetByName("Lead");
                                                LabelDto labelDto = new LabelDto();
                                                if (leadLabelCategoryObj != null)
                                                {
                                                    labelDto.LabelCategoryId = leadLabelCategoryObj.Id;
                                                    labelDto.Name = LeadLabelValueObj.Value;
                                                    labelDto.Color = "gray";
                                                    labelDto.CreatedBy = UserId;
                                                    labelDto.TenantId = TenantId;
                                                    var LeadLabelObj = await _labelService.CheckInsertOrUpdate(labelDto);

                                                    LeadLabelDto leadLabelDto = new LeadLabelDto();
                                                    leadLabelDto.LabelId = LeadLabelObj.Id;
                                                    leadLabelDto.LeadId = leadObj.Id;
                                                    leadLabelDto.CreatedBy = UserId;
                                                    leadLabelDto.TenantId = TenantId;
                                                    var LeadLabelObj1 = await _leadLabelService.CheckInsertOrUpdate(leadLabelDto);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            requestmodel.CreatedLead = leadDto;
                            customModuleObj = _customModuleService.GetByName("Lead");
                            customTableObj = _customTableService.GetByName("Lead");
                        }
                    }
                    else if (isOrganizationCreate == true)
                    {

                        OrganizationDto organizationDto = new OrganizationDto();
                        organizationDto.Name = "Organization_" + requestFormObj.Id;
                        if (formFields != null && formFields.Count() > 0)
                        {
                            var fields = formFields.Where(t => t.CustomTableColumn != null && t.CustomTableColumn.CustomTable != null).ToList();
                            if (fields != null && fields.Count() > 0)
                            {
                                var organizationNameObj = fields.Where(t => t.CustomTableColumn.Name == "Name" && t.CustomTableColumn.CustomTable.Name == "Organization").FirstOrDefault();
                                if (organizationNameObj != null)
                                {
                                    if (requestFormFieldValues != null)
                                    {
                                        var OrganizationValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == organizationNameObj.Id).FirstOrDefault();
                                        if (OrganizationValueObj != null)
                                        {
                                            organizationDto.Name = OrganizationValueObj.Value;
                                        }
                                    }
                                }

                                var organizationAddressObj = fields.Where(t => t.CustomTableColumn.Name == "Address" && t.CustomTableColumn.CustomTable.Name == "Organization").FirstOrDefault();
                                if (organizationAddressObj != null)
                                {
                                    if (requestFormFieldValues != null)
                                    {
                                        var OrganizationValueObj1 = requestFormFieldValues.Where(t => t.OneClappFormFieldId == organizationAddressObj.Id).FirstOrDefault();
                                        if (OrganizationValueObj1 != null)
                                        {
                                            organizationDto.Address = OrganizationValueObj1.Value;
                                        }
                                    }
                                }
                                organizationDto.TenantId = TenantId;
                                organizationDto.CreatedBy = UserId;
                                organizationObj = await _organizationService.CheckInsertOrUpdate(organizationDto);
                                organizationDto = _mapper.Map<OrganizationDto>(organizationObj);

                                var organizationLabelObj = fields.Where(t => t.CustomTableColumn.Name == "Label" && t.CustomTableColumn.CustomTable.Name == "Organization").FirstOrDefault();
                                if (organizationLabelObj != null)
                                {
                                    var OrganizationLabelValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == organizationLabelObj.Id).FirstOrDefault();
                                    if (OrganizationLabelValueObj != null && !string.IsNullOrEmpty(OrganizationLabelValueObj.Value))
                                    {
                                        var organizationLabelCategoryObj = _labelCategoryService.GetByName("Organization");
                                        LabelDto labelDto1 = new LabelDto();
                                        if (organizationLabelCategoryObj != null)
                                        {
                                            labelDto1.LabelCategoryId = organizationLabelCategoryObj.Id;
                                            labelDto1.Name = OrganizationLabelValueObj.Value;
                                            labelDto1.Color = "gray";
                                            labelDto1.CreatedBy = UserId;
                                            labelDto1.TenantId = TenantId;
                                            var OrganizationLabelObj = await _labelService.CheckInsertOrUpdate(labelDto1);

                                            OrganizationLabelDto organizationLabelDto = new OrganizationLabelDto();
                                            organizationLabelDto.LabelId = OrganizationLabelObj.Id;
                                            organizationLabelDto.OrganizationId = organizationObj.Id;
                                            organizationLabelDto.CreatedBy = UserId;
                                            organizationLabelDto.TenantId = TenantId;
                                            var OrganizationLabelObj1 = await _organizationLabelService.CheckInsertOrUpdate(organizationLabelDto);
                                        }
                                    }
                                }
                            }
                        }

                        requestmodel.CreatedOrganization = organizationDto;
                        customModuleObj = _customModuleService.GetByName("Organization");
                        customTableObj = _customTableService.GetByName("Organization");
                    }
                    else if (isCustomerCreate == true)
                    {
                        if (formFields != null && formFields.Count() > 0)
                        {
                            CustomerDto customerDto = new CustomerDto();
                            customerDto.Name = "Customer_" + requestFormObj.Id;
                            var fields = formFields.Where(t => t.CustomTableColumn != null && t.CustomTableColumn.CustomTable != null).ToList();
                            if (fields != null && fields.Count() > 0)
                            {
                                var CustomerNameObj = fields.Where(t => t.CustomTableColumn.Name == "Name" && t.CustomTableColumn.CustomTable.Name == "Person").FirstOrDefault();
                                if (CustomerNameObj != null)
                                {
                                    if (requestFormFieldValues != null)
                                    {
                                        var CustomerValueObj1 = requestFormFieldValues.Where(t => t.OneClappFormFieldId == CustomerNameObj.Id).FirstOrDefault();
                                        if (CustomerValueObj1 != null)
                                        {
                                            customerDto.Name = CustomerValueObj1.Value;
                                        }
                                    }
                                }
                                var CustomerFirstNameObj = fields.Where(t => t.CustomTableColumn.Name == "FirstName" && t.CustomTableColumn.CustomTable.Name == "Person").FirstOrDefault();
                                if (CustomerFirstNameObj != null)
                                {
                                    if (requestFormFieldValues != null)
                                    {
                                        var CustomerFirstNameValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == CustomerFirstNameObj.Id).FirstOrDefault();
                                        if (CustomerFirstNameValueObj != null)
                                        {
                                            customerDto.FirstName = CustomerFirstNameValueObj.Value;
                                        }
                                    }
                                }
                                var CustomerLastNameObj = fields.Where(t => t.CustomTableColumn.Name == "LastName" && t.CustomTableColumn.CustomTable.Name == "Person").FirstOrDefault();
                                if (CustomerLastNameObj != null)
                                {
                                    if (requestFormFieldValues != null)
                                    {
                                        var CustomerLastNameValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == CustomerLastNameObj.Id).FirstOrDefault();
                                        if (CustomerLastNameValueObj != null)
                                        {
                                            customerDto.LastName = CustomerLastNameValueObj.Value;
                                        }
                                    }
                                }
                                var CustomerSalutationObj = fields.Where(t => t.CustomTableColumn.Name == "Salutation" && t.CustomTableColumn.CustomTable.Name == "Person").FirstOrDefault();
                                if (CustomerSalutationObj != null)
                                {
                                    if (requestFormFieldValues != null)
                                    {
                                        var CustomerSalutationValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == CustomerSalutationObj.Id).FirstOrDefault();
                                        if (CustomerSalutationValueObj != null)
                                        {
                                            var value = CustomerSalutationValueObj.Value;
                                            if (!string.IsNullOrEmpty(value))
                                            {
                                                var salutationObj = _salutationService.GetByName(value);
                                                if (salutationObj != null)
                                                {
                                                    customerDto.SalutationId = salutationObj.Id;
                                                }
                                            }
                                        }
                                    }
                                }
                                customerDto.TenantId = TenantId;
                                customerDto.CreatedBy = UserId;
                                customerObj = await _customerService.CheckInsertOrUpdate(customerDto);
                                customerDto = _mapper.Map<CustomerDto>(customerObj);

                                var CustomerPhoneObj = fields.Where(t => t.CustomTableColumn.Name == "Phone" && t.CustomTableColumn.CustomTable.Name == "Person").FirstOrDefault();
                                if (CustomerPhoneObj != null)
                                {

                                    CustomerPhoneDto customerPhoneDto = new CustomerPhoneDto();
                                    customerPhoneDto.CustomerId = customerObj.Id;
                                    customerPhoneDto.CreatedBy = UserId;
                                    if (requestFormFieldValues != null)
                                    {
                                        var CustomerValueObj2 = requestFormFieldValues.Where(t => t.OneClappFormFieldId == CustomerPhoneObj.Id).FirstOrDefault();
                                        if (CustomerValueObj2 != null)
                                        {
                                            customerPhoneDto.PhoneNo = CustomerValueObj2.Value;
                                            var AddUpdatePhone = await _customerPhoneService.CheckInsertOrUpdate(customerPhoneDto);
                                        }
                                    }
                                }

                                var CustomerEmailObj = fields.Where(t => t.CustomTableColumn.Name == "Email" && t.CustomTableColumn.CustomTable.Name == "Person").FirstOrDefault();
                                if (CustomerEmailObj != null)
                                {
                                    CustomerEmailDto customerEmailDto = new CustomerEmailDto();
                                    customerEmailDto.CustomerId = customerObj.Id;
                                    if (requestFormFieldValues != null)
                                    {
                                        var CustomerValueObj3 = requestFormFieldValues.Where(t => t.OneClappFormFieldId == CustomerEmailObj.Id).FirstOrDefault();
                                        if (CustomerValueObj3 != null)
                                        {
                                            customerEmailDto.Email = CustomerValueObj3.Value;
                                            customerEmailDto.CreatedBy = UserId;
                                            var AddUpdateEmail = _customerEmailService.CheckInsertOrUpdate(customerEmailDto);
                                        }
                                    }
                                }

                                var customerLabelObj = fields.Where(t => t.CustomTableColumn.Name == "Label" && t.CustomTableColumn.CustomTable.Name == "Person").FirstOrDefault();
                                if (customerLabelObj != null)
                                {
                                    if (requestFormFieldValues != null)
                                    {
                                        var CustomerLabelValueObj = requestFormFieldValues.Where(t => t.OneClappFormFieldId == customerLabelObj.Id).FirstOrDefault();
                                        if (CustomerLabelValueObj != null && !string.IsNullOrEmpty(CustomerLabelValueObj.Value))
                                        {
                                            var customerLabelCategoryObj = _labelCategoryService.GetByName("Person");
                                            LabelDto labelDto2 = new LabelDto();
                                            if (customerLabelCategoryObj != null)
                                            {
                                                labelDto2.LabelCategoryId = customerLabelCategoryObj.Id;
                                                labelDto2.Name = CustomerLabelValueObj.Value;
                                                labelDto2.Color = "gray";
                                                labelDto2.CreatedBy = UserId;
                                                labelDto2.TenantId = TenantId;
                                                var CustomerLabelObj = await _labelService.CheckInsertOrUpdate(labelDto2);

                                                CustomerLabelDto customerLabelDto = new CustomerLabelDto();
                                                customerLabelDto.LabelId = CustomerLabelObj.Id;
                                                customerLabelDto.CustomerId = customerObj.Id;
                                                customerLabelDto.CreatedBy = UserId;
                                                customerLabelDto.TenantId = TenantId;
                                                var CustomerLabelObj1 = await _customerLabelService.CheckInsertOrUpdate(customerLabelDto);
                                            }
                                        }
                                    }
                                }
                            }

                            requestmodel.CreatedCustomer = customerDto;
                            customModuleObj = _customModuleService.GetByName("Person");
                            customTableObj = _customTableService.GetByName("Person");
                        }
                    }
                    if (customModuleObj != null)
                    {
                        // Add Update tenant module
                        var tenantModuleDto = new TenantModuleDto();
                        tenantModuleDto.TenantId = TenantId;
                        tenantModuleDto.ModuleId = customModuleObj.Id;
                        tenantModuleDto.CreatedBy = UserId;
                        var tenantModuleObj = await _tenantModuleService.CheckInsertOrUpdate(tenantModuleDto);
                    }

                    if (formFields.Count() > 0 && customModuleObj != null)
                    {
                        foreach (var item in formFields)
                        {

                            CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                            if (item.CustomField != null)
                            {
                                customTableColumnDto.Name = item.CustomField.Name;
                                customTableColumnDto.ControlId = item.CustomField.ControlId;
                            }
                            else
                            {
                                if (item.CustomTableColumnId != null)
                                {
                                    var customtableColumnId = item.CustomTableColumnId.Value;
                                    item.CustomTableColumn = _customTableColumnService.GetById(customtableColumnId);
                                    customTableColumnDto.Name = item.CustomTableColumn.Name;
                                    customTableColumnDto.ControlId = item.CustomTableColumn.ControlId;
                                }
                            }

                            customTableColumnDto.IsDefault = false;
                            customTableColumnDto.IsHide = true;
                            customTableColumnDto.CustomFieldId = item.CustomFieldId;
                            if (item.CustomTableColumnId != null && item.CustomField == null)
                            {
                                customTableColumnDto.IsDefault = true;
                            }
                            customTableColumnDto.Id = item.CustomTableColumnId;
                            customTableColumnDto.TenantId = Convert.ToInt32(TenantId);
                            if (customTableObj != null)
                            {
                                customTableColumnDto.MasterTableId = customTableObj.Id;
                            }
                            if (item.CustomField != null)
                            {
                                
                                var customTableColumn = await _customTableColumnService.CheckInsertOrUpdate(customTableColumnDto);

                                var customTenantFieldDto = new CustomTenantFieldDto();
                                customTenantFieldDto.TenantId = TenantId;
                                customTenantFieldDto.FieldId = item.CustomFieldId;
                                customTenantFieldDto.CreatedBy = UserId;
                                var customtenantFieldObj = await _customTenantFieldService.CheckInsertOrUpdate(customTenantFieldDto);

                                // Start logic for Add link entity
                                var moduleFieldDto = new ModuleFieldDto();
                                moduleFieldDto.FieldId = item.CustomFieldId;
                                moduleFieldDto.IsHide = false;
                                moduleFieldDto.ModuleId = customModuleObj.Id;
                                moduleFieldDto.CreatedBy = UserId;
                                var moduleFieldObj = await _moduleFieldService.CheckInsertOrUpdate(moduleFieldDto);
                            }
                            // End logic for Add link entity



                            CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                            customFieldValueDto.FieldId = item.CustomFieldId;
                            customFieldValueDto.ModuleId = customModuleObj.Id;
                            if (isLeadCreate == true)
                            {
                                customFieldValueDto.RecordId = leadObj.Id;
                            }
                            else if (isOrganizationCreate == true)
                            {
                                customFieldValueDto.RecordId = organizationObj.Id;
                            }
                            else if (isCustomerCreate == true)
                            {
                                customFieldValueDto.RecordId = customerObj.Id;
                            }

                            var controlType = "";
                            if (item.CustomField != null && item.CustomField.CustomControl != null)
                            {
                                controlType = item.CustomField.CustomControl.Name;
                                customFieldValueDto.ControlType = controlType;
                            }
                            if (item.CustomTableColumn != null && item.CustomTableColumn.CustomControl != null)
                            {
                                controlType = item.CustomTableColumn.CustomControl.Name;
                                customFieldValueDto.ControlType = controlType;
                            }
                            if (item.CustomFieldId != null)
                            {
                                var formFieldValue = _formFieldValueService.GetByRequestAndCustomField(requestFormObj.Id, item.CustomFieldId.Value);

                                if (formFieldValue != null && formFieldValue.Count() > 0)
                                {
                                    foreach (var option in formFieldValue)
                                    {
                                        if (option != null)
                                        {
                                            customFieldValueDto.Value = option.Value;
                                        }
                                        customFieldValueDto.CreatedBy = UserId;
                                        customFieldValueDto.TenantId = TenantId;
                                        customFieldValueDto.OptionId = option.OptionId;
                                        var AddUpdateCustomFieldValue = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                                    }
                                }
                            }
                        }
                    }
                }
                var responsemodel = _mapper.Map<OneClappRequestFormVerifyResponse>(requestmodel);
                return new OperationResult<OneClappRequestFormVerifyResponse>(true, System.Net.HttpStatusCode.OK,"", responsemodel);
            }
            else
            {
                var responsemodel = _mapper.Map<OneClappRequestFormVerifyResponse>(requestmodel);
                return new OperationResult<OneClappRequestFormVerifyResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", responsemodel);
            }

            // }
            // var VerifyformStatusObj = _formStatusService.GetByName("Verify");
            // if (VerifyformStatusObj != null)
            // {
            //     requestFormDto.OneClappFormStatusId = VerifyformStatusObj.Id;
            // }


        }


        #endregion
    }
}