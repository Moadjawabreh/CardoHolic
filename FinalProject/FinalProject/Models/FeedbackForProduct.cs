using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
	public class FeedbackForProduct
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public bool Status { get; set; } = false;

        public int Rating { get; set; }

        [Required]
        public int userID { get; set; }

        [Required]
        public int productID { get; set; }

        [ForeignKey("productID")]
        public virtual Product Product{ get; set; }

        [ForeignKey("userID")]
        public virtual User User { get; set; }

    }
}
