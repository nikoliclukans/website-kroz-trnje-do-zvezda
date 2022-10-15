using Newtonsoft.Json;

using System.Collections.Generic;

using PerAspera.Infrastructure.Search.Models;

namespace PerAspera.Infrastructure.Search.Converters
{
	internal sealed class JsonDataTypesConverter
	{
		public static List<JsonFieldDataModel> Deserialize(string jsonData)
		{
			if (string.IsNullOrWhiteSpace(jsonData))
			{
				return new List<JsonFieldDataModel>(0);
			}

			return JsonConvert.DeserializeObject<List<JsonFieldDataModel>>(jsonData);
		}
	}
}
