using Examine.Lucene.Indexing;

using Lucene.Net.Documents;

using Microsoft.Extensions.Logging;
using PerAspera.Infrastructure.Search.Converters;
using System.Collections.Generic;

using PerAspera.Infrastructure.Search.Builders;
using PerAspera.Infrastructure.Search.Models;

namespace PerAspera.Infrastructure.Search.ValueTypes
{
    public class JsonValueType : IndexFieldValueTypeBase
	{
		public const string ValueTypeName = "json";

		public JsonValueType(string fieldName, ILoggerFactory loggerFactory, bool store = true) 
			: base(fieldName, loggerFactory, store)
		{
		}

		protected override void AddSingleValue(Document doc, object value)
		{
			if (value is not string valueString) return;

			var data = Deserialize(valueString);

			var searchableContent = SearchableContentBuilder
				.Initialize()
				.WithJsonFieldsValue(data)
				.NoMoreValues()
				.Build();

			doc.Add(new TextField(FieldName, searchableContent, Field.Store.YES));
		}

		private static List<JsonFieldDataModel> Deserialize(string json) => JsonDataTypesConverter.Deserialize(json);
	}
}
