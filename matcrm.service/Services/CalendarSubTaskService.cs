using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class CalendarSubTaskService : Service<CalendarSubTask>, ICalendarSubTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CalendarSubTaskService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CalendarSubTask> CheckInsertOrUpdate(CalendarSubTaskDto model)
        {
            var calendarSubTaskObj = _mapper.Map<CalendarSubTask>(model);
            var existingItem = _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.Id == calendarSubTaskObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddCalendarSubTask(calendarSubTaskObj);
            }
            else
            {
                calendarSubTaskObj.CreatedBy = existingItem.CreatedBy;
                calendarSubTaskObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateCalendarSubTask(calendarSubTaskObj, existingItem.Id);
            }
        }

        public async Task<CalendarSubTask> UpdateCalendarSubTask(CalendarSubTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CalendarSubTaskRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();
            return update;
        }
        public async Task<CalendarSubTask> AddCalendarSubTask(CalendarSubTask calendarSubTaskObj)
        {
            calendarSubTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CalendarSubTaskRepository.AddAsync(calendarSubTaskObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<CalendarSubTask> GetAll(bool IsDone)
        {
            return _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.IsDeleted == false && t.IsDone == IsDone).Result.ToList();
        }

        public CalendarSubTask GetCalendarSubTaskById(long Id)
        {
            return _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<CalendarSubTask> GetByTaskId(long TaskId)
        {
            return _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.IsDeleted == false && t.CalendarTaskId == TaskId).Result.ToList();
        }

        // public CalendarSubTask DeleteCalendarSubTask(CalendarSubTaskDto model)
        // {
        //     var calendarSubTaskObj = _mapper.Map<CalendarSubTask>(model);
        //     var existingItem = _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.Id == calendarSubTaskObj.Id).Result.FirstOrDefault();
        //     if (existingItem != null)
        //     {
        //         existingItem.IsDeleted = true;
        //         existingItem.DeletedOn = DateTime.UtcNow;
        //         var newItem = _unitOfWork.CalendarSubTaskRepository.UpdateAsync(existingItem, existingItem.Id).Result;
        //         _unitOfWork.CommitAsync();
        //         return newItem;
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }

        public async Task<CalendarSubTask> DeleteCalendarSubTask(long Id)
        {            
            var calendarSubTaskObj = _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (calendarSubTaskObj != null)
            {
                calendarSubTaskObj.IsDeleted = true;
                calendarSubTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.CalendarSubTaskRepository.UpdateAsync(calendarSubTaskObj, calendarSubTaskObj.Id);
                await _unitOfWork.CommitAsync();
                
            }
            return calendarSubTaskObj;
        }

        public List<CalendarSubTask> GetAllByUser(int UserId)
        {
            return _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.IsDeleted == false && t.CreatedBy == UserId).Result.ToList();
        }

        public async Task<List<CalendarSubTask>> DeleteByTask(long TaskId)
        {
            var calendarSubTaskList = _unitOfWork.CalendarSubTaskRepository.GetMany(t => t.CalendarTaskId == TaskId && t.IsDeleted == false).Result.ToList();
            if (calendarSubTaskList != null && calendarSubTaskList.Count() > 0)
            {
                foreach (var existingItem in calendarSubTaskList)
                {
                    if (existingItem != null)
                    {
                        existingItem.IsDeleted = true;
                        existingItem.DeletedOn = DateTime.UtcNow;
                        await _unitOfWork.CalendarSubTaskRepository.UpdateAsync(existingItem, existingItem.Id);
                        await _unitOfWork.CommitAsync();
                    }
                }
            }
            return calendarSubTaskList;
        }

    }
    public partial interface ICalendarSubTaskService : IService<CalendarSubTask>
    {
        Task<CalendarSubTask> CheckInsertOrUpdate(CalendarSubTaskDto model);
        List<CalendarSubTask> GetAll(bool IsDone);
        //CalendarSubTask DeleteCalendarSubTask(CalendarSubTaskDto model);
        Task<CalendarSubTask> DeleteCalendarSubTask(long Id);
        CalendarSubTask GetCalendarSubTaskById(long Id);
        List<CalendarSubTask> GetByTaskId(long TaskId);
        // List<CalendarSubTask> DeleteByTask(long TaskId);

        Task<List<CalendarSubTask>> DeleteByTask(long TaskId);
        List<CalendarSubTask> GetAllByUser(int UserId);
        Task<CalendarSubTask> UpdateCalendarSubTask(CalendarSubTask updatedItem, long existingId);
    }
}