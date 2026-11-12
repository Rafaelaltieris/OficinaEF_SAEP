using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OficinaEF.Data;
using OficinaEF.Models;

namespace OficinaEF.Controllers
{
    [Authorize]
    public class AgendamentosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgendamentosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Agendamentos
        public async Task<IActionResult> Index()
        {
            var agendamentos = _context.Agendamentos
                .Include(a => a.Equipamento)
                .ThenInclude(e => e.Cliente);

            return View(await agendamentos.ToListAsync());
        }

        // GET: Agendamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var agendamento = await _context.Agendamentos
                .Include(a => a.Equipamento)
                .ThenInclude(e => e.Cliente)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }

        // GET: Agendamentos/Create
        public IActionResult Create()
        {
            CarregarEquipamentos();
            return View();
        }

        // POST: Agendamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Data,Servico,EquipamentoId")]
            Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(agendamento);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            CarregarEquipamentos(agendamento.EquipamentoId);
            return View(agendamento);
        }

        // GET: Agendamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var agendamento = await _context.Agendamentos.FindAsync(id);

            if (agendamento == null)
                return NotFound();

            CarregarEquipamentos(agendamento.EquipamentoId);
            return View(agendamento);
        }

        // POST: Agendamentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,Data,Servico,EquipamentoId")]
            Agendamento agendamento)
        {
            if (id != agendamento.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agendamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgendamentoExists(agendamento.Id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            CarregarEquipamentos(agendamento.EquipamentoId);
            return View(agendamento);
        }

        // GET: Agendamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var agendamento = await _context.Agendamentos
                .Include(a => a.Equipamento)
                .ThenInclude(e => e.Cliente)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
                return NotFound();

            return View(agendamento);
        }

        // POST: Agendamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agendamento = await _context.Agendamentos.FindAsync(id);

            if (agendamento != null)
                _context.Agendamentos.Remove(agendamento);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void CarregarEquipamentos(int? equipamentoId = null)
        {
            var equipamentos = _context.Equipamentos
                .Include(e => e.Cliente)
                .AsEnumerable()
                .Select(e => new
                {
                    e.Id,
                    Descricao = $"{e.Tipo} - {e.Modelo} ({e.Cliente.Nome})"
                })
                .ToList();

            ViewData["EquipamentoId"] = new SelectList(
                equipamentos,
                "Id",
                "Descricao",
                equipamentoId);
        }

        private bool AgendamentoExists(int id)
        {
            return _context.Agendamentos.Any(a => a.Id == id);
        }
    }
}