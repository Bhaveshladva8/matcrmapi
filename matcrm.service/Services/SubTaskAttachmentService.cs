using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class SubTaskAttachmentService : Service<SubTaskAttachment>, ISubTaskAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SubTaskAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public SubTaskAttachment CheckInsertOrUpdate(SubTaskAttachmentDto model)
        {
            var subTaskAttachmentObj = _mapper.Map<SubTaskAttachment>(model);
            var existingItem = _unitOfWork.SubTaskAttachmentRepository.GetMany(t => t.Name == subTaskAttachmentObj.Name && t.SubTaskId == subTaskAttachmentObj.SubTaskId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertSubTaskAttachment(subTaskAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateSubTaskAttachment (existingItem, existingItem.Id);
            }
        }

        public SubTaskAttachment InsertSubTaskAttachment(SubTaskAttachment subTaskAttachmentObj)
        {
            subTaskAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.SubTaskAttachmentRepository.Add(subTaskAttachmentObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public SubTaskAttachment UpdateSubTaskAttachment(SubTaskAttachment existingItem, int existingId)
        {
            _unitOfWork.SubTaskAttachmentRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<SubTaskAttachment> GetAllBySubTaskId(long subTaskId)
        {
            return _unitOfWork.SubTaskAttachmentRepository.GetMany(t => t.SubTaskId == subTaskId && t.IsDeleted == false).Result.ToList();
        }

        public SubTaskAttachment GetSubTaskAttachmentById(long Id)
        {
            return _unitOfWork.SubTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public SubTaskAttachment DeleteSubTaskAttachmentById(long Id)
        {
            var subTaskAttachmentObj = _unitOfWork.SubTaskAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (subTaskAttachmentObj != null)
            {
                subTaskAttachmentObj.IsDeleted = true;
                subTaskAttachmentObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.SubTaskAttachmentRepository.UpdateAsync(subTaskAttachmentObj, subTaskAttachmentObj.Id);
                _unitOfWork.CommitAsync();
            }
            return subTaskAttachmentObj;
        }

        public List<SubTaskAttachment> DeleteAttachmentBySubTaskId(long SubTaskId)
        {
            var subTaskAttachmentList = _unitOfWork.SubTaskAttachmentRepository.GetMany(t => t.SubTaskId == SubTaskId && t.IsDeleted == false).Result.ToList();
            if (subTaskAttachmentList != null && subTaskAttachmentList.Count() > 0)
            {
                foreach (var item in subTaskAttachmentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    _unitOfWork.SubTaskAttachmentRepository.UpdateAsync(item, item.Id);
                }
                _unitOfWork.CommitAsync();
            }
            return subTaskAttachmentList;
        }

    }

    public partial interface ISubTaskAttachmentService : IService<SubTaskAttachment>
    {
        SubTaskAttachment CheckInsertOrUpdate(SubTaskAttachmentDto model);
        List<SubTaskAttachment> GetAllBySubTaskId(long subTaskId);
        SubTaskAttachment GetSubTaskAttachmentById(long Id);
        SubTaskAttachment DeleteSubTaskAttachmentById(long Id);
        List<SubTaskAttachment> DeleteAttachmentBySubTaskId(long SubTaskId);
    }
}