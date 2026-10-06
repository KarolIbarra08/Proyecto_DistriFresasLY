namespace DistriFresasLY.Api.Contracts.Inventario;

public record ActualizarStockRequest(
    int ProductoId,
    int Cantidad
);