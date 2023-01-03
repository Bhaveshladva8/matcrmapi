using System.Collections.Generic;
using System.Linq;
using matcrm.data.Models.Tables;
using matcrm.data;
using AutoMapper;

namespace matcrm.service.Services
{
    public partial class CityService : Service<City>, ICityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CityService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public List<City> GetAllByStateId(int stateId)
        {
            return _unitOfWork.CityRepository.GetMany(t => t.StateId == stateId && t.DeletedOn == null).Result.ToList();
        }
    }

    public partial interface ICityService : IService<City>
    {
        List<City> GetAllByStateId(int stateId);
    }
}