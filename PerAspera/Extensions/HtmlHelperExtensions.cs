using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace PerAspera.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static Task<IHtmlContent> NestedContent(this IHtmlHelper<dynamic> source, IPublishedElement model, ViewDataDictionary viewData)
        {
            if (model == null) return null;

            string viewName = model.GetType().Name;
            return source.PartialAsync($"~/Views/Partials/NestedContent/_{viewName}.cshtml", model, viewData);
        }
    }
}
