using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace matcrm.service.Utility {
    public static class SlackApiManager<T> {
        public static string slackUrl = "https://slack.com/api/";

        public static async Task<string> GetAsync (string api, T model, string apiKey) {

            using (HttpClient httpClient = new HttpClient ()) {
                httpClient.BaseAddress = new Uri (api);
                httpClient.DefaultRequestHeaders.Accept.Clear ();
                httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                if (!string.IsNullOrEmpty (apiKey))
                    httpClient.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync (api);

                if (httpResponseMessage.IsSuccessStatusCode) {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }

        public static async Task<string> GetAsync (string api, string userName, T model, string apiKey) {
            string mainUrl = slackUrl;
            mainUrl = mainUrl.Replace ("*userName*", userName);

            using (HttpClient httpClient = new HttpClient ()) {
                httpClient.BaseAddress = new Uri (mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear ();
                httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                if (!string.IsNullOrEmpty (apiKey))
                    httpClient.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync (api);

                if (httpResponseMessage.IsSuccessStatusCode) {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }

        public static async Task<string> PostAsync (string api, string userName, T model, string slackToken) {
            string mainUrl = slackUrl;
            mainUrl = mainUrl.Replace ("*userName*", userName);

            using (HttpClient httpClient = new HttpClient ()) {
                httpClient.BaseAddress = new Uri (mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear ();
                httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                if (!string.IsNullOrEmpty (slackToken))
                    httpClient.DefaultRequestHeaders.Add ("Authorization", "Bearer " +slackToken);

                HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Post, api);
                request.Content = new StringContent (JsonConvert.SerializeObject (model), Encoding.UTF8, "application/json"); //CONTENT-TYPE header

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync (request);
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                } else {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }
                // return string.Empty;
            }
        }

        public static async Task<string> PutAsync (string api, string userName, T model, string apiKey) {
            string mainUrl = slackUrl;
            mainUrl = mainUrl.Replace ("*userName*", userName);

            using (HttpClient httpClient = new HttpClient ()) {
                httpClient.BaseAddress = new Uri (mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear ();
                httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                if (!string.IsNullOrEmpty (apiKey))
                    httpClient.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Put, api);
                request.Content = new StringContent (JsonConvert.SerializeObject (model), Encoding.UTF8, "application/json"); //CONTENT-TYPE header

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync (request);
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }

        public static async Task<string> DeleteAsync (string api, string userName, T model, string apiKey) {
            string mainUrl = slackUrl;
            mainUrl = mainUrl.Replace ("*userName*", userName);

            using (HttpClient httpClient = new HttpClient ()) {
                httpClient.BaseAddress = new Uri (mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear ();
                httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                if (!string.IsNullOrEmpty (apiKey))
                    httpClient.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Delete, api);
                request.Content = new StringContent (JsonConvert.SerializeObject (model), Encoding.UTF8, "application/json"); //CONTENT-TYPE header

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync (request);
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                } else {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }
                // return string.Empty;
            }
        }
    }
}