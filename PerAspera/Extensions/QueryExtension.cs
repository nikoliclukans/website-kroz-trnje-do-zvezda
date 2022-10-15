namespace PerAspera.Extensions
{
	public static class QueryExtension
	{

		public static uint? GetUintParameter(this IQueryCollection queryCollection, string key)
		{
			if(queryCollection.TryGetValue(key, out var value))
			{
				return uint.TryParse(value, out var intPar) ? intPar : null;
			}
			return null;
		}

        public static string GetStringParameter(this IQueryCollection queryCollection, string key)
        {
            if (queryCollection.TryGetValue(key, out var value))
            {
                return value.ToString();
            }
			return string.Empty;
        }
    }
}
