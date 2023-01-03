using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using matcrm.data.Extensions;
using matcrm.data.Models.MollieModel;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Payment;
using matcrm.data.Models.MollieModel.PaymentMethod;
using matcrm.data.Models.MollieModel.Url;
using matcrm.service.Services.Client;

namespace matcrm.service.Services.Mollie.Client
{
    public class PaymentMethodClient : BaseMollieClient, IPaymentMethodClient
    {
        public PaymentMethodClient(string apiKey, HttpClient httpClient = null) : base(apiKey, httpClient) {
        }

        public async Task<PaymentMethodResponse> GetPaymentMethodAsync(string paymentMethod, bool includeIssuers = false, string locale = null, bool includePricing = false, string profileId = null, bool testmode = false, string currency = null) {
            Dictionary<string, string> queryParameters = this.BuildQueryParameters(
                locale: locale, 
                currency: currency, 
                profileId: profileId, 
                testmode: testmode, 
                includeIssuers: includeIssuers, 
                includePricing: includePricing);

            return await this.GetAsync<PaymentMethodResponse>($"methods/{paymentMethod.ToLower()}{queryParameters.ToQueryString()}").ConfigureAwait(false);
        }

        public async Task<ListResponse<PaymentMethodResponse>> GetAllPaymentMethodListAsync(string locale = null, Amount amount = null, bool includeIssuers = false, bool includePricing = false) {
            Dictionary<string, string> queryParameters = this.BuildQueryParameters(
               locale: locale,
               amount: amount,
               includeIssuers: includeIssuers,
               includePricing: includePricing);

            return await this.GetListAsync<ListResponse<PaymentMethodResponse>>("methods/all", null, null, queryParameters).ConfigureAwait(false);
        }

        public async Task<ListResponse<PaymentMethodResponse>> GetPaymentMethodListAsync(string sequenceType = null, string locale = null, Amount amount = null, bool includeIssuers = false, bool includePricing = false, string profileId = null, bool testmode = false, Resource? resource = null, string billingCountry = null) {
            Dictionary<string, string> queryParameters = this.BuildQueryParameters(
               sequenceType: sequenceType,
               locale: locale,
               amount: amount,
               includeIssuers: includeIssuers,
               includePricing: includePricing,
               resource: resource,
               profileId: profileId,
               testmode: testmode,
               billingCountry: billingCountry);

            return await this.GetListAsync<ListResponse<PaymentMethodResponse>>("methods", null, null, queryParameters).ConfigureAwait(false);
        }

        public async Task<PaymentMethodResponse> GetPaymentMethodAsync(UrlObjectLink<PaymentMethodResponse> url) {
            return await this.GetAsync(url).ConfigureAwait(false);
        }

        private Dictionary<string, string> BuildQueryParameters(string sequenceType = null, string locale = null, Amount amount = null, bool includeIssuers = false, bool includePricing = false, string profileId = null, bool testmode = false, Resource? resource = null, string currency = null, string billingCountry = null) {
            var result = new Dictionary<string, string>();
            result.AddValueIfTrue(nameof(testmode), testmode);
            result.AddValueIfNotNullOrEmpty(nameof(sequenceType), sequenceType?.ToLower());
            result.AddValueIfNotNullOrEmpty(nameof(profileId), profileId);
            result.AddValueIfNotNullOrEmpty(nameof(locale), locale);
            result.AddValueIfNotNullOrEmpty("amount[currency]", amount?.Currency);
            result.AddValueIfNotNullOrEmpty("amount[value]", amount?.Value);
            result.AddValueIfNotNullOrEmpty("include", this.BuildIncludeParameter(includeIssuers, includePricing));
            result.AddValueIfNotNullOrEmpty(nameof(resource), resource?.ToString()?.ToLower());
            result.AddValueIfNotNullOrEmpty(nameof(currency), currency);
            result.AddValueIfNotNullOrEmpty(nameof(billingCountry), billingCountry);
            return result;
        }

        private string BuildIncludeParameter(bool includeIssuers = false, bool includePricing = false) {
            var includeList = new List<string>();
            includeList.AddValueIfTrue("issuers", includeIssuers);
            includeList.AddValueIfTrue("pricing", includePricing);
            return includeList.ToIncludeParameter();
        }
    }
    public interface IPaymentMethodClient {
		Task<PaymentMethodResponse> GetPaymentMethodAsync(string paymentMethod, bool includeIssuers = false, string locale = null, bool includePricing = false, string profileId = null, bool testmode = false, string currency = null);
        Task<ListResponse<PaymentMethodResponse>> GetAllPaymentMethodListAsync(string locale = null, Amount amount = null, bool includeIssuers = false, bool includePricing = false);
        Task<ListResponse<PaymentMethodResponse>> GetPaymentMethodListAsync(string sequenceType = null, string locale = null, Amount amount = null, bool includeIssuers = false, bool includePricing = false, string profileId = null, bool testmode = false, Resource? resource = null, string billingCountry = null);
        Task<PaymentMethodResponse> GetPaymentMethodAsync(UrlObjectLink<PaymentMethodResponse> url);
    }
}