using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using matcrm.data.Context;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel.Payment.Request;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.service.Services.Mollie.Client;

namespace matcrm.service.Services.Mollie.Payment
{
    public class PaymentStorageClient : IPaymentStorageClient
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public PaymentStorageClient(IPaymentClient paymentClient, IMapper mapper, IConfiguration configuration)
        {
            this._paymentClient = paymentClient;
            this._mapper = mapper;
            this._configuration = configuration;
        }

        public async Task<PaymentResponse> Create(CreatePaymentModel model)
        {
            PaymentRequest paymentRequest = this._mapper.Map<PaymentRequest>(model);
            // paymentRequest.RedirectUrl = this._configuration["DefaultRedirectUrl"];
            paymentRequest.RedirectUrl = OneClappContext.MollieDefaultRedirectUrl;

            return await this._paymentClient.CreatePaymentAsync(paymentRequest);
        }

        public async Task<PaymentResponse> GetPayment(string paymentId)
        {
            return await this._paymentClient.GetPaymentAsync(paymentId);
        }
    }

    public interface IPaymentStorageClient
    {
        Task<PaymentResponse> Create(CreatePaymentModel model);

        Task<PaymentResponse> GetPayment(string paymentId);
    }
}