using System.ComponentModel.DataAnnotations;

namespace BaseLibrary.DTOs
{
    public class UserDTO
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

    }
}
