using PerAspera.Models.Generated;
using System.Drawing.Printing;

namespace PerAspera.Models.ViewModels
{
	public class PaginatedCollectionViewModel<T>
	{
		public PaginatedCollectionViewModel(IEnumerable<T> collection, uint total, uint itemsPerPage, uint currentPage)
		{
			Collection = collection;
			Total = total;
			ItemsPerPage = itemsPerPage;
			CurrentPage = currentPage;
		}

		public IEnumerable<T> Collection { get; }
		public uint Total { get; }
		public uint ItemsPerPage { get; }
		public uint CurrentPage { get; }

		public uint TotalPageNumber => (Total + ItemsPerPage - 1) / ItemsPerPage;

		public bool HasPrevious => 1 < CurrentPage;
		public bool HasNext => TotalPageNumber > CurrentPage;
		
    }
}
