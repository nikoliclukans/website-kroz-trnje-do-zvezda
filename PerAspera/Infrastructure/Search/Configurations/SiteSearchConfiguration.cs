using Examine.Search;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;

using Microsoft.Extensions.Configuration;
using PerAspera.Extensions;
using PerAspera.Models.Generated;
using System;
using System.Collections.Generic;

namespace PerAspera.Infrastructure.Search.Configurations
{
	public sealed class SiteSearchConfiguration : ISiteSearchConfiguration
	{
		private static readonly LuceneVersion LuceneVersionStatic = LuceneVersion.LUCENE_48;

		private readonly Dictionary<string, Lazy<object>> _lazyDictionary;

		public SiteSearchConfiguration(IConfiguration configuration)
		{
			configuration.ThrowIfNull();
			_lazyDictionary = new()
			{
				{ nameof(IndexName), new Lazy<object>(() => "SiteSearch") },
				{ nameof(SearchFields), new Lazy<object>(() => new[] { "title", "text", "content" }) },
				{ nameof(DefaultSearchOperation), new Lazy<object>(() => BooleanOperation.And) },
				{ nameof(FolderName), new Lazy<object>(() => "SiteSearch") },
				{ nameof(Analyzer), new Lazy<object>(() => new StandardAnalyzer(LuceneVersionStatic)) },
				{
					nameof(SearchableDocumentTypes),
					new Lazy<object>(() => new[] {
										Home.ModelTypeAlias,
										AboutUs.ModelTypeAlias,
										YourStories.ModelTypeAlias,
										Shop.ModelTypeAlias,
										BlogPage.ModelTypeAlias,
										Blogs.ModelTypeAlias
									})
				},
				{ nameof(JsonSearchableFields), new Lazy<object>(() => new string[] { "content" }) },
				{ nameof(CombinedSearchFieldName), new Lazy<object>(() => "combinedFieldsContent") },
				{ nameof(HighlightConfiguration), new Lazy<object>(() => new SearchHighlightConfiguration(configuration)) }
			};
		}
		
		public string IndexName => (string)_lazyDictionary[nameof(IndexName)].Value;

		public string[] SearchFields => (string[])_lazyDictionary[nameof(SearchFields)].Value;

		public BooleanOperation DefaultSearchOperation 
				=> (BooleanOperation)_lazyDictionary[nameof(DefaultSearchOperation)].Value;

		public string FolderName => (string)_lazyDictionary[nameof(FolderName)].Value;

		public LuceneVersion LuceneVersion => LuceneVersionStatic;

		public Analyzer Analyzer => (Analyzer)_lazyDictionary[nameof(Analyzer)].Value;

		public string[] SearchableDocumentTypes => (string[])_lazyDictionary[nameof(SearchableDocumentTypes)].Value;

		public string[] JsonSearchableFields => (string[])_lazyDictionary[nameof(JsonSearchableFields)].Value;

		public string CombinedSearchFieldName => (string)_lazyDictionary[nameof(CombinedSearchFieldName)].Value;

		public ISearchHighlightConfiguration HighlightConfiguration 
				=> (ISearchHighlightConfiguration)_lazyDictionary[nameof(HighlightConfiguration)].Value;
	}
}
