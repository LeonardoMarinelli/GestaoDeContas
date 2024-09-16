using Microsoft.AspNetCore.Mvc;
using GestaoDeContas.Data;
using GestaoDeContas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeContas.Controllers;

public class ContasPagarController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContasPagarController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.ContasPagar.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("ContasPagar/Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContaPagar contaPagar)
    {
        if (!ModelState.IsValid) return View(contaPagar);
        
        try
        {
            _context.Add(contaPagar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Não foi possível salvar a conta. Erro: " + ex.Message);
            return View(contaPagar);
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        
        var contaPagar = await _context.ContasPagar.FindAsync(id);
        if (contaPagar is null) return NotFound();
        
        return View(contaPagar);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContaPagar contaPagar)
    {
        if (!ModelState.IsValid) return View(contaPagar);
        
        if (id != contaPagar.Id) return NotFound();
        
        _context.Update(contaPagar);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        
        var contaPagar = await _context.ContasPagar
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contaPagar is null) return NotFound();
        
        return View(contaPagar);
    }

    [HttpPost, ActionName("DeleteConfirmed")]
    [Route("ContasPagar/DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contaPagar = await _context.ContasPagar.FindAsync(id);
        _context.ContasPagar.Remove(contaPagar!);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}