using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PerAspera.Configurations;
using PerAspera.Models.DTOs;
using PerAspera.Services.Interfaces;
using Umbraco.Cms.Core.Web;
using HttpRequest = PayPalHttp.HttpRequest;

namespace PerAspera.Services.Implementation
{
    public class PayPalService : IPaymentService
    {
        private readonly PayPalClientFactory _payPalClientFactory;
        private readonly PayPalConfiguration _configuration;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public PayPalService(PayPalClientFactory payPalClientFactory, PayPalConfiguration configuration, IUmbracoContextAccessor umbracoContextAccessor)
        {
            _payPalClientFactory = payPalClientFactory;
            _configuration = configuration;
            _umbracoContextAccessor = umbracoContextAccessor;
        }
        public async Task<CreateOrderResponse> CreateOrder(Guid key, string price)
        {
            var orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    BrandName = _configuration.BrandName,
                    LandingPage = "BILLING",
                    CancelUrl = string.Format(_configuration.CancelOrderPaymentUrl, key),
                    ReturnUrl = string.Format(_configuration.CaptureOrderPaymentUrl, key),
                    UserAction = "PAY_NOW",
                    ShippingPreference = "NO_SHIPPING"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId =  $"REF-{Guid.NewGuid()}",
                        CustomId = $"CUST-{Guid.NewGuid()}",
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = _configuration.PaymentCurrency,
                            Value = price,
                            AmountBreakdown = new AmountBreakdown
                            {
                                ItemTotal = new PayPalCheckoutSdk.Orders.Money
                                {
                                    CurrencyCode = _configuration.PaymentCurrency,
                                    Value = price
                                }
                            }
                        }
                    }
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(orderRequest);

            var result = await ExecuteRequest<OrdersCreateRequest, Order>(request);


            return new CreateOrderResponse
            {
                OrderId = result.Id,
                ApproveUrl = result.Links.Single(link => link.Rel.Equals("approve")).Href,
                Status = Enum.Parse<OrderPaymentStatus>(result.Status)
            };
        }
        public async Task<CaptureOrderPaymentResponse> CaptureOrder(string token)
        {
            var request = new OrdersCaptureRequest(token);
            request.Prefer("return=representation");
            request.RequestBody(new OrderActionRequest());

            var result = await ExecuteRequest<OrdersCaptureRequest, Order>(request);

            return new CaptureOrderPaymentResponse
            {
                OrderId = result.Id,
                CaptureId = result.PurchaseUnits[0].Payments.Captures[0].Id,
                Status = Enum.Parse<OrderPaymentStatus>(result.Status),
                Token = token
            };
        }

        public async Task<RefundPaymentResponse> RefundPayment(string captureId)
        {
            var request = new CapturesRefundRequest(captureId);
            request.Prefer("return=representation");
            request.RequestBody(new RefundRequest());

            var result = await ExecuteRequest<CapturesRefundRequest, PayPalCheckoutSdk.Payments.Refund>(request);

            return new RefundPaymentResponse
            {
                Id = result.Id,
                Status = Enum.Parse<OrderPaymentStatus>(result.Status)
            };
        }

        private async Task<TResponse> ExecuteRequest<TRequest, TResponse>(TRequest request)
            where TRequest : HttpRequest
        {
            var response = await _payPalClientFactory.Create().Execute(request);
            var result = response.Result<TResponse>();

            return result;
        }
    }
}
