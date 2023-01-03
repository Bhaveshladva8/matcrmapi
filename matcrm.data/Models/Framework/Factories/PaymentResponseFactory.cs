using matcrm.data.Models.MollieModel.Payment;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Payment.Response.Specific;

namespace matcrm.data.Framework.Factories {
    public class PaymentResponseFactory {
        public PaymentResponse Create(string paymentMethod) {
            switch (paymentMethod) {
                case PaymentMethod.BankTransfer:
                    return new BankTransferPaymentResponse();
                case PaymentMethod.CreditCard:
                    return new CreditCardPaymentResponse();
                case PaymentMethod.Ideal:
                    return new IdealPaymentResponse();
                case PaymentMethod.Bancontact:
                    return new BancontactPaymentResponse();
                case PaymentMethod.PayPal:
                    return new PayPalPaymentResponse();
                case PaymentMethod.PaySafeCard:
                    return new PaySafeCardPaymentResponse();
                case PaymentMethod.Sofort:
                    return new SofortPaymentResponse();
                case PaymentMethod.Belfius:
                    return new BelfiusPaymentResponse();
                case PaymentMethod.DirectDebit:
                    return new SepaDirectDebitResponse();
                case PaymentMethod.Kbc:
                    return new KbcPaymentResponse();
                case PaymentMethod.GiftCard:
                    return new GiftcardPaymentResponse();
                case PaymentMethod.IngHomePay:
                    return new IngHomePayPaymentResponse();
                default:
                    return new PaymentResponse();
            }
        }
    }
}