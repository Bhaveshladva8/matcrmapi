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
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class DiscussionReadController : Controller
    {

        private readonly IDiscussionService _discussionService;
        private readonly IDiscussionReadService _discussionReadService;
        private readonly IDiscussionParticipantService _discussionParticipantService;
        private readonly IDiscussionCommentAttachmentService _DiscussionCommentAttachmentService;
        private readonly IDiscussionCommentService _discussionCommentService;
        private readonly IUserService _userService;
        private IMapper _mapper;
        private int UserId = 0;
        public DiscussionReadController(IDiscussionService discussionService,
        IDiscussionReadService discussionReadService,
        IDiscussionParticipantService discussionParticipantService,
        IDiscussionCommentAttachmentService DiscussionCommentAttachmentService,
        IDiscussionCommentService discussionCommentService,
        IUserService userService,
        IMapper mapper)
        {
            _discussionService = discussionService;
            _discussionReadService = discussionReadService;
            _discussionParticipantService = discussionParticipantService;
            _DiscussionCommentAttachmentService = DiscussionCommentAttachmentService;
            _discussionCommentService = discussionCommentService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<DiscussionReadMarkAsReadResponse>> MarkAsRead([FromBody] DiscussionReadMarkAsReadRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            DiscussionDto discussionDto = new DiscussionDto();
            var requestmodel = _mapper.Map<DiscussionReadDto>(model);
            requestmodel.ReadBy = UserId;
            if(requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            var discussionReadObj = await _discussionReadService.CheckInsertOrUpdate(requestmodel);

            requestmodel = _mapper.Map<DiscussionReadDto>(discussionReadObj);
            var responsemodel = _mapper.Map<DiscussionReadMarkAsReadResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<DiscussionReadMarkAsReadResponse>(true, System.Net.HttpStatusCode.OK,"Updated successfully", responsemodel);
            return new OperationResult<DiscussionReadMarkAsReadResponse>(false, System.Net.HttpStatusCode.OK,"Added successfully.", responsemodel);
        }

        [HttpPost]
        public async Task<OperationResult<DiscussionReadMarkAsUnReadResponse>> MarkAsUnRead([FromBody] DiscussionReadMarkAsUnReadRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<DiscussionReadDto>(model);
            requestmodel.ReadBy = UserId;
            if (requestmodel.DiscussionId != null)
            {
                var discussionReadObj = _discussionReadService.GetDiscussionByUserAndDiscussionId(requestmodel.DiscussionId.Value, UserId);
                if (discussionReadObj != null)
                {
                    discussionReadObj = await _discussionReadService.Delete(discussionReadObj.Id);                    
                }
                requestmodel = _mapper.Map<DiscussionReadDto>(discussionReadObj);
            }            
            var responsemodel = _mapper.Map<DiscussionReadMarkAsUnReadResponse>(requestmodel);
            if (responsemodel!= null && requestmodel.Id > 0)
                return new OperationResult<DiscussionReadMarkAsUnReadResponse>(true, System.Net.HttpStatusCode.OK,"Updated successfully", responsemodel);
            return new OperationResult<DiscussionReadMarkAsUnReadResponse>(false, System.Net.HttpStatusCode.OK,"Added successfully.", responsemodel);
        }
    }
}