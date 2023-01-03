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

    public partial class LanguageService : Service<Language>, ILanguageService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public LanguageService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Language CheckInsertOrUpdate(LanguageDto model)
        {
            var languageObj = _mapper.Map<Language>(model);
            Language existingItem;
            if (languageObj.LanguageId != 0)
            {
                existingItem = _unitOfWork.LanguageRepository.GetMany(t => t.LanguageId == languageObj.LanguageId && t.IsDeleted == false).Result.FirstOrDefault();
            }
            else
            {
                existingItem = _unitOfWork.LanguageRepository.GetMany(t => t.LanguageCode == languageObj.LanguageCode && t.LanguageName == languageObj.LanguageName && t.IsDeleted == false).Result.FirstOrDefault();
            }

            if (existingItem == null)
            {
                return InsertLanguage(languageObj);
            }
            else
            {
                languageObj.CreatedOn = existingItem.CreatedOn;
                languageObj.LanguageId = existingItem.LanguageId;
                return UpdateLanguage(languageObj, existingItem.LanguageId);
            }
        }

        public Language InsertLanguage(Language languageObj)
        {
            languageObj.CreatedOn = DateTime.UtcNow;

            var newItem = _unitOfWork.LanguageRepository.Add(languageObj);
            _unitOfWork.CommitAsync();

            return newItem;
        }
        public Language UpdateLanguage(Language existingItem, int existingId)
        {
            existingItem.UpdatedOn = DateTime.UtcNow;

            _unitOfWork.LanguageRepository.UpdateAsync(existingItem, existingId);
            _unitOfWork.CommitAsync();

            return existingItem;
        }

        public List<Language> GetAll()
        {
            var languages = _unitOfWork.LanguageRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
            List<Language> languageList = new List<Language>(languages);
            return languageList;
        }

        public List<Language> GetAllActive()
        {
            return _unitOfWork.LanguageRepository.GetMany(t => t.IsDeleted == false && t.IsActive == true).Result.ToList();
        }

        public Language GetLanguage(string languageName)
        {
            return _unitOfWork.LanguageRepository.GetMany(t => t.LanguageName == languageName && t.IsDeleted == false && t.IsActive == true).Result.FirstOrDefault();
        }

        public Language GetLanguageById(int languageId)
        {
            return _unitOfWork.LanguageRepository.GetMany(t => t.LanguageId == languageId && t.IsDeleted == false && t.IsActive == true).Result.FirstOrDefault();
        }

        public Language DeleteLanguage(int languageId)
        {
            var languageObj = GetMany(t => t.LanguageId == languageId).Result.FirstOrDefault();
            if (languageObj != null)
            {
                languageObj.IsDeleted = true;
                languageObj.DeletedOn = DateTime.UtcNow;

                _unitOfWork.LanguageRepository.UpdateAsync(languageObj, languageObj.LanguageId);
                _unitOfWork.CommitAsync();
            }
            return languageObj;
        }

    }

    public partial interface ILanguageService : IService<Language>
    {
        Language CheckInsertOrUpdate(LanguageDto model);
        List<Language> GetAll();
        List<Language> GetAllActive();
        Language GetLanguage(string languageName);
        Language GetLanguageById(int languageId);
        Language DeleteLanguage(int languageId);
    }
}