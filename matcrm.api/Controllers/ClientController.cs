using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.service.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using matcrm.data.Models.Tables;
using matcrm.data.Context;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using matcrm.service.Utility;
using matcrm.data.Models.Dto;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;
using matcrm.service.Services.ERP;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using Microsoft.AspNetCore.Http;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IClientEmailService _clientEmailService;
        private readonly IClientPhoneService _clientPhoneService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IMapper _mapper;
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IEmployeeProjectActivityService _employeeProjectActivityService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IContractService _contractService;
        private readonly IContractArticleService _contractArticleService;
        private readonly IInvoiceMollieCustomerService _invoiceMollieCustomerService;
        private readonly IClientInvoiceService _clientInvoiceService;
        private readonly IInvoiceMollieSubscriptionService _invoiceMollieSubscriptionService;
        private readonly IMateProjectTimeRecordService _mateProjectTimeRecordService;
        private readonly IMateTaskTimeRecordService _mateTaskTimeRecordService;
        private readonly IContractActivityService _contractActivityService;
        private readonly IInvoiceIntervalService _invoiceIntervalService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IContractInvoiceService _contractInvoiceService;
        private readonly IProjectContractService _projectContractService;
        private readonly IClientSocialMediaService _clientSocialMediaService;
        private readonly IClientUserService _clientUserService;
        private readonly IClientAppointmentService _clientAppointmentService;
        private readonly ICRMNotesService _cRMNotesService;
        private readonly IClientIntProviderAppSecretService _clientIntProviderAppSecretService;
        private readonly IIntProviderContactService _intProviderContactService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomModuleService _customModuleService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IMateSubTaskTimeRecordService _mateSubTaskTimeRecordService;
        private readonly IMateChildTaskTimeRecordService _mateChildTaskTimeRecordService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly IIntProviderAppService _intProviderAppService;
        private readonly ICustomDomainEmailConfigService _customDomainEmailConfigService;
        private readonly ITeamInboxService _teamInboxService;
        private readonly ICustomerAttachmentService _customerAttachmentService;
        private readonly IGoogleCalendarService _calendarService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IServiceArticlePriceService _serviceArticlePriceService;
        private readonly IEmployeeClientTaskService _employeeClientTaskService;
        private readonly IEmployeeProjectTaskService _employeeProjectTaskService;
        private readonly IMateClientTicketService _mateClientTicketService;
        private readonly IMateTicketActivityService _mateTicketActivityService;
        private readonly IMateProjectTicketService _mateProjectTicketService;
        private readonly IMateTicketTaskService _mateTicketTaskService;

        private MailInbox mailInbox;
        private Common Common;
        private int UserId = 0;
        private int TenantId = 0;

        public ClientController(IClientService clientService,
            IClientEmailService clientEmailService,
            IClientPhoneService clientPhoneService,
            IHostingEnvironment hostingEnvironment,
            IEmployeeProjectService employeeProjectService,
            IEmployeeProjectActivityService employeeProjectActivityService,
            IEmployeeTaskService employeeTaskService,
            IEmployeeTaskActivityService employeeTaskActivityService,
            IContractService contractService,
            IContractArticleService contractArticleService,
            IInvoiceMollieCustomerService invoiceMollieCustomerService,
            IClientInvoiceService clientInvoiceService,
            IInvoiceMollieSubscriptionService invoiceMollieSubscriptionService,
            IMateProjectTimeRecordService mateProjectTimeRecordService,
            IMateTaskTimeRecordService mateTaskTimeRecordService,
            IContractActivityService contractActivityService,
            IInvoiceIntervalService invoiceIntervalService,
            IUserService userService,
            IRoleService roleService,
            IContractInvoiceService contractInvoiceService,
            IProjectContractService projectContractService,
            IClientSocialMediaService clientSocialMediaService,
            IClientUserService clientUserService,
            IClientAppointmentService clientAppointmentService,
            ICRMNotesService cRMNotesService,
            IClientIntProviderAppSecretService clientIntProviderAppSecretService,
            IIntProviderContactService intProviderContactService,
            ICustomTableService customTableService,
            ICustomModuleService customModuleService,
            ICustomFieldValueService customFieldValueService,
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeChildTaskService employeeChildTaskService,
            IMateSubTaskTimeRecordService mateSubTaskTimeRecordService,
            IMateChildTaskTimeRecordService mateChildTaskTimeRecordService,
            IIntProviderAppService intProviderAppService,
            IIntProviderAppSecretService intProviderAppSecretService,
            OneClappContext context,
            ICustomDomainEmailConfigService customDomainEmailConfigService,
            ITeamInboxService teamInboxService,
            ICustomerAttachmentService customerAttachmentService,
            IGoogleCalendarService calendarService,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            IServiceArticlePriceService serviceArticlePriceService,
            IEmployeeClientTaskService employeeClientTaskService,
            IEmployeeProjectTaskService employeeProjectTaskService,
            IMateClientTicketService mateClientTicketService,
            IMateTicketActivityService mateTicketActivityService,
            IMateProjectTicketService mateProjectTicketService,
            IMateTicketTaskService mateTicketTaskService,
            IMapper mapper)
        {
            _clientService = clientService;
            _clientEmailService = clientEmailService;
            _clientPhoneService = clientPhoneService;
            _hostingEnvironment = hostingEnvironment;
            _employeeProjectService = employeeProjectService;
            _employeeProjectActivityService = employeeProjectActivityService;
            _employeeTaskService = employeeTaskService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _contractService = contractService;
            _contractArticleService = contractArticleService;
            _invoiceMollieCustomerService = invoiceMollieCustomerService;
            _clientInvoiceService = clientInvoiceService;
            _invoiceMollieSubscriptionService = invoiceMollieSubscriptionService;
            _mateProjectTimeRecordService = mateProjectTimeRecordService;
            _mateTaskTimeRecordService = mateTaskTimeRecordService;
            _contractActivityService = contractActivityService;
            _invoiceIntervalService = invoiceIntervalService;
            _userService = userService;
            _roleService = roleService;
            _contractInvoiceService = contractInvoiceService;
            _projectContractService = projectContractService;
            _clientSocialMediaService = clientSocialMediaService;
            _clientUserService = clientUserService;
            _clientAppointmentService = clientAppointmentService;
            _cRMNotesService = cRMNotesService;
            _clientIntProviderAppSecretService = clientIntProviderAppSecretService;
            _intProviderContactService = intProviderContactService;
            _customTableService = customTableService;
            _customModuleService = customModuleService;
            _customFieldValueService = customFieldValueService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _mateSubTaskTimeRecordService = mateSubTaskTimeRecordService;
            _mateChildTaskTimeRecordService = mateChildTaskTimeRecordService;
            _intProviderAppService = intProviderAppService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _customDomainEmailConfigService = customDomainEmailConfigService;
            _teamInboxService = teamInboxService;
            _customerAttachmentService = customerAttachmentService;
            _calendarService = calendarService;
            _mapper = mapper;
            _hubContext = hubContext;
            _serviceArticlePriceService = serviceArticlePriceService;
            _employeeClientTaskService = employeeClientTaskService;
            _employeeProjectTaskService = employeeProjectTaskService;
            _mateClientTicketService = mateClientTicketService;
            _mateTicketActivityService = mateTicketActivityService;
            _mateProjectTicketService = mateProjectTicketService;
            _mateTicketTaskService = mateTicketTaskService;
            mailInbox = new MailInbox(context, userService, hostingEnvironment, intProviderAppService, intProviderAppSecretService, customerAttachmentService, calendarService, teamInboxService, customDomainEmailConfigService, mapper);
            Common = new Common();
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ClientAddUpdateResponse>> Add([FromForm] ClientAddUpdateRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestModel.Emails != null && requestModel.Emails.Count > 0)
            {
                var model = _mapper.Map<Client>(requestModel);
                if (model.Id == null || model.Id == 0)
                {
                    model.CreatedBy = UserId;
                }
                model.TenantId = TenantId;

                var filePath = "";
                if (requestModel.FileName != null)
                {
                    model.Logo = requestModel.FileName;
                }
                if (requestModel.File != null)
                {
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientLogoDirPath;

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    var fileName = string.Concat(
                                    Path.GetFileNameWithoutExtension(requestModel.File.FileName),
                                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                    Path.GetExtension(requestModel.File.FileName)
                                );
                    filePath = dirPath + "\\" + fileName;


                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    model.Logo = fileName;

                    if (OneClappContext.ClamAVServerIsLive)
                    {
                        ScanDocument scanDocumentObj = new ScanDocument();
                        bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestModel.File);
                        if (fileStatus)
                        {
                            return new OperationResult<ClientAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }

                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await requestModel.File.CopyToAsync(oStream);
                    }
                }

                //invoice interval
                if (requestModel.InvoiceIntervalId != null && requestModel.Interval != null)
                {
                    //check for user id tenant admin or not
                    var checkUser = _userService.GetUserById(UserId);
                    if (checkUser.RoleId != null)
                    {
                        var roleObj = _roleService.GetRoleById(checkUser.RoleId.Value);
                        if (roleObj.RoleName == "TenantAdmin")
                        {
                            var invoiceIntervalObj = _invoiceIntervalService.GetById(requestModel.InvoiceIntervalId.Value);
                            if (invoiceIntervalObj.Interval != requestModel.Interval)
                            {
                                InvoiceInterval intervalObj = new InvoiceInterval();
                                intervalObj.Name = invoiceIntervalObj.Name;
                                intervalObj.Interval = requestModel.Interval;
                                intervalObj.CreatedBy = UserId;
                                var AddUpdateInvoiceInterval = await _invoiceIntervalService.CheckInsertOrUpdate(intervalObj, TenantId);
                                model.InvoiceIntervalId = AddUpdateInvoiceInterval.Id;
                            }
                        }
                    }
                }

                var clientObj = await _clientService.CheckInsertOrUpdate(model);
                //Client email
                if (clientObj != null && requestModel.Emails != null && requestModel.Emails.Count > 0)
                {
                    foreach (var item in requestModel.Emails)
                    {
                        var clientEmailObj = _mapper.Map<ClientEmail>(item);
                        if (clientEmailObj != null)
                        {
                            clientEmailObj.ClientId = clientObj.Id;
                            if (clientEmailObj.Id == 0)
                            {
                                clientEmailObj.CreatedBy = UserId;
                            }
                        }
                        var AddUpdateEmail = await _clientEmailService.CheckInsertOrUpdate(clientEmailObj);
                    }
                }
                //Client phone
                if (clientObj != null && requestModel.Phones != null && requestModel.Phones.Count > 0)
                {
                    foreach (var item in requestModel.Phones)
                    {
                        var clientPhoneObj = _mapper.Map<ClientPhone>(item);
                        if (clientPhoneObj != null)
                        {
                            clientPhoneObj.ClientId = clientObj.Id;
                            if (clientPhoneObj.Id == 0)
                            {
                                clientPhoneObj.CreatedBy = UserId;
                            }
                        }
                        var AddUpdatePhone = await _clientPhoneService.CheckInsertOrUpdate(clientPhoneObj);
                    }
                }
                //Client custom field
                CustomModule? customModuleObj = null;
                var customTable = _customTableService.GetByName("Client");
                if (customTable != null)
                {
                    customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                }
                if (requestModel.CustomFields != null && requestModel.CustomFields.Count() > 0 && customModuleObj != null && clientObj != null)
                {
                    foreach (var item in requestModel.CustomFields)
                    {
                        if (item != null)
                        {
                            CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                            customFieldValueDto.FieldId = item.Id;
                            customFieldValueDto.ModuleId = customModuleObj.Id;
                            customFieldValueDto.RecordId = clientObj.Id;
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

                ClientAddUpdateResponse clientAddUpdateResponseObj = new ClientAddUpdateResponse();
                clientAddUpdateResponseObj = _mapper.Map<ClientAddUpdateResponse>(clientObj);
                if (clientObj != null)
                {
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (clientObj.Logo != null)
                    {
                        clientAddUpdateResponseObj.LogoURL = OneClappContext.CurrentURL + "Client/Logo/" + clientAddUpdateResponseObj.Id + "?" + Timestamp;
                    }
                    else
                    {
                        clientAddUpdateResponseObj.LogoURL = null;
                    }
                    clientAddUpdateResponseObj.Status = clientObj.IsActive ? "Active" : "Inactive";
                }
                return new OperationResult<ClientAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Client added successfully", clientAddUpdateResponseObj);
            }
            else
            {
                return new OperationResult<ClientAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Please provide atleast one email");
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<ClientAddUpdateResponse>> Update([FromForm] ClientAddUpdateRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestModel.Emails != null && requestModel.Emails.Count > 0)
            {
                var model = _mapper.Map<Client>(requestModel);

                model.TenantId = TenantId;

                var filePath = "";
                if (requestModel.FileName != null)
                {
                    model.Logo = requestModel.FileName;
                }
                if (requestModel.File != null)
                {
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientLogoDirPath;

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    var fileName = string.Concat(
                                    Path.GetFileNameWithoutExtension(requestModel.File.FileName),
                                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                    Path.GetExtension(requestModel.File.FileName)
                                );
                    filePath = dirPath + "\\" + fileName;


                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    model.Logo = fileName;

                    if (OneClappContext.ClamAVServerIsLive)
                    {
                        ScanDocument scanDocumentObj = new ScanDocument();
                        bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestModel.File);
                        if (fileStatus)
                        {
                            return new OperationResult<ClientAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }

                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await requestModel.File.CopyToAsync(oStream);
                    }
                }

                //invoice interval                
                if (requestModel.InvoiceIntervalId != null && requestModel.Interval != null)
                {
                    //check for user id tenant admin or not
                    var checkUser = _userService.GetUserById(UserId);
                    if (checkUser.RoleId != null)
                    {
                        var roleObj = _roleService.GetRoleById(checkUser.RoleId.Value);
                        if (roleObj.RoleName == "TenantAdmin")
                        {
                            var invoiceIntervalObj = _invoiceIntervalService.GetById(requestModel.InvoiceIntervalId.Value);
                            if (invoiceIntervalObj.Interval != requestModel.Interval)
                            {
                                InvoiceInterval intervalObj = new InvoiceInterval();
                                intervalObj.Name = invoiceIntervalObj.Name;
                                intervalObj.Interval = requestModel.Interval;
                                intervalObj.CreatedBy = UserId;
                                var AddUpdateInvoiceInterval = await _invoiceIntervalService.CheckInsertOrUpdate(intervalObj, TenantId);
                                model.InvoiceIntervalId = AddUpdateInvoiceInterval.Id;
                            }
                        }
                    }
                }

                var clientObj = await _clientService.CheckInsertOrUpdate(model);

                if (clientObj != null && requestModel.Emails != null && requestModel.Emails.Count > 0)
                {
                    var clientEmails = _clientEmailService.GetByClientId(clientObj.Id);
                    foreach (var item in clientEmails)
                    {
                        var requestModelId = requestModel.Emails.Where(t => t.Id == item.Id).Select(t => t.Id).FirstOrDefault();
                        if (item.Id == requestModelId)
                        {
                            var clientemailmodelObj = requestModel.Emails.Where(t => t.Id == item.Id).FirstOrDefault();
                            var clientEmailObj = _mapper.Map<ClientEmail>(clientemailmodelObj);
                            if (clientEmailObj != null)
                            {
                                clientEmailObj.ClientId = clientObj.Id;
                                if (clientEmailObj.Id == 0)
                                {
                                    clientEmailObj.CreatedBy = UserId;
                                }
                            }
                            var AddUpdateEmail = await _clientEmailService.CheckInsertOrUpdate(clientEmailObj);
                        }
                        else
                        {
                            var DeleteClientEmail = await _clientEmailService.DeleteById(item.Id);
                        }
                    }
                    //Console.WriteLine(requestModel.Emails);
                    foreach (var itememail in requestModel.Emails.Where(t => t.Id == null).ToList())
                    {
                        var clientEmailObj = _mapper.Map<ClientEmail>(itememail);
                        if (clientEmailObj != null)
                        {
                            clientEmailObj.ClientId = clientObj.Id;
                            if (clientEmailObj.Id == 0)
                            {
                                clientEmailObj.CreatedBy = UserId;
                            }
                        }
                        var AddUpdateEmail = await _clientEmailService.CheckInsertOrUpdate(clientEmailObj);
                    }
                }

                if (clientObj != null && requestModel.Phones != null && requestModel.Phones.Count > 0)
                {
                    var clientPhones = _clientPhoneService.GetByClientId(clientObj.Id);

                    foreach (var item in clientPhones)
                    {
                        var requestModelPhoneId = requestModel.Phones.Where(t => t.Id == item.Id).Select(t => t.Id).FirstOrDefault();
                        if (item.Id == requestModelPhoneId)
                        {
                            var clientPhonemodelObj = requestModel.Phones.Where(t => t.Id == item.Id).FirstOrDefault();
                            var clientPhoneObj = _mapper.Map<ClientPhone>(clientPhonemodelObj);
                            if (clientPhoneObj != null)
                            {
                                clientPhoneObj.ClientId = clientObj.Id;
                                if (clientPhoneObj.Id == 0)
                                {
                                    clientPhoneObj.CreatedBy = UserId;
                                }
                            }
                            var AddUpdatePhone = await _clientPhoneService.CheckInsertOrUpdate(clientPhoneObj);
                        }
                        else
                        {
                            var DeleteClientPhone = await _clientPhoneService.DeleteById(item.Id);
                        }
                    }
                    foreach (var itemPhone in requestModel.Phones.Where(t => t.Id == null).ToList())
                    {
                        var clientPhoneObj = _mapper.Map<ClientPhone>(itemPhone);
                        if (clientPhoneObj != null)
                        {
                            clientPhoneObj.ClientId = clientObj.Id;
                            if (clientPhoneObj.Id == 0)
                            {
                                clientPhoneObj.CreatedBy = UserId;
                            }
                        }
                        var AddUpdatePhone = await _clientPhoneService.CheckInsertOrUpdate(clientPhoneObj);
                    }

                    // foreach (var item in requestModel.Phones)
                    // {
                    //     var clientPhoneObj = _mapper.Map<ClientPhone>(item);
                    //     if (clientPhoneObj != null)
                    //     {
                    //         clientPhoneObj.ClientId = clientObj.Id;
                    //         if (clientPhoneObj.Id == 0)
                    //         {
                    //             clientPhoneObj.CreatedBy = UserId;
                    //         }
                    //     }
                    //     var AddUpdatePhone = await _clientPhoneService.CheckInsertOrUpdate(clientPhoneObj);
                    // }
                }

                ClientAddUpdateResponse clientAddUpdateResponseObj = new ClientAddUpdateResponse();
                clientAddUpdateResponseObj = _mapper.Map<ClientAddUpdateResponse>(clientObj);
                if (clientObj != null)
                {
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (clientObj.Logo != null)
                    {
                        clientAddUpdateResponseObj.LogoURL = OneClappContext.CurrentURL + "Client/Logo/" + clientAddUpdateResponseObj.Id + "?" + Timestamp;
                    }
                    else
                    {
                        clientAddUpdateResponseObj.LogoURL = null;
                    }
                    clientAddUpdateResponseObj.Status = clientObj.IsActive ? "Active" : "Inactive";
                }
                return new OperationResult<ClientAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Client updated successfully", clientAddUpdateResponseObj);
            }
            else
            {
                return new OperationResult<ClientAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Please provide atleast one email");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<ClientListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var clientList = _clientService.GetByTenant(TenantId);
            //var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);

            var clientListResponses = _mapper.Map<List<ClientListResponse>>(clientList);
            //int totalCount = 0;
            //totalCount = clientListResponses.Count();
            // if (!string.IsNullOrEmpty(requestModel.SearchString))
            // {
            //     clientListResponses = clientListResponses.Where(t => (!string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.FirstName) && t.FirstName.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.LastName) && t.LastName.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
            //     clientListResponses = clientListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            // }
            // else
            // {
            //     clientListResponses = clientListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            // }

            if (clientListResponses != null)
            {
                if (clientListResponses != null && clientListResponses.Count > 0)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ProjectLogo";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientLogoDirPath;
                    //filePath = dirPath + "\\" + item.Logo;
                    foreach (var item in clientListResponses)
                    {
                        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        if (item.Logo != null)
                        {
                            item.LogoURL = OneClappContext.CurrentURL + "Client/Logo/" + item.Id + "?" + Timestamp;
                        }
                        else
                        {
                            item.LogoURL = null;
                        }
                    }
                }
            }
            return new OperationResult<List<ClientListResponse>>(true, System.Net.HttpStatusCode.OK, "", clientListResponses);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (Id != null && Id > 0)
            {
                var clientInvoiceList = _clientInvoiceService.GetAllByClient(Id).OrderBy(t => t.EndDate).ToList();
                //var clientInvoiceIdList = clientInvoiceList.Select(t => t.Id).ToList();
                //var clientInvoiceStatus = _invoiceMollieSubscriptionService.GetListByInvoiceIdList(clientInvoiceIdList);

                var employeeProjectList = _employeeProjectService.GetAllByClient(Id, TenantId);
                var employeeProjectIdList = employeeProjectList.Select(t => t.Id).ToList();
                var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetListByProjectIdList(employeeProjectIdList);

                //var employeeTaskList = _employeeTaskService.GetTaskByClientWithOutProject(Id, TenantId);

                var employeeClientTaskList = _employeeClientTaskService.GetTaskByClientWithTenant(Id, TenantId);
                var employeeProjectTaskList = _employeeProjectTaskService.GetByTenant(TenantId);

                var employeeTaskList = employeeClientTaskList.Where(p => !employeeProjectTaskList.Any(p2 => p2.EmployeeTaskId == p.EmployeeTaskId)).ToList();

                var employeeTaskIdList = employeeProjectList.Select(t => t.Id).ToList();
                var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetListByTaskIdList(employeeTaskIdList);

                // if (clientInvoiceList != null && clientInvoiceList.Count > 0)
                // {
                //     var ClientInvoiceLastRecord = clientInvoiceList.LastOrDefault();

                //     if (ClientInvoiceLastRecord.EndDate.Value.Date < DateTime.UtcNow.Date)
                //     {
                //         var ProjectTimeRecordExist = mateProjectTimeRecordList.Where(t => t.MateTimeRecord.CreatedOn != null && (t.MateTimeRecord.CreatedOn.Value.Date > ClientInvoiceLastRecord.EndDate.Value.Date && t.MateTimeRecord.CreatedOn.Value <= DateTime.UtcNow.Date)).ToList();
                //         if (ProjectTimeRecordExist != null && ProjectTimeRecordExist.Count > 0)
                //         {
                //             return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please Generate Invoice", Id);
                //         }
                //         var taskTimeRecordExist = mateTaskTimeRecordList.Where(t => t.MateTimeRecord.CreatedOn != null && (t.MateTimeRecord.CreatedOn.Value.Date > ClientInvoiceLastRecord.EndDate.Value.Date && t.MateTimeRecord.CreatedOn.Value <= DateTime.UtcNow.Date)).ToList();
                //         if (taskTimeRecordExist != null && taskTimeRecordExist.Count > 0)
                //         {
                //             return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please Generate Invoice", Id);
                //         }
                //         foreach (var clientInvoice in clientInvoiceList)
                //         {
                //             //check status moliie subscription by client invoice                        
                //             var checkStatus = _invoiceMollieSubscriptionService.GetByInvoiceId(clientInvoice.Id);
                //             if (checkStatus != null)
                //             {
                //                 if (checkStatus.Status != OneClappContext.PaidPaymentStatus)
                //                 {
                //                     return new OperationResult(false, System.Net.HttpStatusCode.OK, "Payment remaining", Id);
                //                 }
                //             }
                //             else
                //             {
                //                 return new OperationResult(false, System.Net.HttpStatusCode.OK, "Payment remaining", Id);
                //             }
                //         }
                //     }
                //     else if (ClientInvoiceLastRecord.EndDate.Value.Date == DateTime.UtcNow.Date)
                //     {
                //         foreach (var clientInvoice in clientInvoiceList)
                //         {
                //             //check status moliie subscription by client invoice                        
                //             var checkStatus = _invoiceMollieSubscriptionService.GetByInvoiceId(clientInvoice.Id);
                //             if (checkStatus != null)
                //             {
                //                 if (checkStatus.Status != OneClappContext.PaidPaymentStatus)
                //                 {
                //                     return new OperationResult(false, System.Net.HttpStatusCode.OK, "Payment remaining", Id);
                //                 }
                //             }
                //             else
                //             {
                //                 return new OperationResult(false, System.Net.HttpStatusCode.OK, "Payment remaining", Id);
                //             }
                //         }
                //     }
                // }
                // else
                // {
                //     var ProjectTimeRecordExist = mateProjectTimeRecordList.Where(t => t.MateTimeRecord.CreatedOn != null && (t.MateTimeRecord.CreatedOn.Value.Date <= DateTime.UtcNow.Date)).ToList();
                //     if (ProjectTimeRecordExist != null && ProjectTimeRecordExist.Count > 0)
                //     {
                //         return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please Generate Invoice", Id);
                //     }
                //     var taskTimeRecordExist = mateTaskTimeRecordList.Where(t => t.MateTimeRecord.CreatedOn.Value.Date <= DateTime.UtcNow.Date).ToList();
                //     if (taskTimeRecordExist != null && taskTimeRecordExist.Count > 0)
                //     {
                //         return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please Generate Invoice", Id);
                //     }
                // }
                //client email
                var clientEmailList = await _clientEmailService.DeleteByClientId(Id);
                //client phone
                var clientPhoneList = await _clientPhoneService.DeleteByClientId(Id);
                //clientSocialMedia
                var clientSocialMediaList = await _clientSocialMediaService.DeleteByClientId(Id);
                //clientAppointment
                var clientAppointmentList = await _clientAppointmentService.DeleteByClientId(Id);
                //CRM notes
                var cRMNoteList = await _cRMNotesService.DeleteByClientId(Id);
                //clientIntProviderAppSecret
                var intProviderAppSecretList = await _clientIntProviderAppSecretService.DeleteByClientId(Id);
                //IntProviderContact
                var intProviderContactList = await _intProviderContactService.DeleteByClientId(Id);
                //clientUser
                var clientUserList = await _clientUserService.DeleteByClientId(Id);

                //Client Ticket
                var mateClientTicketList = await _mateClientTicketService.DeleteByClientId(Id);
                if (mateClientTicketList != null)
                {
                    foreach (var clientTicket in mateClientTicketList)
                    {
                        MateTicketActivity mateClientTicketActivityObj = new MateTicketActivity();
                        mateClientTicketActivityObj.MateTicketId = clientTicket.MateTicketId;
                        mateClientTicketActivityObj.CreatedBy = UserId;
                        mateClientTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Unassign_client_from_ticket.ToString().Replace("_", " ");
                        var AddUpdateClientTicketActivity = await _mateTicketActivityService.CheckInsertOrUpdate(mateClientTicketActivityObj);
                        await _hubContext.Clients.All.OnMateTicketModuleEvent(clientTicket.MateTicketId, TenantId);
                    }

                }

                //employee project                
                if (employeeProjectList != null && employeeProjectList.Count > 0)
                {
                    foreach (var project in employeeProjectList)
                    {
                        if (project != null)
                        {
                            var projectObj = _mapper.Map<EmployeeProjectDto>(project);
                            projectObj.ClientId = null;
                            var employeeProjectObj = _employeeProjectService.CheckInsertOrUpdate(projectObj);
                            if (employeeProjectObj != null)
                            {
                                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                                employeeProjectActivityObj.ProjectId = project.Id;
                                employeeProjectActivityObj.UserId = UserId;
                                employeeProjectActivityObj.Activity = "Unassign client";
                                var AddUpdateProjectActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                                await _hubContext.Clients.All.OnProjectModuleEvent(projectObj.Id, TenantId);
                            }
                            //for project ticket
                            if (projectObj.Id != null)
                            {
                                var mateProjectTicketList = _mateProjectTicketService.GetAllByProjectId(projectObj.Id.Value, TenantId);
                                if (mateProjectTicketList != null && mateProjectTicketList.Count > 0)
                                {
                                    foreach (var projectTicketObj in mateProjectTicketList)
                                    {
                                        var mateProjectTicketObj = await _mateProjectTicketService.DeleteById(projectTicketObj.Id);
                                        if (mateProjectTicketObj != null)
                                        {
                                            MateTicketActivity projectTicketActivityObj = new MateTicketActivity();
                                            projectTicketActivityObj.MateTicketId = mateProjectTicketObj.MateTicketId;
                                            projectTicketActivityObj.EmployeeProjectId = projectTicketObj.EmployeeProjectId;
                                            projectTicketActivityObj.CreatedBy = UserId;
                                            projectTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Unassign_project_from_ticket.ToString().Replace("_", " ");
                                            var AddUpdateProjectTicket = await _mateTicketActivityService.CheckInsertOrUpdate(projectTicketActivityObj);
                                            await _hubContext.Clients.All.OnMateTicketModuleEvent(mateProjectTicketObj.MateTicketId, TenantId);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                //employee task                
                if (employeeTaskList != null && employeeTaskList.Count > 0)
                {
                    foreach (var task in employeeTaskList)
                    {
                        if (task != null)
                        {
                            // var taskObj = _mapper.Map<EmployeeTaskDto>(task);
                            // taskObj.ClientId = null;
                            // var employeeTaskObj = _employeeTaskService.CheckInsertOrUpdate(taskObj);
                            if (task.EmployeeTaskId != null)
                            {
                                var employeeClientTaskObj = await _employeeClientTaskService.DeleteByTaskId(task.EmployeeTaskId.Value);
                                if (employeeClientTaskObj != null)
                                {
                                    EmployeeTaskActivity ClientTaskActivityObj = new EmployeeTaskActivity();
                                    ClientTaskActivityObj.EmployeeTaskId = task.Id;
                                    ClientTaskActivityObj.UserId = UserId;
                                    ClientTaskActivityObj.Activity = "Unassign client";
                                    var AddUpdateClientTaskActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(ClientTaskActivityObj);
                                    await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(task.Id, TenantId);
                                }

                                //Ticket task                               
                                var mateTicketTaskObj = await _mateTicketTaskService.DeleteByTaskId(task.EmployeeTaskId.Value);
                                if (mateTicketTaskObj != null)
                                {
                                    MateTicketActivity mateTicketTaskActivityObj = new MateTicketActivity();
                                    mateTicketTaskActivityObj.EmployeeTaskId = task.EmployeeTaskId;
                                    mateTicketTaskActivityObj.MateTicketId = mateTicketTaskObj.MateTicketId;
                                    mateTicketTaskActivityObj.CreatedBy = UserId;
                                    mateTicketTaskActivityObj.Activity = Enums.MateTicketActivityEnum.Task_removed_from_ticket.ToString().Replace("_", " ");
                                    var AddUpdateTicketTaskActivity = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketTaskActivityObj);
                                    await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketTaskObj.MateTicketId, TenantId);
                                }

                                // var taskToDelete = await _employeeTaskService.Delete(task.EmployeeTaskId.Value);

                                // EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                                // employeeTaskActivityObj.EmployeeTaskId = task.EmployeeTaskId.Value;
                                // employeeTaskActivityObj.UserId = UserId;
                                // employeeTaskActivityObj.Activity = "Removed the task";
                                // var AddUpdate = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                            }
                        }
                    }
                }

                //serviceArticlePrice for client service article
                var serviceArticlePriceList = await _serviceArticlePriceService.DeleteByClientId(Id);

                //Contract
                var contractList = _contractService.GetByClient(Id);
                if (contractList != null && contractList.Count > 0)
                {
                    foreach (var contractItem in contractList)
                    {
                        if (contractItem != null)
                        {
                            var contractArticleList = await _contractArticleService.DeleteByContract(contractItem.Id);
                            if (contractArticleList != null)
                            {
                                ContractActivity contractSubscriptionActivityObj = new ContractActivity();
                                contractSubscriptionActivityObj.ContractId = contractItem.Id;
                                contractSubscriptionActivityObj.ClientId = contractItem.ClientId;
                                contractSubscriptionActivityObj.Activity = Enums.ContractActivityEnum.Contract_article_removed.ToString().Replace("_", " ");
                                var AddUpdateActivity = await _contractActivityService.CheckInsertOrUpdate(contractSubscriptionActivityObj);
                            }

                            //contract invoice
                            var contractInvoiceList = _contractInvoiceService.GetAllByContract(contractItem.Id);
                            if (contractInvoiceList != null && contractInvoiceList.Count > 0)
                            {
                                foreach (var contractInvoice in contractInvoiceList)
                                {
                                    if (contractInvoice != null && contractInvoice.ClientInvoiceId != null)
                                    {
                                        var contractInvoiceObj = await _contractInvoiceService.DeleteContractInvoice(contractInvoice.Id);
                                        if (contractInvoiceObj != null)
                                        {
                                            ContractActivity contractInvoiceActivityObj = new ContractActivity();
                                            contractInvoiceActivityObj.ContractId = contractInvoice.ContractId;
                                            contractInvoiceActivityObj.ClientId = contractItem.ClientId;
                                            contractInvoiceActivityObj.Activity = Enums.ContractActivityEnum.Contract_invoice_removed.ToString().Replace("_", " ");
                                            var AddUpdateActivity = await _contractActivityService.CheckInsertOrUpdate(contractInvoiceActivityObj);
                                        }
                                        var clientInvoiceObj = await _clientInvoiceService.DeleteClientInvoice(contractInvoice.ClientInvoiceId.Value);
                                    }
                                }
                            }
                            //contract invoice

                            //project Contract
                            var projectContractList = await _projectContractService.DeleteByContract(contractItem.Id);
                            if (projectContractList != null && projectContractList.Count > 0)
                            {
                                //contract activity
                                ContractActivity projectContractActivityObj = new ContractActivity();
                                projectContractActivityObj.ContractId = Id;
                                projectContractActivityObj.ClientId = contractItem.ClientId;
                                projectContractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_removed_from_contract.ToString().Replace("_", " ");
                                var projectContractActivity = await _contractActivityService.CheckInsertOrUpdate(projectContractActivityObj);
                            }
                            //project Contract

                            var removedcontractObj = await _contractService.DeleteContract(contractItem.Id, UserId);
                            if (removedcontractObj != null)
                            {
                                ContractActivity contractActivityObj = new ContractActivity();
                                contractActivityObj.ContractId = contractItem.Id;
                                contractActivityObj.ClientId = removedcontractObj.ClientId;
                                contractActivityObj.Activity = Enums.ContractActivityEnum.Contract_removed.ToString().Replace("_", " ");
                                var AddUpdateContractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);
                            }
                        }
                    }
                }

                //client Invoice Mollie Customer
                var invoiceMollieCustomerList = await _invoiceMollieCustomerService.DeleteByClientId(Id);

                //Client Invoice                
                if (clientInvoiceList != null && clientInvoiceList.Count > 0)
                {
                    foreach (var clientInvoice in clientInvoiceList)
                    {
                        if (clientInvoice != null)
                        {
                            var invoiceMollieSubscriptionObj = await _invoiceMollieSubscriptionService.DeleteByInvoiceId(clientInvoice.Id);
                            var clientInvoiceObj = await _clientInvoiceService.DeleteClientInvoice(clientInvoice.Id);
                        }
                    }
                }

                var clientObj = await _clientService.DeleteClient(Id);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> Logo(int Id)
        {
            var clientObj = _clientService.GetById(Id);
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientLogoDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (clientObj != null && !string.IsNullOrEmpty(clientObj.Logo))
            {
                filePath = dirPath + "\\" + clientObj.Logo;
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    return File(bytes, Common.GetMimeTypes(clientObj.Logo), clientObj.Logo);
                }
            }
            return null;
        }
        //for client drop down
        [SwaggerOperation(Description = "Use this api for getting client list for drop down")]
        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<ClientDropdownListResponse>>> DropdownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<ClientDropdownListResponse> clientDropdownListResponseList = new List<ClientDropdownListResponse>();
            var clientList = _clientService.GetByTenant(TenantId);
            if (clientList != null)
            {
                if (clientList != null && clientList.Count > 0)
                {
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientLogoDirPath;
                    foreach (var item in clientList)
                    {
                        ClientDropdownListResponse clientDropdownListResponseObj = new ClientDropdownListResponse();
                        clientDropdownListResponseObj.Id = item.Id;
                        clientDropdownListResponseObj.Name = item.FirstName + "" + item.LastName;
                        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        if (item.Logo != null)
                        {
                            clientDropdownListResponseObj.LogoURL = OneClappContext.CurrentURL + "Client/Logo/" + item.Id + "?" + Timestamp;
                        }
                        else
                        {
                            clientDropdownListResponseObj.LogoURL = null;
                        }
                        clientDropdownListResponseList.Add(clientDropdownListResponseObj);
                    }
                }
            }

            return new OperationResult<List<ClientDropdownListResponse>>(true, System.Net.HttpStatusCode.OK, "", clientDropdownListResponseList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        [SwaggerOperation(Description = "This api is use for CRM client info screen")]
        //to do ma only consider contract's project,task,subtask,child task but client project,task,sub task ,child task remaining
        public async Task<OperationResult<ClientInfoResponse>> Info(long Id)
        {
            ClientInfoResponse clientInfoResponseObj = new ClientInfoResponse();
            var clientObj = _clientService.GetById(Id);
            var clientEmailObj = _clientEmailService.GetByClientIdWithPrimary(Id);
            var clientPhoneObj = _clientPhoneService.GetByClientIdWithPrimary(Id);
            var clientSocialMediaList = _clientSocialMediaService.GetByClientId(Id);
            var clientUserObj = _clientUserService.GetByClientId(Id).Where(t => t.ClientUserRole != null && t.ClientUserRole.Name == OneClappContext.ClientUserRootRole).FirstOrDefault();
            var Tasks = _employeeClientTaskService.GetTaskByClient(Id).Where(t => t.EmployeeTask?.EndDate >= DateTime.Today).ToList();
            var Projects = _employeeProjectService.GetAllByClientId(Id).Where(t => t.EndDate >= DateTime.Today).ToList();
            var Contracts = _contractService.GetByClient(Id).Where(t => t.EndDate >= DateTime.Today && t.CancelDate == null).ToList();

            clientInfoResponseObj = _mapper.Map<ClientInfoResponse>(clientObj);
            if (clientObj != null)
            {
                clientInfoResponseObj.Id = clientObj.Id;
                clientInfoResponseObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (clientObj.Logo != null)
                {
                    clientInfoResponseObj.LogoURL = OneClappContext.CurrentURL + "Client/Logo/" + clientInfoResponseObj.Id + "?" + Timestamp;
                }
                else
                {
                    clientInfoResponseObj.LogoURL = null;
                }
                if (clientObj.SiteAddressLine1 != null || clientObj.SiteAddressLine2 != null || clientObj.SiteAddressLine3 != null)
                {
                    clientInfoResponseObj.Address = clientObj.SiteAddressLine1 + "," + clientObj.SiteAddressLine2 + "," + clientObj.SiteAddressLine3;
                }
                else
                {
                    clientInfoResponseObj.Address = "NA";
                }
                clientInfoResponseObj.SiteName = clientObj.SiteName;
            }
            clientInfoResponseObj.ClientUserName = clientUserObj == null ? "NA" : clientUserObj.FirstName + " " + clientUserObj.LastName;
            clientInfoResponseObj.Email = clientEmailObj != null ? clientEmailObj.Email : "NA";
            clientInfoResponseObj.PhoneNo = clientPhoneObj != null ? clientPhoneObj.PhoneNo : "NA";
            //social media
            if (clientSocialMediaList != null && clientSocialMediaList.Count > 0)
            {
                List<ClientDetailSocialMediaResponse> socialMediaList = new List<ClientDetailSocialMediaResponse>();
                foreach (var item in clientSocialMediaList)
                {
                    ClientDetailSocialMediaResponse socialMediaObj = new ClientDetailSocialMediaResponse();
                    socialMediaObj.Id = item.Id;
                    socialMediaObj.Name = item.SocialMedia.Name;
                    socialMediaObj.URL = item.URL;
                    socialMediaList.Add(socialMediaObj);
                }
                clientInfoResponseObj.SocialMedias = socialMediaList;
            }
            clientInfoResponseObj.TaskCount = Tasks.Count();
            clientInfoResponseObj.ProjectCount = Projects.Count();
            clientInfoResponseObj.ContractCount = Contracts.Count();

            //service contingent
            var contracts = _contractService.GetByClient(Id);
            var contractsWithFixedType = contracts.Where(t => t.ContractTypeId != null && t.ContractType.Name.ToLower() == "fixed").ToList();
            var contractsWithOutFixedType = contracts.Where(t => t.ContractTypeId != null && t.ContractType.Name.ToLower() != "fixed").ToList();
            if (contractsWithFixedType != null && contractsWithFixedType.Count > 0)
            {
                var contractFixed = contractsWithFixedType.Sum(t => t.AllowedHours);
                if (contractFixed != null)
                {
                    clientInfoResponseObj.Fixed = contractFixed.Value;
                }
                long? ConsumedFixDuration = 0;
                foreach (var Obj in contractsWithFixedType)
                {
                    DateTime? StartDate = null;
                    DateTime? EndDate = null;

                    StartDate = Obj.StartDate.Value;
                    EndDate = DateTime.Today.Date;
                    long? FixDuration = 0;
                    long? ProjectFixDuration = 0;
                    long? TaskFixDuration = 0;
                    long? SubTaskFixDuration = 0;
                    long? ChildTaskFixDuration = 0;
                    var projectContractList = _projectContractService.GetByContractId(Obj.Id); //get project contract list by contract
                    var ProjectIdListWithContract = projectContractList.Select(t => t.ProjectId.Value).ToList();
                    foreach (var projectObj in ProjectIdListWithContract)
                    {
                        var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj, StartDate.Value, EndDate.Value)
                                                                                            .Where(t => t.MateTimeRecord != null).ToList();
                        ProjectFixDuration = ProjectFixDuration + mateProjectTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    var employeeTaskIdListWithContract = _employeeProjectTaskService.GetAllByProjectIdList(ProjectIdListWithContract).Select(t => t.Id).ToList();
                    foreach (var taskObj in employeeTaskIdListWithContract)
                    {
                        var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj, StartDate.Value, EndDate.Value)
                                                                                            .Where(t => t.MateTimeRecord != null).ToList();
                        TaskFixDuration = TaskFixDuration + mateTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    var SubTaskIdListWithContract = _employeeSubTaskService.GetAllActiveByTaskIds(employeeTaskIdListWithContract).Select(t => t.Id).ToList();
                    foreach (var subTaskObj in SubTaskIdListWithContract)
                    {
                        var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTask(subTaskObj, StartDate.Value, EndDate.Value)
                                                       .Where(t => t.MateTimeRecord != null).ToList();
                        SubTaskFixDuration = SubTaskFixDuration + mateSubTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    var ChildTaskIdListWithContract = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIdListWithContract).Select(t => t.Id).ToList();
                    foreach (var childTask in ChildTaskIdListWithContract)
                    {
                        var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTask(childTask, StartDate.Value, EndDate.Value)
                                                      .Where(t => t.MateTimeRecord != null).ToList();
                        ChildTaskFixDuration = ChildTaskFixDuration + mateChildTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    FixDuration = ProjectFixDuration + TaskFixDuration + SubTaskFixDuration + ChildTaskFixDuration;
                    ConsumedFixDuration = ConsumedFixDuration + FixDuration;
                }
                clientInfoResponseObj.ConsumedFixed = ConsumedFixDuration.Value;
            }

            if (contractsWithOutFixedType != null && contractsWithOutFixedType.Count > 0)
            {
                long? HourlyWithContractDuration = 0;
                foreach (var Obj in contractsWithOutFixedType)
                {
                    DateTime? StartDate = null;
                    DateTime? EndDate = null;

                    StartDate = Obj.StartDate.Value;
                    EndDate = DateTime.Today.Date;
                    long? HourlyDuration = 0;
                    long? ProjectHourlyDuration = 0;
                    long? TaskHourlyDuration = 0;
                    long? SubTaskHourlyDuration = 0;
                    long? ChildTaskHourlyDuration = 0;
                    var projectContractList = _projectContractService.GetByContractId(Obj.Id); //get project contract list by contract
                    var ProjectIdListWithContract1 = projectContractList.Select(t => t.ProjectId.Value).ToList();
                    foreach (var projectObj in ProjectIdListWithContract1)
                    {
                        var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj, StartDate.Value, EndDate.Value)
                                                                                            .Where(t => t.MateTimeRecord != null).ToList();
                        ProjectHourlyDuration = ProjectHourlyDuration + mateProjectTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    var employeeTaskIdListWithContract1 = _employeeProjectTaskService.GetAllByProjectIdList(ProjectIdListWithContract1).Select(t => t.Id).ToList();
                    foreach (var taskObj in employeeTaskIdListWithContract1)
                    {
                        var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj, StartDate.Value, EndDate.Value)
                                                                                            .Where(t => t.MateTimeRecord != null).ToList();
                        TaskHourlyDuration = TaskHourlyDuration + mateTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    var SubTaskIdListWithContract1 = _employeeSubTaskService.GetAllActiveByTaskIds(employeeTaskIdListWithContract1).Select(t => t.Id).ToList();
                    foreach (var subTaskObj in SubTaskIdListWithContract1)
                    {
                        var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTask(subTaskObj, StartDate.Value, EndDate.Value)
                                                       .Where(t => t.MateTimeRecord != null).ToList();
                        SubTaskHourlyDuration = SubTaskHourlyDuration + mateSubTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    var ChildTaskIdListWithContract1 = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIdListWithContract1).Select(t => t.Id).ToList();
                    foreach (var childTask in ChildTaskIdListWithContract1)
                    {
                        var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTask(childTask, StartDate.Value, EndDate.Value)
                                                      .Where(t => t.MateTimeRecord != null).ToList();
                        ChildTaskHourlyDuration = ChildTaskHourlyDuration + mateChildTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
                    }
                    HourlyDuration = ProjectHourlyDuration + TaskHourlyDuration + SubTaskHourlyDuration + ChildTaskHourlyDuration;
                    HourlyWithContractDuration = HourlyWithContractDuration + HourlyDuration;
                }
                clientInfoResponseObj.ConsumedHourly = HourlyWithContractDuration.Value;
            }

            return new OperationResult<ClientInfoResponse>(true, System.Net.HttpStatusCode.OK, "", clientInfoResponseObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        [SwaggerOperation(Description = "This api is use for CRM client detail when click on edit button")]
        public async Task<OperationResult<ClientDetailResponse>> Detail(long Id)
        {
            ClientDetailResponse clientDetailResponseObj = new ClientDetailResponse();
            var clientObj = _clientService.GetById(Id);
            var clientEmailList = _clientEmailService.GetByClientId(Id);
            var clientPhoneList = _clientPhoneService.GetByClientId(Id);
            clientDetailResponseObj = _mapper.Map<ClientDetailResponse>(clientObj);
            if (clientDetailResponseObj.CountryId != null)
            {
                clientDetailResponseObj.CountryName = clientObj.Country.Name;
            }
            if (clientDetailResponseObj.StateId != null)
            {
                clientDetailResponseObj.StateName = clientObj.State.Name;
            }
            if (clientDetailResponseObj.CityId != null)
            {
                clientDetailResponseObj.CityName = clientObj.City.Name;
            }
            if (clientDetailResponseObj.TimeZoneId != null)
            {
                clientDetailResponseObj.Time = clientObj.StandardTimeZone.Time;
            }
            if (clientDetailResponseObj.InvoiceIntervalId != null)
            {
                clientDetailResponseObj.InvoiceInterval = clientObj.InvoiceInterval.Name;
                clientDetailResponseObj.Interval = clientObj.InvoiceInterval.Interval;
            }

            if (clientEmailList != null && clientEmailList.Count > 0)
            {
                List<ClientEmailDetailResponse> emailList = new List<ClientEmailDetailResponse>();
                foreach (var emailItem in clientEmailList)
                {
                    ClientEmailDetailResponse emailObj = new ClientEmailDetailResponse();
                    emailObj.Id = emailItem.Id;
                    emailObj.Email = emailItem.Email;
                    emailObj.EmailTypeId = emailItem.EmailTypeId;
                    emailObj.IsPrimary = emailItem.IsPrimary;
                    emailList.Add(emailObj);
                }
                clientDetailResponseObj.Emails = emailList;
            }

            if (clientPhoneList != null && clientPhoneList.Count > 0)
            {
                List<ClientPhoneDetailResponse> phoneList = new List<ClientPhoneDetailResponse>();
                foreach (var phoneItem in clientPhoneList)
                {
                    ClientPhoneDetailResponse phoneObj = new ClientPhoneDetailResponse();
                    phoneObj.Id = phoneItem.Id;
                    phoneObj.PhoneNo = phoneItem.PhoneNo;
                    phoneObj.PhoneNoTypeId = phoneItem.PhoneNoTypeId;
                    phoneObj.IsPrimary = phoneItem.IsPrimary;
                    phoneList.Add(phoneObj);
                }
                clientDetailResponseObj.Phones = phoneList;
            }
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (clientObj.Logo != null)
            {
                clientDetailResponseObj.Logo = clientObj.Logo;
                clientDetailResponseObj.LogoURL = OneClappContext.CurrentURL + "Client/Logo/" + clientDetailResponseObj.Id + "?" + Timestamp;
            }
            else
            {
                clientDetailResponseObj.Logo = null;
                clientDetailResponseObj.LogoURL = null;
            }
            return new OperationResult<ClientDetailResponse>(true, System.Net.HttpStatusCode.OK, "", clientDetailResponseObj);
        }

        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        // [HttpDelete("{Id}")]
        // public async Task<OperationResult> RemoveEmail(long Id)
        // {
        //     if (Id != null)
        //     {
        //         var clientEmailObj = await _clientEmailService.DeleteById(Id);
        //         if (clientEmailObj != null)
        //         {
        //             return new OperationResult(true, System.Net.HttpStatusCode.OK, "Removed client email", Id);
        //         }
        //         return new OperationResult(false, System.Net.HttpStatusCode.OK, "Error", Id);
        //     }
        //     else
        //     {
        //         return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
        //     }
        // }

        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        // [HttpDelete("{Id}")]
        // public async Task<OperationResult> RemovePhone(long Id)
        // {
        //     if (Id != null)
        //     {
        //         var clientPhoneObj = await _clientPhoneService.DeleteById(Id);
        //         if (clientPhoneObj != null)
        //         {
        //             return new OperationResult(true, System.Net.HttpStatusCode.OK, "Removed client phone", Id);
        //         }
        //         return new OperationResult(false, System.Net.HttpStatusCode.OK, "Error", Id);
        //     }
        //     else
        //     {
        //         return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
        //     }
        // }

        #region client conversation
        [SwaggerOperation(Description = "This api is use for CRM conversation screen to fetch emails which same domain name of client sitename")]
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<ClientConversationEmailListResponse>> ThreadList([FromBody] ClientConversationEmailListRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            ClientConversationEmailListResponse responseObj = new ClientConversationEmailListResponse();
            responseObj.InboxThread = new List<InboxThread>();
            var intProviderAppSecretList = _intProviderAppSecretService.GetAllByUser(UserId);
            //var intProviderAppSecretDtoList = _mapper.Map<List<IntProviderAppSecretDto>>(intProviderAppSecretList);
            if (intProviderAppSecretList != null && intProviderAppSecretList.Count > 0)
            {
                int Allcount = 0;
                int AllUnreadcount = 0;
                string clientdomain = "";
                var clientObj = _clientService.GetById(requestmodel.ClientId);
                if (clientObj != null && clientObj.SiteName != null)
                {
                    if (IsValidUrl(clientObj.SiteName))
                    {
                        var host = new System.Uri(clientObj.SiteName).Host;
                        clientdomain = host.Substring(host.LastIndexOf('.', host.LastIndexOf('.') - 1) + 1);
                    }
                    else
                    {
                        return new OperationResult<ClientConversationEmailListResponse>(false, System.Net.HttpStatusCode.OK, "Invalid site name");
                    }
                }
                var clientUserList = _clientUserService.GetByClientId(requestmodel.ClientId);
                foreach (var secretItem in intProviderAppSecretList)
                {
                    InboxVM inboxVMObj = new InboxVM();
                    inboxVMObj.Provider = secretItem.IntProviderApp?.IntProvider?.Name;
                    inboxVMObj.ProviderApp = secretItem.IntProviderApp?.Name;
                    inboxVMObj.SelectedEmail = secretItem.Email;
                    inboxVMObj.UserId = UserId;
                    if (requestmodel.top != null)
                    {
                        inboxVMObj.Top = requestmodel.top.Value;
                    }
                    else
                    {
                        inboxVMObj.Top = 20;
                    }
                    if (requestmodel.skip != null)
                    {
                        inboxVMObj.Skip = requestmodel.skip.Value;
                    }
                    else
                    {
                        inboxVMObj.Skip = 0;
                    }
                    inboxVMObj.Label = requestmodel.Label;
                    inboxVMObj.NextPageToken = requestmodel.nextPageToken;
                    var result = await mailInbox.GetThread(UserId, inboxVMObj);
                    responseObj.NextPageToken = result.NextPageToken;

                    if (result.InboxThread != null && result.InboxThread.Count > 0 && !string.IsNullOrEmpty(clientdomain))
                    {
                        int responseAllCount = 0;
                        int responseUnReadCount = 0;
                        //var SameDomainEmailList = clientUserList.Where(t => t.WorkEmail != null && t.WorkEmail.Contains(clientdomain)).Select(t => t.WorkEmail).ToList();
                        //result.InboxThread = result.InboxThread.Where(t => t.FromEmail != null && SameDomainEmailList.Contains(t.FromEmail)).ToList();
                        result.InboxThread = result.InboxThread.Where(t => t.FromEmail != null && t.FromEmail.Contains(clientdomain)).ToList();
                        responseAllCount = result.InboxThread.Count();
                        Allcount = Allcount + responseAllCount;
                        responseUnReadCount = result.InboxThread.Where(t => !t.IsRead).ToList().Count();
                        AllUnreadcount = AllUnreadcount + responseUnReadCount;
                        responseObj.InboxThread.AddRange(result.InboxThread);
                    }
                }
                responseObj.Totalcount = Allcount;
                responseObj.Unreadcount = AllUnreadcount;
            }
            return new OperationResult<ClientConversationEmailListResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }

        public static bool IsValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        [SwaggerOperation(Description = "This api is use for CRM conversation screen to Sendemail.this is only use for client conversation screen")]
        [HttpPost]
        public async Task<OperationResult<SendEmailResponse>> SendEmail([FromForm] SendEmailRequest composeMessage)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<ComposeEmail1>(composeMessage);
            if (UserId > 0)
            {
                if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
                {
                    foreach (IFormFile file in requestmodel.FileList)
                    {
                        if (OneClappContext.ClamAVServerIsLive)
                        {
                            ScanDocument scanDocumentObj = new ScanDocument();
                            bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                            if (fileStatus)
                            {
                                return new OperationResult<SendEmailResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                            }
                        }
                    }
                }
                // var result = await mailInbox.SendEmail(composeMessage.UserId, composeMessage, file);
                var result = await mailInbox.SendEmail(UserId, requestmodel, requestmodel.FileList);
                var threadId = "";
                var responseresult = _mapper.Map<SendEmailResponse>(result);

                if (result != null)
                {
                    threadId = result.threadId;
                }
                await _hubContext.Clients.All.OnMailModuleEvent(UserId, "mail", "addupdate", threadId);
                return new OperationResult<SendEmailResponse>(result.IsValid, System.Net.HttpStatusCode.OK, result.ErrorMessage, responseresult);
            }
            else
                return new OperationResult<SendEmailResponse>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }

        [HttpPost("{ThreadId}")]
        public async Task<OperationResult<List<ThreadByThreadIdResponse>>> ThreadDetail(string ThreadId, [FromBody] ThreadByThreadIdRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var CreatedBy = UserId;
            //InboxVM inboxvmmodel = new InboxVM();
            var inboxvmmodel = _mapper.Map<InboxVM>(model);
            // if (!string.IsNullOrEmpty(inboxvmmodel.EventType))
            // {
            //     if (inboxvmmodel.EventType == "assignuser")
            //     {
            //         var data = _mailAssignUserService.GetByUserThread(UserId, ThreadId);
            //         if (data != null && data.IntProviderAppSecret != null)
            //         {
            //             if (data.IntProviderAppSecret.CreatedBy != null)
            //             {
            //                 CreatedBy = data.IntProviderAppSecret.CreatedBy.Value;
            //             }
            //             inboxvmmodel.SelectedEmail = data.IntProviderAppSecret.Email;
            //             if (data.IntProviderAppSecret.IntProviderApp != null)
            //             {
            //                 inboxvmmodel.ProviderApp = data.IntProviderAppSecret.IntProviderApp.Name;
            //                 if (data.IntProviderAppSecret.IntProviderApp.IntProvider != null)
            //                 {
            //                     inboxvmmodel.Provider = data.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
            //                 }
            //             }
            //             else
            //             {
            //                 inboxvmmodel.Provider = null;
            //                 inboxvmmodel.ProviderApp = null;
            //             }

            //         }
            //     }
            //     else if (inboxvmmodel.EventType == "shareduser")
            //     {
            //         var data = _mailParticipantService.GetByUserThread(UserId, ThreadId);
            //         if (data != null && data.IntProviderAppSecret != null)
            //         {
            //             if (data.IntProviderAppSecret.CreatedBy != null)
            //             {
            //                 CreatedBy = data.IntProviderAppSecret.CreatedBy.Value;
            //             }
            //             inboxvmmodel.SelectedEmail = data.IntProviderAppSecret.Email;
            //             if (data.IntProviderAppSecret.IntProviderApp != null)
            //             {
            //                 inboxvmmodel.ProviderApp = data.IntProviderAppSecret.IntProviderApp.Name;
            //                 if (data.IntProviderAppSecret.IntProviderApp.IntProvider != null)
            //                 {
            //                     inboxvmmodel.Provider = data.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
            //                 }
            //             }
            //             else
            //             {
            //                 inboxvmmodel.Provider = null;
            //                 inboxvmmodel.ProviderApp = null;
            //             }
            //         }
            //     }
            // }
            // if (inboxvmmodel.TeamInboxId != null)
            // {
            //     var teamInboxObj = _teamInboxService.GetById(inboxvmmodel.TeamInboxId.Value);
            //     if (teamInboxObj != null)
            //     {
            //         if (teamInboxObj.CreatedBy != null)
            //         {
            //             CreatedBy = teamInboxObj.CreatedBy.Value;
            //         }
            //         if (teamInboxObj.IntProviderAppSecret != null)
            //         {
            //             inboxvmmodel.SelectedEmail = teamInboxObj.IntProviderAppSecret.Email;
            //             if (teamInboxObj.IntProviderAppSecret.IntProviderApp != null)
            //             {
            //                 inboxvmmodel.ProviderApp = teamInboxObj.IntProviderAppSecret.IntProviderApp.Name;
            //                 if (teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider != null)
            //                 {
            //                     inboxvmmodel.Provider = teamInboxObj.IntProviderAppSecret.IntProviderApp.IntProvider.Name;
            //                 }
            //             }
            //             else
            //             {
            //                 inboxvmmodel.Provider = null;
            //                 inboxvmmodel.ProviderApp = null;
            //             }
            //         }
            //     }
            // }
            // userId = 6;
            if (UserId > 0)
            {
                InboxVM inboxVMObj = new InboxVM();
                inboxVMObj.UserId = CreatedBy;
                inboxVMObj.Provider = inboxvmmodel.Provider;
                inboxVMObj.Label = inboxvmmodel.Label;
                inboxVMObj.ProviderApp = inboxvmmodel.ProviderApp;
                inboxVMObj.SelectedEmail = inboxvmmodel.SelectedEmail;

                // inboxVM.Skip = model.Skip;
                if (inboxvmmodel.Skip != null)
                {
                    inboxVMObj.Skip = inboxvmmodel.Skip;
                }
                else
                {
                    inboxVMObj.Skip = 0;
                }
                if (inboxvmmodel.Top != null)
                {
                    inboxVMObj.Top = inboxvmmodel.Top;
                }
                else
                {
                    inboxVMObj.Top = 20;
                }

                var result = await mailInbox.GetThreadByThreadId(CreatedBy, ThreadId, inboxVMObj);

                result = result.OrderByDescending(t => t.CreatedOn).ToList();

                var threadresponse = _mapper.Map<List<ThreadByThreadIdResponse>>(result);
                return new OperationResult<List<ThreadByThreadIdResponse>>(true, System.Net.HttpStatusCode.OK, "", threadresponse);

            }
            else
                return new OperationResult<List<ThreadByThreadIdResponse>>(false, System.Net.HttpStatusCode.OK, CommonMessage.UnAuthorizedUser);
        }
        #endregion


        // var contracts = _contractService.GetByClient(Id);
        // var contractsWithFixedType = contracts.Where(t => t.ContractTypeId != null && t.ContractType.Name.ToLower() == "fixed").ToList();
        // var contractsWithOutFixedType = contracts.Where(t => t.ContractTypeId != null && t.ContractType.Name.ToLower() != "fixed").ToList();
        // if (contractsWithFixedType != null && contractsWithFixedType.Count > 0)
        // {
        //     var contractFixed = contractsWithFixedType.Sum(t => t.AllowedHours);
        //     if (contractFixed != null)
        //     {
        //         clientInfoResponseObj.Fixed = contractFixed.Value;
        //     }
        //     long? ConsumedFixDuration = 0;
        //     foreach (var Obj in contractsWithFixedType)
        //     {
        //         DateTime? StartDate = null;
        //         DateTime? EndDate = null;

        //         StartDate = Obj.StartDate.Value;
        //         EndDate = DateTime.Today.Date;
        //         long? FixDuration = 0;
        //         long? ProjectFixDuration = 0;
        //         long? TaskFixDuration = 0;
        //         long? SubTaskFixDuration = 0;
        //         long? ChildTaskFixDuration = 0;
        //         var projectContractList = _projectContractService.GetByContractId(Obj.Id); //get project contract list by contract
        //         var ProjectIdListWithContract = projectContractList.Select(t => t.ProjectId.Value).ToList();
        //         foreach (var projectObj in ProjectIdListWithContract)
        //         {
        //             var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj, StartDate.Value, EndDate.Value)
        //                                                                                 .Where(t => t.MateTimeRecord != null).ToList();
        //             ProjectFixDuration = ProjectFixDuration + mateProjectTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         var employeeTaskIdListWithContract = _employeeTaskService.GetAllByProjectIdList(ProjectIdListWithContract).Select(t => t.Id).ToList();
        //         foreach (var taskObj in employeeTaskIdListWithContract)
        //         {
        //             var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj, StartDate.Value, EndDate.Value)
        //                                                                                 .Where(t => t.MateTimeRecord != null).ToList();
        //             TaskFixDuration = TaskFixDuration + mateTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         var SubTaskIdListWithContract = _employeeSubTaskService.GetAllActiveByTaskIds(employeeTaskIdListWithContract).Select(t => t.Id).ToList();
        //         foreach (var subTaskObj in SubTaskIdListWithContract)
        //         {
        //             var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTask(subTaskObj, StartDate.Value, EndDate.Value)
        //                                            .Where(t => t.MateTimeRecord != null).ToList();
        //             SubTaskFixDuration = SubTaskFixDuration + mateSubTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         var ChildTaskIdListWithContract = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIdListWithContract).Select(t => t.Id).ToList();
        //         foreach (var childTask in ChildTaskIdListWithContract)
        //         {
        //             var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTask(childTask, StartDate.Value, EndDate.Value)
        //                                           .Where(t => t.MateTimeRecord != null).ToList();
        //             ChildTaskFixDuration = ChildTaskFixDuration + mateChildTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         FixDuration = ProjectFixDuration + TaskFixDuration + SubTaskFixDuration + ChildTaskFixDuration;
        //         ConsumedFixDuration = ConsumedFixDuration + FixDuration;
        //     }
        //     clientInfoResponseObj.ConsumedFixed = ConsumedFixDuration.Value;
        // }

        // long? HourlyWithContractDuration = 0;
        // if (contractsWithOutFixedType != null && contractsWithOutFixedType.Count > 0)
        // {
        //     foreach (var Obj in contractsWithOutFixedType)
        //     {
        //         DateTime? StartDate = null;
        //         DateTime? EndDate = null;

        //         StartDate = Obj.StartDate.Value;
        //         EndDate = DateTime.Today.Date;
        //         long? HourlyDuration = 0;
        //         long? ProjectHourlyDuration = 0;
        //         long? TaskHourlyDuration = 0;
        //         long? SubTaskHourlyDuration = 0;
        //         long? ChildTaskHourlyDuration = 0;
        //         var projectContractList = _projectContractService.GetByContractId(Obj.Id); //get project contract list by contract
        //         var ProjectIdListWithContract1 = projectContractList.Select(t => t.ProjectId.Value).ToList();
        //         foreach (var projectObj in ProjectIdListWithContract1)
        //         {
        //             var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj, StartDate.Value, EndDate.Value)
        //                                                                                 .Where(t => t.MateTimeRecord != null).ToList();
        //             ProjectHourlyDuration = ProjectHourlyDuration + mateProjectTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         var employeeTaskIdListWithContract1 = _employeeTaskService.GetAllByProjectIdList(ProjectIdListWithContract1).Select(t => t.Id).ToList();
        //         foreach (var taskObj in employeeTaskIdListWithContract1)
        //         {
        //             var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj, StartDate.Value, EndDate.Value)
        //                                                                                 .Where(t => t.MateTimeRecord != null).ToList();
        //             TaskHourlyDuration = TaskHourlyDuration + mateTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         var SubTaskIdListWithContract1 = _employeeSubTaskService.GetAllActiveByTaskIds(employeeTaskIdListWithContract1).Select(t => t.Id).ToList();
        //         foreach (var subTaskObj in SubTaskIdListWithContract1)
        //         {
        //             var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTask(subTaskObj, StartDate.Value, EndDate.Value)
        //                                            .Where(t => t.MateTimeRecord != null).ToList();
        //             SubTaskHourlyDuration = SubTaskHourlyDuration + mateSubTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         var ChildTaskIdListWithContract1 = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIdListWithContract1).Select(t => t.Id).ToList();
        //         foreach (var childTask in ChildTaskIdListWithContract1)
        //         {
        //             var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTask(childTask, StartDate.Value, EndDate.Value)
        //                                           .Where(t => t.MateTimeRecord != null).ToList();
        //             ChildTaskHourlyDuration = ChildTaskHourlyDuration + mateChildTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //         }
        //         HourlyDuration = ProjectHourlyDuration + TaskHourlyDuration + SubTaskHourlyDuration + ChildTaskHourlyDuration;
        //         HourlyWithContractDuration = HourlyWithContractDuration + HourlyDuration;
        //     }
        // }

        // var clientProjects = _employeeProjectService.GetAllByClientId(Id);
        // var AllProjectContractList = _projectContractService.GetAll();
        // var AllProjectContractIdList = AllProjectContractList.Select(t => t.Id).ToList();
        // long? Duration = 0;
        // long? ProjectDuration = 0;
        // long? TaskDuration = 0;
        // long? SubTaskDuration = 0;
        // long? ChildTaskDuration = 0;
        // var ProjectWithoutList = clientProjects.Where(t => !AllProjectContractIdList.Contains(t.Id)).ToList();
        // var ProjectWithoutIdList = ProjectWithoutList.Select(t => t.Id).ToList();
        // foreach (var projectObj in ProjectWithoutList)
        // {
        //     if (projectObj.StartDate != null)
        //     {
        //         var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetMateProjectTimeRecordByProject(projectObj.Id, projectObj.StartDate.Value, DateTime.Today.Date)
        //                                                                             .Where(t => t.MateTimeRecord != null).ToList();
        //         ProjectDuration = ProjectDuration + mateProjectTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //     }
        // }
        // var employeeTaskIdList = _employeeTaskService.GetAllByProjectIdList(ProjectWithoutIdList).ToList();
        // foreach (var taskObj in employeeTaskIdList)
        // {
        //     if (taskObj.StartDate != null)
        //     {
        //         var mateTaskTimeRecordList = _mateTaskTimeRecordService.GetMateTaskTimeRecordByTask(taskObj.Id, taskObj.StartDate.Value, DateTime.Today.Date)
        //                                                                             .Where(t => t.MateTimeRecord != null).ToList();
        //         TaskDuration = TaskDuration + mateTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //     }
        // }
        // var SubTaskIdList = _employeeSubTaskService.GetAllActiveByTaskIds(employeeTaskIdList.Select(t => t.Id).ToList()).ToList();
        // foreach (var subTaskObj in SubTaskIdList)
        // {
        //     if (subTaskObj.StartDate != null)
        //     {
        //         var mateSubTaskTimeRecordList = _mateSubTaskTimeRecordService.GetBySubTask(subTaskObj.Id, subTaskObj.StartDate.Value, DateTime.Today.Date)
        //                                        .Where(t => t.MateTimeRecord != null).ToList();
        //         SubTaskDuration = SubTaskDuration + mateSubTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //     }
        // }
        // var ChildTaskIdList = _employeeChildTaskService.GetAllActiveBySubTaskIds(SubTaskIdList.Select(t => t.Id).ToList()).ToList();
        // foreach (var childTask in ChildTaskIdList)
        // {
        //     if (childTask.StartDate != null)
        //     {
        //         var mateChildTaskTimeRecordList = _mateChildTaskTimeRecordService.GetByChildTask(childTask.Id, childTask.StartDate.Value, DateTime.Today.Date)
        //                                       .Where(t => t.MateTimeRecord != null).ToList();
        //         ChildTaskDuration = ChildTaskDuration + mateChildTaskTimeRecordList.Sum(t => t.MateTimeRecord.Duration);
        //     }
        // }
        // Duration = ProjectDuration + TaskDuration + SubTaskDuration + ChildTaskDuration;
        // HourlyWithContractDuration = HourlyWithContractDuration + Duration;
        // clientInfoResponseObj.ConsumedHourly = HourlyWithContractDuration.Value;
    }
}