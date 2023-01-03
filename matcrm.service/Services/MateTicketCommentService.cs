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
    public partial class MateTicketCommentService : Service<MateTicketComment>, IMateTicketCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MateTicketCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MateTicketComment> CheckInsertOrUpdate(MateTicketComment mateTicketCommentObj)
        {
            var existingItem = _unitOfWork.MateTicketCommentRepository.GetMany(t => t.Id == mateTicketCommentObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMateTicketComment(mateTicketCommentObj);
            }
            else
            {
                return await UpdateMateTicketComment(mateTicketCommentObj, existingItem.Id);
            }
        }

        public async Task<MateTicketComment> InsertMateTicketComment(MateTicketComment mateTicketCommentObj)
        {
            var newItem = await _unitOfWork.MateTicketCommentRepository.AddAsync(mateTicketCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MateTicketComment> UpdateMateTicketComment(MateTicketComment existingItem, long existingId)
        {
            await _unitOfWork.MateTicketCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public MateTicketComment GetByMateCommentId(long MateCommentId)
        {
            return _unitOfWork.MateTicketCommentRepository.GetMany(t => t.MateCommentId == MateCommentId).Result.FirstOrDefault();
        }
        public List<MateTicketComment> GetByTicketId(long TicketId)
        {
            return _unitOfWork.MateTicketCommentRepository.GetMany(t => t.MateTicketId == TicketId && t.MateComment.DeletedOn == null).Result.Include(t => t.MateComment).ToList();
        }
    }
    public partial interface IMateTicketCommentService : IService<MateTicketComment>
    {
        Task<MateTicketComment> CheckInsertOrUpdate(MateTicketComment model);
        MateTicketComment GetByMateCommentId(long MateCommentId);
        List<MateTicketComment> GetByTicketId(long TicketId);
    }
}