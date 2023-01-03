using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class EmployeeChildTaskService : Service<EmployeeChildTask>, IEmployeeChildTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeChildTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeChildTask> CheckInsertOrUpdate(EmployeeChildTaskDto model)
        {
            var employeeChildTaskObj = _mapper.Map<EmployeeChildTask>(model);
            var existingItem = _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.Id == employeeChildTaskObj.Id && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertChildTask(employeeChildTaskObj);
            }
            else
            {
                employeeChildTaskObj.CreatedBy = existingItem.CreatedBy;
                employeeChildTaskObj.CreatedOn = existingItem.CreatedOn;
                employeeChildTaskObj.Id = existingItem.Id;
                return await UpdateChildTask(employeeChildTaskObj, existingItem.Id);
            }
        }

        public async Task<EmployeeChildTask> InsertChildTask(EmployeeChildTask employeeChildTaskObj)
        {
            employeeChildTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeChildTaskRepository.AddAsync(employeeChildTaskObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<EmployeeChildTask> UpdateChildTask(EmployeeChildTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.EmployeeChildTaskRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public List<EmployeeChildTask> GetAllActive()
        {
            return _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeChildTask> GetAllActiveBySubTaskIds(List<long> SubTaskIds)
        {
            return _unitOfWork.EmployeeChildTaskRepository.GetMany(t => SubTaskIds.Contains(t.EmployeeSubTaskId.Value) && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeChildTask> GetAll()
        {
            return _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeChildTask> GetAllChildTaskBySubTask(long SubTaskId)
        {
            return _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.EmployeeSubTaskId == SubTaskId && t.IsActive == true && t.IsDeleted == false).Result.Include(t => t.Status).ToList();
        }

        public EmployeeChildTask GetChildTaskById(long ChildTaskId)
        {
            return _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.Id == ChildTaskId && t.IsActive == true && t.IsDeleted == false).Result.Include(t => t.EmployeeSubTask).FirstOrDefault();
        }

        public async Task<EmployeeChildTask> Delete(long ChildTaskId)
        {
            var employeeChildTaskObj = _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.Id == ChildTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeChildTaskObj != null)
            {
                employeeChildTaskObj.IsDeleted = true;
                employeeChildTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeChildTaskRepository.UpdateAsync(employeeChildTaskObj, employeeChildTaskObj.Id);
                await _unitOfWork.CommitAsync();
            }

            return employeeChildTaskObj;
        }

        public List<EmployeeChildTask> GetBySubTaskId(EmployeeChildTaskListRequest model)
        {
            if (!String.IsNullOrEmpty(model.SearchString))
            {
                var searchString = model.SearchString.ToLower();

                return _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.EmployeeSubTaskId == model.SubTaskId && t.IsActive == true && t.IsDeleted == false && ((t.Description.ToLower().Contains(searchString)))).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            }
            else
            {
                return _unitOfWork.EmployeeChildTaskRepository.GetMany(t => t.EmployeeSubTaskId == model.SubTaskId && t.IsActive == true && t.IsDeleted == false).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            }
        }
    }

    public partial interface IEmployeeChildTaskService : IService<EmployeeChildTask>
    {
        Task<EmployeeChildTask> CheckInsertOrUpdate(EmployeeChildTaskDto model);
        List<EmployeeChildTask> GetAll();
        List<EmployeeChildTask> GetAllActive();
        List<EmployeeChildTask> GetAllActiveBySubTaskIds(List<long> SubTaskIds);
        List<EmployeeChildTask> GetAllChildTaskBySubTask(long SubTaskId);
        Task<EmployeeChildTask> Delete(long ChildTaskId);
        EmployeeChildTask GetChildTaskById(long ChildTaskId);
        List<EmployeeChildTask> GetBySubTaskId(EmployeeChildTaskListRequest model);
    }
}