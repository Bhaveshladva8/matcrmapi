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
    public class CalendarListController : Controller
    {
        private readonly ICalendarListService _calendarListService;
        private readonly ICalendarTaskService _calendarTaskService;
        private readonly ICalendarSubTaskService _calendarSubTaskService;
        private readonly ICalendarRepeatTypeService _calendarRepeatTypeService;
        private IMapper _mapper;

        private int UserId = 0;

        public CalendarListController(ICalendarListService calendarListService, ICalendarSubTaskService calendarSubTaskService,
        ICalendarTaskService calendarTaskService, IMapper mapper,
        ICalendarRepeatTypeService calendarRepeatTypeService)
        {
            _calendarListService = calendarListService;
            _calendarTaskService = calendarTaskService;
            _calendarSubTaskService = calendarSubTaskService;
            _calendarRepeatTypeService = calendarRepeatTypeService;
            _mapper = mapper;
        }

        #region CalendarList
        //[Authorize (Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<CalendarListAddUpdateResponse>> Add([FromBody] CalendarListAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<CalendarListDto>(model);
            requestmodel.CreatedBy = UserId;
            var calendarListObj = await _calendarListService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<CalendarListDto>(calendarListObj);
            var resposemodel = _mapper.Map<CalendarListAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<CalendarListAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", resposemodel);
            return new OperationResult<CalendarListAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Added successfully.", resposemodel);
        }

        [HttpPut]
        public async Task<OperationResult<CalendarListAddUpdateResponse>> Update([FromBody] CalendarListAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<CalendarListDto>(model);
            requestmodel.CreatedBy = UserId;
            var calendarListObj = await _calendarListService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<CalendarListDto>(calendarListObj);
            var resposemodel = _mapper.Map<CalendarListAddUpdateResponse>(requestmodel);
            return new OperationResult<CalendarListAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", resposemodel);
        }

        //[Authorize (Roles = "Admin")]        
        // [HttpDelete]
        // public async Task<OperationResult<CalendarListDeleteResponse>> Remove([FromBody] CalendarListDeleteRequest model)
        // {
        //     var requestmodel = _mapper.Map<CalendarListDto>(model);
        //     CalendarListDeleteResponse responsemodel = new CalendarListDeleteResponse();
        //     if (requestmodel.Id != null)
        //     {
        //         var calendarTaskList = _calendarTaskService.GetAllByList(requestmodel.Id.Value);
        //         if (calendarTaskList != null && calendarTaskList.Count > 0)
        //         {
        //             foreach (var item in calendarTaskList)
        //             {
        //                 var deletedSubTaskList = _calendarSubTaskService.DeleteByTask(item.Id);
        //             }
        //         }
        //         var deletedCalendarTask = _calendarTaskService.DeleteByList(requestmodel.Id.Value);
        //         var calendarListObj = await _calendarListService.DeleteCalendarList(requestmodel);
        //         requestmodel = _mapper.Map<CalendarListDto>(calendarListObj);
        //         responsemodel = _mapper.Map<CalendarListDeleteResponse>(requestmodel);
        //         return new OperationResult<CalendarListDeleteResponse>(true, System.Net.HttpStatusCode.OK, "CalendarList deleted successfully.", responsemodel);
        //     }
        //     else
        //     {
        //         responsemodel = _mapper.Map<CalendarListDeleteResponse>(requestmodel);
        //         return new OperationResult<CalendarListDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide calendarListId.", responsemodel);
        //     }
        // }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            if (Id != null)
            {
                var calendarTaskList = _calendarTaskService.GetAllByList(Id);
                if (calendarTaskList != null && calendarTaskList.Count > 0)
                {
                    foreach (var item in calendarTaskList)
                    {
                        var deletedSubTaskList = _calendarSubTaskService.DeleteByTask(item.Id);
                    }
                }
                var deletedCalendarTask = _calendarTaskService.DeleteByList(Id);
                var calendarListObj = await _calendarListService.DeleteCalendarList(Id);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "CalendarList deleted successfully", Id);

            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Form deleted successfully", Id);
            }
        }

        //get all calendar list        
        [HttpGet]
        public async Task<OperationResult<List<CalendarListGetAllResposne>>> List()
        {
            List<CalendarListDto> calendarListDtoList = new List<CalendarListDto>();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var calendarLists = _calendarListService.GetAll(UserId);
            calendarListDtoList = _mapper.Map<List<CalendarListDto>>(calendarLists);

            var allTaskByUser = _calendarTaskService.GetAllByUser(UserId);
            var allSubTaskByUser = _calendarSubTaskService.GetAllByUser(UserId);
            var allSubTaskList = _calendarSubTaskService.GetAll(false);
            var allRepeatType = _calendarRepeatTypeService.GetAll();

            if (calendarListDtoList != null && calendarListDtoList.Count() > 0)
            {
                foreach (var item in calendarListDtoList)
                {
                    if (allTaskByUser != null)
                    {
                        var calendarRemainingTask = allTaskByUser.Where(t => t.CalendarListId == item.Id && t.IsDone == false).ToList();
                        item.RemainingCalendarTaskList = _mapper.Map<List<CalendarTaskDto>>(calendarRemainingTask);

                        var calendarCompletedTask = allTaskByUser.Where(t => t.CalendarListId == item.Id && t.IsDone == true).ToList();
                        item.CompletedCalendarTaskList = _mapper.Map<List<CalendarTaskDto>>(calendarCompletedTask);

                    }

                    if (item.RemainingCalendarTaskList != null && item.RemainingCalendarTaskList.Count() > 0)
                    {
                        foreach (var data in item.RemainingCalendarTaskList)
                        {
                            var repeatType = allRepeatType.Where(d => d.Id == data.RepeatTypeId).FirstOrDefault();
                            data.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatType);

                            var CalendarSubTask = allSubTaskByUser.Where(s => s.CalendarTaskId == data.Id).ToList();
                            data.CalendarSubTaskList = _mapper.Map<List<CalendarSubTaskDto>>(CalendarSubTask);

                            if (data.CalendarSubTaskList != null && data.CalendarSubTaskList.Count() > 0)
                            {
                                foreach (var subTask in data.CalendarSubTaskList)
                                {
                                    var repeatTypeData = allRepeatType.Where(d => d.Id == subTask.RepeatTypeId).FirstOrDefault();
                                    subTask.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatTypeData);
                                }
                            }
                        }
                    }

                    if (item.CompletedCalendarTaskList != null && item.CompletedCalendarTaskList.Count() > 0)
                    {
                        foreach (var data1 in item.CompletedCalendarTaskList)
                        {
                            var repeatType = allRepeatType.Where(d => d.Id == data1.RepeatTypeId).FirstOrDefault();
                            data1.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatType);

                            var CalendarSubTask = allSubTaskByUser.Where(s => s.CalendarTaskId == data1.Id).ToList();
                            data1.CalendarSubTaskList = _mapper.Map<List<CalendarSubTaskDto>>(CalendarSubTask);

                            if (data1.CalendarSubTaskList != null && data1.CalendarSubTaskList.Count() > 0)
                            {
                                foreach (var subTask1 in data1.CalendarSubTaskList)
                                {
                                    var repeatTypeData = allRepeatType.Where(d => d.Id == subTask1.RepeatTypeId).FirstOrDefault();
                                    subTask1.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatTypeData);
                                }
                            }
                        }
                    }
                }
            }
            var resposnecalendarDtoList = _mapper.Map<List<CalendarListGetAllResposne>>(calendarListDtoList);
            return new OperationResult<List<CalendarListGetAllResposne>>(true, System.Net.HttpStatusCode.OK, "Found successfully", resposnecalendarDtoList);
        }

        //[Authorize (Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CalendarListDto>> Detail(long Id)
        {
            CalendarListDto calendarListDto = new CalendarListDto();
            var calendarListObj = _calendarListService.GetCalendarListById(Id);
            calendarListDto = _mapper.Map<CalendarListDto>(calendarListObj);

            var allTaskList = _calendarTaskService.GetAll(0, false);
            var allSubTaskList = _calendarSubTaskService.GetAll(false);
            var allRepeatType = _calendarRepeatTypeService.GetAll();

            var calendarsTaskList = allTaskList.Where(t => t.CalendarListId == calendarListObj.Id).ToList();
            calendarListDto.RemainingCalendarTaskList = _mapper.Map<List<CalendarTaskDto>>(calendarsTaskList);
            if (calendarListDto.RemainingCalendarTaskList != null && calendarListDto.RemainingCalendarTaskList.Count() > 0)
            {
                foreach (var item in calendarListDto.RemainingCalendarTaskList)
                {
                    var repeatType = allRepeatType.Where(d => d.Id == item.RepeatTypeId).FirstOrDefault();
                    item.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatType);

                    var calendarSubTask = allSubTaskList.Where(s => s.CalendarTaskId == item.Id).ToList();
                    item.CalendarSubTaskList = _mapper.Map<List<CalendarSubTaskDto>>(calendarSubTask);
                    foreach (var data in item.CalendarSubTaskList)
                    {
                        var repeatTypeData = allRepeatType.Where(d => d.Id == data.RepeatTypeId).FirstOrDefault();
                        data.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatTypeData);
                    }
                }
            }
            return new OperationResult<CalendarListDto>(true, System.Net.HttpStatusCode.OK, "", calendarListDto);
        }
        #endregion

    }
}