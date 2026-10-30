using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Infrastructure.Persistence;
using DistriFresasLY.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistriFresasLY.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? configuration.GetConnectionString("DistriFresasLY")
            ?? throw new InvalidOperationException("Falta la cadena de conexión en appsettings.json.");

        // Fábrica de conexiones para la base de datos
        services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(connectionString));

        // Repositorios
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IProductoDanadoRepository, ProductoDanadoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolRepository, RolRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();

        return services;
    }
}