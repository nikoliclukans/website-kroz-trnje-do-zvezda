

using Examine;

using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;

using Lucene.Net.Search.Highlight;
using PerAspera.Extensions;
using PerAspera.Infrastructure.Search.Configurations;
using PerAspera.Infrastructure.Search.Models;
using PerAspera.Infrastructure.Search.Queries;
using static Lucene.Net.Util.Fst.Util;

namespace PerAspera.Infrastructure.Search
{
	public sealed class SearchHighlightService : ISearchHighlightService
	{
		private readonly IExamineManager _examineManager;
		private readonly ISiteSearchConfiguration _configuration;

		public SearchHighlightService(ISiteSearchConfiguration siteSearchConfiguration, IExamineManager examineManager)
		{
			_configuration = siteSearchConfiguration.ThrowIfNull();
			_examineManager = examineManager.ThrowIfNull();
		}

		public string GetHighlight(SearchTerm searchTerm, string highlightFieldValue)
		{
			if (string.IsNullOrWhiteSpace(highlightFieldValue))
			{
				throw new ArgumentException("Value is empty or null");
			}

			var highlighter = GetHighlighter(searchTerm);
			var tokenStream = GetTokenStream(highlightFieldValue);

			return highlighter.GetBestFragments(
					tokenStream,
					highlightFieldValue,
					_configuration.HighlightConfiguration.FragmentsCount,
					_configuration.HighlightConfiguration.FragmentsSeparator
				);
		}

		private Highlighter GetHighlighter(SearchTerm searchTerm)
		{
			var formatter = GetFormatter();
			var fragmentScorer = GetFragmentScorer(searchTerm);

			return new Highlighter(formatter, fragmentScorer)
			{
				TextFragmenter = new SimpleFragmenter(_configuration.HighlightConfiguration.FragmentSize)
			};
		}

		private TokenStream GetTokenStream(string highlightFieldValue)
			=> _configuration
				.Analyzer
				.GetTokenStream(_configuration.CombinedSearchFieldName, new StringReader(highlightFieldValue));

		private SimpleHTMLFormatter GetFormatter()
			=> new(
					$"<{_configuration.HighlightConfiguration.HtmlTag} class='{_configuration.HighlightConfiguration.HtmlTagCssClass}'>", 
					$"</{_configuration.HighlightConfiguration.HtmlTag}>"
				);

		private QueryScorer GetFragmentScorer(SearchTerm searchTerm)
		{
			var indexReader = GetIndexSearcher().IndexReader;

			return new QueryScorer(
				GetLuceneQueryObject(searchTerm)
				.Rewrite(indexReader)
			);
		}

		private Query GetLuceneQueryObject(SearchTerm searchTerm)
		{
            var queryParser = CreateQueryParser();
            var highlightedFieldQuery = new HighlightedFieldQuery(searchTerm, _configuration.CombinedSearchFieldName);

            return queryParser.Parse(highlightedFieldQuery.GetQuery());
        }

		private QueryParser CreateQueryParser()
			=> new(
				_configuration.LuceneVersion,
				_configuration.CombinedSearchFieldName,
				_configuration.Analyzer
				)
				{
					MultiTermRewriteMethod = MultiTermQuery.SCORING_BOOLEAN_QUERY_REWRITE
				};

		private IndexSearcher GetIndexSearcher()
		{
			var searcher = _examineManager.GetSearcherFromIndexName(_configuration.IndexName);

			return searcher.GetSearchContext().GetSearcher().IndexSearcher;
		}

	
	}

    public interface ISearchHighlightService
    {
        string GetHighlight(SearchTerm searchTerm, string highlightFieldValue);
    }


    public class HighlightedFieldQuery : ISearchQuery
    {
        private readonly SearchTerm _searchTerm;
        private readonly string _fieldName;

        public HighlightedFieldQuery(SearchTerm searchTerm, string fieldName)
        {
            _searchTerm = searchTerm;
            _fieldName = fieldName.ThrowIfNullOrWhiteSpace();
        }

        public ISearchQuery AppendQuery(ISearchQuery searchQuery)
            => new GroupedSearchQuery(this, searchQuery);

        public string GetQuery()
        {
            var allWordsQuery = $"{_fieldName}:{_searchTerm.Value}";
            var byWordsQuery = GetQueryByWords(_searchTerm, _fieldName);

            return $"+({allWordsQuery} {byWordsQuery})";
        }

        private static string GetQueryByWords(SearchTerm searchTerm, string field)
        {
            var words = searchTerm.Value.SplitByWords();

            const int minimumWordsCount = 2;
            if (words.Length < minimumWordsCount) return string.Empty;

            var wordsQuery = words.Select(word => $"{field}:{word}");

            return string.Join(" ", wordsQuery);
        }
    }
}
