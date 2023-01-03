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
    public partial class EmployeeChildTaskUserService : Service<EmployeeChildTaskUser>, IEmployeeChildTaskUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeChildTaskUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeChildTaskUser> CheckInsertOrUpdate(EmployeeChildTaskUserDto model)
        {
            var employeeChildTaskUserObj = _mapper.Map<EmployeeChildTaskUser>(model);
            var existingItem = _unitOfWork.EmployeeChildTaskUserRepository.GetMany(t => t.EmployeeChildTaskId == employeeChildTaskUserObj.EmployeeChildTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertChildTaskUser(employeeChildTaskUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public async Task<EmployeeChildTaskUser> InsertChildTaskUser(EmployeeChildTaskUser employeeChildTaskUserObj)
        {
            employeeChildTaskUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeChildTaskUserRepository.AddAsync(employeeChildTaskUserObj);
            await _unitOfWork.CommitAsync();
            return newItem;           
        }

        public List<EmployeeChildTaskUser> GetAssignUsersByChildTask(long ChildTaskId)
        {
            return _unitOfWork.EmployeeChildTaskUserRepository.GetMany(t => t.EmployeeChildTaskId == ChildTaskId && t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeChildTaskUser> GetAll()
        {
            return _unitOfWork.EmployeeChildTaskUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public bool IsExistOrNot(EmployeeChildTaskUserDto model)
        {
            var existingItem = _unitOfWork.EmployeeChildTaskUserRepository.GetMany(t => t.EmployeeChildTaskId == model.EmployeeChildTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<List<EmployeeChildTaskUser>> DeleteByChildTaskId(long ChildTaskId)
        {
            var employeeChildTaskUsersList = _unitOfWork.EmployeeChildTaskUserRepository.GetMany(t => t.EmployeeChildTaskId == ChildTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeChildTaskUsersList != null && employeeChildTaskUsersList.Count() > 0)
            {
                foreach (var item in employeeChildTaskUsersList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeChildTaskUserRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskUsersList;
        }
        public async Task<EmployeeChildTaskUser> DeleteAssignedChildTaskUser(long Id)
        {
            var employeeChildTaskUsersObj = _unitOfWork.EmployeeChildTaskUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeChildTaskUsersObj != null)
            {
                employeeChildTaskUsersObj.IsDeleted = true;
                employeeChildTaskUsersObj.DeletedOn = DateTime.UtcNow;

               await _unitOfWork.EmployeeChildTaskUserRepository.UpdateAsync(employeeChildTaskUsersObj, employeeChildTaskUsersObj.Id);
               await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskUsersObj;
        }
    }

    public partial interface IEmployeeChildTaskUserService : IService<EmployeeChildTaskUser>
    {
        Task<EmployeeChildTaskUser> CheckInsertOrUpdate(EmployeeChildTaskUserDto model);
        List<EmployeeChildTaskUser> GetAll();
        List<EmployeeChildTaskUser> GetAssignUsersByChildTask(long TaskId);
        bool IsExistOrNot(EmployeeChildTaskUserDto model);
        Task<List<EmployeeChildTaskUser>> DeleteByChildTaskId(long ChildTaskId);
        Task<EmployeeChildTaskUser> DeleteAssignedChildTaskUser(long Id);
    }
}