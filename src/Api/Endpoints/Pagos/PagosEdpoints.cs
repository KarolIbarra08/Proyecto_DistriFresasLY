using DistriFresasLY.Domain.Entities.Pagos;

namespace DistriFresasLY.Api.Endpoints.Pagos;

public static class PagoDataStore
{
    public static readonly List<PagoCliente> PagosClientesDb = [];
    public static readonly List<PagoProveedor> PagosProveedoresDb = [];
}