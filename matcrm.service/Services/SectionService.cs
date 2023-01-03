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
    public partial class SectionService : Service<Section>, ISectionService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SectionService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Section> CheckInsertOrUpdate(SectionDto model)
        {
            var sectionObj = _mapper.Map<Section>(model);
            // var existingItem = _unitOfWork.SectionRepository.GetMany (t => t.Id == model.Id && t.TenantId == model.TenantId.Value && t.IsDeleted == false).Result.FirstOrDefault ();
            // var existingItem = _unitOfWork.SectionRepository.GetMany (t => t.Id == model.Id && t.TenantId == model.TenantId.Value && t.TicketNumber == model.TicketNumber && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.SectionRepository.GetMany(t => t.Id == model.Id && t.TenantId == model.TenantId.Value && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSection(sectionObj);
            }
            else
            {
                sectionObj.CreatedBy = existingItem.CreatedBy;
                sectionObj.CreatedOn = existingItem.CreatedOn;
                sectionObj.TenantId = existingItem.TenantId;
                return await UpdateSection(sectionObj, existingItem.Id);
            }
        }

        public async Task<Section> InsertSection(Section sectionObj)
        {
            sectionObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.SectionRepository.AddAsync(sectionObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Section> UpdateSection(Section existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.SectionRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<Section> GetAll()
        {
            return _unitOfWork.SectionRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<Section> GetAllByTenant(long tenantId)
        {
            return _unitOfWork.SectionRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }
        public List<Section> GetAllByTenantWithoutProject(long tenantId)
        {
            return _unitOfWork.SectionRepository.GetMany(t => t.TenantId == tenantId && t.ProjectId == null && t.IsDeleted == false).Result.ToList();
        }

        public List<Section> GetByProject(long ProjectId)
        {
            return _unitOfWork.SectionRepository.GetMany(t => t.ProjectId == ProjectId && t.IsDeleted == false).Result.ToList();
        }

        public List<Section> GetAllByTenantAndTicket(long tenantId, long ticketNumber)
        {
            return _unitOfWork.SectionRepository.GetMany(t => t.TenantId == tenantId && t.TicketNumber == ticketNumber && t.IsDeleted == false).Result.ToList();
        }

        public Section GetSectionById(long Id)
        {
            return _unitOfWork.SectionRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<List<Section>> DeleteByProject(long ProjectId)
        {
            var sectionListToDelete = _unitOfWork.SectionRepository.GetMany(u => u.ProjectId == ProjectId && u.IsDeleted == false).Result.ToList();
            foreach (var sectionToDelete in sectionListToDelete)
            {
                sectionToDelete.IsDeleted = true;
                sectionToDelete.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.SectionRepository.UpdateAsync(sectionToDelete, sectionToDelete.Id);
                await _unitOfWork.CommitAsync();
            }

            return sectionListToDelete;
        }

        public async Task<Section> DeleteSection(long Id)
        {
            var sectionObj = _unitOfWork.SectionRepository.GetMany(u => u.Id == Id && u.IsDeleted == false).Result.FirstOrDefault();
            if (sectionObj != null)
            {
                sectionObj.IsDeleted = true;
                sectionObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.SectionRepository.UpdateAsync(sectionObj, sectionObj.Id);
                await _unitOfWork.CommitAsync();
            }

            return sectionObj;
        }
    }

    public partial interface ISectionService : IService<Section>
    {
        Task<Section> CheckInsertOrUpdate(SectionDto model);
        List<Section> GetAll();
        List<Section> GetByProject(long ProjectId);
        List<Section> GetAllByTenant(long tenantId);
        List<Section> GetAllByTenantAndTicket(long tenantId, long ticketNumber);
        List<Section> GetAllByTenantWithoutProject(long tenantId);
        Section GetSectionById(long Id);
        Task<Section> DeleteSection(long Id);
        Task<List<Section>> DeleteByProject(long ProjectId);
        Task<Section> UpdateSection(Section existingItem, long existingId);
    }
}