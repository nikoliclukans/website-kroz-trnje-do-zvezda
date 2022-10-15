using Examine.Search;
using Examine;
using PerAspera.Extensions;
using System.Text;
using CSharpFunctionalExtensions;
using PerAspera.Infrastructure.Search.Models;

namespace PerAspera.Infrastructure.Search.Queries
{
    public interface ISearchQuery
    {
        string GetQuery();
        ISearchQuery AppendQuery(ISearchQuery searchQuery);
    }

    public class SearchQuery : ISearchQuery
    {
        private readonly SearchTerm _searchTerm;
        private readonly string[] _searchFields;

        public SearchQuery(SearchTerm searchTerm, string[] searchFields)
        {
            _searchTerm = searchTerm;
            _searchFields = searchFields.ThrowIfNull();
        }

        public string GetQuery()
        {
            const string hideFromSiteSearchFieldAlias = "hideFromSiteSearch";

            IExamineValue wholeExamineValue = CreateBoostValue(_searchTerm.Value);
            IExamineValue[] wordsExamineValues = SplitQueryByWords(_searchTerm.Value);

            var builder = new StringBuilder();
            builder.Append("+((");
            builder.Append(string.Join(" ", _searchFields.Select(f => $"{f}:\"{wholeExamineValue.Value}\"")));
            builder.Append(")(");
            builder.Append(string.Join(" ", _searchFields.SelectMany(f => wordsExamineValues.Select(w => $"{f}:{w.Value}"))));
            builder.Append("))");

            return builder.ToString();
        }

        public ISearchQuery AppendQuery(ISearchQuery searchQuery)
            => new GroupedSearchQuery(this, searchQuery);

        private static IExamineValue CreateBoostValue(string query)
        {
            const int highBoostValue = 4;

            return query.Boost(highBoostValue);
        }

        private static IExamineValue[] SplitQueryByWords(string query)
            => query
                .SplitByWords()
                .Select(w => w.Escape())
                .ToArray();
    }


    public class GroupedSearchQuery : ISearchQuery
    {
        private readonly ISearchQuery _firstQuery;
        private readonly ISearchQuery _secondQuery;

        public GroupedSearchQuery(ISearchQuery firstQuery, ISearchQuery secondQuery)
        {
            _firstQuery = firstQuery.ThrowIfNull();
            _secondQuery = secondQuery.ThrowIfNull();
        }

        public ISearchQuery AppendQuery(ISearchQuery searchQuery)
            => new GroupedSearchQuery(this, searchQuery);

        public string GetQuery()
            => $"{_firstQuery.GetQuery()} {_secondQuery.GetQuery()}";
    }
}
