using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class LabelService : Service<Label>, ILabelService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LabelService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Label> CheckInsertOrUpdate (LabelDto model) {
            var labelObj = _mapper.Map<Label> (model);
            // var existingItem = _unitOfWork.LabelRepository.GetMany (t => t.Id == obj.Id && t.LabelCategoryId == obj.LabelCategoryId && t.TenantId == obj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
             var existingItem = _unitOfWork.LabelRepository.GetMany (t => t.Id == labelObj.Id && t.TenantId == labelObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertLabel (labelObj);
            } else {
                labelObj.CreatedOn = existingItem.CreatedOn;
                labelObj.CreatedBy = existingItem.CreatedBy;
                labelObj.TenantId = existingItem.TenantId;
                labelObj.Id = existingItem.Id;
                // obj.LabelCategoryId = existingItem.LabelCategoryId;
                return await UpdateLabel (labelObj, existingItem.Id);
            }
        }

        public async Task<Label> UpdateLabel (Label updatedItem, long existingId) {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.LabelRepository.UpdateAsync (updatedItem, existingId);
            await _unitOfWork.CommitAsync ();

            return update;
        }

        public async Task<Label> InsertLabel (Label labelObj) {
            
            labelObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.LabelRepository.AddAsync (labelObj);
            await _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<Label> GetAll () {
            return _unitOfWork.LabelRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public Label GetById (long Id) {
            return _unitOfWork.LabelRepository.GetMany (t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault ();
        }

        public List<Label> GetByTenant (int tenantId) {
            return _unitOfWork.LabelRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<Label> GetAllDefault () {
            return _unitOfWork.LabelRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public List<Label> GetAllByCategory (long CategoryId) {
            return _unitOfWork.LabelRepository.GetMany (t => t.LabelCategoryId == CategoryId && t.IsDeleted == false).Result.ToList ();
        }

        public List<Label> GetAllByCategoryAndTenant (long CategoryId, int TenantId) {
            return _unitOfWork.LabelRepository.GetMany (t => t.LabelCategoryId == CategoryId && t.TenantId == TenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<Label> GetByUser (int userId) {
            return _unitOfWork.LabelRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public Label CheckExistOrNot(LabelDto requestmodel){
            return _unitOfWork.LabelRepository.GetMany(t => t.Id == requestmodel.Id && t.TenantId == requestmodel.TenantId && t.LabelCategoryId == requestmodel.LabelCategoryId && t.IsDeleted == false).Result.FirstOrDefault();
            
        }

        public List<Label> GetByUserAndDefault (int userId, long categoryId) {
            return _unitOfWork.LabelRepository.GetMany (t => t.LabelCategoryId == categoryId && (t.CreatedBy == userId || t.CreatedBy == null) && t.IsDeleted == false).Result.ToList ();
        }

        public Label DeleteLabel (LabelDto model) {
            var labelObj = _mapper.Map<Label> (model);
            var existingItem = _unitOfWork.LabelRepository.GetMany (t => t.Id == labelObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.LabelRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
                return null;
            }
        }
    }

    public partial interface ILabelService : IService<Label> {
        Task<Label> CheckInsertOrUpdate (LabelDto model);
        List<Label> GetAll ();
        List<Label> GetByTenant (int tenantId);
        Label DeleteLabel (LabelDto model);
        Label GetById (long Id);
        List<Label> GetByUser (int userId);
        List<Label> GetAllDefault ();
        List<Label> GetByUserAndDefault (int userId, long categoryId);
        List<Label> GetAllByCategory (long CategoryId);
        List<Label> GetAllByCategoryAndTenant (long CategoryId, int TenantId);
        Label CheckExistOrNot(LabelDto requestmodel);
    }
}