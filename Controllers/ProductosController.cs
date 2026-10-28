using Lab11_DesAEA_2.Data;
using Lab11_DesAEA_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab11_DesAEA_2.Controllers;

public class ProductosController(AppDbContext db) : Controller
{
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> Listar(string? buscar = null, int? categoriaId = null, string? estado = null)
    {
        var q = db.Productos.Include(p => p.Categoria).AsQueryable();
        if (!string.IsNullOrWhiteSpace(buscar)) q = q.Where(p => p.Nombre.Contains(buscar) || p.Categoria!.Nombre.Contains(buscar));
        if (categoriaId.HasValue) q = q.Where(p => p.CategoriaId == categoriaId.Value);
        if (estado == "bajo") q = q.Where(p => p.Stock < 6);
        if (estado == "agotado") q = q.Where(p => p.Stock == 0);
        return Json(await q.OrderBy(p => p.Nombre).Select(p => new { p.Id, p.Nombre, p.Descripcion, p.Precio, p.Stock, categoria = p.Categoria!.Nombre, p.CategoriaId }).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Categorias() => Json(await db.Categorias.OrderBy(c => c.Nombre).Select(c => new { c.Id, c.Nombre }).ToListAsync());

    [HttpGet]
    public async Task<IActionResult> Resumen() => Json(new
    {
        productos = await db.Productos.CountAsync(),
        unidades = await db.Productos.SumAsync(p => (int?)p.Stock) ?? 0,
        categorias = await db.Categorias.CountAsync(),
        stockBajo = await db.Productos.CountAsync(p => p.Stock < 6)
    });

    [HttpGet]
    public async Task<IActionResult> Paneles() => Json(new
    {
        categorias = await db.Categorias.Select(c => new { c.Nombre, total = c.Productos.Count }).OrderByDescending(c => c.total).ToListAsync(),
        alertas = await db.Productos.Where(p => p.Stock < 6).OrderBy(p => p.Stock).Take(5)
            .Select(p => new { p.Id, p.Nombre, p.Stock, categoria = p.Categoria!.Nombre }).ToListAsync(),
        recientes = await db.Productos.OrderByDescending(p => p.FechaRegistro).Take(4)
            .Select(p => new { p.Nombre, p.FechaRegistro, categoria = p.Categoria!.Nombre }).ToListAsync()
    });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar([FromForm] Producto producto)
    {
        if (!await db.Categorias.AnyAsync(c => c.Id == producto.CategoriaId)) ModelState.AddModelError("CategoriaId", "Selecciona una categoría válida.");
        if (!ModelState.IsValid) return BadRequest(new { mensaje = "Revisa los datos ingresados." });
        if (producto.Id == 0) db.Productos.Add(producto);
        else { var actual = await db.Productos.FindAsync(producto.Id); if (actual is null) return NotFound(); actual.Nombre=producto.Nombre; actual.Descripcion=producto.Descripcion; actual.Precio=producto.Precio; actual.Stock=producto.Stock; actual.CategoriaId=producto.CategoriaId; }
        await db.SaveChangesAsync(); return Json(new { ok = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        var item = await db.Productos.FindAsync(id); if (item is null) return NotFound();
        item.Activo = false; await db.SaveChangesAsync(); return Json(new { ok = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reponer(int id, int cantidad = 5)
    {
        if (cantidad < 1 || cantidad > 500) return BadRequest(new { mensaje = "La reposición debe ser entre 1 y 500 unidades." });
        var item = await db.Productos.FindAsync(id); if (item is null) return NotFound();
        item.Stock += cantidad; await db.SaveChangesAsync(); return Json(new { ok = true, stock = item.Stock });
    }
}
