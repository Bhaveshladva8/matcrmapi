using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Request;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class EmployeeTaskService : Service<EmployeeTask>, IEmployeeTaskService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeTaskService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeTask> CheckInsertOrUpdate(EmployeeTaskDto model)
        {
            var employeeTaskObj = _mapper.Map<EmployeeTask>(model);
            var existingItem = _unitOfWork.EmployeeTaskRepository.GetMany(t => t.Id == employeeTaskObj.Id && t.TenantId == employeeTaskObj.TenantId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertTask(employeeTaskObj);
            }
            else
            {
                employeeTaskObj.CreatedBy = existingItem.CreatedBy;
                employeeTaskObj.CreatedOn = existingItem.CreatedOn;
                employeeTaskObj.Id = existingItem.Id;
                return await UpdateTask(employeeTaskObj, existingItem.Id);
            }
        }

        public async Task<EmployeeTask> InsertTask(EmployeeTask employeeTaskObj)
        {
            employeeTaskObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeTaskRepository.AddAsync(employeeTaskObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<EmployeeTask> UpdateTask(EmployeeTask updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.EmployeeTaskRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();
            return update;
        }

        public List<EmployeeTask> GetAllActiveByTenant(long TenantId)
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.TenantId == TenantId && t.IsActive == true && t.IsDeleted == false).Result.Include(s => s.Status).ToList();
        }

        public List<EmployeeTask> GetByTenantWithoutProject(long TenantId)
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.TenantId == TenantId && t.IsDeleted == false).Result.Include(s => s.Status).ToList();
        }

        public List<EmployeeTask> GetAllTaskByTenant(long TenantId, EmployeeTaskListRequest model)
        {
            if (!String.IsNullOrEmpty(model.SearchString))
            {
                var searchString = model.SearchString.ToLower();

                return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.TenantId == TenantId && t.IsDeleted == false && ((t.Description.ToLower().Contains(searchString)))).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            }
            else
            {
                return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.TenantId == TenantId && t.IsDeleted == false).Result.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            }
        }
        
        public List<EmployeeTask> GetAllTaskWithOutSection(long TenantId)
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.TenantId == TenantId && t.IsActive == true && t.IsDeleted == false && t.SectionId == null).Result.ToList();
        }

        public List<EmployeeTask> GetAll()
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }       

        public List<EmployeeTask> GetAllTaskBySection(long SectionId)
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.SectionId == SectionId && t.IsActive == true && t.IsDeleted == false).Result.ToList();
        }        
        public EmployeeTask GetTaskById(long Id)
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.Id == Id && t.IsActive == true && t.IsDeleted == false).Result.Include(t => t.Status).Include(t => t.MatePriority).FirstOrDefault();
        }        

        public EmployeeTask GetTaskByUser(int UserId)
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.CreatedBy == UserId && t.IsActive == true && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<EmployeeTask> Delete(long TaskId)
        {
            var employeeTaskObj = _unitOfWork.EmployeeTaskRepository.GetMany(t => t.Id == TaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeTaskObj != null)
            {
                employeeTaskObj.IsDeleted = true;
                employeeTaskObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeTaskRepository.UpdateAsync(employeeTaskObj, employeeTaskObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskObj;

        }
        public List<EmployeeTask> GetAllByStatusId(long TenantId, long StatusId)
        {
            return _unitOfWork.EmployeeTaskRepository.GetMany(t => t.TenantId == TenantId && t.StatusId == StatusId && t.DeletedOn == null).Result.Include(t => t.Status).ToList();
        }       
    }
    public partial interface IEmployeeTaskService : IService<EmployeeTask>
    {
        Task<EmployeeTask> CheckInsertOrUpdate(EmployeeTaskDto model);
        List<EmployeeTask> GetAll();        
        List<EmployeeTask> GetAllTaskBySection(long SectionId);        
        List<EmployeeTask> GetAllActiveByTenant(long TenantId);
        List<EmployeeTask> GetAllTaskByTenant(long TenantId, EmployeeTaskListRequest model);
        EmployeeTask GetTaskById(long TaskId);
        Task<EmployeeTask> Delete(long TaskId);
        Task<EmployeeTask> UpdateTask(EmployeeTask updatedItem, long existingId);
        EmployeeTask GetTaskByUser(int UserId);
        List<EmployeeTask> GetByTenantWithoutProject(long TenantId);
        List<EmployeeTask> GetAllByStatusId(long TenantId,long StatusId);
    }
}