namespace BaseLibrary.DTOs
{
    public class AuthResponseDTO
    {
        /// <summary>
        /// JWT access token.
        /// </summary>
        public string AccessToken { get; set; } = default!;

        /// <summary>
        /// Refresh token для обновления access token.
        /// </summary>
        public string RefreshToken { get; set; } = default!;
        public string? FullName { get; set; }


        /// <summary>
        /// Дата и время истечения access token.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Имя пользователя (необязательно, если хочешь отправлять).
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Почта пользователя (опционально).
        /// </summary>
        public string? Email { get; set; }
    }
}
