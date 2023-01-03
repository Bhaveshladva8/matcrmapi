using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class LeadNoteService : Service<LeadNote>, ILeadNoteService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LeadNoteService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public LeadNote CheckInsertOrUpdate(LeadNoteDto model)
        {
            var leadNoteObj = _mapper.Map<LeadNote>(model);
            var existingItem = _unitOfWork.LeadNoteRepository.GetMany(t => t.Id == leadNoteObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertLeadNote(leadNoteObj);
            }
            else
            {
                leadNoteObj.CreatedOn = existingItem.CreatedOn;
                leadNoteObj.CreatedBy = existingItem.CreatedBy;
                leadNoteObj.TenantId = existingItem.TenantId;
                leadNoteObj.LeadId = existingItem.LeadId;
                return UpdateLeadNote(leadNoteObj, existingItem.Id);
            }
        }

        public LeadNote UpdateLeadNote(LeadNote leadNoteObj, long existingId)
        {
            leadNoteObj.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.LeadNoteRepository.UpdateAsync(leadNoteObj, existingId).Result;
            _unitOfWork.CommitAsync();

            return update;
        }

        public LeadNote InsertLeadNote(LeadNote leadNoteObj)
        {
            leadNoteObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.LeadNoteRepository.Add(leadNoteObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<LeadNote> GetAll()
        {
            return _unitOfWork.LeadNoteRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public LeadNote GetById(long Id)
        {
            return _unitOfWork.LeadNoteRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<LeadNote> GetByTenant(int tenantId)
        {
            return _unitOfWork.LeadNoteRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<LeadNote> GetByLead(long LeadId)
        {
            return _unitOfWork.LeadNoteRepository.GetMany(t => t.LeadId == LeadId && t.IsDeleted == false).Result.OrderByDescending(t => t.CreatedOn).ToList();
        }

        public List<LeadNote> GetByUser(int userId)
        {
            return _unitOfWork.LeadNoteRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public LeadNote DeleteLeadNote(LeadNoteDto model)
        {
            var leadNoteObj = _mapper.Map<LeadNote>(model);
            var leadNote = _unitOfWork.LeadNoteRepository.GetMany(t => t.Id == leadNoteObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (leadNote != null)
            {
                leadNote.IsDeleted = true;
                leadNote.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.LeadNoteRepository.UpdateAsync(leadNote, leadNote.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public List<LeadNote> DeleteByLead(long LeadId)
        {
            var leadNoteList = _unitOfWork.LeadNoteRepository.GetMany(t => t.LeadId == LeadId && t.IsDeleted == false).Result.ToList();
            if (leadNoteList != null && leadNoteList.Count() > 0)
            {
                foreach (var existingItem in leadNoteList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = _unitOfWork.LeadNoteRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                }
                _unitOfWork.CommitAsync();
            }
            return leadNoteList;
        }
    }

    public partial interface ILeadNoteService : IService<LeadNote>
    {
        LeadNote CheckInsertOrUpdate(LeadNoteDto model);
        List<LeadNote> GetAll();
        List<LeadNote> GetByTenant(int tenantId);
        List<LeadNote> GetByLead(long LeadId);
        LeadNote DeleteLeadNote(LeadNoteDto model);
        List<LeadNote> DeleteByLead(long LeadId);
        LeadNote GetById(long Id);
        List<LeadNote> GetByUser(int userId);
    }
}