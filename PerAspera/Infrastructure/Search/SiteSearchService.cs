
using CSharpFunctionalExtensions;

using Examine;
using Examine.Lucene.Providers;
using Examine.Search;
using PerAspera.Extensions;
using PerAspera.Infrastructure.Search.Configurations;
using PerAspera.Infrastructure.Search.Models;
using PerAspera.Infrastructure.Search.Queries;
using PerAspera.Models.Generated;
using PerAspera.Models.ViewModels;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;


namespace PerAspera.Infrastructure.Search
{
	public sealed class SiteSearchService 
	{
		private readonly IExamineManager _examineManager;
		private readonly BaseLuceneSearcher _searcher;
		private readonly IUmbracoContextAccessor _contextAccessor;
		private readonly ISearchHighlightService _searchHighlightService;

		public ISiteSearchConfiguration Configuration { get; }

		public SiteSearchService(ISiteSearchConfiguration siteSearchConfiguration,
			IExamineManager examineManager,
			IUmbracoContextAccessor contextAccessor, ISearchHighlightService searchHighlightService)
		{
			Configuration = siteSearchConfiguration.ThrowIfNull();
			_examineManager = examineManager.ThrowIfNull();
			_searcher = _examineManager.GetSearcherFromIndexName(Configuration.IndexName);
			_contextAccessor = contextAccessor;
			_searchHighlightService = searchHighlightService;
		}

		public Result<SearchResultsPerPageViewModel> Search(
			ISearchQuery query,
			SearchTerm searchTerm,
			PaginationRequest paginationRequest)
		{
			if (query is null)
			{
				return CreateFailure("Could not get search results, the query is null.");
			}

			var luceneQuery = _searcher
								.CreateQuery()
								.NativeQuery(query.GetQuery());

			var resultsToSkip = (paginationRequest.Page - 1) * paginationRequest.ItemsPerPage;
			var queryOptions = new QueryOptions(resultsToSkip, paginationRequest.ItemsPerPage);
			var searchResults = luceneQuery.Execute(queryOptions);
			if(_contextAccessor.TryGetUmbracoContext(out var context))
			{
                var searchResultsPerPage = searchResults.ToSearchResultItems(context).ToList();

				return new SearchResultsPerPageViewModel
				{
					TotalResults = searchResults.TotalItemCount,
					Items = searchResultsPerPage.Select(item => new SearchResultsItemViewModel
					{
						Url = item.Content.Url(),
						Text = item.SearchResult.Values["content"],
                        Title = GetTitle(item.Content)

                    }).ToList()
                };
            }
            return new SearchResultsPerPageViewModel
            {
                TotalResults = 0
            };

        }

		private static Result<SearchResultsPerPageViewModel> CreateFailure(string errorMessage)
			=> Result.Failure<SearchResultsPerPageViewModel>(errorMessage);

        private static string GetTitle(IPublishedContent content)
        {
            if (content is not IPageBase page) return content.Name;

            return page.Title;
        }
    }
}
