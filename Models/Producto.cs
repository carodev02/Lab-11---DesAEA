using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Lab11_DesAEA_2.Models;
public class Producto
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string Nombre { get; set; } = string.Empty;
    [StringLength(300)] public string? Descripcion { get; set; }
    [Range(0.01, 999999), Column(TypeName = "decimal(10,2)")] public decimal Precio { get; set; }
    [Range(0, 999999)] public int Stock { get; set; }
    [Display(Name = "Categoría")] public int CategoriaId { get; set; }
    public Categoria? Categoria { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
}
