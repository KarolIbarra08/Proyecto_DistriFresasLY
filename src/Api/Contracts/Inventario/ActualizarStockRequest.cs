using System.Text.Json.Serialization;

namespace DistriFresasLY.Api.Contracts.Inventario;

public record ActualizarStockRequest(
    [property: JsonPropertyName("productoId")] int ProductoId,
    [property: JsonPropertyName("cantidad")] int Cantidad
);