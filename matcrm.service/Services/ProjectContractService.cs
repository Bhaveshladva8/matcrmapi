using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class ProjectContractService : Service<ProjectContract>, IProjectContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public ProjectContractService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ProjectContract> CheckInsertOrUpdate(ProjectContract projectContractObj)
        {
            ProjectContract? existingItem = null;

            if (projectContractObj.ProjectId != null && projectContractObj.ProjectId > 0)
            {
                existingItem = _unitOfWork.ProjectContractRepository.GetMany(t => t.ProjectId == projectContractObj.ProjectId && t.DeletedOn == null).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.ProjectContractRepository.GetMany(t => t.Id == projectContractObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return await InsertProjectContract(projectContractObj);
            }
            else
            {
                projectContractObj.Id = existingItem.Id;
                projectContractObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateProjectContract(projectContractObj, existingItem.Id);
            }
        }

        public async Task<ProjectContract> InsertProjectContract(ProjectContract projectContractObj)
        {
            projectContractObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ProjectContractRepository.AddAsync(projectContractObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<ProjectContract> UpdateProjectContract(ProjectContract existingItem, long existingId)
        {
            await _unitOfWork.ProjectContractRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();
            return existingItem;
        }

        public async Task<ProjectContract> Delete(long Id)
        {
            var projectContractObj = _unitOfWork.ProjectContractRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (projectContractObj != null)
            {
                projectContractObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ProjectContractRepository.UpdateAsync(projectContractObj, projectContractObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return projectContractObj;
        }

        public async Task<List<ProjectContract>> DeleteByContract(long ContractId)
        {
            var projectContractList = _unitOfWork.ProjectContractRepository.GetMany(u => u.ContractId == ContractId && u.DeletedOn == null).Result.ToList();
            if (projectContractList != null && projectContractList.Count > 0)
            {
                foreach (var projectContract in projectContractList)
                {
                    projectContract.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.ProjectContractRepository.UpdateAsync(projectContract, projectContract.Id);
                    await _unitOfWork.CommitAsync();
                }
            }
            return projectContractList;
        }
        public ProjectContract GetByProjectId(long ProjectId)
        {
            return _unitOfWork.ProjectContractRepository.GetMany(t => t.DeletedOn == null && t.ProjectId == ProjectId).Result.FirstOrDefault();
        }
        public async Task<ProjectContract> DeleteByProjectId(long ProjectId)
        {
            var projectContractObj = _unitOfWork.ProjectContractRepository.GetMany(u => u.ProjectId == ProjectId && u.DeletedOn == null).Result.FirstOrDefault();
            if (projectContractObj != null)
            {
                projectContractObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.ProjectContractRepository.UpdateAsync(projectContractObj, projectContractObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return projectContractObj;
        }
        public List<ProjectContract> GetByContractId(long ContractId)
        {
            return _unitOfWork.ProjectContractRepository.GetMany(u => u.ContractId == ContractId && u.DeletedOn == null).Result.ToList();
        }
        public List<ProjectContract> GetAll()
        {
            return _unitOfWork.ProjectContractRepository.GetMany(u => u.DeletedOn == null).Result.ToList();
        }
    }
    public partial interface IProjectContractService : IService<ProjectContract>
    {
        Task<ProjectContract> CheckInsertOrUpdate(ProjectContract projectContractObj);
        Task<ProjectContract> Delete(long Id);
        Task<List<ProjectContract>> DeleteByContract(long ContractId);
        ProjectContract GetByProjectId(long ProjectId);
        Task<ProjectContract> DeleteByProjectId(long ProjectId);
        List<ProjectContract> GetByContractId(long ContractId);
        List<ProjectContract> GetAll();
    }
}