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
    public partial class EmployeeTaskCommentService : Service<EmployeeTaskComment>, IEmployeeTaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeTaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeTaskComment> CheckInsertOrUpdate(EmployeeTaskCommentDto model)
        {
            var employeeTaskCommentObj = _mapper.Map<EmployeeTaskComment>(model);
            // var existingItem = _unitOfWork.EmployeeTaskCommentRepository.GetMany (t => t.UserId == obj.UserId && t.TaskId == obj.TaskId && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.EmployeeTaskCommentRepository.GetMany(t => t.Id == employeeTaskCommentObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeTaskComment(employeeTaskCommentObj);
            }
            else
            {
                existingItem.Comment = model.Comment;
                return await UpdateEmployeeTaskComment(existingItem, existingItem.Id);
            }
        }

        public async Task<EmployeeTaskComment> InsertEmployeeTaskComment(EmployeeTaskComment employeeTaskCommentObj)
        {
            employeeTaskCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeTaskCommentRepository.AddAsync(employeeTaskCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeTaskComment> UpdateEmployeeTaskComment(EmployeeTaskComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.EmployeeTaskCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeTaskComment> GetAllByEmployeeTaskId(long EmployeeTaskId)
        {
            return _unitOfWork.EmployeeTaskCommentRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.ToList();
        }

        public EmployeeTaskComment GetEmployeeTaskCommentById(long Id)
        {
            return _unitOfWork.EmployeeTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<EmployeeTaskComment> DeleteEmployeeTaskComment(long Id)
        {
            var employeeTaskCommentObj = _unitOfWork.EmployeeTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeTaskCommentObj != null)
            {
                employeeTaskCommentObj.IsDeleted = true;
                employeeTaskCommentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeTaskCommentRepository.UpdateAsync(employeeTaskCommentObj, employeeTaskCommentObj.Id);
                await _unitOfWork.CommitAsync();

                return employeeTaskCommentObj;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<EmployeeTaskComment>> DeleteCommentByEmployeeTaskId(long EmployeeTaskId)
        {
            var employeeTaskCommentsList = _unitOfWork.EmployeeTaskCommentRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeTaskCommentsList != null && employeeTaskCommentsList.Count() > 0)
            {
                foreach (var item in employeeTaskCommentsList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeTaskCommentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskCommentsList;
        }
    }

    public partial interface IEmployeeTaskCommentService : IService<EmployeeTaskComment>
    {
        Task<EmployeeTaskComment> CheckInsertOrUpdate(EmployeeTaskCommentDto model);
        List<EmployeeTaskComment> GetAllByEmployeeTaskId(long EmployeeTaskId);
        EmployeeTaskComment GetEmployeeTaskCommentById(long Id);
        Task<EmployeeTaskComment> DeleteEmployeeTaskComment(long Id);
        Task<List<EmployeeTaskComment>> DeleteCommentByEmployeeTaskId(long EmployeeTaskId);
    }
}