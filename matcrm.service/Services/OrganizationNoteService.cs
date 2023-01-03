using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OrganizationNoteService : Service<OrganizationNote>, IOrganizationNoteService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationNoteService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationNote> CheckInsertOrUpdate(OrganizationNoteDto model)
        {
            var organizationNoteObj = _mapper.Map<OrganizationNote>(model);
            var existingItem = _unitOfWork.OrganizationNoteRepository.GetMany(t => t.Id == organizationNoteObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOrganizationNote(organizationNoteObj);
            }
            else
            {
                organizationNoteObj.CreatedOn = existingItem.CreatedOn;
                organizationNoteObj.CreatedBy = existingItem.CreatedBy;
                organizationNoteObj.TenantId = existingItem.TenantId;
                organizationNoteObj.OrganizationId = existingItem.OrganizationId;
                return await UpdateOrganizationNote(organizationNoteObj, existingItem.Id);
            }
        }

        public async Task<OrganizationNote> UpdateOrganizationNote(OrganizationNote updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OrganizationNoteRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OrganizationNote> InsertOrganizationNote(OrganizationNote organizationNoteObj)
        {
            organizationNoteObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OrganizationNoteRepository.AddAsync(organizationNoteObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OrganizationNote> GetAll()
        {
            return _unitOfWork.OrganizationNoteRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public OrganizationNote GetById(long Id)
        {
            return _unitOfWork.OrganizationNoteRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<OrganizationNote> GetByTenant(int tenantId)
        {
            return _unitOfWork.OrganizationNoteRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<OrganizationNote> GetByOrganization(long OrganizationId)
        {
            return _unitOfWork.OrganizationNoteRepository.GetMany(t => t.OrganizationId == OrganizationId && t.IsDeleted == false).Result.OrderByDescending(t => t.CreatedOn).ToList();
        }

        public List<OrganizationNote> GetByUser(int userId)
        {
            return _unitOfWork.OrganizationNoteRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public OrganizationNote DeleteOrganizationNote(OrganizationNoteDto model)
        {
            var organizationNoteObj = _mapper.Map<OrganizationNote>(model);
            var existingItem = _unitOfWork.OrganizationNoteRepository.GetMany(t => t.Id == organizationNoteObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OrganizationNoteRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<OrganizationNote>> DeleteByOrganization(long OrganizationId)
        {
            var organizationNoteList = _unitOfWork.OrganizationNoteRepository.GetMany(t => t.OrganizationId == OrganizationId && t.IsDeleted == false).Result.ToList();
            if (organizationNoteList != null && organizationNoteList.Count() > 0)
            {
                foreach (var existingItem in organizationNoteList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.OrganizationNoteRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return organizationNoteList;
        }
    }

    public partial interface IOrganizationNoteService : IService<OrganizationNote>
    {
        Task<OrganizationNote> CheckInsertOrUpdate(OrganizationNoteDto model);
        List<OrganizationNote> GetAll();
        List<OrganizationNote> GetByTenant(int tenantId);
        List<OrganizationNote> GetByOrganization(long OrganizationId);
        OrganizationNote DeleteOrganizationNote(OrganizationNoteDto model);
        OrganizationNote GetById(long Id);
        List<OrganizationNote> GetByUser(int userId);
        Task<List<OrganizationNote>> DeleteByOrganization(long OrganizationId);
    }
}