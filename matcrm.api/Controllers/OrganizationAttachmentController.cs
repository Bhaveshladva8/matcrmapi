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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{
    // [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class OrganizationAttachmentController : Controller
    {

        private readonly IOrganizationAttachmentService _organizationAttachmentService;
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public OrganizationAttachmentController(
            IOrganizationAttachmentService organizationAttachmentService,
            ICustomerService customerService,
            IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            OneClappContext context,
            IMapper mapper)
        {
            _organizationAttachmentService = organizationAttachmentService;
            _customerService = customerService;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _hubContext = hubContext;
            _mapper = mapper;
            _context = context;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<OrganizationAttachUploadFilesResponse>>> UploadFiles([FromForm] OrganizationAttachUploadFilesRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<OrganizationAttachmentDto>(model);
            List<OrganizationAttachment> organizationAttachmentList = new List<OrganizationAttachment>();

            if (requestmodel.FileList == null) throw new Exception("File is null");
            if (requestmodel.FileList.Length == 0) throw new Exception("File is empty");

            foreach (IFormFile file in requestmodel.FileList)
            {
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\OrganizationUpload";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.OrganizationUploadDirPath;

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
                        return new OperationResult<List<OrganizationAttachUploadFilesResponse>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                OrganizationAttachmentDto organizationAttachmentDto = new OrganizationAttachmentDto();
                organizationAttachmentDto.FileName = fileName;
                organizationAttachmentDto.OrganizationId = requestmodel.OrganizationId;
                organizationAttachmentDto.CreatedBy = UserId;
                organizationAttachmentDto.TenantId = TenantId;
                var addedItem = await _organizationAttachmentService.CheckInsertOrUpdate(organizationAttachmentDto);
                organizationAttachmentList.Add(addedItem);
            }

            await _hubContext.Clients.All.OnUploadOrganizationDocumentEventEmit(requestmodel.OrganizationId);
            var responseAttachmentList = _mapper.Map<List<OrganizationAttachUploadFilesResponse>>(organizationAttachmentList);
            return new OperationResult<List<OrganizationAttachUploadFilesResponse>>(true, System.Net.HttpStatusCode.OK, "", responseAttachmentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{OrganizationId}")]
        public async Task<OperationResult<List<OrganizationAttachmentGetAllResponse>>> List(long OrganizationId)
        {
            var users = _userService.GetAll();

            List<OrganizationAttachmentDto> organizationAttachmentDtoList = new List<OrganizationAttachmentDto>();
            var organizationAttachments = _organizationAttachmentService.GetAllByOrganizationId(OrganizationId);
            organizationAttachmentDtoList = _mapper.Map<List<OrganizationAttachmentDto>>(organizationAttachments);
            if (organizationAttachmentDtoList != null && organizationAttachmentDtoList.Count() > 0)
            {
                foreach (var item in organizationAttachmentDtoList)
                {
                    if (users != null)
                    {
                        var userObj1 = users.Where(t => t.Id == item.CreatedBy).FirstOrDefault();
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
            var responseAttachmentListDto = _mapper.Map<List<OrganizationAttachmentGetAllResponse>>(organizationAttachmentDtoList);
            return new OperationResult<List<OrganizationAttachmentGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseAttachmentListDto);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<string>> DownloadFile(long Id)
        {

            var organizationAttachmentObj = _organizationAttachmentService.GetOrganizationAttachmentById(Id);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\OrganizationUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.OrganizationUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (organizationAttachmentObj != null)
            {
                filePath = dirPath + "\\" + organizationAttachmentObj.FileName;
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
        public async Task<OperationResult<OrganizationAttachmentDeleteResponse>> Remove([FromBody] OrganizationAttachmentDeleteRequest model)
        {
            var requestmodel = _mapper.Map<OrganizationAttachmentDto>(model);
            OrganizationAttachmentDeleteResponse responsemodel = new OrganizationAttachmentDeleteResponse();
            if (requestmodel.Id != null)
            {
                var attachmentId = requestmodel.Id.Value;

                var organizationDocument = _organizationAttachmentService.DeleteOrganizationAttachmentById(attachmentId);
                if (organizationDocument != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\OrganizationUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.OrganizationUploadDirPath;
                    var filePath = dirPath + "\\" + organizationDocument.FileName;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    await _hubContext.Clients.All.OnUploadOrganizationDocumentEventEmit(requestmodel.OrganizationId);

                    responsemodel = _mapper.Map<OrganizationAttachmentDeleteResponse>(requestmodel);
                    return new OperationResult<OrganizationAttachmentDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Organization file deleted successfully", responsemodel);
                }
                else
                {
                    return new OperationResult<OrganizationAttachmentDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Doccument not found");
                }
            }
            else
            {
                responsemodel = _mapper.Map<OrganizationAttachmentDeleteResponse>(requestmodel);
                return new OperationResult<OrganizationAttachmentDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Attachment Id null", responsemodel);
            }
        }

        [AllowAnonymous]
        [HttpPut]
        public async Task<OperationResult<OrganizationAttachUpdateDescResponse>> UpdateDescription([FromBody] OrganizationAttachUpdateDescRequest model)
        {
            var requestmodel = _mapper.Map<OrganizationAttachmentDto>(model);
            if (requestmodel.Id != null)
            {
                var organizationFileObj = _organizationAttachmentService.GetOrganizationAttachmentById(requestmodel.Id.Value);
                organizationFileObj.Description = requestmodel.Description;
                var data = await _organizationAttachmentService.UpdateOrganizationAttachment(organizationFileObj, organizationFileObj.Id);
                await _hubContext.Clients.All.OnUpdateOrganizationDescriptionEventEmit(requestmodel.OrganizationId, requestmodel.Id, requestmodel.Description);
                var responsemodel = _mapper.Map<OrganizationAttachUpdateDescResponse>(requestmodel);
                return new OperationResult<OrganizationAttachUpdateDescResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
            else
            {
                var responsemodel = _mapper.Map<OrganizationAttachUpdateDescResponse>(requestmodel);
                return new OperationResult<OrganizationAttachUpdateDescResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", responsemodel);
            }
        }
    }
}