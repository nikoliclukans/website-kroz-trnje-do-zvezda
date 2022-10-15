using Microsoft.AspNetCore.Mvc;
using PerAspera.Extensions;
using PerAspera.Infrastructure.Implementation;
using PerAspera.Infrastructure.Interfaces;
using PerAspera.Models.Generated;
using PerAspera.Models.ViewModels;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace PerAspera.Controllers.Surface
{
    public class YourStoryFormController : SurfaceController
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public YourStoryFormController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, 
            IPublishedUrlProvider publishedUrlProvider, IEmailService emailService, IConfiguration configuration) 
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider) 
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
            if (emailService == null) throw new ArgumentNullException(nameof(emailService));
            _emailService = emailService;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult YourStory(YourStoryFormDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var from = _configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:From");
            var to = _configuration.GetValue<string>("Umbraco:CMS:Global:Smtp:To");

            var message = @$"
                Ime: {model.Name}
                Email: {model.Email} 
                Moja prica: {model.Message}";

            _emailService.Send(new Umbraco.Cms.Core.Models.Email.EmailMessage(from, to, "Moja prica", message, false), new ContactUsEmailTemplate(message));

            return Ok();
        }
    }
}
