using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using matcrm.service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using matcrm.service.Common;
using matcrm.data.Models.Response;
using matcrm.data.Models.Request;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Tables;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MateProjectTicketController : Controller
    {
        private readonly IMateProjectTicketService _mateProjectTicketService;
        private readonly IMateClientTicketService _mateClientTicketService;
        private readonly IStatusService _statusService;
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        private readonly IMateTicketUserService _mateTicketUserService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public MateProjectTicketController(IMateProjectTicketService mateProjectTicketService,
        IStatusService statusService,
        IUserService userService,
        IMateClientTicketService mateClientTicketService,
        IClientService clientService,
        IMateTicketUserService mateTicketUserService,
        IMapper mapper)
        {
            _mateProjectTicketService = mateProjectTicketService;
            _mateClientTicketService = mateClientTicketService;
            _statusService = statusService;
            _userService = userService;
            _clientService = clientService;
            _mateTicketUserService = mateTicketUserService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        [SwaggerOperation(Description = "This api is use for Project Ticket Tab")]
        public async Task<OperationResult<List<MateProjectTicketListResponse>>> TicketList([FromBody] MateProjectTicketListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            int totalCount = 0;
            List<MateProjectTicketListResponse> ticketResponseList = new List<MateProjectTicketListResponse>();
            if (requestModel.EmployeeProjectId != null)
            {
                var ticketList = _mateProjectTicketService.GetAllByProjectId(requestModel.EmployeeProjectId.Value, TenantId);
                totalCount = ticketList.Count();

                var AllUsers = _userService.GetAll();
                var AllStatus = _statusService.GetByTenant(TenantId);

                if (ticketList != null && ticketList.Count() > 0)
                {
                    foreach (var item in ticketList)
                    {
                        MateProjectTicketListResponse ticketListObj = new MateProjectTicketListResponse();
                        if (item.MateTicket != null)
                        {
                            if (item.MateTicketId != null)
                            {
                                ticketListObj.Id = item.MateTicketId.Value;
                            }
                            ticketListObj.TicketNo = item.MateTicket.TicketNo;
                            ticketListObj.Name = item.MateTicket.Name;
                            if (item.MateTicket.StatusId != null)
                            {
                                var statusObj = AllStatus.Where(t => t.Id == item.MateTicket?.StatusId).FirstOrDefault();
                                if (statusObj != null)
                                {
                                    ticketListObj.StatusId = item.MateTicket.StatusId;
                                    ticketListObj.Status = statusObj.Name;
                                }
                            }                            
                            var mateClientTicketObj = _mateClientTicketService.GetByTicketId(ticketListObj.Id);
                            if (mateClientTicketObj != null && mateClientTicketObj.ClientId != null)
                            {
                                var clientObj = _clientService.GetById(mateClientTicketObj.ClientId.Value);
                                if (clientObj != null)
                                {
                                    ticketListObj.ClientId = clientObj.Id;
                                    ticketListObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                                }
                            }
                            ticketListObj.CreatedOn = item.MateTicket?.CreatedOn;
                            var assignTaskUsers = _mateTicketUserService.GetByTicketId(ticketListObj.Id);
                            if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                            {
                                List<MateProjectTicketUserListResponse> assignTaskUserVMList = new List<MateProjectTicketUserListResponse>();

                                foreach (var assignTaskUserObj in assignTaskUsers)
                                {
                                    MateProjectTicketUserListResponse ticketUserListResponseObj = new MateProjectTicketUserListResponse();
                                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                    if (AllUsers != null)
                                    {
                                        var userObj2 = AllUsers.Where(t => t.Id == assignTaskUserObj.UserId).FirstOrDefault();
                                        if (userObj2 != null)
                                        {
                                            ticketUserListResponseObj.UserId = assignTaskUserObj.UserId;
                                            if (userObj2.ProfileImage == null)
                                            {
                                                ticketUserListResponseObj.ProfileURL = null;
                                            }
                                            else
                                            {
                                                ticketUserListResponseObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj2.Id + "?" + Timestamp;
                                            }
                                        }
                                        assignTaskUserVMList.Add(ticketUserListResponseObj);
                                    }
                                }
                                ticketListObj.AssignedUsers = assignTaskUserVMList;
                            }
                            ticketResponseList.Add(ticketListObj);
                        }
                    }
                }
            }
            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            if (!string.IsNullOrEmpty(requestModel.SearchString))
            {
                ticketResponseList = ticketResponseList.Where(t => (!string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.ClientName) && t.ClientName.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.Status) && t.Status.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                ticketResponseList = ticketResponseList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            else
            {
                ticketResponseList = ticketResponseList.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }

            // var pagedTicketList = taskList.Skip((requestModel.PageNumber - 1) * requestModel.PageSize).Take(requestModel.PageSize).ToList();

            ticketResponseList = ShortTaskByColumn(requestModel.ShortColumnName, requestModel.SortType, ticketResponseList);
            return new OperationResult<List<MateProjectTicketListResponse>>(true, System.Net.HttpStatusCode.OK, "", ticketResponseList, totalCount);
        }

        private List<MateProjectTicketListResponse> ShortTaskByColumn(string ShortColumn, string ShortOrder, List<MateProjectTicketListResponse> ticketList)
        {
            List<MateProjectTicketListResponse> ticketVMList = new List<MateProjectTicketListResponse>();
            ticketVMList = ticketList;
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn.ToLower() == "name")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t.Name).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t.Name).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "createdon")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t.CreatedOn).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t.CreatedOn).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "status")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t?.Status).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t?.Status).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "clientname")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ticketVMList = ticketList.OrderBy(t => t?.ClientName).ToList();
                    }
                    else
                    {
                        ticketVMList = ticketList.OrderByDescending(t => t?.ClientName).ToList();
                    }
                }
                else
                {
                    ticketVMList = ticketList.OrderByDescending(t => t.CreatedOn).ToList();
                }
            }

            return ticketVMList;
        }

    }
}