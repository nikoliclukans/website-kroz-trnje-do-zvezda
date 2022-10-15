namespace PerAspera.Models.ViewModels
{
    public class SearchResultsPerPageViewModel
    {
        public long TotalResults { get; set; }
        public IReadOnlyList<SearchResultsItemViewModel> Items { get; set; }
    }

    public class SearchResultsItemViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
    }
}
