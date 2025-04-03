using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BaseLibrary.Entities.Auth
{
    public class Register
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string? Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Required]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [PasswordPropertyText]
        [Required]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Required]
        public string? ConfirmPassword { get; set; }
       

    }
}
