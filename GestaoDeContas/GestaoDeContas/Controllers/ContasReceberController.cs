using Microsoft.AspNetCore.Mvc;
using GestaoDeContas.Data;
using GestaoDeContas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeContas.Controllers;

public class ContasReceberController : Controller
{
    private readonly ApplicationDbContext _context;
    
    public ContasReceberController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
        return View(await _context.ContasReceber.ToListAsync());
    }
    
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [Route("ContasReceber/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContaReceber contaReceber)
    {
        if (!ModelState.IsValid) return View(contaReceber);
        
        try
        {
            _context.Add(contaReceber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Não foi possível salvar a conta. Erro: " + ex.Message);
            return View(contaReceber);
        }
    }
    
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        
        var contaReceber = await _context.ContasReceber.FindAsync(id);
        if (contaReceber is null) return NotFound();
        
        return View(contaReceber);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContaReceber contaReceber)
    {
        if (!ModelState.IsValid) return View(contaReceber);
        
        if (id != contaReceber.Id) return NotFound();
        
        _context.Update(contaReceber);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        
        var contaReceber = await _context.ContasReceber
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contaReceber is null) return NotFound();
        
        return View(contaReceber);
    }
    
    [HttpPost, ActionName("DeleteConfirmed")]
    [Route("ContasReceber/DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contaReceber = await _context.ContasReceber.FindAsync(id);
        
        _context.ContasReceber.Remove(contaReceber!);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
}