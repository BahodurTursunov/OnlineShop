using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs.Cart
{
    public class CartItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
        /*public decimal Price { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public CartItemDTO(int productId, int quantity, decimal price, string name, string imageUrl, string description)
        {
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            Name = name;
            ImageUrl = imageUrl;
            Description = description;
        }*/
    }
}
