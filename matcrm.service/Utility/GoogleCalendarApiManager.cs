using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using matcrm.data.Context;
using matcrm.data.Models.ViewModels;

namespace matcrm.service.Utility
{
    public static class GoogleCalendarApiManager<T>
    {
        public static string calendarUrl = "https://www.googleapis.com/calendar/v3/";

        public static string calendarUrl1 = "https://www.googleapis.com/oauth2/v4/";

        public static string calendarAuthGetUrl = "https://accounts.google.com/o/oauth2/v2/auth?state=1&redirect_uri=[redirectUrl]&response_type=code&access_type=offline&client_id=[cliend_id]&prompt=consent&include_granted_scopes=true&login_hint=shraddha.prof21@gmail.com&scope=https://www.googleapis.com/auth/calendar.events";

        public static async Task<string> GetAsync(string api, T model, string apiKey)
        {

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(api);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    client.DefaultRequestHeaders.Add("AuthenticationToken", apiKey);

                HttpResponseMessage responseMessage = await client.GetAsync(api);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }

        public static async Task<string> GetAsync(string api, T model, string apiKey, string token)
        {
            string mainUrl = calendarUrl;

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

        public static async Task<string> GetAsyncByUrl(string api, T model, string mainUrl, string token)
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

        public static async Task<string> PostAcccessTokenAsync(string api, GoogleCalendarTokenVM model, string apiKey, string type)
        {
            string mainUrl = calendarUrl1;

            using (HttpClient httpClient = new HttpClient())
            {
                // client.BaseAddress = new Uri (mainUrl);
                // client.DefaultRequestHeaders.Accept.Clear ();
                // client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/x-www-form-urlencoded"));
                // if (!string.IsNullOrEmpty (apiKey))
                //     client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                var dict = new Dictionary<string, string>();
                dict.Add("client_id", OneClappContext.GoogleCalendarClientId);
                dict.Add("client_secret", OneClappContext.GoogleCalendarSecretKey);
                dict.Add("grant_type", model.grant_type);
                dict.Add("redirect_uri", model.redirect_uri);
                if (type == "Refresh_Token")
                {
                    dict.Add("refresh_token", model.refresh_token);
                }
                else
                {
                    dict.Add("code", model.code);
                }
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://www.googleapis.com/oauth2/v4/token") { Content = new FormUrlEncodedContent(dict) };

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                // HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Post, api);
                httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/x-www-form-urlencoded"); //CONTENT-TYPE header

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


        public static async Task<string> PostAsync(string api, T model, string apiKey, string token)
        {
            string mainUrl = calendarUrl;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    // client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, api);
                request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); //CONTENT-TYPE header

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);
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

        public static async Task<string> PutAsync(string api, T model, string apiKey, string token)
        {
            string mainUrl = calendarUrl;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    // client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, api);
                request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); //CONTENT-TYPE header

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);
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

        public static async Task<string> DeleteAsync(string api, T model, string apiKey, string token)
        {
            string mainUrl = calendarUrl;

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    // client.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);
                    httpClient.DefaultRequestHeaders.Add("Authorization", token);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, api);

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);
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
    }
}