using Lab11_DesAEA_2.Models;
using Microsoft.EntityFrameworkCore;
namespace Lab11_DesAEA_2.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Producto> Productos => Set<Producto>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>().HasQueryFilter(x => x.Activo);
        modelBuilder.Entity<Producto>().HasQueryFilter(x => x.Activo);
        modelBuilder.Entity<Categoria>().HasIndex(x => x.Nombre).IsUnique();
        modelBuilder.Entity<Producto>().HasOne(x => x.Categoria).WithMany(x => x.Productos)
            .HasForeignKey(x => x.CategoriaId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Cuadernos", Descripcion = "Cuadernos y papel" },
            new Categoria { Id = 2, Nombre = "Escritura", Descripcion = "Lapiceros, lápices y marcadores" },
            new Categoria { Id = 3, Nombre = "Accesorios", Descripcion = "Accesorios para estudio y oficina" });
        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Cuaderno universitario", Descripcion = "100 hojas cuadriculadas", Precio = 8.50m, Stock = 24, CategoriaId = 1, FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Producto { Id = 2, Nombre = "Lapicero tinta azul", Descripcion = "Punta fina de 0.7 mm", Precio = 2.20m, Stock = 58, CategoriaId = 2, FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Producto { Id = 3, Nombre = "Resaltador pastel", Descripcion = "Tinta violeta, secado rápido", Precio = 4.90m, Stock = 3, CategoriaId = 2, FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Producto { Id = 4, Nombre = "Organizador de escritorio", Descripcion = "Compartimentos para accesorios", Precio = 18.00m, Stock = 12, CategoriaId = 3, FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
    }
}
