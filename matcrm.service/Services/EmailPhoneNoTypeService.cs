using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services {
    public partial class EmailPhoneNoTypeService : Service<EmailPhoneNoType>, IEmailPhoneNoTypeService {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public EmailPhoneNoTypeService (IUnitOfWork unitOfWork, IMapper mapper) : base (unitOfWork) {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public List<EmailPhoneNoType> GetAll () {
            return _unitOfWork.EmailPhoneNoTypeRepository.GetMany (t => t.IsDeleted == false).Result.ToList ();
        }

        public EmailPhoneNoType GetById (long Id) {
            return _unitOfWork.EmailPhoneNoTypeRepository.GetMany (t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault ();
        }
    }

    public partial interface IEmailPhoneNoTypeService : IService<EmailPhoneNoType> {
        List<EmailPhoneNoType> GetAll ();
        EmailPhoneNoType GetById (long Id);
    }
}