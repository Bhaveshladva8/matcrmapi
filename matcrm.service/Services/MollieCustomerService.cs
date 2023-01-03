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
    public partial class MollieCustomerService : Service<MollieCustomer>, IMollieCustomerService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MollieCustomerService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MollieCustomer> CheckInsertOrUpdate(MollieCustomerDto model)
        {
            var mollieCustomerObj = _mapper.Map<MollieCustomer>(model);
            var existingItem = _unitOfWork.MollieCustomerRepository.GetMany(t => t.UserId == mollieCustomerObj.UserId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMollieCustomer(mollieCustomerObj);
            }
            else
            {
                mollieCustomerObj.CreatedOn = existingItem.CreatedOn;
                mollieCustomerObj.UserId = existingItem.UserId;
                mollieCustomerObj.CustomerId = existingItem.CustomerId;
                mollieCustomerObj.Id = existingItem.Id;
                return await UpdateMollieCustomer(mollieCustomerObj, existingItem.Id);
            }
        }

        public async Task<MollieCustomer> UpdateMollieCustomer(MollieCustomer updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.MollieCustomerRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<MollieCustomer> InsertMollieCustomer(MollieCustomer mollieCustomerObj)
        {
            mollieCustomerObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MollieCustomerRepository.AddAsync(mollieCustomerObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<MollieCustomer> GetAll()
        {
            return _unitOfWork.MollieCustomerRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }

        public MollieCustomer GetById(long Id)
        {
            return _unitOfWork.MollieCustomerRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.Include(t => t.User).FirstOrDefault();
        }

        public MollieCustomer GetByUser(long UserId)
        {
            return _unitOfWork.MollieCustomerRepository.GetMany(t => t.DeletedOn == null && t.UserId == UserId).Result.Include(t => t.User).FirstOrDefault();
        }

        public MollieCustomer GetByCustomerId(string CustomerId)
        {
            return _unitOfWork.MollieCustomerRepository.GetMany(t => t.DeletedOn == null && t.CustomerId == CustomerId).Result.Include(t => t.User).FirstOrDefault();
        }

        public MollieCustomer DeleteMollieCustomer(long Id)
        {
            var mollieCustomerObj = _unitOfWork.MollieCustomerRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mollieCustomerObj != null)
            {
                mollieCustomerObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MollieCustomerRepository.UpdateAsync(mollieCustomerObj, mollieCustomerObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IMollieCustomerService : IService<MollieCustomer>
    {
        Task<MollieCustomer> CheckInsertOrUpdate(MollieCustomerDto model);
        List<MollieCustomer> GetAll();
        MollieCustomer DeleteMollieCustomer(long Id);
        MollieCustomer GetById(long Id);
        MollieCustomer GetByUser(long UserId);
        MollieCustomer GetByCustomerId(string CustomerId);
    }
}