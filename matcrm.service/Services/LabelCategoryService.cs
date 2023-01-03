using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class LabelCategoryService : Service<LabelCategory>, ILabelCategoryService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public LabelCategoryService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public LabelCategory CheckInsertOrUpdate (LabelCategory labelCategoryObj) {
            // var obj = _mapper.Map<LabelCategory> (model);
            var labelCategory = _unitOfWork.LabelCategoryRepository.GetMany (t => t.Name == labelCategoryObj.Name && t.IsDeleted == false).Result.FirstOrDefault ();
            if (labelCategory == null) {
                return InsertLabelCategory (labelCategoryObj);
            } else {
                labelCategory.Name = labelCategoryObj.Name;
                return UpdateLabelCategory (labelCategory, labelCategory.Id);
            }
        }

        public LabelCategory InsertLabelCategory (LabelCategory labelCategoryObj) {
            labelCategoryObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.LabelCategoryRepository.Add (labelCategoryObj);
            _unitOfWork.CommitAsync ();

            return newItem;
        }
        public LabelCategory UpdateLabelCategory (LabelCategory labelCategory, long existingId) {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.LabelCategoryRepository.UpdateAsync (labelCategory, existingId);
            _unitOfWork.CommitAsync ();

            return labelCategory;
        }

        public List<LabelCategory> GetAll () {
            return _unitOfWork.LabelCategoryRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public LabelCategory GetById (long Id) {
            return _unitOfWork.LabelCategoryRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public LabelCategory GetByName (string Name) {
            return _unitOfWork.LabelCategoryRepository.GetMany (t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault ();
        }

    }

    public partial interface ILabelCategoryService : IService<LabelCategory> {
        LabelCategory CheckInsertOrUpdate (LabelCategory model);
        List<LabelCategory> GetAll ();
        LabelCategory GetById (long Id);
        LabelCategory GetByName (string Name);
    }
}