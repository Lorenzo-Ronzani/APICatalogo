using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models
{
    [Table("Products")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "The name must have between 3 and 20 characters", MinimumLength = 3)]
        public string? Name { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "The description must have up to 40 characters")]
        public string? Description{ get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Range(1, 10000, ErrorMessage = "Price must be between {1} and {2} dollars")]
        public double? Price{ get; set; }
        [Required]
        [StringLength(300)]
        public string? ImageUrl{ get; set; }
        [Required]
        [Range(1, 20000, ErrorMessage = "Stock must be between {1} and {2}")]
        public float Stock { get; set; }
        public DateTime RegisterDate { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }

    }
}
