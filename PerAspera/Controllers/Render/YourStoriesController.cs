using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using PerAspera.Extensions;
using PerAspera.Models.Generated;
using PerAspera.Models.ViewModels;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace PerAspera.Controllers.Render
{
    public class YourStoriesController : RenderController
    {
        public YourStoriesController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }

        public override IActionResult Index()
        {
            var query = this.HttpContext.Request.Query;
            var currentPage = query.GetUintParameter("page") ?? 1;
            var page = this.CurrentPage as YourStories;
            var storiesQuery = page.Children<YourStoriesItem>();
            var stories = storiesQuery.Skip((Convert.ToInt32(currentPage) - 1) * page.ItemsPerPage).Take(page.ItemsPerPage);
            page.Stories = new PaginatedCollectionViewModel<YourStoriesItem>(stories, (uint)storiesQuery.Count(), (uint)page.ItemsPerPage, currentPage);

            return CurrentTemplate(page);
        }
    }
}
