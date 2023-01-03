using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class ActivityAvailabilityService : Service<ActivityAvailability>, IActivityAvailabilityService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ActivityAvailabilityService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<ActivityAvailability> GetAll () {
            return _unitOfWork.ActivityAvailabilityRepository.GetAll().ToList();
        }

        public ActivityAvailability GetByName (string Name) {
            return _unitOfWork.ActivityAvailabilityRepository.GetMany (t => t.Name == Name).Result.FirstOrDefault ();
        }

        public ActivityAvailability GetById (long Id) {
            return _unitOfWork.ActivityAvailabilityRepository.GetMany (t => t.Id == Id).Result.FirstOrDefault ();
        }

    }

    public partial interface IActivityAvailabilityService : IService<ActivityAvailability> {
        List<ActivityAvailability> GetAll ();
        ActivityAvailability GetByName (string Name);
        ActivityAvailability GetById (long Id);
    }
}