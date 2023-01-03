using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.ViewModels;
using matcrm.service.Utility;
using Newtonsoft.Json;

namespace matcrm.service.Services.ERP
{
    public class ContactService : IContactService
    {
        public async Task<ContactTokenVM> GetAccessToken(CommonContactTokenVM listVM, string Provider)
        {
            if (Provider.ToLower() == "microsoft")
            {
                var contactTokenObj = await MicroSoftManager<CommonContactTokenVM>.PostAcccessTokenAsync("token", listVM);
                if (!string.IsNullOrEmpty(contactTokenObj))
                {
                    ContactTokenVM contactTokenDtoObj = JsonConvert.DeserializeObject<ContactTokenVM>(contactTokenObj);

                    if (contactTokenDtoObj == null || contactTokenDtoObj == null) { return null; }

                    return contactTokenDtoObj;
                }
                return null;
            }
            if (Provider.ToLower() == "google")
            {
                var contactTokenObj = await GoogleManager<CommonContactTokenVM>.PostAcccessTokenAsync(listVM);
                if (!string.IsNullOrEmpty(contactTokenObj))
                {
                    ContactTokenVM contactTokenDtoObj = JsonConvert.DeserializeObject<ContactTokenVM>(contactTokenObj);

                    if (contactTokenDtoObj == null || contactTokenDtoObj == null) { return null; }

                    return contactTokenDtoObj;
                }
                return null;
            }
            return null;
        }
        public async Task<ClientUserContactTokenVM> GetTokenInfo(string Token, string Provider)
        {
            try
            {
                if (Provider.ToLower() == "microsoft")
                {
                    int pageNumber = 1;
                    var contactTokenVMObj = await MicroSoftManager<ClientUserContactTokenVM>.GetAsync("", null, OneClappContext.MicroSoftClientId, Token);
                    pageNumber++;
                    var contactTokenVM = JsonConvert.DeserializeObject<ClientUserContactTokenVM>(contactTokenVMObj);
                    if (contactTokenVM != null)
                    {
                        return contactTokenVM;
                    }
                    return null;
                }
                if (Provider.ToLower() == "google")
                {
                    int pageNumber = 1;

                    var contactGoogleTokenVMObj = await GoogleManager<ClientUserContactTokenVM>.GetAsyncByUrl("tokeninfo?access_token=" + Token, null, Token);
                    pageNumber++;
                    var contactTokenVM = JsonConvert.DeserializeObject<ClientUserContactTokenVM>(contactGoogleTokenVMObj);
                    if (contactTokenVM != null)
                    {
                        return contactTokenVM;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }


    }
    public interface IContactService
    {
        Task<ContactTokenVM> GetAccessToken(CommonContactTokenVM listVM, string Provider);
        Task<ClientUserContactTokenVM> GetTokenInfo(string Token, string Provider);
        //Task<ContactTokenVM> GetGoogleAccessToken(ContactTokenVM listVM);
    }
}