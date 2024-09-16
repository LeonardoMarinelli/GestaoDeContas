using Microsoft.AspNetCore.Mvc;
using GestaoDeContas.Data;
using GestaoDeContas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeContas.Controllers;

public class ComprasCartaoController : Controller
{
    private readonly ApplicationDbContext _context;
    public ComprasCartaoController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        var comprasCartao = await _context.ComprasCartao
            .Include(c => c.Cartao)
            .ToListAsync();
        return View(comprasCartao);
    }
    
    public IActionResult Create()
    {
        ViewBag.Cartoes = _context.Cartoes.ToList();
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompraCartao compraCartao)
    {
        if (!ModelState.IsValid) return View(compraCartao);
        
        var cartao = await _context.Cartoes.FindAsync(compraCartao.CartaoId);
        if (cartao is null)
        {
            ModelState.AddModelError("", "Cartão não encontrado.");
            return View(compraCartao);
        }
        
        compraCartao.Cartao = cartao;

        if (compraCartao is { Parcelada: true, NumeroParcelas: > 0 })
        {
            compraCartao.ValorParcela = compraCartao.Valor / compraCartao.NumeroParcelas;
            compraCartao.ValorParcela = Math.Round(compraCartao.ValorParcela, 2);
        }
        else
        {
            compraCartao.NumeroParcelas = 1;
            compraCartao.ValorParcela = compraCartao.Valor;
        }

        cartao.TotalGastos += compraCartao.Valor;
        if (cartao.LimiteDisponivel < 0)
        {
            ModelState.AddModelError("", "Limite insuficiente no cartão.");
            return View(compraCartao);
        }

        try
        {
            _context.Add(compraCartao);
            _context.Update(cartao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Erro ao registrar a compra: " + ex.Message);
            return View(compraCartao);
        }
    }
    
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        
        var compraCartao = await _context.ComprasCartao
            .Include(c => c.Cartao)
            .FirstOrDefaultAsync(m => m.Id == id);
        
        if (compraCartao is null) return NotFound();
        
        ViewBag.Cartoes = _context.Cartoes.ToList();
        return View(compraCartao);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CompraCartao compraCartao)
    {
        if (!ModelState.IsValid) return View(compraCartao);
        
        if (id != compraCartao.Id) return NotFound();
        
        if (compraCartao is { Parcelada: true, NumeroParcelas: > 0 })
        {
            compraCartao.ValorParcela = compraCartao.Valor / compraCartao.NumeroParcelas;
            compraCartao.ValorParcela = Math.Round(compraCartao.ValorParcela, 2);
        }
        else
        {
            compraCartao.NumeroParcelas = 1;
            compraCartao.ValorParcela = compraCartao.Valor;
        }
        
        _context.Update(compraCartao);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        
        var compraCartao = await _context.ComprasCartao
            .FirstOrDefaultAsync(m => m.Id == id);
        if (compraCartao is null) return NotFound();
        
        return View(compraCartao);
    }
    
    [HttpPost, ActionName("DeleteConfirmed")]
    [Route("ComprasCartao/DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var compraCartao = await _context.ComprasCartao.FindAsync(id);
        _context.ComprasCartao.Remove(compraCartao!);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> ObterCartao(int id)
    {
        var cartao = await _context.Cartoes.FindAsync(id);
        if (cartao is null) return NotFound();
        
        return Json(cartao);
    }
}