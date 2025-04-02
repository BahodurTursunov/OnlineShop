using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Product : BaseEntity
    {
        [Required(ErrorMessage = "Название товара обязательно к заполнению")]
        [StringLength(100, ErrorMessage = "Название не может превышать 100 символов")]
        public string Name { get; set; } = "";

        [MaxLength(500, ErrorMessage = "Описание к товару не может превышать 500 символов")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Цена товара обязательна к заполнению")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена товара должна быть больше 0")]
        [Column(TypeName = "numeric(18, 2)")]
        public decimal Price { get; set; }

        public int? Stock { get; set; }

        [Column(TypeName = "numeric(18, 2)")]
        public decimal Discount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Review> Reviews { get; set; }

    }
}
