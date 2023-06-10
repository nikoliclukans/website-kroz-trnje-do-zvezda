namespace PerAspera.Models.DTOs
{
    public class CaptureOrderPaymentResponse
    {
        public string OrderId { get; set; }
        public string CaptureId { get; set; }
        public OrderPaymentStatus Status { get; set; }
        public string Token { get; set; }

    }
}
