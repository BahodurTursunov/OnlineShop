using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class CreateCategoryDTO
    {
        [Required(ErrorMessage = "The category name must not be empty.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The category name must be between 3 and 100 characters.")]
        public required string Name { get; set; }
    }
}
