using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class SecretCodeForProduct
    {
        public int Id { get; set; }
        public string Code { get; set; }

        [Required]
        public int? status { get; set; } = 0;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
    }
}
