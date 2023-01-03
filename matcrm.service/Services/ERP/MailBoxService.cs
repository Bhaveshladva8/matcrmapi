using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.service.Utility;

namespace matcrm.service.Services.ERP
{
    public class MailBoxService: IMailBoxService
    {
        public async Task<MailTokenDto> GetAccessToken(MailTokenDto listVM)
        {

            var mailTokenObj = await MailBoxManager<MailTokenDto>.PostAcccessTokenAsync("token", listVM);
            if (!string.IsNullOrEmpty(mailTokenObj))
            {
                MailTokenDto mailTokenDtoObj = JsonConvert.DeserializeObject<MailTokenDto>(mailTokenObj);

                if (mailTokenDtoObj == null || mailTokenDtoObj == null) { return null; }

                return mailTokenDtoObj;
            }
            return null;
        }

         public async Task<MailBoxTokenVM> GetTokenInfo(string calendarToken, string calendarKey)
        {
            try
            {
                int pageNumber = 1;

                var mailBoxTokenVMObj = await MailBoxManager<MailBoxTokenVM>.GetAsync("", null, OneClappContext.MicroSoftClientId, calendarToken);
                pageNumber++;
                var mailBoxTokenVM = JsonConvert.DeserializeObject<MailBoxTokenVM>(mailBoxTokenVMObj);
                if (mailBoxTokenVM != null)
                {
                    return mailBoxTokenVM;
                }
                return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        public async Task<InboxThreads> GetInboxThread(string api, string calendarToken)
        {
            try
            {
                int pageNumber = 1;

                var inboxThreadsObj = await MailBoxManager<InboxThreads>.GetAsync("", null, OneClappContext.MicroSoftClientId, calendarToken);
                pageNumber++;
                var inboxThreads = JsonConvert.DeserializeObject<InboxThreads>(inboxThreadsObj);
                if (inboxThreads != null)
                {
                    return inboxThreads;
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

    public interface IMailBoxService
    {
        Task<MailTokenDto> GetAccessToken(MailTokenDto listVM);
        Task<MailBoxTokenVM> GetTokenInfo(string calendarToken, string calendarKey);
        Task<InboxThreads> GetInboxThread(string api, string calendarToken);
    }
}