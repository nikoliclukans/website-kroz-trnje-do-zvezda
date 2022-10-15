using System.ComponentModel.DataAnnotations;

namespace PerAspera.Models.ViewModels
{
    public class YourStoryFormDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
