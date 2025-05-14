using BaseLibrary.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseLibrary.Entities
{
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; } // Например, Stripe или PayPal

        [Required]
        public DateTime PaymentDate { get; set; }
    }
}
