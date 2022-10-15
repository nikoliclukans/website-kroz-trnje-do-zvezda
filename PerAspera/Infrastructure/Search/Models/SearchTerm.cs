
using CSharpFunctionalExtensions;

namespace PerAspera.Infrastructure.Search.Models
{

    public class SearchTerm
    {
        private SearchTerm(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<SearchTerm> Create(string value)
        {
            value = value?.Trim('\"', '\'');

            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<SearchTerm>(
                    $"The parameter '{nameof(value)}' cannot be null or whitespace.");
            }

            return Result.Success(new SearchTerm(value));
        }
    }
}
