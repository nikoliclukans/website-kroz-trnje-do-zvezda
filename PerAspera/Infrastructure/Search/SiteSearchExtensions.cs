
using Examine;
using PerAspera.Extensions;
using PerAspera.Infrastructure.Search.Models;


using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;

namespace PerAspera.Infrastructure.Search
{
    public static class SiteSearchExtensions
    {
        public static string[] GetSearchableFieldsValue(this ValueSet source, string[] searchableFields)
            => source
                .Values
                .Where(value => searchableFields.Contains(value.Key))
                .SelectMany(value => value.Value.Select(v => v.ToString()))
                .ToArray();

        public static IEnumerable<SearchResultItem> ToSearchResultItems(
            this IEnumerable<ISearchResult> source,
            IUmbracoContext nodeService)
        {
            _ = nodeService.ThrowIfNull();

            return GetItems();

            IEnumerable<SearchResultItem> GetItems()
            {
                if (source is null) yield break;

                foreach (var item in source)
                {
                    _ = int.TryParse(item.Id, out int contentId);
                    var contentResult = nodeService.Content.GetById(contentId);
                    if (contentResult != null)
                    {
                        yield return new SearchResultItem
                        {
                            Content = contentResult,
                            SearchResult = item
                        };
                    }
                    else
                    {
                        yield return new SearchResultItem
                        {
                            Content = null,
                            SearchResult = null
                        };
                    }

                }
            }
        }
    }
}
