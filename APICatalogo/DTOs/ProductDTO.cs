using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using APICatalogo.Models;

namespace APICatalogo.DTOs
{
    public class ProductDTO
    {

        [Key]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(20, ErrorMessage = "The name must have between 3 and 20 characters", MinimumLength = 3)]
        public string? Name { get; set; }
        [Required]
        [StringLength(40, ErrorMessage = "The description must have up to 40 characters")]
        public string? Description { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        [StringLength(300)]
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
       
        
    }
}
