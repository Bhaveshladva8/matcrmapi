using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class EmployeeProjectUserService : Service<EmployeeProjectUser>, IEmployeeProjectUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeProjectUserService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public bool IsExistOrNot(EmployeeProjectUserDto model)
        {
            var existingItem = _unitOfWork.EmployeeProjectUserRepository.GetMany(t => t.EmployeeProjectId == model.EmployeeProjectId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task<EmployeeProjectUser> CheckInsertOrUpdate(EmployeeProjectUserDto model)
        {
            var employeeProjectUserObj = _mapper.Map<EmployeeProjectUser>(model);
            var existingItem = _unitOfWork.EmployeeProjectUserRepository.GetMany(t => t.EmployeeProjectId == employeeProjectUserObj.EmployeeProjectId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertProjectUser(employeeProjectUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public async Task<EmployeeProjectUser> InsertProjectUser(EmployeeProjectUser employeeProjectUserObj)
        {
            employeeProjectUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeProjectUserRepository.AddAsync(employeeProjectUserObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<List<EmployeeProjectUser>> DeleteByEmployeeProjectId(long EmployeeProjectId)
        {
            var employeeProjectUserList = _unitOfWork.EmployeeProjectUserRepository.GetMany(t => t.EmployeeProjectId == EmployeeProjectId && t.IsDeleted == false).Result.ToList();
            if (employeeProjectUserList != null && employeeProjectUserList.Count() > 0)
            {
                foreach (var item in employeeProjectUserList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeProjectUserRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeProjectUserList;
        }
        public List<EmployeeProjectUser> GetAssignUsersByEmployeeProject(long EmployeeProjectId)
        {
            return _unitOfWork.EmployeeProjectUserRepository.GetMany(t => t.EmployeeProjectId == EmployeeProjectId && t.IsDeleted == false).Result.Include(t => t.User).ToList();
        }

        public async Task<EmployeeProjectUser> UnAssign(long Id)
        {
            var employeeProjectUserObj = _unitOfWork.EmployeeProjectUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeProjectUserObj != null)
            {
                employeeProjectUserObj.IsDeleted = true;
                employeeProjectUserObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeProjectUserRepository.UpdateAsync(employeeProjectUserObj, employeeProjectUserObj.Id);
                await _unitOfWork.CommitAsync();
            }
            else
            {
                return null;
            }
            return employeeProjectUserObj;
        }
        public List<EmployeeProjectUser> GetByUserId(long UserId)
        {
            return _unitOfWork.EmployeeProjectUserRepository.GetMany(t => t.UserId == UserId && t.IsDeleted == false).Result.Include(t => t.User).Include(t => t.EmployeeProject).ToList();
        }       
        
    }
    public partial interface IEmployeeProjectUserService : IService<EmployeeProjectUser> {
        Task<EmployeeProjectUser> CheckInsertOrUpdate(EmployeeProjectUserDto model);
        bool IsExistOrNot(EmployeeProjectUserDto model);
        Task<List<EmployeeProjectUser>> DeleteByEmployeeProjectId(long ProjectId);
        List<EmployeeProjectUser> GetAssignUsersByEmployeeProject(long ProjectId);
        Task<EmployeeProjectUser> UnAssign(long Id);
        List<EmployeeProjectUser> GetByUserId(long UserId);
        
    }    
}