using System.ComponentModel.DataAnnotations;

namespace GestaoDeContas.Models;

public class ContaPagar : Conta
{
    [Key]
    public int Id { get; set; }
    
    private DateTime _dataVencimento;

    [Required]
    public DateTime DataVencimento
    {
        get => _dataVencimento;
        set => _dataVencimento = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}