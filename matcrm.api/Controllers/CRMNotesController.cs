using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
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
    public class CRMNotesController : Controller
    {
        private readonly ICRMNotesService _cRMNotesService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public CRMNotesController(ICRMNotesService cRMNotesService,
        IMateTimeRecordService mateTimeRecordService,
        IMapper mapper)
        {
            _cRMNotesService = cRMNotesService;
            _mateTimeRecordService = mateTimeRecordService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<OperationResult<CRMNotesAddUpdateResponse>> Add([FromBody] CRMNotesAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<CRMNotes>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            CRMNotes cRMNotesObj = new CRMNotes();
            if (requestmodel.Duration != null)
            {
                MateTimeRecord mateTimeRecordObj = new MateTimeRecord();
                if (requestmodel.MateTimeRecordId != null)
                {
                    mateTimeRecordObj.Id = requestmodel.MateTimeRecordId.Value;
                }
                mateTimeRecordObj.UserId = UserId;
                mateTimeRecordObj.Comment = requestmodel.Note;
                mateTimeRecordObj.Duration = requestmodel.Duration;
                mateTimeRecordObj.IsBillable = requestmodel.IsBillable;
                mateTimeRecordObj.IsManual = true;
                var AddUpdatetimeRecordObj = await _mateTimeRecordService.CheckInsertOrUpdate(mateTimeRecordObj);
                if (AddUpdatetimeRecordObj != null)
                {
                    model.MateTimeRecordId = AddUpdatetimeRecordObj.Id;
                    cRMNotesObj = await _cRMNotesService.CheckInsertOrUpdate(model);
                }
            }
            var responseObj = _mapper.Map<CRMNotesAddUpdateResponse>(cRMNotesObj);
            return new OperationResult<CRMNotesAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responseObj);
        }

        [HttpPut]
        public async Task<OperationResult<CRMNotesAddUpdateResponse>> Update([FromBody] CRMNotesAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<CRMNotes>(requestmodel);
            if (model.Id == 0)
            {
                model.UpdatedBy = UserId;
            }
            CRMNotes cRMNotesObj = new CRMNotes();
            if (requestmodel.Duration != null)
            {
                MateTimeRecord mateTimeRecordObj = new MateTimeRecord();
                if (requestmodel.MateTimeRecordId != null)
                {
                    mateTimeRecordObj.Id = requestmodel.MateTimeRecordId.Value;
                }
                mateTimeRecordObj.UserId = UserId;
                mateTimeRecordObj.Comment = requestmodel.Note;
                mateTimeRecordObj.Duration = requestmodel.Duration;
                mateTimeRecordObj.IsBillable = requestmodel.IsBillable;
                mateTimeRecordObj.IsManual = true;
                var AddUpdatetimeRecordObj = await _mateTimeRecordService.CheckInsertOrUpdate(mateTimeRecordObj);
                if (AddUpdatetimeRecordObj != null)
                {
                    model.MateTimeRecordId = AddUpdatetimeRecordObj.Id;
                    cRMNotesObj = await _cRMNotesService.CheckInsertOrUpdate(model);
                }
            }
            var responseObj = _mapper.Map<CRMNotesAddUpdateResponse>(cRMNotesObj);
            return new OperationResult<CRMNotesAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", responseObj);
        }

        //[HttpGet("{ClientId}")]
        [HttpPost]
        public async Task<OperationResult<List<CRMNotesListResponse>>> List([FromBody] ClientAppointmentListRequest requestModel)
        {
            var cRMNotesList = _cRMNotesService.GetAllByClientId(requestModel.ClientId);
            List<CRMNotesListResponse> cRMNotesListResponses = new List<CRMNotesListResponse>();
            if (cRMNotesList != null && cRMNotesList.Count > 0)
            {
                foreach (var item in cRMNotesList)
                {
                    CRMNotesListResponse cRMNotesObj = new CRMNotesListResponse();
                    cRMNotesObj.Id = item.Id;
                    cRMNotesObj.Note = item.Note;
                    if (item.ClientUserId != null)
                    {
                        cRMNotesObj.ClientUserName = item.ClientUser.FirstName + " " + item.ClientUser.LastName;
                        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        if (item.ClientUser.Logo != null)
                        {
                            cRMNotesObj.LogoURL = OneClappContext.CurrentURL + "ClientUser/Logo/" + item.ClientUserId + "?" + Timestamp;
                        }
                        else
                        {
                            cRMNotesObj.LogoURL = null;
                        }
                    }
                    cRMNotesObj.CreatedOn = item.CreatedOn;
                    cRMNotesListResponses.Add(cRMNotesObj);
                }
            }
            int totalCount = 0;
            totalCount = cRMNotesListResponses.Count();
            var SkipValue = requestModel.PageSize * (requestModel.PageNumber - 1);
            if (!string.IsNullOrEmpty(requestModel.SearchString))
            {
                cRMNotesListResponses = cRMNotesListResponses.Where(t => (!string.IsNullOrEmpty(t.Note) && t.Note.ToLower().Contains(requestModel.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.ClientUserName) && t.ClientUserName.ToLower().Contains(requestModel.SearchString.ToLower()))).ToList();
                cRMNotesListResponses = cRMNotesListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            else
            {
                cRMNotesListResponses = cRMNotesListResponses.Skip(SkipValue).Take(requestModel.PageSize).ToList();
            }
            cRMNotesListResponses = ShortByColumn(requestModel.ShortColumnName, requestModel.SortType, cRMNotesListResponses);
            return new OperationResult<List<CRMNotesListResponse>>(true, System.Net.HttpStatusCode.OK, "", cRMNotesListResponses, totalCount);
        }

        private List<CRMNotesListResponse> ShortByColumn(string ShortColumn, string ShortOrder, List<CRMNotesListResponse> ClientNoteList)
        {
            List<CRMNotesListResponse> ClientNoteListVMList = new List<CRMNotesListResponse>();
            ClientNoteListVMList = ClientNoteList;
            if (ShortColumn.ToLower() == "note")
            {
                if (ShortOrder.ToLower() == "asc")
                {
                    ClientNoteListVMList = ClientNoteList.OrderBy(t => t.Note).ToList();
                }
                else
                {
                    ClientNoteListVMList = ClientNoteList.OrderByDescending(t => t.Note).ToList();
                }
            }
            else if (ShortColumn.ToLower() == "clientusername")
            {
                if (ShortOrder.ToLower() == "asc")
                {
                    ClientNoteListVMList = ClientNoteList.OrderBy(t => t.ClientUserName).ToList();
                }
                else
                {
                    ClientNoteListVMList = ClientNoteList.OrderByDescending(t => t.ClientUserName).ToList();
                }
            }
            else if (ShortColumn.ToLower() == "createdon")
            {
                if (ShortOrder.ToLower() == "asc")
                {
                    ClientNoteListVMList = ClientNoteList.OrderBy(t => t.CreatedOn).ToList();
                }
                else
                {
                    ClientNoteListVMList = ClientNoteList.OrderByDescending(t => t.CreatedOn).ToList();
                }
            }
            else
            {
                ClientNoteListVMList = ClientNoteList.OrderByDescending(t => t.Id).ToList();
            }
            return ClientNoteListVMList;
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<CRMNotesDetailResponse>> Detail(long Id)
        {
            var cRMNotesObj = _cRMNotesService.GetById(Id);
            var ResponseObj = _mapper.Map<CRMNotesDetailResponse>(cRMNotesObj);
            if (cRMNotesObj.ClientUserId != null)
            {
                ResponseObj.ClientUserName = cRMNotesObj.ClientUser.FirstName + " " + cRMNotesObj.ClientUser.LastName;
                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                if (cRMNotesObj.ClientUser.Logo != null)
                {
                    ResponseObj.LogoURL = OneClappContext.CurrentURL + "ClientUser/Logo/" + cRMNotesObj.ClientUserId + "?" + Timestamp;
                }
                else
                {
                    ResponseObj.LogoURL = null;
                }
            }
            if (cRMNotesObj.MateTimeRecordId != null)
            {
                ResponseObj.MateTimeRecordId = cRMNotesObj.MateTimeRecordId;
                ResponseObj.Duration = cRMNotesObj.MateTimeRecord.Duration;
                ResponseObj.IsBillable = cRMNotesObj.MateTimeRecord.IsBillable;
                
            }
            ResponseObj.NextCallDate = cRMNotesObj.NextCallDate;
            if (cRMNotesObj.SatisficationLevel != null)
            {
                ResponseObj.SatisficationLevel = cRMNotesObj.SatisficationLevel.Name;
            }
            return new OperationResult<CRMNotesDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }


    }
}