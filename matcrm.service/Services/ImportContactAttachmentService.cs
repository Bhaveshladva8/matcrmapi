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
    public partial class ImportContactAttachmentService : Service<ImportContactAttachment>, IImportContactAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ImportContactAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ImportContactAttachment> CheckInsertOrUpdate(ImportContactAttachmentDto model)
        {
            var importContactAttachmentObj = _mapper.Map<ImportContactAttachment>(model);
            var existingItem = _unitOfWork.ImportContactAttachmentRepository.GetMany(t => t.FileName == importContactAttachmentObj.FileName && t.ModuleId == importContactAttachmentObj.ModuleId && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertImportContactAttachment(importContactAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateImportContactAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<ImportContactAttachment> InsertImportContactAttachment(ImportContactAttachment importContactAttachmentObj)
        {
            importContactAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ImportContactAttachmentRepository.AddAsync(importContactAttachmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ImportContactAttachment> UpdateImportContactAttachment(ImportContactAttachment existingItem, long existingId)
        {
            await _unitOfWork.ImportContactAttachmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<ImportContactAttachment> GetAllByModuleId(long ModuleId)
        {
            return _unitOfWork.ImportContactAttachmentRepository.GetMany(t => t.ModuleId == ModuleId && t.DeletedOn == null).Result.ToList();
        }

        public ImportContactAttachment GetImportContactAttachmentById(long Id)
        {
            return _unitOfWork.ImportContactAttachmentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
        }

        public ImportContactAttachment DeleteImportContactAttachmentById(long Id)
        {
            var importContactAttachmentObj = _unitOfWork.ImportContactAttachmentRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (importContactAttachmentObj != null)
            {
                importContactAttachmentObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.ImportContactAttachmentRepository.UpdateAsync(importContactAttachmentObj, importContactAttachmentObj.Id);
                _unitOfWork.CommitAsync();
            }
            return importContactAttachmentObj;
        }


        public List<ImportContactAttachment> DeleteAttachmentByModuleId(long ModuleId)
        {
            var importContactAttachmentsList = _unitOfWork.ImportContactAttachmentRepository.GetMany(t => t.ModuleId == ModuleId && t.DeletedOn == null).Result.ToList();
            if (importContactAttachmentsList != null && importContactAttachmentsList.Count() > 0)
            {
                foreach (var item in importContactAttachmentsList)
                {
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.ImportContactAttachmentRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return importContactAttachmentsList;
        }
    }
    public partial interface IImportContactAttachmentService : IService<ImportContactAttachment>
    {
        Task<ImportContactAttachment> CheckInsertOrUpdate(ImportContactAttachmentDto model);
        List<ImportContactAttachment> GetAllByModuleId(long CustomerId);
        ImportContactAttachment GetImportContactAttachmentById(long Id);
        ImportContactAttachment DeleteImportContactAttachmentById(long Id);
        List<ImportContactAttachment> DeleteAttachmentByModuleId(long ModuleId);
        Task<ImportContactAttachment> UpdateImportContactAttachment(ImportContactAttachment existingItem, long existingId);
    }
}