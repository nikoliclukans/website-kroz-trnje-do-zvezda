
using PerAspera.Infrastructure.Search.Builders.Interfaces;
using System.Text;
using PerAspera.Infrastructure.Search.Models;
using PerAspera.Extensions;

namespace PerAspera.Infrastructure.Search.Builders
{
	internal sealed class SearchableContentBuilder : IJsonFieldValue, IBuilder<string>
	{
		private readonly StringBuilder _builder;

		private SearchableContentBuilder()
		{
			_builder = new StringBuilder();
		}

		public static IExpectJsonFieldValue Initialize() => new SearchableContentBuilder();

		public IJsonFieldValue WithJsonFieldValue(JsonFieldDataModel value)
		{
			if (value is null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			_ = _builder.TryAppendLine(value.Title);
			_ = _builder.TryAppendLine(value.Text?.StripHtml());
			value.Items?.ForEach(item => WithJsonFieldValue(item));

			return this;
		}

		public IJsonFieldValue WithJsonFieldsValue(List<JsonFieldDataModel> values)
		{
			values.ThrowIfNull().ForEach(data => WithJsonFieldValue(data));

			return this;
		}

		public IBuilder<string> NoMoreValues() => this;

		public string Build() => _builder.ToString();
	}
}
