using Examine.Search;

using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace PerAspera.Infrastructure.Search.Configurations
{
	public interface ISiteSearchConfiguration
	{
		string IndexName { get; }
		string FolderName { get; }
		LuceneVersion LuceneVersion { get; }
		Analyzer Analyzer { get; }
		string[] SearchFields { get; }
		string[] SearchableDocumentTypes { get; }
		string[] JsonSearchableFields { get; }
		string CombinedSearchFieldName { get; }
		ISearchHighlightConfiguration HighlightConfiguration { get; }
		BooleanOperation DefaultSearchOperation { get; }
	}
}
