using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class OneClappFormStatusService : Service<OneClappFormStatus>, IOneClappFormStatusService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OneClappFormStatusService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappFormStatus> CheckInsertOrUpdate (OneClappFormStatusDto model) {
            var oneClappFormStatusObj = _mapper.Map<OneClappFormStatus> (model);
            var existingItem = _unitOfWork.OneClappFormStatusRepository.GetMany (t => t.Id == oneClappFormStatusObj.Id && t.TenantId == oneClappFormStatusObj.TenantId && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertOneClappFormStatus (oneClappFormStatusObj);
            } else {
                return await UpdateOneClappFormStatus (oneClappFormStatusObj, existingItem.Id);
            }
        }

        public async Task<OneClappFormStatus> UpdateOneClappFormStatus (OneClappFormStatus updatedItem, long existingId) {
            // updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.OneClappFormStatusRepository.UpdateAsync (updatedItem, existingId);
           await _unitOfWork.CommitAsync ();

            return update;
        }

        public async Task<OneClappFormStatus> InsertOneClappFormStatus (OneClappFormStatus oneClappFormStatusObj) {
            oneClappFormStatusObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappFormStatusRepository.AddAsync (oneClappFormStatusObj);
           await _unitOfWork.CommitAsync ();
            return newItem;
        }

        public List<OneClappFormStatus> GetAll () {
            return _unitOfWork.OneClappFormStatusRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public OneClappFormStatus GetOneClappFormStatusById (int statusId) {
            return _unitOfWork.OneClappFormStatusRepository.GetMany (t => t.IsDeleted == false && t.Id == statusId).Result.FirstOrDefault ();
        }

         public OneClappFormStatus GetByName (string Name) {
            return _unitOfWork.OneClappFormStatusRepository.GetMany (t => t.IsDeleted == false && t.Name == Name).Result.FirstOrDefault ();
        }

        public List<OneClappFormStatus> GetStatusByTenant (int tenantId) {
            return _unitOfWork.OneClappFormStatusRepository.GetMany (t => t.TenantId == tenantId && t.IsDeleted == false).Result.ToList ();
        }

        public List<OneClappFormStatus> GetStatusByUser (int userId) {
            return _unitOfWork.OneClappFormStatusRepository.GetMany (t => t.CreatedBy == userId && t.IsDeleted == false).Result.ToList ();
        }

        public OneClappFormStatus DeleteOneClappFormStatus (OneClappFormStatusDto model) {
            var oneClappFormStatusObj = _mapper.Map<OneClappFormStatus> (model);
            var existingItem = _unitOfWork.OneClappFormStatusRepository.GetMany (t => t.Id == oneClappFormStatusObj.Id).Result.FirstOrDefault ();
            if (existingItem != null) {
                existingItem.IsDeleted = true;
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.OneClappFormStatusRepository.UpdateAsync (existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync ();
                return newItem;
            } else {
               return null;
            }
        }
    }

    public partial interface IOneClappFormStatusService : IService<OneClappFormStatus> {
        Task<OneClappFormStatus> CheckInsertOrUpdate (OneClappFormStatusDto model);
        List<OneClappFormStatus> GetAll ();
        List<OneClappFormStatus> GetStatusByTenant (int tenantId);
        OneClappFormStatus DeleteOneClappFormStatus (OneClappFormStatusDto model);
        OneClappFormStatus GetOneClappFormStatusById (int statusId);
        List<OneClappFormStatus> GetStatusByUser (int userId);
        OneClappFormStatus GetByName (string Name);
    }
}