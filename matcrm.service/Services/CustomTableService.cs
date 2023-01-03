using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomTableService : Service<CustomTable>, ICustomTableService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomTableService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public CustomTable CheckInsertOrUpdate (CustomTable customTableObj) {
            // var obj = _mapper.Map<CustomTable> (model);
            var existingItem = _unitOfWork.CustomTableRepository.GetMany (t => t.Name == customTableObj.Name && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return InsertCustomTable (customTableObj);
            } else {
                existingItem.Name = customTableObj.Name;
                return UpdateCustomTable (existingItem, existingItem.Id);
            }
        }

        public CustomTable InsertCustomTable (CustomTable customTableObj) {
            customTableObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.CustomTableRepository.Add (customTableObj);
            _unitOfWork.CommitAsync ();

            return newItem;
        }
        public CustomTable UpdateCustomTable (CustomTable existingItem, long existingId) {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.CustomTableRepository.UpdateAsync (existingItem, existingId);
            _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<CustomTable> GetAll () {
            return _unitOfWork.CustomTableRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomTable GetById (long Id) {
            return _unitOfWork.CustomTableRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public CustomTable GetByName (string Name) {
            return _unitOfWork.CustomTableRepository.GetMany (t => t.Name.ToLower() == Name.ToLower() && t.IsDeleted == false).Result.FirstOrDefault ();
        }

    }

    public partial interface ICustomTableService : IService<CustomTable> {
        CustomTable CheckInsertOrUpdate (CustomTable model);
        List<CustomTable> GetAll ();
        CustomTable GetById (long Id);
        CustomTable GetByName (string Name);
    }
}