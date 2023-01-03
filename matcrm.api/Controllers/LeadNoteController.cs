using System.Collections.Generic;
using System.Linq;
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
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class LeadNoteController : Controller
    {
        private readonly ILeadService _leadService;
        private readonly ILeadNoteService _leadNoteService;
        // private readonly ILeadActivityService _leadActivityService;
        private IMapper _mapper;
        private readonly IUserService _userSerVice;
        private int UserId = 0;
        private int TenantId = 0;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;

        public LeadNoteController(
            ILeadService leadService,
            ILeadNoteService leadNoteService,
            // ILeadActivityService LeadActivityService,
            IMapper mapper,
            IUserService userSerVice,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _leadService = leadService;
            _leadNoteService = leadNoteService;
            // _leadActivityService = leadActivityService;
            _mapper = mapper;
            _userSerVice = userSerVice;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<LeadNoteAddUpdateResponse>> AddUpdate([FromBody] LeadNoteAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<LeadNoteDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var leadNoteObj = _leadNoteService.CheckInsertOrUpdate(requestmodel);

            await _hubContext.Clients.All.OnLeadNoteEventEmit(requestmodel.LeadId);
            var responseLeadNoteObj = _mapper.Map<LeadNoteAddUpdateResponse>(leadNoteObj);
            return new OperationResult<LeadNoteAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseLeadNoteObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<LeadNoteDeleteResponse>> Remove([FromBody] LeadNoteDeleteRequest model)
        {
            var requestmodel = _mapper.Map<LeadNoteDto>(model);
            var leadNoteObj = _leadNoteService.DeleteLeadNote(requestmodel);
            var resposeleadnoteObj = _mapper.Map<LeadNoteDeleteResponse>(leadNoteObj);
            if (leadNoteObj != null)
            {
                if (requestmodel.LeadId != null)
                {
                    await _hubContext.Clients.All.OnLeadNoteEventEmit(requestmodel.LeadId);
                    return new OperationResult<LeadNoteDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Note deleted successfully", resposeleadnoteObj);
                }
                else
                {
                    return new OperationResult<LeadNoteDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id", resposeleadnoteObj);
                }
            }
            else
            {
                return new OperationResult<LeadNoteDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Something went to wrong", resposeleadnoteObj);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        //[HttpGet("GetAll")]
        [HttpGet("{LeadId}")]
        public async Task<OperationResult<List<LeadNoteGetAllResponse>>> List(long LeadId)
        {
            var leadNoteList = _leadNoteService.GetByLead(LeadId);
            var users = _userSerVice.GetAll();
            var leadNoteDtoList = _mapper.Map<List<LeadNoteDto>>(leadNoteList);
            if (leadNoteDtoList != null && leadNoteDtoList.Count() > 0)
            {
                foreach (var item in leadNoteDtoList)
                {
                    if (users != null)
                    {
                        if (item.CreatedBy != null)
                        {
                            var userObj = users.Where(t => t.Id == item.CreatedBy).FirstOrDefault();
                            if (userObj != null)
                            {
                                item.FirstName = userObj.FirstName;
                                item.LastName = userObj.LastName;
                                item.Email = userObj.Email;
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
            }
            var responseLeadNoteDtoList = _mapper.Map<List<LeadNoteGetAllResponse>>(leadNoteDtoList);
            return new OperationResult<List<LeadNoteGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseLeadNoteDtoList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<LeadNoteDto>> Detail(long Id)
        {
            LeadNoteDto leadNoteDto = new LeadNoteDto();
            var leadNoteObj = _leadNoteService.GetById(Id);
            leadNoteDto = _mapper.Map<LeadNoteDto>(leadNoteObj);
            return new OperationResult<LeadNoteDto>(true, System.Net.HttpStatusCode.OK, "", leadNoteDto);
        }

    }
}