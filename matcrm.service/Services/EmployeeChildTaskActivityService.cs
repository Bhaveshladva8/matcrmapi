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
    public partial class EmployeeChildTaskActivityService : Service<EmployeeChildTaskActivity>, IEmployeeChildTaskActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeChildTaskActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeChildTaskActivity> CheckInsertOrUpdate(EmployeeChildTaskActivity employeeChildTaskActivityObj)
        {
            var existingItem = _unitOfWork.EmployeeChildTaskActivityRepository.GetMany(t => t.Id == employeeChildTaskActivityObj.Id).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeChildTaskActivity(employeeChildTaskActivityObj);
            }
            else
            {
                return existingItem;
                // return UpdateEmployeeChildTaskActivity (existingItem, existingItem.Id);
            }
        }

        public async Task<EmployeeChildTaskActivity> InsertEmployeeChildTaskActivity(EmployeeChildTaskActivity employeeChildTaskActivityObj)
        {
            employeeChildTaskActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeChildTaskActivityRepository.AddAsync(employeeChildTaskActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeChildTaskActivity> UpdateEmployeeChildTaskActivity(EmployeeChildTaskActivity existingItem, int existingId)
        {
            await _unitOfWork.EmployeeChildTaskActivityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeChildTaskActivity> GetAllByEmployeeChildTaskId(long EmployeeChildTaskId)
        {
            return _unitOfWork.EmployeeChildTaskActivityRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId).Result.ToList();
        }

        public EmployeeChildTaskActivity GetEmployeeChildTaskActivitytById(long Id)
        {
            return _unitOfWork.EmployeeChildTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public EmployeeChildTaskActivity DeleteEmployeeChildTaskActivityById(long Id)
        {
            var employeeChildTaskActivitiesObj = _unitOfWork.EmployeeChildTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (employeeChildTaskActivitiesObj != null)
            {
                _unitOfWork.EmployeeChildTaskActivityRepository.DeleteAsync(employeeChildTaskActivitiesObj);
                _unitOfWork.CommitAsync();
            }

            return employeeChildTaskActivitiesObj;
        }

        public async Task<List<EmployeeChildTaskActivity>> DeleteByEmployeeChildTaskId(long EmployeeChildTaskId)
        {
            var employeeChildTaskActivitiesList = _unitOfWork.EmployeeChildTaskActivityRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId).Result.ToList();
            if (employeeChildTaskActivitiesList != null && employeeChildTaskActivitiesList.Count() > 0)
            {
                foreach (var item in employeeChildTaskActivitiesList)
                {
                    await _unitOfWork.EmployeeChildTaskActivityRepository.DeleteAsync(item);
                }

                await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskActivitiesList;
        }

    }

    public partial interface IEmployeeChildTaskActivityService : IService<EmployeeChildTaskActivity>
    {
        Task<EmployeeChildTaskActivity> CheckInsertOrUpdate(EmployeeChildTaskActivity model);
        List<EmployeeChildTaskActivity> GetAllByEmployeeChildTaskId(long EmployeeChildTaskId);
        EmployeeChildTaskActivity GetEmployeeChildTaskActivitytById(long Id);
        EmployeeChildTaskActivity DeleteEmployeeChildTaskActivityById(long Id);
        Task<List<EmployeeChildTaskActivity>> DeleteByEmployeeChildTaskId(long EmployeeChildTaskId);
    }
}