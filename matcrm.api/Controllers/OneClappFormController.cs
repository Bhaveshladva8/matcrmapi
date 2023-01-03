using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Data;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]

    public class OneClappFormController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomerService _customerService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly IOneClappFormService _oneClappFormService;
        private readonly IOneClappFormFieldService _oneClappFormFieldService;
        private readonly IOneClappFormTypeService _oneClappFormTypeService;
        private readonly IOneClappFormActionService _oneClappFormActionService;
        private readonly IOneClappRequestFormService _requestFormService;
        private readonly IOneClappFormFieldValueService _formFieldValueService;
        private readonly IOneClappFormActionService _formActionService;
        private readonly ISalutationService _salutationService;
        private readonly IOneClappFormHeaderService _oneClappFormHeaderService;
        private readonly IOneClappFormLayoutService _oneClappFormLayoutService;
        private readonly IOneClappFormLayoutBackgroundService _oneClappFormLayoutBackgroundService;
        private readonly IBorderStyleService _borderStyleService;
        private IMapper _mapper;
        private CustomFieldLogic customFieldLogic;
        private readonly OneClappContext _context;
        private int UserId = 0;
        private int TenantId = 0;
        public OneClappFormController(
           IHostingEnvironment hostingEnvironment,
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomerService customerService,
            ICustomFieldService customFieldService,
            ICustomModuleService customModuleService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomTableColumnService customTableColumnService,
            ICustomFieldValueService customFieldValueService,
            IOneClappFormService oneClappFormService,
            IOneClappFormFieldService oneClappFormFieldService,
            IOneClappFormTypeService oneClappFormTypeService,
            IOneClappFormActionService oneClappFormActionService,
            IOneClappRequestFormService requestFormService,
            IOneClappFormFieldValueService formFieldValueService,
            IOneClappFormActionService formActionService,
            ISalutationService salutationService,
            IOneClappFormHeaderService oneClappFormHeaderService,
            IOneClappFormLayoutService oneClappFormLayoutService,
            IOneClappFormLayoutBackgroundService oneClappFormLayoutBackgroundService,
            IBorderStyleService borderStyleService,
            IMapper mapper,
            OneClappContext context
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
            _customTableColumnService = customTableColumnService;
            _customFieldValueService = customFieldValueService;
            _oneClappFormService = oneClappFormService;
            _oneClappFormFieldService = oneClappFormFieldService;
            _oneClappFormTypeService = oneClappFormTypeService;
            _oneClappFormActionService = oneClappFormActionService;
            _requestFormService = requestFormService;
            _formFieldValueService = formFieldValueService;
            _formActionService = formActionService;
            _salutationService = salutationService;
            _oneClappFormHeaderService = oneClappFormHeaderService;
            _oneClappFormLayoutService = oneClappFormLayoutService;
            _oneClappFormLayoutBackgroundService = oneClappFormLayoutBackgroundService;
            _borderStyleService = borderStyleService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _context = context;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
               customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
        }

        /// <summary>
        /// Add Update oneclapp form and create dynamic js
        /// </summary>

        [HttpPost]
        public async Task<OperationResult<OneClappFormAddResponse>> Add([FromBody] OneClappFormAddRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var oneClappFormObj = new OneClappForm();
            var requestmodel = _mapper.Map<OneClappFormDto>(model);
            requestmodel.TenantId = TenantId;
            requestmodel.IsActive = true;
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
                requestmodel.SlidingFormPosition = "Right";
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            if (requestmodel.Id == null || requestmodel.Id == 0)
            {
                var leadAction = _formActionService.GetByName("Lead");
                requestmodel.FormGuid = Guid.NewGuid();
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = GuidString.Replace("=", "").Replace("+", "").Replace("/", "");
                if (leadAction != null)
                {
                    requestmodel.FormActionId = leadAction.Id;
                }
                requestmodel.FormKey = GuidString;
            }

            oneClappFormObj = await _oneClappFormService.CheckInsertOrUpdate(requestmodel);
            requestmodel.Id = oneClappFormObj.Id;
            // await _hubContext.Clients.All.OnLeadNoteEventEmit(model.LeadId);
            var responsemodel = _mapper.Map<OneClappFormAddResponse>(requestmodel);
            return new OperationResult<OneClappFormAddResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }


        // [HttpPost]
        // public async Task<OperationResult<OneClappFormAddUpdateResponse>> Add([FromBody] OneClappFormAddUpdateRequest model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     var oneClappFormObj = new OneClappForm();
        //     var requestmodel = _mapper.Map<OneClappFormDto>(model);
        //     requestmodel.TenantId = TenantId;
        //     if (requestmodel.Id == null)
        //     {
        //         requestmodel.CreatedBy = UserId;
        //     }
        //     else
        //     {
        //         requestmodel.UpdatedBy = UserId;
        //     }
        //     if (requestmodel.Id == null || requestmodel.Id == 0)
        //     {
        //         var leadAction = _formActionService.GetByName("Lead");
        //         requestmodel.FormGuid = Guid.NewGuid();
        //         Guid g = Guid.NewGuid();
        //         string GuidString = Convert.ToBase64String(g.ToByteArray());
        //         GuidString = GuidString.Replace("=", "").Replace("+", "").Replace("/", "");
        //         // GuidString = GuidString.Replace("+", "");
        //         // GuidString = GuidString.Replace("+", "");
        //         if (leadAction != null)
        //         {
        //             requestmodel.FormActionId = leadAction.Id;
        //         }
        //         requestmodel.FormKey = GuidString;
        //     }
        //     // if (model.OneClappFormStyle != null)
        //     // {
        //     //     if (model.OneClappFormStyle.BorderDto != null)
        //     //     {
        //     //         BorderDto formBorderDto = model.OneClappFormStyle.BorderDto;
        //     //         var FormBorderAddUpdate = await _borderService.CheckInsertOrUpdate(formBorderDto);
        //     //         model.OneClappFormStyle.BorderId = FormBorderAddUpdate.Id;
        //     //     }

        //     //     if (model.OneClappFormStyle.BoxShadowDto != null)
        //     //     {
        //     //         BoxShadowDto formBoxShadowDto = model.OneClappFormStyle.BoxShadowDto;
        //     //         var FormBoxShadowAddUpdate = await _boxShadowService.CheckInsertOrUpdate(formBoxShadowDto);
        //     //         model.OneClappFormStyle.BoxShadowId = FormBoxShadowAddUpdate.Id;
        //     //     }

        //     //     var FormStyleAddUpdate = await _oneClappFormStyleService.CheckInsertOrUpdate(model.OneClappFormStyle);
        //     //     model.FormStyleId = FormStyleAddUpdate.Id;
        //     // }

        //     if (requestmodel.FormHeader != null)
        //     {
        //         if (requestmodel.FormHeader.CustomHeaderFile != null)
        //         {
        //             var file = requestmodel.FormHeader.CustomHeaderFile;
        //             var dirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormHeader";

        //             if (!Directory.Exists(dirPath))
        //             {
        //                 Directory.CreateDirectory(dirPath);
        //             }

        //             var fileName = string.Concat(
        //                 Path.GetFileNameWithoutExtension(file.FileName),
        //                 DateTime.Now.ToString("yyyyMMdd_HHmmss"),
        //                 Path.GetExtension(file.FileName)
        //             );
        //             var filePath = dirPath + "\\" + fileName;

        //             using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
        //             {
        //                 await file.CopyToAsync(oStream);
        //             }

        //             requestmodel.FormHeader.CustomHeaderImage = fileName;

        //         }

        //         var AddUpdateFormHeader = await _oneClappFormHeaderService.CheckInsertOrUpdate(requestmodel.FormHeader);
        //         requestmodel.FormHeader = _mapper.Map<OneClappFormHeaderDto>(AddUpdateFormHeader);
        //         requestmodel.FormHeaderId = AddUpdateFormHeader.Id;
        //     }

        //     if (requestmodel.FormLayout != null)
        //     {
        //         if (requestmodel.FormLayout.LayoutBackground != null)
        //         {
        //             if (requestmodel.FormLayout.LayoutBackgroundId == null)
        //             {
        //                 requestmodel.FormLayout.LayoutBackground.CreatedBy = UserId;
        //             }
        //             if (requestmodel.FormLayout.LayoutBackground.CustomBackgroundImageFile != null)
        //             {
        //                 var layoutFile = requestmodel.FormLayout.LayoutBackground.CustomBackgroundImageFile;
        //                 var layoutDirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormLayout";

        //                 if (!Directory.Exists(layoutDirPath))
        //                 {
        //                     Directory.CreateDirectory(layoutDirPath);
        //                 }

        //                 var layoutFileName = string.Concat(
        //                     Path.GetFileNameWithoutExtension(layoutFile.FileName),
        //                     DateTime.Now.ToString("yyyyMMdd_HHmmss"),
        //                     Path.GetExtension(layoutFile.FileName)
        //                 );
        //                 var layoutFilePath = layoutDirPath + "\\" + layoutFileName;

        //                 using (var oStream = new FileStream(layoutFilePath, FileMode.Create, FileAccess.ReadWrite))
        //                 {
        //                     await layoutFile.CopyToAsync(oStream);
        //                 }

        //                 requestmodel.FormLayout.LayoutBackground.CustomBackgroundImage = layoutFileName;
        //             }

        //             var FormLayoutBackgroundAddUpdate = await _oneClappFormLayoutBackgroundService.CheckInsertOrUpdate(requestmodel.FormLayout.LayoutBackground);
        //             requestmodel.FormLayout.LayoutBackgroundId = FormLayoutBackgroundAddUpdate.Id;
        //         }

        //         if (requestmodel.FormLayoutId == null)
        //         {
        //             requestmodel.CreatedBy = UserId;
        //         }
        //         var FormLayoutAddUpdate = await _oneClappFormLayoutService.CheckInsertOrUpdate(requestmodel.FormLayout);
        //         requestmodel.FormLayoutId = FormLayoutAddUpdate.Id;
        //     }

        //     // if (model.FormFieldStyle != null)
        //     // {
        //     //     if (model.FormFieldStyle.BorderDto != null)
        //     //     {
        //     //         if (model.FormFieldStyle.BorderId == null)
        //     //         {
        //     //             model.FormFieldStyle.BorderDto.CreatedBy = model.UserId;
        //     //         }
        //     //         var FormFieldBorderAddUpdate = await _borderService.CheckInsertOrUpdate(model.FormFieldStyle.BorderDto);
        //     //         model.FormFieldStyle.BorderId = FormFieldBorderAddUpdate.Id;
        //     //     }

        //     //     if (model.FormFieldStyle.BoxShadowDto != null)
        //     //     {
        //     //         if (model.FormFieldStyle.BoxShadowId == null)
        //     //         {
        //     //             model.FormFieldStyle.BoxShadowDto.CreatedBy = model.UserId;
        //     //         }
        //     //         var FormFieldBoxShadowAddUpdate = await _boxShadowService.CheckInsertOrUpdate(model.FormFieldStyle.BoxShadowDto);
        //     //         model.FormFieldStyle.BoxShadowId = FormFieldBoxShadowAddUpdate.Id;
        //     //     }

        //     //     if (model.FormFieldStyle.BoxShadowDto != null)
        //     //     {
        //     //         if (model.FormFieldStyle.BoxShadowId == null)
        //     //         {
        //     //             model.FormFieldStyle.BoxShadowDto.CreatedBy = model.UserId;
        //     //         }
        //     //         var FormFieldBoxShadowAddUpdate = await _boxShadowService.CheckInsertOrUpdate(model.FormFieldStyle.BoxShadowDto);
        //     //         model.FormFieldStyle.BoxShadowId = FormFieldBoxShadowAddUpdate.Id;
        //     //     }

        //     //     if (model.FormFieldStyle.TypographyDto != null)
        //     //     {
        //     //         if (model.FormFieldStyle.TypographyId == null)
        //     //         {
        //     //             model.FormFieldStyle.TypographyDto.CreatedBy = model.UserId;
        //     //         }
        //     //         var FormFieldTypographyAddUpdate = await _typographyService.CheckInsertOrUpdate(model.FormFieldStyle.TypographyDto);
        //     //         model.FormFieldStyle.TypographyId = FormFieldTypographyAddUpdate.Id;
        //     //     }

        //     //     if (model.FormFieldStyleId == null)
        //     //     {
        //     //         model.FormFieldStyle.CreatedBy = model.UserId;
        //     //     }

        //     //     var AddUpdateFormFieldStyle = await _oneClappFormFieldStyleService.CheckInsertOrUpdate(model.FormFieldStyle);
        //     //     model.FormFieldStyleId = AddUpdateFormFieldStyle.Id;
        //     // }

        //     oneClappFormObj = await _oneClappFormService.CheckInsertOrUpdate(requestmodel);
        //     requestmodel.Id = oneClappFormObj.Id;

        //     var allFields = _oneClappFormFieldService.GetAllByForm(oneClappFormObj.Id);

        //     List<OneClappFormField> oneClappFormFieldList = new List<OneClappFormField>();

        //     if (allFields != null && allFields.Count() > 0)
        //     {
        //         foreach (var item in allFields)
        //         {
        //             OneClappFormFieldDto? isExistData = null;
        //             if (requestmodel.Fields != null)
        //             {
        //                 if (item.CustomFieldId != null)
        //                 {
        //                     isExistData = requestmodel.Fields.Where(t => t.CustomFieldId == item.CustomFieldId && t.CustomModuleId == item.CustomModuleId).FirstOrDefault();
        //                 }
        //                 else
        //                 {
        //                     isExistData = requestmodel.Fields.Where(t => t.CustomTableColumnId == item.CustomTableColumnId && t.CustomModuleId == item.CustomModuleId).FirstOrDefault();
        //                 }
        //             }

        //             if (isExistData == null)
        //             {
        //                 oneClappFormFieldList.Add(item);
        //             }
        //         }
        //     }

        //     if (oneClappFormFieldList != null && oneClappFormFieldList.Count() > 0)
        //     {
        //         var listdeleted = _oneClappFormFieldService.DeleteList(oneClappFormFieldList);
        //     }
        //     if (requestmodel.Fields != null && requestmodel.Fields.Count > 0)
        //     {
        //         foreach (var fieldObj in requestmodel.Fields)
        //         {
        //             OneClappFormFieldDto oneClappFormFieldDto = new OneClappFormFieldDto();
        //             oneClappFormFieldDto.LabelName = fieldObj.LabelName;
        //             oneClappFormFieldDto.PlaceHolder = fieldObj.PlaceHolder;
        //             oneClappFormFieldDto.OneClappFormId = oneClappFormObj.Id;
        //             oneClappFormFieldDto.CustomFieldId = fieldObj.CustomFieldId;
        //             oneClappFormFieldDto.CustomTableColumnId = fieldObj.CustomTableColumnId;
        //             oneClappFormFieldDto.CssClassName = fieldObj.CssClassName;
        //             oneClappFormFieldDto.IsRequired = fieldObj.IsRequired;
        //             oneClappFormFieldDto.CreatedBy = UserId;
        //             oneClappFormFieldDto.CustomModuleId = fieldObj.CustomModuleId;
        //             oneClappFormFieldDto.FormFieldStyle = fieldObj.FormFieldStyle;
        //             oneClappFormFieldDto.TypographyStyle = fieldObj.TypographyStyle;
        //             oneClappFormFieldDto.Priority = fieldObj.Priority;
        //             var AddUpdate = await _oneClappFormFieldService.CheckInsertOrUpdate(oneClappFormFieldDto);
        //             fieldObj.OneClappFormFieldId = AddUpdate.Id;
        //             fieldObj.CreatedOn = AddUpdate.CreatedOn;
        //             // fieldObj.Id = AddUpdate.Id;

        //         }
        //     }
        //     if (oneClappFormObj.Id != null && oneClappFormObj.Id != 0)
        //     {
        //         requestmodel.Id = oneClappFormObj.Id;
        //         requestmodel.FormGuid = oneClappFormObj.FormGuid;

        //         var aa = await GenerateDynamicJS(oneClappFormObj.Id);
        //         requestmodel.JSUrl = "FormsJS" + "/" + oneClappFormObj.FormGuid + "-" + oneClappFormObj.FormKey + "-" + "form.js";
        //     }

        //     // await _hubContext.Clients.All.OnLeadNoteEventEmit(model.LeadId);
        //     var responsemodel = _mapper.Map<OneClappFormAddUpdateResponse>(requestmodel);
        //     return new OperationResult<OneClappFormAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        // }

        [HttpPut]
        public async Task<OperationResult<OneClappFormAddUpdateResponse>> Update([FromBody] OneClappFormAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var oneClappFormObj = new OneClappForm();
            var requestmodel = _mapper.Map<OneClappFormDto>(model);
            requestmodel.TenantId = TenantId;
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            if (requestmodel.Id == null || requestmodel.Id == 0)
            {
                var leadAction = _formActionService.GetByName("Lead");
                requestmodel.FormGuid = Guid.NewGuid();
                Guid g = Guid.NewGuid();
                string GuidString = Convert.ToBase64String(g.ToByteArray());
                GuidString = GuidString.Replace("=", "").Replace("+", "").Replace("/", "");
                // GuidString = GuidString.Replace("+", "");
                // GuidString = GuidString.Replace("+", "");
                if (leadAction != null)
                {
                    requestmodel.FormActionId = leadAction.Id;
                }
                requestmodel.FormKey = GuidString;
            }
            // if (model.OneClappFormStyle != null)
            // {
            //     if (model.OneClappFormStyle.BorderDto != null)
            //     {
            //         BorderDto formBorderDto = model.OneClappFormStyle.BorderDto;
            //         var FormBorderAddUpdate = await _borderService.CheckInsertOrUpdate(formBorderDto);
            //         model.OneClappFormStyle.BorderId = FormBorderAddUpdate.Id;
            //     }

            //     if (model.OneClappFormStyle.BoxShadowDto != null)
            //     {
            //         BoxShadowDto formBoxShadowDto = model.OneClappFormStyle.BoxShadowDto;
            //         var FormBoxShadowAddUpdate = await _boxShadowService.CheckInsertOrUpdate(formBoxShadowDto);
            //         model.OneClappFormStyle.BoxShadowId = FormBoxShadowAddUpdate.Id;
            //     }

            //     var FormStyleAddUpdate = await _oneClappFormStyleService.CheckInsertOrUpdate(model.OneClappFormStyle);
            //     model.FormStyleId = FormStyleAddUpdate.Id;
            // }

            if (requestmodel.FormHeader != null)
            {
                if (requestmodel.FormHeader.CustomHeaderFile != null)
                {
                    var file = requestmodel.FormHeader.CustomHeaderFile;
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormHeader";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormHeaderDirPath;

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    var fileName = string.Concat(
                        Path.GetFileNameWithoutExtension(file.FileName),
                        DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                        Path.GetExtension(file.FileName)
                    );
                    var filePath = dirPath + "\\" + fileName;

                    if (OneClappContext.ClamAVServerIsLive)
                    {
                        ScanDocument scanDocumentObj = new ScanDocument();
                        bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                        if (fileStatus)
                        {
                            return new OperationResult<OneClappFormAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }

                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(oStream);
                    }

                    requestmodel.FormHeader.CustomHeaderImage = fileName;

                }

                var AddUpdateFormHeader = await _oneClappFormHeaderService.CheckInsertOrUpdate(requestmodel.FormHeader);
                requestmodel.FormHeader = _mapper.Map<OneClappFormHeaderDto>(AddUpdateFormHeader);
                requestmodel.FormHeaderId = AddUpdateFormHeader.Id;
            }

            if (requestmodel.FormLayout != null)
            {
                if (requestmodel.FormLayout.LayoutBackground != null)
                {
                    if (requestmodel.FormLayout.LayoutBackgroundId == null)
                    {
                        requestmodel.FormLayout.LayoutBackground.CreatedBy = UserId;
                    }
                    if (requestmodel.FormLayout.LayoutBackground.CustomBackgroundImageFile != null)
                    {
                        var layoutFile = requestmodel.FormLayout.LayoutBackground.CustomBackgroundImageFile;
                        //var layoutDirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormLayout";
                        var layoutDirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormLayoutDirPath;

                        if (!Directory.Exists(layoutDirPath))
                        {
                            Directory.CreateDirectory(layoutDirPath);
                        }

                        var layoutFileName = string.Concat(
                            Path.GetFileNameWithoutExtension(layoutFile.FileName),
                            DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                            Path.GetExtension(layoutFile.FileName)
                        );
                        var layoutFilePath = layoutDirPath + "\\" + layoutFileName;
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(layoutFile);
                            if (fileStatus)
                            {
                                return new OperationResult<OneClappFormAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }

                        using (var oStream = new FileStream(layoutFilePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            await layoutFile.CopyToAsync(oStream);
                        }

                        requestmodel.FormLayout.LayoutBackground.CustomBackgroundImage = layoutFileName;
                    }

                    var FormLayoutBackgroundAddUpdate = await _oneClappFormLayoutBackgroundService.CheckInsertOrUpdate(requestmodel.FormLayout.LayoutBackground);
                    requestmodel.FormLayout.LayoutBackgroundId = FormLayoutBackgroundAddUpdate.Id;
                }

                if (requestmodel.FormLayoutId == null)
                {
                    requestmodel.CreatedBy = UserId;
                }
                var FormLayoutAddUpdate = await _oneClappFormLayoutService.CheckInsertOrUpdate(requestmodel.FormLayout);
                requestmodel.FormLayoutId = FormLayoutAddUpdate.Id;
            }

            // if (model.FormFieldStyle != null)
            // {
            //     if (model.FormFieldStyle.BorderDto != null)
            //     {
            //         if (model.FormFieldStyle.BorderId == null)
            //         {
            //             model.FormFieldStyle.BorderDto.CreatedBy = model.UserId;
            //         }
            //         var FormFieldBorderAddUpdate = await _borderService.CheckInsertOrUpdate(model.FormFieldStyle.BorderDto);
            //         model.FormFieldStyle.BorderId = FormFieldBorderAddUpdate.Id;
            //     }

            //     if (model.FormFieldStyle.BoxShadowDto != null)
            //     {
            //         if (model.FormFieldStyle.BoxShadowId == null)
            //         {
            //             model.FormFieldStyle.BoxShadowDto.CreatedBy = model.UserId;
            //         }
            //         var FormFieldBoxShadowAddUpdate = await _boxShadowService.CheckInsertOrUpdate(model.FormFieldStyle.BoxShadowDto);
            //         model.FormFieldStyle.BoxShadowId = FormFieldBoxShadowAddUpdate.Id;
            //     }

            //     if (model.FormFieldStyle.BoxShadowDto != null)
            //     {
            //         if (model.FormFieldStyle.BoxShadowId == null)
            //         {
            //             model.FormFieldStyle.BoxShadowDto.CreatedBy = model.UserId;
            //         }
            //         var FormFieldBoxShadowAddUpdate = await _boxShadowService.CheckInsertOrUpdate(model.FormFieldStyle.BoxShadowDto);
            //         model.FormFieldStyle.BoxShadowId = FormFieldBoxShadowAddUpdate.Id;
            //     }

            //     if (model.FormFieldStyle.TypographyDto != null)
            //     {
            //         if (model.FormFieldStyle.TypographyId == null)
            //         {
            //             model.FormFieldStyle.TypographyDto.CreatedBy = model.UserId;
            //         }
            //         var FormFieldTypographyAddUpdate = await _typographyService.CheckInsertOrUpdate(model.FormFieldStyle.TypographyDto);
            //         model.FormFieldStyle.TypographyId = FormFieldTypographyAddUpdate.Id;
            //     }

            //     if (model.FormFieldStyleId == null)
            //     {
            //         model.FormFieldStyle.CreatedBy = model.UserId;
            //     }

            //     var AddUpdateFormFieldStyle = await _oneClappFormFieldStyleService.CheckInsertOrUpdate(model.FormFieldStyle);
            //     model.FormFieldStyleId = AddUpdateFormFieldStyle.Id;
            // }

            oneClappFormObj = await _oneClappFormService.CheckInsertOrUpdate(requestmodel);
            requestmodel.Id = oneClappFormObj.Id;

            var allFields = _oneClappFormFieldService.GetAllByForm(oneClappFormObj.Id);

            List<OneClappFormField> oneClappFormFieldList = new List<OneClappFormField>();

            if (allFields != null && allFields.Count() > 0)
            {
                foreach (var item in allFields)
                {
                    OneClappFormFieldDto? isExistData = null;
                    if (requestmodel.Fields != null)
                    {
                        if (item.CustomFieldId != null)
                        {
                            isExistData = requestmodel.Fields.Where(t => t.CustomFieldId == item.CustomFieldId && t.CustomModuleId == item.CustomModuleId).FirstOrDefault();
                        }
                        else
                        {
                            isExistData = requestmodel.Fields.Where(t => t.CustomTableColumnId == item.CustomTableColumnId && t.CustomModuleId == item.CustomModuleId).FirstOrDefault();
                        }
                    }

                    if (isExistData == null)
                    {
                        oneClappFormFieldList.Add(item);
                    }
                }
            }

            if (oneClappFormFieldList != null && oneClappFormFieldList.Count() > 0)
            {
                var listdeleted = _oneClappFormFieldService.DeleteList(oneClappFormFieldList);
            }
            if (requestmodel.Fields != null && requestmodel.Fields.Count > 0)
            {
                foreach (var fieldObj in requestmodel.Fields)
                {
                    OneClappFormFieldDto oneClappFormFieldDto = new OneClappFormFieldDto();
                    oneClappFormFieldDto.LabelName = fieldObj.LabelName;
                    oneClappFormFieldDto.PlaceHolder = fieldObj.PlaceHolder;
                    oneClappFormFieldDto.OneClappFormId = oneClappFormObj.Id;
                    oneClappFormFieldDto.CustomFieldId = fieldObj.CustomFieldId;
                    oneClappFormFieldDto.CustomTableColumnId = fieldObj.CustomTableColumnId;
                    oneClappFormFieldDto.CssClassName = fieldObj.CssClassName;
                    oneClappFormFieldDto.IsRequired = fieldObj.IsRequired;
                    oneClappFormFieldDto.CreatedBy = UserId;
                    oneClappFormFieldDto.CustomModuleId = fieldObj.CustomModuleId;
                    oneClappFormFieldDto.FormFieldStyle = fieldObj.FormFieldStyle;
                    oneClappFormFieldDto.TypographyStyle = fieldObj.TypographyStyle;
                    oneClappFormFieldDto.Priority = fieldObj.Priority;
                    var AddUpdate = await _oneClappFormFieldService.CheckInsertOrUpdate(oneClappFormFieldDto);
                    fieldObj.OneClappFormFieldId = AddUpdate.Id;
                    fieldObj.CreatedOn = AddUpdate.CreatedOn;
                    // fieldObj.Id = AddUpdate.Id;

                }
            }
            if (oneClappFormObj.Id != null && oneClappFormObj.Id != 0)
            {
                requestmodel.Id = oneClappFormObj.Id;
                requestmodel.FormGuid = oneClappFormObj.FormGuid;

                var aa = await GenerateDynamicJS(oneClappFormObj.Id);
                requestmodel.JSUrl = "FormsJS" + "/" + oneClappFormObj.FormGuid + "-" + oneClappFormObj.FormKey + "-" + "form.js";
            }

            // await _hubContext.Clients.All.OnLeadNoteEventEmit(model.LeadId);
            var responsemodel = _mapper.Map<OneClappFormAddUpdateResponse>(requestmodel);
            return new OperationResult<OneClappFormAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        /// <summary>
        /// Get oneclapp for using formid
        /// </summary>
        [HttpGet("{FormId}")]
        public async Task<OperationResult<OneClappFormDto>> DetailById(long FormId)
        {
            var oneClappFormObj = new OneClappForm();

            oneClappFormObj = _oneClappFormService.GetById(FormId);

            var formFields = _oneClappFormFieldService.GetAllByForm(FormId).OrderBy(t => t.Priority).ToList();

            var oneClappFormDto = new OneClappFormDto();
            oneClappFormDto = _mapper.Map<OneClappFormDto>(oneClappFormObj);

            oneClappFormDto.JSUrl = "FormsJS" + "/" + oneClappFormDto.FormGuid + "-" + oneClappFormDto.FormKey + "-" + "form.js";
            oneClappFormDto.ModalPopUpUrl = "ModalFormsJS" + "/" + oneClappFormDto.FormGuid + "-" + oneClappFormDto.FormKey + "-" + "form.js";
            oneClappFormDto.SlidingFormUrl = "SlidingFormJS" + "/" + oneClappFormDto.FormGuid + "-" + oneClappFormDto.FormKey + "-" + "form.js";

            var customControls = _customControlService.GetAllControl();

            // var customFields = _customFieldService.GetById();
            if (formFields != null && formFields.Count() > 0)
            {
                foreach (var fieldObj in formFields)
                {
                    OneClappFormFieldDto oneClappFormFieldDto = new OneClappFormFieldDto();
                    oneClappFormFieldDto = _mapper.Map<OneClappFormFieldDto>(fieldObj);
                    if (fieldObj.CustomFieldId != null)
                    {
                        var customFieldId = fieldObj.CustomFieldId.Value;
                        var customFieldObj = _customFieldService.GetById(fieldObj.CustomFieldId.Value);
                        oneClappFormFieldDto.CustomFieldDto = _mapper.Map<CustomFieldDto>(customFieldObj);
                        var controlObj = customControls.Where(t => t.Id == customFieldObj.ControlId).FirstOrDefault();
                        oneClappFormFieldDto.CustomControl = _mapper.Map<CustomControlDto>(controlObj);
                    }

                    // if (customFieldObj != null)
                    // {
                    //     if (customFieldObj.Name == "DropDown" || customFieldObj.Name == "Radio" || customFieldObj.Name == "Checkbox")
                    //     {
                    //         var options = _customControlOptionService.GetAllControlOption(customFieldId);
                    //         formFieldDto.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(options);
                    //     }
                    // }
                    oneClappFormDto.Fields.Add(oneClappFormFieldDto);
                }
            }
            // await _hubContext.Clients.All.OnLeadNoteEventEmit(model.LeadId);
            return new OperationResult<OneClappFormDto>(true, System.Net.HttpStatusCode.OK, "", oneClappFormDto);
        }

        /// <summary>
        /// Get oneclapp for using formkey
        /// </summary>
        //[HttpGet("GetByKey")]
        [HttpGet("{FormKey}")]
        public async Task<OperationResult<OneClappFormGetByKeyResponse>> Detail(string FormKey)
        {
            var oneClappFormObj = new OneClappForm();

            oneClappFormObj = _oneClappFormService.GetByFormKey(FormKey);

            var formFields = _oneClappFormFieldService.GetAllByForm(oneClappFormObj.Id);

            var oneClappFormDto = new OneClappFormDto();
            oneClappFormDto = _mapper.Map<OneClappFormDto>(oneClappFormObj);

            // if (addUpdateForm.FormStyleId != null && addUpdateForm.OneClappFormStyle != null)
            // {
            //     customFormDto.OneClappFormStyle = _mapper.Map<OneClappFormStyleDto>(addUpdateForm.OneClappFormStyle);

            //     if (addUpdateForm.OneClappFormStyle.BoxShadowId != null)
            //     {
            //         customFormDto.OneClappFormStyle.BoxShadowDto = _mapper.Map<BoxShadowDto>(addUpdateForm.OneClappFormStyle.BoxShadow);
            //     }

            //     if (addUpdateForm.OneClappFormStyle.BorderId != null)
            //     {
            //         customFormDto.OneClappFormStyle.BorderDto = _mapper.Map<BorderDto>(addUpdateForm.OneClappFormStyle.Border);
            //     }
            // }

            if (oneClappFormObj.FormHeaderId != null && oneClappFormObj.OneClappFormHeader != null)
            {
                oneClappFormDto.FormHeader = _mapper.Map<OneClappFormHeaderDto>(oneClappFormObj.OneClappFormHeader);

            }

            if (oneClappFormObj.FormLayoutId != null && oneClappFormObj.OneClappFormLayout != null)
            {
                oneClappFormDto.FormLayout = _mapper.Map<OneClappFormLayoutDto>(oneClappFormObj.OneClappFormLayout);

                if (oneClappFormObj.OneClappFormLayout.LayoutBackgroundId != null)
                {
                    oneClappFormDto.FormLayout.LayoutBackground = _mapper.Map<OneClappFormLayoutBackgroundDto>(oneClappFormObj.OneClappFormLayout.OneClappFormLayoutBackground);
                }
            }

            // if (addUpdateForm.FormFieldStyleId != null && addUpdateForm.OneClappFormFieldStyle != null)
            // {
            //     customFormDto.FormFieldStyle = _mapper.Map<OneClappFormFieldStyleDto>(addUpdateForm.OneClappFormFieldStyle);

            //     if (addUpdateForm.OneClappFormFieldStyle.BorderId != null)
            //     {
            //         customFormDto.FormFieldStyle.BorderDto = _mapper.Map<BorderDto>(addUpdateForm.OneClappFormFieldStyle.Border);

            //         if (addUpdateForm.OneClappFormFieldStyle.Border.BorderStyleId != null)
            //         {
            //             customFormDto.FormFieldStyle.BorderDto.BorderStyleDto = _mapper.Map<BorderStyleDto>(addUpdateForm.OneClappFormFieldStyle.Border.BorderStyle);
            //         }
            //     }

            //     if (addUpdateForm.OneClappFormFieldStyle.BoxShadowId != null)
            //     {
            //         customFormDto.FormFieldStyle.BoxShadowDto = _mapper.Map<BoxShadowDto>(addUpdateForm.OneClappFormFieldStyle.BoxShadow);
            //     }

            //     if (addUpdateForm.OneClappFormFieldStyle.TypographyId != null)
            //     {
            //         customFormDto.FormFieldStyle.TypographyDto = _mapper.Map<TypographyDto>(addUpdateForm.OneClappFormFieldStyle.Typography);
            //     }
            // }

            oneClappFormDto.JSUrl = "FormsJS" + "/" + oneClappFormDto.FormGuid + "-" + oneClappFormDto.FormKey + "-" + "form.js";
            oneClappFormDto.ModalPopUpUrl = "ModalFormsJS" + "/" + oneClappFormDto.FormGuid + "-" + oneClappFormDto.FormKey + "-" + "form.js";
            oneClappFormDto.SlidingFormUrl = "SlidingFormJS" + "/" + oneClappFormDto.FormGuid + "-" + oneClappFormDto.FormKey + "-" + "form.js";

            var customControls = _customControlService.GetAllControl();

            if (formFields != null && formFields.Count() > 0)
            {
                formFields = formFields.Where(t => t.CustomFieldId != null || t.CustomTableColumnId != null).OrderBy(t => t.Priority).ToList();

                // var customFields = _customFieldService.GetById();

                foreach (var fieldObj in formFields)
                {
                    OneClappFormFieldDto oneClappFormFieldDto = new OneClappFormFieldDto();
                    oneClappFormFieldDto = _mapper.Map<OneClappFormFieldDto>(fieldObj);

                    if (fieldObj.CustomFieldId != null)
                    {
                        var customFieldId = fieldObj.CustomFieldId.Value;
                        var customFieldObj = _customFieldService.GetById(fieldObj.CustomFieldId.Value);
                        oneClappFormFieldDto.CustomFieldDto = _mapper.Map<CustomFieldDto>(customFieldObj);
                        if (customFieldObj != null)
                        {
                            var controlObj = customControls.Where(t => t.Id == customFieldObj.ControlId).FirstOrDefault();
                            oneClappFormFieldDto.CustomControl = _mapper.Map<CustomControlDto>(controlObj);
                        }
                        else
                        {
                            oneClappFormFieldDto.CustomControl = new CustomControlDto();
                        }
                        var options = _customControlOptionService.GetAllControlOption(fieldObj.CustomFieldId.Value);
                        oneClappFormFieldDto.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(options);
                    }
                    else
                    {
                        if (fieldObj.CustomTableColumnId != null)
                        {
                            var tableColumnId = fieldObj.CustomTableColumnId.Value;
                            var tableColumnObj = _customTableColumnService.GetById(tableColumnId);
                            oneClappFormFieldDto.CustomControl = _mapper.Map<CustomControlDto>(tableColumnObj.CustomControl);
                            oneClappFormFieldDto.CustomTableColumn = tableColumnObj;
                            CustomFieldDto customFieldDto = new CustomFieldDto();
                            customFieldDto.Name = tableColumnObj.Name;
                            customFieldDto.CustomTableColumnId = tableColumnObj.Id;
                            // fieldObj.CustomModuleId = tableColumnObj.

                            if (tableColumnObj != null && tableColumnObj.Name == "Salutation")
                            {
                                var options = _salutationService.GetAll();
                                foreach (var optionObj in options)
                                {
                                    CustomControlOptionDto customControlOptionDto = new CustomControlOptionDto();
                                    customControlOptionDto.Id = optionObj.Id;
                                    customControlOptionDto.Option = optionObj.Name;
                                    oneClappFormFieldDto.CustomControlOptions.Add(customControlOptionDto);
                                }
                            }
                        }
                    }


                    // if (customFieldObj != null)
                    // {
                    //     if (customFieldObj.Name == "DropDown" || customFieldObj.Name == "Radio" || customFieldObj.Name == "Checkbox")
                    //     {
                    //         var options = _customControlOptionService.GetAllControlOption(customFieldId);
                    //         formFieldDto.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(options);
                    //     }
                    // }
                    oneClappFormDto.Fields.Add(oneClappFormFieldDto);
                    // customFormDto.FormStyle  = JsonConvert.DeserializeObject<object>(customFormDto.FormStyle.ToString());
                }
            }
            var responsemodel = _mapper.Map<OneClappFormGetByKeyResponse>(oneClappFormDto);
            return new OperationResult<OneClappFormGetByKeyResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        /// <summary>
        /// Get all form types
        /// </summary>
        //[HttpGet("GetAllFormTypes")]
        [HttpGet]
        public async Task<OperationResult<List<OneClappFormTypeDto>>> Types()
        {
            List<OneClappFormTypeDto> oneClappFormTypeDtoList = new List<OneClappFormTypeDto>();
            var formTypeList = _oneClappFormTypeService.GetAll();
            oneClappFormTypeDtoList = _mapper.Map<List<OneClappFormTypeDto>>(formTypeList);
            return new OperationResult<List<OneClappFormTypeDto>>(true, System.Net.HttpStatusCode.OK, "", oneClappFormTypeDtoList);
        }

        /// <summary>
        /// Get all form actions
        /// </summary>
        //[HttpGet("GetAllFormAction")]
        [HttpGet]
        public async Task<OperationResult<List<OneClappFormActionResponse>>> Actions()
        {
            List<OneClappFormActionDto> oneClappFormActionDtoList = new List<OneClappFormActionDto>();
            var formActionList = _oneClappFormActionService.GetAll();
            oneClappFormActionDtoList = _mapper.Map<List<OneClappFormActionDto>>(formActionList);
            var responseformActionDtos = _mapper.Map<List<OneClappFormActionResponse>>(oneClappFormActionDtoList);
            return new OperationResult<List<OneClappFormActionResponse>>(true, System.Net.HttpStatusCode.OK, "", responseformActionDtos);
        }

        /// <summary>
        /// Get all oneclapp form by userid and tenantid
        /// </summary>
        [HttpPost]
        public async Task<OperationResult<List<OneClappFormGetAllResponse>>> List([FromBody] OneClappFormGetAllRequest model)
        {
            List<OneClappFormDto> oneClappFormDtoList = new List<OneClappFormDto>();
            List<OneClappFormGetAllResponse> responseDtos = new List<OneClappFormGetAllResponse>();
            var requestmodel = _mapper.Map<OneClappFormDto>(model);
            if (requestmodel.TenantId != null && requestmodel.UserId != null)
            {
                var formTypeList = _oneClappFormService.GetByUserAndTenant(requestmodel.TenantId.Value, requestmodel.UserId.Value);
                oneClappFormDtoList = _mapper.Map<List<OneClappFormDto>>(formTypeList);
                foreach (var item in oneClappFormDtoList)
                {
                    if (item.Id != null)
                    {
                        item.SubmissionCount = _requestFormService.GetByFormId(item.Id.Value).Count();
                    }
                }
                responseDtos = _mapper.Map<List<OneClappFormGetAllResponse>>(oneClappFormDtoList);
                return new OperationResult<List<OneClappFormGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseDtos);

            }
            else
            {
                responseDtos = _mapper.Map<List<OneClappFormGetAllResponse>>(oneClappFormDtoList);
                return new OperationResult<List<OneClappFormGetAllResponse>>(false, System.Net.HttpStatusCode.OK, "Please provide tenantid and userid.", responseDtos);
            }

        }

        /// <summary>
        /// Update form status active, disable
        /// </summary>
        [HttpPut]
        public async Task<OperationResult<OneClappFormUpdateStatusResponse>> Status([FromBody] OneClappFormUpdateStatusRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<OneClappFormDto>(model);
            OneClappFormUpdateStatusResponse responsemodel = new OneClappFormUpdateStatusResponse();

            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            if (requestmodel.Id != null)
            {
                var formObj = _oneClappFormService.GetById(requestmodel.Id.Value);
                formObj.IsActive = requestmodel.IsActive;
                requestmodel = _mapper.Map<OneClappFormDto>(formObj);
                var fieldObj = await _oneClappFormService.CheckInsertOrUpdate(requestmodel);
                responsemodel = _mapper.Map<OneClappFormUpdateStatusResponse>(requestmodel);
                return new OperationResult<OneClappFormUpdateStatusResponse>(true, System.Net.HttpStatusCode.OK, "Update status successfully", responsemodel);
            }
            responsemodel = _mapper.Map<OneClappFormUpdateStatusResponse>(requestmodel);
            return new OperationResult<OneClappFormUpdateStatusResponse>(false, System.Net.HttpStatusCode.OK, "Please provide form id", responsemodel);
        }

        /// <summary>
        /// Update form position left, right , bottom
        /// </summary>
        [HttpPut]
        public async Task<OperationResult<OneClappFormPositionResponse>> Position([FromBody] OneClappFormPositionRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<OneClappFormDto>(model);
            OneClappFormPositionResponse responsemodel = new OneClappFormPositionResponse();
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            if (requestmodel.Id != null)
            {
                var formObj = _oneClappFormService.GetById(requestmodel.Id.Value);
                formObj.SlidingFormPosition = requestmodel.SlidingFormPosition;
                requestmodel = _mapper.Map<OneClappFormDto>(formObj);
                var fieldObj = await _oneClappFormService.CheckInsertOrUpdate(requestmodel);
                responsemodel = _mapper.Map<OneClappFormPositionResponse>(requestmodel);
                return new OperationResult<OneClappFormPositionResponse>(true, System.Net.HttpStatusCode.OK, "Update status successfully", responsemodel);
            }
            return new OperationResult<OneClappFormPositionResponse>(false, System.Net.HttpStatusCode.OK, "Please provide form id", responsemodel);
        }

        /// <summary>
        /// Add /Remove / Update oneclapp form field by id
        /// </summary>
        [HttpPut]
        public async Task<OperationResult<OneClappFormFieldVM>> FormField([FromBody] OneClappFormFieldVM model)
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


            if (model.Id != null && model.CustomModuleId != null)
            {
                var fieldObj = await _oneClappFormFieldService.CheckInsertOrUpdate1(model);
                model = _mapper.Map<OneClappFormFieldVM>(fieldObj);

                if (fieldObj.OneClappFormId != null)
                {
                    var formId = fieldObj.OneClappFormId.Value;
                    var aa = await GenerateDynamicJS(formId);
                }

                return new OperationResult<OneClappFormFieldVM>(true, System.Net.HttpStatusCode.OK, "Updated field successfully", model);
            }
            return new OperationResult<OneClappFormFieldVM>(false, System.Net.HttpStatusCode.OK, "Please provide id and custom module id", model);
        }

        /// <summary>
        /// Remove oneclapp form field by id
        /// </summary>
        [HttpDelete]
        public async Task<OperationResult<OneClappFormFieldDto>> RemoveFormField([FromBody] OneClappFormFieldDto model)
        {
            if (model.Id != null)
            {
                var formId = model.OneClappFormId;
                if (formId == null)
                {
                    var fieldData = _oneClappFormFieldService.GetById(model.Id.Value);
                    formId = fieldData.OneClappFormId;
                }
                var fieldObj = _oneClappFormFieldService.DeleteOneClappFormField(model.Id.Value);
                if (formId != null)
                {
                    var aa = await GenerateDynamicJS(formId.Value);
                }

                return new OperationResult<OneClappFormFieldDto>(true, System.Net.HttpStatusCode.OK, "Deleted successfully", model);
            }
            return new OperationResult<OneClappFormFieldDto>(false, System.Net.HttpStatusCode.OK, "Please provide id", model);
        }

        /// <summary>
        /// Remove oneclapp form id
        /// </summary>
        //[HttpPost("Remove")]
        [HttpDelete]
        public async Task<OperationResult<OneClappFormRemoveResponse>> Remove([FromBody] OneClappFormRemoveRequest model)
        {
            var requestmodel = _mapper.Map<OneClappFormDto>(model);
            OneClappFormRemoveResponse responsemodel = new OneClappFormRemoveResponse();
            if (requestmodel.Id != null)
            {
                var fieldObj = _oneClappFormFieldService.DeleteByFormId(requestmodel.Id.Value);
                var formFieldValues = _formFieldValueService.DeleteByForm(requestmodel.Id.Value);
                var requestFormList = _requestFormService.DeleteByFormId(requestmodel.Id.Value);
                var formObj = _oneClappFormService.GetById(requestmodel.Id.Value);
                //var dirPath = _hostingEnvironment.WebRootPath + "\\FormsJS";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.FormsJSUploadDirPath;
                var filePath = dirPath + "\\" + formObj.FormGuid + "-" + formObj.FormKey + "-" + "form.js";

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }
                var removeformObj = _oneClappFormService.DeleteOneClappForm(requestmodel.Id.Value);
                responsemodel = _mapper.Map<OneClappFormRemoveResponse>(requestmodel);
                return new OperationResult<OneClappFormRemoveResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
            return new OperationResult<OneClappFormRemoveResponse>(false, System.Net.HttpStatusCode.OK, "Please provide form id", responsemodel);
        }


        [AllowAnonymous]
        [HttpGet("{Name}")]
        public async Task<ActionResult> JSFile(string Name)
        {
            //var dirPath = _hostingEnvironment.WebRootPath + "\\FormsJS";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.FormsJSUploadDirPath;
            var filePath = dirPath + "\\" + Name;

            Byte[] newBytes = System.IO.File.ReadAllBytes(filePath);
            String file = Convert.ToBase64String(newBytes);
            return File(newBytes, "text/javascript", Path.GetFileName(filePath));
        }

        private async Task<bool> GenerateDynamicJS(long FormId)
        {
            OneClappFormDto oneClappFormDto = new OneClappFormDto();
            if (FormId != null && FormId != 0)
            {
                var formObj = _oneClappFormService.GetById(FormId);
                oneClappFormDto = _mapper.Map<OneClappFormDto>(formObj);
                var fieldList = _oneClappFormFieldService.GetAllByForm(FormId);
                oneClappFormDto.Fields = _mapper.Map<List<OneClappFormFieldDto>>(fieldList);
                oneClappFormDto.Fields = oneClappFormDto.Fields.Where(t => t.CustomFieldId != null || t.CustomTableColumnId != null).OrderBy(t => t.Priority).ToList();

                oneClappFormDto.Id = FormId;
                oneClappFormDto.FormGuid = formObj.FormGuid;

                // var data = Common.GetFieldCode(model);
                FormClass formClassObj = new FormClass();
                formClassObj.FormId = formObj.Id;
                formClassObj.FormName = oneClappFormDto.Name;
                formClassObj.IsActive = true;
                // ObjFormClass.CreatedOn = formObj.CreatedOn;
                // ObjFormClass.LastModifiedOn = formObj.UpdatedOn;
                // ObjFormClass.IsDeleted = formObj.IsDeleted;
                // ObjFormClass.DeletedOn = formObj.DeletedOn;
                // ObjFormClass.HeadLine = "AAAAAA";
                // ObjFormClass.Description = "Test Description";
                formClassObj.ButtonText = formObj.ButtonText;
                formClassObj.ButtonTextSubmit = formObj.ButtonText;
                formClassObj.RedirectUrl = formObj.RedirectUrl;
                // ObjFormClass.IsSendEmail = false;
                // ObjFormClass.FontSize = 20;
                // ObjFormClass.Opacity = null;
                // ObjFormClass.FormLablePlaceId = null;
                // ObjFormClass.LandingPageLoad = 1;

                #region 'Form Fields'

                // DataTable dtFormFields = dstForm.Tables[1];

                List<FormFieldVM> formFieldVMList = new List<FormFieldVM>();
                oneClappFormDto.Fields = oneClappFormDto.Fields.OrderBy(t => t.CreatedOn).OrderBy(t => t.Priority).ToList();
                formClassObj.FormFields = formFieldVMList;
                formClassObj.CustomFields = oneClappFormDto.Fields;
                #endregion

                #region 'Form HTML String'
                FormModalPopUP formModalPopUPObj = new FormModalPopUP();

                formModalPopUPObj.FormId = formObj.Id;
                // ObjFormModalPopup.Description = "Test Description";
                // ObjFormModalPopup.FontSize = 15;
                // ObjFormModalPopup.Opacity = 15;
                formModalPopUPObj.ButtonText = formObj.ButtonText;
                formModalPopUPObj.ButtonTextSubmit = formObj.ButtonText;
                // ObjFormModalPopup.FormLabelPlaceId = 1;
                formModalPopUPObj.FormName = formObj.Name;
                // ObjFormModalPopup.Orientation = 5;
                // ObjFormModalPopup.ImageName = null;
                // ObjFormModalPopup.ImagePlacement = 1;
                // ObjFormModalPopup.ImagePath = Common.BuildUrl("ImagePath") + drForm.Field<string>("ImageName");
                // ObjFormModalPopup.FormFields = ObjFormClass.FormFields;
                formModalPopUPObj.CustomFields = formClassObj.CustomFields;
                formModalPopUPObj.FormStyle = formObj.FormStyle;
                formModalPopUPObj.HeaderStyle = formObj.HeaderStyle;
                formModalPopUPObj.LayoutStyle = formObj.LayoutStyle;
                // ObjFormModalPopup.SubmitUrl = Common.BuildUrl("SubmitWidgetUrl") + formObj.Id;
                // ObjFormModalPopup.ButtonColor = "red";

                // var contentstyle = "";
                // var headerstyle = "";
                // var sidetabclass = "";
                // switch (ObjFormModalPopup.Orientation)
                // {
                //     case 1:
                //         sidetabclass = "side left";
                //         contentstyle = "height: auto; left: 0px;";
                //         headerstyle = "left: 22px;";
                //         break;
                //     case 2:
                //         sidetabclass = "side right";
                //         contentstyle = "height: auto; right: 0px;";
                //         headerstyle = "right: 22px;";
                //         break;
                //     case 3:
                //         sidetabclass = "bottom left";
                //         contentstyle = "height: auto; bottom: 0px;";
                //         headerstyle = "";
                //         break;
                //     case 4:
                //         sidetabclass = "bottom right";
                //         contentstyle = "height: auto; bottom: 0px;";
                //         headerstyle = "";
                //         break;
                //     default:
                //         sidetabclass = "bottom right";
                //         contentstyle = "height: auto;";
                //         headerstyle = "";
                //         break;
                // }
                // ObjFormModalPopup.HeaderStyle = headerstyle;
                // ObjFormModalPopup.WidgetStyle = contentstyle;
                // ObjFormModalPopup.SideTabClass = sidetabclass;

                if (formModalPopUPObj.CustomFields != null && formModalPopUPObj.CustomFields.Count() > 0)
                {
                    // var customControls = _customControlService.GetAllControl();

                    foreach (var fieldObj in formModalPopUPObj.CustomFields)
                    {
                        fieldObj.OneClappFormFieldId = fieldObj.Id;
                        if (fieldObj.CustomFieldId != null)
                        {
                            var customField = _customFieldService.GetById(fieldObj.CustomFieldId.Value);
                            if (customField != null && customField.ControlId != null)
                            {
                                if (customField.CustomControl != null)
                                {
                                    fieldObj.CustomControl = _mapper.Map<CustomControlDto>(customField.CustomControl);
                                    if (customField.CustomControl.Name == "Radio" || customField.CustomControl.Name == "DropDown" || customField.CustomControl.Name == "Checkbox")
                                    {
                                        var options = _customControlOptionService.GetAllControlOption(fieldObj.CustomFieldId.Value);
                                        fieldObj.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(options);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (fieldObj.CustomTableColumnId != null)
                            {
                                var tableColumnObj = _customTableColumnService.GetById(fieldObj.CustomTableColumnId.Value);
                                if (tableColumnObj != null && tableColumnObj.ControlId != null)
                                {
                                    fieldObj.CustomControl = _mapper.Map<CustomControlDto>(tableColumnObj.CustomControl);
                                    // if (fieldObj.CustomControl.Name == "Radio" || fieldObj.CustomControl.Name == "DropDown" || fieldObj.CustomControl.Name == "Checkbox")
                                    // {
                                    //     var options = _customControlOptionService.GetAllControlOption(fieldObj.CustomFieldId.Value);
                                    //     fieldObj.CustomControlOptions = _mapper.Map<List<CustomControlOptionDto>>(options);
                                    // }

                                    // Add Logic for Salutation value 
                                    if (tableColumnObj.Name == "Salutation")
                                    {
                                        var SalutationList = _salutationService.GetAll();
                                        if (SalutationList != null && SalutationList.Count() > 0)
                                        {
                                            foreach (var item in SalutationList)
                                            {
                                                CustomControlOptionDto customControlOptionDto = new CustomControlOptionDto();
                                                customControlOptionDto.Id = item.Id;
                                                customControlOptionDto.Option = item.Name;
                                                fieldObj.CustomControlOptions.Add(customControlOptionDto);
                                            }
                                        }
                                    }
                                    // End logic
                                }
                            }
                        }

                    }
                }


                // ObjFormClass.FormHTMLString = Common.GetModalPopUpHtml(ObjFormModalPopup);

                formClassObj.FormHTMLString = DynamicJs.GetEmbededCode(formModalPopUPObj);

                #endregion

                // ObjFormClass.EmailTemplate = FormEmailTemplate;
                string layoutStyle = "";
                if (formModalPopUPObj.LayoutStyle != null)
                {
                    layoutStyle = formModalPopUPObj.LayoutStyle.ToString().Replace("_", "-").Replace("\"", "").Replace("\\", "").Replace("{", "").Replace(",", ";").Replace("}", ";");
                }
                formClassObj.LayoutBackground = layoutStyle;

                string Contents = "Form" + "=" + JsonConvert.SerializeObject(formClassObj);

                CreateJSVM createJSVMObj = new CreateJSVM();

                createJSVMObj.FormId = formClassObj.FormId;
                createJSVMObj.FormName = formClassObj.FormName;

                //var dirPath = _hostingEnvironment.WebRootPath + "\\FormsJS";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.FormsJSUploadDirPath;
                var filePath = dirPath + "\\" + formObj.FormGuid + "-" + formObj.FormKey + "-" + "form.js";

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }
                // ObjJs.JSPath = Server.MapPath("~/FormsJS/") + FormId + ".js"; //Common.BuildUrl("JSPath") 
                createJSVMObj.JSPath = filePath;
                createJSVMObj.Contents = Contents;
                createJSVMObj.SubmitUrl = Common.BuildUrl("SubmitWidgetUrl") + formObj.Id;
                createJSVMObj.WidgetUrl = Common.BuildUrl("OpenWidgetUrl") + formObj.Id;


                // Common.CreateJS(ObjJs);
                DynamicJs.CreateJS(createJSVMObj);


                #region Create JS for Modal popup

                formClassObj.FormHTMLString = DynamicJs.GetModalPopUpHtml(formModalPopUPObj);
                formClassObj.LayoutBackground = "";

                string ModalPopUpContents = "Form" + "=" + JsonConvert.SerializeObject(formClassObj);

                CreateJSVM ModalPopUpFormObjJs = new CreateJSVM();

                ModalPopUpFormObjJs.FormId = formClassObj.FormId;
                ModalPopUpFormObjJs.FormName = formClassObj.FormName;

                //var ModalFormDirPath = _hostingEnvironment.WebRootPath + "\\ModalFormsJS";
                var ModalFormDirPath = _hostingEnvironment.WebRootPath + OneClappContext.ModalFormsJSUploadDirPath;
                var ModalFormFilePath = ModalFormDirPath + "\\" + formObj.FormGuid + "-" + formObj.FormKey + "-" + "form.js";

                if (!Directory.Exists(ModalFormDirPath))
                {
                    Directory.CreateDirectory(ModalFormDirPath);
                }

                if (System.IO.File.Exists(ModalFormFilePath))
                {
                    System.IO.File.Delete(Path.Combine(ModalFormFilePath));
                }
                ModalPopUpFormObjJs.JSPath = ModalFormFilePath;
                ModalPopUpFormObjJs.Contents = ModalPopUpContents;

                oneClappFormDto.ModalPopUpUrl = "ModalFormsJS" + "/" + formObj.FormGuid + "-" + formObj.FormKey + "-" + "form.js";
                DynamicJs.CreateJS(ModalPopUpFormObjJs);

                #endregion

                #region Create Js for Sliding FormJS

                formClassObj.FormHTMLString = DynamicJs.GetSlidingFormHtml(formModalPopUPObj);
                formClassObj.LayoutBackground = "";

                string SlidingContents = "Form" + "=" + JsonConvert.SerializeObject(formClassObj);

                CreateJSVM SlidingFormObjJs = new CreateJSVM();

                SlidingFormObjJs.FormId = formClassObj.FormId;
                SlidingFormObjJs.FormName = formClassObj.FormName;

                //var slidingFormDirPath = _hostingEnvironment.WebRootPath + "\\SlidingFormJS";
                var slidingFormDirPath = _hostingEnvironment.WebRootPath + OneClappContext.SlidingFormJSUploadDirPath;
                var slidingFormFilePath = slidingFormDirPath + "\\" + formObj.FormGuid + "-" + formObj.FormKey + "-" + "form.js";

                if (!Directory.Exists(slidingFormDirPath))
                {
                    Directory.CreateDirectory(slidingFormDirPath);
                }

                if (System.IO.File.Exists(slidingFormFilePath))
                {
                    System.IO.File.Delete(Path.Combine(slidingFormFilePath));
                }

                SlidingFormObjJs.JSPath = slidingFormFilePath;
                SlidingFormObjJs.Contents = SlidingContents;
                DynamicJs.CreateJS(SlidingFormObjJs);

                oneClappFormDto.SlidingFormUrl = "SlidingFormJS" + "/" + formObj.FormGuid + "-" + formObj.FormKey + "-" + "form.js";

                #endregion

                oneClappFormDto.JSUrl = "FormsJS" + "/" + formObj.FormGuid + "-" + formObj.FormKey + "-" + "form.js";
                return true;
            }
            return false;
        }


    }

}