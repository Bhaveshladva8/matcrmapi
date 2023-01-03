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
    public partial class CRMNotesService : Service<CRMNotes>, ICRMNotesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CRMNotesService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CRMNotes> CheckInsertOrUpdate(CRMNotes CRMNotesObj)
        {
            var existingItem = _unitOfWork.CRMNotesRepository.GetMany(t => t.Id == CRMNotesObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCRMNotes(CRMNotesObj);
            }
            else
            {
                CRMNotesObj.CreatedBy = existingItem.CreatedBy;
                CRMNotesObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateCRMNotes(CRMNotesObj, existingItem.Id);
            }
        }
        public async Task<CRMNotes> InsertCRMNotes(CRMNotes CRMNotesObj)
        {
            CRMNotesObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CRMNotesRepository.AddAsync(CRMNotesObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CRMNotes> UpdateCRMNotes(CRMNotes existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CRMNotesRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<CRMNotes> DeleteById(long Id)
        {
            var CRMNotesObj = _unitOfWork.CRMNotesRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (CRMNotesObj != null)
            {
                CRMNotesObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.CRMNotesRepository.UpdateAsync(CRMNotesObj, CRMNotesObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return CRMNotesObj;
        }
        public List<CRMNotes> GetAllByClientId(long ClientId)
        {
            return _unitOfWork.CRMNotesRepository.GetMany(t => t.DeletedOn == null && t.ClientId == ClientId).Result.Include(t => t.ClientUser).ToList();
        }
        public CRMNotes GetById(long Id)
        {
            return _unitOfWork.CRMNotesRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.ClientUser).Include(t => t.MateTimeRecord).Include(t => t.SatisficationLevel).FirstOrDefault();
        }
        public async Task<List<CRMNotes>> DeleteByClientUserId(long ClientUserId)
        {
            var cRMNotesList = _unitOfWork.CRMNotesRepository.GetMany(t => t.ClientUserId == ClientUserId && t.DeletedOn == null).Result.ToList();
            if (cRMNotesList != null && cRMNotesList.Count() > 0)
            {
                foreach (var existingItem in cRMNotesList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.CRMNotesRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return cRMNotesList;
        }
        public async Task<List<CRMNotes>> DeleteByClientId(long ClientId)
        {
            var cRMNotesList = _unitOfWork.CRMNotesRepository.GetMany(t => t.ClientId == ClientId && t.DeletedOn == null).Result.ToList();
            if (cRMNotesList != null && cRMNotesList.Count() > 0)
            {
                foreach (var existingItem in cRMNotesList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.CRMNotesRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return cRMNotesList;
        }
    }
    public partial interface ICRMNotesService : IService<CRMNotes>
    {
        Task<CRMNotes> CheckInsertOrUpdate(CRMNotes model);
        Task<CRMNotes> DeleteById(long Id);
        List<CRMNotes> GetAllByClientId(long ClientId);
        CRMNotes GetById(long Id);
        Task<List<CRMNotes>> DeleteByClientUserId(long ClientUserId);
        Task<List<CRMNotes>> DeleteByClientId(long ClientId);
    }
}