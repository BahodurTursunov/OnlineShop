using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.Entities
{
    public class Review : BaseEntity
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5")]
        public int Rating { get; set; }

        [MaxLength(500, ErrorMessage = "Текст отзыва не должен превышать 500 символов")]
        public string Comment { get; set; } = string.Empty;
    }
}
