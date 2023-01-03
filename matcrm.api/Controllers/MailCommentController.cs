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
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{

    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MailCommentController : Controller
    {
        private readonly IDiscussionParticipantService _discussionParticipantService;
        private readonly IMailCommentAttachmentService _mailCommentAttachmentService;
        private readonly IMailAssignUserService _mailAssignUserService;
        private readonly IMailCommentService _mailCommentService;
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private IMapper _mapper;
        private readonly OneClappContext _context;
        private int UserId = 0;
        private int TenantId = 0;
        private Common Common;
        public MailCommentController(
        IDiscussionParticipantService discussionParticipantService,
        IMailAssignUserService mailAssignUserService,
        IMailCommentAttachmentService mailCommentAttachmentService,
        IMailCommentService mailCommentService,
        IHostingEnvironment hostingEnvironment,
        IUserService userService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IMapper mapper,
        OneClappContext context)
        {
            _discussionParticipantService = discussionParticipantService;
            _mailAssignUserService = mailAssignUserService;
            _mailCommentAttachmentService = mailCommentAttachmentService;
            _mailCommentService = mailCommentService;
            _hostingEnvironment = hostingEnvironment;
            _userService = userService;
            _hubContext = hubContext;
            _mapper = mapper;
            _context = context;
            Common = new Common();
        }


        [HttpPost]
        public async Task<OperationResult<MailCommentAddUpdateResponse>> AddUpdate([FromForm] MailCommentAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<MailCommentDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.TeamMemberId = UserId;
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var mailCommentObj = await _mailCommentService.CheckInsertOrUpdate(requestmodel, "");

            if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0 && mailCommentObj != null)
            {

                foreach (IFormFile file in requestmodel.FileList)
                {
                    // full path to file in temp location
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\MailCommentUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MailCommentUploadDirPath;

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
                            return new OperationResult<MailCommentAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }

                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(oStream);
                    }

                    MailCommentAttachmentDto mailCommentAttachmentDto = new MailCommentAttachmentDto();
                    mailCommentAttachmentDto.FileName = fileName;
                    mailCommentAttachmentDto.MailCommentId = mailCommentObj.Id;
                    mailCommentAttachmentDto.CreatedBy = UserId;
                    var addedItem = await _mailCommentAttachmentService.CheckInsertOrUpdate(mailCommentAttachmentDto);
                    MailCommentAttachmentDto mailCommentAttachmentDto1 = _mapper.Map<MailCommentAttachmentDto>(addedItem);
                    requestmodel.Attachments.Add(mailCommentAttachmentDto1);
                }
            }

            requestmodel = _mapper.Map<MailCommentDto>(mailCommentObj);

            await _hubContext.Clients.All.OnMailCommentEmit(mailCommentObj.ThreadId);
            var responsemodel = _mapper.Map<MailCommentAddUpdateResponse>(requestmodel);

            if (requestmodel.Id > 0)
                return new OperationResult<MailCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            return new OperationResult<MailCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responsemodel);
        }


        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null && Id > 0)
            {
                var mailCommentAttachments = _mailCommentAttachmentService.DeleteAttachmentByMailComment(Id);
                if (mailCommentAttachments != null && mailCommentAttachments.Count() > 0)
                {
                    foreach (var mailCommentAttachmentObj in mailCommentAttachments)
                    {
                        if (mailCommentAttachmentObj != null)
                        {
                            //var dirPath = _hostingEnvironment.WebRootPath + "\\MailCommentUpload";
                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MailCommentUploadDirPath;
                            var filePath = dirPath + "\\" + mailCommentAttachmentObj.FileName;

                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(Path.Combine(filePath));
                            }
                        }
                    }
                }
                var discussionObj = await _mailCommentService.Delete(Id);

                await _hubContext.Clients.All.OnMailCommentEmit(discussionObj.ThreadId);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpPut]
        public async Task<OperationResult<MailCommentPinUnpinResponse>> PinUnpin([FromBody] MailCommentPinUnpinRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<MailCommentDto>(model);
            if (requestmodel.Id != null)
            {
                var mailCommentObj = _mailCommentService.GetById(requestmodel.Id.Value);
                mailCommentObj.IsPinned = requestmodel.IsPinned;
                if (requestmodel.IsPinned)
                {
                    mailCommentObj.PinnedBy = UserId;
                }
                else
                {
                    requestmodel.PinnedBy = null;
                }
                if (requestmodel.Id == null)
                {
                    requestmodel.CreatedBy = UserId;
                }
                else
                {
                    requestmodel.UpdatedBy = UserId;
                }
                requestmodel.TenantId = TenantId;
                requestmodel = _mapper.Map<MailCommentDto>(mailCommentObj);
                var mailObj = await _mailCommentService.CheckInsertOrUpdate(requestmodel, "PinUnpin");
                var responseresult = _mapper.Map<MailCommentPinUnpinResponse>(mailCommentObj);
                await _hubContext.Clients.All.OnMailCommentEmit(mailCommentObj.ThreadId);
                return new OperationResult<MailCommentPinUnpinResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responseresult);
            }
            else
            {
                var responseresult = _mapper.Map<MailCommentPinUnpinResponse>(requestmodel);
                return new OperationResult<MailCommentPinUnpinResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id.", responseresult);
            }
        }


        [HttpDelete("{Id}")]
        public async Task<OperationResult> RemoveAttachment(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null && Id > 0)
            {
                var mailCommentAttachmentObj = await _mailCommentAttachmentService.DeleteMailCommentAttachmentById(Id);
                if (mailCommentAttachmentObj != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\MailCommentUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MailCommentUploadDirPath;
                    var filePath = dirPath + "\\" + mailCommentAttachmentObj.FileName;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    await _hubContext.Clients.All.OnMailCommentEmit(mailCommentAttachmentObj.MailComment.ThreadId);

                    return new OperationResult(true, System.Net.HttpStatusCode.OK, "Mail comment file deleted successfully", Id);
                }
                else
                {
                    return new OperationResult(false, System.Net.HttpStatusCode.OK, "Doccument not found", Id);
                }
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        // [AllowAnonymous]
        // [HttpGet("GetAttachmentById")]
        // public async Task<OperationResult<string>> GetAttachmentById(long Id)
        // {
        //     var mailCommentAttachmentObj = _mailCommentAttachmentService.GetById(Id);

        //     // full path to file in temp location
        //     var dirPath = _hostingEnvironment.WebRootPath + "\\MailCommentUpload";
        //     var filePath = dirPath + "\\" + "default.png";
        //     if (mailCommentAttachmentObj != null)
        //     {
        //         filePath = dirPath + "\\" + mailCommentAttachmentObj.FileName;
        //     }
        //     Byte[] newBytes = System.IO.File.ReadAllBytes(filePath);
        //     String file = Convert.ToBase64String(newBytes);
        //     if (file != "")
        //     {
        //         return new OperationResult<string>(true, "File received successfully", file);
        //     }
        //     else
        //     {
        //         return new OperationResult<string>(false, "Issue in downloading file.");
        //     }
        // }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> Attachment(long Id)
        {
            var mailCommentAttachmentObj = _mailCommentAttachmentService.GetById(Id);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\MailCommentUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MailCommentUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (mailCommentAttachmentObj != null)
            {
                filePath = dirPath + "\\" + mailCommentAttachmentObj.FileName;
            }
            Byte[] newBytes = System.IO.File.ReadAllBytes(filePath);
            return File(newBytes, Common.GetMimeTypes(mailCommentAttachmentObj.FileName), mailCommentAttachmentObj.FileName);
        }
        
        [Authorize]
        [HttpGet("{ThreadId}")]
        public async Task<OperationResult<List<MailCommentGetAllResponse>>> List(string ThreadId)
        {
            var mailCommentList = _mailCommentService.GetAllByThread(ThreadId);
            var users = _userService.GetAll();
            List<MailCommentDto> mailCommentDtoList = _mapper.Map<List<MailCommentDto>>(mailCommentList);
            if (mailCommentDtoList != null && mailCommentDtoList.Count() > 0)
            {
                foreach (var mailCommentObj in mailCommentDtoList)
                {
                    if (users != null && users.Count() > 0)
                    {
                        var userObj = users.Where(t => t.Id == mailCommentObj.TeamMemberId).FirstOrDefault();
                        if (userObj != null)
                        {
                            mailCommentObj.FirstName = userObj.FirstName;
                            mailCommentObj.LastName = userObj.LastName;
                            mailCommentObj.Email = userObj.Email;
                            if (mailCommentObj.FirstName != null)
                            {
                                mailCommentObj.ShortName = mailCommentObj.FirstName.Substring(0, 1);
                            }
                            if (mailCommentObj.LastName != null)
                            {
                                mailCommentObj.ShortName = mailCommentObj.ShortName + mailCommentObj.LastName.Substring(0, 1);
                            }
                        }
                    }
                    if (mailCommentObj.Id != null)
                    {
                        var attachments = _mailCommentAttachmentService.GetAllByMailCommentId(mailCommentObj.Id.Value);
                        mailCommentObj.Attachments = _mapper.Map<List<MailCommentAttachmentDto>>(attachments);
                    }
                }
            }
            var mailCommentgetallresponse = _mapper.Map<List<MailCommentGetAllResponse>>(mailCommentDtoList);
            return new OperationResult<List<MailCommentGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", mailCommentgetallresponse);
        }
    }
}