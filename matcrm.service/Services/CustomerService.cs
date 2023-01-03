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
    public partial class CustomerService : Service<Customer>, ICustomerService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Customer> CheckInsertOrUpdate(CustomerDto model)
        {
            var customerObj = _mapper.Map<Customer>(model);
            Customer? existingItem = null;
            if (model.WeClappCustomerId != null)
            {
                if (model.Id != null)
                {
                    existingItem = _unitOfWork.CustomerRepository.GetMany(t => t.Id == customerObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
                }
                else
                {
                    existingItem = _unitOfWork.CustomerRepository.GetMany(t => t.WeClappCustomerId == customerObj.WeClappCustomerId && t.CreatedBy == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
                }

            }
            else
            {
                existingItem = _unitOfWork.CustomerRepository.GetMany(t => t.Id == customerObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            }
            // var existingItem = _unitOfWork.CustomerRepository.GetMany (t => t.CustomerNumber == obj.CustomerNumber && t.IsDeleted == false).Result.FirstOrDefault ();

            if (existingItem == null)
            {
                return await InsertCustomer(customerObj);
            }
            else
            {
                // existingItem.CustomerNumber = obj.CustomerNumber;
                existingItem.Name = customerObj.Name;
                // existingItem.LabelId = obj.LabelId;
                existingItem.FirstName = customerObj.FirstName;
                existingItem.LastName = customerObj.LastName;
                existingItem.SalutationId = customerObj.SalutationId;
                existingItem.OrganizationId = customerObj.OrganizationId;
                existingItem.WeClappCustomerId = customerObj.WeClappCustomerId;
                return await UpdateCustomer(existingItem, existingItem.Id);
            }
        }

        public async Task<Customer> InsertCustomer(Customer customerObj)
        {
            customerObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomerRepository.AddAsync(customerObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Customer> UpdateCustomer(Customer existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomerRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<Customer> GetAllCustomer()
        {
            return _unitOfWork.CustomerRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public Customer GetById(long Id)
        {
            return _unitOfWork.CustomerRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }

        public List<Customer> GetAllByTenant(int tenantId)
        {
            return _unitOfWork.CustomerRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.OrderByDescending(t => t.CreatedBy).ToList();
        }

         public long GetCustomerCount(int tenantId)
        {
            return _unitOfWork.CustomerRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.Count();
        }

        public List<Customer> GetByUser(int userId)
        {
            return _unitOfWork.CustomerRepository.GetMany(t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList();
        }

        // public Customer DeleteCustomer(CustomerDto model)
        // {
        //     var customerObj = _mapper.Map<Customer>(model);
        //     var existingItem = _unitOfWork.CustomerRepository.GetMany(t => t.Id == customerObj.Id).Result.FirstOrDefault();
        //     if (existingItem != null)
        //     {
        //         existingItem.IsDeleted = true;
        //         existingItem.DeletedOn = DateTime.UtcNow;
        //         var newItem = _unitOfWork.CustomerRepository.UpdateAsync(existingItem, existingItem.Id).Result;
        //         _unitOfWork.CommitAsync();
        //         return newItem;
        //     }
        //     else
        //     {
        //         return null;
        //     }
        // }
        public async Task<Customer> DeleteCustomer(long Id)
        {
            var customerObj = _unitOfWork.CustomerRepository.GetMany(u => u.Id == Id && u.IsDeleted == false).Result.FirstOrDefault();
            if (customerObj != null)
            {
                customerObj.IsDeleted = true;
                customerObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.CustomerRepository.UpdateAsync(customerObj, customerObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return customerObj;
        }
    }

    public partial interface ICustomerService : IService<Customer>
    {
        Task<Customer> CheckInsertOrUpdate(CustomerDto model);
        List<Customer> GetAllCustomer();
        //Customer DeleteCustomer(CustomerDto model);
        Task<Customer> DeleteCustomer(long Id);
        Customer GetById(long Id);
        List<Customer> GetAllByTenant(int tenantId);

        long GetCustomerCount(int tenantId);
        List<Customer> GetByUser(int userId);
    }
}