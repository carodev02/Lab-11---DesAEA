# Catálogo de productos

Aplicación ASP.NET Core MVC con SQL Server, Entity Framework Core Code First y operaciones AJAX.

## Requisitos

- Visual Studio 2022 o posterior con la carga de trabajo ASP.NET y desarrollo web.
- .NET 10 SDK y SQL Server LocalDB.

## Base de datos

Abre `Lab11Catalogo.slnx` en Visual Studio y restaura los paquetes NuGet. Al iniciar la aplicación, Entity Framework aplica las migraciones pendientes automáticamente y crea la base `Lab11Catalogo` con categorías y productos de ejemplo.

Para aplicar cambios desde la Consola del Administrador de Paquetes:

```powershell
Update-Database
```

Para cambios posteriores en los modelos, crea una nueva migración con `Add-Migration NombreDelCambio` y luego ejecuta `Update-Database`.

La conexión predeterminada usa `(localdb)\mssqllocaldb` y crea `Lab11Catalogo`. Para otro servidor, cambia `ConnectionStrings:DefaultConnection` en `appsettings.json`.

## Funciones

- Catálogo de productos con búsqueda, edición y alta mediante AJAX.
- Categorías como entidad relacionada y selección desde el formulario.
- Desactivación lógica: los registros se conservan con `Activo = false` y no se muestran en consultas normales.
- Validación en servidor, antiforgery y restricción de eliminación de categorías con productos vinculados.

No se usan procedimientos almacenados ni borrados físicos.
