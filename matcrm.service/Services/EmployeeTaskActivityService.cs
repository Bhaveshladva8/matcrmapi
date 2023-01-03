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
    public partial class EmployeeTaskActivityService : Service<EmployeeTaskActivity>, IEmployeeTaskActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeTaskActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeTaskActivity> CheckInsertOrUpdate(EmployeeTaskActivity employeeTaskActivityObj)
        {
            // var existingItem = _unitOfWork.EmployeeTaskActivityRepository.GetMany (t => t.UserId == obj.UserId && t.TaskId == obj.TaskId).Result.FirstOrDefault ();
            // if (existingItem == null) {
            //     return InsertEmployeeTaskActivity (obj);
            // } else {
            //     return existingItem;
            //     // return UpdateEmployeeTaskActivity (existingItem, existingItem.Id);
            // }
            return await InsertEmployeeTaskActivity(employeeTaskActivityObj);
        }

        public async Task<EmployeeTaskActivity> InsertEmployeeTaskActivity(EmployeeTaskActivity employeeTaskActivityObj)
        {
            employeeTaskActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeTaskActivityRepository.AddAsync(employeeTaskActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeTaskActivity> UpdateEmployeeTaskActivity(EmployeeTaskActivity existingItem, int existingId)
        {
            await _unitOfWork.EmployeeTaskActivityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeTaskActivity> GetAllByEmployeeTaskId(long EmployeeTaskId)
        {
            return _unitOfWork.EmployeeTaskActivityRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId).Result.ToList();
        }

        public EmployeeTaskActivity GetById(long Id)
        {
            return _unitOfWork.EmployeeTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public async Task<EmployeeTaskActivity> DeleteById(long Id)
        {
            var employeeTaskActivityObj = _unitOfWork.EmployeeTaskActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();

            if (employeeTaskActivityObj != null)
            {
                await _unitOfWork.EmployeeTaskActivityRepository.DeleteAsync(employeeTaskActivityObj);
                await _unitOfWork.CommitAsync();
            }

            return employeeTaskActivityObj;
        }

        public async Task<List<EmployeeTaskActivity>> DeleteByEmployeeTaskId(long EmployeeTaskId)
        {
            var employeeTaskActivitiesList = _unitOfWork.EmployeeTaskActivityRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId).Result.ToList();
            if (employeeTaskActivitiesList != null && employeeTaskActivitiesList.Count() > 0)
            {
                foreach (var item in employeeTaskActivitiesList)
                {
                    await _unitOfWork.EmployeeTaskActivityRepository.DeleteAsync(item);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskActivitiesList;
        }

    }

    public partial interface IEmployeeTaskActivityService : IService<EmployeeTaskActivity>
    {
        Task<EmployeeTaskActivity> CheckInsertOrUpdate(EmployeeTaskActivity model);
        List<EmployeeTaskActivity> GetAllByEmployeeTaskId(long EmployeeTaskId);
        EmployeeTaskActivity GetById(long Id);
        Task<EmployeeTaskActivity> DeleteById(long Id);
        Task<List<EmployeeTaskActivity>> DeleteByEmployeeTaskId(long EmployeeTaskId);
    }
}