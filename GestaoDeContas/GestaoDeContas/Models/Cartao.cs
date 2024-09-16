using System.ComponentModel.DataAnnotations;

namespace GestaoDeContas.Models;

public class Cartao
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(120)]
    public string Banco { get; set; }
    
    [Required(ErrorMessage = "O final do cartão é obrigatório.")]
    [StringLength(4)]
    public string FinalCartao { get; set; }
    
    [Required(ErrorMessage = "A anotação é obrigatória.")]
    [StringLength(100)]
    public string Anotacoes { get; set; }
    
    [Required]
    public bool Status { get; set; }
    
    [Required(ErrorMessage = "O limite é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal LimiteCartao { get; set; }
    
    public decimal TotalGastos { get; set; } // Soma das compras feitas com este cartão
    public decimal LimiteDisponivel => LimiteCartao - TotalGastos;
}