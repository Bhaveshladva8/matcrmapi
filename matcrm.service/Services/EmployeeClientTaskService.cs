using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data;
using matcrm.data.Models.Tables;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class EmployeeClientTaskService : Service<EmployeeClientTask>, IEmployeeClientTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeClientTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<EmployeeClientTask> CheckInsertOrUpdate(EmployeeClientTask employeeClientTaskObj)
        {
            //EmployeeClientTask? existingItem = null;
            var existingItem = _unitOfWork.EmployeeClientTaskRepository.GetMany(t => t.EmployeeTaskId == employeeClientTaskObj.EmployeeTaskId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeClientTask(employeeClientTaskObj);
            }
            else
            {
                employeeClientTaskObj.Id = existingItem.Id;
                return await UpdateEmployeeClientTask(employeeClientTaskObj, existingItem.Id);
            }
        }
        public async Task<EmployeeClientTask> InsertEmployeeClientTask(EmployeeClientTask employeeClientTaskObj)
        {
            var newItem = await _unitOfWork.EmployeeClientTaskRepository.AddAsync(employeeClientTaskObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeClientTask> UpdateEmployeeClientTask(EmployeeClientTask existingItem, long existingId)
        {
            await _unitOfWork.EmployeeClientTaskRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public List<EmployeeClientTask> GetTaskByClient(long ClientId)
        {
            return _unitOfWork.EmployeeClientTaskRepository.GetMany(t => t.ClientId == ClientId).Result.Include(t => t.EmployeeTask).ToList();
        }
        public EmployeeClientTask GetByTaskId(long TaskId)
        {
            return _unitOfWork.EmployeeClientTaskRepository.GetMany(t => t.EmployeeTaskId == TaskId && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public List<EmployeeClientTask> GetTaskByClientWithTenant(long ClientId, long TenantId)
        {
            return _unitOfWork.EmployeeClientTaskRepository.GetMany(t => t.ClientId == ClientId).Result.Include(t => t.EmployeeTask).ToList();
        }
        public async Task<EmployeeClientTask> DeleteByTaskId(long TaskId)
        {
            var employeeClientTaskObj = _unitOfWork.EmployeeClientTaskRepository.GetMany(u => u.EmployeeTaskId == TaskId && u.DeletedOn == null).Result.FirstOrDefault();
            if (employeeClientTaskObj != null)
            {
                employeeClientTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeClientTaskRepository.UpdateAsync(employeeClientTaskObj, employeeClientTaskObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeClientTaskObj;
        }
    }
    public partial interface IEmployeeClientTaskService : IService<EmployeeClientTask>
    {
        Task<EmployeeClientTask> CheckInsertOrUpdate(EmployeeClientTask model);
        List<EmployeeClientTask> GetTaskByClient(long ClientId);
        EmployeeClientTask GetByTaskId(long TaskId);
        List<EmployeeClientTask> GetTaskByClientWithTenant(long ClientId, long TenantId);
        Task<EmployeeClientTask> DeleteByTaskId(long TaskId);
    }
}