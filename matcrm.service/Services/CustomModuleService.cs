using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomModuleService : Service<CustomModule>, ICustomModuleService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomModuleService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomModule> CheckInsertOrUpdate (CustomModule customModuleObj) {
            // var obj = _mapper.Map<CustomModule> (model);
            var existingItem = _unitOfWork.CustomModuleRepository.GetMany (t => t.Name == customModuleObj.Name && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertCustomModule (customModuleObj);
            } else {
                existingItem.Name = customModuleObj.Name;
                return await UpdateCustomModule (existingItem, existingItem.Id);
            }
        }

        public async Task<CustomModule> InsertCustomModule (CustomModule customModuleObj) {
            customModuleObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomModuleRepository.AddAsync (customModuleObj);
           await _unitOfWork.CommitAsync ();

            return newItem;
        }
        public async Task<CustomModule> UpdateCustomModule (CustomModule existingItem, long existingId) {
            // existingItem.UpdatedOn = DateTime.UtcNow;
           await _unitOfWork.CustomModuleRepository.UpdateAsync (existingItem, existingId);
           await _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<CustomModule> GetAll () {
            return _unitOfWork.CustomModuleRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomModule GetById (long Id) {
            return _unitOfWork.CustomModuleRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }

        public CustomModule GetByName (string Name) {
            return _unitOfWork.CustomModuleRepository.GetMany (t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault ();
        }
         public CustomModule GetByCustomTable (long CustomTableId) {
            return _unitOfWork.CustomModuleRepository.GetMany (t => t.MasterTableId == CustomTableId && t.IsDeleted == false).Result.FirstOrDefault ();
        }
    }

    public partial interface ICustomModuleService : IService<CustomModule> {
        Task<CustomModule> CheckInsertOrUpdate (CustomModule model);
        List<CustomModule> GetAll ();
        CustomModule GetById (long Id);
        CustomModule GetByName (string Name);
        CustomModule GetByCustomTable (long CustomTableId);
    }
}