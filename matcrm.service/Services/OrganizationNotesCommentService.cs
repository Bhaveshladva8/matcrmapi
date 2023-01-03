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
    public partial class OrganizationNotesCommentService : Service<OrganizationNotesComment>, IOrganizationNotesCommentService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public OrganizationNotesCommentService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<OrganizationNotesComment> CheckInsertOrUpdate(OrganizationNotesCommentDto model)
        {
            var organizationNotesCommentObj = _mapper.Map<OrganizationNotesComment>(model);
            var existingItem = _unitOfWork.OrganizationNotesCommentRepository.GetMany(t => t.Id == organizationNotesCommentObj.Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertOrganizationNotesComment(organizationNotesCommentObj);
            }
            else
            {
                existingItem.Comment = model.Comment;
                return await UpdateOrganizationNotesComment(existingItem, existingItem.Id);
            }
        }

        public async Task<OrganizationNotesComment> InsertOrganizationNotesComment(OrganizationNotesComment organizationNotesCommentObj)
        {
            organizationNotesCommentObj.CreatedOn = DateTime.UtcNow;
            var newItem = await _unitOfWork.OrganizationNotesCommentRepository.AddAsync(organizationNotesCommentObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<OrganizationNotesComment> UpdateOrganizationNotesComment(OrganizationNotesComment existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.OrganizationNotesCommentRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<OrganizationNotesComment> GetAllByNoteId(long OrganizationNoteId)
        {
            return _unitOfWork.OrganizationNotesCommentRepository.GetMany(t => t.OrganizationNoteId == OrganizationNoteId && t.IsDeleted == false).Result.ToList();
        }

        public OrganizationNotesComment GetOrganizationNotesCommenttById(long Id)
        {
            return _unitOfWork.OrganizationNotesCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public OrganizationNotesComment DeleteOrganizationNotesComment(long Id)
        {
            var organizationNotesCommentObj = _unitOfWork.OrganizationNotesCommentRepository.GetMany(t => t.Id == Id && t.IsDeleted == false).Result.FirstOrDefault();
            if (organizationNotesCommentObj != null)
            {
                organizationNotesCommentObj.IsDeleted = true;
                organizationNotesCommentObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.OrganizationNotesCommentRepository.UpdateAsync(organizationNotesCommentObj, organizationNotesCommentObj.Id);
                _unitOfWork.CommitAsync();

                return organizationNotesCommentObj;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<OrganizationNotesComment>> DeleteCommentByNoteId(long OrganizationNoteId)
        {
            var organizationNotesCommentList = _unitOfWork.OrganizationNotesCommentRepository.GetMany(t => t.OrganizationNoteId == OrganizationNoteId && t.IsDeleted == false).Result.ToList();
            if (organizationNotesCommentList != null && organizationNotesCommentList.Count() > 0)
            {
                foreach (var item in organizationNotesCommentList)
                {
                    item.IsDeleted = true;
                    item.DeletedOn = DateTime.UtcNow;
                    await _unitOfWork.OrganizationNotesCommentRepository.UpdateAsync(item, item.Id);
                }
                await _unitOfWork.CommitAsync();
            }
            return organizationNotesCommentList;
        }
    }

    public partial interface IOrganizationNotesCommentService : IService<OrganizationNotesComment>
    {
        Task<OrganizationNotesComment> CheckInsertOrUpdate(OrganizationNotesCommentDto model);
        List<OrganizationNotesComment> GetAllByNoteId(long OrganizationNoteId);
        OrganizationNotesComment GetOrganizationNotesCommenttById(long Id);
        OrganizationNotesComment DeleteOrganizationNotesComment(long Id);
        Task<List<OrganizationNotesComment>> DeleteCommentByNoteId(long OrganizationNoteId);
    }
}