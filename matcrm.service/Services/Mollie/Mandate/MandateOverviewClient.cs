using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Mandate;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.Mandate {
    public class MandateOverviewClient : OverviewClientBase<MandateResponse>, IMandateOverviewClient {
        private readonly IMandateClient _mandateClient;

        public MandateOverviewClient(IMapper mapper, IMandateClient mandateClient) : base (mapper) {
            this._mandateClient = mandateClient;
        }

        public async Task<OverviewModel<MandateResponse>> GetList(string customerId) {
            return this.Map(await this._mandateClient.GetMandateListAsync(customerId));
        }

        public async Task<OverviewModel<MandateResponse>> GetListByUrl(string url) {
            return this.Map(await this._mandateClient.GetMandateListAsync(this.CreateUrlObject(url)));
        }
    }

    public interface IMandateOverviewClient {
        Task<OverviewModel<MandateResponse>> GetList(string customerId);
        Task<OverviewModel<MandateResponse>> GetListByUrl(string url);
    }
}