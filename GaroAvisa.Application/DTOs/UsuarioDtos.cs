using System.ComponentModel.DataAnnotations;

namespace GaroAvisa.Application.DTOs
{
    public class CadastroUsuarioDto
    {
        [Required(ErrorMessage = "Informe o nome.")]
        [MaxLength(100, ErrorMessage = "O nome pode ter até 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
        [MaxLength(150, ErrorMessage = "O e-mail pode ter até 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        [MinLength(6, ErrorMessage = "A senha precisa ter pelo menos 6 caracteres.")]
        public string Senha { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe a senha.")]
        public string Senha { get; set; } = string.Empty;
    }

    public class UsuarioRespostaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class LoginRespostaDto
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioRespostaDto Usuario { get; set; } = new();
    }
}
