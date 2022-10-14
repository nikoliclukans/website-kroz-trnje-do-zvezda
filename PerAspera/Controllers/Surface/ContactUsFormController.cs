using Microsoft.AspNetCore.Mvc;
using PerAspera.Infrastructure.Implementation;
using PerAspera.Infrastructure.Interfaces;
using PerAspera.Models.Generated;
using PerAspera.Models.ViewModels;
using System.Text;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace PerAspera.Controllers.Surface
{
	public class ContactUsFormController : SurfaceController
	{
		private readonly IEmailService _emailService;

		public ContactUsFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, 
			ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, 
			IPublishedUrlProvider publishedUrlProvider, IEmailService emailService) 
			: base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{
			_emailService = emailService;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Contact(ContactUsFormDto mode)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			//Send email
			var message = $"Ime: {mode.Name}\n" +
				$"Prezime: {mode.Surname}\n" +
				$"Email adresa: {mode.Email}\n" +
				$"Poruka: {mode.Message}";

			_emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage("dev@vegait.rs", "andrej.iviciak@vegait.rs", "Kontaktirajte nas", message, false),new ContactUsEmailTemplate(message));

			return Ok();
		}
	}
}
