using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CalendarTaskController : Controller
    {
        private readonly ICalendarTaskService _calendarTaskService;
        private readonly ICalendarSubTaskService _calendarSubTaskService;
        private readonly ICalendarRepeatTypeService _calendarRepeatTypeService;
        public IMapper _mapper;
        private int UserId = 0;
        public CalendarTaskController(
        ICalendarTaskService calendarTaskService,
        ICalendarSubTaskService calendarSubTaskService,
        ICalendarRepeatTypeService calendarRepeatTypeService,
        IMapper mapper)
        {
            _calendarRepeatTypeService = calendarRepeatTypeService;
            _calendarSubTaskService = calendarSubTaskService;
            _calendarTaskService = calendarTaskService;
            _mapper = mapper;
        }

        #region CalendarTask
        //[Authorize (Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<CalendarTaskAddUpdateResponse>> Add([FromBody] CalendarTaskAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<CalendarTaskDto>(model);

            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            var calendarTaskObj = await _calendarTaskService.CheckInsertOrUpdate(requestmodel);
            var calendarSubTaskList = _calendarSubTaskService.GetByTaskId(calendarTaskObj.Id);
            if (requestmodel.IsDone)
            {
                if (calendarSubTaskList != null && calendarSubTaskList.Count() > 0)
                {
                    foreach (var item in calendarSubTaskList)
                    {
                        item.IsDone = true;
                        var AddUpdate = await _calendarSubTaskService.UpdateCalendarSubTask(item, item.Id);
                    }
                }
            }
            requestmodel = _mapper.Map<CalendarTaskDto>(calendarTaskObj);
            if (calendarTaskObj.IsDone == false)
            {
                var calendarSubTaskList1 = _calendarSubTaskService.GetByTaskId(calendarTaskObj.Id);
                requestmodel.CalendarSubTaskList = _mapper.Map<List<CalendarSubTaskDto>>(calendarSubTaskList1);
            }
            var responsemodel = _mapper.Map<CalendarTaskAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<CalendarTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            return new OperationResult<CalendarTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Added successfully.", responsemodel);
        }

        [HttpPut]
        public async Task<OperationResult<CalendarTaskAddUpdateResponse>> Update([FromBody] CalendarTaskAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<CalendarTaskDto>(model);

            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }

            var calendarTaskObj = await _calendarTaskService.CheckInsertOrUpdate(requestmodel);
            var calendarSubTaskList = _calendarSubTaskService.GetByTaskId(calendarTaskObj.Id);
            if (requestmodel.IsDone)
            {
                if (calendarSubTaskList != null && calendarSubTaskList.Count() > 0)
                {
                    foreach (var item in calendarSubTaskList)
                    {
                        item.IsDone = true;
                        var AddUpdate = await _calendarSubTaskService.UpdateCalendarSubTask(item, item.Id);
                    }
                }
            }
            requestmodel = _mapper.Map<CalendarTaskDto>(calendarTaskObj);
            if (calendarTaskObj.IsDone == false)
            {
                var calendarSubTaskList1 = _calendarSubTaskService.GetByTaskId(calendarTaskObj.Id);
                requestmodel.CalendarSubTaskList = _mapper.Map<List<CalendarSubTaskDto>>(calendarSubTaskList1);
            }
            var responsemodel = _mapper.Map<CalendarTaskAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<CalendarTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            return new OperationResult<CalendarTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Added successfully.", responsemodel);
        }

        //[Authorize (Roles = "Admin")]        
        // [HttpDelete]
        // public async Task<OperationResult<CalendarTaskDeleteResponse>> Remove([FromBody] CalendarTaskDeleteRequest model)
        // {
        //     var requestmodel = _mapper.Map<CalendarTaskDto>(model);
        //     CalendarTaskDeleteResponse responsemodel = new CalendarTaskDeleteResponse();
        //     if (requestmodel.Id != null)
        //     {
        //         var calendarSubTasks = _calendarSubTaskService.DeleteByTask(requestmodel.Id.Value);
        //         var calendarTaskObj = _calendarTaskService.DeleteCalendarTask(requestmodel);
        //         requestmodel = _mapper.Map<CalendarTaskDto>(calendarTaskObj);
        //         responsemodel = _mapper.Map<CalendarTaskDeleteResponse>(requestmodel);
        //         return new OperationResult<CalendarTaskDeleteResponse>(true, System.Net.HttpStatusCode.OK, "Calendar task deleted successfully.", responsemodel);
        //     }
        //     else
        //     {
        //         responsemodel = _mapper.Map<CalendarTaskDeleteResponse>(requestmodel);
        //         return new OperationResult<CalendarTaskDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide calendarTaskId.", responsemodel);
        //     }
        // }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {            
            if (Id != null && Id > 0)
            {
                var calendarSubTasks = await _calendarSubTaskService.DeleteByTask(Id);
                var calendarTaskObj = await _calendarTaskService.DeleteCalendarTask(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Calendar task deleted successfully.", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide calendarTaskId.", Id);

            }
        }

        //[Authorize (Roles = "Admin")]
        [HttpGet("{CalendarListId}/{IsDone}")]
        public async Task<OperationResult<List<CalendarTaskDto>>> List(long CalendarListId, bool IsDone)
        {
            List<CalendarTaskDto> calendarTaskDtoList = new List<CalendarTaskDto>();
            var calendarTaskList = new List<CalendarTask>();
            calendarTaskList = _calendarTaskService.GetAll(CalendarListId, IsDone);
            calendarTaskDtoList = _mapper.Map<List<CalendarTaskDto>>(calendarTaskList);

            var allSubTaskList = _calendarSubTaskService.GetAll(IsDone);
            var allRepeatType = _calendarRepeatTypeService.GetAll();
            if (calendarTaskDtoList != null && calendarTaskDtoList.Count() > 0)
            {
                foreach (var item in calendarTaskDtoList)
                {
                    var repeatType = allRepeatType.Where(d => d.Id == item.RepeatTypeId).FirstOrDefault();
                    item.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatType);

                    var calendarSubTasks = allSubTaskList.Where(d => d.CalendarTaskId == item.Id).ToList();
                    item.CalendarSubTaskList = _mapper.Map<List<CalendarSubTaskDto>>(calendarSubTasks);

                    if (item.CalendarSubTaskList != null && item.CalendarSubTaskList.Count() > 0)
                    {
                        foreach (var data in item.CalendarSubTaskList)
                        {
                            var repeatTypeData = allRepeatType.Where(d => d.Id == data.RepeatTypeId).FirstOrDefault();
                            data.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatTypeData);
                        }
                    }
                }
            }
            return new OperationResult<List<CalendarTaskDto>>(true, System.Net.HttpStatusCode.OK, "Found successfully", calendarTaskDtoList);
        }
        //[Authorize (Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CalendarTaskDto>> Detail(long Id)
        {
            CalendarTaskDto calendarTaskDto = new CalendarTaskDto();
            var calendarTaskObj = _calendarTaskService.GetCalendarTaskById(Id);
            calendarTaskDto = _mapper.Map<CalendarTaskDto>(calendarTaskObj);

            var allSubTaskList = _calendarSubTaskService.GetAll(false);
            var allRepeatType = _calendarRepeatTypeService.GetAll();

            if (calendarTaskObj.RepeatTypeId != null)
            {
                var repeatType = allRepeatType.Where(d => d.Id == calendarTaskObj.RepeatTypeId).FirstOrDefault();
                calendarTaskDto.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatType);
            }
            if (calendarTaskObj.Id != null)
            {
                var calendarSubTasks = allSubTaskList.Where(d => d.CalendarTaskId == calendarTaskObj.Id).ToList();
                calendarTaskDto.CalendarSubTaskList = _mapper.Map<List<CalendarSubTaskDto>>(calendarSubTasks);
            }
            if (calendarTaskDto.CalendarSubTaskList != null && calendarTaskDto.CalendarSubTaskList.Count() > 0)
            {
                foreach (var data in calendarTaskDto.CalendarSubTaskList)
                {
                    var repeatTypeData = allRepeatType.Where(d => d.Id == data.RepeatTypeId).FirstOrDefault();
                    data.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(repeatTypeData);
                }
            }
            return new OperationResult<CalendarTaskDto>(true, System.Net.HttpStatusCode.OK, "", calendarTaskDto);
        }
        #endregion

        //get all repeat types        
        [HttpGet]
        public async Task<OperationResult<List<CalendarRepeatTypeResponse>>> RepeatTypes()
        {
            var calendarRepeatTypes = _calendarRepeatTypeService.GetAll();
            var calendarRepeatTypesDto = _mapper.Map<List<CalendarRepeatTypeDto>>(calendarRepeatTypes);
            var responseRepeatTypesDto = _mapper.Map<List<CalendarRepeatTypeResponse>>(calendarRepeatTypesDto);
            return new OperationResult<List<CalendarRepeatTypeResponse>>(true, System.Net.HttpStatusCode.OK, "", responseRepeatTypesDto);
        }
    }
}