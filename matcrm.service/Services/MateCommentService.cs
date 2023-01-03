using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class MateCommentService : Service<MateComment>, IMateCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateComment> CheckInsertOrUpdate(MateComment mateCommentObj)
        {
            var existingItem = _unitOfWork.MateCommentRepository.GetMany(t => t.Id == mateCommentObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateComment(mateCommentObj);
            }
            else
            {
                mateCommentObj.CreatedOn = existingItem.CreatedOn;  
                return await UpdateMateComment(mateCommentObj, existingItem.Id);
            }
        }

        public async Task<MateComment> InsertMateComment(MateComment mateCommentObj)
        {
            mateCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.MateCommentRepository.Add(mateCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateComment> UpdateMateComment(MateComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;            
            await _unitOfWork.MateCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<MateComment> DeleteMateComment(long Id)
        {
            var mateCommentObj = _unitOfWork.MateCommentRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (mateCommentObj != null)
            {
                mateCommentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.MateCommentRepository.UpdateAsync(mateCommentObj, mateCommentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return mateCommentObj;
        }
        

    }
    public partial interface IMateCommentService : IService<MateComment>
    {
        Task<MateComment> CheckInsertOrUpdate(MateComment model);
        Task<MateComment> DeleteMateComment(long Id);
        //List<MateComment> GetAllById(string Id);
    }
}