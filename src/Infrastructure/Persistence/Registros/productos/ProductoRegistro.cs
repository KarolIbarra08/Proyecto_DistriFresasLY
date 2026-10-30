using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Infrastructure.Persistence.Registros;

internal sealed class ProductoRegistro
{
    public int Id_Producto { get; init; }
    public string Tipo_Producto { get; init; } = string.Empty; 

    public int? Calibre { get; init; }
    public string? Calidad { get; init; }
    public double? Peso { get; init; }

    public string? Nombre { get; init; }
    public string? Tipo_Insumo { get; init; }
    public string? Unidad_Medida { get; init; }

    public string Descripcion { get; init; } = string.Empty;

    public Producto ToEntidad()
    {
        var tipoNormalizado = Tipo_Producto?.Trim().ToLowerInvariant();

        return tipoNormalizado switch
        {
            "fresa" => ProductoFresa.Rehidratar(
                Id_Producto,
                Calibre ?? 0,
                Calidad ?? string.Empty,
                Peso ?? 0,
                Descripcion),

            "insumo" => ProductoInsumo.Rehidratar(
                Id_Producto,
                Nombre ?? string.Empty,
                Tipo_Insumo ?? string.Empty,
                Unidad_Medida ?? string.Empty,
                Descripcion),

            _ => throw new InvalidOperationException($"Tipo de producto no reconocido: '{Tipo_Producto}'")
        };
    }
}