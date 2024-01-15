using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Category
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

		[NotMapped]
		public IFormFile? ImageFile { get; set; }


		[Required]
        public string Image { get; set; }

		[Required]
		public int? CategoryContainerID { get; set; }

		[ForeignKey("CategoryContainerID")]
		public virtual CategoryContainer CategoryContainer  { get; set; }

		public ICollection<Product> Products { get; set; }
    }
}
