using Backend.Models.Payment;

namespace Backend.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPay PaymentExecute(IQueryCollection collections);
    }
}