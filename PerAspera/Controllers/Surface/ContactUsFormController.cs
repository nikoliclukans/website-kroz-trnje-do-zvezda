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
		private readonly IConfiguration _configuration;

		public ContactUsFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, 
			ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, 
			IPublishedUrlProvider publishedUrlProvider, IEmailService emailService,IConfiguration configuration) 
			: base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
		{
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));
			_emailService = emailService;
			_configuration = configuration;
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Contact(ContactUsFormDto mode)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			var from = _configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:From");
			var to = _configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:To");

            //Send email
            var message = @$"
				Ime: {mode.Name}
				Prezime: {mode.Surname}
				Email adresa: {mode.Email}
				Poruka: {mode.Message}";

			_emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage(from,to, "Kontaktirajte nas",message, false),new ContactUsEmailTemplate(message));

			return Ok();
		}
	}
}
