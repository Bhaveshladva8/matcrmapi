using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
     [Route("[controller]/[action]")]
    public class OrganizationNotesCommentController : Controller
    {
        private readonly IOrganizationNoteService _OrganizationNoteService;
        private readonly IOrganizationNotesCommentService _OrganizationNotesCommentService;
        // private readonly ITaskActivityService _taskActivityService;
        private IMapper _mapper;        
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;

        public OrganizationNotesCommentController(
            IOrganizationNoteService OrganizationNoteService,
            IOrganizationNotesCommentService OrganizationNotesCommentService,
            IMapper mapper,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _OrganizationNoteService = OrganizationNoteService;
            _OrganizationNotesCommentService = OrganizationNotesCommentService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OrganizationNotesCommentAddUpdateResponse>> AddUpdate([FromBody] OrganizationNotesCommentAddUpdateRequest model)
        {
            var requestmodel = _mapper.Map<OrganizationNotesCommentDto>(model);
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if(requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            var OrganizationNotesCommentObj = await _OrganizationNotesCommentService.CheckInsertOrUpdate(requestmodel);
            // Log in to SubTask Activity table 

            // TaskActivity taskActivityObj = new TaskActivity ();
            // taskActivityObj.TaskId = model.TaskId;
            // taskActivityObj.UserId = model.UserId;

            // if (model.Id == null) {
            //     taskActivityObj.Activity = "Created the comment";
            // } else {
            //     taskActivityObj.Activity = "Updated the comment";
            // }
            // var AddUpdate = _taskActivityService.CheckInsertOrUpdate (taskActivityObj);
            await _hubContext.Clients.All.OnOrganizationNoteEventEmit(requestmodel.OrganizationId);
            var OrganizationNotesCommentDto = _mapper.Map<OrganizationNotesCommentDto>(OrganizationNotesCommentObj);
            var responseDto = _mapper.Map<OrganizationNotesCommentAddUpdateResponse>(OrganizationNotesCommentDto);
            return new OperationResult<OrganizationNotesCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK,"", responseDto);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OrganizationNotesCommentDeleteResponse>> Remove([FromBody] OrganizationNotesCommentDeleteRequest model)
        {
            var requestmodel = _mapper.Map<OrganizationNotesCommentDto>(model);
            OrganizationNotesCommentDto OrganizationNotesCommentDto = new OrganizationNotesCommentDto();
            OrganizationNotesCommentDeleteResponse responseDto = new OrganizationNotesCommentDeleteResponse();
            var OrganizationNotesCommentObj = _OrganizationNotesCommentService.DeleteOrganizationNotesComment(requestmodel.Id.Value);
            if (OrganizationNotesCommentObj != null)
            {
                // TaskActivity taskActivityObj = new TaskActivity ();
                // taskActivityObj.TaskId = model.TaskId;
                // taskActivityObj.UserId = model.UserId;
                // taskActivityObj.Activity = "Removed the comment";
                // var AddUpdate = _taskActivityService.CheckInsertOrUpdate (taskActivityObj);
                await _hubContext.Clients.All.OnOrganizationNoteEventEmit(requestmodel.OrganizationId);
                OrganizationNotesCommentDto = _mapper.Map<OrganizationNotesCommentDto>(OrganizationNotesCommentObj);
                responseDto = _mapper.Map<OrganizationNotesCommentDeleteResponse>(OrganizationNotesCommentDto);
                return new OperationResult<OrganizationNotesCommentDeleteResponse>(true, System.Net.HttpStatusCode.OK,"Comment deleted successfully", responseDto);
            }
            else
            {
                responseDto = _mapper.Map<OrganizationNotesCommentDeleteResponse>(OrganizationNotesCommentDto);
                return new OperationResult<OrganizationNotesCommentDeleteResponse>(false, System.Net.HttpStatusCode.OK,"Something went to wrong", responseDto);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{NoteId}")]
        public async Task<OperationResult<List<OrganizationNotesCommentListResponse>>> List(long NoteId)
        {
            var organizationNotesCommentList = _OrganizationNotesCommentService.GetAllByNoteId(NoteId);
            var organizationNotesCommentDtos = _mapper.Map<List<OrganizationNotesCommentDto>>(organizationNotesCommentList);
            var responseCommentList = _mapper.Map<List<OrganizationNotesCommentListResponse>>(organizationNotesCommentList);
            return new OperationResult<List<OrganizationNotesCommentListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseCommentList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<OrganizationNotesCommentDto>> Detail(long Id)
        {
            var OrganizationNotesCommentObj = _OrganizationNotesCommentService.GetOrganizationNotesCommenttById(Id);
            var OrganizationNotesCommentDto = _mapper.Map<OrganizationNotesCommentDto>(OrganizationNotesCommentObj);
            return new OperationResult<OrganizationNotesCommentDto>(true, "", OrganizationNotesCommentDto);
        }

    }
}