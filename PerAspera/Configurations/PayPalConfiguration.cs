using System.Text;

namespace PerAspera.Configurations
{
    public class PayPalConfiguration
    {
        public PayPalConfiguration(IConfiguration configuration)
        {
            ClientId = configuration.GetValue<string>("PayPal:ClientId");
            Secret = configuration.GetValue<string>("PayPal:Secret");
            IsDevMode = configuration.GetValue<bool>("PayPal:IsDevMode");
            CaptureOrderPaymentUrl = configuration.GetValue<string>("PayPal:CaptureOrderPaymentUrl");
            CancelOrderPaymentUrl = configuration.GetValue<string>("PayPal:CancelOrderPaymentUrl");
            BrandName = configuration.GetValue<string>("PayPal:BrandName");
            PaymentCurrency = configuration.GetValue<string>("PayPal:PaymentCurrency");
            ValidateConfigs();
        }

        public string ClientId { get; }
        public string Secret { get; }
        public bool IsDevMode { get; }
        public string CaptureOrderPaymentUrl { get; }
        public string CancelOrderPaymentUrl { get; }
        public string BrandName { get; }
        public string PaymentCurrency { get; }

        private void ValidateConfigs()
        {
            var builder = new StringBuilder();
            if (string.IsNullOrEmpty(ClientId)) builder.AppendLine("The client id is not configured.");
            if (string.IsNullOrEmpty(Secret)) builder.AppendLine("The secret is not configured.");
            if (string.IsNullOrEmpty(CaptureOrderPaymentUrl)) builder.AppendLine("The capture order url is not configured.");
            if (string.IsNullOrEmpty(CancelOrderPaymentUrl)) builder.AppendLine("The cancel order url is not configured.");

            if (builder.Length > 0) throw new InvalidOperationException($"Invalid PayPal configuration: {builder}");
        }

    }
}
