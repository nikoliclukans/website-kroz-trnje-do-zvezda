using PerAspera.Models.DTOs;

namespace PerAspera.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<CreateOrderResponse> CreateOrder(Guid bookKey, string price);
        Task<CaptureOrderPaymentResponse> CaptureOrder(string token);
        Task<RefundPaymentResponse> RefundPayment(string captureId);

    }
}
