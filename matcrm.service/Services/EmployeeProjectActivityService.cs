using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class EmployeeProjectActivityService : Service<EmployeeProjectActivity>, IEmployeeProjectActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeProjectActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeProjectActivity> CheckInsertOrUpdate(EmployeeProjectActivity employeeProjectActivityObj)
        {
            // var existingItem = _unitOfWork.EmployeeProjectActivityRepository.GetMany (t => t.UserId == obj.UserId && t.TaskId == obj.TaskId).Result.FirstOrDefault ();
            // if (existingItem == null) {
            //     return InsertEmployeeProjectActivity (obj);
            // } else {
            //     return existingItem;
            //     // return UpdateEmployeeProjectActivity (existingItem, existingItem.Id);
            // }
            return await InsertEmployeeProjectActivity(employeeProjectActivityObj);
        }

        public async Task<EmployeeProjectActivity> InsertEmployeeProjectActivity(EmployeeProjectActivity employeeProjectActivityObj)
        {
            employeeProjectActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeProjectActivityRepository.AddAsync(employeeProjectActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeProjectActivity> UpdateEmployeeProjectActivity(EmployeeProjectActivity existingItem, int existingId)
        {
            await _unitOfWork.EmployeeProjectActivityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeProjectActivity> GetAllByProjectId(long ProjectId)
        {
            return _unitOfWork.EmployeeProjectActivityRepository.GetMany(t => t.ProjectId == ProjectId).Result.ToList();
        }

        public EmployeeProjectActivity GetEmployeeProjectActivityById(long Id)
        {
            return _unitOfWork.EmployeeProjectActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public List<EmployeeProjectActivity> DeleteActivityByProjectId(long ProjectId)
        {
            var employeeProjectActivitiesList = _unitOfWork.EmployeeProjectActivityRepository.GetMany(t => t.ProjectId == ProjectId).Result.ToList();
            if (employeeProjectActivitiesList != null)
            {
                foreach (var item in employeeProjectActivitiesList)
                {
                    _unitOfWork.EmployeeProjectActivityRepository.DeleteAsync(item);
                }
                _unitOfWork.CommitAsync();
            }
            return employeeProjectActivitiesList;
        }
    }

    public partial interface IEmployeeProjectActivityService : IService<EmployeeProjectActivity>
    {
        Task<EmployeeProjectActivity> CheckInsertOrUpdate(EmployeeProjectActivity model);
        List<EmployeeProjectActivity> GetAllByProjectId(long ProjectId);
        EmployeeProjectActivity GetEmployeeProjectActivityById(long Id);
        List<EmployeeProjectActivity> DeleteActivityByProjectId(long ProjectId);

    }
}