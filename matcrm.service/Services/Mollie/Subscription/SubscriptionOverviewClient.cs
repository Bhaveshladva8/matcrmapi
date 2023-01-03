using System.Threading.Tasks;
using AutoMapper;
using matcrm.service.Services.Mollie;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Subscription;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.Subscription {
    public class SubscriptionOverviewClient : OverviewClientBase<SubscriptionResponse>, ISubscriptionOverviewClient {
        private readonly ISubscriptionClient _subscriptionClient;

        public SubscriptionOverviewClient(IMapper mapper, ISubscriptionClient subscriptionClient) : base(mapper) {
            this._subscriptionClient = subscriptionClient;
        }

        public async Task<OverviewModel<SubscriptionResponse>> GetList(string customerId) {
            return this.Map(await this._subscriptionClient.GetSubscriptionListAsync(customerId));
        }

        public async Task<OverviewModel<SubscriptionResponse>> GetListByUrl(string url) {
            return this.Map(await this._subscriptionClient.GetSubscriptionListAsync(this.CreateUrlObject(url)));
        }
    }

    public interface ISubscriptionOverviewClient {
        Task<OverviewModel<SubscriptionResponse>> GetList(string customerId);
        Task<OverviewModel<SubscriptionResponse>> GetListByUrl(string url);
    }
}