using Examine;

using Microsoft.Extensions.DependencyInjection;
using PerAspera.Infrastructure.Search.Configurations;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;
//PerAspera.Infrastructure.Search
namespace PerAspera.Infrastructure.Search.Indexes
{
	public sealed class IndexesComposer : IComposer
	{
		public void Compose(IUmbracoBuilder builder)
		{
            builder.Services
               .AddSingleton<ISiteSearchConfiguration, SiteSearchConfiguration>()
               .AddTransient<ISearchHighlightService, SearchHighlightService>()
               .AddTransient<SiteSearchService>();

            var configuration = builder
									.Services
									.BuildServiceProvider()
									.GetRequiredService<ISiteSearchConfiguration>();

			_ = builder.Services.AddExamineLuceneIndex<SiteSearchIndex, ConfigurationEnabledDirectoryFactory>(configuration.IndexName);
		}
	}
}
