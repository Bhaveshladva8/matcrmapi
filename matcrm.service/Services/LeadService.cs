using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class LeadService : Service<Lead>, ILeadService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LeadService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Lead CheckInsertOrUpdate(LeadDto model)
        {
            var leadObj = _mapper.Map<Lead>(model);
            var existingItem = _unitOfWork.LeadRepository.GetMany(t => t.Id == leadObj.Id && t.TenantId == leadObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertLead(leadObj);
            }
            else
            {
                leadObj.CreatedOn = existingItem.CreatedOn;
                leadObj.CreatedBy = existingItem.CreatedBy;
                leadObj.TenantId = existingItem.TenantId;
                return UpdateLead(leadObj, existingItem.Id);
            }
        }

        public Lead UpdateLead(Lead leadObj, long existingId)
        {
            leadObj.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.LeadRepository.UpdateAsync(leadObj, existingId).Result;
            _unitOfWork.CommitAsync();

            return update;
        }

        public Lead InsertLead(Lead leadObj)
        {
            leadObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.LeadRepository.Add(leadObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<Lead> GetAll()
        {
            return _unitOfWork.LeadRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public Lead GetById(long Id)
        {
            return _unitOfWork.LeadRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.Include(t => t.Customer).Include(t => t.Organization).FirstOrDefault();
        }

        public List<Lead> GetAllByTenant(int tenantId)
        {
            return _unitOfWork.LeadRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.OrderByDescending(t => t.CreatedBy).ToList();
        }

        public List<Lead> GetByUser(int userId)
        {
            return _unitOfWork.LeadRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        public long GetLeadCount(int tenantId)
        {
            return _unitOfWork.LeadRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.Count();
        }

        // public Lead DeleteLead(LeadDto model)
        // {
        //     var leadObj = _mapper.Map<Lead>(model);
        //     var lead = _unitOfWork.LeadRepository.GetMany(t => t.Id == leadObj.Id).Result.FirstOrDefault();
        //     if (lead != null)
        //     {
        //         lead.IsDeleted = true;
        //         lead.DeletedOn = DateTime.UtcNow;
        //         var newItem = _unitOfWork.LeadRepository.UpdateAsync(lead, lead.Id).Result;
        //         _unitOfWork.CommitAsync();
        //         return newItem;
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }
        public async Task<Lead> DeleteLead(long Id)
        {            
            var leadObj = _unitOfWork.LeadRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (leadObj != null)
            {
                leadObj.IsDeleted = true;
                leadObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.LeadRepository.UpdateAsync(leadObj, leadObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return leadObj;
        }
    }

    public partial interface ILeadService : IService<Lead>
    {
        Lead CheckInsertOrUpdate(LeadDto model);
        List<Lead> GetAll();
        List<Lead> GetAllByTenant(int tenantId);
        // Lead DeleteLead(LeadDto model);
        Task<Lead> DeleteLead(long Id);
        Lead GetById(long Id);
        long GetLeadCount(int tenantId);
        List<Lead> GetByUser(int userId);
    }
}