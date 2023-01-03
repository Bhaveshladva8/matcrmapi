using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class SectionActivityService : Service<SectionActivity>, ISectionActivityService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public SectionActivityService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SectionActivity> CheckInsertOrUpdate(SectionActivity sectionActivityObj)
        {
            // var existingItem = _unitOfWork.SectionActivityRepository.GetMany (t => t.UserId == obj.UserId && t.TaskId == obj.TaskId).Result.FirstOrDefault ();
            // if (existingItem == null) {
            //     return InsertSectionActivity (obj);
            // } else {
            //     return existingItem;
            //     // return UpdateSectionActivity (existingItem, existingItem.Id);
            // }
            return await InsertSectionActivity(sectionActivityObj);
        }

        public async Task<SectionActivity> InsertSectionActivity(SectionActivity sectionActivityObj)
        {
            sectionActivityObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.SectionActivityRepository.AddAsync(sectionActivityObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<SectionActivity> UpdateSectionActivity(SectionActivity existingItem, int existingId)
        {
            await _unitOfWork.SectionActivityRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<SectionActivity> GetAllBySectionId(long sectionId)
        {
            return _unitOfWork.SectionActivityRepository.GetMany(t => t.SectionId == sectionId).Result.ToList();
        }

        public SectionActivity GetSectionActivityById(long Id)
        {
            return _unitOfWork.SectionActivityRepository.GetMany(t => t.Id == Id).Result.FirstOrDefault();
        }

        public async Task<List<SectionActivity>> DeleteActivityBySectionId(long sectionId)
        {
            var sectionActivityList = _unitOfWork.SectionActivityRepository.GetMany(t => t.SectionId == sectionId).Result.ToList();
            if (sectionActivityList != null && sectionActivityList.Count() > 0)
            {
                foreach (var item in sectionActivityList)
                {
                    await _unitOfWork.SectionActivityRepository.DeleteAsync(item);
                }
                await _unitOfWork.CommitAsync();
            }
            return sectionActivityList;
        }
    }

    public partial interface ISectionActivityService : IService<SectionActivity>
    {
        Task<SectionActivity> CheckInsertOrUpdate(SectionActivity model);
        List<SectionActivity> GetAllBySectionId(long sectionId);
        SectionActivity GetSectionActivityById(long Id);
        Task<List<SectionActivity>> DeleteActivityBySectionId(long sectionId);

    }
}