using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using PerAspera.Infrastructure.Configuration;
using PerAspera.Infrastructure.Implementation;
using PerAspera.Infrastructure.Interfaces;

namespace PerAspera.Extensions
{
	public static class UmbracoBuilderExtensions
	{
		public static IUmbracoBuilder AddCustomServices(this IUmbracoBuilder umbracoBuilder)
		{
			umbracoBuilder.Services.AddSingleton<SmtpConfiguration>();
            umbracoBuilder.Services.AddSingleton<IEmailService, SmtpEmailService>();
			return umbracoBuilder;
		}
    }
}
