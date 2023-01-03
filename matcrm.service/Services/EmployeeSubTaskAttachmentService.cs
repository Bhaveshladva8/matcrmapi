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
    public partial class EmployeeSubTaskAttachmentService : Service<EmployeeSubTaskAttachment>, IEmployeeSubTaskAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeSubTaskAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeSubTaskAttachment> CheckInsertOrUpdate(EmployeeSubTaskAttachmentDto model)
        {
            var employeeSubTaskAttachmentObj = _mapper.Map<EmployeeSubTaskAttachment>(model);
            var existingItem = _unitOfWork.EmployeeSubTaskAttachmentRepository.GetMany(t => t.Name == employeeSubTaskAttachmentObj.Name && t.EmployeeSubTaskId == employeeSubTaskAttachmentObj.EmployeeSubTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeSubTaskAttachment(employeeSubTaskAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateEmployeeSubTaskAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<EmployeeSubTaskAttachment> InsertEmployeeSubTaskAttachment(EmployeeSubTaskAttachment employeeSubTaskAttachmentObj)
        {
            employeeSubTaskAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeSubTaskAttachmentRepository.AddAsync(employeeSubTaskAttachmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public EmployeeSubTaskAttachment UpdateEmployeeSubTaskAttachment(EmployeeSubTaskAttachment existingItem, int existingId)
        {
            _unitOfWork.EmployeeSubTaskAttachmentRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeSubTaskAttachment> GetAllByEmployeeSubTaskId(long EmployeeSubTaskId)
        {
            return _unitOfWork.EmployeeSubTaskAttachmentRepository.GetMany(t => t.EmployeeSubTaskId == EmployeeSubTaskId && t.IsDeleted == false).Result.ToList();
        }

        public EmployeeSubTaskAttachment GetEmployeeSubTaskAttachmentById(long Id)
        {
            return _unitOfWork.EmployeeSubTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<EmployeeSubTaskAttachment> DeleteEmployeeSubTaskAttachmentById(long Id)
        {
            var employeeSubTaskAttachmentObj = _unitOfWork.EmployeeSubTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeSubTaskAttachmentObj != null)
            {
                employeeSubTaskAttachmentObj.IsDeleted = true;
                employeeSubTaskAttachmentObj.DeletedOn = DateTime.UtcNow;
                await _unitOfWork.EmployeeSubTaskAttachmentRepository.UpdateAsync(employeeSubTaskAttachmentObj, employeeSubTaskAttachmentObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskAttachmentObj;
        }

        public async Task<List<EmployeeSubTaskAttachment>> DeleteAttachmentByEmployeeSubTaskId(long EmployeeSubTaskId)
        {
            var employeeSubTaskAttachmentsList = _unitOfWork.EmployeeSubTaskAttachmentRepository.GetMany(t => t.EmployeeSubTaskId == EmployeeSubTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeSubTaskAttachmentsList != null && employeeSubTaskAttachmentsList.Count() > 0)
            {
                foreach (var item in employeeSubTaskAttachmentsList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeSubTaskAttachmentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeSubTaskAttachmentsList;
        }

    }

    public partial interface IEmployeeSubTaskAttachmentService : IService<EmployeeSubTaskAttachment>
    {
        Task<EmployeeSubTaskAttachment> CheckInsertOrUpdate(EmployeeSubTaskAttachmentDto model);
        List<EmployeeSubTaskAttachment> GetAllByEmployeeSubTaskId(long EmployeeSubTaskId);
        EmployeeSubTaskAttachment GetEmployeeSubTaskAttachmentById(long Id);
        Task<EmployeeSubTaskAttachment> DeleteEmployeeSubTaskAttachmentById(long Id);
        Task<List<EmployeeSubTaskAttachment>> DeleteAttachmentByEmployeeSubTaskId(long EmployeeSubTaskId);
    }
}