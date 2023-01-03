using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class EmployeeTaskUserSerivce : Service<EmployeeTaskUser>, IEmployeeTaskUserSerivce
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeTaskUserSerivce(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeTaskUser> CheckInsertOrUpdate(EmployeeTaskUserDto model)
        {
            var employeeTaskUserObj = _mapper.Map<EmployeeTaskUser>(model);
            var existingItem = _unitOfWork.EmployeeTaskUserRepository.GetMany(t => t.EmployeeTaskId == employeeTaskUserObj.EmployeeTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertTaskUser(employeeTaskUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public bool IsExistOrNot(EmployeeTaskUserDto model)
        {
            var existingItem = _unitOfWork.EmployeeTaskUserRepository.GetMany(t => t.EmployeeTaskId == model.EmployeeTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<EmployeeTaskUser> InsertTaskUser(EmployeeTaskUser employeeTaskUserObj)
        {
            employeeTaskUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeTaskUserRepository.AddAsync(employeeTaskUserObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<EmployeeTaskUser> GetAssignUsersByEmployeeTask(long EmployeeTaskId)
        {
            return _unitOfWork.EmployeeTaskUserRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.Include(t => t.User).ToList();
        }

        public List<EmployeeTaskUser> GetAll()
        {
            return _unitOfWork.EmployeeTaskUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public async Task<List<EmployeeTaskUser>> DeleteByEmployeeTaskId(long EmployeeTaskId)
        {
            var employeeTaskUserList = _unitOfWork.EmployeeTaskUserRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeTaskUserList != null && employeeTaskUserList.Count() > 0)
            {
                foreach (var item in employeeTaskUserList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeTaskUserRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskUserList;
        }

        public async Task<EmployeeTaskUser> DeleteAssignedEmployeeTaskUser(long Id)
        {
            var employeeTaskUserObj = _unitOfWork.EmployeeTaskUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeTaskUserObj != null)
            {
                employeeTaskUserObj.IsDeleted = true;
                employeeTaskUserObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeTaskUserRepository.UpdateAsync(employeeTaskUserObj, employeeTaskUserObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskUserObj;
        }
        public List<EmployeeTaskUser> GetByUserId(long UserId)
        {
            return _unitOfWork.EmployeeTaskUserRepository.GetMany(t => t.UserId == UserId && t.IsDeleted == false).Result.Include(t => t.User).Include(t => t.EmployeeTask).ToList();
        }
    }

    public partial interface IEmployeeTaskUserSerivce : IService<EmployeeTaskUser>
    {
        Task<EmployeeTaskUser> CheckInsertOrUpdate(EmployeeTaskUserDto model);
        List<EmployeeTaskUser> GetAll();
        List<EmployeeTaskUser> GetAssignUsersByEmployeeTask(long TaskId);
        bool IsExistOrNot(EmployeeTaskUserDto model);
        Task<List<EmployeeTaskUser>> DeleteByEmployeeTaskId(long TaskId);
        Task<EmployeeTaskUser> DeleteAssignedEmployeeTaskUser(long Id);
        List<EmployeeTaskUser> GetByUserId(long UserId);
    }
}