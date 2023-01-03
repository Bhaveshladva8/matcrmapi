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
    public partial class EmployeeSubTaskCommentService : Service<EmployeeSubTaskComment>, IEmployeeSubTaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeSubTaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeSubTaskComment> CheckInsertOrUpdate(EmployeeSubTaskCommentDto model)
        {
            var employeeSubTaskCommentObj = _mapper.Map<EmployeeSubTaskComment>(model);
            var existingItem = _unitOfWork.EmployeeSubTaskCommentRepository.GetMany(t => t.Id == employeeSubTaskCommentObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeSubTaskComment(employeeSubTaskCommentObj);
            }
            else
            {
                existingItem.Comment = model.Comment;
                return await UpdateEmployeeSubTaskComment(existingItem, existingItem.Id);
            }
        }

        public async Task<EmployeeSubTaskComment> InsertEmployeeSubTaskComment(EmployeeSubTaskComment employeeSubTaskCommentObj)
        {
            employeeSubTaskCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeSubTaskCommentRepository.AddAsync(employeeSubTaskCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeSubTaskComment> UpdateEmployeeSubTaskComment(EmployeeSubTaskComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.EmployeeSubTaskCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeSubTaskComment> GetAllByEmployeeSubTaskId(long EmployeeSubTaskId)
        {
            return _unitOfWork.EmployeeSubTaskCommentRepository.GetMany(t => t.EmployeeSubTaskId == EmployeeSubTaskId && t.IsDeleted == false).Result.ToList();
        }

        public EmployeeSubTaskComment GetEmployeeSubTaskCommentById(long Id)
        {
            return _unitOfWork.EmployeeSubTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<EmployeeSubTaskComment> DeleteEmployeeSubTaskComment(long Id)
        {
            var employeeSubTaskCommentObj = _unitOfWork.EmployeeSubTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeSubTaskCommentObj != null)
            {
                employeeSubTaskCommentObj.IsDeleted = true;
                employeeSubTaskCommentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeSubTaskCommentRepository.UpdateAsync(employeeSubTaskCommentObj, employeeSubTaskCommentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskCommentObj;
        }

        public async Task<List<EmployeeSubTaskComment>> DeleteCommentByEmployeeSubTaskId(long EmployeeSubTaskId)
        {
            var employeeSubTaskCommentsList = _unitOfWork.EmployeeSubTaskCommentRepository.GetMany(t => t.EmployeeSubTaskId == EmployeeSubTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeSubTaskCommentsList != null && employeeSubTaskCommentsList.Count() > 0)
            {
                foreach (var item in employeeSubTaskCommentsList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeSubTaskCommentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskCommentsList;
        }
    }

    public partial interface IEmployeeSubTaskCommentService : IService<EmployeeSubTaskComment>
    {
        Task<EmployeeSubTaskComment> CheckInsertOrUpdate(EmployeeSubTaskCommentDto model);
        List<EmployeeSubTaskComment> GetAllByEmployeeSubTaskId(long EmployeeSubTaskId);
        EmployeeSubTaskComment GetEmployeeSubTaskCommentById(long Id);
        Task<EmployeeSubTaskComment> DeleteEmployeeSubTaskComment(long Id);
        Task<List<EmployeeSubTaskComment>> DeleteCommentByEmployeeSubTaskId(long EmployeeSubTaskId);
    }
}