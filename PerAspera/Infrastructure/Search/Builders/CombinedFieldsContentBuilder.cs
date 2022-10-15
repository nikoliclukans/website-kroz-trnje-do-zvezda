using PerAspera.Extensions;
using PerAspera.Infrastructure.Search.Builders.Interfaces;
using PerAspera.Infrastructure.Search.Converters;
using System.Text;
using PerAspera.Infrastructure.Search.Models;

namespace PerAspera.Infrastructure.Search.Builders
{
	internal sealed class CombinedFieldsContentBuilder : IStringValue, IBuilder<string>
	{
		private readonly StringBuilder _builder;

		private CombinedFieldsContentBuilder()
		{
			_builder = new StringBuilder();
		}

		public static IExpectStringValue Initialize() => new CombinedFieldsContentBuilder();

		public IStringValue WithValue(string value)
		{
			var isJson = value.DetectIsJson();
			if (isJson) return WithJsonValue(GetJsonData(value));

			_ = _builder.TryAppendLine(value?.StripHtml());

			return this;
		}

		public IStringValue WithValues(string[] values)
		{
			if (values is null) return this;

			values.ForEach(value => WithValue(value));

			return this;
		}

		public IBuilder<string> NoMoreValues() => this;

		public string Build() => _builder.ToString();

		private static List<JsonFieldDataModel> GetJsonData(string value)
			=> JsonDataTypesConverter.Deserialize(value);

		private IStringValue WithJsonValue(List<JsonFieldDataModel> jsonData)
		{
			var searchableContent = SearchableContentBuilder
				.Initialize()
				.WithJsonFieldsValue(jsonData)
				.NoMoreValues()
				.Build();

			_ = _builder.AppendLine(searchableContent);

			return this;
		}
	}
}
