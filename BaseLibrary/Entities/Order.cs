using BaseLibrary.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Order : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column(TypeName = "numeric(18,2)")]
        public decimal TotalAmount { get; set; }
        public int PromocodeId { get; set; }
        public PromoCode? PromoCodes { get; set; }
        public DateTime OrderDate { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public Statuses Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }

    }
}
