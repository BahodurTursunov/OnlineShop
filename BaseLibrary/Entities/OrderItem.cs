using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество товара должно быть больше 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть положительной")]

        [Column(TypeName = "numeric(18,2)")]
        public decimal UnitPrice { get; set; }
    }
}
