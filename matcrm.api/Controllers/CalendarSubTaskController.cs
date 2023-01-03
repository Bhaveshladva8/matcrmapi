using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class CalendarSubTaskController : Controller
    {
        private readonly ICalendarRepeatTypeService _calendarRepeatTypeService;
        private readonly ICalendarSubTaskService _calendarSubTaskService;
        public IMapper _mapper;
        private int UserId = 0;
        public CalendarSubTaskController(ICalendarSubTaskService calendarSubTaskService, ICalendarRepeatTypeService calendarRepeatTypeService, IMapper mapper)
        {
            _calendarRepeatTypeService = calendarRepeatTypeService;
            _calendarSubTaskService = calendarSubTaskService;
            _mapper = mapper;
        }

        #region CalendarSubTask
        [HttpPost]
        public async Task<OperationResult<CalendarSubTaskAddUpdateResponse>> Add([FromBody] CalendarSubTaskAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var requestmodel = _mapper.Map<CalendarSubTaskDto>(model);

            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }

            var calendarSubTaskObj = await _calendarSubTaskService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<CalendarSubTaskDto>(calendarSubTaskObj);
            var responsemodel = _mapper.Map<CalendarSubTaskAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<CalendarSubTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            return new OperationResult<CalendarSubTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Added successfully.", responsemodel);
        }

        [HttpPut]
        public async Task<OperationResult<CalendarSubTaskAddUpdateResponse>> Update([FromBody] CalendarSubTaskAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var requestmodel = _mapper.Map<CalendarSubTaskDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }

            var calendarSubTaskObj = await _calendarSubTaskService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<CalendarSubTaskDto>(calendarSubTaskObj);
            var responsemodel = _mapper.Map<CalendarSubTaskAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<CalendarSubTaskAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully", responsemodel);
            return new OperationResult<CalendarSubTaskAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Added successfully.", responsemodel);
        }

        //[Authorize (Roles = "Admin")]        
        // [HttpDelete]
        // public async Task<OperationResult<CalendarSubTaskDeleteResponse>> Remove([FromBody] CalendarSubTaskDeleteRequest model)
        // {
        //     var requestmodel =  _mapper.Map<CalendarSubTaskDto>(model);
        //     CalendarSubTaskDeleteResponse responsemodel = new CalendarSubTaskDeleteResponse();
        //     if (requestmodel.Id != null)
        //     {
        //         var calendarSubTaskObj = _calendarSubTaskService.DeleteCalendarSubTask(requestmodel);
        //         requestmodel = _mapper.Map<CalendarSubTaskDto>(calendarSubTaskObj);
        //         responsemodel = _mapper.Map<CalendarSubTaskDeleteResponse>(requestmodel);
        //         return new OperationResult<CalendarSubTaskDeleteResponse>(true, System.Net.HttpStatusCode.OK,"CalendarSubTask deleted successfully.", responsemodel);
        //     }
        //     else
        //     {
        //         responsemodel = _mapper.Map<CalendarSubTaskDeleteResponse>(requestmodel);
        //         return new OperationResult<CalendarSubTaskDeleteResponse>(false, System.Net.HttpStatusCode.OK,"Please provide calendarSubTaskId.", responsemodel);
        //     }
        // }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            CalendarSubTaskDeleteResponse responsemodel = new CalendarSubTaskDeleteResponse();
            if (Id != null && Id > 0)
            {
                var calendarSubTaskObj = _calendarSubTaskService.DeleteCalendarSubTask(Id);
                
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "CalendarSubTask deleted successfully.", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide calendarSubTaskId.", Id);
            }
        }

        //[Authorize (Roles = "Admin")]
        [HttpGet("{IsDone}")]
        public async Task<OperationResult<List<CalendarSubTaskDto>>> List(bool IsDone)
        {
            List<CalendarSubTaskDto> calendarSubTaskDtoList = new List<CalendarSubTaskDto>();
            var calendarSubTasksObj = new List<CalendarSubTask>();

            calendarSubTasksObj = _calendarSubTaskService.GetAll(IsDone);
            calendarSubTaskDtoList = _mapper.Map<List<CalendarSubTaskDto>>(calendarSubTasksObj);

            var allRepeatTypes = _calendarRepeatTypeService.GetAll();
            if (calendarSubTaskDtoList != null && calendarSubTaskDtoList.Count() > 0)
            {
                foreach (var item in calendarSubTaskDtoList)
                {
                    var CalendarRepeatType = allRepeatTypes.Where(d => d.Id == item.RepeatTypeId).FirstOrDefault();
                    item.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(CalendarRepeatType);
                }
            }
            return new OperationResult<List<CalendarSubTaskDto>>(true, System.Net.HttpStatusCode.OK, "Found successfully", calendarSubTaskDtoList);
        }
        //[Authorize (Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CalendarSubTaskDto>> Detail(long Id)
        {
            CalendarSubTaskDto calendarSubTaskDto = new CalendarSubTaskDto();
            var calendarSubTaskObj = _calendarSubTaskService.GetCalendarSubTaskById(Id);
            calendarSubTaskDto = _mapper.Map<CalendarSubTaskDto>(calendarSubTaskObj);

            var allRepeatTypes = _calendarRepeatTypeService.GetAll();

            var calendarRepeatType = allRepeatTypes.Where(d => d.Id == calendarSubTaskDto.RepeatTypeId).FirstOrDefault();
            calendarSubTaskDto.calendarRepeatType = _mapper.Map<CalendarRepeatTypeDto>(calendarRepeatType);

            return new OperationResult<CalendarSubTaskDto>(true, System.Net.HttpStatusCode.OK, "", calendarSubTaskDto);
        }
        #endregion
    }
}