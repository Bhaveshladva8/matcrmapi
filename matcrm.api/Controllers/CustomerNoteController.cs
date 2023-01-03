using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CustomerNoteController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ICustomerNoteService _CustomerNoteService;
        private readonly ICustomerNotesCommentService _customerNotesCommentService;
        private readonly ICustomerActivityService _customerActivityService;
        private IMapper _mapper;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IUserService _userSerVice;
        private int UserId = 0;
        private int TenantId = 0;

        public CustomerNoteController(
            ICustomerService customerService,
            ICustomerNoteService CustomerNoteService,
            ICustomerNotesCommentService customerNotesCommentService,
            ICustomerActivityService customerActivityService,
            IMapper mapper,
            IUserService userSerVice,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            _customerService = customerService;
            _CustomerNoteService = CustomerNoteService;
            _customerNotesCommentService = customerNotesCommentService;
            _customerActivityService = customerActivityService;
            _mapper = mapper;
            _userSerVice = userSerVice;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomerNoteAddUpdateResponse>> Add([FromBody] CustomerNoteAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<CustomerNoteDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var customerNoteObj = await _CustomerNoteService.CheckInsertOrUpdate(requestmodel);

            await _hubContext.Clients.All.OnCustomerNoteEventEmit(requestmodel.CustomerId);
            var responseObj = _mapper.Map<CustomerNoteAddUpdateResponse>(customerNoteObj);
            return new OperationResult<CustomerNoteAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CustomerNoteAddUpdateResponse>> Update([FromBody] CustomerNoteAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<CustomerNoteDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            var customerNoteObj = await _CustomerNoteService.CheckInsertOrUpdate(requestmodel);

            await _hubContext.Clients.All.OnCustomerNoteEventEmit(requestmodel.CustomerId);
            var responseObj = _mapper.Map<CustomerNoteAddUpdateResponse>(customerNoteObj);
            return new OperationResult<CustomerNoteAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<CustomerNoteDeleteResponse>> Remove([FromBody] CustomerNoteDeleteRequest model)
        {
            var requestmodel = _mapper.Map<CustomerNoteDto>(model);
            if (requestmodel.Id != null)
            {
                var deletedNotesComment = await _customerNotesCommentService.DeleteCommentByNoteId(requestmodel.Id.Value);

                var customerNoteObj = _CustomerNoteService.DeleteCustomerNote(requestmodel);
                var responseCustomerNoteObj = _mapper.Map<CustomerNoteDeleteResponse>(customerNoteObj);
                if (customerNoteObj != null)
                {
                    await _hubContext.Clients.All.OnCustomerNoteEventEmit(requestmodel.CustomerId);
                    return new OperationResult<CustomerNoteDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Note deleted successfully", responseCustomerNoteObj);
                }
                else
                {
                    return new OperationResult<CustomerNoteDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Something went to wrong", responseCustomerNoteObj);
                }
            }
            else
            {
                return new OperationResult<CustomerNoteDeleteResponse>(false, System.Net.HttpStatusCode.OK, "please provide id");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{CustomerId}")]
        public async Task<OperationResult<List<CustomerNoteListResponse>>> List(long CustomerId)
        {
            var customerNotes = _CustomerNoteService.GetByCustomer(CustomerId);
            var customerNoteListDto = _mapper.Map<List<CustomerNoteDto>>(customerNotes);
            var users = _userSerVice.GetAll();
            if (customerNoteListDto != null && customerNoteListDto.Count() > 0)
            {
                foreach (var item in customerNoteListDto)
                {
                    if (item.Id != null)
                    {
                        var comments = _customerNotesCommentService.GetAllByNoteId(item.Id.Value);
                        if (comments != null && comments.Count() > 0)
                        {
                            item.Comments = _mapper.Map<List<CustomerNotesCommentDto>>(comments);

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

                            // var customerNotesCommentDtoList = _mapper.Map<List<CustomerNotesCommentDto>>(comments);

                            // if (customerNotesCommentDtoList != null && customerNotesCommentDtoList.Count() > 0)
                            // {
                            //     foreach (var itemcomm in customerNotesCommentDtoList)
                            //     {
                            //         var userObj = users.Where(t => t.Id == item.CreatedBy).FirstOrDefault();
                            //         if (userObj != null)
                            //         {
                            //             itemcomm.FirstName = userObj.FirstName;
                            //             itemcomm.LastName = userObj.LastName;
                            //             itemcomm.Email = userObj.Email;
                            //             if (itemcomm.FirstName != null)
                            //             {
                            //                 itemcomm.ShortName = item.FirstName.Substring(0, 1);
                            //             }
                            //             if (item.LastName != null)
                            //             {
                            //                 itemcomm.ShortName = item.ShortName + item.LastName.Substring(0, 1);
                            //             }
                            //         }

                            //     }
                            // }
                            
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
            var CustomerNoteListResponse = _mapper.Map<List<CustomerNoteListResponse>>(customerNoteListDto);
            return new OperationResult<List<CustomerNoteListResponse>>(true, System.Net.HttpStatusCode.OK, "", CustomerNoteListResponse);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CustomerNoteDto>> Detail(long Id)
        {
            CustomerNoteDto customerNoteDto = new CustomerNoteDto();
            var customerNoteObj = _CustomerNoteService.GetById(Id);
            customerNoteDto = _mapper.Map<CustomerNoteDto>(customerNoteObj);
            var comments = _customerNotesCommentService.GetAllByNoteId(customerNoteObj.Id);
            if (comments != null && comments.Count() > 0)
            {
                customerNoteDto.Comments = _mapper.Map<List<CustomerNotesCommentDto>>(comments);
            }
            return new OperationResult<CustomerNoteDto>(true, System.Net.HttpStatusCode.OK, "", customerNoteDto);
        }

    }
}