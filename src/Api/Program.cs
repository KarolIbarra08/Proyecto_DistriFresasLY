using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DistriFresasLY.Api.Endpoints;
using DistriFresasLY.Api.Middleware;
using DistriFresasLY.Application.Clientes;
using DistriFresasLY.Application.DetalleRecepciones;
using DistriFresasLY.Application.Inventarios;
using DistriFresasLY.Application.Pagos;
using DistriFresasLY.Application.Pedidos;
using DistriFresasLY.Application.Productos;
using DistriFresasLY.Application.ProductosDanados;
using DistriFresasLY.Application.Proveedores;
using DistriFresasLY.Application.Recibos;
using DistriFresasLY.Application.Recepciones;
using DistriFresasLY.Application.Roles;
using DistriFresasLY.Application.Usuarios;
using DistriFresasLY.Application.Ventas;
using DistriFresasLY.Domain.Common;
using DistriFresasLY.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new CustomDateTimeConverter());
});

builder.Services.AddInfrastructure(builder.Configuration);

// Módulo Clientes
builder.Services.AddScoped<ObtenerClientesUseCase>();
builder.Services.AddScoped<ObtenerClientePorIdUseCase>();
builder.Services.AddScoped<CrearClienteUseCase>();
builder.Services.AddScoped<ActualizarClienteUseCase>();
builder.Services.AddScoped<EliminarClienteUseCase>();

// Módulo Roles
builder.Services.AddScoped<ObtenerRolesUseCase>();
builder.Services.AddScoped<CrearRolUseCase>();
builder.Services.AddScoped<ActualizarRolUseCase>();
builder.Services.AddScoped<EliminarRolUseCase>();
builder.Services.AddScoped<AsignarPermisoUseCase>();
builder.Services.AddScoped<VerificarAccesoUseCase>();

// Módulo Usuarios
builder.Services.AddScoped<ObtenerUsuariosUseCase>();
builder.Services.AddScoped<ObtenerUsuarioPorIdUseCase>();
builder.Services.AddScoped<CrearUsuarioUseCase>();
builder.Services.AddScoped<ActualizarUsuarioUseCase>();
builder.Services.AddScoped<EliminarUsuarioUseCase>();
builder.Services.AddScoped<IniciarSesionUseCase>();

// Módulo Pedidos
builder.Services.AddScoped<ObtenerPedidosUseCase>();
builder.Services.AddScoped<ObtenerPedidoPorIdUseCase>();
builder.Services.AddScoped<RegistrarPedidoUseCase>();
builder.Services.AddScoped<ActualizarPedidoUseCase>();
builder.Services.AddScoped<EliminarPedidoUseCase>();

// Módulo Ventas
builder.Services.AddScoped<ListarVentasUseCase>();
builder.Services.AddScoped<ObtenerVentaUseCase>();
builder.Services.AddScoped<ObtenerDetallesVentaUseCase>();
builder.Services.AddScoped<RegistrarVentaUseCase>();
builder.Services.AddScoped<ActualizarVentaUseCase>();
builder.Services.AddScoped<EliminarVentaUseCase>();

// Módulo Pagos (Clientes y Proveedores)
builder.Services.AddScoped<ListarPagosClientesUseCase>();
builder.Services.AddScoped<ObtenerPagoClienteUseCase>();
builder.Services.AddScoped<RegistrarPagoClienteUseCase>();
builder.Services.AddScoped<ActualizarPagoClienteUseCase>();
builder.Services.AddScoped<EliminarPagoClienteUseCase>();

builder.Services.AddScoped<ListarPagosProveedoresUseCase>();
builder.Services.AddScoped<ObtenerPagoProveedorUseCase>();
builder.Services.AddScoped<RegistrarPagoProveedorUseCase>();
builder.Services.AddScoped<ActualizarPagoProveedorUseCase>();
builder.Services.AddScoped<EliminarPagoProveedorUseCase>();

// Módulo Recibos
builder.Services.AddScoped<ListarRecibosUseCase>();
builder.Services.AddScoped<GenerarReciboUseCase>();
builder.Services.AddScoped<ImprimirReciboUseCase>();

// Módulo Proveedores
builder.Services.AddScoped<ObtenerProveedoresUseCase>();
builder.Services.AddScoped<ObtenerProveedorPorIdUseCase>();
builder.Services.AddScoped<CrearProveedorUseCase>();
builder.Services.AddScoped<ActualizarProveedorUseCase>();
builder.Services.AddScoped<EliminarProveedorUseCase>();

// Módulo Productos (Fresa e Insumo)
builder.Services.AddScoped<ObtenerProductosFresaUseCase>();
builder.Services.AddScoped<ObtenerProductoFresaPorIdUseCase>();
builder.Services.AddScoped<RegistrarProductoFresaUseCase>();
builder.Services.AddScoped<ActualizarProductoFresaUseCase>();
builder.Services.AddScoped<EliminarProductoFresaUseCase>();

builder.Services.AddScoped<ObtenerProductosInsumoUseCase>();
builder.Services.AddScoped<ObtenerProductoInsumoPorIdUseCase>();
builder.Services.AddScoped<RegistrarProductoInsumoUseCase>();
builder.Services.AddScoped<ActualizarProductoInsumoUseCase>();
builder.Services.AddScoped<EliminarProductoInsumoUseCase>();

// Módulo Productos Dañados
builder.Services.AddScoped<ObtenerProductosDanadosUseCase>();
builder.Services.AddScoped<ObtenerProductoDanadoPorIdUseCase>();
builder.Services.AddScoped<RegistrarProductoDanadoUseCase>();
builder.Services.AddScoped<ActualizarProductoDanadoUseCase>();
builder.Services.AddScoped<EliminarProductoDanadoUseCase>();
builder.Services.AddScoped<CalcularDescuentoProductoDanadoUseCase>();

// Módulo Recepciones y DetalleRecepciones
builder.Services.AddScoped<ListarRecepcionesUseCase>();
builder.Services.AddScoped<ObtenerRecepcionUseCase>();
builder.Services.AddScoped<RegistrarRecepcionUseCase>();
builder.Services.AddScoped<ActualizarRecepcionUseCase>();
builder.Services.AddScoped<EliminarRecepcionUseCase>();
builder.Services.AddScoped<ProcesarRecepcionUseCase>();
builder.Services.AddScoped<CalcularTotalRecepcionUseCase>();
builder.Services.AddScoped<ObtenerDetallesRecepcionUseCase>();

builder.Services.AddScoped<ListarDetallesRecepcionUseCase>();
builder.Services.AddScoped<ObtenerDetalleRecepcionUseCase>();
builder.Services.AddScoped<RegistrarDetalleRecepcionUseCase>();
builder.Services.AddScoped<ActualizarDetalleRecepcionUseCase>();
builder.Services.AddScoped<EliminarDetalleRecepcionUseCase>();

// Módulo Inventario
builder.Services.AddScoped<ConsultarTodoInventarioUseCase>();
builder.Services.AddScoped<ConsultarStockUseCase>();
builder.Services.AddScoped<AjustarStockUseCase>();
builder.Services.AddScoped<AumentarStockUseCase>();
builder.Services.AddScoped<DisminuirStockUseCase>();

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddEndpoints(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("DistriFresasLY API")
               .WithTheme(ScalarTheme.Moon)
               .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();
app.MapEndpoints();
app.Run();

public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? dateString = reader.GetString();

            if (string.IsNullOrWhiteSpace(dateString))
                return default;

            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateInvariant))
            {
                return dateInvariant.Kind == DateTimeKind.Utc
                    ? ColombiaTime.AColombia(dateInvariant)
                    : DateTime.SpecifyKind(dateInvariant, DateTimeKind.Unspecified);
            }

            if (DateTime.TryParse(dateString, CultureInfo.CurrentCulture, DateTimeStyles.RoundtripKind, out var dateLocal))
            {
                return dateLocal.Kind == DateTimeKind.Utc
                    ? ColombiaTime.AColombia(dateLocal)
                    : DateTime.SpecifyKind(dateLocal, DateTimeKind.Unspecified);
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            var ms = reader.GetInt64();
            if (ms <= 0)
                return default;

            return ColombiaTime.AColombia(DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime);
        }

        throw new JsonException("El valor de fecha ingresado no tiene un formato válido.");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var enColombia = value.Kind == DateTimeKind.Utc
            ? ColombiaTime.AColombia(value)
            : DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

        var conOffset = new DateTimeOffset(enColombia, ColombiaTime.Offset);

        writer.WriteStringValue(conOffset.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture));
    }
}