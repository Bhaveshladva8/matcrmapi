using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class IntProviderService : Service<IntProvider>, IIntProviderService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public IntProviderService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public IntProvider CheckInsertOrUpdate(IntProviderDto model)
        {
            var intProviderObj = _mapper.Map<IntProvider>(model);
            // var existingItem = _unitOfWork.IntProviderRepository.GetMany (t => t.Id == obj.Id && t.IsDeleted == false).Result.FirstOrDefault ();
            var existingItem = _unitOfWork.IntProviderRepository.GetMany(t => t.Name.ToLower() == intProviderObj.Name.ToLower() && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertIntProvider(intProviderObj);
            }
            else
            {
                existingItem.Name = intProviderObj.Name;
                return UpdateIntProvider(existingItem, existingItem.Id);
            }
        }

        public IntProvider InsertIntProvider(IntProvider intProviderObj)
        {
            intProviderObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.IntProviderRepository.Add(intProviderObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public IntProvider UpdateIntProvider(IntProvider existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.IntProviderRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<IntProvider> GetAll()
        {
            return _unitOfWork.IntProviderRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }


        public IntProvider GetIntProvider(string Name)
        {
            return _unitOfWork.IntProviderRepository.GetMany(t => t.Name.ToLower() == Name.ToLower() && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public IntProvider GetIntProviderById(long Id)
        {
            return _unitOfWork.IntProviderRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public IntProvider DeleteIntProvider(long Id)
        {
            var intProviderObj = _unitOfWork.IntProviderRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (intProviderObj != null)
            {
                intProviderObj.IsDeleted = true;
                intProviderObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.IntProviderRepository.UpdateAsync(intProviderObj, intProviderObj.Id);
                _unitOfWork.CommitAsync();
            }
            return intProviderObj;
        }
    }

    public partial interface IIntProviderService : IService<IntProvider>
    {
        IntProvider CheckInsertOrUpdate(IntProviderDto model);
        List<IntProvider> GetAll();
        IntProvider GetIntProvider(string Name);
        IntProvider GetIntProviderById(long Id);
        IntProvider DeleteIntProvider(long Id);
    }
}