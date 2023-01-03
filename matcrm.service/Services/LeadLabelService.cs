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

    public partial class LeadLabelService : Service<LeadLabel>, ILeadLabelService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LeadLabelService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<LeadLabel> CheckInsertOrUpdate(LeadLabelDto model)
        {
            var leadLabelObj = _mapper.Map<LeadLabel>(model);
            if (leadLabelObj.CreatedBy != null)
            {
                // var existingItem = _unitOfWork.LeadLabelRepository.GetMany (t => t.Id == obj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
                var existingItem = _unitOfWork.LeadLabelRepository.GetMany(t => t.LeadId == leadLabelObj.LeadId && t.LabelId == leadLabelObj.LabelId && t.IsDeleted == false).Result.FirstOrDefault();

                if (existingItem == null)
                {
                    return await InsertLeadLabel(leadLabelObj);
                }
                else
                {
                    leadLabelObj.CreatedOn = existingItem.CreatedOn;
                    leadLabelObj.CreatedBy = existingItem.CreatedBy;
                    leadLabelObj.Id = existingItem.Id;
                    leadLabelObj.UpdatedBy = model.UserId;
                    return await UpdateLeadLabel(leadLabelObj, existingItem.Id);
                }
            }
            else
            {
                return null;
            }


        }

        public async Task<LeadLabel> InsertLeadLabel(LeadLabel leadLabelObj)
        {
            leadLabelObj.CreatedOn = DateTime.UtcNow;

            var newItem = await _unitOfWork.LeadLabelRepository.AddAsync(leadLabelObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<LeadLabel> UpdateLeadLabel(LeadLabel leadLabelObj, long existingId)
        {
            leadLabelObj.UpdatedOn = DateTime.UtcNow;

            var update = await _unitOfWork.LeadLabelRepository.UpdateAsync(leadLabelObj, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public List<LeadLabel> GetAll()
        {
            return _unitOfWork.LeadLabelRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<LeadLabel> GetAllByTenant(int tenantId)
        {
            return _unitOfWork.LeadLabelRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList();
        }

        public List<LeadLabel> GetAllByLeadId(long LeadId)
        {
            return _unitOfWork.LeadLabelRepository.GetMany(t => t.LeadId == LeadId && t.IsDeleted == false).Result.ToList();
        }

        // public LeadLabel GetLeadLabel (string LeadLabelName) {
        //     return _unitOfWork.LeadLabelRepository.GetMany (t => t.Name == LeadLabelName && t.IsDeleted == false).Result.FirstOrDefault ();
        // }

        public LeadLabel GetLeadLabelById(long LeadLabelId)
        {
            return _unitOfWork.LeadLabelRepository.GetMany(t => t.Id == LeadLabelId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<LeadLabel> DeleteLeadLabel(long LeadLabelId)
        {
            var leadLabelObj = _unitOfWork.LeadLabelRepository.GetMany(t => t.Id == LeadLabelId).Result.FirstOrDefault();
            if (leadLabelObj != null)
            {
                leadLabelObj.IsDeleted = true;
                leadLabelObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.LeadLabelRepository.UpdateAsync(leadLabelObj, leadLabelObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return leadLabelObj;
        }

        public async Task<List<LeadLabel>> DeleteByLabel(long LabelId)
        {
            var leadLabelList = _unitOfWork.LeadLabelRepository.GetMany(t => t.LabelId == LabelId && t.IsDeleted == false).Result.ToList();
            if (leadLabelList != null && leadLabelList.Count() > 0)
            {
                foreach (var existingItem in leadLabelList)
                {
                    existingItem.IsDeleted = true;
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var newItem = await _unitOfWork.LeadLabelRepository.UpdateAsync(existingItem, existingItem.Id);                    
                }
                await _unitOfWork.CommitAsync();
            }
            return leadLabelList;
        }

    }

    public partial interface ILeadLabelService : IService<LeadLabel>
    {
        Task<LeadLabel> CheckInsertOrUpdate(LeadLabelDto model);
        List<LeadLabel> GetAll();
        List<LeadLabel> GetAllByLeadId(long LeadId);
        List<LeadLabel> GetAllByTenant(int tenantId);
        // LeadLabel GetLeadLabel (string LeadLabelName);
        LeadLabel GetLeadLabelById(long LeadLabelId);
        Task<LeadLabel> DeleteLeadLabel(long LeadLabelId);
        Task<List<LeadLabel>> DeleteByLabel(long LabelId);
    }
}