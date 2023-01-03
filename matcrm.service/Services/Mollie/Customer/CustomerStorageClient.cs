using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.Customer {
    public class CustomerStorageClient : ICustomerStorageClient {
        private readonly ICustomerClient _customerClient;
        private readonly IMapper _mapper;

        public CustomerStorageClient(ICustomerClient customerClient, IMapper mapper) {
            this._customerClient = customerClient;
            this._mapper = mapper;
        }

        public async Task<CustomerResponse> Create(CreateCustomerModel model) {
            CustomerRequest customerRequest = this._mapper.Map<CustomerRequest>(model);
            return await this._customerClient.CreateCustomerAsync(customerRequest);
        }
    }

    public interface ICustomerStorageClient {
        Task<CustomerResponse> Create(CreateCustomerModel model);
    }
}