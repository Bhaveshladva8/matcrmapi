using System.Collections.Generic;
using System.Linq;
using matcrm.data.Models.Tables;
using matcrm.data;
using AutoMapper;

namespace matcrm.service.Services
{
    public partial class StandardTimeZoneService : Service<StandardTimeZone>, IStandardTimeZoneService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        public StandardTimeZoneService(IUnitOfWork unitOfWork,
        IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public List<StandardTimeZone> GetAll()
        {
            return _unitOfWork.StandardTimeZoneRepository.GetMany(t => t.DeletedOn == null).Result.ToList();
        }
    }

    public partial interface IStandardTimeZoneService : IService<StandardTimeZone>
    {
        List<StandardTimeZone> GetAll();
    }
}