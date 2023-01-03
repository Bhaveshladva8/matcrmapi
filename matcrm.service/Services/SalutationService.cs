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
    public partial class SalutationService : Service<Salutation>, ISalutationService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SalutationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Salutation> CheckInsertOrUpdate(SalutationDto model)
        {
            var salutationObj = _mapper.Map<Salutation>(model);
            var existingItem = _unitOfWork.SalutationRepository.GetMany(t => t.Name == salutationObj.Name && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertSalutation(salutationObj);
            }
            else
            {
                salutationObj.CreatedOn = existingItem.CreatedOn;
                salutationObj.CreatedBy = existingItem.CreatedBy;
                salutationObj.Id = existingItem.Id;
                return await UpdateSalutation(salutationObj, existingItem.Id);
            }
        }

        public async Task<Salutation> UpdateSalutation(Salutation updatedItem, long existingId)
        {
            var update = await _unitOfWork.SalutationRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<Salutation> InsertSalutation(Salutation salutationObj)
        {
            salutationObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.SalutationRepository.AddAsync(salutationObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<Salutation> GetAll()
        {
            return _unitOfWork.SalutationRepository.GetMany(t => t.DeletedOn == null).Result.OrderByDescending(t => t.Id).ToList();
        }

        public Salutation GetById(long Id)
        {
            return _unitOfWork.SalutationRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public Salutation GetByName(string Name)
        {
            return _unitOfWork.SalutationRepository.GetMany(t => t.DeletedOn == null && t.Name == Name).Result.FirstOrDefault();
        }

        public Salutation DeleteSalutation(long Id)
        {
            var salutationObj = _unitOfWork.SalutationRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (salutationObj != null)
            {
                salutationObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.SalutationRepository.UpdateAsync(salutationObj, salutationObj.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface ISalutationService : IService<Salutation>
    {
        Task<Salutation> CheckInsertOrUpdate(SalutationDto model);
        List<Salutation> GetAll();
        Salutation DeleteSalutation(long Id);
        Salutation GetById(long Id);
        Salutation GetByName(string Name);
    }
}