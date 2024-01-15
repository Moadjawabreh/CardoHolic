using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    
	public class Cart
    {
		[Key]
		[Required]
		public int ID { get; set; }
		public int Quantity { get; set; } = 1;
		[Required]
		public int UserId { get; set; }

		[Required]
		public bool Flag { get; set; } = true;

		[ForeignKey("UserId")]
		public User User { get; set; }
		[Required]
		public int productId { get; set; }
		[ForeignKey("productId")]
		public Product product { get; set; }

		public int? OrderId { get; set; }
		[ForeignKey("OrderId")]
		public Order? Order { get; set; }
	}
}
