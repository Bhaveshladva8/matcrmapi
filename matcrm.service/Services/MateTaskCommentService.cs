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
    public partial class MateTaskCommentService : Service<MateTaskComment>, IMateTaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateTaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateTaskComment> CheckInsertOrUpdate(MateTaskComment mateTaskCommentObj)
        {
            var existingItem = _unitOfWork.MateTaskCommentRepository.GetMany(t => t.Id == mateTaskCommentObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateTaskComment(mateTaskCommentObj);
            }
            else
            {
                return await UpdateMateTaskComment(mateTaskCommentObj, existingItem.Id);
            }
        }

        public async Task<MateTaskComment> InsertMateTaskComment(MateTaskComment mateTaskCommentObj)
        {
            var newItem = await _unitOfWork.MateTaskCommentRepository.AddAsync(mateTaskCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateTaskComment> UpdateMateTaskComment(MateTaskComment existingItem, long existingId)
        {
            await _unitOfWork.MateTaskCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public MateTaskComment GetByMateCommentId(long MateCommentId)
        {
            return _unitOfWork.MateTaskCommentRepository.GetMany(t => t.MateCommentId == MateCommentId).Result.FirstOrDefault();
        }
        public List<MateTaskComment> GetByTaskId(long TaskId)
        {
            return _unitOfWork.MateTaskCommentRepository.GetMany(t => t.TaskId == TaskId && t.MateComment.DeletedOn == null).Result.Include(t => t.MateComment).ToList();
        }
    }
    public partial interface IMateTaskCommentService : IService<MateTaskComment>
    {
        Task<MateTaskComment> CheckInsertOrUpdate(MateTaskComment model);
        MateTaskComment GetByMateCommentId(long MateCommentId);
        List<MateTaskComment> GetByTaskId(long TaskId);
    }
}