using System.ComponentModel.DataAnnotations;

namespace PerAspera.Models.ViewModels
{
	public class ShopOrderDto
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string Surename { get; set; }
		[Required]
		[EmailAddress]
		public string Email { get; set; }
		[Required]
		public string Address { get; set; }
		[Required]
		public string PhoneNumber { get; set; }
		[Required]
		public string OrderItem { get; set; }
		[Required]
		public string Message { get; set; }
	}
}
