using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace matcrm.service.Utility
{
    public static class BaseApiManager<T>
    {
        public static async Task<string> GetAsync(string url, string apiKey)
        {

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    httpClient.DefaultRequestHeaders.Add("AuthenticationToken", apiKey);

                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(url);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }

        public static async Task<string> PostAsync(string url, T model, string apiKey)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(url);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (!string.IsNullOrEmpty(apiKey))
                    httpClient.DefaultRequestHeaders.Add("AuthenticationToken", apiKey);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(request);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var responseData = httpResponseMessage.Content.ReadAsStringAsync();
                    return responseData.Result;
                }
                return string.Empty;
            }
        }

        //public static async Task<T> PutAsync(string url, T model, string apiKey)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(url);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        if (!string.IsNullOrEmpty(apiKey))
        //            client.DefaultRequestHeaders.Add("AuthenticationToken", apiKey);

        //        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
        //        request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); //CONTENT-TYPE header

        //        HttpResponseMessage responseMessage = await client.SendAsync(request);
        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            var responseData = responseMessage.Content.ReadAsStringAsync();
        //            return responseData.Result;
        //        }
        //        return string.Empty;
        //    }
        //}
        //public static async Task<T> PostFileAsync(string url,string apiKey, byte[] fileData, string fileType)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(url);
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", fileType);

        //        if (!string.IsNullOrEmpty(apiKey))
        //            client.DefaultRequestHeaders.Add("AuthenticationToken", apiKey);

        //        ByteArrayContent fileContent = new ByteArrayContent(fileData);

        //        HttpResponseMessage responseMessage = await client.PostAsync(url, fileContent);
        //        if (responseMessage.IsSuccessStatusCode)
        //        {
        //            var responseData = responseMessage.Content.ReadAsStringAsync();
        //            return responseData.Result;
        //        }
        //        return string.Empty;
        //    }
        //}
    }
}
