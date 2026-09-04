using DistriFresasLY.Api.Contracts.Clientes;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public static class ClienteDataStore
{
    public static readonly List<ClienteResponse> ClientesDb =
    [
        new(
            Id: 1,
            Nombre: "Fruver El Campestre",
            CedulaNit: "900123456-1",
            Telefono: "3101234567",
            Direccion: "Calle 5 # 10-20, Puracé",
            TipoNegocio: "Supermercado"
        ),
        new(
            Id: 2,
            Nombre: "María Eugenia Gómez",
            CedulaNit: "34567890",
            Telefono: "3129876543",
            Direccion: "Carrera 8 # 12-45, Popayán",
            TipoNegocio: "Tienda de Barrio"
        )
    ];
}