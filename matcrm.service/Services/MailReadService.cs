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
    public partial class MailReadService : Service<MailRead>, IMailReadService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public MailReadService(IUnitOfWork unitOfWork,
            IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<MailRead> CheckInsertOrUpdate(MailReadDto model)
        {
            var mailReadObj = _mapper.Map<MailRead>(model);
            var existingItem = _unitOfWork.MailReadRepository.GetMany(t => t.ThreadId == mailReadObj.ThreadId && t.ReadBy == mailReadObj.ReadBy && t.DeletedOn == null).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return await InsertMailRead(mailReadObj);
            }
            else
            {
                // existingItem.Comment = obj.Comment;
                return await UpdateMailRead(existingItem, existingItem.Id);
            }
        }

        public async Task<MailRead> InsertMailRead(MailRead mailReadObj)
        {
            mailReadObj.CreatedOn = DateTime.UtcNow;
            var newItem = _unitOfWork.MailReadRepository.Add(mailReadObj);
            await _unitOfWork.CommitAsync();

            return newItem;
        }
        public async Task<MailRead> UpdateMailRead(MailRead existingItem, long existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;
            await _unitOfWork.MailReadRepository.UpdateAsync(existingItem, existingId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<MailRead> GetAllMailRead(string ThreadId)
        {
            return _unitOfWork.MailReadRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.ToList();
        }



        public async Task<MailRead> Delete(long Id)
        {
            var mailReadObj = _unitOfWork.MailReadRepository.GetMany(t => t.Id == Id && t.DeletedOn == null).Result.FirstOrDefault();
            if (mailReadObj != null)
            {
                mailReadObj.DeletedOn = DateTime.UtcNow;
                var newItem = _unitOfWork.MailReadRepository.UpdateAsync(mailReadObj, mailReadObj.Id).Result;
                await _unitOfWork.CommitAsync();

                return newItem;
            }
            else
            {
                return null;
            }
        }

        public List<MailRead> DeleteByThread(string ThreadId)
        {
            var mailReadList = _unitOfWork.MailReadRepository.GetMany(t => t.ThreadId == ThreadId && t.DeletedOn == null).Result.ToList();
            if (mailReadList != null && mailReadList.Count() > 0)
            {
                foreach (var existingItem in mailReadList)
                {
                    existingItem.DeletedOn = DateTime.UtcNow;
                    var deletedItem = _unitOfWork.MailReadRepository.UpdateAsync(existingItem, existingItem.Id).Result;
                }
                _unitOfWork.CommitAsync();
            }
            return mailReadList;
        }
    }

    public partial interface IMailReadService : IService<MailRead>
    {
        Task<MailRead> CheckInsertOrUpdate(MailReadDto model);
        List<MailRead> GetAllMailRead(string ThreadId);
        Task<MailRead> Delete(long Id);
        List<MailRead> DeleteByThread(string ThreadId);
    }
}