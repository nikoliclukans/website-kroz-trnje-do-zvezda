using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;
using PerAspera.Infrastructure.Configuration;
using PerAspera.Infrastructure.Implementation;
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
            
            var orderdItems = "";

            foreach (var orderedItem in shopOrderDto.OrderItems)
            {
                orderdItems += "Artikal - " + orderedItem.Name + " Kolicina - " + orderedItem.Quantity + " Cena - " + orderedItem.Price + ";";
            }

			string paymentMethod = shopOrderDto.SelectedPaymentOption == "CashOnDelivery" ? "Pouzećem" : "Kartica";

			var message = $@"
<html>
<body>
    <table border='1' style='border-collapse: collapse;'>
        <tr>
            <th style='text-align: left;'>Ime</th>
            <td>{shopOrderDto.Name}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Prezime</th>
            <td>{shopOrderDto.Surename}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Email</th>
            <td>{shopOrderDto.Email}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Adresa za slanje</th>
            <td>{shopOrderDto.Address}, {shopOrderDto.City}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Broj telefona</th>
            <td>{shopOrderDto.PhoneNumber}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Porudžbina</th>
            <td>{orderdItems}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Poruka</th>
            <td>{shopOrderDto.Message}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Ukupna cena</th>
            <td>{shopOrderDto.TotalPrice}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Način plaćanja</th>
            <td>{paymentMethod}</td>
        </tr>
    </table>
</body>
</html>";

			var messageReply = $@"
<html>
<body>
    <p><strong>Hvala vam što ste poručili sa našeg sajta!</strong></p>

    <p>Vaša porudžbina je uspešno primljena i možete je očekivati u roku od 1-2 radna dana. Pakete šaljemo Post Expressom. Hvala Vam na poverenju i što ste odabrali naše proizvode koji će Vam pomoći da promenite Vaš životni stil!</p>

    <p>S poštovanjem,</p>
    <p>Nastasja & Nemanja</p>

    <table border='1' style='border-collapse: collapse;'>
        <tr>
            <th style='text-align: left;'>Ime</th>
            <td>{shopOrderDto.Name}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Prezime</th>
            <td>{shopOrderDto.Surename}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Email</th>
            <td>{shopOrderDto.Email}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Adresa za slanje</th>
            <td>{shopOrderDto.Address}, {shopOrderDto.City}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Broj telefona</th>
            <td>{shopOrderDto.PhoneNumber}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Porudžbina</th>
            <td>{orderdItems}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Poruka</th>
            <td>{shopOrderDto.Message}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Ukupna cena</th>
            <td>{shopOrderDto.TotalPrice}</td>
        </tr>
        <tr>
            <th style='text-align: left;'>Način plaćanja</th>
            <td>{paymentMethod}</td>
        </tr>
    </table>
</body>
</html>";



			_emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage(_smtpConfiguration.From, _smtpConfiguration.To,
                $"Porudžbina od strane {shopOrderDto.Name} {shopOrderDto.Surename}", message, true), new ContactUsEmailTemplate(message));

            _emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage("cancerinfluencer.org", shopOrderDto.Email,
                $"Vaša porudžbina sa cancerinfluencer.org je primljena!\r\n", messageReply, true), new ContactUsEmailTemplate(messageReply));

            return Ok();
        }
    }
}
