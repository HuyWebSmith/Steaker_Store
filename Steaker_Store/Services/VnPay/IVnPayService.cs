using Steaker_Store.Models.Vnpay;

namespace Steaker_Store.Services.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context, string orderCode);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
