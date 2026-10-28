using System.ComponentModel.DataAnnotations;
namespace Lab11_DesAEA_2.Models;
public class Categoria
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string Nombre { get; set; } = string.Empty;
    [StringLength(240)] public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
