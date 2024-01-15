using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
	public enum Role
    {
        Admin,
        User
    }
    public class User
    {
        public int ID { get; set; }

		[Required]
		public string Name { get; set; }

        [Required]
		public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? phoneNumber { get; set; }

        public string? City { get; set; }

        [Url]
        public string? location { get; set; }

		[NotMapped]
		public IFormFile? ImageFile { get; set; }

		public string? ImageUrl { get; set; }

        [Required]
        public Role Role { get; set; } = Role.User;

        public ICollection<FeedbackForProduct> FeedbackForProducts { get; set; }

        public ICollection<FeedbackForWeb> FeedbackForWebs { get; set; }
        public ICollection<Cart> carts { get; set; }
        public ICollection<Order> orders { get; set; }

    }
}
