using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;
using PerAspera.Infrastructure.Configuration;
using PerAspera.Infrastructure.Implementation;
using PerAspera.Infrastructure.Interfaces;
using PerAspera.Models.ViewModels;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Models.PublishedContent;
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

		public ShopOrderFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory,
			ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger,
			IPublishedUrlProvider publishedUrlProvider, IEmailService emailService,
			SmtpConfiguration smtpConfiguration)
			: base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{
			_emailService = emailService;
			_smtpConfiguration = smtpConfiguration;
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

			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var orderdItems = "";

			foreach (var orderedItem in shopOrderDto.OrderItems)
			{
				orderdItems += "Artikal - " + orderedItem.Name + " Kolicina - " + orderedItem.Quantity + " Cena - " + orderedItem.Price + ";";
			}


			//if (!Guid.TryParse(shopOrderDto.OrderItem, out Guid orderItemKey) 
			//	|| !TryGetNode(orderItemKey, out IPublishedContent orderItem))
			//{
			//	return BadRequest();
			//}

			var message = @$"
                Ime: {shopOrderDto.Name}
				Prezime: {shopOrderDto.Surename}
                Email: {shopOrderDto.Email}
				Adresa za slanje: {shopOrderDto.Address}
				Broj telefona: {shopOrderDto.PhoneNumber}
				Porudzbina: {orderdItems}
                Poruka: {shopOrderDto.Message}
                Ukupna cena: {shopOrderDto.TotalPrice}
				";

			_emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage(_smtpConfiguration.From, _smtpConfiguration.To,
				$"Porudzbina od strane {shopOrderDto.Name} {shopOrderDto.Surename}", message, false), new ContactUsEmailTemplate(message));

			return Ok();
		}

		public bool TryGetNode(Guid nodeKey, out IPublishedContent node)
		{
			node = UmbracoContextAccessor.GetRequiredUmbracoContext()
				   .Content?
				   .GetById(nodeKey);
			return node != null;
		}

	}
}
