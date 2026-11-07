using DistriFresasLY.Application.Abstractions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Infrastructure.Persistence;
using DistriFresasLY.Infrastructure.Persistence.Repositories;
using DistriFresasLY.Infrastructure.Servicios;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DistriFresasLY.Infrastructure;

/// <summary>
/// Registro de los servicios de Infraestructura en el contenedor
/// de Inyección de Dependencias.
///
/// Aquí se relacionan los contratos de Application con sus
/// implementaciones concretas de Infrastructure.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? configuration.GetConnectionString("DistriFresasLY")
            ?? throw new InvalidOperationException(
                "Falta la cadena de conexión en appsettings.json.");

        // Fábrica de conexiones a la base de datos.
        services.AddSingleton<IDbConnectionFactory>(
            new SqliteConnectionFactory(connectionString));

        // Reloj del sistema para registrar fechas en UTC.
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);

        // Usuario actual mientras no exista autenticación.
        services.TryAddScoped<IUsuarioActual, UsuarioActualSistema>();

        // Repositorios.
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IProductoDanadoRepository, ProductoDanadoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
        services.AddScoped<IInventarioRepository, InventarioRepository>();
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IRecepcionRepository, RecepcionRepository>();
        services.AddScoped<IPagoClienteRepository, PagoClienteRepository>();
        services.AddScoped<IPagoProveedorRepository, PagoProveedorRepository>();
        services.AddScoped<IReciboRepository, ReciboRepository>();

        return services;
    }
}