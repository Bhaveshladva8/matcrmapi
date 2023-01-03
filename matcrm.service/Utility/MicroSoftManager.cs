using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.ViewModels;
using Newtonsoft.Json;

namespace matcrm.service.Utility
{
    public static class MicroSoftManager<T>
    {
        //public const string Office365Token = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        public const string mainUrl = "https://graph.microsoft.com/v1.0/me";        
        public static async Task<string> PostAcccessTokenAsync(string api, CommonContactTokenVM model)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var dict = new Dictionary<string, string>();
                dict.Add("client_id", OneClappContext.MicroSoftClientId);
                dict.Add("client_secret", OneClappContext.MicroSecretKey);
                dict.Add("grant_type", model.grant_type);
                dict.Add("redirect_uri", model.redirect_uri);
                if (model.type == "Refresh_Token")
                {
                    dict.Add("refresh_token", model.refresh_token);
                }
                else
                {
                    dict.Add("code", model.code);
                }
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, DataUtility.Office365Token) { Content = new FormUrlEncodedContent(dict) };

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);

                // HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Post, api);
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
        }

        public static async Task<string> GetAsync(string api, T model, string apiKey, string token)
        {            
            
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    // client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(api);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }
    }
}