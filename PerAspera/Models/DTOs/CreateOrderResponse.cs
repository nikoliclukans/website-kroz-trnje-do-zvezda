namespace PerAspera.Models.DTOs
{
    public class CreateOrderResponse
    {
        public string OrderId { get; set; }
        public string ApproveUrl { get; set; }
        public OrderPaymentStatus Status { get; set; }

    }
}
