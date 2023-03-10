using System.Threading.Tasks;
using matcrm.data.Models.MollieModel.Mandate;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.Mandate {
    public interface IMandateStorageClient {
        Task<MandateResponse> Create(string customerId);
    }

    public class MandateStorageClient : IMandateStorageClient {
        private readonly IMandateClient _mandateClient;
        public MandateStorageClient(IMandateClient mandateClient) {
            this._mandateClient = mandateClient;
        }

        public async Task<MandateResponse> Create(string customerId) {
            MandateRequest mandateRequest = this.CreateDefaultMandateRequest();
            return await this._mandateClient.CreateMandateAsync(customerId, mandateRequest);
        }

        private MandateRequest CreateDefaultMandateRequest() {
            return new SepaDirectDebitMandateRequest() {
                Method = matcrm.data.Models.MollieModel.Payment.PaymentMethod.DirectDebit,
                ConsumerName = "John Smit",
                ConsumerAccount = "NL86INGB0002445588", // IBAN van de belastingdienst ;D
            };
        }
    }
}