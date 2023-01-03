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
    public partial class EmployeeSubTaskService : Service<EmployeeSubTask>, IEmployeeSubTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeSubTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeSubTask> CheckInsertOrUpdate(EmployeeSubTaskDto model)
        {
            var employeeSubTaskObj = _mapper.Map<EmployeeSubTask>(model);
            EmployeeSubTask? existingItem = null;
            existingItem = _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.Id == employeeSubTaskObj.Id && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSubTask(employeeSubTaskObj);
            }
            else
            {
                employeeSubTaskObj.CreatedBy = existingItem.CreatedBy;
                employeeSubTaskObj.CreatedOn = existingItem.CreatedOn;
                employeeSubTaskObj.Id = existingItem.Id;
                return await UpdateSubTask(employeeSubTaskObj, existingItem.Id);
            }
        }

        public async Task<EmployeeSubTask> InsertSubTask(EmployeeSubTask employeeSubTaskObj)
        {
            employeeSubTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeSubTaskRepository.AddAsync(employeeSubTaskObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<EmployeeSubTask> UpdateSubTask(EmployeeSubTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.EmployeeSubTaskRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public List<EmployeeSubTask> GetAllActive()
        {
            return _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeSubTask> GetAllActiveByTaskIds(List<long> TaskIds)
        {
            return _unitOfWork.EmployeeSubTaskRepository.GetMany(t => TaskIds.Contains(t.EmployeeTaskId.Value) && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeSubTask> GetAll()
        {
            return _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeSubTask> GetAllSubTaskByTask(long TaskId)
        {
            return _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.EmployeeTaskId == TaskId && t.IsActive == true && t.IsDeleted == false).Result.Include(t => t.Status).ToList();
        }

        public EmployeeSubTask GetSubTaskById(long SubTaskId)
        {
            return _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.Id == SubTaskId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<EmployeeSubTask> Delete(long SubTaskId)
        {
            var employeeSubTaskObj = _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.Id == SubTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeSubTaskObj != null)
            {
                employeeSubTaskObj.IsDeleted = true;
                employeeSubTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeSubTaskRepository.UpdateAsync(employeeSubTaskObj, employeeSubTaskObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskObj;
        }

        public List<EmployeeSubTask> GetByTaskId(EmployeeSubTaskListRequest model)
        {
            if (!String.IsNullOrEmpty(model.SearchString))
            {
                var searchString = model.SearchString.ToLower();

                return _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.EmployeeTaskId == model.TaskId && t.IsActive == true && t.IsDeleted == false && ((t.Description.ToLower().Contains(searchString)))).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            }
            else
            {
                return _unitOfWork.EmployeeSubTaskRepository.GetMany(t => t.EmployeeTaskId == model.TaskId && t.IsActive == true && t.IsDeleted == false).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            }
        }
    }
    public partial interface IEmployeeSubTaskService : IService<EmployeeSubTask>
    {
        Task<EmployeeSubTask> CheckInsertOrUpdate(EmployeeSubTaskDto model);
        List<EmployeeSubTask> GetAll();
        List<EmployeeSubTask> GetAllActive();
        List<EmployeeSubTask> GetAllActiveByTaskIds(List<long> TaskIds);
        List<EmployeeSubTask> GetAllSubTaskByTask(long TaskId);
        Task<EmployeeSubTask> Delete(long SubTaskId);
        EmployeeSubTask GetSubTaskById(long SubTaskId);
        List<EmployeeSubTask> GetByTaskId(EmployeeSubTaskListRequest model);        
    }
    
}