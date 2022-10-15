
namespace PerAspera.Infrastructure.Search.Models
{
    public class PaginationRequest
    {
        private PaginationRequest(int page, int itemsPerPage)
        {
            Page = page > 0 ? page : 1;
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
