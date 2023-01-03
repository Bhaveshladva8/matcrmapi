using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data;
using matcrm.data.Helpers;
using matcrm.data.Models.Tables;

namespace matcrm.service.Services
{
    public partial class VerificationCodeService : Service<VerificationCode>, IVerificationCodeService
    {

        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public VerificationCodeService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<VerificationCode> CheckInsertOrUpdate(VerificationCode verificationCodeObj)
        {
            var existingItem = _unitOfWork.VerificationCodeRepository.GetMany(t => t.Email.ToLower() == verificationCodeObj.Email.ToLower() && t.VerificationFor.ToLower() == verificationCodeObj.VerificationFor.ToLower() && t.IsUsed == false && t.UserId == verificationCodeObj.UserId && t.IsDeleted == false).Result.FirstOrDefault();

            if (existingItem != null)
            {
                return await UpdateVerificationCode(existingItem, verificationCodeObj);
            }
            else
            {
                return await InsertVerificationCode(verificationCodeObj);
            }
        }

        public async Task<VerificationCode> InsertVerificationCode(VerificationCode verificationCodeObj)
        {
            verificationCodeObj.VerificationCodeId = 0;
            var newItem = await _unitOfWork.VerificationCodeRepository.AddAsync(verificationCodeObj);
            await _unitOfWork.CommitAsync();
            return newItem;
        }

        public async Task<VerificationCode> UpdateVerificationCode(VerificationCode existingItem, VerificationCode verificationCode)
        {
            existingItem.VerificationFor = verificationCode.VerificationFor;
            existingItem.Code = verificationCode.Code;
            existingItem.IsUsed = verificationCode.IsUsed;
            existingItem.IsExpired = verificationCode.IsExpired;
            existingItem.ExpiredOn = verificationCode.ExpiredOn;
            existingItem.UpdatedOn = DataUtility.GetCurrentDateTime();

            var newItem = await _unitOfWork.VerificationCodeRepository.UpdateAsync(existingItem, existingItem.VerificationCodeId);
            await _unitOfWork.CommitAsync();

            return existingItem;
        }

        public void UpdateVerificationCode(VerificationCode verificationCode)
        {
            var newItem = _unitOfWork.VerificationCodeRepository.UpdateAsync(verificationCode, verificationCode.VerificationCodeId);
            _unitOfWork.CommitAsync();
        }

        public VerificationCode GetVerificationCodeDetail(string code, string email)
        {
            return _unitOfWork.VerificationCodeRepository.GetMany(t => t.Code.ToLower() == code.ToLower() && t.IsUsed == false && t.Email.ToLower() == email.ToLower() && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public VerificationCode GetVerificationCodeDetailByUserId(string code, long userId)
        {
            return _unitOfWork.VerificationCodeRepository.GetMany(t => t.Code.ToLower() == code.ToLower() && t.IsUsed == false && t.UserId == userId && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public VerificationCode GetVerificationCodeByHash(string codeHash, long userId)
        {
            return _unitOfWork.VerificationCodeRepository.GetMany(t => DataUtility.ShaHash.GetHash(t.Code) == codeHash && t.IsUsed == false && t.UserId == userId && t.IsDeleted == false).Result.FirstOrDefault();
        }
    }

    public partial interface IVerificationCodeService : IService<VerificationCode>
    {
        Task<VerificationCode> CheckInsertOrUpdate(VerificationCode verificationCode);

        void UpdateVerificationCode(VerificationCode verificationCode);

        VerificationCode GetVerificationCodeDetail(string code, string email);

        VerificationCode GetVerificationCodeDetailByUserId(string code, long userId);

        VerificationCode GetVerificationCodeByHash(string codeHash, long userId);
    }
}