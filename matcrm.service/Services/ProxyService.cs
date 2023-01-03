using matcrm.service.Utility;
using System.Threading.Tasks;

namespace matcrm.service.Services
{
    public class ProxyService : IProxyService
    {
        public async Task<string> GetProxy(string url, string apiKey)
        {
            return await BaseApiManager<string>.GetAsync(url, apiKey);
        }
        public async Task<string> PostProxy(string url, string apiKey)
        {
            return await BaseApiManager<string>.PostAsync(url, null, apiKey);
        }
        //public async Task<string> PutProxy(string url, string apiKey)
        //{
        //    return await BaseApiManager<string>.PutAsync(url, null, apiKey);
        //}
    }

    public interface IProxyService
    {
        Task<string> GetProxy(string url, string apiKey);
        Task<string> PostProxy(string url, string apiKey);
        //Task<string> PutProxy(string url, string apiKey);

    }
}
