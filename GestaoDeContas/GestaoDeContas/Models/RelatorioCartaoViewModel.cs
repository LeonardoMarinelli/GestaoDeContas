namespace GestaoDeContas.Models;

public class RelatorioCartaoViewModel
{
    public Cartao Cartao { get; set; }
    public List<CompraCartao> Compras { get; set; }
}