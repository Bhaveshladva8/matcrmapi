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
    public partial class CustomerActivityMemberService : Service<CustomerActivityMember>, ICustomerActivityMemberService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerActivityMemberService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerActivityMember> CheckInsertOrUpdate(CustomerActivityMemberDto model)
        {
            var customerActivityMemberObj = _mapper.Map<CustomerActivityMember>(model);
            var existingItem = _unitOfWork.CustomerActivityMemberRepository.GetMany(t => t.CustomerActivityId == customerActivityMemberObj.CustomerActivityId && t.Email == customerActivityMemberObj.Email).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCustomerActivityMember(customerActivityMemberObj);
            }
            else
            {
                return await UpdateCustomerActivityMember(customerActivityMemberObj, existingItem.Id);
            }
        }

        public async Task<CustomerActivityMember> UpdateCustomerActivityMember(CustomerActivityMember updatedItem, long existingId)
        {

            var update = await _unitOfWork.CustomerActivityMemberRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<CustomerActivityMember> InsertCustomerActivityMember(CustomerActivityMember customerActivityMemberObj)
        {

            var newItem = await _unitOfWork.CustomerActivityMemberRepository.AddAsync(customerActivityMemberObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<CustomerActivityMember> GetAllByActivity(long customerActivityId)
        {
            return _unitOfWork.CustomerActivityMemberRepository.GetMany(t => t.CustomerActivityId == customerActivityId).Result.ToList();
        }

        public CustomerActivityMember GetById(long Id)
        {
            return _unitOfWork.CustomerActivityMemberRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public CustomerActivityMember GetActivityMember(CustomerActivityMemberDto model)
        {
            return _unitOfWork.CustomerActivityMemberRepository.GetMany(t => t.CustomerActivityId == model.CustomerActivityId && t.Email == model.Email).Result.FirstOrDefault();
        }

        public async Task<CustomerActivityMemberDto> DeleteCustomerActivityMember(CustomerActivityMemberDto model)
        {
            var CustomerActivityMemberObj = _mapper.Map<CustomerActivityMember>(model);
            var existingItem = _unitOfWork.CustomerActivityMemberRepository.GetMany(t => t.Id == CustomerActivityMemberObj.Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                var newItem = await _unitOfWork.CustomerActivityMemberRepository.DeleteAsync(existingItem);
                await _unitOfWork.CommitAsync();
                return model;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<CustomerActivityMember>> DeleteByActivityId(long customerActivityId)
        {
            var customerActivityMemberList = _unitOfWork.CustomerActivityMemberRepository.GetMany(t => t.CustomerActivityId == customerActivityId).Result.ToList();
            if(customerActivityMemberList != null && customerActivityMemberList.Count() > 0)
            {
                foreach (var existingItem in customerActivityMemberList)
                {
                    if (existingItem != null)
                    {
                        var newItem = await _unitOfWork.CustomerActivityMemberRepository.DeleteAsync(existingItem);
                    }
                    else
                    {
                        return null;
                    }
                }
                await _unitOfWork.CommitAsync();
            }
            return customerActivityMemberList;
        }

        public CustomerActivityMember DeleteById(long Id)
        {
            var customerActivityMemberObj = _unitOfWork.CustomerActivityMemberRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (customerActivityMemberObj != null)
            {
                var newItem = _unitOfWork.CustomerActivityMemberRepository.DeleteAsync(customerActivityMemberObj).Result;
                _unitOfWork.CommitAsync();
            }
            else
            {
                return null;
            }
            return customerActivityMemberObj;
        }

    }
}

public partial interface ICustomerActivityMemberService : IService<CustomerActivityMember>
{
    Task<CustomerActivityMember> CheckInsertOrUpdate(CustomerActivityMemberDto model);
    List<CustomerActivityMember> GetAllByActivity(long customerActivityId);
    Task<CustomerActivityMemberDto> DeleteCustomerActivityMember(CustomerActivityMemberDto model);
    CustomerActivityMember GetById(long Id);
    Task<List<CustomerActivityMember>> DeleteByActivityId(long customerActivityId);
    CustomerActivityMember GetActivityMember(CustomerActivityMemberDto model);
    CustomerActivityMember DeleteById(long Id);
}