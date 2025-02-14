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
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [AllowAnonymous]
        // GET: Productos
        public async Task<IActionResult> Index(string BusqNombre, int? TipoProductoId, int pagina = 1)
        {
            Paginador paginador = new Paginador
            {
                paginaActual = pagina,
                cantRegistrosPagina = 10,
            };

            var producto = _context.Productos.Include(e => e.tipoProductos).Select(e => e);
            var lista = await _context.Productos.ToListAsync();
            var tipos = await _context.TiposProductos.ToListAsync();
            foreach (var item in lista)
            {
                item.tipoProductos = tipos.Find(e => e.Id == item.TipoProductoId);
            }

            if (!string.IsNullOrEmpty(BusqNombre))
            {
                producto = producto.Where(e => e.Nombre.Contains(BusqNombre));
                paginador.filtros.Add("BusqNombre", BusqNombre);
            }
            if (TipoProductoId.HasValue)
            {
                producto = producto.Where(e => e.TipoProductoId == TipoProductoId);
                paginador.filtros.Add("TipoProductoId", TipoProductoId.ToString());
            }

            paginador.cantRegistros = producto.Count();

            producto = producto
                .Skip(paginador.cantRegistrosPagina * (pagina - 1))
                .Take(paginador.cantRegistrosPagina);

            vmProductos modelo = new vmProductos
            {
                ListProductos = await producto.ToListAsync(),
                ListTiposProd = new SelectList(_context.TiposProductos, "Id", "Tipo"),
                BusqNombre = BusqNombre,
                TipoProductoId = TipoProductoId,
                paginadorVM = paginador
            };


            ViewData["TipoProductoId"] = new SelectList(_context.TiposProductos, "Id", "Tipo");
            return View(modelo);
        }

        public async Task<IActionResult> Import()
        {
            var archivos = HttpContext.Request.Form.Files;
            if (archivos != null && archivos.Count > 0)
            {
                var archivo = archivos[0];
                if (archivo.Length > 0)
                {
                    var pathDestino = Path.Combine(_env.WebRootPath, "Importaciones");
                    var archivoDestino = Guid.NewGuid().ToString().Replace("-", "");
                    archivoDestino += Path.GetExtension(archivo.FileName);
                    var rutaDestino = Path.Combine(pathDestino, archivoDestino);

                    using (var filestream = new FileStream(rutaDestino, FileMode.Create))
                    {
                        archivo.CopyTo(filestream);
                    }

                    using (var file = new FileStream(rutaDestino, FileMode.Open))
                    {
                        List<string> reng = new List<string>();
                        List<Producto> ProductosArch = new List<Producto>();

                        StreamReader fileContent = new StreamReader(file, System.Text.Encoding.Default);
                        do
                        {
                            reng.Add(fileContent.ReadLine());
                        }
                        while (!fileContent.EndOfStream);

                        if (reng.Count() > 0)
                        {
                            foreach (var row in reng)
                            {
                                string[] data = row.Split(';');
                                if (data.Length == 5)
                                {
                                    Producto Productos = new Producto();
                                    Productos.Nombre = data[0].Trim();
                                    Productos.Descripcion = data[1].Trim();
                                    Productos.TipoProductoId = int.Parse(data[2].Trim());
                                    Productos.Precio = int.Parse(data[3].Trim());
                                    ProductosArch.Add(Productos);
                                }

                            }
                            if (ProductosArch.Count() > 0)
                            {
                                _context.AddRange(ProductosArch);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
            var producto = _context.Productos.Include(e => e.tipoProductos);
            return RedirectToAction("Index");
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(e => e.tipoProductos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                var lista = await _context.Productos.ToListAsync();
                var tipos = await _context.TiposProductos.ToListAsync();
                foreach (var item in lista)
                {
                    item.tipoProductos = tipos.Find(e => e.Id == item.TipoProductoId);
                }
                return View(lista);
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            ViewData["TipoProductoId"] = new SelectList(_context.TiposProductos, "Id", "Tipo");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,TipoProductoId,Precio,Imagen")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                var archivos = HttpContext.Request.Form.Files;
                if (archivos != null && archivos.Count > 0)
                {
                    var archivoImagen = archivos[0];
                    if (archivoImagen.Length > 0)
                    {
                        var rutaDestino = Path.Combine(_env.WebRootPath, "Imagenes", "Productos");
                        var extension = Path.GetExtension(archivoImagen.FileName);
                        var archivoDestino = producto.Nombre.ToString() + extension;
                        //var archivoDestino = Guid.NewGuid().ToString().Replace("-", "") + extension; //NOMBRE ALEATORIO

                        using (var filestream = new FileStream(Path.Combine(rutaDestino, archivoDestino), FileMode.Create))
                        {
                            archivoImagen.CopyTo(filestream);
                            producto.Imagen = archivoDestino;
                        }
                    }
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TipoProductoId"] = new SelectList(_context.TiposProductos, "Id", "Tipo", producto.TipoProductoId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["TipoProductoId"] = new SelectList(_context.TiposProductos, "Id", "Tipo", producto.TipoProductoId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,TipoProductoId,Precio,Imagen")] Producto producto)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var archivos = HttpContext.Request.Form.Files;
                    string imagenAnterior = await _context.Productos
                        .Where(p => p.Id == producto.Id)
                        .Select(p => p.Imagen)
                        .FirstOrDefaultAsync();

                    if (archivos != null && archivos.Count > 0)
                    {
                        var archivoImagen = archivos[0];
                        if (archivoImagen.Length > 0)
                        {
                            var rutaDestino = Path.Combine(_env.WebRootPath, "Imagenes", "Productos");
                            var extension = Path.GetExtension(archivoImagen.FileName);
                            var archivoDestino = producto.Nombre.ToString() + extension;

                            using (var filestream = new FileStream(Path.Combine(rutaDestino, archivoDestino), FileMode.Create))
                            {
                                archivoImagen.CopyTo(filestream);

                                // Eliminar la imagen anterior si existía
                                if (!string.IsNullOrEmpty(imagenAnterior))
                                {
                                    string fotoAnterior = Path.Combine(rutaDestino, imagenAnterior);
                                    if (System.IO.File.Exists(fotoAnterior))
                                        System.IO.File.Delete(fotoAnterior);
                                }

                                producto.Imagen = archivoDestino;
                            }
                        }
                    }
                    else
                    {
                        // Si no se subió una nueva imagen, conservar la anterior
                        producto.Imagen = imagenAnterior;
                    }

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["TipoProductoId"] = new SelectList(_context.TiposProductos, "Id", "Tipo", producto.TipoProductoId);
            return View(producto);
        }


        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.tipoProductos)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
