using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class EmployeeProjectTaskService : Service<EmployeeProjectTask>, IEmployeeProjectTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeProjectTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<EmployeeProjectTask> CheckInsertOrUpdate(EmployeeProjectTask employeeProjectTaskObj)
        {
            //EmployeeProjectTask? existingItem = null;
            var existingItem = _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeTaskId == employeeProjectTaskObj.EmployeeTaskId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeProjectTask(employeeProjectTaskObj);
            }
            else
            {
                employeeProjectTaskObj.Id = existingItem.Id;
                return await UpdateEmployeeProjectTask(employeeProjectTaskObj, existingItem.Id);
            }
        }
        public async Task<EmployeeProjectTask> InsertEmployeeProjectTask(EmployeeProjectTask employeeProjectTaskObj)
        {
            var newItem = await _unitOfWork.EmployeeProjectTaskRepository.AddAsync(employeeProjectTaskObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeProjectTask> UpdateEmployeeProjectTask(EmployeeProjectTask existingItem, long existingId)
        {
            await _unitOfWork.EmployeeProjectTaskRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<EmployeeProjectTask> GetAllTaskByProjectId(long ProjectId)
        {
            return _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeProjectId == ProjectId && t.DeletedOn == null).Result.Include(t => t.EmployeeTask).ToList();
        }

        public List<EmployeeProjectTask> GetAllTaskListByProjectId(EmployeeProjectTaskListRequest model)
        {
            if (!String.IsNullOrEmpty(model.SearchString))
            {
                var searchString = model.SearchString.ToLower();
                return _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeProjectId == model.EmployeeProjectId && t.DeletedOn == null && ((t.EmployeeTask.Description.ToLower().Contains(searchString)))).Result.Include(t => t.EmployeeTask).ToList();
            }
            else
            {
                return _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeProjectId == model.EmployeeProjectId && t.DeletedOn == null).Result.Include(t => t.EmployeeTask).ToList();
            }
        }
        public List<EmployeeProjectTask> GetAllByProjectIdList(List<long> ProjectIds)
        {
            return _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.DeletedOn == null && t.EmployeeTask.IsActive == true && ProjectIds.Any(b => t.EmployeeProjectId.Value == b)).Result.Include(t => t.EmployeeTask).ToList();
        }
        public async Task<EmployeeProjectTask> DeleteByTaskId(long TaskId)
        {
            var employeeProjectTaskObj = _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeTaskId == TaskId && t.DeletedOn == null).Result.FirstOrDefault();
            if (employeeProjectTaskObj != null)
            {
                employeeProjectTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeProjectTaskRepository.UpdateAsync(employeeProjectTaskObj, employeeProjectTaskObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeProjectTaskObj;

        }
        public List<EmployeeProjectTask> GetAllTaskByTenant(long ProjectId, long TenantId)
        {
            return _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeProjectId == ProjectId && t.EmployeeTask.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.EmployeeTask).ToList();
        }
        public List<EmployeeProjectTask> GetByTenant(long TenantId)
        {
            return _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeTask.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.EmployeeTask).ToList();
        }
        public EmployeeProjectTask GetByTaskId(long TaskId)
        {
            return _unitOfWork.EmployeeProjectTaskRepository.GetMany(t => t.EmployeeTaskId == TaskId && t.DeletedOn == null).Result.Include(t => t.EmployeeTask).Include(t => t.EmployeeProject).FirstOrDefault();
        }
        
    }
    public partial interface IEmployeeProjectTaskService : IService<EmployeeProjectTask>
    {
        Task<EmployeeProjectTask> CheckInsertOrUpdate(EmployeeProjectTask model);
        List<EmployeeProjectTask> GetAllTaskByProjectId(long ProjectId);
        List<EmployeeProjectTask> GetAllTaskListByProjectId(EmployeeProjectTaskListRequest model);
        List<EmployeeProjectTask> GetAllByProjectIdList(List<long> ProjectIds);
        Task<EmployeeProjectTask> DeleteByTaskId(long TaskId);
        List<EmployeeProjectTask> GetAllTaskByTenant(long ProjectId, long TenantId);
        List<EmployeeProjectTask> GetByTenant(long TenantId);
        EmployeeProjectTask GetByTaskId(long TaskId);
    }
}