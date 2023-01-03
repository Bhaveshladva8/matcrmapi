using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.PaymentMethod;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.PaymentMethod {
    public class PaymentMethodOverviewClient : OverviewClientBase<PaymentMethodResponse>, IPaymentMethodOverviewClient {
        private readonly IPaymentMethodClient _paymentMethodClient;

        public PaymentMethodOverviewClient(IMapper mapper, IPaymentMethodClient paymentMethodClient) : base(mapper) {
            this._paymentMethodClient = paymentMethodClient;
        }

        public async Task<OverviewModel<PaymentMethodResponse>> GetList() {
            return this.Map(await this._paymentMethodClient.GetPaymentMethodListAsync());
        }
    }

    public interface IPaymentMethodOverviewClient {
        Task<OverviewModel<PaymentMethodResponse>> GetList();
    }
}