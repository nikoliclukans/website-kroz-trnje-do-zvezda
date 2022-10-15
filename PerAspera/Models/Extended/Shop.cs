using Microsoft.AspNetCore.Mvc.Rendering;
﻿using PerAspera.Models.ViewModels;

namespace PerAspera.Models.Generated
{
    public partial class Shop
    {
        public PaginatedCollectionViewModel<ShopItem> ShopItemList { get; set; }

        public string GetPaginatedUrl(int pageNumber) => $"{this.Url()}?page={pageNumber}";

		public List<SelectListItem> GetProducts()
		{
			List<SelectListItem> products = ShopItemList.Collection
												.Select(si => new SelectListItem {Text = si.ItemName, Value = si.Key.ToString()})
												.ToList();
			products.Insert(0, new SelectListItem { Text = "Odaberite predmet za poručivanje...", Value = Guid.Empty.ToString() });
			return products;
		}
    }
}
