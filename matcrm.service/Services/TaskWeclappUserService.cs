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
    public partial class TaskWeclappUserService : Service<TaskWeclappUser>, ITaskWeclappUserService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITenantService _tenantService;

        public TaskWeclappUserService(IUnitOfWork unitOfWork, ITenantService tenantService,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tenantService = tenantService;
        }
        public async Task<TaskWeclappUser> CheckInsertOrUpdate(TaskWeclappUserDto model)
        {
            var taskWeclappUserObj = _mapper.Map<TaskWeclappUser>(model);
            TaskWeclappUser? existingItem;
            if (model.Id != null)
            {
                existingItem = _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.Id == taskWeclappUserObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.TenantId == taskWeclappUserObj.TenantId && t.UserId == taskWeclappUserObj.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                taskWeclappUserObj.Id = 0;
                return await InsertTaskWeclappUser(taskWeclappUserObj);
            }
            else
            {
                existingItem.TenantName = taskWeclappUserObj.TenantName;
                existingItem.ApiKey = taskWeclappUserObj.ApiKey;
                return await UpdateTaskWeclappUser(existingItem, existingItem.Id);
            }
        }

        public async Task<TaskWeclappUser> InsertTaskWeclappUser(TaskWeclappUser taskWeclappUserObj)
        {
            taskWeclappUserObj.CreatedOn = DateTime.UtcNow;
            taskWeclappUserObj.UserId = taskWeclappUserObj.UserId;
            var newItem = await _unitOfWork.TaskWeclappUserRepository.AddAsync(taskWeclappUserObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<TaskWeclappUser> UpdateTaskWeclappUser(TaskWeclappUser existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.TaskWeclappUserRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public TaskWeclappUser GetByUser(int UserId)
        {
            return _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.UserId == UserId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public TaskWeclappUser GetByTenantId(int TenantId)
        {
            return _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.TenantId == TenantId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public TaskWeclappUser GetById(long Id)
        {
            return _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public TaskWeclappUser GetByTenantName(string TenantName)
        {
            return _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.TenantName == TenantName && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public TaskWeclappUser DeleteById(long Id)
        {
            var taskWeclappUserObj = _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (taskWeclappUserObj != null)
            {
                taskWeclappUserObj.IsDeleted = true;
                taskWeclappUserObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.TaskWeclappUserRepository.UpdateAsync(taskWeclappUserObj, Id);
                _unitOfWork.CommitAsync();
            }
            return taskWeclappUserObj;
        }

        public TaskWeclappUser DeleteByUser(int UserId)
        {
            var taskWeclappUserObj = _unitOfWork.TaskWeclappUserRepository.GetMany(t => t.UserId == UserId).Result.FirstOrDefault();
            if (taskWeclappUserObj != null)
            {
                taskWeclappUserObj.IsDeleted = true;
                taskWeclappUserObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.TaskWeclappUserRepository.UpdateAsync(taskWeclappUserObj, taskWeclappUserObj.Id);
                _unitOfWork.CommitAsync();
            }
            return taskWeclappUserObj;
        }
    }

    public partial interface ITaskWeclappUserService : IService<TaskWeclappUser>
    {
        Task<TaskWeclappUser> CheckInsertOrUpdate(TaskWeclappUserDto model);
        TaskWeclappUser GetByUser(int UserId);
        TaskWeclappUser GetByTenantId(int TenantId);
        TaskWeclappUser GetById(long Id);
        TaskWeclappUser GetByTenantName(string TenantName);
        TaskWeclappUser DeleteById(long Id);
        TaskWeclappUser DeleteByUser(int UserId);
    }
}