using System.ComponentModel.DataAnnotations;

namespace PerAspera.Models.ViewModels
{
	public class ShopOrderDto
	{
		[Required]
		public string? Name { get; set; }
		[Required]
		public string? Surename { get; set; }
		[Required]
		[EmailAddress]
		public string? Email { get; set; }
		[Required]
		public string? Address { get; set; }
		[Required]
		public string? PhoneNumber { get; set; }
		public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
		public string? Message { get; set; }

		public decimal TotalPrice { get; set; }
	}

	public class OrderItemDto
	{
		public string? Name { get; set; }
		public string? Quantity { get; set; }
		public decimal Price { get; set; }
	}

}
