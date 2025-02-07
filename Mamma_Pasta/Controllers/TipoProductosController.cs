using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mamma_Pasta.Data;
using Mamma_Pasta.Models;

namespace Mamma_Pasta.Controllers
{
    public class TipoProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TipoProductos
        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposProductos.ToListAsync());
        }

        // GET: TipoProductos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoProducto = await _context.TiposProductos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoProducto == null)
            {
                return NotFound();
            }

            return View(tipoProducto);
        }

        // GET: TipoProductos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoProductos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo")] TipoProducto tipoProducto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoProducto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoProducto);
        }

        // GET: TipoProductos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoProducto = await _context.TiposProductos.FindAsync(id);
            if (tipoProducto == null)
            {
                return NotFound();
            }
            return View(tipoProducto);
        }

        // POST: TipoProductos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo")] TipoProducto tipoProducto)
        {
            if (id != tipoProducto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoProducto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoProductoExists(tipoProducto.Id))
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
            return View(tipoProducto);
        }

        // GET: TipoProductos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoProducto = await _context.TiposProductos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoProducto == null)
            {
                return NotFound();
            }

            return View(tipoProducto);
        }

        // POST: TipoProductos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoProducto = await _context.TiposProductos.FindAsync(id);
            if (tipoProducto != null)
            {
                _context.TiposProductos.Remove(tipoProducto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoProductoExists(int id)
        {
            return _context.TiposProductos.Any(e => e.Id == id);
        }
    }
}
