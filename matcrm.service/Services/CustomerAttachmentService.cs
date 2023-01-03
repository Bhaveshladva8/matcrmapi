using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomerAttachmentService : Service<CustomerAttachment>, ICustomerAttachmentService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerAttachmentService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerAttachment> CheckInsertOrUpdate (CustomerAttachmentDto model) {
            var customerAttachmentObj = _mapper.Map<CustomerAttachment> (model);
            var existingItem = _unitOfWork.CustomerAttachmentRepository.GetMany (t => t.FileName == customerAttachmentObj.FileName && t.CustomerId == customerAttachmentObj.CustomerId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertCustomerAttachment (customerAttachmentObj);
            } else {
                return existingItem;
                // return UpdateCustomerAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<CustomerAttachment> InsertCustomerAttachment (CustomerAttachment customerAttachmentObj) {
            customerAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomerAttachmentRepository.AddAsync (customerAttachmentObj);
           await _unitOfWork.CommitAsync ();

            return newItem;
        }
        public async Task<CustomerAttachment> UpdateCustomerAttachment (CustomerAttachment existingItem, long existingId) {
           await _unitOfWork.CustomerAttachmentRepository.UpdateAsync (existingItem, existingId);
           await _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<CustomerAttachment> GetAllByCustomerId (long CustomerId) {
            return _unitOfWork.CustomerAttachmentRepository.GetMany (t => t.CustomerId == CustomerId && t.IsDeleted == false).Result.ToList ();
        }

        public CustomerAttachment GetCustomerAttachmentById (long Id) {
            return _unitOfWork.CustomerAttachmentRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public CustomerAttachment DeleteCustomerAttachmentById (long Id) {
            var customerAttachmentObj = _unitOfWork.CustomerAttachmentRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if(customerAttachmentObj != null)
            {
                customerAttachmentObj.IsDeleted = true;
                customerAttachmentObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.CustomerAttachmentRepository.UpdateAsync (customerAttachmentObj, customerAttachmentObj.Id);
                _unitOfWork.CommitAsync ();
            }
            return customerAttachmentObj;
        }

     
        public List<CustomerAttachment> DeleteAttachmentByCustomerId (long CustomerId) {
            var customerAttachmentList = _unitOfWork.CustomerAttachmentRepository.GetMany (t => t.CustomerId == CustomerId && t.IsDeleted == false).Result.ToList ();
            if(customerAttachmentList != null && customerAttachmentList.Count() > 0 )
            {
                foreach (var item in customerAttachmentList) {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.CustomerAttachmentRepository.UpdateAsync (item, item.Id);
                }

                _unitOfWork.CommitAsync ();
            }

            return customerAttachmentList;
        }
    }
    public partial interface ICustomerAttachmentService : IService<CustomerAttachment> {
        Task<CustomerAttachment> CheckInsertOrUpdate (CustomerAttachmentDto model);
        List<CustomerAttachment> GetAllByCustomerId (long CustomerId);
        CustomerAttachment GetCustomerAttachmentById (long Id);
        CustomerAttachment DeleteCustomerAttachmentById (long Id);
        List<CustomerAttachment> DeleteAttachmentByCustomerId (long CustomerId);
        Task<CustomerAttachment> UpdateCustomerAttachment (CustomerAttachment existingItem, long existingId);
    }
}