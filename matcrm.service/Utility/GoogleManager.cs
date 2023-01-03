using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.ViewModels;
using Newtonsoft.Json;

namespace matcrm.service.Utility
{
    public static class GoogleManager<T>
    {
        public const string mainUrl = "https://www.googleapis.com/oauth2/v3/";
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> PostAcccessTokenAsync(CommonContactTokenVM model)
        {
            client.DefaultRequestHeaders.Clear();

            var param = new Dictionary<string, string>();
            param.Add("code", model.code);
            param.Add("client_id", OneClappContext.GoogleCalendarClientId);   //DataUtility.GmailApiClientId
            param.Add("client_secret", OneClappContext.GoogleCalendarSecretKey);   //DataUtility.GmailApiClientSecret
            param.Add("redirect_uri", model.redirect_uri);
            param.Add("grant_type", model.grant_type);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, DataUtility.GmailToken) { Content = new FormUrlEncodedContent(param) };

            HttpResponseMessage httpResponseMessage = await client.SendAsync(request);

            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/x-www-form-urlencoded"); //CONTENT-TYPE header

            // HttpResponseMessage responseMessage = await client.SendAsync (request);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                return responseData.Result;
            }
            else
            {
                var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                return responseData.Result;
            }
        }

        public static async Task<string> GetAsyncByUrl(string api, T model, string token)
        {
            // string mainUrl = calendarUrl;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(OneClappContext.GoogleCalendarApiKey))
                    // client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);


                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(api);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                else
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                // return string.Empty;
            }
        }
    }
}