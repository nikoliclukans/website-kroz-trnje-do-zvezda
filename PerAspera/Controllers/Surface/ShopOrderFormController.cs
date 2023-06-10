using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;
using PerAspera.Infrastructure.Configuration;
using PerAspera.Infrastructure.Interfaces;
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
    public class ShopOrderFormController : SurfaceController
    {
        private readonly IEmailService _emailService;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly IPaymentService _paymentService;
        private readonly IAppPolicyCache _runTimeCache;
        private readonly IConfiguration _config;

        public ShopOrderFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider, IEmailService emailService,
            SmtpConfiguration smtpConfiguration, IPaymentService paymentService, AppCaches appCache, IConfiguration config)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            _emailService = emailService;
            _smtpConfiguration = smtpConfiguration;
            _paymentService = paymentService;
            _runTimeCache = appCache.RuntimeCache;
            _config = config;
        }

        [ValidateReCaptcha]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Order(ShopOrderDto shopOrderDto)
        {
            if (shopOrderDto.OrderItems.Count == 0)
            {
                return BadRequest();
            }

            if (shopOrderDto.SelectedPaymentOption == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (shopOrderDto.SelectedPaymentOption == "PayPal")
            {
                var purchaseKey = Guid.NewGuid().ToString();

                var exchangeRate = _config.GetValue<decimal>("PayPal:ExchangeRate");

                var priceInEuros = shopOrderDto.TotalPrice / exchangeRate;

                shopOrderDto.TotalPrice = Math.Round(priceInEuros, 2);

                _runTimeCache.InsertCacheItem<ShopOrderDto>(purchaseKey, () => shopOrderDto, TimeSpan.FromMinutes(15));

                return Redirect($"/umbraco/surface/order/create?key={purchaseKey}&price={shopOrderDto.TotalPrice}");
            }
            else
            {
                var orderdItems = "";

                foreach (var orderedItem in shopOrderDto.OrderItems)
                {
                    orderdItems += "Artikal - " + orderedItem.Name + " Kolicina - " + orderedItem.Quantity + " Cena - " + orderedItem.Price + ";";
                }

                var message = @$"
                Ime: {shopOrderDto.Name}
				Prezime: {shopOrderDto.Surename}
                Email: {shopOrderDto.Email}
				Adresa za slanje: {shopOrderDto.Address}
				Broj telefona: {shopOrderDto.PhoneNumber}
				Porudzbina: {orderdItems}
                Poruka: {shopOrderDto.Message}
                Ukupna cena: {shopOrderDto.TotalPrice}
				Način plaćanja: {shopOrderDto.SelectedPaymentOption}
				";

                //_emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage(_smtpConfiguration.From, _smtpConfiguration.To,
                //    $"Porudzbina od strane {shopOrderDto.Name} {shopOrderDto.Surename}", message, false), new ContactUsEmailTemplate(message));

                return Ok();
            }
        }
    }
}
