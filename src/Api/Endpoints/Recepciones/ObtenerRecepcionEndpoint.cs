using DistriFresasLY.Api.Contracts.Recepciones;
using DistriFresasLY.Api.Extensions;
using DistriFresasLY.Application.Abstractions.Persistence;
using DistriFresasLY.Domain.Common;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public class ObtenerRecepcionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/recepciones/{id:int}", Manejador)
           .WithName("ObtenerRecepcion")
           .WithTags("Recepciones")
           .WithSummary("Obtiene una recepción por su ID aplicando deducciones por productos dañados");
    }

    private static async Task<IResult> Manejador(
        int id,
        IRecepcionRepository recepcionRepository,
        IProductoDanadoRepository productoDanadoRepository, 
        CancellationToken cancellationToken)
    {
        var recepcion = await recepcionRepository.ObtenerPorIdAsync(id, cancellationToken);
        if (recepcion is null)
        {
            return Result.Failure<RecepcionResponse>(
                Error.NotFound("Recepcion.NotFound", $"No se encontró la recepción con ID {id}."))
                .ToHttpResult();
        }


        var productosDanados = await productoDanadoRepository.ObtenerTodosAsync(cancellationToken);


        double totalDescuentos = (double)productosDanados
            .Where(pd => pd.Descuento is not null && 
                         recepcion.Detalles.Any(d => d.IdProducto == pd.ProductoId))
            .Sum(pd => pd.Descuento!.Valor);


        double totalCalculadoOriginal = recepcion.CalcularTotal();
        double valorPagoFinal = Math.Max(0, totalCalculadoOriginal - totalDescuentos);

        var response = new RecepcionResponse(
            recepcion.Id,
            recepcion.PedidoId,
            recepcion.FechaRecepcion,
            valorPagoFinal,                   
            recepcion.ProcesadaEnInventario,
            totalCalculadoOriginal            
        );

        return Result.Success(response).ToHttpResult();
    }
}