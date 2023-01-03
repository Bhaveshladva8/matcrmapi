using System.Collections.Generic;
using System.Linq;
using matcrm.data.Models.Tables;
using matcrm.data;
using AutoMapper;

namespace matcrm.service.Services
{
    public partial class CountryService : Service<Country>, ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public CountryService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<Country> GetAll()
        {
            return _unitOfWork.CountryRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
    }

    public partial interface ICountryService : IService<Country>
    {
        List<Country> GetAll();
    }
}