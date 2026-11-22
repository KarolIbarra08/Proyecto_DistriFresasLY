using DistriFresasLY.Application.ProductosDanados;

namespace DistriFresasLY.Api.Contracts.ProductosDanados;

public static class ProductoDanadoMappingExtensions
{
    public static RegistrarProductoDanadoCommand ToCommand(this RegistrarProductoDanadoRequest request) => new(
        request.ProductoId,
        request.TipoProducto,
        request.Cantidad,
        request.Identificacion,
        request.Valor,
        request.FechaProductoDanado,
        request.Motivo);

    public static ActualizarProductoDanadoCommand ToCommand(this ActualizarProductoDanadoRequest request, int id) => new(
        id,
        request.Cantidad,
        request.Identificacion,
        request.Valor,
        request.FechaProductoDanado,
        request.Motivo);

    public static ProductoDanadoResponse ToResponse(this ProductoDanadoDetalle d) => new(
        d.Id,
        d.ProductoId,
        d.TipoProducto,
        d.Cantidad,
        d.Identificacion,
        d.Valor,
        d.FechaProductoDanado,
        d.Motivo);
}