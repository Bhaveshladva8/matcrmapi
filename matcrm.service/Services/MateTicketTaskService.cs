using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class MateTicketTaskService : Service<MateTicketTask>, IMateTicketTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateTicketTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public List<MateTicketTask> GetAllByTicketId(long TicketId)
        {
            return _unitOfWork.MateTicketTaskRepository.GetMany(t => t.MateTicketId == TicketId).Result.ToList();
        }
        public async Task<MateTicketTask> CheckInsertOrUpdate(MateTicketTask mateTicketTaskObj)
        {
            //MateTicketTask? existingItem = null;
            var existingItem = _unitOfWork.MateTicketTaskRepository.GetMany(t => t.MateTicketId == mateTicketTaskObj.MateTicketId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateTicketTask(mateTicketTaskObj);
            }
            else
            {
                mateTicketTaskObj.Id = existingItem.Id;
                return await UpdateMateTicketTask(mateTicketTaskObj, existingItem.Id);
            }
        }
        public async Task<MateTicketTask> InsertMateTicketTask(MateTicketTask mateTicketTaskObj)
        {
            var newItem = await _unitOfWork.MateTicketTaskRepository.AddAsync(mateTicketTaskObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateTicketTask> UpdateMateTicketTask(MateTicketTask existingItem, long existingId)
        {
            await _unitOfWork.MateTicketTaskRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<MateTicketTask> DeleteByTaskId(long TaskId)
        {
            var mateTicketTaskObj = _unitOfWork.MateTicketTaskRepository.GetMany(u => u.EmployeeTaskId == TaskId && u.DeletedOn == null).Result.FirstOrDefault();
            if (mateTicketTaskObj != null)
            {
                mateTicketTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateTicketTaskRepository.UpdateAsync(mateTicketTaskObj, mateTicketTaskObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateTicketTaskObj;
        }
    }
    public partial interface IMateTicketTaskService : IService<MateTicketTask>
    {
        List<MateTicketTask> GetAllByTicketId(long TicketId);
        Task<MateTicketTask> CheckInsertOrUpdate(MateTicketTask model);
        Task<MateTicketTask> DeleteByTaskId(long TaskId);
    }
}