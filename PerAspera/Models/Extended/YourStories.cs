using PerAspera.Models.ViewModels;

namespace PerAspera.Models.Generated
{
	public partial class YourStories
	{
		public PaginatedCollectionViewModel<YourStoriesItem> Stories { get; set; }

        public string GetPaginatedUrl(int pageNumber) => $"{this.Url()}?page={pageNumber}";

    }
}
