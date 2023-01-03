using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappFormActionService : Service<OneClappFormAction>, IOneClappFormActionService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappFormActionService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
       
        public List<OneClappFormAction> GetAll()
        {
            return _unitOfWork.OneClappFormActionRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }
        public OneClappFormAction GetById(long Id)
        {
            return _unitOfWork.OneClappFormActionRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        public OneClappFormAction GetByName(string Name)
        {
            return _unitOfWork.OneClappFormActionRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }
        
    }
    public partial interface IOneClappFormActionService : IService<OneClappFormAction>
    {
        List<OneClappFormAction> GetAll();
        OneClappFormAction GetById(long Id);
        OneClappFormAction GetByName(string Name);
    }
}