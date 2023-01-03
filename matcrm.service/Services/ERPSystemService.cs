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
    public partial class ERPSystemService : Service<ERPSystem>, IERPSystemService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ERPSystemService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ERPSystem> CheckInsertOrUpdate(ERPSystemDto model)
        {
            var eRPSystemObj = _mapper.Map<ERPSystem>(model);
            var existingItem = _unitOfWork.ERPSystemRepository.GetMany(t => t.Id == eRPSystemObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertERPSystem(eRPSystemObj);
            }
            else
            {
                existingItem.Name = eRPSystemObj.Name;
                return await UpdateERPSystem(existingItem, existingItem.Id);
            }
        }

        public async Task<ERPSystem> InsertERPSystem(ERPSystem eRPSystemObj)
        {
            eRPSystemObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ERPSystemRepository.AddAsync(eRPSystemObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ERPSystem> UpdateERPSystem(ERPSystem existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ERPSystemRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<ERPSystem> GetAll()
        {
            return _unitOfWork.ERPSystemRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }


        public ERPSystem GetERPSystem(string Name)
        {
            return _unitOfWork.ERPSystemRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public ERPSystem GetERPSystemById(long Id)
        {
            return _unitOfWork.ERPSystemRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public ERPSystem DeleteERPSystem(long Id)
        {
            var eRPSystemObj = _unitOfWork.ERPSystemRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (eRPSystemObj != null)
            {
                eRPSystemObj.IsDeleted = true;
                eRPSystemObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.ERPSystemRepository.UpdateAsync(eRPSystemObj, eRPSystemObj.Id);
                _unitOfWork.CommitAsync();
            }
            return eRPSystemObj;
        }
    }

    public partial interface IERPSystemService : IService<ERPSystem>
    {
        Task<ERPSystem> CheckInsertOrUpdate(ERPSystemDto model);
        List<ERPSystem> GetAll();
        ERPSystem GetERPSystem(string Name);
        ERPSystem GetERPSystemById(long Id);
        ERPSystem DeleteERPSystem(long Id);
    }
}