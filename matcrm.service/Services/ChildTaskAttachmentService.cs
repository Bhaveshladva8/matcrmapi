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
    public partial class ChildTaskAttachmentService : Service<ChildTaskAttachment>, IChildTaskAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ChildTaskAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public ChildTaskAttachment CheckInsertOrUpdate(ChildTaskAttachmentDto model)
        {
            var childTaskAttachmentObj = _mapper.Map<ChildTaskAttachment>(model);
            var existingItem = _unitOfWork.ChildTaskAttachmentRepository.GetMany(t => t.Name == childTaskAttachmentObj.Name && t.ChildTaskId == childTaskAttachmentObj.ChildTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertChildTaskAttachment(childTaskAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateTaskAttachment (existingItem, existingItem.Id);
            }
        }

        public ChildTaskAttachment InsertChildTaskAttachment(ChildTaskAttachment childTaskAttachmentObj)
        {
            childTaskAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.ChildTaskAttachmentRepository.Add(childTaskAttachmentObj);
            _unitOfWork.CommitAsync();
            return newItem;
        }
        public ChildTaskAttachment UpdateChildTaskAttachment(ChildTaskAttachment existingItem, int existingId)
        {
            _unitOfWork.ChildTaskAttachmentRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();
            return existingItem;
        }

        public List<ChildTaskAttachment> GetAllByChildTaskId(long childTaskId)
        {
            return _unitOfWork.ChildTaskAttachmentRepository.GetMany(t => t.ChildTaskId == childTaskId && t.IsDeleted == false).Result.ToList();
        }

        public ChildTaskAttachment GetChildTaskAttachmentById(long Id)
        {
            return _unitOfWork.ChildTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public async Task<ChildTaskAttachment> DeleteChildTaskAttachmentById(long Id)
        {
            var childTaskAttachmentObj = _unitOfWork.ChildTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (childTaskAttachmentObj != null)
            {
                childTaskAttachmentObj.IsDeleted = true;
                childTaskAttachmentObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.ChildTaskAttachmentRepository.UpdateAsync(childTaskAttachmentObj, childTaskAttachmentObj.Id);
                await _unitOfWork.CommitAsync();
            }

            return childTaskAttachmentObj;
        }

        public async Task<List<ChildTaskAttachment>> DeleteAttachmentByChildTaskId(long ChildTaskId)
        {
            var childTaskAttachmentsList = _unitOfWork.ChildTaskAttachmentRepository.GetMany(t => t.ChildTaskId == ChildTaskId && t.IsDeleted == false).Result.ToList();
            if (childTaskAttachmentsList != null && childTaskAttachmentsList.Count() > 0)
            {
                foreach (var item in childTaskAttachmentsList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.ChildTaskAttachmentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }

            return childTaskAttachmentsList;
        }
    }

    public partial interface IChildTaskAttachmentService : IService<ChildTaskAttachment>
    {
        ChildTaskAttachment CheckInsertOrUpdate(ChildTaskAttachmentDto model);
        List<ChildTaskAttachment> GetAllByChildTaskId(long childTaskId);
        ChildTaskAttachment GetChildTaskAttachmentById(long Id);
        Task<ChildTaskAttachment> DeleteChildTaskAttachmentById(long Id);
        Task<List<ChildTaskAttachment>> DeleteAttachmentByChildTaskId(long ChildTaskId);
    }
}