using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using PerAspera.Extensions;
using PerAspera.Models.Generated;
using PerAspera.Models.ViewModels;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace PerAspera.Controllers.Render
{
    public class ShopController : RenderController
    {
        public ShopController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
        }

        public override IActionResult Index()
        {
            var query = this.HttpContext.Request.Query;
            var currentPage = query.GetUintParameter("page") ?? 1;
            var maxItemPerPage = 500;
            var page = this.CurrentPage as Shop;
            var shopQuery = this.CurrentPage.Children<ShopItem>();

            page.ShopItemList = new PaginatedCollectionViewModel<ShopItem>(shopQuery.Skip((Convert.ToInt32(currentPage) - 1) * maxItemPerPage).Take(maxItemPerPage),
                (uint)shopQuery.Count(), (uint)maxItemPerPage, currentPage);

            return CurrentTemplate(page);
        }
    }
}
