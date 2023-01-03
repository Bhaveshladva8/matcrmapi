using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data;
using matcrm.data.Models.Tables;
using AutoMapper;

namespace matcrm.service.Services
{
    public partial class MateTicketActivityService : Service<MateTicketActivity>, IMateTicketActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public MateTicketActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<MateTicketActivity> CheckInsertOrUpdate(MateTicketActivity MateTicketActivityObj)
        {           
            return await InsertMateTicketActivity(MateTicketActivityObj);
        }
        public async Task<MateTicketActivity> InsertMateTicketActivity(MateTicketActivity MateTicketActivityObj)
        {
            MateTicketActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.MateTicketActivityRepository.AddAsync(MateTicketActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public List<MateTicketActivity> GetAllByTicketId(long TicketId)
        {
            return _unitOfWork.MateTicketActivityRepository.GetMany(t=>t.MateTicketId == TicketId).Result.ToList();
        }
    }
    public partial interface IMateTicketActivityService : IService<MateTicketActivity>
    {
        Task<MateTicketActivity> CheckInsertOrUpdate(MateTicketActivity model);
        List<MateTicketActivity> GetAllByTicketId(long TicketId);
    }
}