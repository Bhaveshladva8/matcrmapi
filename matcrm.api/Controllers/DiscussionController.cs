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
using matcrm.data.Models.Response;
using matcrm.data.Models.Request;
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class DiscussionController : Controller
    {

        private readonly IDiscussionService _discussionService;
        private readonly IDiscussionReadService _discussionReadService;
        private readonly IDiscussionParticipantService _discussionParticipantService;
        private readonly IDiscussionCommentAttachmentService _DiscussionCommentAttachmentService;
        private readonly IDiscussionCommentService _discussionCommentService;
        private readonly ITeamInboxService _teamInboxService;
        private readonly IMailBoxTeamService _mailBoxTeamService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IUserService _userService;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public DiscussionController(IDiscussionService discussionService,
        IDiscussionReadService discussionReadService,
        IDiscussionParticipantService discussionParticipantService,
        IDiscussionCommentAttachmentService DiscussionCommentAttachmentService,
        IDiscussionCommentService discussionCommentService,
        IMailBoxTeamService mailBoxTeamService,
        IUserService userService,
        ITeamInboxService teamInboxService,
        IHostingEnvironment hostingEnvironment,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        OneClappContext context,
        IMapper mapper)
        {
            _discussionService = discussionService;
            _discussionReadService = discussionReadService;
            _discussionParticipantService = discussionParticipantService;
            _DiscussionCommentAttachmentService = DiscussionCommentAttachmentService;
            _discussionCommentService = discussionCommentService;
            _teamInboxService = teamInboxService;
            _mailBoxTeamService = mailBoxTeamService;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<DiscussionAddUpdateResponse>> AddUpdate([FromForm] DiscussionAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<DiscussionDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var discussionObj = await _discussionService.CheckInsertOrUpdate(requestmodel, "");
            DiscussionComment? discussionCommentObj = null;

            if (!string.IsNullOrEmpty(requestmodel.Note))
            {
                DiscussionCommentDto discussionCommentDto = new DiscussionCommentDto();
                discussionCommentDto.Comment = requestmodel.Note;
                discussionCommentDto.DiscussionId = discussionObj.Id;
                discussionCommentDto.TeamMemberId = UserId;
                discussionCommentDto.CreatedBy = UserId;
                discussionCommentObj = await _discussionCommentService.CheckInsertOrUpdate(discussionCommentDto, "");
            }

            if (requestmodel.ToTeamMateIds != null && requestmodel.ToTeamMateIds.Count() > 0)
            {
                foreach (var item in requestmodel.ToTeamMateIds)
                {
                    DiscussionParticipantDto discussionParticipantDto = new DiscussionParticipantDto();
                    discussionParticipantDto.TeamMemberId = item;
                    discussionParticipantDto.DiscussionId = discussionObj.Id;
                    discussionParticipantDto.CreatedBy = UserId;

                    var AddUpdateParticipantObj = await _discussionParticipantService.CheckInsertOrUpdate(discussionParticipantDto);
                }
            }
            List<DiscussionCommentAttachment> discussionCommentAttachmentList = new List<DiscussionCommentAttachment>();

            if (requestmodel.FileList != null && requestmodel.FileList.Count() > 0)
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
                            return new OperationResult<DiscussionAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }

                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await file.CopyToAsync(oStream);
                    }

                    DiscussionCommentAttachmentDto discussionCommentAttachmentDto = new DiscussionCommentAttachmentDto();
                    discussionCommentAttachmentDto.FileName = fileName;
                    if (discussionCommentObj != null)
                    {
                        discussionCommentAttachmentDto.DiscussionCommentId = discussionCommentObj.Id;
                    }

                    discussionCommentAttachmentDto.CreatedBy = UserId;
                    var addedItem = await _DiscussionCommentAttachmentService.CheckInsertOrUpdate(discussionCommentAttachmentDto);
                    discussionCommentAttachmentList.Add(addedItem);
                }
            }

            requestmodel = _mapper.Map<DiscussionDto>(discussionObj);
            await _hubContext.Clients.All.OnMailModuleEvent(UserId, "discussion", "addupdate", discussionObj.Id.ToString());
            var responsemodel = _mapper.Map<DiscussionAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<DiscussionAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            return new OperationResult<DiscussionAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Added successfully.", responsemodel);
        }

        [HttpPost]
        public async Task<OperationResult<List<DiscussionGetAllResponse>>> List([FromBody] DiscussionGetAllRequest model)
        {
            List<DiscussionDto> discussionDtoList = new List<DiscussionDto>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<MailTokenDto>(model);
            List<Discussion> discussionList = new List<Discussion>();
            if (!string.IsNullOrEmpty(requestmodel.label) && requestmodel.label == "trash")
            {
                discussionList = _discussionService.GetTrashedByUser(UserId);
            }
            else if (!string.IsNullOrEmpty(requestmodel.label) && requestmodel.label == "open")
            {
                discussionList = _discussionService.GetByUser(UserId);
            }
            else
            {
                discussionList = _discussionService.GetAllByUser(UserId);
            }

            discussionDtoList = _mapper.Map<List<DiscussionDto>>(discussionList);
            if (discussionDtoList != null && discussionDtoList.Count() > 0)
            {
                foreach (var item in discussionDtoList)
                {
                    if (item.Id != null)
                    {
                        var discussionReadList = _discussionReadService.GetAllDiscussionRead(item.Id.Value);
                        var discussionReadObj = discussionReadList.Where(t => t.ReadBy == UserId).FirstOrDefault();
                        if (discussionReadObj != null)
                        {
                            item.IsRead = true;
                        }
                    }
                }
            }
            if (requestmodel.top == null)
            {
                requestmodel.top = 20;
            }
            if (requestmodel.skip == null)
            {
                requestmodel.skip = 0;
            }

            discussionDtoList = discussionDtoList.OrderByDescending(t => t.CreatedOn.Value).Skip(requestmodel.skip.Value).Take(requestmodel.top.Value).ToList();

            var resposnseDiscussionDtoList = _mapper.Map<List<DiscussionGetAllResponse>>(discussionDtoList);

            return new OperationResult<List<DiscussionGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", resposnseDiscussionDtoList);
        }


        [HttpGet("{DiscussionId}")]
        public async Task<OperationResult<GetDiscussionResponse>> Detail(long DiscussionId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var userList = _userService.GetAll();
            // var DiscussionObj = _discussionService.GetById(DiscussionId);
            var discussionObj = _discussionService.GetDiscussion(DiscussionId);
            DiscussionDto discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
            List<DiscussionComment> discussionCommentList = _discussionCommentService.GetAllCommentByDiscussionId(DiscussionId);
            List<DiscussionRead> discussionReadList = _discussionReadService.GetAllDiscussionRead(DiscussionId);
            discussionDto.Reads = _mapper.Map<List<DiscussionReadDto>>(discussionReadList);
            if (discussionCommentList != null && discussionCommentList.Count() > 0)
            {
                discussionDto.Comments = _mapper.Map<List<DiscussionCommentDto>>(discussionCommentList);
            }

            List<DiscussionParticipant> discussionParticipantList = _discussionParticipantService.GetAllDiscussionParticipant(DiscussionId);
            discussionDto.Participants = _mapper.Map<List<DiscussionParticipantDto>>(discussionParticipantList);

            if (discussionDto.Comments != null && discussionDto.Comments.Count() > 0)
            {
                foreach (var commentObj in discussionDto.Comments)
                {
                    if (commentObj.TeamMemberId != null)
                    {
                        if (userList != null)
                        {
                            var teamMemberObj = userList.Where(t => t.Id == commentObj.TeamMemberId).FirstOrDefault();
                            if (teamMemberObj != null)
                            {
                                commentObj.FirstName = teamMemberObj.FirstName;
                                commentObj.LastName = teamMemberObj.LastName;
                                commentObj.Email = teamMemberObj.Email;
                                if (teamMemberObj.FirstName != null)
                                {
                                    commentObj.ShortName = teamMemberObj.FirstName.Substring(0, 1);
                                }
                                if (teamMemberObj.LastName != null)
                                {
                                    commentObj.ShortName = commentObj.ShortName + teamMemberObj.LastName.Substring(0, 1);
                                }
                            }
                        }
                        if (commentObj.Id != null)
                        {
                            var attachments = _DiscussionCommentAttachmentService.GetAllByDiscussionCommentId(commentObj.Id.Value);

                            commentObj.Attachments = _mapper.Map<List<DiscussionCommentAttachmentDto>>(attachments);
                        }
                    }
                }
            }

            if (discussionDto.Reads != null && discussionDto.Reads.Count() > 0)
            {
                foreach (var readObj in discussionDto.Reads)
                {
                    if (readObj.User != null)
                    {
                        readObj.FirstName = readObj.User.FirstName;
                        readObj.LastName = readObj.User.LastName;
                    }
                    if (readObj.ReadBy == UserId)
                    {
                        discussionDto.IsRead = true;
                    }
                }
            }
            var responsedto = _mapper.Map<GetDiscussionResponse>(discussionDto);
            return new OperationResult<GetDiscussionResponse>(true, System.Net.HttpStatusCode.OK, "", responsedto);
        }


        [HttpPost]
        public async Task<OperationResult<DiscussionAssignUserResponse>> AssignUser([FromBody] DiscussionAssignUserRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<DiscussionDto>(model);
            if (requestmodel.Id != null)
            {
                var discussionObj = _discussionService.GetById(requestmodel.Id.Value);
                var discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
                discussionDto.UpdatedBy = UserId;
                discussionDto.TenantId = TenantId;
                discussionDto.AssignUserId = requestmodel.AssignUserId;
                discussionObj = await _discussionService.CheckInsertOrUpdate(discussionDto, "Assign");
                requestmodel = _mapper.Map<DiscussionDto>(discussionObj);
                var reponsemodel = _mapper.Map<DiscussionAssignUserResponse>(requestmodel);
                await _hubContext.Clients.All.OnMailModuleEvent(requestmodel.AssignUserId.Value, "discussion", "assignuser", discussionObj.Id.ToString());
                return new OperationResult<DiscussionAssignUserResponse>(true, System.Net.HttpStatusCode.OK, "Assigned", reponsemodel);

            }
            else
            {
                var reponsemodel = _mapper.Map<DiscussionAssignUserResponse>(requestmodel);
                return new OperationResult<DiscussionAssignUserResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", reponsemodel);
            }
        }

        [HttpPost]
        public async Task<OperationResult<DiscussionAssignCustomerResponse>> AssignCustomer([FromBody] DiscussionAssignCustomerRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<DiscussionDto>(model);
            if (requestmodel.Id != null)
            {
                var discussionObj = _discussionService.GetById(requestmodel.Id.Value);
                var discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
                discussionDto.UpdatedBy = UserId;
                discussionDto.CustomerId = requestmodel.CustomerId;
                discussionDto.TenantId = TenantId;
                discussionObj = await _discussionService.CheckInsertOrUpdate(discussionDto, "AssignCustomer");
                requestmodel = _mapper.Map<DiscussionDto>(discussionObj);
                // await _hubContext.Clients.All.OnMailModuleEvent(model.AssignUserId.Value, "discussion", "assignuser", discussionObj.Id.ToString());
                var responsemodel = _mapper.Map<DiscussionAssignCustomerResponse>(requestmodel);
                return new OperationResult<DiscussionAssignCustomerResponse>(true, System.Net.HttpStatusCode.OK, "Assigned", responsemodel);

            }
            else
            {
                var responsemodel = _mapper.Map<DiscussionAssignCustomerResponse>(requestmodel);
                return new OperationResult<DiscussionAssignCustomerResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", responsemodel);
            }
        }

        [HttpDelete]
        public async Task<OperationResult<DiscussionUnAssignCustResponse>> UnAssignCustomer([FromBody] DiscussionUnAssignCustRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<DiscussionDto>(model);
            if (requestmodel.Id != null)
            {
                var discussionObj = _discussionService.GetById(requestmodel.Id.Value);
                // var assignUserId = 0;
                // if (discussionObj.AssignUserId != null)
                // {
                //     assignUserId = discussionObj.AssignUserId.Value;
                // }

                var oldDiscussionData = discussionObj;
                var discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
                discussionDto.UpdatedBy = UserId;
                discussionDto.CustomerId = null;
                discussionDto.TenantId = TenantId;
                var discussionObj1 = await _discussionService.CheckInsertOrUpdate(discussionDto, "UnAssignCustomer");
                requestmodel = _mapper.Map<DiscussionDto>(discussionObj1);
                // await _hubContext.Clients.All.OnMailModuleEvent(assignUserId, "discussion", "unassign", discussionObj.Id.ToString());
                var responsemodel = _mapper.Map<DiscussionUnAssignCustResponse>(requestmodel);
                return new OperationResult<DiscussionUnAssignCustResponse>(true, System.Net.HttpStatusCode.OK, "Unassigned", responsemodel);
            }
            else
            {
                var responsemodel = _mapper.Map<DiscussionUnAssignCustResponse>(requestmodel);
                return new OperationResult<DiscussionUnAssignCustResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", responsemodel);
            }
        }

        [HttpPost]
        public async Task<OperationResult<DiscussionParticipantResponse>> Participate([FromBody] DiscussionParticipantRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<DiscussionParticipantDto>(model);
            DiscussionParticipantResponse responsemodel = new DiscussionParticipantResponse();
            if (requestmodel.DiscussionId == null)
            {
                return new OperationResult<DiscussionParticipantResponse>(false, System.Net.HttpStatusCode.OK, "Please provide discussion id", responsemodel);
            }
            else if (requestmodel.TeamMemberId == null)
            {
                return new OperationResult<DiscussionParticipantResponse>(false, System.Net.HttpStatusCode.OK, "Please provide team member", responsemodel);
            }
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            var discussionParticipantObj = await _discussionParticipantService.CheckInsertOrUpdate(requestmodel);
            await _hubContext.Clients.All.OnMailModuleEvent(requestmodel.TeamMemberId.Value, "discussion", "shareduser", requestmodel.DiscussionId.Value.ToString());
            requestmodel = _mapper.Map<DiscussionParticipantDto>(discussionParticipantObj);
            responsemodel = _mapper.Map<DiscussionParticipantResponse>(requestmodel);
            return new OperationResult<DiscussionParticipantResponse>(true, System.Net.HttpStatusCode.OK, "Invited", responsemodel);
        }

        [HttpDelete]
        public async Task<OperationResult<DiscussionUnAssignResponse>> UnAssign([FromBody] DiscussionUnAssignRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<DiscussionDto>(model);
            if (requestmodel.Id != null)
            {
                var discussionObj = _discussionService.GetById(requestmodel.Id.Value);
                var assignUserId = 0;
                if (discussionObj.AssignUserId != null)
                {
                    assignUserId = discussionObj.AssignUserId.Value;
                }

                var oldDiscussionData = discussionObj;
                var discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
                discussionDto.UpdatedBy = UserId;
                discussionDto.AssignUserId = null;
                discussionDto.TenantId = TenantId;
                var discussionObj1 = await _discussionService.CheckInsertOrUpdate(discussionDto, "UnAssign");
                requestmodel = _mapper.Map<DiscussionDto>(discussionObj1);
                var responsemodel = _mapper.Map<DiscussionUnAssignResponse>(requestmodel);
                await _hubContext.Clients.All.OnMailModuleEvent(assignUserId, "discussion", "assignuser", discussionObj.Id.ToString());
                return new OperationResult<DiscussionUnAssignResponse>(true, System.Net.HttpStatusCode.OK, "UnAssigned", responsemodel);
            }
            else
            {
                var responsemodel = _mapper.Map<DiscussionUnAssignResponse>(requestmodel);
                return new OperationResult<DiscussionUnAssignResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", responsemodel);
            }
        }

        [HttpPost]
        public async Task<OperationResult<DiscussionDto>> MoveToTeam([FromBody] DiscussionDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (model.Id != null)
            {
                var discussionObj = _discussionService.GetById(model.Id.Value);
                var discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
                discussionDto.UpdatedBy = UserId;
                discussionDto.TeamInboxId = model.TeamInboxId;
                discussionDto.TenantId = TenantId;
                discussionObj = await _discussionService.CheckInsertOrUpdate(discussionDto, "");
                model = _mapper.Map<DiscussionDto>(discussionObj);
                return new OperationResult<DiscussionDto>(true, System.Net.HttpStatusCode.OK, "Moved", model);
            }
            else
                return new OperationResult<DiscussionDto>(false, System.Net.HttpStatusCode.OK, "Please provide id", model);
        }

        // [HttpPost]
        // public async Task<OperationResult<DiscussionTrashResponse>> Trash([FromBody] DiscussionTrashRequest model)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     var requestmodel = _mapper.Map<DiscussionDto>(model);
        //     if (requestmodel.Id != null)
        //     {
        //         var discussionObj = await _discussionService.Trash(requestmodel.Id.Value);
        //         var discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
        //         requestmodel = _mapper.Map<DiscussionDto>(discussionObj);
        //         await _hubContext.Clients.All.OnMailModuleEvent(null, "discussion", "trash", requestmodel.Id.Value.ToString());
        //         var responsemodel = _mapper.Map<DiscussionTrashResponse>(requestmodel);
        //         return new OperationResult<DiscussionTrashResponse>(true, System.Net.HttpStatusCode.OK,"Trashed successfully", responsemodel);
        //     }
        //     else
        //     {
        //         var responsemodel = _mapper.Map<DiscussionTrashResponse>(requestmodel);
        //         return new OperationResult<DiscussionTrashResponse>(false,System.Net.HttpStatusCode.OK, "Please provide id",responsemodel);
        //     }
        // }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Trash(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (Id != null && Id > 0)
            {
                var discussionObj = await _discussionService.Trash(Id);
                var discussionDto = _mapper.Map<DiscussionDto>(discussionObj);
                await _hubContext.Clients.All.OnMailModuleEvent(null, "discussion", "trash", Id.ToString());
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Trashed successfully", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }


        [HttpDelete]
        public async Task<OperationResult<DiscussionDto>> Remove([FromBody] DiscussionDto model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            DiscussionDto discussionDto = new DiscussionDto();
            if (model.Id != null)
            {
                var discussionReads = _discussionReadService.DeleteByDiscussion(model.Id.Value);
                var discussionComments = _discussionCommentService.DeleteByDiscussion(model.Id.Value);
                var discussionParticipants = _discussionParticipantService.DeleteByDiscussion(model.Id.Value);
                foreach (var discussionCommentObj in discussionComments)
                {
                    var discussionAttachments = _DiscussionCommentAttachmentService.DeleteAttachmentByDiscussionComment(discussionCommentObj.Id);
                    if (discussionAttachments != null && discussionAttachments.Count() > 0)
                    {
                        foreach (var discussionCommentAttachmentObj in discussionAttachments)
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
                }

                await _discussionService.Delete(model.Id.Value);

                await _hubContext.Clients.All.OnMailModuleEvent(null, "discussion", "delete", model.Id.Value.ToString());

                return new OperationResult<DiscussionDto>(true, System.Net.HttpStatusCode.OK, "Deleted successfully", discussionDto);
            }
            else
                return new OperationResult<DiscussionDto>(false, System.Net.HttpStatusCode.OK, "Please provide id", discussionDto);
        }

        [HttpGet]
        public async Task<OperationResult<List<DiscussionTeamMateListResponse>>> TeamMateList()
        {
            List<UserDto> userDtoList = new List<UserDto>();
            List<DiscussionTeamMateListResponse> responseUserList = new List<DiscussionTeamMateListResponse>();
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            int CreatedBy = UserId;
            var userObj = _userService.GetUserById(CreatedBy);
            if (userObj.CreatedBy != null)
            {
                CreatedBy = userObj.CreatedBy.Value;
            }

            var teamMates = _userService.GetAllTeamMate(CreatedBy);

            userDtoList = _mapper.Map<List<UserDto>>(teamMates);

            var mailBoxTeamObj = _mailBoxTeamService.GetByUser(CreatedBy);

            if (mailBoxTeamObj != null)
            {
                var teamInboxList = _teamInboxService.GetByTeam(mailBoxTeamObj.Id);
                foreach (var teamInboxObj in teamInboxList)
                {
                    UserDto userDto = new UserDto();
                    if (teamInboxObj.IntProviderAppSecret != null)
                    {
                        userDto.Email = teamInboxObj.IntProviderAppSecret.Email;
                        userDto.LastName = teamInboxObj.IntProviderAppSecret.Email;
                        userDto.FirstName = teamInboxObj.IntProviderAppSecret.Email;
                        userDto.MailBoxTeamId = mailBoxTeamObj.Id;
                        userDtoList.Add(userDto);
                    }

                }
            }
            responseUserList = _mapper.Map<List<DiscussionTeamMateListResponse>>(userDtoList);
            return new OperationResult<List<DiscussionTeamMateListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseUserList);
        }

    }
}