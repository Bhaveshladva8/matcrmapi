using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using matcrm.service.Services;
using AutoMapper;
using matcrm.service.Common;
using matcrm.data.Models.Response;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.service.Services.ERP;
using matcrm.data.Models.ViewModels;
using matcrm.service.Utility;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using matcrm.api.SignalR;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ClientUserController : Controller
    {
        private readonly IClientUserService _clientUserService;
        private readonly IIntProviderService _intProviderService;
        private readonly IIntProviderAppService _intProviderAppService;
        //private readonly IClientIntProviderAppSecretService _clientIntProviderAppSecretService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly IContactService _contactService;
        private readonly IUserService _userSerVice;
        private readonly IIntProviderContactService _intProviderContactService;
        private readonly IClientUserRoleService _clientUserRoleService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IClientService _clientService;
        private readonly ISalutationService _salutationService;
        private readonly IDepartmentService _departmentService;
        private readonly IClientAppointmentService _clientAppointmentService;
        private readonly ICRMNotesService _cRMNotesService;
        private readonly IClientEmailService _clientEmailService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IMapper _mapper;
        private Contact contact;
        private Common Common;
        private int UserId = 0;
        private int TenantId = 0;
        private string MicroSoftClientId;
        public ClientUserController(IClientUserService clientUserService,
        IIntProviderService intProviderService,
        IIntProviderAppService intProviderAppService,
        //IClientIntProviderAppSecretService clientIntProviderAppSecretService,
        IIntProviderAppSecretService intProviderAppSecretService,
        IContactService contactService,
        IUserService userService,
        IIntProviderContactService intProviderContactService,
        IClientUserRoleService clientUserRoleService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IClientService clientService,
        ISalutationService salutationService,
        IDepartmentService departmentService,
        IClientAppointmentService clientAppointmentService,
        ICRMNotesService cRMNotesService,
        IClientEmailService clientEmailService,
        IHostingEnvironment hostingEnvironment,
        IMapper mapper)
        {
            _clientUserService = clientUserService;
            _contactService = contactService;
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            //_clientIntProviderAppSecretService = clientIntProviderAppSecretService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _mapper = mapper;
            _userSerVice = userService;
            _intProviderContactService = intProviderContactService;
            _clientUserRoleService = clientUserRoleService;
            _clientService = clientService;
            _salutationService = salutationService;
            _departmentService = departmentService;
            _cRMNotesService = cRMNotesService;
            _clientAppointmentService = clientAppointmentService;
            _clientEmailService = clientEmailService;
            MicroSoftClientId = OneClappContext.MicroSoftClientId;
            _hubContext = hubContext;
            _hostingEnvironment = hostingEnvironment;
            contact = new Contact(intProviderService, intProviderAppService, intProviderAppSecretService, userService);
            Common = new Common();
        }

        [HttpPost]
        public async Task<OperationResult<ClientUserAddUpdateResponse>> Add([FromForm] ClientUserAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ClientUser>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var filePath = "";
            if (requestmodel.FileName != null)
            {
                model.Logo = requestmodel.FileName;
            }
            if (requestmodel.File != null)
            {
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientUserLogoDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestmodel.File.FileName),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestmodel.File.FileName)
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
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestmodel.File);
                    if (fileStatus)
                    {
                        return new OperationResult<ClientUserAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await requestmodel.File.CopyToAsync(oStream);
                }
            }
            var clientUserObj = await _clientUserService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ClientUserAddUpdateResponse>(clientUserObj);
            if (clientUserObj != null)
            {
                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (clientUserObj.Logo != null)
                {
                    responseObj.LogoURL = OneClappContext.CurrentURL + "ClientUser/Logo/" + responseObj.Id + "?" + Timestamp;
                }
                else
                {
                    responseObj.LogoURL = null;
                }
                await _hubContext.Clients.All.OnClientUserEventEmit(clientUserObj.ClientId);
            }
            return new OperationResult<ClientUserAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responseObj);
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> Logo(int Id)
        {
            var clientUserObj = _clientUserService.GetById(Id);
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientUserLogoDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (clientUserObj != null && !string.IsNullOrEmpty(clientUserObj.Logo))
            {
                filePath = dirPath + "\\" + clientUserObj.Logo;
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    return File(bytes, Common.GetMimeTypes(clientUserObj.Logo), clientUserObj.Logo);
                }
            }
            return null;
        }

        [HttpPut]
        public async Task<OperationResult<ClientUserAddUpdateResponse>> Update([FromForm] ClientUserAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ClientUser>(requestmodel);
            if (model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            var filePath = "";
            if (requestmodel.FileName != null)
            {
                model.Logo = requestmodel.FileName;
            }
            if (requestmodel.File != null)
            {
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ClientUserLogoDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestmodel.File.FileName),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestmodel.File.FileName)
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
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestmodel.File);
                    if (fileStatus)
                    {
                        return new OperationResult<ClientUserAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await requestmodel.File.CopyToAsync(oStream);
                }
            }
            var clientUserObj = await _clientUserService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ClientUserAddUpdateResponse>(clientUserObj);
            if (clientUserObj != null)
            {
                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (clientUserObj.Logo != null)
                {
                    responseObj.LogoURL = OneClappContext.CurrentURL + "ClientUser/Logo/" + responseObj.Id + "?" + Timestamp;
                }
                else
                {
                    responseObj.LogoURL = null;
                }
                await _hubContext.Clients.All.OnClientUserEventEmit(clientUserObj.ClientId);
            }
            return new OperationResult<ClientUserAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", responseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (Id > 0)
            {
                //IntProviderContact
                var intProviderContactObj = await _intProviderContactService.DeleteByClientUserId(Id);

                //ClientAppointment
                var clientAppointmentList = _clientAppointmentService.GetAllByClientUserId(Id);
                if (clientAppointmentList != null && clientAppointmentList.Count > 0)
                {
                    foreach (var item in clientAppointmentList)
                    {
                        //clientAppointmentObj.Id = item.Id;
                        item.ClientUserId = null;
                        item.UpdatedBy = UserId;
                        var clientAppointment = await _clientAppointmentService.CheckInsertOrUpdate(item);
                    }
                }

                //CRM notes
                var cRMNoteList = await _cRMNotesService.DeleteByClientUserId(Id);

                //clientuser
                var clientUserList = _clientUserService.GetByClientUserId(Id);
                if (clientUserList != null)
                {
                    foreach (var clientUserItem in clientUserList)
                    {
                        clientUserItem.ReportTo = null;
                        clientUserItem.UpdatedBy = UserId;
                        var AddUpdateclientUser = await _clientUserService.CheckInsertOrUpdate(clientUserItem);
                    }
                }
                var clientUserObj = await _clientUserService.DeleteById(Id);

                await _hubContext.Clients.All.OnClientUserEventEmit(clientUserObj.ClientId);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpPost]
        public async Task<OperationResult<List<ClientUserListResponse>>> List([FromBody] ClientUserListRequest requestModel)
        {
            if (requestModel.ClientId != null && requestModel.ClientId > 0)
            {
                var clientUserList = _clientUserService.GetByClientId(requestModel.ClientId);
                var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);

                List<ClientUserListResponse> clientUserListResponses = new List<ClientUserListResponse>();
                if (clientUserList != null && clientUserList.Count > 0)
                {
                    foreach (var item in clientUserList)
                    {
                        ClientUserListResponse clientUserObj = new ClientUserListResponse();
                        clientUserObj.Id = item.Id;
                        clientUserObj.Name = item.FirstName + " " + item.LastName;
                        clientUserObj.WorkEmail = item.WorkEmail;
                        if (item.ClientUserRole != null)
                        {
                            clientUserObj.Role = item.ClientUserRole.Name;
                        }
                        clientUserObj.WorkTelephoneNo = item.WorkTelephoneNo;
                        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        if (item.Logo != null)
                        {
                            clientUserObj.LogoURL = OneClappContext.CurrentURL + "ClientUser/Logo/" + item.Id + "?" + Timestamp;
                        }
                        else
                        {
                            clientUserObj.LogoURL = null;
                        }
                        clientUserListResponses.Add(clientUserObj);
                    }
                }
                int totalCount = 0;
                totalCount = clientUserListResponses.Count();
                if (!string.IsNullOrEmpty(requestModel.SearchString))
                {
                    clientUserListResponses = clientUserListResponses.Where(t => (!string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.Role) && t.Role.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.WorkEmail) && t.WorkEmail.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                    clientUserListResponses = clientUserListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
                }
                else
                {
                    clientUserListResponses = clientUserListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
                }
                clientUserListResponses = ShortClientUserByColumn(requestModel.ShortColumnName, requestModel.SortType, clientUserListResponses);
                return new OperationResult<List<ClientUserListResponse>>(true, System.Net.HttpStatusCode.OK, "", clientUserListResponses, totalCount);
            }
            else
            {
                return new OperationResult<List<ClientUserListResponse>>(false, System.Net.HttpStatusCode.OK, "Please provide clientid");
            }
        }

        private List<ClientUserListResponse> ShortClientUserByColumn(string ShortColumn, string ShortOrder, List<ClientUserListResponse> ClientUserList)
        {
            List<ClientUserListResponse> ClientUserVMList = new List<ClientUserListResponse>();
            ClientUserVMList = ClientUserList;
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn.ToLower() == "name")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientUserVMList = ClientUserList.OrderBy(t => t.Name).ToList();
                    }
                    else
                    {
                        ClientUserVMList = ClientUserList.OrderByDescending(t => t.Name).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "workwmail")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientUserVMList = ClientUserList.OrderBy(t => t.WorkEmail).ToList();
                    }
                    else
                    {
                        ClientUserVMList = ClientUserList.OrderByDescending(t => t.WorkEmail).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "role")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientUserVMList = ClientUserList.OrderBy(t => t.Role).ToList();
                    }
                    else
                    {
                        ClientUserVMList = ClientUserList.OrderByDescending(t => t.Role).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "worktelephoneno")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientUserVMList = ClientUserList.OrderBy(t => t.WorkTelephoneNo).ToList();
                    }
                    else
                    {
                        ClientUserVMList = ClientUserList.OrderByDescending(t => t.WorkTelephoneNo).ToList();
                    }
                }
                else
                {
                    ClientUserVMList = ClientUserList.OrderByDescending(t => t.Id).ToList();
                }
            }
            return ClientUserVMList;
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<ClientUserDetailResponse>> Detail(long Id)
        {
            var clientUserObj = _clientUserService.GetById(Id);
            var clientUserDetailResponseObj = _mapper.Map<ClientUserDetailResponse>(clientUserObj);
            clientUserDetailResponseObj.Department = clientUserObj.Department?.Name;
            if (clientUserObj.ReportTo != null)
            {
                clientUserDetailResponseObj.ReportToName = clientUserObj.ReportToUser.FirstName + " " + clientUserObj.ReportToUser.LastName;
            }
            if (clientUserObj.ClientUserRole != null)
            {
                clientUserDetailResponseObj.ClientUserRole = clientUserObj.ClientUserRole.Name;
            }
            if (clientUserObj.SalutationId != null)
            {
                clientUserDetailResponseObj.Salutation = clientUserObj.Salutation.Name;
            }
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (clientUserObj.Logo != null)
            {
                clientUserDetailResponseObj.FileName = clientUserObj.Logo;
                clientUserDetailResponseObj.LogoURL = OneClappContext.CurrentURL + "ClientUser/Logo/" + clientUserDetailResponseObj.Id + "?" + Timestamp;
            }
            else
            {
                clientUserDetailResponseObj.FileName = null;
                clientUserDetailResponseObj.LogoURL = null;
            }
            return new OperationResult<ClientUserDetailResponse>(true, System.Net.HttpStatusCode.OK, "", clientUserDetailResponseObj);
        }

        [HttpGet("{ClientId}")]
        public async Task<OperationResult<ClientUserDropDownListResponse>> DropdownList(long ClientId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<ClientUserDropDownListResponse> responseList = new List<ClientUserDropDownListResponse>();
            var clientUserList = _clientUserService.GetByClientId(ClientId);
            if (clientUserList != null && clientUserList.Count > 0)
            {
                foreach (var item in clientUserList)
                {
                    ClientUserDropDownListResponse responseObj = new ClientUserDropDownListResponse();
                    responseObj.Id = item.Id;
                    responseObj.Name = item.FirstName + " " + item.LastName;
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (item.Logo != null)
                    {                        
                        responseObj.LogoURL = OneClappContext.CurrentURL + "ClientUser/Logo/" + responseObj.Id + "?" + Timestamp;
                    }
                    else
                    {                        
                        responseObj.LogoURL = null;
                    }
                    responseList.Add(responseObj);
                }
            }
            return new OperationResult<ClientUserDropDownListResponse>(true, System.Net.HttpStatusCode.OK, "", responseList);
        }

        //for mircosoft reference link = https://learn.microsoft.com/en-us/graph/api/user-list-contacts?view=graph-rest-1.0&tabs=http
        //for google reference link = https://developers.google.com/people/v1/contacts#protocol
        //https://developers.google.com/people/api/rest/v1/people
        //getting token and save into db
        [HttpPost]
        public async Task<OperationResult<ClientUserContactTokenResponse>> Contacts([FromBody] ClientUserContactTokenRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            ContactTokenVM contactTokenVM = new ContactTokenVM();
            ClientUserContactTokenResponse ContactTokenResponseObj = new ClientUserContactTokenResponse();
            var model = _mapper.Map<CommonContactTokenVM>(requestmodel);
            model.grant_type = "authorization_code";
            model.type = "Access_Token";

            var customers = await _contactService.GetAccessToken(model, requestmodel.Provider);
            if (customers != null)
            {
                contactTokenVM = customers;
                if (contactTokenVM.error == null || contactTokenVM.error == "")
                {
                    List<ClientUserContactsList> ListObj = new List<ClientUserContactsList>();
                    IntProviderAppSecretDto intProviderAppSecretDto = new IntProviderAppSecretDto();
                    if (UserId != null)
                    {
                        var IntProviderObj = _intProviderService.GetIntProvider(requestmodel.Provider);
                        var IntProviderAppObj = _intProviderAppService.GetIntProviderAppByProviderId(IntProviderObj.Id, requestmodel.ProviderApp);
                        if (IntProviderObj != null)
                        {
                            intProviderAppSecretDto.Access_Token = contactTokenVM.access_token;
                            intProviderAppSecretDto.Expires_In = contactTokenVM.expires_in;
                            intProviderAppSecretDto.Refresh_Token = contactTokenVM.refresh_token;
                            intProviderAppSecretDto.Scope = contactTokenVM.scope;
                            intProviderAppSecretDto.Token_Type = contactTokenVM.token_type;
                            intProviderAppSecretDto.Id_Token = contactTokenVM.id_token;
                            intProviderAppSecretDto.IntProviderAppId = IntProviderAppObj.Id;
                            intProviderAppSecretDto.Color = requestmodel.Color;
                            intProviderAppSecretDto.CreatedBy = UserId;
                            ClientUserContactTokenVM contactTokenVMObj = new ClientUserContactTokenVM();
                            if (requestmodel.Provider.ToLower() == "microsoft")
                            {
                                contactTokenVMObj = await _contactService.GetTokenInfo("Bearer " + contactTokenVM.access_token.ToString(), requestmodel.Provider);
                                if (contactTokenVMObj != null)
                                {
                                    intProviderAppSecretDto.Email = contactTokenVMObj.userPrincipalName;
                                }
                            }
                            if (requestmodel.Provider.ToLower() == "google")
                            {
                                contactTokenVMObj = await _contactService.GetTokenInfo(contactTokenVM.access_token.ToString(), requestmodel.Provider);
                                if (contactTokenVMObj != null)
                                {
                                    intProviderAppSecretDto.Email = contactTokenVMObj.email;
                                }
                            }

                            var isExist = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(UserId, intProviderAppSecretDto.Email, IntProviderAppObj.Id);
                            if (isExist != null)
                            {
                                var responsemodel = _mapper.Map<ClientUserContactTokenResponse>(requestmodel);
                                return new OperationResult<ClientUserContactTokenResponse>(false, System.Net.HttpStatusCode.OK, "Email account already synced", responsemodel);
                            }

                            var AddUppdateIntProviderAppSecretObj = await _intProviderAppSecretService.CheckInsertOrUpdate(intProviderAppSecretDto);
                            if (IntProviderAppObj != null)
                            {
                                var clientObj = _clientService.GetById(requestmodel.ClientId);
                                var host = new System.Uri(clientObj.SiteName).Host;
                                var clientdomain = host.Substring(host.LastIndexOf('.', host.LastIndexOf('.') - 1) + 1);
                                InboxVM inboxVMObj = new InboxVM();
                                inboxVMObj.Provider = requestmodel.Provider;
                                inboxVMObj.ProviderApp = requestmodel.ProviderApp;
                                inboxVMObj.SelectedEmail = requestmodel.Email;
                                var data = await contact.GetContact(UserId, inboxVMObj);
                                if (requestmodel.Provider.ToLower() == "microsoft")
                                {
                                    if (data.value != null && data.value.Count() > 0)
                                    {
                                        foreach (var item in data.value)
                                        {
                                            ClientUserContactsList microsoftResposneObj = new ClientUserContactsList();
                                            microsoftResposneObj.Id = item.id;
                                            microsoftResposneObj.FirstName = item.givenName;
                                            microsoftResposneObj.LastName = item.surname;
                                            if (item.emailAddresses != null && item.emailAddresses.Count > 0)
                                            {
                                                foreach (var itememail in item.emailAddresses)
                                                {
                                                    microsoftResposneObj.Email = itememail.address;
                                                    string stremail = itememail.address;
                                                    string[] strArray = stremail.Split('@');
                                                    if (strArray[1] == clientdomain)
                                                    {
                                                        microsoftResposneObj.IsSameDomain = true;
                                                    }
                                                    else
                                                    {
                                                        microsoftResposneObj.IsSameDomain = false;
                                                    }
                                                }
                                            }

                                            ListObj.Add(microsoftResposneObj);
                                        }
                                    }
                                }
                                if (requestmodel.Provider.ToLower() == "google")
                                {
                                    if (data.connections != null && data.connections.Count() > 0)
                                    {
                                        foreach (var item in data.connections)
                                        {
                                            ClientUserContactsList googleResposneObj = new ClientUserContactsList();
                                            googleResposneObj.Id = item.resourceName;
                                            if (item.names != null && item.names.Count > 0)
                                            {
                                                foreach (var itemname in item.names)
                                                {
                                                    googleResposneObj.FirstName = itemname.givenName;
                                                    string strValue = itemname.displayNameLastFirst;
                                                    string[] strArray = strValue.Split(',');
                                                    for (int i = 0; i < strArray.Length; i++)
                                                    {
                                                        //ContactTokenResponseObj.FirstName = strArray[0].Trim();
                                                        googleResposneObj.LastName = strArray[0].Trim();
                                                    }

                                                }
                                            }
                                            if (item.emailAddresses != null && item.emailAddresses.Count > 0)
                                            {
                                                foreach (var itememail in item.emailAddresses)
                                                {
                                                    googleResposneObj.Email = itememail.value;
                                                    string stremail = itememail.value;
                                                    string[] strArray = stremail.Split('@');
                                                    if (strArray[1] == clientdomain)
                                                    {
                                                        googleResposneObj.IsSameDomain = true;
                                                    }
                                                    else
                                                    {
                                                        googleResposneObj.IsSameDomain = false;
                                                    }
                                                }
                                            }
                                            ListObj.Add(googleResposneObj);
                                        }
                                    }
                                }

                            }
                        }
                    }
                    ContactTokenResponseObj = _mapper.Map<ClientUserContactTokenResponse>(contactTokenVM);
                    ContactTokenResponseObj.email = intProviderAppSecretDto.Email;
                    ContactTokenResponseObj.Contacts = ListObj;
                    return new OperationResult<ClientUserContactTokenResponse>(true, System.Net.HttpStatusCode.OK, "", ContactTokenResponseObj);
                }
                else
                {
                    ContactTokenResponseObj = _mapper.Map<ClientUserContactTokenResponse>(contactTokenVM);
                    return new OperationResult<ClientUserContactTokenResponse>(false, System.Net.HttpStatusCode.OK, ContactTokenResponseObj.error, ContactTokenResponseObj);
                }
            }
            ContactTokenResponseObj = _mapper.Map<ClientUserContactTokenResponse>(contactTokenVM);
            return new OperationResult<ClientUserContactTokenResponse>(false, System.Net.HttpStatusCode.OK, "", ContactTokenResponseObj);
        }

        //[HttpPost]
        //public async Task<OperationResult<List<ClientUserContactsList>>> Contacts(long ClientId)
        // public async Task<ClientContactVM> Contacts([FromBody] ClientUserTempRequest requestmodel)
        // {
        //     //ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     //UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     //List<ClientUserContactsList> clientUserContactsList = new List<ClientUserContactsList>();

        //     //int clientId = UserId;
        //     //var userObj = _userSerVice.GetUserById(requestmodel.UserId);
        //     //var intProviderAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUser(requestmodel.UserId);
        //     //clientIntProviderAppSecretList = clientIntProviderAppSecretList.Where(t => t.IntProviderApp != null && (!string.IsNullOrEmpty(t.IntProviderApp.Name))).ToList();

        //     if (requestmodel.Provider != null && requestmodel.Provider != null && requestmodel.Email != null)
        //     {
        //         //foreach (var intProviderAppSecretObj in clientIntProviderAppSecretObj)
        //         //{
        //         InboxVM inboxVMObj = new InboxVM();
        //         inboxVMObj.Provider = requestmodel.Provider;
        //         inboxVMObj.ProviderApp = requestmodel.ProviderApp;
        //         inboxVMObj.SelectedEmail = requestmodel.Email;
        //         var data = await contact.GetContact(requestmodel.UserId, inboxVMObj);
        //         return data;
        //         // if (requestmodel.Provider.ToLower() == "microsoft")
        //         // {
        //         //     if (data.value != null && data.value.Count() > 0)
        //         //     {
        //         //         // foreach (var item in data.value)
        //         //         // {
        //         //         //     //ClientUserContactsList responseObj = new ClientUserContactsList();
        //         //         //     var clientUserRoleList = _clientUserRoleService.GetAll();

        //         //         //     ClientUser clientUserObj = new ClientUser();
        //         //         //     clientUserObj.ClientId = ClientId;
        //         //         //     clientUserObj.ClientUserRoleId = clientUserRoleList.Where(t => t.Name == "ClientUser").Select(t => t.Id).FirstOrDefault();
        //         //         //     var AddUpdateClientUser = await _clientUserService.CheckInsertOrUpdate(clientUserObj);
        //         //         //     if (AddUpdateClientUser != null)
        //         //         //     {
        //         //         //         IntProviderContact intProviderContactObj = new IntProviderContact();
        //         //         //         intProviderContactObj.ContactId = item.id;
        //         //         //         intProviderContactObj.ContactJson = item;
        //         //         //         intProviderContactObj.ClientIntProviderAppSecretId = clientIntProviderAppSecretObj.Id;
        //         //         //         intProviderContactObj.ClientUserId = AddUpdateClientUser.Id;
        //         //         //         intProviderContactObj.ClientId = ClientId;
        //         //         //         intProviderContactObj.LoggedInUserId = UserId;
        //         //         //         var AddUpdateContact = await _intProviderContactService.CheckInsertOrUpdate(intProviderContactObj);
        //         //         //     }
        //         //         //     //for response                                
        //         //         // }
        //         //     }
        //         // }
        //         // if (intProviderAppSecretObj?.IntProviderApp?.IntProvider?.Name?.ToLower() == "google")
        //         // {
        //         //     if (data.connections != null && data.connections.Count() > 0)
        //         //     {
        //         //         // foreach (var item in data.connections)
        //         //         // {
        //         //         //     var clientUserRoleList = _clientUserRoleService.GetAll();

        //         //         //     ClientUser clientUserObj = new ClientUser();
        //         //         //     clientUserObj.ClientId = ClientId;
        //         //         //     clientUserObj.ClientUserRoleId = clientUserRoleList.Where(t => t.Name == "ClientUser").Select(t => t.Id).FirstOrDefault();
        //         //         //     var AddUpdateClientUser = await _clientUserService.CheckInsertOrUpdate(clientUserObj);
        //         //         //     if (AddUpdateClientUser != null)
        //         //         //     {
        //         //         //         IntProviderContact intProviderContactObj = new IntProviderContact();
        //         //         //         intProviderContactObj.ContactId = item.resourceName;
        //         //         //         intProviderContactObj.ContactJson = item;
        //         //         //         intProviderContactObj.ClientIntProviderAppSecretId = clientIntProviderAppSecretObj.Id;
        //         //         //         intProviderContactObj.ClientUserId = AddUpdateClientUser.Id;
        //         //         //         intProviderContactObj.ClientId = ClientId;
        //         //         //         intProviderContactObj.LoggedInUserId = UserId;
        //         //         //         var AddUpdateContact = await _intProviderContactService.CheckInsertOrUpdate(intProviderContactObj);
        //         //         //     }
        //         //         // }
        //         //     }
        //         // }
        //         //}
        //         //await _hubContext.Clients.All.OnClientUserEventEmit(requestmodel.ClientId);
        //         //return new OperationResult(true, System.Net.HttpStatusCode.OK, "Sync successfully", requestmodel.ClientId);
        //     }
        //     return null;
        //     //return new OperationResult(false, System.Net.HttpStatusCode.OK, "", UserId);
        // }

        [HttpPost]
        public async Task<OperationResult<ClientUserCreateContactResponse>> Create([FromBody] ClientUserCreateContactRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            ClientUserCreateContactResponse responseObj = new ClientUserCreateContactResponse();
            if (requestmodel?.ClientId != null && requestmodel.ClientId > 0)
            {
                ClientCreateContactVM data = new ClientCreateContactVM();
                if (string.IsNullOrEmpty(requestmodel.ContactId))
                {
                    data = await contact.CreateContact(UserId, "", requestmodel);
                    if (!string.IsNullOrEmpty(data.id))
                    {
                        responseObj.Id = data.id;
                    }
                    if (!string.IsNullOrEmpty(data.resourceName))
                    {
                        responseObj.Id = data.resourceName;
                    }
                    return new OperationResult<ClientUserCreateContactResponse>(true, System.Net.HttpStatusCode.OK, "Created", responseObj);
                }
                else
                {
                    data = await contact.CreateContact(UserId, requestmodel.ContactId, requestmodel);
                    if (!string.IsNullOrEmpty(data.id))
                    {
                        responseObj.Id = data.id;
                    }
                    if (!string.IsNullOrEmpty(data.resourceName))
                    {
                        responseObj.Id = data.resourceName;
                    }
                    return new OperationResult<ClientUserCreateContactResponse>(true, System.Net.HttpStatusCode.OK, "Updated", responseObj);
                }
            }
            else
            {
                return new OperationResult<ClientUserCreateContactResponse>(true, System.Net.HttpStatusCode.OK, "Please provide clientid");
            }
        }

        [SwaggerOperation(Description = "this api used for import ClientUser from csv(excel) file")]
        [HttpPost]
        public async Task<OperationResult<List<ClientUserImportResponse>>> Import([FromBody] List<ClientUserImportRequest> requestModelList)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<ClientUserImportResponse> clientUserImportResponseList = new List<ClientUserImportResponse>();
            if (requestModelList != null && requestModelList.Count > 0)
            {
                long? clientId = 0;
                foreach (var modelObj in requestModelList)
                {
                    if (modelObj != null)
                    {
                        if (modelObj.ClientId != null && modelObj.ClientId > 0)
                        {
                            clientId = modelObj.ClientId;
                            var salutationObj = _salutationService.GetAll().Where(t => t.Name.ToLower() == modelObj.Salutation.ToLower()).FirstOrDefault();
                            var departmentObj = _departmentService.GetAll().Where(t => t.Name.ToLower() == modelObj.Department.ToLower()).FirstOrDefault();
                            var reportToObj = _clientUserService.GetAll().Where(t => t.ClientId == modelObj.ClientId && ((t.FirstName != null && t.FirstName.ToLower() == modelObj.FirstName.ToLower()) || (t.LastName != null && t.LastName.ToLower() == modelObj.LastName.ToLower()))).FirstOrDefault();
                            var clientUserRoleObj = _clientUserRoleService.GetAll().Where(t => t.Name.ToLower() == modelObj.ClientUserRole.ToLower()).FirstOrDefault();

                            ClientUser clientUserObj = new ClientUser();
                            clientUserObj.FirstName = modelObj.FirstName;
                            clientUserObj.MiddleName = modelObj.MiddleName;
                            clientUserObj.LastName = modelObj.LastName;
                            clientUserObj.Title = modelObj.Title;
                            clientUserObj.ClientId = modelObj.ClientId;
                            if (departmentObj != null)
                            {
                                clientUserObj.DepartmentId = departmentObj.Id;
                            }
                            else
                            {
                                clientUserObj.DepartmentId = null;
                            }
                            if (reportToObj != null)
                            {
                                clientUserObj.ReportTo = reportToObj.Id;
                            }
                            else
                            {
                                clientUserObj.ReportTo = null;
                            }
                            if (clientUserRoleObj != null)
                            {
                                clientUserObj.ClientUserRoleId = clientUserRoleObj.Id;
                            }
                            else
                            {
                                clientUserObj.ClientUserRoleId = null;
                            }
                            if (salutationObj != null)
                            {
                                clientUserObj.SalutationId = salutationObj.Id;
                            }
                            else
                            {
                                clientUserObj.SalutationId = null;
                            }
                            clientUserObj.Description = modelObj.AdditionalInfo;
                            clientUserObj.IsActive = true;
                            clientUserObj.IsUnSubscribe = false;
                            clientUserObj.IntProviderId = null;
                            clientUserObj.PersonalEmail = modelObj.PersonalEmail;
                            clientUserObj.WorkEmail = modelObj.WorkEmail;
                            clientUserObj.MobileNo = modelObj.MobileNo;
                            clientUserObj.WorkTelephoneNo = modelObj.WorkTelephoneNo;
                            clientUserObj.PrivateTelephoneNo = modelObj.PrivateTelephoneNo;
                            clientUserObj.CreatedBy = UserId;                            
                            var AddUpdateclientUserObj = await _clientUserService.CheckInsertOrUpdate(clientUserObj);
                            if (AddUpdateclientUserObj == null)
                            {
                                return new OperationResult<List<ClientUserImportResponse>>(false, System.Net.HttpStatusCode.OK, "Not inserted");
                            }
                            else
                            {
                                ClientUserImportResponse responseObj = new ClientUserImportResponse();
                                responseObj.Id = AddUpdateclientUserObj.Id;
                                responseObj.Name = AddUpdateclientUserObj.FirstName + " " + AddUpdateclientUserObj.LastName;
                                responseObj.WorkEmail = AddUpdateclientUserObj.WorkEmail;
                                if (AddUpdateclientUserObj.ClientUserRole != null)
                                {
                                    responseObj.Role = AddUpdateclientUserObj.ClientUserRole.Name;
                                }
                                responseObj.WorkTelephoneNo = AddUpdateclientUserObj.WorkTelephoneNo;
                                clientUserImportResponseList.Add(responseObj);
                            }
                            //clientId = modelObj.ClientId;
                            //await _hubContext.Clients.All.OnClientUserEventEmit(clientId);
                        }
                        else
                        {
                            return new OperationResult<List<ClientUserImportResponse>>(true, System.Net.HttpStatusCode.OK, "Please provide clientid");
                        }
                    }
                }
                await _hubContext.Clients.All.OnClientUserEventEmit(clientId);
                return new OperationResult<List<ClientUserImportResponse>>(true, System.Net.HttpStatusCode.OK, "", clientUserImportResponseList);
            }
            else
            {
                return new OperationResult<List<ClientUserImportResponse>>(true, System.Net.HttpStatusCode.OK, "data empty");
            }
        }

    }
}