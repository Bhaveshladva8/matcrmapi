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
    public partial class CalendarListService : Service<CalendarList>, ICalendarListService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CalendarListService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CalendarList> CheckInsertOrUpdate(CalendarListDto model)
        {
            var calendarListObj = _mapper.Map<CalendarList>(model);
            var existingItem = _unitOfWork.CalendarListRepository.GetMany(t => t.Id == calendarListObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddCalendarList(calendarListObj);
            }
            else
            {
                return await UpdateCalendarList(calendarListObj, existingItem.Id);
            }
        }
        public async Task<CalendarList> UpdateCalendarList(CalendarList updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CalendarListRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }
        public async Task<CalendarList> AddCalendarList(CalendarList calendarListObj)
        {
            calendarListObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CalendarListRepository.AddAsync(calendarListObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<CalendarList> GetAll(int UserId)
        {
            return _unitOfWork.CalendarListRepository.GetMany(t => t.IsDeleted == false && (t.CreatedBy == UserId || t.CreatedBy == null)).Result.ToList();
        }

        public CalendarList GetCalendarListById(long Id)
        {
            return _unitOfWork.CalendarListRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        // public async Task<CalendarList> DeleteCalendarList(CalendarListDto model)
        // {
        //     var calendarListObj = _mapper.Map<CalendarList>(model);
        //     var existingItem = _unitOfWork.CalendarListRepository.GetMany(t => t.Id == calendarListObj.Id).Result.FirstOrDefault();
        //     if (existingItem != null)
        //     {
        //         existingItem.IsDeleted = true;
        //         existingItem.DeletedOn = DateTime.UtcNow;
        //         var newItem = await _unitOfWork.CalendarListRepository.UpdateAsync(existingItem, existingItem.Id);
        //         await _unitOfWork.CommitAsync();
        //         return newItem;
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }
        public async Task<CalendarList> DeleteCalendarList(long Id)
        {            
            var existingItem = _unitOfWork.CalendarListRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.CalendarListRepository.UpdateAsync(existingItem, existingItem.Id);
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

    }

    public partial interface ICalendarListService : IService<CalendarList>
    {
        Task<CalendarList> CheckInsertOrUpdate(CalendarListDto model);
        List<CalendarList> GetAll(int UserId);
        //Task<CalendarList> DeleteCalendarList(CalendarListDto model);
        Task<CalendarList> DeleteCalendarList(long Id);
        CalendarList GetCalendarListById(long Id);
    }
}