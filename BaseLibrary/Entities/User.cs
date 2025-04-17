using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BaseLibrary.Entities
{
    public class User : BaseEntity
    {
        [Required(ErrorMessage = "Имя обязательно к заполнению"), StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Фамилия обязательно к заполнению"), StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Имя пользователя обязательно к заполнению")]
        [StringLength(30, ErrorMessage = "Имя пользователя не должно превышать 30 символов")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Email обязателен к заполнению")]
        [EmailAddress(ErrorMessage = "Неверный формат Email")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Пароль обязателен к заполнению")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "User";
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime? RefreshTokenExpiryDate { get; set; }
        //public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
