
namespace PerAspera.Extensions
{
	public static class EnumerableExtensions
	{
		public static IList<TValue> GetItemsPerPage<TValue>(this IEnumerable<TValue> sequence, int page, int itemsPerPage)
		{
			_ = page.ThrowIfNegativeOrZero();
			_ = itemsPerPage.ThrowIfNegativeOrZero();

			return sequence
				?.Skip((page - 1) * itemsPerPage)
				.Take(itemsPerPage)
				.ToList()
				?? new List<TValue>(0);
		}

		public static void ForEach<TValue>(this IEnumerable<TValue> sequence, Action<TValue> action)
		{
			if (sequence is null) return;

			foreach (var item in sequence) action(item);
		}
	}
}
