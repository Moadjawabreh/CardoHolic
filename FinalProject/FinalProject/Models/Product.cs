using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
	public class Product
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }
        public double Cost { get; set; }

        [Required]
        public string Description { get; set; }

        public double percentageOfDiscount { get; set; } = 1;

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        [Required]
        public string UrlImage { get; set; }

        [Required]
        public int? categoryID { get; set; }

        [ForeignKey("categoryID")]
        public virtual Category Category { get; set; }
        public ICollection<FeedbackForProduct> FeedbackForProducts { get; set; }
        public ICollection<Cart> carts { get; set; }
        public ICollection<SecretCodeForProduct> secretCodeForProducts { get; set; }

    }
}
