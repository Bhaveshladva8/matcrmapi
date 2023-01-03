using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data;
using AutoMapper;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class MateTicketUserService : Service<MateTicketUser>, IMateTicketUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MateTicketUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateTicketUser> CheckInsertOrUpdate(MateTicketUser model)
        {
            var mateTicketUserObj = _mapper.Map<MateTicketUser>(model);
            var existingItem = _unitOfWork.MateTicketUserRepository.GetMany(t => t.MateTicketId == mateTicketUserObj.MateTicketId && t.UserId == model.UserId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertTaskUser(mateTicketUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public bool IsExistOrNot(MateTicketUser model)
        {
            var existingItem = _unitOfWork.MateTicketUserRepository.GetMany(t => t.MateTicketId == model.MateTicketId && t.UserId == model.UserId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<MateTicketUser> InsertTaskUser(MateTicketUser mateTicketUserObj)
        {
            mateTicketUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MateTicketUserRepository.AddAsync(mateTicketUserObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }
        public List<MateTicketUser> GetByTicketId(long TicketId)
        {
            return _unitOfWork.MateTicketUserRepository.GetMany(t => t.MateTicketId == TicketId && t.DeletedOn == null).Result.Include(t => t.User).ToList();
        }
        public async Task<List<MateTicketUser>> DeleteByTicketId(long TicketId)
        {
            var mateTicketUserList = _unitOfWork.MateTicketUserRepository.GetMany(t => t.MateTicketId == TicketId && t.DeletedOn == null).Result.ToList();
            if (mateTicketUserList != null && mateTicketUserList.Count() > 0)
            {
                foreach (var item in mateTicketUserList)
                {
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.MateTicketUserRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return mateTicketUserList;
        }
        public async Task<MateTicketUser> UnAssign(long Id)
        {
            var mateTicketUserObj = _unitOfWork.MateTicketUserRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mateTicketUserObj != null)
            {
                mateTicketUserObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateTicketUserRepository.UpdateAsync(mateTicketUserObj, mateTicketUserObj.Id);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                return null;
            }
            return mateTicketUserObj;
        }
        public List<MateTicketUser> GetByUserId(long UserId)
        {
            return _unitOfWork.MateTicketUserRepository.GetMany(t => t.UserId == UserId && t.DeletedOn == null).Result.Include(t => t.User).Include(t => t.MateTicket).ToList();
        }
    }
    public partial interface IMateTicketUserService : IService<MateTicketUser>
    {
        Task<MateTicketUser> CheckInsertOrUpdate(MateTicketUser model);
        bool IsExistOrNot(MateTicketUser model);
        List<MateTicketUser> GetByTicketId(long TicketId);
        Task<List<MateTicketUser>> DeleteByTicketId(long TicketId);
        Task<MateTicketUser> UnAssign(long Id);
        List<MateTicketUser> GetByUserId(long UserId);
    }
}