using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mamma_Pasta.Data;
using Mamma_Pasta.Models;
using Microsoft.AspNetCore.Authorization;

namespace Mamma_Pasta.Controllers
{

    public class TipoDePagosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoDePagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoDePagos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposDePago.ToListAsync());
        }

        // GET: TipoDePagos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDePago = await _context.TiposDePago
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDePago == null)
            {
                return NotFound();
            }

            return View(tipoDePago);
        }

        // GET: TipoDePagos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoDePagos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo")] TipoDePago tipoDePago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoDePago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDePago);
        }

        // GET: TipoDePagos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDePago = await _context.TiposDePago.FindAsync(id);
            if (tipoDePago == null)
            {
                return NotFound();
            }
            return View(tipoDePago);
        }

        // POST: TipoDePagos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo")] TipoDePago tipoDePago)
        {
            if (id != tipoDePago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoDePago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoDePagoExists(tipoDePago.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDePago);
        }

        // GET: TipoDePagos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDePago = await _context.TiposDePago
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDePago == null)
            {
                return NotFound();
            }

            return View(tipoDePago);
        }

        // POST: TipoDePagos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoDePago = await _context.TiposDePago.FindAsync(id);
            if (tipoDePago != null)
            {
                _context.TiposDePago.Remove(tipoDePago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoDePagoExists(int id)
        {
            return _context.TiposDePago.Any(e => e.Id == id);
        }
    }
}
