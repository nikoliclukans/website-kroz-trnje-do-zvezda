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
            var searchTermResult = SearchTerm.Create(search);

            if (searchTermResult.IsSuccess)
            {
                var searchQuery = new SearchQuery(searchTermResult.Value, _siteSearch.Configuration.SearchFields);
                const int pageNumber = 1;
                const int itemsPerPage = 10;
                var paginationRequestResult = PaginationRequest.Create(pageNumber, itemsPerPage);
                var result = _siteSearch.Search(searchQuery, searchTermResult.Value, paginationRequestResult);
            }


            var page = this.CurrentPage as Search;

            return CurrentTemplate(page);
        }
    }
}
