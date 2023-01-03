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
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CustomerNotesCommentController : Controller
    {
        private readonly ICustomerNoteService _customerNoteService;
        private readonly ICustomerNotesCommentService _customerNotesCommentService;
        private IMapper _mapper;
        private readonly IUserService _userSerVice;
        private int UserId = 0;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;

        public CustomerNotesCommentController(
            ICustomerNoteService customerNoteService,
            ICustomerNotesCommentService customerNotesCommentService,
            IMapper mapper,
            IUserService userSerVice,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _customerNoteService = customerNoteService;
            _customerNotesCommentService = customerNotesCommentService;
            _mapper = mapper;
            _userSerVice = userSerVice;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomerNotesCommentAddUpdateResponse>> Add([FromBody] CustomerNotesCommentAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<CustomerNotesCommentDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            var customerNotesCommentObj = await _customerNotesCommentService.CheckInsertOrUpdate(requestmodel);
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
            await _hubContext.Clients.All.OnCustomerNoteEventEmit(requestmodel.CustomerId);
            var customerNotesCommentDto = _mapper.Map<CustomerNotesCommentDto>(customerNotesCommentObj);
            var responseCommentDto = _mapper.Map<CustomerNotesCommentAddUpdateResponse>(customerNotesCommentDto);
            responseCommentDto.CustomerId = requestmodel.CustomerId;
            return new OperationResult<CustomerNotesCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseCommentDto);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CustomerNotesCommentAddUpdateResponse>> Update([FromBody] CustomerNotesCommentAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<CustomerNotesCommentDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            var customerNotesCommentObj = await _customerNotesCommentService.CheckInsertOrUpdate(requestmodel);
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
            await _hubContext.Clients.All.OnCustomerNoteEventEmit(requestmodel.CustomerId);
            var customerNotesCommentDto = _mapper.Map<CustomerNotesCommentDto>(customerNotesCommentObj);
            var responseCommentDto = _mapper.Map<CustomerNotesCommentAddUpdateResponse>(customerNotesCommentDto);
            return new OperationResult<CustomerNotesCommentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseCommentDto);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CustomerNotesCommentdeleteResponse>> Remove([FromBody] CustomerNotesCommentdeleteRequest model)
        {
            CustomerNotesCommentDto customerNotesCommentDto = new CustomerNotesCommentDto();
            CustomerNotesCommentdeleteResponse customerNotesCommentdeleteResponseObj = new CustomerNotesCommentdeleteResponse();
            var requestmodel = _mapper.Map<CustomerNotesCommentDto>(model);
            if (requestmodel.Id != null)
            {
                var customerNotesCommentObj = await _customerNotesCommentService.DeleteCustomerNotesComment(requestmodel.Id.Value);
                if (customerNotesCommentObj != null)
                {
                    // TaskActivity taskActivityObj = new TaskActivity ();
                    // taskActivityObj.TaskId = model.TaskId;
                    // taskActivityObj.UserId = model.UserId;
                    // taskActivityObj.Activity = "Removed the comment";
                    // var AddUpdate = _taskActivityService.CheckInsertOrUpdate (taskActivityObj);
                    await _hubContext.Clients.All.OnCustomerNoteEventEmit(requestmodel.CustomerId);
                    customerNotesCommentDto = _mapper.Map<CustomerNotesCommentDto>(customerNotesCommentObj);
                    customerNotesCommentdeleteResponseObj = _mapper.Map<CustomerNotesCommentdeleteResponse>(customerNotesCommentDto);
                    return new OperationResult<CustomerNotesCommentdeleteResponse>(true, System.Net.HttpStatusCode.OK, "Comment deleted successfully", customerNotesCommentdeleteResponseObj);
                }
                else
                {
                    return new OperationResult<CustomerNotesCommentdeleteResponse>(false, System.Net.HttpStatusCode.OK, "Something went to wrong", customerNotesCommentdeleteResponseObj);
                }
            }
            else
            {
                return new OperationResult<CustomerNotesCommentdeleteResponse>(false, System.Net.HttpStatusCode.OK, "Id is null", customerNotesCommentdeleteResponseObj);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{NoteId}")]
        public async Task<OperationResult<List<CustomerNotesCommentListResponse>>> List(long NoteId)
        {
            var customerNotesCommentList = _customerNotesCommentService.GetAllByNoteId(NoteId);
            var customerNotesCommentListDto = _mapper.Map<List<CustomerNotesCommentDto>>(customerNotesCommentList);
            // if (customerNotesCommentListDto != null && customerNotesCommentListDto.Count > 0)
            // {
            //     foreach (var item in customerNotesCommentListDto)
            //     {
            //         var userObj = customerNotesCommentList.Where(t => t.User.Id == item.CreatedBy).FirstOrDefault();
            //         if (userObj != null)
            //         {
            //             item.FirstName = userObj.User.FirstName;
            //             item.LastName = userObj.User.LastName;
            //             item.Email = userObj.User.Email;
            //             if (item.FirstName != null)
            //             {
            //                 item.ShortName = item.FirstName.Substring(0, 1);
            //             }
            //             if (item.LastName != null)
            //             {
            //                 item.ShortName = item.ShortName + item.LastName.Substring(0, 1);
            //             }
            //         }

            //     }
            // }
            var responseCommentList = _mapper.Map<List<CustomerNotesCommentListResponse>>(customerNotesCommentListDto);
            return new OperationResult<List<CustomerNotesCommentListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseCommentList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CustomerNotesCommentDto>> Detail(long Id)
        {
            var customerNotesCommentObj = _customerNotesCommentService.GetCustomerNotesCommenttById(Id);
            var customerNotesCommentDto = _mapper.Map<CustomerNotesCommentDto>(customerNotesCommentObj);
            return new OperationResult<CustomerNotesCommentDto>(true, System.Net.HttpStatusCode.OK, "", customerNotesCommentDto);
        }

    }
}