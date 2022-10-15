using Newtonsoft.Json;
using PerAspera.Infrastructure.Search.Converters;
using System.Collections.Generic;

namespace PerAspera.Infrastructure.Search.Models
{
    internal class JsonFieldDataModel
	{
		public string Title { get; set; }

		public string Text { get; set; }

		public List<JsonFieldDataModel> Items => GetItems(ItemsRawValue);

		[JsonProperty("items")]
		public string ItemsRawValue { get; set; }

		private static List<JsonFieldDataModel> GetItems(string jsonData)
			=> JsonDataTypesConverter.Deserialize(jsonData);
	}
}
