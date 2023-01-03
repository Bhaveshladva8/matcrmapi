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
using System;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class OrganizationNoteController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IOrganizationNoteService _OrganizationNoteService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IOrganizationNotesCommentService _organizationNotesCommentService;
        private IMapper _mapper;
        private readonly IUserService _userSerVice;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private int UserId = 0;
        private int TenantId = 0;
        public OrganizationNoteController(
            ICustomerService customerService,
            IOrganizationNoteService OrganizationNoteService,
            ICustomerActivityService customerActivityService,
            IOrganizationNotesCommentService organizationNotesCommentService,
            IMapper mapper,
            IUserService userSerVice,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _customerService = customerService;
            _OrganizationNoteService = OrganizationNoteService;
            _customerActivityService = customerActivityService;
            _organizationNotesCommentService = organizationNotesCommentService;
            _mapper = mapper;
            _userSerVice = userSerVice;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<OrganizationNoteAddUpdateResponse>> AddUpdate([FromBody] OrganizationNoteAddUpdateRequest model)
        {
            var requestmodel = _mapper.Map<OrganizationNoteDto>(model);
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var OrganizationNoteObj = await _OrganizationNoteService.CheckInsertOrUpdate(requestmodel);

            await _hubContext.Clients.All.OnOrganizationNoteEventEmit(requestmodel.OrganizationId);
            var responseObj = _mapper.Map<OrganizationNoteAddUpdateResponse>(OrganizationNoteObj);
            return new OperationResult<OrganizationNoteAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<OrganizationNoteDeleteResponse>> Remove([FromBody] OrganizationNoteDeleteRequest model)
        {
            var requestmodel = _mapper.Map<OrganizationNoteDto>(model);

            var OrganizationNoteObj = _OrganizationNoteService.DeleteOrganizationNote(requestmodel);
            var responseObj = _mapper.Map<OrganizationNoteDeleteResponse>(OrganizationNoteObj);
            if (OrganizationNoteObj != null)
            {
                var notes = await _organizationNotesCommentService.DeleteCommentByNoteId(OrganizationNoteObj.Id);
                await _hubContext.Clients.All.OnOrganizationNoteEventEmit(requestmodel.OrganizationId);

                return new OperationResult<OrganizationNoteDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Note deleted successfully", responseObj);
            }
            else
            {
                return new OperationResult<OrganizationNoteDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Something went to wrong", responseObj);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{OrganizationId}")]
        public async Task<OperationResult<List<OrganizationNoteGetAllRespnse>>> List(long OrganizationId)
        {
            var OrganizationNoteList = _OrganizationNoteService.GetByOrganization(OrganizationId);
            var OrganizationNoteListDto = _mapper.Map<List<OrganizationNoteDto>>(OrganizationNoteList);
            var users = _userSerVice.GetAll();
            if (OrganizationNoteListDto != null && OrganizationNoteListDto.Count() > 0)
            {
                foreach (var item in OrganizationNoteListDto)
                {
                    if (item.Id != null)
                    {
                        var comments = _organizationNotesCommentService.GetAllByNoteId(item.Id.Value);
                        if (comments.Count() > 0)
                        {
                            item.Comments = _mapper.Map<List<OrganizationNotesCommentDto>>(comments);

                            if (item.Comments != null && item.Comments.Count() > 0)
                            {
                                foreach (var itemcomm in item.Comments)
                                {
                                    var userObj = users.Where(t => t.Id == itemcomm.CreatedBy).FirstOrDefault();
                                    if (userObj != null)
                                    {
                                        itemcomm.FirstName = userObj.FirstName;
                                        itemcomm.LastName = userObj.LastName;
                                        itemcomm.Email = userObj.Email;
                                        if (itemcomm.FirstName != null)
                                        {
                                            itemcomm.ShortName = itemcomm.FirstName.Substring(0, 1);
                                        }
                                        if (item.LastName != null)
                                        {
                                            itemcomm.ShortName = itemcomm.ShortName + itemcomm.LastName.Substring(0, 1);
                                        }
                                    }

                                }
                            }
                        }
                    }

                    if (users != null)
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
            var responselistDto = _mapper.Map<List<OrganizationNoteGetAllRespnse>>(OrganizationNoteListDto);
            return new OperationResult<List<OrganizationNoteGetAllRespnse>>(true, System.Net.HttpStatusCode.OK, "", responselistDto);
        }

    }
}