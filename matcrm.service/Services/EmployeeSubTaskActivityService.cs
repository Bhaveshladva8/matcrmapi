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
    public partial class EmployeeSubTaskActivityService : Service<EmployeeSubTaskActivity>, IEmployeeSubTaskActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeSubTaskActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeSubTaskActivity> CheckInsertOrUpdate(EmployeeSubTaskActivity employeeSubTaskActivityObj)
        {
            // var existingItem = _unitOfWork.EmployeeSubTaskActivityRepository.GetMany (t => t.UserId == obj.UserId && t.EmployeeSubTaskId == obj.EmployeeSubTaskId).Result.FirstOrDefault ();
            // if (existingItem == null) {
            //     return InsertEmployeeSubTaskActivity (obj);
            // } else {
            //     return existingItem;
            //     // return UpdateEmployeeSubTaskActivity (existingItem, existingItem.Id);
            // }
            return await InsertEmployeeSubTaskActivity(employeeSubTaskActivityObj);
        }

        public async Task<EmployeeSubTaskActivity> InsertEmployeeSubTaskActivity(EmployeeSubTaskActivity employeeSubTaskActivityObj)
        {
            employeeSubTaskActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeSubTaskActivityRepository.AddAsync(employeeSubTaskActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public EmployeeSubTaskActivity UpdateEmployeeSubTaskActivity(EmployeeSubTaskActivity existingItem, int existingId)
        {
            _unitOfWork.EmployeeSubTaskActivityRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeSubTaskActivity> GetAllByEmployeeSubTaskId(long EmployeeSubTaskId)
        {
            return _unitOfWork.EmployeeSubTaskActivityRepository.GetMany(t => t.EmployeeSubTaskId == EmployeeSubTaskId).Result.ToList();
        }

        public EmployeeSubTaskActivity GetEmployeeSubTaskActivitytById(long Id)
        {
            return _unitOfWork.EmployeeSubTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public EmployeeSubTaskActivity DeleteEmployeeSubTaskActivityById(long Id)
        {
            var employeeSubTaskActivityObj = _unitOfWork.EmployeeSubTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (employeeSubTaskActivityObj != null)
            {
                _unitOfWork.EmployeeSubTaskActivityRepository.DeleteAsync(employeeSubTaskActivityObj);
                _unitOfWork.CommitAsync();
            }
            return employeeSubTaskActivityObj;
        }

        public async Task<List<EmployeeSubTaskActivity>> DeleteByEmployeeSubTaskId(long EmployeeSubTaskId)
        {
            var employeeSubTaskActivitiesList = _unitOfWork.EmployeeSubTaskActivityRepository.GetMany(t => t.EmployeeSubTaskId == EmployeeSubTaskId).Result.ToList();
            if (employeeSubTaskActivitiesList != null && employeeSubTaskActivitiesList.Count() > 0)
            {
                foreach (var item in employeeSubTaskActivitiesList)
                {
                    await _unitOfWork.EmployeeSubTaskActivityRepository.DeleteAsync(item);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskActivitiesList;
        }

    }

    public partial interface IEmployeeSubTaskActivityService : IService<EmployeeSubTaskActivity>
    {
        Task<EmployeeSubTaskActivity> CheckInsertOrUpdate(EmployeeSubTaskActivity model);
        List<EmployeeSubTaskActivity> GetAllByEmployeeSubTaskId(long taskId);
        EmployeeSubTaskActivity GetEmployeeSubTaskActivitytById(long Id);
        EmployeeSubTaskActivity DeleteEmployeeSubTaskActivityById(long Id);
        Task<List<EmployeeSubTaskActivity>> DeleteByEmployeeSubTaskId(long taskId);
    }
}