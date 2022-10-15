using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PerAspera.Models.ViewModels
{
	public class ContactUsFormDto
	{

		[Required]
		public string  Name{ get; set; }

        [Required]
        public string  Surname{ get; set; }

        [Required]
        [EmailAddress]
        public string  Email{ get; set; }

        [Required]
        public string  Message{ get; set; }

        [IgnoreDataMember]
        public string? ThankYouMessage { get; set; }
	}
}
