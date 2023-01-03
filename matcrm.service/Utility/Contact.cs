using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Net.Http;
using matcrm.data.Context;
using matcrm.data.Helpers;
using System.Net;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using matcrm.data.Models.Request;
using System.Dynamic;
using System.Net.Http.Headers;
using System.Text;

namespace matcrm.service.Utility
{
    public class Contact
    {
        private readonly IIntProviderService _intProviderService;
        private readonly IIntProviderAppService _intProviderAppService;
        //private readonly IClientIntProviderAppSecretService _clientIntProviderAppSecretService;
        private readonly IIntProviderAppSecretService _intProviderAppSecretService;
        private readonly IUserService _userService;

        #region 'Global Initialization'
        private static readonly HttpClient client = new HttpClient();
        private ContactTokenVM contactTokenVMObj = new ContactTokenVM();
        private IntProviderAppSecret intProviderAppSecretObj = new IntProviderAppSecret();
        private string token;
        private string MicrosoftScope;
        #endregion
        public Contact(IIntProviderService intProviderService,
        IIntProviderAppService intProviderAppService,
        //IClientIntProviderAppSecretService clientIntProviderAppSecretService,
        IIntProviderAppSecretService intProviderAppSecretService,
        IUserService userService)
        {
            _intProviderService = intProviderService;
            _intProviderAppService = intProviderAppService;
            //_clientIntProviderAppSecretService = clientIntProviderAppSecretService;
            _intProviderAppSecretService = intProviderAppSecretService;
            _userService = userService;
            MicrosoftScope = OneClappContext.MicrosoftClientScopes;
        }
        public async Task<ClientContactVM> GetContact(int userId,InboxVM model)
        {
            ClientContactVM contactVM = new ClientContactVM();
            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                contactVM.IsValid = false;
                contactVM.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return contactVM;
            }

            var task = new List<Task>();
            var intProviderObj = _intProviderService.GetIntProvider(model.Provider);
            var intproviderApp = _intProviderAppService.GetIntProviderAppByProviderId(intProviderObj.Id, model.ProviderApp);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);
            if (intproviderApp != null && intAppSecretObj != null)
            {
                contactTokenVMObj.refresh_token = intAppSecretObj.Refresh_Token;
                contactTokenVMObj.access_token = intAppSecretObj.Access_Token;
                //contactTokenVMObj.code = model.Code;
                contactTokenVMObj.ProviderApp = model.Provider;
                contactTokenVMObj.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }
            switch (intproviderApp.IntProvider.Name)
            {
                case "Google":
                    await SetGmailToken();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    var api = string.Format(DataUtility.AllGoogleContacts, model.SelectedEmail);

                    var googleCalendarResponse = await client.GetAsync(api);
                    if (googleCalendarResponse.StatusCode == HttpStatusCode.OK)
                    {
                        var json = JObject.Parse(await googleCalendarResponse.Content.ReadAsStringAsync());
                        var data1 = JsonConvert.DeserializeObject<ClientContactVM>(json.ToString());
                        contactVM = data1;
                    }
                    break;
                case "Microsoft":
                    await SetOffice365Token();
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    string office365Uri = DataUtility.AllMSContactEvents;

                    var office365Response = await client.GetAsync(office365Uri);
                    if (office365Response.StatusCode == HttpStatusCode.OK)
                    {
                        var json = JObject.Parse(await office365Response.Content.ReadAsStringAsync());
                        contactVM = JsonConvert.DeserializeObject<ClientContactVM>(json.ToString());
                    }
                    break;
                default:
                    break;
            }
            return contactVM;
        }

        public async Task SetOffice365Token()
        {
            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("client_id", OneClappContext.MicroSoftClientId);  //DataUtility.Office365ClientId
            param.Add("client_secret", OneClappContext.MicroSecretKey);  //DataUtility.Office365ClientSecret
            param.Add("refresh_token", intProviderAppSecretObj.Refresh_Token);
            param.Add("redirect_uri", OneClappContext.MicroSoftRedirectUrl);
            param.Add("scope", MicrosoftScope);
            param.Add("grant_type", "refresh_token");

            var response = await client.PostAsync(DataUtility.Office365Token, new FormUrlEncodedContent(param));
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var serializer = new DataContractJsonSerializer(typeof(Office365Token));
                    var office365Token = (Office365Token)serializer.ReadObject(stream);

                    if (office365Token != null)
                    {
                        token = office365Token.access_token;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task SetGmailToken()
        {
            client.DefaultRequestHeaders.Clear();
            var param = new Dictionary<string, string>();
            param.Add("client_id", OneClappContext.GoogleCalendarClientId);           //DataUtility.GmailApiClientId
            param.Add("client_secret", OneClappContext.GoogleCalendarSecretKey);   //DataUtility.GmailApiClientSecret
            param.Add("refresh_token", intProviderAppSecretObj.Refresh_Token);
            param.Add("grant_type", "refresh_token");
            var response = await client.PostAsync(DataUtility.GmailToken, new FormUrlEncodedContent(param));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new DataContractJsonSerializer(typeof(Token));
                var gmailToken = (Token)serializer.ReadObject(stream);
                if (gmailToken != null) token = gmailToken.access_token;
            }
        }
        public async Task<ClientCreateContactVM> CreateContact(int userId, string ContactId, ClientUserCreateContactRequest model)
        {
            ClientCreateContactVM contactVMObj = new ClientCreateContactVM();
            var user = _userService.GetUserById(userId);
            if (user == null)
            {
                contactVMObj.IsValid = false;
                contactVMObj.ErrorMessage = CommonMessage.UnAuthorizedUser;
                return contactVMObj;
            }
            var intProviderObj = _intProviderService.GetIntProvider(model.Provider);
            var intproviderApp = _intProviderAppService.GetIntProviderAppByProviderId(intProviderObj.Id, model.ProviderApp);
            var intAppSecretObj = _intProviderAppSecretService.GetActiveSecretByUserAndEmail(userId, model.SelectedEmail, intproviderApp.Id);

            if (intproviderApp != null && intAppSecretObj != null)
            {
                contactTokenVMObj.refresh_token = intAppSecretObj.Refresh_Token;
                contactTokenVMObj.access_token = intAppSecretObj.Access_Token;
                //contactTokenVMObj.code = model.Code;
                contactTokenVMObj.ProviderApp = model.Provider;
                contactTokenVMObj.UserId = userId;
                intProviderAppSecretObj.Refresh_Token = intAppSecretObj.Refresh_Token;
                intProviderAppSecretObj.Access_Token = intAppSecretObj.Access_Token;
                intProviderAppSecretObj.Email = intAppSecretObj.Email;
            }

            switch (model.Provider)
            {
                case "Google":
                    await SetGmailToken();
                    dynamic myObject = new ExpandoObject();
                    // if (!string.IsNullOrEmpty(model.givenName))
                    // {
                    //     myObject.givenName = model.givenName;
                    // }
                    // if (!string.IsNullOrEmpty(model.FirstName))
                    // {
                    //     myObject.names = model.FirstName + " " + model.Lastname;
                    // }

                    if (model.Email.Count > 0)
                    {
                        myObject.emailAddresses = new List<ClientUserGoogleContactEmailRequest>();
                        foreach (var emailObj in model.Email)
                        {
                            ClientUserGoogleContactEmailRequest Obj1 = new ClientUserGoogleContactEmailRequest();
                            Obj1.value = emailObj.Email;
                            Obj1.displayName = model.FirstName;
                            myObject.emailAddresses.Add(Obj1);
                        }
                        myObject.emailAddresses = myObject.emailAddresses.ToArray();
                    }
                    // Serialize our concrete class into a JSON String
                    var stringPayload1 = JsonConvert.SerializeObject(myObject);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent1 = new StringContent(stringPayload1, Encoding.UTF8, "application/json");
                    string apiUrl = null;
                    if (string.IsNullOrEmpty(ContactId))
                    {
                        apiUrl = string.Format(DataUtility.CreateGoogleContactEvents);
                        var googleResponse = await client.PostAsync(apiUrl, httpContent1);
                        if (googleResponse.StatusCode == HttpStatusCode.OK)
                        {
                            contactVMObj.IsValid = true;
                            contactVMObj.error_description = "";
                            var json = JObject.Parse(await googleResponse.Content.ReadAsStringAsync());
                            var data1 = JsonConvert.DeserializeObject<ClientCreateContactVM>(json.ToString());
                            contactVMObj = data1;                            
                        }
                    }
                    // else
                    // {
                    //     apiUrl = string.Format(DataUtility.CreateGoogleContactEvents, model.SelectedEmail, ContactId);
                    //     var googleResponse = await client.PutAsync(apiUrl, httpContent1);
                    //     if (googleResponse.StatusCode == HttpStatusCode.OK)
                    //     {
                    //         contactVMObj.IsValid = true;
                    //         contactVMObj.error_description = "";
                    //         var data = await googleResponse.Content.ReadAsStringAsync();
                    //         if (!string.IsNullOrEmpty(data))
                    //         {
                    //             ClientCreateContactVM result = JsonConvert.DeserializeObject<ClientCreateContactVM>(data);
                    //             if (result != null)
                    //             {
                    //                 contactVMObj.id = result.id;
                    //             }
                    //         }
                    //     }
                    // }

                    break;
                case "Microsoft":
                    await SetOffice365Token();

                    dynamic myMSObject = new ExpandoObject();
                    if (!string.IsNullOrEmpty(model.FirstName))
                    {
                        myMSObject.givenName = model.FirstName;
                    }
                    if (!string.IsNullOrEmpty(model.Lastname))
                    {
                        myMSObject.surname = model.Lastname;
                    }

                    if (model.Email.Count > 0)
                    {
                        myMSObject.emailAddresses = new List<ClientUserMicrosoftContactEmailRequest>();
                        foreach (var emailObj in model.Email)
                        {
                            ClientUserMicrosoftContactEmailRequest Obj1 = new ClientUserMicrosoftContactEmailRequest();
                            Obj1.address = emailObj.Email;
                            myMSObject.emailAddresses.Add(Obj1);
                        }
                        myMSObject.emailAddresses = myMSObject.emailAddresses.ToArray();
                    }
                    // Serialize our concrete class into a JSON String
                    var stringPayload = JsonConvert.SerializeObject(myMSObject);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
                    var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                    if (string.IsNullOrEmpty(ContactId))
                    {
                        var url = string.Format(DataUtility.CreateMSContactEvents);
                        var threadResponse = await client.PostAsync(url, httpContent);
                        if (threadResponse.StatusCode == HttpStatusCode.Created)
                        {
                            contactVMObj.IsValid = true;
                            contactVMObj.error_description = "";
                            var json = JObject.Parse(await threadResponse.Content.ReadAsStringAsync());

                            var data1 = JsonConvert.DeserializeObject<ClientCreateContactVM>(json.ToString());
                            contactVMObj = data1;
                        }
                    }
                    else
                    {
                        var url = string.Format(DataUtility.CreateMSContactEvents, ContactId);
                        var threadResponse = await client.PatchAsync(url, httpContent);
                        if (threadResponse.StatusCode == HttpStatusCode.Created)
                        {
                            contactVMObj.IsValid = true;
                            contactVMObj.error_description = "";
                            var json = JObject.Parse(await threadResponse.Content.ReadAsStringAsync());

                            var data1 = JsonConvert.DeserializeObject<ClientCreateContactVM>(json.ToString());
                            contactVMObj = data1;
                        }
                    }
                    break;

                default:
                    break;
            }
            return contactVMObj;
        }

    }
}