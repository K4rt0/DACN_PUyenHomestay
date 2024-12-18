using Backend.Models.Payment;

namespace Backend.Services
{
    public interface IMomoService
    {
        Task<string?> CreatePaymentAsync(OrderInfoModel model);
        MomoExecuteResponse PaymentExecuteAsync(IQueryCollection collection);
    }
}