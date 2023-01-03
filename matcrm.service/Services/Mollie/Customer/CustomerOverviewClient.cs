using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.Customer {
    public class CustomerOverviewClient : OverviewClientBase<CustomerResponse>, ICustomerOverviewClient {
        private readonly ICustomerClient _customerClient;

        public CustomerOverviewClient(IMapper mapper, ICustomerClient customerClient) : base(mapper) {
            this._customerClient = customerClient;
        }

        public async Task<OverviewModel<CustomerResponse>> GetList() {
            return this.Map(await this._customerClient.GetCustomerListAsync());
        }

        public async Task<OverviewModel<CustomerResponse>> GetListByUrl(string url) {
            return this.Map(await this._customerClient.GetCustomerListAsync(this.CreateUrlObject(url)));
        }
    }

    public interface ICustomerOverviewClient {
        Task<OverviewModel<CustomerResponse>> GetList();
        Task<OverviewModel<CustomerResponse>> GetListByUrl(string url);
    }
}