using Microsoft.AspNetCore.Mvc;
using PerAspera.Infrastructure.Configuration;
using PerAspera.Infrastructure.Interfaces;
using PerAspera.Models.DTOs;
using PerAspera.Models.ViewModels;
using PerAspera.Services.Interfaces;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace PerAspera.Controllers.Surface
{
    public class OrderController : SurfaceController
    {
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly IAppPolicyCache _runTimeCache;

        public OrderController(IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IPaymentService paymentService,
            IEmailService emailService,
            SmtpConfiguration smtpConfiguration) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _paymentService = paymentService;
            _emailService = emailService;
            _smtpConfiguration = smtpConfiguration;
            _runTimeCache = appCaches.RuntimeCache;
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid key, string price)
        {
            if (price is null) return BadRequest();

            var response = await _paymentService.CreateOrder(key, price);

            return Redirect(response.ApproveUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Capture(string token, string key)
        {
            var shopOrderDto = _runTimeCache.Get(key) as ShopOrderDto;
            var response = await _paymentService.CaptureOrder(token);

            try
            {
                var orderedItems = "";

                foreach (var orderedItem in shopOrderDto.OrderItems)
                {
                    orderedItems += "Artikal - " + orderedItem.Name + " Kolicina - " + orderedItem.Quantity + " Cena - " + orderedItem.Price + ";";
                }

                var message = @$"
                Ime: {shopOrderDto.Name}
				Prezime: {shopOrderDto.Surename}
                Email: {shopOrderDto.Email}
				Adresa za slanje: {shopOrderDto.Address}
				Broj telefona: {shopOrderDto.PhoneNumber}
				Porudzbina: {orderedItems}
                Poruka: {shopOrderDto.Message}
                Ukupna cena: {shopOrderDto.TotalPrice}
                Način plaćanja: {shopOrderDto.SelectedPaymentOption}
				";

                //_emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage(_smtpConfiguration.From, _smtpConfiguration.To,
                //$"Porudzbina od strane {shopOrderDto.Name} {shopOrderDto.Surename}", message, false), new ContactUsEmailTemplate(message));

                return Redirect($"/shop?status={response.Status}");
            }
            catch (Exception ex)
            {
                await _paymentService.RefundPayment(response.CaptureId);
            }
            return Redirect($"/shop?status={OrderPaymentStatus.FAILED}");
        }

        [HttpGet]
        public IActionResult Cancel(string key)
        {
            if (!Guid.TryParse(key, out Guid keyGuid)) return BadRequest();

            _runTimeCache.Clear(key);

            return Redirect($"/shop?status={OrderPaymentStatus.CANCELLED}");
        }
    }
}
