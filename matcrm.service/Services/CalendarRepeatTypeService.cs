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
    public partial class CalendarRepeatTypeService : Service<CalendarRepeatType>, ICalendarRepeatTypeService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CalendarRepeatTypeService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CalendarRepeatType> CheckInsertOrUpdate(CalendarRepeatTypeDto model)
        {
            var calendarRepeatTypeObj = _mapper.Map<CalendarRepeatType>(model);
            var existingItem = _unitOfWork.CalendarRepeatTypeRepository.GetMany(t => t.Id == calendarRepeatTypeObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await AddCalendarRepeatType(calendarRepeatTypeObj);
            }
            else
            {
                return await UpdateCalendarRepeatType(calendarRepeatTypeObj, existingItem.Id);
            }
        }
        public async Task<CalendarRepeatType> UpdateCalendarRepeatType(CalendarRepeatType updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.CalendarRepeatTypeRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();
            return update;
        }
        public async Task<CalendarRepeatType> AddCalendarRepeatType(CalendarRepeatType calendarRepeatTypeObj)
        {
            calendarRepeatTypeObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CalendarRepeatTypeRepository.AddAsync(calendarRepeatTypeObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<CalendarRepeatType> GetAll()
        {
            return _unitOfWork.CalendarRepeatTypeRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public CalendarRepeatType GetCalendarRepeatTypeById(long Id)
        {
            return _unitOfWork.CalendarRepeatTypeRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        public CalendarRepeatType DeleteCalendarRepeatType(CalendarRepeatTypeDto model)
        {
            var calendarRepeatTypeObj = _mapper.Map<CalendarRepeatType>(model);
            var existingItem = _unitOfWork.CalendarRepeatTypeRepository.GetMany(t => t.Id == calendarRepeatTypeObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.CalendarRepeatTypeRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface ICalendarRepeatTypeService : IService<CalendarRepeatType>
    {
        Task<CalendarRepeatType> CheckInsertOrUpdate(CalendarRepeatTypeDto model);
        List<CalendarRepeatType> GetAll();
        CalendarRepeatType DeleteCalendarRepeatType(CalendarRepeatTypeDto model);
        CalendarRepeatType GetCalendarRepeatTypeById(long Id);
    }
}