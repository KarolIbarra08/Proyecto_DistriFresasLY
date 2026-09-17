using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DistriFresasLY.Api.Endpoints;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray();

        services.TryAddEnumerable(endpointDescriptors);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, string? prefix = "/api")
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

       
        IEndpointRouteBuilder routeBuilder = string.IsNullOrWhiteSpace(prefix)
            ? app
            : app.MapGroup(prefix);

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(routeBuilder);
        }

        return app;
    }
}
