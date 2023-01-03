using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappModuleService : Service<OneClappModule>, IOneClappModuleService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappModuleService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OneClappModule> CheckInsertOrUpdate(OneClappModuleDto model)
        {
            var oneClappModuleObj = _mapper.Map<OneClappModule>(model);
            var existingItem = _unitOfWork.OneClappModuleRepository.GetMany(t => t.Name == oneClappModuleObj.Name && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOneClappModule(oneClappModuleObj);
            }
            else
            {
                existingItem.Name = oneClappModuleObj.Name;
                return await UpdateOneClappModule(existingItem, existingItem.Id);
            }
        }

        public async Task<OneClappModule> InsertOneClappModule(OneClappModule oneClappModuleObj)
        {
            oneClappModuleObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OneClappModuleRepository.AddAsync(oneClappModuleObj);
           await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<OneClappModule> UpdateOneClappModule(OneClappModule existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
           await _unitOfWork.OneClappModuleRepository.UpdateAsync(existingItem, existingId);
           await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<OneClappModule> GetAll()
        {
            return _unitOfWork.OneClappModuleRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public OneClappModule GetById(long Id)
        {
            return _unitOfWork.OneClappModuleRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public OneClappModule GetByName(string Name)
        {
            return _unitOfWork.OneClappModuleRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public OneClappModule DeleteById(long Id)
        {
            var oneClappModuleObj = _unitOfWork.OneClappModuleRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (oneClappModuleObj != null)
            {
                oneClappModuleObj.IsDeleted = true;
                oneClappModuleObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.OneClappModuleRepository.UpdateAsync(oneClappModuleObj, oneClappModuleObj.Id);
                _unitOfWork.CommitAsync();
                return oneClappModuleObj;
            }
            return null;
        }
    }

    public partial interface IOneClappModuleService : IService<OneClappModule>
    {
        Task<OneClappModule> CheckInsertOrUpdate(OneClappModuleDto model);
        List<OneClappModule> GetAll();
        OneClappModule GetById(long Id);
        OneClappModule GetByName(string Name);
        OneClappModule DeleteById(long Id);
    }
}