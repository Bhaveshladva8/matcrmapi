using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OneClappFormTypeService : Service<OneClappFormType>, IOneClappFormTypeService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public OneClappFormTypeService(IMapper mapper, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
       
        public List<OneClappFormType> GetAll()
        {
            return _unitOfWork.OneClappFormTypeRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }
        public OneClappFormType GetOneClappFormTypeById(long Id)
        {
            return _unitOfWork.OneClappFormTypeRepository.GetMany(t => t.IsDeleted == false && t.Id == Id).Result.FirstOrDefault();
        }
        public OneClappFormType GetByName(string Name)
        {
            return _unitOfWork.OneClappFormTypeRepository.GetMany(t => t.Name == Name && t.IsDeleted == false).Result.FirstOrDefault();
        }
        
    }
    public partial interface IOneClappFormTypeService : IService<OneClappFormType>
    {
        List<OneClappFormType> GetAll();
        OneClappFormType GetOneClappFormTypeById(long Id);
        OneClappFormType GetByName(string Name);
    }
}