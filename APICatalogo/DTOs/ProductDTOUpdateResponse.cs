using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using APICatalogo.Models;

namespace APICatalogo.DTOs
{
    public class ProductDTOUpdateResponse
    {
        public int ProductId { get; set; }
        
        public string? Name { get; set; }
        
        public string? Description { get; set; }
       
        public double? Price { get; set; }
      
        public string? ImageUrl { get; set; }
       
        public float Stock { get; set; }
        public DateTime RegisterDate { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
