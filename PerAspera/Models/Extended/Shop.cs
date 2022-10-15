using Microsoft.AspNetCore.Mvc.Rendering;

namespace PerAspera.Models.Generated
{
	public partial class Shop
	{
		public List<SelectListItem> GetProducts()
		{
			List<SelectListItem> products = new List<SelectListItem>
			{
				new SelectListItem {Text = "Odaberite predmet za poručivanje...", Value = "1"},
				new SelectListItem {Text = "Imunomania", Value = "1"},
				new SelectListItem {Text = "Fearless kapa", Value = "2"},
			};
			return products;
		}
	}
}
