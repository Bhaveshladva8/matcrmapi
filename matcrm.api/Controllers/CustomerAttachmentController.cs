using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
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
    public class CustomerAttachmentController : Controller
    {
        private readonly ICustomerAttachmentService _customerAttachmentService;
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public CustomerAttachmentController(
            ICustomerAttachmentService customerAttachmentService,
            ICustomerService customerService,
            IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            OneClappContext context,
            IMapper mapper)
        {
            _customerAttachmentService = customerAttachmentService;
            _customerService = customerService;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<CustomerAttachmentUploadFilesResponse>>> UploadFiles([FromForm] CustomerAttachmentUploadFilesRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<CustomerAttachmentDto>(model);
            List<CustomerAttachment> customerAttachmentList = new List<CustomerAttachment>();
            List<CustomerAttachmentUploadFilesResponse> responseCustomerAttachmentList = new List<CustomerAttachmentUploadFilesResponse>();

            if (requestmodel.FileList == null) throw new Exception("File is null");
            if (requestmodel.FileList.Length == 0) throw new Exception("File is empty");

            foreach (IFormFile file in requestmodel.FileList)
            {
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\CustomerUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.CustomerFileUploadDirPath;

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
                        return new OperationResult<List<CustomerAttachmentUploadFilesResponse>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }
                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                CustomerAttachmentDto customerAttchmentObj = new CustomerAttachmentDto();
                customerAttchmentObj.FileName = fileName;
                customerAttchmentObj.CustomerId = requestmodel.CustomerId;
                customerAttchmentObj.TenantId = TenantId;
                if (requestmodel.Id == null)
                {
                    customerAttchmentObj.CreatedBy = UserId;
                }
                else
                {
                    customerAttchmentObj.UpdatedBy = UserId;
                }
                var addedItem = await _customerAttachmentService.CheckInsertOrUpdate(customerAttchmentObj);
                customerAttachmentList.Add(addedItem);
            }

            await _hubContext.Clients.All.OnUploadCustomerDocumentEventEmit(requestmodel.CustomerId);
            responseCustomerAttachmentList = _mapper.Map<List<CustomerAttachmentUploadFilesResponse>>(customerAttachmentList);
            return new OperationResult<List<CustomerAttachmentUploadFilesResponse>>(true, System.Net.HttpStatusCode.OK, "", responseCustomerAttachmentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{CustomerId}")]
        public async Task<OperationResult<List<CustomerAttachmentGetAllResposne>>> List(long CustomerId)
        {

            var userList = _userService.GetAll();
            List<CustomerAttachmentDto> customerAttachmentDtoList = new List<CustomerAttachmentDto>();
            var CustomerAttachments = _customerAttachmentService.GetAllByCustomerId(CustomerId);
            customerAttachmentDtoList = _mapper.Map<List<CustomerAttachmentDto>>(CustomerAttachments);
            if (customerAttachmentDtoList != null && customerAttachmentDtoList.Count() > 0)
            {
                foreach (var item in customerAttachmentDtoList)
                {
                    if (userList != null)
                    {
                        var userObj1 = userList.Where(t => t.Id == item.CreatedBy).FirstOrDefault();
                        if (userObj1 != null)
                        {
                            item.FirstName = userObj1.FirstName;
                            item.LastName = userObj1.LastName;
                            item.Email = userObj1.Email;
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
            var responseCustomerAttachmentDtoList = _mapper.Map<List<CustomerAttachmentGetAllResposne>>(customerAttachmentDtoList);
            return new OperationResult<List<CustomerAttachmentGetAllResposne>>(true, System.Net.HttpStatusCode.OK, "", responseCustomerAttachmentDtoList);
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<OperationResult<string>> Document(long Id)
        {
            var customerAttachmentObj = _customerAttachmentService.GetCustomerAttachmentById(Id);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\CustomerUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.CustomerFileUploadDirPath;
            var filePath = "";
            if (customerAttachmentObj != null && !string.IsNullOrEmpty(customerAttachmentObj.FileName))
            {
                filePath = dirPath + "\\" + customerAttachmentObj.FileName;
            }
            else
            {
                filePath = dirPath + "\\" + "default.png";
            }
            Byte[] newBytes = System.IO.File.ReadAllBytes(filePath);
            String file = Convert.ToBase64String(newBytes);
            if (file != "")
            {
                return new OperationResult<string>(true, System.Net.HttpStatusCode.OK, "File received successfully", file);
            }
            else
            {
                return new OperationResult<string>(false, System.Net.HttpStatusCode.OK, "Issue in downloading file.");
            }

        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CustomerAttachmentDeleteResponse>> Remove([FromBody] CustomerAttachmentDeleteRequest model)
        {
            var requestmodel = _mapper.Map<CustomerAttachmentDto>(model);
            if (requestmodel.Id != null)
            {
                var attachmentId = requestmodel.Id.Value;

                var customerDocument = _customerAttachmentService.DeleteCustomerAttachmentById(attachmentId);
                if (customerDocument != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\CustomerUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.CustomerFileUploadDirPath;
                    var filePath = dirPath + "\\" + customerDocument.FileName;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    await _hubContext.Clients.All.OnUploadCustomerDocumentEventEmit(requestmodel.CustomerId);

                    var reponsemodel = _mapper.Map<CustomerAttachmentDeleteResponse>(requestmodel);
                    return new OperationResult<CustomerAttachmentDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Customer file deleted successfully", reponsemodel);
                }
                else
                {
                    return new OperationResult<CustomerAttachmentDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Doccument not found");
                }
            }
            else
            {
                var reponsemodel = _mapper.Map<CustomerAttachmentDeleteResponse>(requestmodel);
                return new OperationResult<CustomerAttachmentDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Attachment Id null", reponsemodel);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<OperationResult<CustomerAttachDescriptionResponse>> UpdateDescription([FromBody] CustomerAttachDescriptionRequest model)
        {
            var requestmodel = _mapper.Map<CustomerAttachmentDto>(model);
            if (requestmodel.Id != null)
            {
                var customerAttachmentObj = _customerAttachmentService.GetCustomerAttachmentById(requestmodel.Id.Value);
                customerAttachmentObj.Description = requestmodel.Description;
                var data = await _customerAttachmentService.UpdateCustomerAttachment(customerAttachmentObj, customerAttachmentObj.Id);
                await _hubContext.Clients.All.OnUpdateCustomerDescriptionEventEmit(requestmodel.CustomerId, requestmodel.Id, requestmodel.Description);
                var resposemodel = _mapper.Map<CustomerAttachDescriptionResponse>(requestmodel);
                return new OperationResult<CustomerAttachDescriptionResponse>(true, System.Net.HttpStatusCode.OK, "", resposemodel);
            }
            else
            {
                var resposemodel = _mapper.Map<CustomerAttachDescriptionResponse>(requestmodel);
                return new OperationResult<CustomerAttachDescriptionResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", resposemodel);
            }
        }
    }
}