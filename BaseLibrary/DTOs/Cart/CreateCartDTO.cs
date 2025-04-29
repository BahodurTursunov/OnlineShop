using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs.Cart
{
    public class CreateCartDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Cart must have at least one item.")]
        public List<CartItemDTO> CartItems { get; set; } = new List<CartItemDTO>();
    }
}
