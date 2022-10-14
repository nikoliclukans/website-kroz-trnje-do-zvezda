using PerAspera.Models.ViewModels;

namespace PerAspera.Models.Generated
{
	public partial class Blogs
	{

		public PaginatedCollectionViewModel<BlogPage> BlogList { get; set; }

		public string GetPaginatedUrl(int pageNumber)=>$"{this.Url()}?page={pageNumber}&category={this.CurrentCategory}&year={this.CurrentYear}";

		public uint? CurrentCategory { get; set; }
		public uint? CurrentYear { get; set; }

		public bool IsSelectedCategory(int categoryId) => CurrentCategory == categoryId;
		public bool IsSelectedYear(int year) => CurrentYear == year;
		
	}
}
