using Microsoft.AspNetCore.Mvc;
using GestaoDeContas.Data;
using GestaoDeContas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeContas.Controllers;

public class CartaoController : Controller
{
    private readonly ApplicationDbContext _context;

        public CartaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Cartoes.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cartao cartao)
        {
            if (!ModelState.IsValid) return View(cartao);

            _context.Add(cartao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var cartao = await _context.Cartoes.FindAsync(id);
            if (cartao is null) return NotFound();

            return View(cartao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cartao cartao)
        {
            if (!ModelState.IsValid) return View(cartao);
            if (id != cartao.Id) return NotFound();

            _context.Update(cartao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var cartao = await _context.Cartoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartao is null) return NotFound();

            return View(cartao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartao = await _context.Cartoes.FindAsync(id);
            if (cartao is null) return RedirectToAction(nameof(Index));
            
            _context.Cartoes.Remove(cartao);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
}