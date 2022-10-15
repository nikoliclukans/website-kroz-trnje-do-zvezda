
using System.Collections.Generic;

using PerAspera.Infrastructure.Search.Models;

namespace PerAspera.Infrastructure.Search.Builders.Interfaces
{
	internal interface IExpectJsonFieldValue
	{
		IJsonFieldValue WithJsonFieldValue(JsonFieldDataModel value);
		IJsonFieldValue WithJsonFieldsValue(List<JsonFieldDataModel> values);
	}
}
