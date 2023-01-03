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
    public partial class EmployeeChildTaskAttachmentService : Service<EmployeeChildTaskAttachment>, IEmployeeChildTaskAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeChildTaskAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public EmployeeChildTaskAttachment CheckInsertOrUpdate(EmployeeChildTaskAttachmentDto model)
        {
            var employeeChildTaskAttachmentObj = _mapper.Map<EmployeeChildTaskAttachment>(model);
            var existingItem = _unitOfWork.EmployeeChildTaskAttachmentRepository.GetMany(t => t.Name == employeeChildTaskAttachmentObj.Name && t.EmployeeChildTaskId == employeeChildTaskAttachmentObj.EmployeeChildTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertEmployeeChildTaskAttachment(employeeChildTaskAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateTaskAttachment (existingItem, existingItem.Id);
            }
        }

        public EmployeeChildTaskAttachment InsertEmployeeChildTaskAttachment(EmployeeChildTaskAttachment employeeChildTaskAttachmentObj)
        {
            employeeChildTaskAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.EmployeeChildTaskAttachmentRepository.Add(employeeChildTaskAttachmentObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }
        public EmployeeChildTaskAttachment UpdateEmployeeChildTaskAttachment(EmployeeChildTaskAttachment existingItem, int existingId)
        {
            _unitOfWork.EmployeeChildTaskAttachmentRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();
            return existingItem;
        }

        public List<EmployeeChildTaskAttachment> GetAllByChildTaskId(long EmployeeChildTaskId)
        {
            return _unitOfWork.EmployeeChildTaskAttachmentRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId && t.IsDeleted == false).Result.ToList();
        }

        public EmployeeChildTaskAttachment GetEmployeeChildTaskAttachmentById(long Id)
        {
            return _unitOfWork.EmployeeChildTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<EmployeeChildTaskAttachment> DeleteEmployeeChildTaskAttachmentById(long Id)
        {
            var employeeChildTaskAttachmentObj = _unitOfWork.EmployeeChildTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeChildTaskAttachmentObj != null)
            {
                employeeChildTaskAttachmentObj.IsDeleted = true;
                employeeChildTaskAttachmentObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.EmployeeChildTaskAttachmentRepository.UpdateAsync(employeeChildTaskAttachmentObj, employeeChildTaskAttachmentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskAttachmentObj;
        }

        public async Task<List<EmployeeChildTaskAttachment>> DeleteAttachmentByChildTaskId(long EmployeeChildTaskId)
        {
            var employeeChildTaskAttachmentList = _unitOfWork.EmployeeChildTaskAttachmentRepository.GetMany(t => t.EmployeeChildTaskId == EmployeeChildTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeChildTaskAttachmentList != null && employeeChildTaskAttachmentList.Count() > 0)
            {
                foreach (var item in employeeChildTaskAttachmentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeChildTaskAttachmentRepository.UpdateAsync(item, item.Id);
                }

                await _unitOfWork.CommitAsync();
            }
            return employeeChildTaskAttachmentList;
        }
    }

    public partial interface IEmployeeChildTaskAttachmentService : IService<EmployeeChildTaskAttachment>
    {
        EmployeeChildTaskAttachment CheckInsertOrUpdate(EmployeeChildTaskAttachmentDto model);
        List<EmployeeChildTaskAttachment> GetAllByChildTaskId(long childTaskId);
        EmployeeChildTaskAttachment GetEmployeeChildTaskAttachmentById(long Id);
        Task<EmployeeChildTaskAttachment> DeleteEmployeeChildTaskAttachmentById(long Id);
        Task<List<EmployeeChildTaskAttachment>> DeleteAttachmentByChildTaskId(long ChildTaskId);
    }
}