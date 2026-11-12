using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;

namespace OficinaEF.Controllers
{
    [Authorize]
    public class EquipamentosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Equipamentos
        public async Task<IActionResult> Index()
        {
            var equipamentos = _context.Equipamentos
                .Include(e => e.Cliente);

            return View(await equipamentos.ToListAsync());
        }

        // GET: Equipamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var equipamento = await _context.Equipamentos
                .Include(e => e.Cliente)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipamento == null)
                return NotFound();

            return View(equipamento);
        }

        // GET: Equipamentos/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(
                _context.Clientes, "Id", "Nome");

            return View();
        }

        // POST: Equipamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Tipo,Modelo,ClienteId")] Equipamento equipamento)
        {
            if (ModelState.IsValid &&
                !await _context.Clientes.AnyAsync(c => c.Id == equipamento.ClienteId))
            {
                ModelState.AddModelError(
                    nameof(Equipamento.ClienteId),
                    "O cliente selecionado não foi encontrado.");
            }

            if (ModelState.IsValid)
            {
                _context.Equipamentos.Add(equipamento);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClienteId"] = new SelectList(
                _context.Clientes, "Id", "Nome", equipamento.ClienteId);

            return View(equipamento);
        }

        // POST: Equipamentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [FromRoute] int id,
            [Bind("Tipo,Modelo,ClienteId")] Equipamento equipamento)
        {
            var equipamentoBanco = await _context.Equipamentos.FindAsync(id);

            if (equipamentoBanco == null)
                return NotFound();

            if (ModelState.IsValid &&
                !await _context.Clientes.AnyAsync(c => c.Id == equipamento.ClienteId))
            {
                ModelState.AddModelError(
                    nameof(Equipamento.ClienteId),
                    "O cliente selecionado não foi encontrado.");
            }

            if (ModelState.IsValid)
            {
                equipamentoBanco.Tipo = equipamento.Tipo;
                equipamentoBanco.Modelo = equipamento.Modelo;
                equipamentoBanco.ClienteId = equipamento.ClienteId;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Equipamentos.AnyAsync(e => e.Id == id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            equipamento.Id = id;

            ViewData["ClienteId"] = new SelectList(
                _context.Clientes, "Id", "Nome", equipamento.ClienteId);

            return View(equipamento);
        }

        // GET: Equipamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var equipamento = await _context.Equipamentos
                .Include(e => e.Cliente)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipamento == null)
                return NotFound();

            return View(equipamento);
        }

        // POST: Equipamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipamento = await _context.Equipamentos.FindAsync(id);

            if (equipamento != null)
                _context.Equipamentos.Remove(equipamento);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EquipamentoExists(int id)
        {
            return _context.Equipamentos.Any(e => e.Id == id);
        }
    }
}