using System.Drawing;
using System.Globalization;
using DinkToPdf;
using DinkToPdf.Contracts;
using GestaoDeContas.Data;
using GestaoDeContas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace GestaoDeContas.Controllers;

public class RelatoriosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter _pdfConverter;
    private readonly ICompositeViewEngine _viewEngine;

    public RelatoriosController(ApplicationDbContext context, IConverter pdfConverter, ICompositeViewEngine viewEngine)
    {
        _context = context;
        _pdfConverter = pdfConverter;
        _viewEngine = viewEngine;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> RelatorioDespesasMensais()
    {
        var relatorio = await GenerateDespesasMensais();
        return View(relatorio);
    }
    
    public async Task<IActionResult> RelatorioEvolucaoSaldo()
    {
        var contasPagar = await _context.ContasPagar
            .OrderBy(c => c.DataVencimento)
            .ToListAsync();

        var contasReceber = await _context.ContasReceber
            .OrderBy(c => c.DataRecebimento)
            .ToListAsync();

        var todasTransacoes = contasPagar.Select(c => new
            {
                Data = c.DataVencimento,
                Valor = -c.Valor
            })
            .Concat(contasReceber.Select(c => new
            {
                Data = c.DataRecebimento,
                Valor = c.Valor
            }))
            .OrderBy(t => t.Data)
            .ToList();
        
        decimal saldoAcumulado = 0;
        var evolucaoSaldo = todasTransacoes.Select(t => new
        {
            t.Data,
            SaldoAcumulado = saldoAcumulado += t.Valor
        }).ToList();

        var labels = evolucaoSaldo.Select(t => t.Data.ToString("dd/MM/yyyy")).ToArray();
        var saldoValores = evolucaoSaldo.Select(t => t.SaldoAcumulado).ToArray();

        ViewBag.Labels = labels;
        ViewBag.SaldoValores = saldoValores;

        return View();
    }
    
    public async Task<IActionResult> RelatorioTransacoesCartao()
    {
        var relatorioCartao = await GenerateTransacoesCartao();
        return View(relatorioCartao);
    }
    
    public async Task<IActionResult> SaldoPagarReceber()
    {
        var (contasPagar, contasReceber, totalPagar, totalReceber, saldo) = 
            await CalculateSaldoPagarReceber();

        ViewBag.ContasPagar = contasPagar;
        ViewBag.ContasReceber = contasReceber;
        ViewBag.TotalPagar = totalPagar;
        ViewBag.TotalReceber = totalReceber;
        ViewBag.Saldo = saldo;

        return View();
    }
    
    public async Task<IActionResult> ExportarPdf()
    {
        var relatorio = await GenerateDespesasMensais();

        var html = relatorio
            .Aggregate("""
                           <html>
                               <head>
                                   <style>
                                       body {
                                           font-family: Arial, sans-serif;
                                           margin: 20px;
                                       }
                                       h2 {
                                           text-align: center;
                                           color: #333;
                                       }
                                       table {
                                           width: 100%;
                                           border-collapse: collapse;
                                           margin-top: 20px;
                                       }
                                       table, th, td {
                                           border: 1px solid #ddd;
                                       }
                                       th, td {
                                           padding: 10px;
                                           text-align: left;
                                       }
                                       th {
                                           background-color: #f2f2f2;
                                       }
                                       tr:nth-child(even) {
                                           background-color: #f9f9f9;
                                       }
                                       tr:hover {
                                           background-color: #f1f1f1;
                                       }
                                       .total {
                                           font-weight: bold;
                                       }
                                   </style>
                               </head>
                               <body>
                                   <h2>Relatório de Despesas Mensais por Categoria</h2>
                                   <table>
                                       <thead>
                                           <tr>
                                               <th>Categoria</th>
                                               <th>Mês</th>
                                               <th>Ano</th>
                                               <th>Valor Total</th>
                                           </tr>
                                       </thead>
                                       <tbody>
                           """, 
            (current, item) => 
                current + $"""
                              <tr>
                                  <td>{item.Categoria}</td>
                                  <td>{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Mes)}</td>
                                  <td>{item.Ano}</td>
                                  <td>R$ {item.ValorTotal.ToString("F2")}</td>
                              </tr>
                          """);

        html += """
                                </tbody>
                            </table>
                        </body>
                    </html>
                """;

        var pdfDocument = new HtmlToPdfDocument
        {
            GlobalSettings = 
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
            },
            Objects = 
            {
                new ObjectSettings 
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    FooterSettings = { Right = "[page] of [toPage]" }
                }
            }
        };

        var file = _pdfConverter.Convert(pdfDocument);
        return File(file, "application/pdf", "RelatorioDespesasMensais.pdf");
    }
    
    public async Task<IActionResult> ExportarSaldoPagarReceberPdf()
    {
        var (contasPagar, contasReceber, totalPagar, totalReceber, saldo) = 
            await CalculateSaldoPagarReceber();

        var html = """
        <html>
            <head>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 20px;
                    }
                    h2 {
                        text-align: center;
                        color: #333;
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 20px;
                    }
                    table, th, td {
                        border: 1px solid #ddd;
                    }
                    th, td {
                        padding: 10px;
                        text-align: left;
                    }
                    th {
                        background-color: #f2f2f2;
                    }
                    tr:nth-child(even) {
                        background-color: #f9f9f9;
                    }
                    tr:hover {
                        background-color: #f1f1f1;
                    }
                    .total {
                        font-weight: bold;
                    }
                    .saldoPositivo {
                        color: green;
                    }
                    .saldoNegativo {
                        color: red;
                    }
                </style>
            </head>
            <body>
                <h2>Relatório de Saldo entre Contas a Pagar e Contas a Receber</h2>
                <div style="display: flex;">
                    <div style="width: 50%; padding-right: 10px;">
                        <h3>Contas a Pagar</h3>
                        <table>
                            <thead>
                                <tr>
                                    <th>Categoria</th>
                                    <th>Valor Total</th>
                                </tr>
                            </thead>
                            <tbody>
        """ + 
                string.Join("", contasPagar.Select(p => $"""
                <tr>
                    <td>{p.Categoria}</td>
                    <td>R$ {p.ValorTotal:F2}</td>
                </tr>
                """)) + 
                        $"""
                         </tbody>
                        <tfoot>
                            <tr>
                                <th>Total</th>
                                <th>R$ {totalPagar:F2}</th>
                            </tr>
                        </tfoot>
                    </table>
                </div>

                    <div style="width: 50%; padding-left: 10px;">
                        <h3>Contas a Receber</h3>
                        <table>
                            <thead>
                                <tr>
                                    <th>Categoria</th>
                                    <th>Valor Total</th>
                                </tr>
                            </thead>
                            <tbody>
                                {string.Join("", contasReceber.Select(r => $"""
                                <tr>
                                    <td>{r.Categoria}</td>
                                    <td>R$ {r.ValorTotal:F2}</td>
                                </tr>
                                """))}
                            </tbody>
                            <tfoot>
                                <tr>
                                    <th>Total</th>
                                    <th>R$ {totalReceber:F2}</th>
                                </tr>
                            </tfoot>
                        </table>
                    </div>
                </div>

                <h3 style="text-align: center;">
                    Saldo:
                    <span class="{(saldo >= 0 ? "saldoPositivo" : "saldoNegativo")}">
                        R$ {saldo:F2}
                    </span>
                </h3>
            </body>
        </html>
        """;

        var pdfDocument = new HtmlToPdfDocument
        {
            GlobalSettings = 
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
            },
            Objects = 
            {
                new ObjectSettings 
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    FooterSettings = { Right = "[page] of [toPage]" }
                }
            }
        };

        var file = _pdfConverter.Convert(pdfDocument);
        return File(file, "application/pdf", "RelatorioSaldoPagarReceber.pdf");
    }
    
    public async Task<IActionResult> ExportarTransacoesCartaoPdf()
    {
        var relatorioCartao = await GenerateTransacoesCartao();

        var html = """
        <html>
            <head>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 20px;
                    }
                    h2 {
                        color: #333;
                    }
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        margin-top: 10px;
                    }
                    th, td {
                        border: 1px solid #ddd;
                        padding: 8px;
                        text-align: left;
                    }
                    th {
                        background-color: #f2f2f2;
                    }
                    .total {
                        font-weight: bold;
                    }
                    .saldoPositivo {
                        color: green;
                    }
                    .saldoNegativo {
                        color: red;
                    }
                </style>
            </head>
            <body>
                <h1>Relatório de Transações por Cartão de Crédito</h1>
    """ + $"""
                {
                string.Join("", relatorioCartao.Select(cartaoViewModel => $"""
                <h2>Cartão: {cartaoViewModel.Cartao.Banco} - Final: {cartaoViewModel.Cartao.FinalCartao}</h2>
                <p><strong>Anotações:</strong> {cartaoViewModel.Cartao.Anotacoes}</p>
                <table>
                    <thead>
                        <tr>
                            <th>Categoria</th>
                            <th>Data</th>
                            <th>Descrição</th>
                            <th>Valor</th>
                            <th>Parcelada</th>
                            <th>Número de Parcelas</th>
                        </tr>
                    </thead>
                    <tbody>
                        {string.Join("", cartaoViewModel.Compras.Select(compra => $"""
                        <tr>
                            <td>{compra.Categoria}</td>
                            <td>{compra.DataCompra.ToString("dd/MM/yyyy")}</td>
                            <td>{compra.Descricao}</td>
                            <td>R$ {compra.Valor.ToString("F2")}</td>
                            <td>{(compra.Parcelada ? "Sim" : "Não")}</td>
                            <td>{(compra.Parcelada ? compra.NumeroParcelas.ToString() : "-")}</td>
                        </tr>
                        """))}
                    </tbody>
                </table>
                <p><strong>Total Gasto:</strong> R$ {cartaoViewModel.Compras.Sum(c => c.Valor):F2}</p>
                <p><strong>Limite Disponível:</strong> R$ {cartaoViewModel.Cartao.LimiteDisponivel:F2}</p>
                <hr />
                """))}
            </body>
        </html>
        """;

        var pdfDocument = new HtmlToPdfDocument
        {
            GlobalSettings = 
            {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait,
            },
            Objects = 
            {
                new ObjectSettings 
                {
                    PagesCount = true,
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    FooterSettings = { Right = "[page] of [toPage]" }
                }
            }
        };

        var file = _pdfConverter.Convert(pdfDocument);
        return File(file, "application/pdf", "RelatorioTransacoesCartao.pdf");
    }
    
    public async Task<IActionResult> ExportarExcel()
    {
        var relatorio = await GenerateDespesasMensais();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Despesas Mensais");

        worksheet.Cells[1, 1].Value = "Categoria";
        worksheet.Cells[1, 2].Value = "Mês";
        worksheet.Cells[1, 3].Value = "Ano";
        worksheet.Cells[1, 4].Value = "Valor Total (R$)";

        using (var range = worksheet.Cells[1, 1, 1, 4])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        }

        var row = 2;
        foreach (var item in relatorio)
        {
            worksheet.Cells[row, 1].Value = item.Categoria;
            worksheet.Cells[row, 2].Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.Mes);
            worksheet.Cells[row, 3].Value = item.Ano;
            worksheet.Cells[row, 4].Value = item.ValorTotal.ToString("F2");
            row++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        var excelBytes = await package.GetAsByteArrayAsync();
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            "RelatorioDespesasMensais.xlsx");
    }

    public async Task<IActionResult> ExportarSaldoPagarReceberExcel()
    {
        var (contasPagar, contasReceber, totalPagar, totalReceber, saldo) =
            await CalculateSaldoPagarReceber();

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Saldo Pagar Receber");

        worksheet.Cells[1, 1].Value = "Contas a Pagar - Categoria";
        worksheet.Cells[1, 2].Value = "Valor Total (R$)";
        worksheet.Cells[1, 4].Value = "Contas a Receber - Categoria";
        worksheet.Cells[1, 5].Value = "Valor Total (R$)";

        using (var range = worksheet.Cells[1, 1, 1, 5])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
        }

        var row = 2;
        var maxRows = Math.Max(contasPagar.Count, contasReceber.Count);

        for (var i = 0; i < maxRows; i++)
        {
            if (i < contasPagar.Count)
            {
                worksheet.Cells[row, 1].Value = contasPagar[i].Categoria;
                worksheet.Cells[row, 2].Value = contasPagar[i].ValorTotal.ToString("F2");
            }

            if (i < contasReceber.Count)
            {
                worksheet.Cells[row, 4].Value = contasReceber[i].Categoria;
                worksheet.Cells[row, 5].Value = contasReceber[i].ValorTotal.ToString("F2");
            }

            row++;
        }

        worksheet.Cells[row, 1].Value = "Total";
        worksheet.Cells[row, 2].Value = totalPagar.ToString("F2");
        worksheet.Cells[row, 4].Value = "Total";
        worksheet.Cells[row, 5].Value = totalReceber.ToString("F2");

        using (var totalRange = worksheet.Cells[row, 1, row, 5])
        {
            totalRange.Style.Font.Bold = true;
            totalRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            totalRange.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
        }

        row++;
        worksheet.Cells[row, 1].Value = "Saldo (Receber - Pagar)";
        worksheet.Cells[row, 2].Value = saldo.ToString("F2");

        worksheet.Cells[row, 2].Style.Font.Color.SetColor(saldo >= 0 ? Color.Green : Color.Red);

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        var excelBytes = await package.GetAsByteArrayAsync();
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            "RelatorioSaldoPagarReceber.xlsx");
    }
    
    public async Task<IActionResult> ExportarTransacoesCartaoExcel()
    {
        var relatorioCartao = await GenerateTransacoesCartao();

        using var package = new ExcelPackage();
        
        foreach (var cartaoViewModel in relatorioCartao)
        {
            var worksheet = package.Workbook.Worksheets.Add($"Cartão {cartaoViewModel.Cartao.FinalCartao}");

            worksheet.Cells[1, 1].Value = "Categoria";
            worksheet.Cells[1, 2].Value = "Data";
            worksheet.Cells[1, 3].Value = "Descrição";
            worksheet.Cells[1, 4].Value = "Valor (R$)";
            worksheet.Cells[1, 5].Value = "Parcelada";
            worksheet.Cells[1, 6].Value = "Número de Parcelas";

            using (var range = worksheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            var row = 2;
            foreach (var compra in cartaoViewModel.Compras)
            {
                worksheet.Cells[row, 1].Value = compra.Categoria;
                worksheet.Cells[row, 2].Value = compra.DataCompra.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 3].Value = compra.Descricao;
                worksheet.Cells[row, 4].Value = compra.Valor.ToString("F2");
                worksheet.Cells[row, 5].Value = compra.Parcelada ? "Sim" : "Não";
                worksheet.Cells[row, 6].Value = compra.Parcelada ? compra.NumeroParcelas.ToString() : "-";
                row++;
            }

            worksheet.Cells[row, 1].Value = "Total Gasto";
            worksheet.Cells[row, 4].Value = cartaoViewModel.Compras.Sum(c => c.Valor).ToString("F2");
            
            worksheet.Cells[row + 1, 1].Value = "Limite Disponível";
            worksheet.Cells[row + 1, 4].Value = cartaoViewModel.Cartao.LimiteDisponivel.ToString("F2");

            using (var totalRange = worksheet.Cells[row, 1, row + 1, 6])
            {
                totalRange.Style.Font.Bold = true;
                totalRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                totalRange.Style.Fill.BackgroundColor.SetColor(Color.LightYellow);
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        var excelBytes = await package.GetAsByteArrayAsync();
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            "RelatorioTransacoesCartao.xlsx");
    }

    private async Task<List<RelatorioDespesasMensaisViewModel>> GenerateDespesasMensais()
    {
        var relatorio = await _context.ContasPagar
            .Where(c => c.Status == false)
            .GroupBy(c => new { c.Categoria, c.DataVencimento.Month, c.DataVencimento.Year })
            .Select(g => new RelatorioDespesasMensaisViewModel
            {
                Categoria = g.Key.Categoria,
                Mes = g.Key.Month,
                Ano = g.Key.Year,
                ValorTotal = g.Sum(x => x.Valor)
            })
            .ToListAsync();
        return relatorio;
    }
    
    private async Task<(List<dynamic>, List<dynamic>, decimal, decimal, decimal)> CalculateSaldoPagarReceber()
    {
        var contasPagar = await _context.ContasPagar
            .GroupBy(c => c.Categoria)
            .Select(g => new
            {
                Categoria = g.Key,
                ValorTotal = g.Sum(c => c.Valor)
            })
            .ToListAsync();

        var contasReceber = await _context.ContasReceber
            .GroupBy(c => c.Categoria)
            .Select(g => new
            {
                Categoria = g.Key,
                ValorTotal = g.Sum(c => c.Valor)
            })
            .ToListAsync();

        var totalPagar = contasPagar.Sum(c => c.ValorTotal);
        var totalReceber = contasReceber.Sum(c => c.ValorTotal);

        var saldo = totalReceber - totalPagar;
        return (contasPagar.Cast<dynamic>().ToList(), contasReceber.Cast<dynamic>().ToList(), totalPagar, totalReceber, saldo);
    }

    private async Task<List<RelatorioCartaoViewModel>> GenerateTransacoesCartao()
    {
        var cartoes = await _context.Cartoes.ToListAsync();

        var comprasCartao = await _context.ComprasCartao
            .Include(c => c.Cartao)
            .ToListAsync();

        var relatorioCartao = cartoes.Select(cartao => new RelatorioCartaoViewModel
        {
            Cartao = cartao,
            Compras = comprasCartao.Where(compra => compra.CartaoId == cartao.Id).ToList()
        }).ToList();
        return relatorioCartao;
    }
}