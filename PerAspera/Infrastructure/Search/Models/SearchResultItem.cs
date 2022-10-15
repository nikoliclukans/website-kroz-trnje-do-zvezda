using Examine;

using Umbraco.Cms.Core.Models.PublishedContent;

namespace PerAspera.Infrastructure.Search.Models
{
	public class SearchResultItem
	{
		public IPublishedContent Content { get; set; }
		public ISearchResult SearchResult { get; set; }
	}
}
