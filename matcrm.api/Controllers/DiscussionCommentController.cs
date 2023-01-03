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
    public class DiscussionCommentController : Controller
    {

        private readonly IDiscussionService _discussionService;
        private readonly IDiscussionReadService _discussionReadService;
        private readonly IDiscussionParticipantService _discussionParticipantService;
        private readonly IDiscussionCommentAttachmentService _discussionCommentAttachmentService;
        private readonly IDiscussionCommentService _discussionCommentService;
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private Common Common;
        private int UserId = 0;
        public DiscussionCommentController(IDiscussionService discussionService,
        IDiscussionReadService discussionReadService,
        IDiscussionParticipantService discussionParticipantService,
        IDiscussionCommentAttachmentService discussionCommentAttachmentService,
        IDiscussionCommentService discussionCommentService,
        IHostingEnvironment hostingEnvironment,
        IUserService userService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        OneClappContext context,
        IMapper mapper)
        {
            _discussionService = discussionService;
            _discussionReadService = discussionReadService;
            _discussionParticipantService = discussionParticipantService;
            _discussionCommentAttachmentService = discussionCommentAttachmentService;
            _discussionCommentService = discussionCommentService;
            _hostingEnvironment = hostingEnvironment;
            _userService = userService;
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
            Common = new Common();
        }

        [HttpPost]
        public async Task<OperationResult<DiscussionCommentAddUpdateResponse>> AddUpdate([FromForm] DiscussionCommentAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<DiscussionCommentDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.TeamMemberId = UserId;
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }

            var discussionCommentObj = await _discussionCommentService.CheckInsertOrUpdate(requestmodel, "");

            if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0 && discussionCommentObj != null)
            {

                foreach (IFormFile file in requestmodel.FileList)
                {
                    // full path to file in temp location
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\DiscussionCommentUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DiscussionCommentUploadDirPath;

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
                            return new OperationResult<DiscussionCommentAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }

                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(oStream);
                    }

                    DiscussionCommentAttachmentDto discussionCommentAttachmentDto1 = new DiscussionCommentAttachmentDto();
                    discussionCommentAttachmentDto1.FileName = fileName;
                    discussionCommentAttachmentDto1.DiscussionCommentId = discussionCommentObj.Id;
                    discussionCommentAttachmentDto1.CreatedBy = UserId;
                    var addedItem = await _discussionCommentAttachmentService.CheckInsertOrUpdate(discussionCommentAttachmentDto1);
                    DiscussionCommentAttachmentDto discussionCommentAttachmentDto = _mapper.Map<DiscussionCommentAttachmentDto>(addedItem);
                    requestmodel.Attachments.Add(discussionCommentAttachmentDto);
                }
            }

            requestmodel = _mapper.Map<DiscussionCommentDto>(discussionCommentObj);

            await _hubContext.Clients.All.OnDiscussionCommentEmit(discussionCommentObj.DiscussionId);
            var responsemodel = _mapper.Map<DiscussionCommentAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<DiscussionCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            return new OperationResult<DiscussionCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responsemodel);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (Id != null && Id > 0)
            {
                var discussionCommentAttachments = _discussionCommentAttachmentService.DeleteAttachmentByDiscussionComment(Id);
                if (discussionCommentAttachments != null && discussionCommentAttachments.Count() > 0)
                {
                    foreach (var discussionCommentAttachmentObj in discussionCommentAttachments)
                    {
                        if (discussionCommentAttachmentObj != null)
                        {
                            //var dirPath = _hostingEnvironment.WebRootPath + "\\DiscussionCommentUpload";
                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DiscussionCommentUploadDirPath;
                            var filePath = dirPath + "\\" + discussionCommentAttachmentObj.FileName;

                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(Path.Combine(filePath));
                            }
                        }
                    }
                }
                var discussionCommentObj = await _discussionCommentService.Delete(Id);
                await _hubContext.Clients.All.OnDiscussionCommentEmit(discussionCommentObj.DiscussionId);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpPut]
        public async Task<OperationResult<DiscussionCommentPinUnpinResponse>> PinUnpin([FromBody] DiscussionCommentPinUnpinRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<DiscussionCommentDto>(model);
            if (requestmodel.Id != null)
            {
                var userObj = _userService.GetUserById(UserId);
                var discussionCommentObj = _discussionCommentService.GetById(requestmodel.Id.Value);
                var discussionCommentDto = _mapper.Map<DiscussionCommentDto>(discussionCommentObj);
                discussionCommentDto.IsPinned = requestmodel.IsPinned;
                if (requestmodel.IsPinned)
                {
                    requestmodel.PinnedBy = UserId;
                    if (userObj != null)
                    {
                        discussionCommentDto.FirstName = userObj.FirstName;
                        discussionCommentDto.LastName = userObj.LastName;
                        discussionCommentDto.Email = userObj.Email;
                        if (discussionCommentDto.FirstName != null)
                        {
                            discussionCommentDto.ShortName = discussionCommentDto.FirstName.Substring(0, 1);
                        }
                        if (discussionCommentDto.LastName != null)
                        {
                            discussionCommentDto.ShortName = discussionCommentDto.ShortName + discussionCommentDto.LastName.Substring(0, 1);
                        }
                    }
                }
                else
                {
                    discussionCommentDto.PinnedBy = null;
                }
                // model = _mapper.Map<DiscussionCommentDto>(discussionCommentObj);
                if (requestmodel.Id == null)
                {
                    discussionCommentDto.CreatedBy = UserId;
                }
                else
                {
                    discussionCommentDto.UpdatedBy = UserId;
                }
                discussionCommentObj = await _discussionCommentService.CheckInsertOrUpdate(discussionCommentDto, "PinUnpin");
                await _hubContext.Clients.All.OnDiscussionCommentEmit(discussionCommentObj.DiscussionId);
                var reponsemodel = _mapper.Map<DiscussionCommentPinUnpinResponse>(requestmodel);
                return new OperationResult<DiscussionCommentPinUnpinResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", reponsemodel);
            }
            else
            {
                return new OperationResult<DiscussionCommentPinUnpinResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id.");
            }

        }


        // [HttpDelete]
        // public async Task<OperationResult<NewDiscussionCommentAttachment>> RemoveAttachment([FromBody] NewDiscussionCommentAttachment model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     var requestmodel = _mapper.Map<DiscussionCommentAttachmentDto>(model);
        //     if (requestmodel.Id != null)
        //     {
        //         var discussionCommentAttachmentObj = _discussionCommentAttachmentService.DeleteDiscussionCommentAttachmentById(requestmodel.Id.Value);
        //         if (discussionCommentAttachmentObj != null)
        //         {
        //             var dirPath = _hostingEnvironment.WebRootPath + "\\DiscussionCommentUpload";
        //             var filePath = dirPath + "\\" + discussionCommentAttachmentObj.FileName;

        //             if (System.IO.File.Exists(filePath))
        //             {
        //                 System.IO.File.Delete(Path.Combine(filePath));
        //             }

        //             await _hubContext.Clients.All.OnDiscussionCommentEmit(discussionCommentAttachmentObj.DiscussionComment.DiscussionId);

        //             var responsemodel = _mapper.Map<NewDiscussionCommentAttachment>(requestmodel);
        //             return new OperationResult<NewDiscussionCommentAttachment>(true, System.Net.HttpStatusCode.OK, "Discussion comment file deleted successfully", responsemodel);
        //         }
        //         else
        //         {
        //             return new OperationResult<NewDiscussionCommentAttachment>(false, System.Net.HttpStatusCode.OK, "Doccument not found");
        //         }
        //     }
        //     else
        //     {
        //         return new OperationResult<NewDiscussionCommentAttachment>(false, System.Net.HttpStatusCode.OK, "Please provide id.", model);
        //     }
        // }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> RemoveAttachment(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null && Id > 0)
            {
                var discussionCommentAttachmentObj = await _discussionCommentAttachmentService.DeleteDiscussionCommentAttachmentById(Id);
                if (discussionCommentAttachmentObj != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\DiscussionCommentUpload";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DiscussionCommentUploadDirPath;
                    var filePath = dirPath + "\\" + discussionCommentAttachmentObj.FileName;

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    await _hubContext.Clients.All.OnDiscussionCommentEmit(discussionCommentAttachmentObj.DiscussionComment.DiscussionId);

                    return new OperationResult(true, System.Net.HttpStatusCode.OK, "Discussion comment file deleted successfully", Id);
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
        //download file
        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> Attachment(long Id)
        {
            var discussionCommentAttachmentObj = _discussionCommentAttachmentService.GetById(Id);

            // full path to file in temp location
            //var dirPath = _hostingEnvironment.WebRootPath + "\\DiscussionCommentUpload";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DiscussionCommentUploadDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (discussionCommentAttachmentObj != null)
            {
                filePath = dirPath + "\\" + discussionCommentAttachmentObj.FileName;
            }
            Byte[] newBytes = System.IO.File.ReadAllBytes(filePath);
            return File(newBytes, Common.GetMimeTypes(discussionCommentAttachmentObj.FileName), discussionCommentAttachmentObj.FileName);
        }
        
    }
}