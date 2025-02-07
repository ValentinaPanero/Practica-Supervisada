using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mamma_Pasta.Data;
using Mamma_Pasta.Models;
using Newtonsoft.Json;

namespace Mamma_Pasta.Controllers
{
    public class VentasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var ventas = _context.Ventas
                          .Include(v => v.RengVentas)
                          .AsQueryable();

            // Aplicar filtro por fechas si están presentes
            if (fechaDesde.HasValue)
            {
                ventas = ventas.Where(v => v.Fecha >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                ventas = ventas.Where(v => v.Fecha <= fechaHasta.Value);
            }

            ViewData["FechaDesde"] = fechaDesde?.ToString("yyyy-MM-dd");
            ViewData["FechaHasta"] = fechaHasta?.ToString("yyyy-MM-dd");
            // Ordenar por fecha descendente
            var listaVentas = await ventas.OrderByDescending(v => v.Fecha).ToListAsync();

            return View(listaVentas);
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
            .Include(v => v.RengVentas)
            .ThenInclude(r => r.Productos)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (venta == null)
            {
                return NotFound();
            }
            var vm = new vmVentas
            {
                Ventas = venta,
                RenglonesVentas = venta.RengVentas.ToList()
            };

            return View(vm);
        }

        // GET: Ventas/Create
        public IActionResult Create(int? tipoProductoId)
        {
            var vm = new vmVentas
            {
                Ventas = new Venta { Fecha = DateTime.Now },
                RenglonesVentas = new List<RenglonVenta> { new RenglonVenta() } // Inicializa un renglón vacío
            };

            // Lista de tipos de productos
            ViewBag.TiposProducto = _context.TiposProductos.ToList();

            // Filtrar productos según el tipo seleccionado
            var productos = _context.Productos.AsQueryable();

            if (tipoProductoId.HasValue)
            {
                productos = productos.Where(p => p.TipoProductoId == tipoProductoId.Value);
            }


            // Pasa la lista de productos al ViewBag para poder mostrarlos en la vista con filtrado
            ViewBag.Productos = productos.Select(p => new
            {
                p.Id,
                p.Nombre,
                p.Precio
            }).ToList();

            // Mantener el tipo seleccionado en la vista
            ViewBag.TipoSeleccionado = tipoProductoId;

            return View(vm);
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(vmVentas vm, string productosSeleccionados)
        {
            if (ModelState.IsValid)
            {
                // Mostrar los datos recibidos en la consola para debug
                Console.WriteLine("Datos recibidos: " + productosSeleccionados);

                if (!string.IsNullOrEmpty(productosSeleccionados))
                {
                    var renglones = JsonConvert.DeserializeObject<List<RenglonVenta>>(productosSeleccionados);

                    if (renglones != null && renglones.Any())
                    {
                        // Crear la venta con sus renglones
                        var venta = new Venta
                        {
                            Fecha = vm.Ventas.Fecha,
                            Total = renglones.Sum(r => r.Subtotal),
                            RengVentas = renglones.Select(r => new RenglonVenta
                            {
                                ProductoId = r.Id, // Asegúrate de que se asigna correctamente el ID del producto
                                Cantidad = r.Cantidad,
                                Precio = r.Precio,
                                Subtotal = r.Subtotal,
                                Productos = _context.Productos.FirstOrDefault(p => p.Id == r.Id) // Cargar el producto asociado
                            }).ToList()
                        };

                        _context.Ventas.Add(venta);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se recibieron productos en la venta.");
                    }
                }
            }


            ViewBag.Productos = _context.Productos.ToList();
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Nombre", vm.Productos?.Id);
            return View(vm);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }

            var renglones = await _context.renglonesVentas
                                  .Where(r => r.VentaId == id)
                                  .ToListAsync();

            // Construir el modelo de vista `vmVentas`
            var vm = new vmVentas
            {
                Ventas = venta,
                RenglonesVentas = renglones
            };

            // Pasar la lista de productos al ViewBag (si es necesario para un dropdown en la vista)
            ViewBag.Productos = await _context.Productos.ToListAsync();
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Nombre");
            return View(vm);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, vmVentas vm)
        {
            if (id != vm.Ventas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vm.Ventas);
                    foreach (var renglon in vm.RenglonesVentas)
                    {
                        if (renglon.Id == 0)
                        {
                            // Si el Id es 0, es un nuevo renglón, así que lo agregamos
                            _context.renglonesVentas.Add(renglon);
                        }
                        else
                        {
                            // Si el Id existe, actualizamos el renglón
                            _context.renglonesVentas.Update(renglon);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(vm.Ventas.Id))
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
            ViewBag.Productos = await _context.Productos.ToListAsync();
            ViewData["IdProducto"] = new SelectList(_context.Productos, "Id", "Nombre", vm.Productos.Id);
            return View(vm);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.Include(v => v.RengVentas)
           .ThenInclude(r => r.Productos)
           .FirstOrDefaultAsync(m => m.Id == id);
            if (venta == null)
            {
                return NotFound();
            }
            var vm = new vmVentas
            {
                Ventas = venta,
                RenglonesVentas = venta.RengVentas.ToList()
            };


            return View(vm);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas
            .Include(v => v.RengVentas) // Incluye los renglones para eliminarlos también si es necesario
            .FirstOrDefaultAsync(m => m.Id == id);
            if (venta != null)
            {
                _context.renglonesVentas.RemoveRange(venta.RengVentas); // Elimina renglones asociados
                _context.Ventas.Remove(venta); // Elimina la venta
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.Id == id);
        }
    }
}
