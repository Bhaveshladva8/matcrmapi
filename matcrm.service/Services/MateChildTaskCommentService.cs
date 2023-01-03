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
    public partial class MateChildTaskCommentService : Service<MateChildTaskComment>, IMateChildTaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateChildTaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateChildTaskComment> CheckInsertOrUpdate(MateChildTaskComment mateChildTaskCommentObj)
        {
            var existingItem = _unitOfWork.MateChildTaskCommentRepository.GetMany(t => t.Id == mateChildTaskCommentObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateChildTaskComment(mateChildTaskCommentObj);
            }
            else
            {
                return await UpdateMateChildTaskComment(mateChildTaskCommentObj, existingItem.Id);
            }
        }

        public async Task<MateChildTaskComment> InsertMateChildTaskComment(MateChildTaskComment mateChildTaskCommentObj)
        {
            var newItem = await _unitOfWork.MateChildTaskCommentRepository.AddAsync(mateChildTaskCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateChildTaskComment> UpdateMateChildTaskComment(MateChildTaskComment existingItem, long existingId)
        {
            await _unitOfWork.MateChildTaskCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public MateChildTaskComment GetByMateCommentId(long MateCommentId)
        {
            return _unitOfWork.MateChildTaskCommentRepository.GetMany(t => t.MateCommentId == MateCommentId).Result.FirstOrDefault();
        }
        public List<MateChildTaskComment> GetByChildTaskId(long ChildTaskId)
        {
            return _unitOfWork.MateChildTaskCommentRepository.GetMany(t => t.ChildTaskId == ChildTaskId && t.MateComment.DeletedOn == null).Result.Include(t => t.MateComment).ToList();
        }
    }
    public partial interface IMateChildTaskCommentService : IService<MateChildTaskComment>
    {
        Task<MateChildTaskComment> CheckInsertOrUpdate(MateChildTaskComment model);
        MateChildTaskComment GetByMateCommentId(long MateCommentId);
        List<MateChildTaskComment> GetByChildTaskId(long ChildTaskId);
    }
}