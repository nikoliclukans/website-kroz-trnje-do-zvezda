using Examine;
using Examine.Lucene.Providers;

using System;

using Umbraco.Cms.Infrastructure.Examine;

namespace PerAspera.Infrastructure.Search
{
	public static class ExamineManagerExtensions
	{
		public static BaseLuceneSearcher GetSearcherFromIndexName(this IExamineManager examineManager, string indexName)
		{
			var index = examineManager.GetIndex(indexName);

			return (BaseLuceneSearcher)index.Searcher;
		}

		public static UmbracoExamineIndex GetIndex(this IExamineManager examineManager, string indexName)
		{
			if(!examineManager.TryGetIndex(indexName, out IIndex index))
			{
				throw new ArgumentException($"Could not find index by name '{indexName}'.");
			}

			return (UmbracoExamineIndex)index;
		}
	}
}
