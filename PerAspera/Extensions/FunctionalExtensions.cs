using System.Runtime.CompilerServices;

namespace PerAspera.Extensions
{
	public static class FunctionalExtensions
	{
		public static TResult GetValueOrDefault<TSource, TResult>(
			this TSource @this,
			Func<TSource, TResult> valueFunction,
			Func<TResult> factory)
			=> valueFunction(@this)
				.Or(
					value => value != null && !value.Equals(default(TResult)) && !value.Equals(string.Empty),
					factory()
				);

		public static TValue Or<TValue>(this TValue @this, Func<TValue, bool> predicate, TValue value)
			=> predicate(@this) ? @this : value;

		public static TValue ThrowIfNull<TValue>(
			this TValue argument,
			string message = default,
			[CallerArgumentExpression("argument")] string paramName = default)
		{
			return argument ?? throw new ArgumentNullException(paramName, message);
		}

		public static string ThrowIfNullOrWhiteSpace(
			this string argument,
			string message = default,
			[CallerArgumentExpression("argument")] string paramName = default)
		{
			return argument ?? throw new ArgumentException(
											paramName,
											message ?? $"'{paramName}' cannot be null or whitespace.");
		}

		public static int ThrowIfNegativeOrZero(
			this int argument,
			string message = default,
			[CallerArgumentExpression("argument")] string paramName = default)
		{
			return argument > 0
					? argument
					: throw new ArgumentException(
									paramName,
									message ?? $"'{paramName}' cannot be less than 1.");
		}
	}
}
