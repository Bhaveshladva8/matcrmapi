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
    public partial class EmployeeProjectService : Service<EmployeeProject>, IEmployeeProjectService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeProjectService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeProject> CheckInsertOrUpdate(EmployeeProjectDto model)
        {
            var employeeProjectObj = _mapper.Map<EmployeeProject>(model);
            // var existingItem = _unitOfWork.EmployeeProjectRepository.GetMany (t => t.Id == model.Id && t.TenantId == model.TenantId.Value && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.EmployeeProjectRepository.GetMany(t => t.Id == model.Id && t.TenantId == model.TenantId.Value && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeProject(employeeProjectObj);
            }
            else
            {
                employeeProjectObj.Id = existingItem.Id;
                employeeProjectObj.CreatedBy = existingItem.CreatedBy;
                employeeProjectObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateEmployeeProject(employeeProjectObj, existingItem.Id);
            }
        }

        public async Task<EmployeeProject> InsertEmployeeProject(EmployeeProject employeeProjectObj)
        {
            employeeProjectObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeProjectRepository.AddAsync(employeeProjectObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeProject> UpdateEmployeeProject(EmployeeProject existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.EmployeeProjectRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeProject> GetAll(string searchString, EmployeeProjectDto model)
        {
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();

                return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.IsDeleted == false && ((t.Name.ToLower().Contains(searchString)) || (t.Description.ToLower().Contains(searchString)) || (t.Status.Name.ToLower().Contains(searchString)))).Result.Include(t => t.Status)
                        .Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).OrderBy(t => t.CreatedOn).ToList();
            }
            else
            {
                return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.IsDeleted == false).Result.Include(t => t.Status).Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).OrderBy(t => t.CreatedOn).ToList();
            }
        }

        public List<EmployeeProject> GetAll(EmployeeProjectListRequest model, long tenantId)
        {
            if (!String.IsNullOrEmpty(model.SearchString))
            {
                var searchString = model.SearchString.ToLower();

                return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.IsDeleted == false && t.TenantId == tenantId && ((t.Name.ToLower().Contains(searchString)) || (t.Description.ToLower().Contains(searchString)))).Result.Include(t => t.Status).Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).OrderBy(t => t.CreatedOn).ToList();
            }
            else
            {
                return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.IsDeleted == false && t.TenantId == tenantId).Result.Include(t => t.Status).Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            }
        }

        public List<EmployeeProject> GetAllByTenant(long tenantId)
        {
            return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.TenantId == tenantId && t.IsDeleted == false).Result.Include(t => t.Status).ToList();
        }

        public EmployeeProject GetEmployeeProjectById(long Id)
        {
            return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.Include(t => t.Status).Include(t => t.MateCategory).Include(t => t.Client).FirstOrDefault();
        }

        public async Task<EmployeeProject> DeleteEmployeeProject(long Id)
        {
            var employeeProjectObj = _unitOfWork.EmployeeProjectRepository.GetMany(u => u.Id == Id && u.IsDeleted == false).Result.FirstOrDefault();
            if (employeeProjectObj != null)
            {
                employeeProjectObj.IsDeleted = true;
                employeeProjectObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeProjectRepository.UpdateAsync(employeeProjectObj, employeeProjectObj.Id);
                await _unitOfWork.CommitAsync();
            }

            return employeeProjectObj;
        }

        public List<EmployeeProject> GetAllByClient(long clientId, long tenantId)
        {
            return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.ClientId == clientId && t.TenantId == tenantId & t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeProject> GetAllByClientId(long clientId)
        {
            return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.ClientId == clientId && t.IsDeleted == false).Result.ToList();
        }
        public List<EmployeeProject> GetAll()
        {
            return _unitOfWork.EmployeeProjectRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }
    }

    public partial interface IEmployeeProjectService : IService<EmployeeProject>
    {
        Task<EmployeeProject> CheckInsertOrUpdate(EmployeeProjectDto model);
        List<EmployeeProject> GetAll(string searchString, EmployeeProjectDto model);
        List<EmployeeProject> GetAll(EmployeeProjectListRequest model, long tenantId);
        List<EmployeeProject> GetAllByTenant(long tenantId);
        EmployeeProject GetEmployeeProjectById(long Id);
        Task<EmployeeProject> DeleteEmployeeProject(long Id);
        Task<EmployeeProject> UpdateEmployeeProject(EmployeeProject existingItem, long existingId);
        List<EmployeeProject> GetAllByClient(long clientId, long tenantId);
        List<EmployeeProject> GetAllByClientId(long clientId);
        List<EmployeeProject> GetAll();

    }
}