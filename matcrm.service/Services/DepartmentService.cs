using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;
using Microsoft.EntityFrameworkCore;

namespace matcrm.service.Services
{
    public partial class DepartmentService : Service<Department>, IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public DepartmentService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Department> CheckInsertOrUpdate(Department DepartmentObj)
        {
            var existingItem = _unitOfWork.DepartmentRepository.GetMany(t => t.Id == DepartmentObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertDepartment(DepartmentObj);
            }
            else
            {
                DepartmentObj.CreatedBy = existingItem.CreatedBy;
                DepartmentObj.CreatedOn = existingItem.CreatedOn;
                return await UpdateDepartment(DepartmentObj, existingItem.Id);
            }
        }
        public async Task<Department> InsertDepartment(Department DepartmentObj)
        {
            DepartmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.DepartmentRepository.AddAsync(DepartmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<Department> UpdateDepartment(Department existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.DepartmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }
        public async Task<Department> DeleteById(int Id)
        {
            var departmentObj = _unitOfWork.DepartmentRepository.GetMany(u => u.Id == Id && u.DeletedOn == null).Result.FirstOrDefault();
            if (departmentObj != null)
            {
                departmentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.DepartmentRepository.UpdateAsync(departmentObj, departmentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return departmentObj;
        }
        public List<Department> GetAll()
        {
            return _unitOfWork.DepartmentRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
        public Department GetById(long Id)
        {
            return _unitOfWork.DepartmentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }
        public List<Department> GetByTenant(long TenantId)
        {
            return _unitOfWork.DepartmentRepository.GetMany(t => (t.CreatedBy == null || t.CreatedUser.TenantId == TenantId) && t.DeletedOn == null).Result.Include(t => t.CreatedUser).ToList();
        }
    }
    public partial interface IDepartmentService : IService<Department>
    {
        Task<Department> CheckInsertOrUpdate(Department model);
        Task<Department> DeleteById(int Id);
        List<Department> GetAll();
        Department GetById(long Id);
        List<Department> GetByTenant(long TenantId);
    }
}