using Lucene.Net.Index;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using PerAspera.Extensions;
using PerAspera.Infrastructure.Search;
using PerAspera.Infrastructure.Search.Models;
using PerAspera.Infrastructure.Search.Queries;
using PerAspera.Models.Generated;
using PerAspera.Models.ViewModels;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace PerAspera.Controllers.Render
{
    public class SearchController : RenderController
    {
        private readonly SiteSearchService _siteSearch;

        public SearchController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor, SiteSearchService siteSearch) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _siteSearch = siteSearch;
        }

        public override IActionResult Index()
        {
            var query = this.HttpContext.Request.Query;
            var search = query.GetStringParameter("query");
            var pageNumber = query.GetUintParameter("page");
            var searchTermResult = SearchTerm.Create(search);
            var page = this.CurrentPage as Search;
            page.Query = search;
            if (searchTermResult.IsSuccess)
            {
                var searchQuery = new SearchQuery(searchTermResult.Value, _siteSearch.Configuration.SearchFields);
                int itemsPerPage =(int) page.ItemsPerPage;
                var paginationRequestResult = PaginationRequest.Create(Convert.ToInt32(pageNumber), itemsPerPage);
                var result = _siteSearch.Search(searchQuery, searchTermResult.Value, paginationRequestResult);
                page.SearchItems = new PaginatedCollectionViewModel<SearchResultsItemViewModel>(result.Value.Items,
                    (uint)result.Value.TotalResults, (uint)itemsPerPage, pageNumber ?? 1);
            }           



            return CurrentTemplate(page);
        }
    }
}
