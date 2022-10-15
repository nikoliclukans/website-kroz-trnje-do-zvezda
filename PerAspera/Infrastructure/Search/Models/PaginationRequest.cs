
namespace PerAspera.Infrastructure.Search.Models
{
    public class PaginationRequest
    {
        private PaginationRequest(int page, int itemsPerPage)
        {
            Page = page;
            ItemsPerPage = itemsPerPage;
        }

        public static PaginationRequest Create(int page, int itemsPerPage)
        {

            return new PaginationRequest(page, itemsPerPage);
        }

        public int Page { get; }
        public int ItemsPerPage { get; }
    }
}
