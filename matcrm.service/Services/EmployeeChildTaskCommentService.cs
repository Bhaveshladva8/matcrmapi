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
    public partial class EmployeeChildTaskCommentService : Service<EmployeeChildTaskComment>, IEmployeeChildTaskCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeChildTaskCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeChildTaskComment> CheckInsertOrUpdate(EmployeeChildTaskCommentDto model)
        {
            var employeeChildTaskCommentObj = _mapper.Map<EmployeeChildTaskComment>(model);
            var existingItem = _unitOfWork.EmployeeChildTaskCommentRepository.GetMany(t => t.Id == model.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeChildTaskComment(employeeChildTaskCommentObj);
            }
            else
            {
                // existingItem.Name = model.Name;
                return await UpdateEmployeeChildTaskComment(existingItem, existingItem.Id);
            }
        }

        public async Task<EmployeeChildTaskComment> InsertEmployeeChildTaskComment(EmployeeChildTaskComment employeeChildTaskCommentObj)
        {
            employeeChildTaskCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeChildTaskCommentRepository.AddAsync(employeeChildTaskCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeChildTaskComment> UpdateEmployeeChildTaskComment(EmployeeChildTaskComment existingItem, long existingId)
        {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.EmployeeChildTaskCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeChildTaskComment> GetAllByChildTaskId(long EmployeeChildTaskId)
        {
            return _unitOfWork.EmployeeChildTaskCommentRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId && t.IsDeleted == false).Result.ToList();
        }

        public EmployeeChildTaskComment GetEmployeeChildTaskCommentById(long Id)
        {
            return _unitOfWork.EmployeeChildTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<EmployeeChildTaskComment> DeleteEmployeeChildTaskComment(long Id)
        {
            var employeeChildTaskCommentObj = _unitOfWork.EmployeeChildTaskCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeChildTaskCommentObj != null)
            {
                employeeChildTaskCommentObj.IsDeleted = true;
                employeeChildTaskCommentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeChildTaskCommentRepository.UpdateAsync(employeeChildTaskCommentObj, employeeChildTaskCommentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskCommentObj;
        }

        public async Task<List<EmployeeChildTaskComment>> DeleteCommentByChildTaskId(long EmployeeChildTaskId)
        {
            var employeeChildTaskCommentList = _unitOfWork.EmployeeChildTaskCommentRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeChildTaskCommentList != null && employeeChildTaskCommentList.Count() > 0)
            {
                foreach (var item in employeeChildTaskCommentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeChildTaskCommentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskCommentList;
        }
    }

    public partial interface IEmployeeChildTaskCommentService : IService<EmployeeChildTaskComment>
    {
        Task<EmployeeChildTaskComment> CheckInsertOrUpdate(EmployeeChildTaskCommentDto model);
        List<EmployeeChildTaskComment> GetAllByChildTaskId(long childTaskId);
        EmployeeChildTaskComment GetEmployeeChildTaskCommentById(long Id);
        Task<EmployeeChildTaskComment> DeleteEmployeeChildTaskComment(long Id);
        Task<List<EmployeeChildTaskComment>> DeleteCommentByChildTaskId(long ChildTaskId);
    }
}