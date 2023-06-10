using PerAspera.Configurations;
using PerAspera.Infrastructure.Configuration;
using PerAspera.Infrastructure.Implementation;
using PerAspera.Infrastructure.Interfaces;
using PerAspera.Services.Implementation;
using PerAspera.Services.Interfaces;

namespace PerAspera.Extensions
{
	public static class UmbracoBuilderExtensions
	{
		public static IUmbracoBuilder AddCustomServices(this IUmbracoBuilder umbracoBuilder)
		{
			umbracoBuilder.Services.AddSingleton<SmtpConfiguration>();
			umbracoBuilder.Services.AddSingleton<IEmailService, SmtpEmailService>();
			umbracoBuilder.Services.AddSingleton<IPaymentService, PayPalService>();
			umbracoBuilder.Services.AddSingleton<PayPalClientFactory>();
			umbracoBuilder.Services.AddSingleton<PayPalConfiguration>();
			return umbracoBuilder;
		}
	}
}
