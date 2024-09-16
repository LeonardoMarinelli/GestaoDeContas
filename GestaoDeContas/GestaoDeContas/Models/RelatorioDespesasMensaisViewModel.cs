namespace GestaoDeContas.Models;

public class RelatorioDespesasMensaisViewModel
{
    public string Categoria { get; set; }
    public decimal ValorTotal { get; set; }
    public int Mes { get; set; }
    public int Ano { get; set; }
}