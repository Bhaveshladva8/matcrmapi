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
    public partial class CalendarSyncActivityService : Service<CalendarSyncActivity>, ICalendarSyncActivityService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CalendarSyncActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CalendarSyncActivity> CheckInsertOrUpdate(CalendarSyncActivityDto model)
        {
            var calendarSyncActivityObj = _mapper.Map<CalendarSyncActivity>(model);
            var existingItem = _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.IntProviderAppSecretId == calendarSyncActivityObj.IntProviderAppSecretId && t.CreatedBy == calendarSyncActivityObj.CreatedBy && t.CalendarEventId == calendarSyncActivityObj.CalendarEventId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCalendarSyncActivity(calendarSyncActivityObj);
            }
            else
            {
                return await UpdateCalendarSyncActivity(existingItem, existingItem.Id);
            }
        }

        public async Task<CalendarSyncActivity> InsertCalendarSyncActivity(CalendarSyncActivity calendarSyncActivityObj)
        {
            calendarSyncActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CalendarSyncActivityRepository.AddAsync(calendarSyncActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CalendarSyncActivity> UpdateCalendarSyncActivity(CalendarSyncActivity existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CalendarSyncActivityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CalendarSyncActivity> GetAll()
        {
            return _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public CalendarSyncActivity GetByEventId(string eventId, long scretAppId)
        {
            return _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.CalendarEventId == eventId && t.IntProviderAppSecretId == scretAppId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<CalendarSyncActivity> GetByAppSecret(long secretAppId)
        {
            return _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.IntProviderAppSecretId == secretAppId && t.IsDeleted == false).Result.ToList();
        }

        public CalendarSyncActivity GetById(long Id)
        {
            return _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<CalendarSyncActivity> GetByModule(long ModuleId)
        {
            return _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.ModuleId == ModuleId && t.IsDeleted == false).Result.ToList();
        }

        public List<CalendarSyncActivity> GetByUser(int UserId)
        {
            return _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.CreatedBy == UserId && t.IsDeleted == false).Result.ToList();
        }

        public CalendarSyncActivity GetCalendarSyncActivity(CalendarSyncActivityDto model)
        {
            return _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.ModuleId == model.ModuleId && t.IntProviderAppSecretId == model.IntProviderAppSecretId && t.CreatedBy == model.CreatedBy
           && t.ActivityId == model.ActivityId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<CalendarSyncActivity> DeleteCalendarSyncActivity(long Id)
        {
            var calendarSyncActivityObj = _unitOfWork.CalendarSyncActivityRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if(calendarSyncActivityObj != null)
            {
                calendarSyncActivityObj.IsDeleted = true;
                calendarSyncActivityObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.CalendarSyncActivityRepository.UpdateAsync(calendarSyncActivityObj, calendarSyncActivityObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return calendarSyncActivityObj;
            
        }
    }

    public partial interface ICalendarSyncActivityService : IService<CalendarSyncActivity>
    {
        Task<CalendarSyncActivity> CheckInsertOrUpdate(CalendarSyncActivityDto model);
        List<CalendarSyncActivity> GetAll();
        CalendarSyncActivity GetByEventId(string eventId, long scretAppId);
        List<CalendarSyncActivity> GetByAppSecret(long secretAppId);
        List<CalendarSyncActivity> GetByModule(long ModuleId);
        CalendarSyncActivity GetById(long Id);
        Task<CalendarSyncActivity> DeleteCalendarSyncActivity(long Id);
        CalendarSyncActivity GetCalendarSyncActivity(CalendarSyncActivityDto model);
        List<CalendarSyncActivity> GetByUser(int UserId);
    }
}