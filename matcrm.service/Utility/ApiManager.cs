using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace matcrm.service.Utility {
    public static class ApiManager<T> {
        public static string weClappUrl = "https://*check*.weclapp.com/webapp/api/v1/";

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
        public static async Task<string> GetAsync (string api, string tenant, T model, string apiKey) {
            string mainUrl = weClappUrl;
            mainUrl = mainUrl.Replace ("*check*", tenant);

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
        public static async Task<string> PostAsync (string api, string tenant, T model, string apiKey) {
            string mainUrl = weClappUrl;
            mainUrl = mainUrl.Replace ("*check*", tenant);

            using (HttpClient httpClient = new HttpClient ()) {
                httpClient.BaseAddress = new Uri (mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear ();
                httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                if (!string.IsNullOrEmpty (apiKey))
                    httpClient.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Post, api);
                request.Content = new StringContent (JsonConvert.SerializeObject (model), Encoding.UTF8, "application/json"); //CONTENT-TYPE header

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync (request);
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }else {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }
                // return string.Empty;
            }
        }
        public static async Task<string> PutAsync (string api, string tenant, T model, string apiKey) {
            string mainUrl = weClappUrl;
            mainUrl = mainUrl.Replace ("*check*", tenant);

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
        public static async Task<string> PostFileAsync (string api, string tenant, T model, string apiKey, byte[] fileData, string fileType) {
            string mainUrl = weClappUrl;
            mainUrl = mainUrl.Replace ("*check*", tenant);

            using (HttpClient httpClient = new HttpClient ()) {
                httpClient.BaseAddress = new Uri (mainUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear ();
                httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation ("Content-Type", fileType);

                if (!string.IsNullOrEmpty (apiKey))
                    httpClient.DefaultRequestHeaders.Add ("AuthenticationToken", apiKey);

                //MultipartFormDataContent content = new MultipartFormDataContent();

                ByteArrayContent fileContent = new ByteArrayContent (fileData);
                //fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileType);
                //content.Add(fileContent, fileName.Replace(".pdf", ""), fileName);

                HttpResponseMessage httpResponseMessage = await httpClient.PostAsync (api, fileContent);

                //HttpResponseMessage responseMessage = await client.PostAsync(api, content);
                if (httpResponseMessage.IsSuccessStatusCode) {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync ();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }
    }
}