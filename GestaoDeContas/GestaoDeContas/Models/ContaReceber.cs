using System.ComponentModel.DataAnnotations;

namespace GestaoDeContas.Models;

public class ContaReceber : Conta
{
    [Key]
    public int Id { get; set; }
    
    private DateTime _dataRecebimento;

    [Required]
    public DateTime DataRecebimento
    {
        get => _dataRecebimento;
        set => _dataRecebimento = DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }
}