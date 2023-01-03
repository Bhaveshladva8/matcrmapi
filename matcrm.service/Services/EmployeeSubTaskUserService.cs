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
    public partial class EmployeeSubTaskUserService : Service<EmployeeSubTaskUser>, IEmployeeSubTaskUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeSubTaskUserService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeSubTaskUser> CheckInsertOrUpdate(EmployeeSubTaskUserDto model)
        {
            var employeeSubTaskUserObj = _mapper.Map<EmployeeSubTaskUser>(model);
            var existingItem = _unitOfWork.EmployeeSubTaskUserRepository.GetMany(t => t.EmployeeSubTaskId == employeeSubTaskUserObj.EmployeeSubTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSubTaskUser(employeeSubTaskUserObj);
            }
            else
            {
                return existingItem;
            }
        }

        public async Task<EmployeeSubTaskUser> InsertSubTaskUser(EmployeeSubTaskUser employeeSubTaskUserObj)
        {
            employeeSubTaskUserObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeSubTaskUserRepository.AddAsync(employeeSubTaskUserObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<EmployeeSubTaskUser> GetAssignUsersBySubTask(long SubTaskId)
        {
            return _unitOfWork.EmployeeSubTaskUserRepository.GetMany(t => t.EmployeeSubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
        }

        public List<EmployeeSubTaskUser> GetAll()
        {
            return _unitOfWork.EmployeeSubTaskUserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public bool IsExistOrNot(EmployeeSubTaskUserDto model)
        {
            var existingItem = _unitOfWork.EmployeeSubTaskUserRepository.GetMany(t => t.EmployeeSubTaskId == model.EmployeeSubTaskId && t.UserId == model.UserId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<List<EmployeeSubTaskUser>> DeleteBySubTaskId(long SubTaskId)
        {
            var employeeSubTaskUserList = _unitOfWork.EmployeeSubTaskUserRepository.GetMany(t => t.EmployeeSubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeSubTaskUserList != null && employeeSubTaskUserList.Count() > 0)
            {
                foreach (var item in employeeSubTaskUserList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeSubTaskUserRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskUserList;
        }
        public async Task<EmployeeSubTaskUser> DeleteAssignedSubTaskUser(long Id)
        {
            var employeeSubTaskUserObj = _unitOfWork.EmployeeSubTaskUserRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeSubTaskUserObj != null)
            {
                employeeSubTaskUserObj.IsDeleted = true;
                employeeSubTaskUserObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.EmployeeSubTaskUserRepository.UpdateAsync(employeeSubTaskUserObj, employeeSubTaskUserObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskUserObj;
        }
    }

    public partial interface IEmployeeSubTaskUserService : IService<EmployeeSubTaskUser>
    {
        Task<EmployeeSubTaskUser> CheckInsertOrUpdate(EmployeeSubTaskUserDto model);
        List<EmployeeSubTaskUser> GetAll();
        List<EmployeeSubTaskUser> GetAssignUsersBySubTask(long SubTaskId);
        bool IsExistOrNot(EmployeeSubTaskUserDto model);
        Task<List<EmployeeSubTaskUser>> DeleteBySubTaskId(long SubTaskId);
        Task<EmployeeSubTaskUser> DeleteAssignedSubTaskUser(long Id);
    }
}