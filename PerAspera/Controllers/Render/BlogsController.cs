using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using PerAspera.Extensions;
using PerAspera.Models.Generated;
using PerAspera.Models.ViewModels;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;

namespace PerAspera.Controllers.Render
{
	public class BlogsController : RenderController
	{
		public BlogsController(ILogger<RenderController> logger, ICompositeViewEngine compositeViewEngine, 
			IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine, umbracoContextAccessor)
		{
		}

		public override IActionResult Index()
		{
            var query = this.HttpContext.Request.Query;
			var category = query.GetUintParameter("category");
			var year = query.GetUintParameter("year");
			var currentPage = query.GetUintParameter("page") ?? 1;
			var maxItemPerPage = 9;
            var page = this.CurrentPage as Blogs;
			var blogsQuery = this.CurrentPage.Children<BlogPage>();
			if(category != null)
			{
				blogsQuery = blogsQuery.Where(blog => category.HasValue && blog.Category?.Id == category);
            }

            if (year != null)
            {
                blogsQuery = blogsQuery.Where(blog =>blog.CreateDate.Year == year);
            }
			page.CurrentCategory =category;
			page.CurrentYear = year;
			page.BlogList = new PaginatedCollectionViewModel<BlogPage>(blogsQuery.Skip((Convert.ToInt32(currentPage) - 1) * maxItemPerPage).Take(maxItemPerPage),
				(uint)this.CurrentPage.Children<BlogPage>().Count(),(uint)maxItemPerPage, currentPage);


            return CurrentTemplate(page);
		}
	}
}
