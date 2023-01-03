using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ClientAppointmentController : Controller
    {
        private readonly IClientAppointmentService _clientAppointmentService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ClientAppointmentController(IClientAppointmentService clientAppointmentService,
        IMapper mapper)
        {
            _clientAppointmentService = clientAppointmentService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<ClientAppointmentAddResponse>> Add([FromBody] ClientAppointmentAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var model = _mapper.Map<ClientAppointment>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var clientAppointmentObj = await _clientAppointmentService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ClientAppointmentAddResponse>(clientAppointmentObj);
            return new OperationResult<ClientAppointmentAddResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responseObj);
        }

        [HttpPut]
        public async Task<OperationResult<ClientAppointmentAddResponse>> Update([FromBody] ClientAppointmentAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var model = _mapper.Map<ClientAppointment>(requestmodel);
            if (model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            var clientAppointmentObj = await _clientAppointmentService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ClientAppointmentAddResponse>(clientAppointmentObj);
            return new OperationResult<ClientAppointmentAddResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", responseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            if (Id > 0)
            {
                var clientAppointmentObj = await _clientAppointmentService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        //[HttpGet("{ClientId}")]
        [HttpPost]
        public async Task<OperationResult<List<ClientAppointmentListResponse>>> List([FromBody] ClientAppointmentListRequest requestModel)
        {
            var clientAppointmentList = _clientAppointmentService.GetAllByClientId(requestModel.ClientId);

            List<ClientAppointmentListResponse> clientAppointmentListResponses = new List<ClientAppointmentListResponse>();
            if (clientAppointmentList != null && clientAppointmentList.Count > 0)
            {
                foreach (var item in clientAppointmentList)
                {
                    ClientAppointmentListResponse clientAppointmentObj = new ClientAppointmentListResponse();
                    clientAppointmentObj.Id = item.Id;
                    clientAppointmentObj.Description = item.Description;
                    clientAppointmentObj.StartDate = item.StartDate;
                    clientAppointmentObj.EndDate = item.EndDate;
                    if (item.ClientUser != null)
                    {
                        clientAppointmentObj.ClientUserName = item.ClientUser.FirstName + " " + item.ClientUser.LastName;
                    }
                    if (item.Status != null)
                    {
                        clientAppointmentObj.Status = item.Status.Name;
                    }
                    clientAppointmentListResponses.Add(clientAppointmentObj);
                }
            }
            int totalCount = 0;
            totalCount = clientAppointmentListResponses.Count();
            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            if (!string.IsNullOrEmpty(requestModel.SearchString))
            {
                clientAppointmentListResponses = clientAppointmentListResponses.Where(t => (!string.IsNullOrEmpty(t.Description) && t.Description.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.Status) && t.Status.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.ClientUserName) && t.ClientUserName.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                clientAppointmentListResponses = clientAppointmentListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            else
            {
                clientAppointmentListResponses = clientAppointmentListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            clientAppointmentListResponses = ShortByColumn(requestModel.ShortColumnName, requestModel.SortType, clientAppointmentListResponses);
            return new OperationResult<List<ClientAppointmentListResponse>>(true, System.Net.HttpStatusCode.OK, "", clientAppointmentListResponses, totalCount);
        }

        private List<ClientAppointmentListResponse> ShortByColumn(string ShortColumn, string ShortOrder, List<ClientAppointmentListResponse> ClientAppointmentList)
        {
            List<ClientAppointmentListResponse> ClientAppointmentListVMList = new List<ClientAppointmentListResponse>();
            ClientAppointmentListVMList = ClientAppointmentList;
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn.ToLower() == "description")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderBy(t => t.Description).ToList();
                    }
                    else
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderByDescending(t => t.Description).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "clientusername")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderBy(t => t.ClientUserName).ToList();
                    }
                    else
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderByDescending(t => t.ClientUserName).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "status")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderBy(t => t.Status).ToList();
                    }
                    else
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderByDescending(t => t.Status).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "startdate")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderBy(t => t.StartDate).ToList();
                    }
                    else
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderByDescending(t => t.StartDate).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "enddate")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderBy(t => t.EndDate).ToList();
                    }
                    else
                    {
                        ClientAppointmentListVMList = ClientAppointmentList.OrderByDescending(t => t.EndDate).ToList();
                    }
                }
                else
                {
                    ClientAppointmentListVMList = ClientAppointmentList.OrderByDescending(t => t.Id).ToList();
                }
            }
            return ClientAppointmentListVMList;
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<ClientAppointmentDetailResponse>> Detail(long Id)
        {
            var clientAppointmentObj = _clientAppointmentService.GetById(Id);
            var ResponseObj = _mapper.Map<ClientAppointmentDetailResponse>(clientAppointmentObj);
            if (clientAppointmentObj.ClientUserId != null)
            {
                ResponseObj.ClientUserName = clientAppointmentObj.ClientUser.FirstName + " " + clientAppointmentObj.ClientUser.LastName;
            }
            if (clientAppointmentObj.StatusId != null)
            {
                ResponseObj.Status = clientAppointmentObj.Status.Name;
            }
            return new OperationResult<ClientAppointmentDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }
    }
}