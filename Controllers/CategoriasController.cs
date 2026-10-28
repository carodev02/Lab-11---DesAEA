using Lab11_DesAEA_2.Data;
using Lab11_DesAEA_2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Lab11_DesAEA_2.Controllers;
public class CategoriasController(AppDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Listar() => Json(await db.Categorias
        .Select(c => new { c.Id, c.Nombre, c.Descripcion, productos = c.Productos.Count })
        .OrderBy(c => c.Nombre).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar([FromForm] Categoria categoria)
    {
        if (!ModelState.IsValid) return BadRequest(new { mensaje="Ingresa un nombre válido." });
        if (await db.Categorias.AnyAsync(c => c.Nombre == categoria.Nombre && c.Id != categoria.Id))
            return Conflict(new { mensaje="Ya existe una categoría con ese nombre." });
        if (categoria.Id == 0) db.Categorias.Add(categoria);
        else { var actual=await db.Categorias.FindAsync(categoria.Id); if(actual is null)return NotFound(); actual.Nombre=categoria.Nombre; actual.Descripcion=categoria.Descripcion; }
        await db.SaveChangesAsync(); return Json(new { ok=true });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Desactivar(int id)
    {
        if(await db.Productos.AnyAsync(p=>p.CategoriaId==id)) return Conflict(new { mensaje="La categoría tiene productos activos." });
        var item=await db.Categorias.FindAsync(id); if(item is null)return NotFound(); item.Activo=false; await db.SaveChangesAsync(); return Json(new {ok=true});
    }
}
