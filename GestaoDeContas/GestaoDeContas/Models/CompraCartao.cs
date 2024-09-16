using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoDeContas.Models;

public class CompraCartao
{
    [Key]
    public int Id { get; set; }
    
    private DateTime _dataCompra;

    [Required(ErrorMessage = "A data da compra é obrigatória.")]
    public DateTime DataCompra
    {
        get => _dataCompra;
        set => _dataCompra = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [StringLength(100)]
    public string Descricao { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [StringLength(50)]
    public string Categoria { get; set; }
    
    [Required]
    public bool Status { get; set; }
    
    [Required(ErrorMessage = "Informe se é parcelada ou não.")]
    public bool Parcelada { get; set; }
    
    [Required(ErrorMessage = "Informe o número de parcelas.")]
    public int NumeroParcelas { get; set; }

    [ForeignKey("CartaoId")]
    public int CartaoId { get; set; }
    
    public Cartao? Cartao { get; set; }
    public decimal ValorParcela { get; set; }
}