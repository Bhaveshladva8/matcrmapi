using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Subscription;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.Subscription {
    public class SubscriptionStorageClient : ISubscriptionStorageClient {
        private readonly ISubscriptionClient _subscriptionClient;
        private readonly IMapper _mapper;

        public SubscriptionStorageClient(ISubscriptionClient subscriptionClient, IMapper mapper) {
            this._subscriptionClient = subscriptionClient;
            this._mapper = mapper;
        }

        public async Task<SubscriptionResponse> Create(CreateSubscriptionModel model) {
            SubscriptionRequest subscriptionRequest = this._mapper.Map<SubscriptionRequest>(model);
            subscriptionRequest.Amount.Value = subscriptionRequest.Amount.Value.ToString();
            return await this._subscriptionClient.CreateSubscriptionAsync(model.CustomerId, subscriptionRequest);
        }

        public async Task Cancel(string customerId, string subscriptionId) {
            await this._subscriptionClient.CancelSubscriptionAsync(customerId, subscriptionId);
        }
    }

    public interface ISubscriptionStorageClient {
        Task<SubscriptionResponse> Create(CreateSubscriptionModel model);
        Task Cancel(string customerId, string subscriptionId);
    }
}