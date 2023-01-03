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
    public partial class EmployeeTaskAttachmentService : Service<EmployeeTaskAttachment>, IEmployeeTaskAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeTaskAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<EmployeeTaskAttachment> CheckInsertOrUpdate(EmployeeTaskAttachmentDto model)
        {
            var employeeTaskAttachmentObj = _mapper.Map<EmployeeTaskAttachment>(model);
            var existingItem = _unitOfWork.EmployeeTaskAttachmentRepository.GetMany(t => t.Id == employeeTaskAttachmentObj.Id && t.EmployeeTaskId == employeeTaskAttachmentObj.EmployeeTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertEmployeeTaskAttachment(employeeTaskAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateEmployeeTaskAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<EmployeeTaskAttachment> InsertEmployeeTaskAttachment(EmployeeTaskAttachment employeeTaskAttachmentObj)
        {
            employeeTaskAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.EmployeeTaskAttachmentRepository.AddAsync(employeeTaskAttachmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<EmployeeTaskAttachment> UpdateEmployeeTaskAttachment(EmployeeTaskAttachment existingItem, int existingId)
        {
            await _unitOfWork.EmployeeTaskAttachmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<EmployeeTaskAttachment> GetAllByEmployeeTaskId(long EmployeeTaskId)
        {
            return _unitOfWork.EmployeeTaskAttachmentRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.ToList();
        }

        public EmployeeTaskAttachment GetEmployeeTaskAttachmentById(long Id)
        {
            return _unitOfWork.EmployeeTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public EmployeeTaskAttachment DeleteEmployeeTaskAttachmentById(long Id)
        {
            var employeeTaskAttachmentObj = _unitOfWork.EmployeeTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (employeeTaskAttachmentObj != null)
            {
                employeeTaskAttachmentObj.IsDeleted = true;
                employeeTaskAttachmentObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.EmployeeTaskAttachmentRepository.UpdateAsync(employeeTaskAttachmentObj, employeeTaskAttachmentObj.Id);
                _unitOfWork.CommitAsync();
            }
            return employeeTaskAttachmentObj;
        }


        public async Task<List<EmployeeTaskAttachment>> DeleteAttachmentByTaskId(long EmployeeTaskId)
        {
            var employeeTaskAttachmentsList = _unitOfWork.EmployeeTaskAttachmentRepository.GetMany(t => t.EmployeeTaskId == EmployeeTaskId && t.IsDeleted == false).Result.ToList();
            if (employeeTaskAttachmentsList != null && employeeTaskAttachmentsList.Count() > 0)
            {
                foreach (var item in employeeTaskAttachmentsList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.EmployeeTaskAttachmentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return employeeTaskAttachmentsList;
        }
    }
    public partial interface IEmployeeTaskAttachmentService : IService<EmployeeTaskAttachment>
    {
        Task<EmployeeTaskAttachment> CheckInsertOrUpdate(EmployeeTaskAttachmentDto model);
        List<EmployeeTaskAttachment> GetAllByEmployeeTaskId(long EmployeeTaskId);
        EmployeeTaskAttachment GetEmployeeTaskAttachmentById(long Id);
        EmployeeTaskAttachment DeleteEmployeeTaskAttachmentById(long Id);
        Task<List<EmployeeTaskAttachment>> DeleteAttachmentByTaskId(long EmployeeTaskId);
    }
}