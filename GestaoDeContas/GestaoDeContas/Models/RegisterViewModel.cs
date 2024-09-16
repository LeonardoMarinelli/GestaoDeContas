using System.ComponentModel.DataAnnotations;

namespace GestaoDeContas.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "As senhas não coincidem.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}