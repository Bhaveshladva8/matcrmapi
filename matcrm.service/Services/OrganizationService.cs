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
    public partial class OrganizationService : Service<Organization>, IOrganizationService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Organization> CheckInsertOrUpdate(OrganizationDto model)
        {
            var organizationObj = _mapper.Map<Organization>(model);

            Organization? existingItem = null;
            if (model.WeClappOrganizationId != null)
            {
                if (model.Id != null)
                {
                    existingItem = _unitOfWork.OrganizationRepository.GetMany(t => t.Id == organizationObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
                }
                else
                {
                    existingItem = _unitOfWork.OrganizationRepository.GetMany(t => t.WeClappOrganizationId == organizationObj.WeClappOrganizationId && t.CreatedBy == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
                }
            }
            else
            {
                existingItem = _unitOfWork.OrganizationRepository.GetMany(t => t.Id == organizationObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            }

            // var existingItem = _unitOfWork.OrganizationRepository.GetMany(t => t.Id == obj.Id && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOrganization(organizationObj);
            }
            else
            {
                organizationObj.CreatedOn = existingItem.CreatedOn;
                organizationObj.CreatedBy = existingItem.CreatedBy;
                organizationObj.TenantId = existingItem.TenantId;
                organizationObj.Id = existingItem.Id;
                return await UpdateOrganization(organizationObj, existingItem.Id);
            }
        }

        public async Task<Organization> UpdateOrganization(Organization updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OrganizationRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<Organization> InsertOrganization(Organization organizationObj)
        {
            organizationObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OrganizationRepository.AddAsync(organizationObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<Organization> GetAll()
        {
            return _unitOfWork.OrganizationRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public Organization GetById(long Id)
        {
            return _unitOfWork.OrganizationRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<Organization> GetAllByTenant(int tenantId)
        {
            return _unitOfWork.OrganizationRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.OrderByDescending(t => t.CreatedBy).ToList();
        }

        public List<Organization> GetByUser(int userId)
        {
            return _unitOfWork.OrganizationRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public long GetOrganizationCount(int tenantId)
        {
            return _unitOfWork.OrganizationRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.Count();
        }

        // public Organization DeleteOrganization(OrganizationDto model)
        // {
        //     var organizationObj = _mapper.Map<Organization>(model);
        //     var organization = _unitOfWork.OrganizationRepository.GetMany(t => t.Id == organizationObj.Id).Result.FirstOrDefault();
        //     if (organization != null)
        //     {
        //         organization.IsDeleted = true;
        //         organization.DeletedOn = DateTime.UtcNow;
        //         var newItem = _unitOfWork.OrganizationRepository.UpdateAsync(organization, organization.Id).Result;
        //         _unitOfWork.CommitAsync();
        //         return newItem;
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }

        public async Task<Organization> DeleteOrganization(long Id)
        {
            var organizationObj = _unitOfWork.OrganizationRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (organizationObj != null)
            {
                organizationObj.IsDeleted = true;
                organizationObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.OrganizationRepository.UpdateAsync(organizationObj, organizationObj.Id);
                await _unitOfWork.CommitAsync();
                
            }
           return  organizationObj;
        }
    }

    public partial interface IOrganizationService : IService<Organization>
    {
        Task<Organization> CheckInsertOrUpdate(OrganizationDto model);
        List<Organization> GetAll();
        List<Organization> GetAllByTenant(int tenantId);
        long GetOrganizationCount(int tenantId);
        //Organization DeleteOrganization(OrganizationDto model);
        Task<Organization> DeleteOrganization(long Id);
        Organization GetById(long Id);
        List<Organization> GetByUser(int userId);
    }
}