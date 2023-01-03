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
    public partial class OrganizationLabelService : Service<OrganizationLabel>, IOrganizationLabelService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationLabelService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationLabel> CheckInsertOrUpdate(OrganizationLabelDto model)
        {
            var organizationLabelObj = _mapper.Map<OrganizationLabel>(model);
            // var existingItem = _unitOfWork.OrganizationLabelRepository.GetMany (t => t.Id == obj.Id && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.OrganizationLabelRepository.GetMany(t => t.OrganizationId == organizationLabelObj.OrganizationId && t.TenantId == organizationLabelObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertLabel(organizationLabelObj);
            }
            else
            {
                organizationLabelObj.CreatedOn = existingItem.CreatedOn;
                organizationLabelObj.CreatedBy = existingItem.CreatedBy;
                organizationLabelObj.TenantId = existingItem.TenantId;
                organizationLabelObj.Id = existingItem.Id;
                return await UpdateLabel(organizationLabelObj, existingItem.Id);
            }
        }

        public async Task<OrganizationLabel> UpdateLabel(OrganizationLabel updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OrganizationLabelRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OrganizationLabel> InsertLabel(OrganizationLabel organizationLabelObj)
        {
            organizationLabelObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OrganizationLabelRepository.AddAsync(organizationLabelObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OrganizationLabel> GetAll()
        {
            return _unitOfWork.OrganizationLabelRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public OrganizationLabel GetById(long Id)
        {
            return _unitOfWork.OrganizationLabelRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<OrganizationLabel> GetByTenant(int tenantId)
        {
            return _unitOfWork.OrganizationLabelRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<OrganizationLabel> GetAllDefault()
        {
            return _unitOfWork.OrganizationLabelRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<OrganizationLabel> GetByUser(int userId)
        {
            return _unitOfWork.OrganizationLabelRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public List<OrganizationLabel> GetByUserAndDefault(int userId)
        {
            return _unitOfWork.OrganizationLabelRepository.GetMany(t => (t.CreatedBy == userId || t.CreatedBy == null) && t.IsDeleted == false).Result.ToList();
        }

        public OrganizationLabel GetByOrganization(long OrganizationId)
        {
            return _unitOfWork.OrganizationLabelRepository.GetMany(t => t.IsDeleted == false && t.OrganizationId == OrganizationId).Result.FirstOrDefault();
        }

        public OrganizationLabel DeleteLabel(OrganizationLabelDto model)
        {
            var organizationLabelObj = _mapper.Map<OrganizationLabel>(model);
            var existingItem = _unitOfWork.OrganizationLabelRepository.GetMany(t => t.Id == organizationLabelObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OrganizationLabelRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<OrganizationLabel>> DeleteByLabel(long LabelId)
        {
            var organizationLabelList = _unitOfWork.OrganizationLabelRepository.GetMany(t => t.LabelId == LabelId && t.IsDeleted == false).Result.ToList();
            if (organizationLabelList != null && organizationLabelList.Count() > 0)
            {
                foreach (var existingItem in organizationLabelList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.OrganizationLabelRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return organizationLabelList;
        }
    }

    public partial interface IOrganizationLabelService : IService<OrganizationLabel>
    {
        Task<OrganizationLabel> CheckInsertOrUpdate(OrganizationLabelDto model);
        List<OrganizationLabel> GetAll();
        List<OrganizationLabel> GetByTenant(int tenantId);
        OrganizationLabel DeleteLabel(OrganizationLabelDto model);
        OrganizationLabel GetById(long Id);
        List<OrganizationLabel> GetByUser(int userId);
        List<OrganizationLabel> GetAllDefault();
        List<OrganizationLabel> GetByUserAndDefault(int userId);
        Task<List<OrganizationLabel>> DeleteByLabel(long LabelId);
        OrganizationLabel GetByOrganization(long OrganizationId);
    }
}