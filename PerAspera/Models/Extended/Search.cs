using PerAspera.Models.ViewModels;

namespace PerAspera.Models.Generated
{
    public partial class Search
    {
        public PaginatedCollectionViewModel<SearchResultsItemViewModel> SearchItems { get; set; }

        public string Query { get; set; }

        public string GetPaginatedUrl(int pageNumber) => $"{this.Url()}?query={Query}&page={pageNumber}";

    }
}
