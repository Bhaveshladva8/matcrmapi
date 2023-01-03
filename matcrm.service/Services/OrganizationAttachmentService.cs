using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class OrganizationAttachmentService : Service<OrganizationAttachment>, IOrganizationAttachmentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationAttachmentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<OrganizationAttachment> CheckInsertOrUpdate(OrganizationAttachmentDto model)
        {
            var organizationAttachmentObj = _mapper.Map<OrganizationAttachment>(model);
            var existingItem = _unitOfWork.OrganizationAttachmentRepository.GetMany(t => t.FileName == organizationAttachmentObj.FileName && t.OrganizationId == organizationAttachmentObj.OrganizationId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOrganizationAttachment(organizationAttachmentObj);
            }
            else
            {
                return existingItem;
                // return UpdateOrganizationAttachment (existingItem, existingItem.Id);
            }
        }

        public async Task<OrganizationAttachment> InsertOrganizationAttachment(OrganizationAttachment organizationAttachmentObj)
        {
            organizationAttachmentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OrganizationAttachmentRepository.AddAsync(organizationAttachmentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<OrganizationAttachment> UpdateOrganizationAttachment(OrganizationAttachment existingItem, long existingId)
        {
            await _unitOfWork.OrganizationAttachmentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<OrganizationAttachment> GetAllByOrganizationId(long OrganizationId)
        {
            return _unitOfWork.OrganizationAttachmentRepository.GetMany(t => t.OrganizationId == OrganizationId && t.IsDeleted == false).Result.ToList();
        }

        public OrganizationAttachment GetOrganizationAttachmentById(long Id)
        {
            return _unitOfWork.OrganizationAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public OrganizationAttachment DeleteOrganizationAttachmentById(long Id)
        {
            var organizationAttachmentObj = _unitOfWork.OrganizationAttachmentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (organizationAttachmentObj != null)
            {
                organizationAttachmentObj.IsDeleted = true;
                organizationAttachmentObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.OrganizationAttachmentRepository.UpdateAsync(organizationAttachmentObj, organizationAttachmentObj.Id);
                _unitOfWork.CommitAsync();
            }
            return organizationAttachmentObj;
        }


        public async Task<List<OrganizationAttachment>> DeleteAttachmentByOrganizationId(long OrganizationId)
        {
            var organizationAttachmentList = _unitOfWork.OrganizationAttachmentRepository.GetMany(t => t.OrganizationId == OrganizationId && t.IsDeleted == false).Result.ToList();
            if (organizationAttachmentList != null && organizationAttachmentList.Count() > 0)
            {
                foreach (var item in organizationAttachmentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.OrganizationAttachmentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return organizationAttachmentList;
        }
    }
    public partial interface IOrganizationAttachmentService : IService<OrganizationAttachment>
    {
        Task<OrganizationAttachment> CheckInsertOrUpdate(OrganizationAttachmentDto model);
        List<OrganizationAttachment> GetAllByOrganizationId(long OrganizationId);
        OrganizationAttachment GetOrganizationAttachmentById(long Id);
        OrganizationAttachment DeleteOrganizationAttachmentById(long Id);
        Task<List<OrganizationAttachment>> DeleteAttachmentByOrganizationId(long OrganizationId);
        Task<OrganizationAttachment> UpdateOrganizationAttachment(OrganizationAttachment existingItem, long existingId);
    }
}