using DistriFresasLY.Api.Contracts.Pagos.Clientes;
using DistriFresasLY.Api.Contracts.Pagos.Proveedores;

namespace DistriFresasLY.Api.Endpoints.Pagos;

public static class PagoDataStore
{
    public static readonly List<PagoClienteResponse> PagosClientesDb = [];
    public static readonly List<PagoProveedorResponse> PagosProveedoresDb = [];
}