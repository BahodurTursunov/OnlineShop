using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Цена товара обязательна к заполнению")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена товара должна быть больше 0")]
        [Column(TypeName = "numeric(18, 2)")]
        public decimal Price { get; set; }

        public int? Stock { get; set; }

        [Column(TypeName = "numeric(18, 2)")]
        public decimal Discount { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public Category? Category { get; set; }

        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [JsonIgnore]
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
