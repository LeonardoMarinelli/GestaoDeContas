using System.ComponentModel.DataAnnotations;

namespace GestaoDeContas.Models;

public class Conta
{
    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(100)]
    public string Descricao { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
    
    [Required]
    public bool Status { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(50)]
    public string Categoria { get; set; }
}