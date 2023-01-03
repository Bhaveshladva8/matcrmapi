using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class MateCommentAttachmentService : Service<MateCommentAttachment>, IMateCommentAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateCommentAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateCommentAttachment> CheckInsertOrUpdate(MateCommentAttachment mateCommentAttachmentObj)
        {
            var existingItem = _unitOfWork.MateCommentAttachmentRepository.GetMany(t => t.MateCommentId == mateCommentAttachmentObj.MateCommentId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateCommentAttachment(mateCommentAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateMateCommentAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<MateCommentAttachment> InsertMateCommentAttachment(MateCommentAttachment MateCommentAttachmentObj)
        {
            var newItem = await _unitOfWork.MateCommentAttachmentRepository.AddAsync(MateCommentAttachmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateCommentAttachment> UpdateMateCommentAttachment(MateCommentAttachment existingItem, long existingId)
        {
            await _unitOfWork.MateCommentAttachmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<MateCommentAttachment> DeleteById(long Id)
        {
            var mateCommentAttachmentObj = _unitOfWork.MateCommentAttachmentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mateCommentAttachmentObj != null)
            {
                mateCommentAttachmentObj.DeletedOn = DateTime.UtcNow;
                var newItem = await _unitOfWork.MateCommentAttachmentRepository.UpdateAsync(mateCommentAttachmentObj, mateCommentAttachmentObj.Id);

                await _unitOfWork.CommitAsync();
            }
            return mateCommentAttachmentObj;
        }
        public List<MateCommentAttachment> GetByMateCommentId(long MateCommentId)
        {
            return _unitOfWork.MateCommentAttachmentRepository.GetMany(t => t.MateCommentId == MateCommentId && t.DeletedOn == null).Result.ToList();
        }
        public MateCommentAttachment GetById(long Id)
        {
            return _unitOfWork.MateCommentAttachmentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public async Task<List<MateCommentAttachment>> DeleteByMateCommentId(long MateCommentId)
        {
            var mateCommentAttachmentList = _unitOfWork.MateCommentAttachmentRepository.GetMany(t => t.MateCommentId == MateCommentId && t.DeletedOn == null).Result.ToList();
            if (mateCommentAttachmentList != null && mateCommentAttachmentList.Count() > 0)
            {
                foreach (var item in mateCommentAttachmentList)
                {                    
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.MateCommentAttachmentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return mateCommentAttachmentList;
        }
    }
    public partial interface IMateCommentAttachmentService : IService<MateCommentAttachment>
    {
        Task<MateCommentAttachment> CheckInsertOrUpdate(MateCommentAttachment model);
        Task<MateCommentAttachment> DeleteById(long Id);
        List<MateCommentAttachment> GetByMateCommentId(long MateCommentId);
        MateCommentAttachment GetById(long Id);
        Task<List<MateCommentAttachment>> DeleteByMateCommentId(long MateCommentId);
    }
}