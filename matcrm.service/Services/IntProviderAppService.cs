using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class IntProviderAppService : Service<IntProviderApp>, IIntProviderAppService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public IntProviderAppService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public IntProviderApp CheckInsertOrUpdate(IntProviderAppDto model)
        {
            var intProviderAppObj = _mapper.Map<IntProviderApp>(model);
            var existingItem = _unitOfWork.IntProviderAppRepository.GetMany(t => t.Id == intProviderAppObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return InsertIntProviderApp(intProviderAppObj);
            }
            else
            {
                existingItem.Name = intProviderAppObj.Name;
                return UpdateIntProviderApp(existingItem, existingItem.Id);
            }
        }

        public IntProviderApp InsertIntProviderApp(IntProviderApp intProviderAppObj)
        {
            intProviderAppObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.IntProviderAppRepository.Add(intProviderAppObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public IntProviderApp UpdateIntProviderApp(IntProviderApp existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            _unitOfWork.IntProviderAppRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<IntProviderApp> GetAll()
        {
            return _unitOfWork.IntProviderAppRepository.GetMany(t => t.IsDeleted == false).Result.Include(t => t.IntProvider).ToList();
        }


        public IntProviderApp GetIntProviderApp(string Name)
        {
            return _unitOfWork.IntProviderAppRepository.GetMany(t => t.Name.ToLower() == Name.ToLower() && t.IsDeleted == false).Result.Include(t => t.IntProvider).FirstOrDefault();
        }

        public IntProviderApp GetIntProviderAppById(long Id)
        {
            return _unitOfWork.IntProviderAppRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public IntProviderApp GetIntProviderAppByProviderId(long ProviderId, string Name)
        {
            return _unitOfWork.IntProviderAppRepository.GetMany(t => t.IntProviderId == ProviderId && t.Name == Name && t.IsDeleted == false).Result.Include(t => t.IntProvider).FirstOrDefault();
        }
        public List<IntProviderApp> GetByProviderId(long ProviderId)
        {
            return _unitOfWork.IntProviderAppRepository.GetMany(t => t.IntProviderId == ProviderId && t.IsDeleted == false).Result.ToList();
        }

        public IntProviderApp DeleteIntProviderApp(long Id)
        {
            var intProviderAppObj = _unitOfWork.IntProviderAppRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (intProviderAppObj != null)
            {
                intProviderAppObj.IsDeleted = true;
                intProviderAppObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.IntProviderAppRepository.UpdateAsync(intProviderAppObj, intProviderAppObj.Id);
                _unitOfWork.CommitAsync();
            }
            return intProviderAppObj;
        }
    }

    public partial interface IIntProviderAppService : IService<IntProviderApp>
    {
        IntProviderApp CheckInsertOrUpdate(IntProviderAppDto model);
        List<IntProviderApp> GetAll();
        IntProviderApp GetIntProviderApp(string Name);
        IntProviderApp GetIntProviderAppById(long Id);
        IntProviderApp DeleteIntProviderApp(long Id);
        IntProviderApp GetIntProviderAppByProviderId(long ProviderId, string Name);
        List<IntProviderApp> GetByProviderId(long ProviderId);
    }
}