using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Application.Inventarios;

public sealed class ConsultarTodoInventarioUseCase
{
    private readonly IInventarioRepository _inventarios;

    public ConsultarTodoInventarioUseCase(IInventarioRepository inventarios) => _inventarios = inventarios;

    public async Task<Result<IReadOnlyList<InventarioDetalleDto>>> EjecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var lista = await _inventarios.ObtenerTodosAsync(cancellationToken);

        var dtos = lista.Select(i => new InventarioDetalleDto(
            i.Id,
            i.ProductoId,
            i.CantidadDisponible,
            i.FechaActualizacion)).ToList();

        return Result.Success<IReadOnlyList<InventarioDetalleDto>>(dtos);
    }
}