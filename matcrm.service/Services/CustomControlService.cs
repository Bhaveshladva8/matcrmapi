using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class CustomControlService : Service<CustomControl>, ICustomControlService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CustomControlService (IUnitOfWork unitOfWork,
            IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomControl> CheckInsertOrUpdate (CustomControl customControlObj) {
            // var obj = _mapper.Map<CustomControl> (model);
            var existingItem = _unitOfWork.CustomControlRepository.GetMany (t => t.Name == customControlObj.Name && t.IsDeleted == false).Result.FirstOrDefault ();
            if (existingItem == null) {
                return await InsertCustomControl (customControlObj);
            } else {
                existingItem.Name = customControlObj.Name;
                return await UpdateCustomControl (existingItem, existingItem.Id);
            }
        }

        public async Task<CustomControl> InsertCustomControl (CustomControl customControlObj) {
            customControlObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.CustomControlRepository.AddAsync (customControlObj);
            await _unitOfWork.CommitAsync ();

            return newItem;
        }
        public async Task<CustomControl> UpdateCustomControl (CustomControl existingItem, long existingId) {
            // existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.CustomControlRepository.UpdateAsync (existingItem, existingId);
            await _unitOfWork.CommitAsync ();

            return existingItem;
        }

        public List<CustomControl> GetAllControl () {
            return _unitOfWork.CustomControlRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public CustomControl GetControl (long Id) {
            return _unitOfWork.CustomControlRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }
    }

    public partial interface ICustomControlService : IService<CustomControl> {
        Task<CustomControl> CheckInsertOrUpdate (CustomControl model);
        List<CustomControl> GetAllControl ();
        CustomControl GetControl (long Id);
    }
}