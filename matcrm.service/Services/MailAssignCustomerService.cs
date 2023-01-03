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
    public partial class MailAssignCustomerService : Service<MailAssignCustomer>, IMailAssignCustomerService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MailAssignCustomerService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MailAssignCustomer> CheckInsertOrUpdate(MailAssignCustomerDto model)
        {
            var mailAssignCustomerObj = _mapper.Map<MailAssignCustomer>(model);
            var existingItem = _unitOfWork.MailAssignCustomerRepository.GetMany(t => t.ThreadId == mailAssignCustomerObj.ThreadId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMailAssignCustomer(mailAssignCustomerObj);
            }
            else
            {
                existingItem.CustomerId = mailAssignCustomerObj.CustomerId;
                existingItem.UpdatedBy = mailAssignCustomerObj.CreatedBy;
                return await UpdateMailAssignCustomer(existingItem, existingItem.Id);
            }
        }

        public async Task<MailAssignCustomer> InsertMailAssignCustomer(MailAssignCustomer mailAssignCustomerObj)
        {
            mailAssignCustomerObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.MailAssignCustomerRepository.Add(mailAssignCustomerObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MailAssignCustomer> UpdateMailAssignCustomer(MailAssignCustomer existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MailAssignCustomerRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public MailAssignCustomer GetMailAssignCustomerByThread(string ThreadId)
        {
            return _unitOfWork.MailAssignCustomerRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.Include(t => t.Customer).FirstOrDefault();
        }

        public List<MailAssignCustomer> GetAllByCustomer(long CustomerId)
        {
            return _unitOfWork.MailAssignCustomerRepository.GetMany(t => t.CustomerId == CustomerId && t.DeletedOn == null).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.Customer).ToList();
        }

        public MailAssignCustomer GetById(long Id)
        {
            return _unitOfWork.MailAssignCustomerRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.Include(t => t.IntProviderAppSecret).Include(t => t.IntProviderAppSecret.IntProviderApp).Include(t => t.Customer).FirstOrDefault();
        }

        public async Task<MailAssignCustomer> Delete(long Id)
        {
            var mailAssignCustomerObj = _unitOfWork.MailAssignCustomerRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailAssignCustomerObj != null)
            {
                mailAssignCustomerObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MailAssignCustomerRepository.UpdateAsync(mailAssignCustomerObj, mailAssignCustomerObj.Id).Result;
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }

        public MailAssignCustomer DeleteByThread(string ThreadId)
        {
            var mailAssignCustomerObj = _unitOfWork.MailAssignCustomerRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailAssignCustomerObj != null)
            {
                mailAssignCustomerObj.DeletedOn = DateTime.UtcNow;
                var deletedItem = _unitOfWork.MailAssignCustomerRepository.UpdateAsync(mailAssignCustomerObj, mailAssignCustomerObj.Id).Result;
                _unitOfWork.CommitAsync();
            }
            return mailAssignCustomerObj;
        }

        public async Task<List<MailAssignCustomer>> DeleteBySecretId(long IntProviderAppSecretId)
        {
            var mailAssignCustomerList = _unitOfWork.MailAssignCustomerRepository.GetMany(t => t.IntProviderAppSecretId == IntProviderAppSecretId && t.DeletedOn == null).Result.ToList();
            if (mailAssignCustomerList != null && mailAssignCustomerList.Count() > 0)
            {
                foreach (var existingItem in mailAssignCustomerList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = await _unitOfWork.MailAssignCustomerRepository.UpdateAsync(existingItem, existingItem.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return mailAssignCustomerList;
        }
    }

    public partial interface IMailAssignCustomerService : IService<MailAssignCustomer>
    {
        Task<MailAssignCustomer> CheckInsertOrUpdate(MailAssignCustomerDto model);
        MailAssignCustomer GetMailAssignCustomerByThread(string ThreadId);
        Task<MailAssignCustomer> Delete(long Id);
        MailAssignCustomer DeleteByThread(string ThreadId);
        Task<List<MailAssignCustomer>> DeleteBySecretId(long IntProviderAppSecretId);
        List<MailAssignCustomer> GetAllByCustomer(long CustomerId);
        MailAssignCustomer GetById(long Id);
    }
}