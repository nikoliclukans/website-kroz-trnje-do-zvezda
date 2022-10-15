using PerAspera.Models.ViewModels;

namespace PerAspera.Models.Generated
{
    public partial class Shop
    {
        public PaginatedCollectionViewModel<ShopItem> ShopItemList { get; set; }

        public string GetPaginatedUrl(int pageNumber) => $"{this.Url()}?page={pageNumber}";

    }
}
