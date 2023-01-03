using System.Globalization;
using AutoMapper;
using matcrm.data.Models.MollieModel;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.MollieModel.List;
using matcrm.data.Models.MollieModel.Mandate;
using matcrm.data.Models.MollieModel.Payment.Request;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.PaymentMethod;
using matcrm.data.Models.MollieModel.Subscription;
using matcrm.data.Models.Dto.Mollie;

namespace matcrm.service.Services.Mollie.Automapper {
    public class MollieApiProfile : Profile {
        public MollieApiProfile() {
            this.CreateMap<CreatePaymentModel, PaymentRequest>()
                .ForMember(x => x.Amount, m => m.MapFrom(x => new Amount(x.Currency, x.Amount.ToString(CultureInfo.InvariantCulture))));

            this.CreateMap<CreateSubscriptionModel, SubscriptionRequest>()
                .ForMember(x => x.Amount, m => m.MapFrom(x => new Amount(x.Amount.Currency, x.Amount.value.ToString(CultureInfo.InvariantCulture))))
                .ForMember(x => x.Interval, m => m.MapFrom(x => $"{x.IntervalAmount} {x.IntervalPeriod.ToString().ToLower()}"));

            this.CreateMap<CreateCustomerModel, CustomerRequest>();

            this.CreateOverviewMap<PaymentResponse>();
            this.CreateOverviewMap<CustomerResponse>();
            this.CreateOverviewMap<SubscriptionResponse>();
            this.CreateOverviewMap<MandateResponse>();
            this.CreateOverviewMap<PaymentMethodResponse>();
        }

        private void CreateOverviewMap<TResponseType>() where TResponseType : IResponseObject {
            this.CreateMap<ListResponse<TResponseType>, OverviewModel<TResponseType>>()
                .ForMember(x => x.Items, m => m.MapFrom(x => x.Items))
                .ForMember(x => x.Navigation, m => m.MapFrom(x => new OverviewNavigationLinksModel(x.Links.Previous, x.Links.Next)));
        }
    }
}