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
    public partial class LeadActivityMemberService : Service<LeadActivityMember>, ILeadActivityMemberService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LeadActivityMemberService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public LeadActivityMember CheckInsertOrUpdate(LeadActivityMemberDto model)
        {
            var leadActivityMemberObj = _mapper.Map<LeadActivityMember>(model);
            var existingItem = _unitOfWork.LeadActivityMemberRepository.GetMany(t => t.LeadActivityId == leadActivityMemberObj.LeadActivityId && t.Email == leadActivityMemberObj.Email).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertLeadActivityMember(leadActivityMemberObj);
            }
            else
            {
                return UpdateLeadActivityMember(existingItem, existingItem.Id);
            }
        }

        public LeadActivityMember UpdateLeadActivityMember(LeadActivityMember existingItem, long existingId)
        {

            var update = _unitOfWork.LeadActivityMemberRepository.UpdateAsync(existingItem, existingId).Result;
            _unitOfWork.CommitAsync();

            return update;
        }

        public LeadActivityMember InsertLeadActivityMember(LeadActivityMember leadActivityMemberObj)
        {

            var newItem = _unitOfWork.LeadActivityMemberRepository.Add(leadActivityMemberObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<LeadActivityMember> GetAllByActivity(long LeadActivityId)
        {
            return _unitOfWork.LeadActivityMemberRepository.GetMany(t => t.LeadActivityId == LeadActivityId).Result.ToList();
        }

        public LeadActivityMember GetById(long Id)
        {
            return _unitOfWork.LeadActivityMemberRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public LeadActivityMember GetActivityMember(LeadActivityMemberDto model)
        {
            return _unitOfWork.LeadActivityMemberRepository.GetMany(t => t.LeadActivityId == model.LeadActivityId && t.Email == model.Email).Result.FirstOrDefault();
        }

        public LeadActivityMemberDto DeleteLeadActivityMember(LeadActivityMemberDto model)
        {
            var leadActivityMemberObj = _mapper.Map<LeadActivityMember>(model);
            var existingItem = _unitOfWork.LeadActivityMemberRepository.GetMany(t => t.Id == leadActivityMemberObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                var newItem = _unitOfWork.LeadActivityMemberRepository.DeleteAsync(existingItem).Result;
                _unitOfWork.CommitAsync();
                return model;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<LeadActivityMember>> DeleteByActivityId(long LeadActivityId)
        {
            var leadActivityMembersList = _unitOfWork.LeadActivityMemberRepository.GetMany(t => t.LeadActivityId == LeadActivityId).Result.ToList();
            if (leadActivityMembersList != null && leadActivityMembersList.Count() > 0)
            {
                foreach (var existingItem in leadActivityMembersList)
                {

                    var newItem = await _unitOfWork.LeadActivityMemberRepository.DeleteAsync(existingItem);

                }
                await _unitOfWork.CommitAsync();
            }
            return leadActivityMembersList;
        }

        public LeadActivityMember DeleteById(long Id)
        {
            var leadActivityMemberObj = _unitOfWork.LeadActivityMemberRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (leadActivityMemberObj != null)
            {
                var newItem = _unitOfWork.LeadActivityMemberRepository.DeleteAsync(leadActivityMemberObj).Result;
                _unitOfWork.CommitAsync();
            }
            return leadActivityMemberObj;
        }
    }

    public partial interface ILeadActivityMemberService : IService<LeadActivityMember>
    {
        LeadActivityMember CheckInsertOrUpdate(LeadActivityMemberDto model);
        List<LeadActivityMember> GetAllByActivity(long LeadActivityId);
        LeadActivityMemberDto DeleteLeadActivityMember(LeadActivityMemberDto model);
        LeadActivityMember GetById(long Id);
        Task<List<LeadActivityMember>> DeleteByActivityId(long LeadActivityId);
        LeadActivityMember GetActivityMember(LeadActivityMemberDto model);
        LeadActivityMember DeleteById(long Id);
    }
}