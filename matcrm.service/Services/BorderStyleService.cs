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
    public partial class BorderStyleService : Service<BorderStyle>, IBorderStyleService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public BorderStyleService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<BorderStyle> CheckInsertOrUpdate(BorderStyleDto model)
        {
            var borderStyleObj = _mapper.Map<BorderStyle>(model);
            var existingItem = _unitOfWork.BorderStyleRepository.GetMany(t => t.Id == borderStyleObj.Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertBorderStyle(borderStyleObj);
            }
            else
            {
                borderStyleObj.CreatedOn = existingItem.CreatedOn;
                borderStyleObj.CreatedBy = existingItem.CreatedBy;
                borderStyleObj.Id = existingItem.Id;
                return await UpdateBorderStyle(borderStyleObj, existingItem.Id);
            }
        }

        public async Task<BorderStyle> UpdateBorderStyle(BorderStyle updatedItem, long existingId)
        {
            updatedItem.UpdatedOn = DateTime.UtcNow;
            var update = await _unitOfWork.BorderStyleRepository.UpdateAsync(updatedItem, existingId);
            await _unitOfWork.CommitAsync();

            return update;
        }

        public async Task<BorderStyle> InsertBorderStyle(BorderStyle borderStyleObj)
        {
            borderStyleObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.BorderStyleRepository.AddAsync(borderStyleObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public List<BorderStyle> GetAll()
        {
            return _unitOfWork.BorderStyleRepository.GetMany(t => t.DeletedOn == null).Result.OrderByDescending(t => t.Id).ToList();
        }

        public BorderStyle GetById(long Id)
        {
            return _unitOfWork.BorderStyleRepository.GetMany(t => t.DeletedOn == null && t.Id == Id).Result.FirstOrDefault();
        }

        public BorderStyle DeleteBorderStyle(long Id)
        {
            var existingItem = _unitOfWork.BorderStyleRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
            if (existingItem != null)
            {
                existingItem.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.BorderStyleRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                return null;
            }
        }
    }

    public partial interface IBorderStyleService : IService<BorderStyle>
    {
        Task<BorderStyle> CheckInsertOrUpdate(BorderStyleDto model);
        List<BorderStyle> GetAll();
        BorderStyle DeleteBorderStyle(long Id);
        BorderStyle GetById(long Id);
    }
}