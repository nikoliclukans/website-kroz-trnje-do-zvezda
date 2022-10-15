
using System.Text;

namespace PerAspera.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes (at most one instance of) the <paramref name="removeString"/> string from the end of the <paramref name="source"/> string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="removeString">String to remove from the <paramref name="source"/>.</param>
        /// <param name="comparisonType">Determines how <paramref name="source"/> and <paramref name="removeString"/> strings are compared to each other.</param>
        /// <returns><paramref name="source"/> string without <paramref name="removeString"/> at the end.</returns>
        public static string RemoveSuffix(this string source, string removeString, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            if (string.IsNullOrWhiteSpace(removeString)) return source;

            if (source.EndsWith(removeString, comparisonType))
            {
                source = source.Remove(source.Length - removeString.Length);
            }

            return source;
        }

        public static string[] SplitByWords(this string source)
            => source?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        public static string ToCsv(this IEnumerable<string> source)
            => string.Join(",", source ?? Enumerable.Empty<string>());

        public static bool IsUrl(this string source)
            => Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out Uri result)
                && (
                    !result.IsAbsoluteUri
                    || result.Scheme == Uri.UriSchemeHttp
                    || result.Scheme == Uri.UriSchemeHttps
                );

        public static string ReplaceNewLinesWithLineBreaks(this string source)
            => source
                    ?.Replace("\n\r", "<br/>")
                    .Replace("\n", "<br/>")
                    .Replace("\r", "<br/>")
                    ?? string.Empty;
    }

    public static class StringBuilderExtensions
    {
        public static StringBuilder TryAppendLine(this StringBuilder builder, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return builder;

            return builder
                    .ThrowIfNull()
                    .AppendLine(value);
        }
    }
}
