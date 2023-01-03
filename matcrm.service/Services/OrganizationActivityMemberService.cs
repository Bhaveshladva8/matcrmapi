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
    public partial class OrganizationActivityMemberService : Service<OrganizationActivityMember>, IOrganizationActivityMemberService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationActivityMemberService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationActivityMember> CheckInsertOrUpdate(OrganizationActivityMemberDto model)
        {
            var organizationActivityMemberObj = _mapper.Map<OrganizationActivityMember>(model);
            var existingItem = _unitOfWork.OrganizationActivityMemberRepository.GetMany(t => t.OrganizationActivityId == organizationActivityMemberObj.OrganizationActivityId && t.Email == organizationActivityMemberObj.Email).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOrganizationActivityMember(organizationActivityMemberObj);
            }
            else
            {
                return await UpdateOrganizationActivityMember(organizationActivityMemberObj, existingItem.Id);
            }
        }

        public async Task<OrganizationActivityMember> UpdateOrganizationActivityMember(OrganizationActivityMember updatedItem, long existingId)
        {

            var update = await _unitOfWork.OrganizationActivityMemberRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<OrganizationActivityMember> InsertOrganizationActivityMember(OrganizationActivityMember organizationActivityMemberObj)
        {

            var newItem = await _unitOfWork.OrganizationActivityMemberRepository.AddAsync(organizationActivityMemberObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<OrganizationActivityMember> GetAllByActivity(long OrganizationActivityId)
        {
            return _unitOfWork.OrganizationActivityMemberRepository.GetMany(t => t.OrganizationActivityId == OrganizationActivityId).Result.ToList();
        }

        public OrganizationActivityMember GetById(long Id)
        {
            return _unitOfWork.OrganizationActivityMemberRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public OrganizationActivityMember GetActivityMember(OrganizationActivityMemberDto model)
        {
            return _unitOfWork.OrganizationActivityMemberRepository.GetMany(t => t.OrganizationActivityId == model.OrganizationActivityId && t.Email == model.Email).Result.FirstOrDefault();
        }

        public OrganizationActivityMemberDto DeleteOrganizationActivityMember(OrganizationActivityMemberDto model)
        {
            var organizationActivityMemberObj = _mapper.Map<OrganizationActivityMember>(model);
            var existingItem = _unitOfWork.OrganizationActivityMemberRepository.GetMany(t => t.Id == organizationActivityMemberObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                var newItem = _unitOfWork.OrganizationActivityMemberRepository.DeleteAsync(existingItem).Result;
                _unitOfWork.CommitAsync();
                return model;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<OrganizationActivityMember>> DeleteByActivityId(long organizationActivityId)
        {
            var organizationActivityMemberList = _unitOfWork.OrganizationActivityMemberRepository.GetMany(t => t.OrganizationActivityId == organizationActivityId).Result.ToList();
            if (organizationActivityMemberList != null && organizationActivityMemberList.Count() > 0)
            {
                foreach (var existingItem in organizationActivityMemberList)
                {
                    var newItem = await _unitOfWork.OrganizationActivityMemberRepository.DeleteAsync(existingItem);
                    await _unitOfWork.CommitAsync();
                }
            }
            return organizationActivityMemberList;
        }

        public OrganizationActivityMember DeleteById(long Id)
        {
            var organizationActivityMemberObj = _unitOfWork.OrganizationActivityMemberRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (organizationActivityMemberObj != null)
            {
                var newItem = _unitOfWork.OrganizationActivityMemberRepository.DeleteAsync(organizationActivityMemberObj).Result;
                _unitOfWork.CommitAsync();
            }            
            return organizationActivityMemberObj;
        }
    }

    public partial interface IOrganizationActivityMemberService : IService<OrganizationActivityMember>
    {
        Task<OrganizationActivityMember> CheckInsertOrUpdate(OrganizationActivityMemberDto model);
        List<OrganizationActivityMember> GetAllByActivity(long OrganizationActivityId);
        OrganizationActivityMemberDto DeleteOrganizationActivityMember(OrganizationActivityMemberDto model);
        OrganizationActivityMember GetById(long Id);
        Task<List<OrganizationActivityMember>> DeleteByActivityId(long OrganizationActivityId);
        OrganizationActivityMember GetActivityMember(OrganizationActivityMemberDto model);
        OrganizationActivityMember DeleteById(long Id);
    }
}