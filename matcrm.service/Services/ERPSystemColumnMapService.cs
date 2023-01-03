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
    public partial class ERPSystemColumnMapService : Service<ERPSystemColumnMap>, IERPSystemColumnMapService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ERPSystemColumnMapService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ERPSystemColumnMap> CheckInsertOrUpdate(ERPSystemColumnMapDto model)
        {
            var eRPSystemColumnMapObj = _mapper.Map<ERPSystemColumnMap>(model);
            ERPSystemColumnMap? existingItem = null;
            if (model.Id != null)
            {
                existingItem = _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.Id == eRPSystemColumnMapObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.SourceColumnName == eRPSystemColumnMapObj.SourceColumnName && t.UserId == model.UserId && t.CustomModuleId == model.CustomModuleId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            // var existingItem = _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.Id == obj.Id && t.IsDeleted == false).Result.FirstOrDefault();

            if (existingItem == null)
            {
                return await InsertERPSystemColumnMap(eRPSystemColumnMapObj);
            }
            else
            {
                // existingItem.Name = obj.Name;
                existingItem.SourceColumnName = eRPSystemColumnMapObj.SourceColumnName;
                existingItem.DestinationColumnName = eRPSystemColumnMapObj.DestinationColumnName;
                return await UpdateERPSystemColumnMap(existingItem, existingItem.Id);
            }
        }

        public async Task<ERPSystemColumnMap> InsertERPSystemColumnMap(ERPSystemColumnMap eRPSystemColumnMapObj)
        {
            eRPSystemColumnMapObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.ERPSystemColumnMapRepository.AddAsync(eRPSystemColumnMapObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<ERPSystemColumnMap> UpdateERPSystemColumnMap(ERPSystemColumnMap existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.ERPSystemColumnMapRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<ERPSystemColumnMap> GetAll()
        {
            return _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }


        // public ERPSystemColumnMap GetERPSystemColumnMap (string Name) {
        //     return _unitOfWork.ERPSystemColumnMapRepository.GetMany (t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault ();
        // }

        public ERPSystemColumnMap GetById(long Id)
        {
            return _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public List<ERPSystemColumnMap> GetByUser(long UserId)
        {
            return _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.UserId == UserId && t.IsDeleted == false).Result.ToList();
        }

        public async Task<ERPSystemColumnMap> DeleteERPSystemColumnMap(long Id)
        {
            var eRPSystemColumnMapObj = _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (eRPSystemColumnMapObj != null)
            {
                eRPSystemColumnMapObj.IsDeleted = true;
                eRPSystemColumnMapObj.DeletedOn = DateTime.UtcNow;

                await _unitOfWork.ERPSystemColumnMapRepository.UpdateAsync(eRPSystemColumnMapObj, eRPSystemColumnMapObj.Id);
                await _unitOfWork.CommitAsync();
            }
            return eRPSystemColumnMapObj;
        }

        public List<ERPSystemColumnMap> GetByUserAndModule(int UserId, long ModuleId)
        {
            return _unitOfWork.ERPSystemColumnMapRepository.GetMany(t => t.UserId == UserId && t.CustomModuleId == ModuleId && t.IsDeleted == false).Result.ToList();
        }
    }

    public partial interface IERPSystemColumnMapService : IService<ERPSystemColumnMap>
    {
        Task<ERPSystemColumnMap> CheckInsertOrUpdate(ERPSystemColumnMapDto model);
        List<ERPSystemColumnMap> GetAll();
        // ERPSystemColumnMap GetERPSystemColumnMap (string Name);
        ERPSystemColumnMap GetById(long Id);
        List<ERPSystemColumnMap> GetByUserAndModule(int UserId, long ModuleId);
        List<ERPSystemColumnMap> GetByUser(long UserId);
        Task<ERPSystemColumnMap> DeleteERPSystemColumnMap(long Id);
    }
}