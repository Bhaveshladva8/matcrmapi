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
    public partial class ProjectCategoryService : Service<ProjectCategory>, IProjectCategoryService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ProjectCategoryService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProjectCategory> CheckInsertOrUpdate(ProjectCategory projectCategoryObj)
        {
            var existingItem = _unitOfWork.ProjectCategoryRepository.GetMany(t => t.Id == projectCategoryObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertProjectCategory(projectCategoryObj);
            }
            else
            {
                projectCategoryObj.CreatedBy = existingItem.CreatedBy;
                projectCategoryObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateProjectCategory(projectCategoryObj, existingItem.Id);
            }
        }
        public async Task<ProjectCategory> InsertProjectCategory(ProjectCategory projectCategoryObj)
        {
            projectCategoryObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ProjectCategoryRepository.AddAsync(projectCategoryObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ProjectCategory> UpdateProjectCategory(ProjectCategory existingItem, long existingId)
        {
            //existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ProjectCategoryRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<ProjectCategory> DeleteById(long Id)
        {
            var projectCategoryObj = _unitOfWork.ProjectCategoryRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (projectCategoryObj != null)
            {
                projectCategoryObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ProjectCategoryRepository.UpdateAsync(projectCategoryObj, projectCategoryObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return projectCategoryObj;
        }
        public List<ProjectCategory> GetAll()
        {
            return _unitOfWork.ProjectCategoryRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
        public ProjectCategory GetById(long Id)
        {
            return _unitOfWork.ProjectCategoryRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public List<ProjectCategory> GetByTenant(long TenantId)
        {
            return _unitOfWork.ProjectCategoryRepository.GetMany(t => t.CreatedUser.TenantId == TenantId && t.DeletedOn == null).Result.Include(t => t.CreatedUser).ToList();
        }
    }
    public partial interface IProjectCategoryService : IService<ProjectCategory>
    {
        Task<ProjectCategory> CheckInsertOrUpdate(ProjectCategory model);
        Task<ProjectCategory> DeleteById(long Id);
        List<ProjectCategory> GetAll();
        ProjectCategory GetById(long Id);
        List<ProjectCategory> GetByTenant(long TenantId);
    }
}