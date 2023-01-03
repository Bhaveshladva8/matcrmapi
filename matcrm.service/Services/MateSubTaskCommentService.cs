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
    public partial class MateSubTaskCommentService : Service<MateSubTaskComment>, IMateSubTaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateSubTaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateSubTaskComment> CheckInsertOrUpdate(MateSubTaskComment mateSubTaskCommentObj)
        {
            var existingItem = _unitOfWork.MateSubTaskCommentRepository.GetMany(t => t.Id == mateSubTaskCommentObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateSubTaskComment(mateSubTaskCommentObj);
            }
            else
            {
                return await UpdateMateSubTaskComment(mateSubTaskCommentObj, existingItem.Id);
            }
        }

        public async Task<MateSubTaskComment> InsertMateSubTaskComment(MateSubTaskComment mateSubTaskCommentObj)
        {
            var newItem = await _unitOfWork.MateSubTaskCommentRepository.AddAsync(mateSubTaskCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateSubTaskComment> UpdateMateSubTaskComment(MateSubTaskComment existingItem, long existingId)
        {
            await _unitOfWork.MateSubTaskCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public MateSubTaskComment GetByMateCommentId(long MateCommentId)
        {
            return _unitOfWork.MateSubTaskCommentRepository.GetMany(t => t.MateCommentId == MateCommentId).Result.FirstOrDefault();
        }
        public List<MateSubTaskComment> GetBySubTaskId(long SubTaskId)
        {
            return _unitOfWork.MateSubTaskCommentRepository.GetMany(t => t.SubTaskId == SubTaskId && t.MateComment.DeletedOn == null).Result.Include(t => t.MateComment).ToList();
        }
    }
    public partial interface IMateSubTaskCommentService : IService<MateSubTaskComment>
    {
        Task<MateSubTaskComment> CheckInsertOrUpdate(MateSubTaskComment model);
        MateSubTaskComment GetByMateCommentId(long MateCommentId);
        List<MateSubTaskComment> GetBySubTaskId(long SubTaskId);
    }
}