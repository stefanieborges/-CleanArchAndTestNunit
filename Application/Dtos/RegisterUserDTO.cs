using System.ComponentModel.DataAnnotations;

namespace Application.Dtos
{
    public class RegisterUserDTO
    {
        [Required]
        public string? Nome { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required]
        public string? Senha { get; set; } = string.Empty;
        [Required, Compare(nameof(Senha))]
        public string? ConfimarSenha { get; set; } = string.Empty;
    }
}
