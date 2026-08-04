using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs
{
    public class ProductDTOUpdateRequest : IValidatableObject
    {
        [Range(1, 9999, ErrorMessage = "Stock must be between 1 and 9999")]
        public float Stock { get; set; }
       
        public DateTime RegisterDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RegisterDate <= DateTime.Now.Date)
            {
                yield return new ValidationResult("The registration date must be greater than the current date.",
                new[] { nameof(this.RegisterDate)});
            }
        }
    }
}
