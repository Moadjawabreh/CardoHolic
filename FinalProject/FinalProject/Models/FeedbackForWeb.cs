using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
	public class FeedbackForWeb
    {
        public int ID { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public bool Status { get; set; } = false;

        public int userID { get; set; }

        [ForeignKey("userID")]
        public virtual User User { get; set; }


    }
}
