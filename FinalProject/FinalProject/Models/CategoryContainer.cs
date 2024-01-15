using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
	public class CategoryContainer
	{
		public int Id { get; set; }
		public string Name { get; set; }

		[NotMapped]
		public IFormFile? ImageFile { get; set; }

		[Required]
		public string Image { get; set; }
		public ICollection<Category> Categories { get; set; }

	}
}
