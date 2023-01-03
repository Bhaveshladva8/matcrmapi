using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class TagService : Service<Tag>, ITagService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public TagService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Tag CheckInsertOrUpdate (TagDto model) {
            var tagObj = _mapper.Map<Tag> (model);
            var existingItem = _unitOfWork.TagRepository.GetMany (t => t.Id == tagObj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertTag (tagObj);
            } else {
                return UpdateTag (tagObj, existingItem.Id);
            }
        }

        public Tag UpdateTag (Tag updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = _unitOfWork.TagRepository.UpdateAsync (updatedItem, existingId).Result;
            _unitOfWork.CommitAsync ();

            return update;
        }

        public Tag InsertTag (Tag tagObj) {
            tagObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.TagRepository.Add (tagObj);
            _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<Tag> GetAll () {
            return _unitOfWork.TagRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public Tag GetTagById (long Id) {
            return _unitOfWork.TagRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<Tag> GetByName (string Name) {
            return _unitOfWork.TagRepository.GetMany (t => t.Name == Name && t.IsDeleted == false).Result.ToList ();
        }

        public Tag DeleteTag (TagDto model) {
            var tagObj = _mapper.Map<Tag> (model);
            var existingItem = _unitOfWork.TagRepository.GetMany (t => t.Id == tagObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.TagRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }
    }

    public partial interface ITagService : IService<Tag> {
        Tag CheckInsertOrUpdate (TagDto model);
        List<Tag> GetAll ();
        Tag DeleteTag (TagDto model);
        Tag GetTagById (long Id);
        List<Tag> GetByName (string Name);
    }
}