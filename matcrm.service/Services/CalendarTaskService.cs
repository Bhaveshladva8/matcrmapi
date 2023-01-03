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
    public partial class CalendarTaskService : Service<CalendarTask>, ICalendarTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CalendarTaskService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CalendarTask> CheckInsertOrUpdate(CalendarTaskDto model)
        {
            var calendarTaskObj = _mapper.Map<CalendarTask>(model);
            var existingItem = _unitOfWork.CalendarTaskRepository.GetMany(t => t.Id == calendarTaskObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddCalendarTask(calendarTaskObj);
            }
            else
            {
                calendarTaskObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateCalendarTask(calendarTaskObj, existingItem.Id);
            }
        }
        public async Task<CalendarTask> UpdateCalendarTask(CalendarTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CalendarTaskRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();
            return update;
        }
        public async Task<CalendarTask> AddCalendarTask(CalendarTask calendarTaskObj)
        {
            calendarTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CalendarTaskRepository.AddAsync(calendarTaskObj);
           await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<CalendarTask> GetAll(long calendarListId, bool IsDone)
        {
            return _unitOfWork.CalendarTaskRepository.GetMany(t => t.IsDeleted == false &&
             (calendarListId == 0 || t.CalendarListId == calendarListId) && t.IsDone == IsDone).Result.ToList();
        }
        
        public List<CalendarTask> GetAllByList(long calendarListId)
        {
            return _unitOfWork.CalendarTaskRepository.GetMany(t => t.IsDeleted == false && t.CalendarListId == calendarListId).Result.ToList();
        }

        public CalendarTask GetCalendarTaskById(long Id)
        {
            return _unitOfWork.CalendarTaskRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<CalendarTask> GetAllByUser(int UserId)
        {
            return _unitOfWork.CalendarTaskRepository.GetMany(t => t.IsDeleted == false && t.CreatedBy == UserId).Result.ToList();
        }
        // public CalendarTask DeleteCalendarTask(CalendarTaskDto model)
        // {
        //     var calendarTaskObj = _mapper.Map<CalendarTask>(model);
        //     var existingItem = _unitOfWork.CalendarTaskRepository.GetMany(t => t.Id == calendarTaskObj.Id).Result.FirstOrDefault();
        //     if (existingItem != null)
        //     {
        //         existingItem.IsDeleted = true;
        //         existingItem.DeletedOn = DateTime.UtcNow;
        //         var newItem = _unitOfWork.CalendarTaskRepository.UpdateAsync(existingItem, existingItem.Id).Result;
        //         _unitOfWork.CommitAsync();
        //         return newItem;
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }

        public async Task<CalendarTask> DeleteCalendarTask(long Id)
        {            
            var calendarTaskObj = _unitOfWork.CalendarTaskRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (calendarTaskObj != null)
            {
                calendarTaskObj.IsDeleted = true;
                calendarTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.CalendarTaskRepository.UpdateAsync(calendarTaskObj, calendarTaskObj.Id);
                await _unitOfWork.CommitAsync();                
            }
            return calendarTaskObj;
        }

         public List<CalendarTask> DeleteByList(long listId)
        {
            var calendarTaskList = _unitOfWork.CalendarTaskRepository.GetMany(t => t.CalendarListId == listId && t.IsDeleted == false).Result.ToList();
            if(calendarTaskList != null && calendarTaskList.Count() > 0)
            {
                foreach (var existingItem in calendarTaskList)
                {
                    if (existingItem != null)
                    {
                        existingItem.IsDeleted = true;
                        existingItem.DeletedOn = DateTime.UtcNow;
                        var newItem = _unitOfWork.CalendarTaskRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                        _unitOfWork.CommitAsync();
                    }
                }
            }            
            return calendarTaskList;

        }

    }
    public partial interface ICalendarTaskService : IService<CalendarTask>
    {
        Task<CalendarTask> CheckInsertOrUpdate(CalendarTaskDto model);
        List<CalendarTask> GetAll(long calendarListId, bool IsDone);
        List<CalendarTask> GetAllByUser(int UserId);
        //CalendarTask DeleteCalendarTask(CalendarTaskDto model);
        Task<CalendarTask> DeleteCalendarTask(long Id);
        CalendarTask GetCalendarTaskById(long Id);
        List<CalendarTask> GetAllByList(long calendarListId);
        List<CalendarTask> DeleteByList(long listId);
    }
}