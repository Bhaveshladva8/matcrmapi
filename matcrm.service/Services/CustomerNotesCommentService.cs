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
    public partial class CustomerNotesCommentService : Service<CustomerNotesComment>, ICustomerNotesCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerNotesCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerNotesComment> CheckInsertOrUpdate(CustomerNotesCommentDto model)
        {
            var customerNotesCommentObj = _mapper.Map<CustomerNotesComment>(model);
            var existingItem = _unitOfWork.CustomerNotesCommentRepository.GetMany(t => t.Id == customerNotesCommentObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertCustomerNotesComment(customerNotesCommentObj);
            }
            else
            {
                existingItem.Comment = model.Comment;
                return await UpdateCustomerNotesComment(existingItem, existingItem.Id);
            }
        }

        public async Task<CustomerNotesComment> InsertCustomerNotesComment(CustomerNotesComment customerNotesCommentObj)
        {
            customerNotesCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomerNotesCommentRepository.AddAsync(customerNotesCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<CustomerNotesComment> UpdateCustomerNotesComment(CustomerNotesComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomerNotesCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<CustomerNotesComment> GetAllByNoteId(long CustomerNoteId)
        {
            return _unitOfWork.CustomerNotesCommentRepository.GetMany(t => t.CustomerNoteId == CustomerNoteId && t.IsDeleted == false).Result.ToList();
        }

        public CustomerNotesComment GetCustomerNotesCommenttById(long Id)
        {
            return _unitOfWork.CustomerNotesCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<CustomerNotesComment> DeleteCustomerNotesComment(long Id)
        {
            var customerNotesCommentObj = _unitOfWork.CustomerNotesCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.Include(t => t.CustomerNote).FirstOrDefault();
            if (customerNotesCommentObj != null)
            {
                customerNotesCommentObj.IsDeleted = true;
                customerNotesCommentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.CustomerNotesCommentRepository.UpdateAsync(customerNotesCommentObj, customerNotesCommentObj.Id);
                await _unitOfWork.CommitAsync();

                return customerNotesCommentObj;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<CustomerNotesComment>> DeleteCommentByNoteId(long CustomerNoteId)
        {
            var customerNotesCommentList = _unitOfWork.CustomerNotesCommentRepository.GetMany(t => t.CustomerNoteId == CustomerNoteId && t.IsDeleted == false).Result.ToList();
            if (customerNotesCommentList != null && customerNotesCommentList.Count() > 0)
            {
                foreach (var item in customerNotesCommentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.CustomerNotesCommentRepository.UpdateAsync(item, item.Id);
                }

                await _unitOfWork.CommitAsync();
            }

            return customerNotesCommentList;
        }
    }

    public partial interface ICustomerNotesCommentService : IService<CustomerNotesComment>
    {
        Task<CustomerNotesComment> CheckInsertOrUpdate(CustomerNotesCommentDto model);
        List<CustomerNotesComment> GetAllByNoteId(long CustomerNoteId);
        CustomerNotesComment GetCustomerNotesCommenttById(long Id);
        Task<CustomerNotesComment> DeleteCustomerNotesComment(long Id);
        Task<List<CustomerNotesComment>> DeleteCommentByNoteId(long CustomerNoteId);
    }
}