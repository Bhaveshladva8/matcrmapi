using System.Collections.Generic;
using System.Linq;
using matcrm.data.Models.Tables;
using matcrm.data;
using AutoMapper;

namespace matcrm.service.Services
{
    public partial class StateService : Service<State>, IStateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public StateService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<State> GetAllByCountryId(int countryId)
        {
            return _unitOfWork.StateRepository.GetMany(t => t.CountryId == countryId && t.DeletedOn == null).Result.ToList();
        }
    }
    public partial interface IStateService : IService<State>
    {
        List<State> GetAllByCountryId(int countryId);
    }
}