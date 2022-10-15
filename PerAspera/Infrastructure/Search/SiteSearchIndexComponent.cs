using Examine;
using PerAspera.Infrastructure.Search.Builders;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Infrastructure.Examine;
using PerAspera.Infrastructure.Search.Indexes;
using PerAspera.Infrastructure.Search.Configurations;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using static Lucene.Net.Queries.Function.ValueSources.MultiFunction;

namespace PerAspera.Infrastructure.Search
{
	public sealed class SiteSearchIndexComponent : IComponent
	{
		private readonly IExamineManager _examineManager;
		private readonly ISiteSearchConfiguration _siteSearchConfiguration;
		private SiteSearchIndex _siteSearchIndex;

		public SiteSearchIndexComponent(IExamineManager examineManager, 
			ISiteSearchConfiguration siteSearchConfiguration)
		{
			_examineManager = examineManager;
			_siteSearchConfiguration = siteSearchConfiguration;
		}

		public void Initialize()
		{
			_ = _examineManager.TryGetIndex(_siteSearchConfiguration.IndexName, out IIndex index);
			_siteSearchIndex = (SiteSearchIndex)index;

			_siteSearchIndex.TransformingIndexValues += SiteSearchIndex_TransformingIndexValues;
		}

		public void Terminate()
		{
			_siteSearchIndex.TransformingIndexValues -= SiteSearchIndex_TransformingIndexValues;
		}

		private void SiteSearchIndex_TransformingIndexValues(object sender, IndexingItemEventArgs e)
		{
			if (e.ValueSet.Category != IndexTypes.Content) return;

			var values = e.ValueSet.GetSearchableFieldsValue(_siteSearchConfiguration.SearchFields);

			var combinedFieldsValue = CombinedFieldsContentBuilder
				.Initialize()
			.WithValues(values)
			.NoMoreValues()
				.Build();

			var dictionary = e.ValueSet.Values.ToDictionary(k => k.Key, v => v.Value) ?? new Dictionary<string, IReadOnlyList<object>>();

			if (dictionary.ContainsKey(_siteSearchConfiguration.CombinedSearchFieldName)) return;


            dictionary.Add(_siteSearchConfiguration.CombinedSearchFieldName, new List<object> { combinedFieldsValue });

			e.SetValues(dictionary.ToDictionary(keySelector => keySelector.Key, values => values.Value.AsEnumerable()));
          
		}
	}

	public class SiteSearchIndexComposer : ComponentComposer<SiteSearchIndexComponent>, IComposer
	{
	}
}
