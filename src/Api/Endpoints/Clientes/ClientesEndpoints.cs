using DistriFresasLY.Domain.Entities.Cliente;

namespace DistriFresasLY.Api.Endpoints.Clientes;

public static class ClienteDataStore
{
    public static readonly List<Cliente> ClientesDb =
    [
        Cliente.Create(
            nombre: "Fruver El Campestre",
            cedulaNit: "900123456-1",
            telefono: "3101234567",
            direccion: "Calle 5 # 10-20, Purace",
            tipoNegocio: "Supermercado",
            id: 1).Value,
        Cliente.Create(
            nombre: "Maria Eugenia Gomez",
            cedulaNit: "34567890",
            telefono: "3129876543",
            direccion: "Carrera 8 # 12-45, Popayan",
            tipoNegocio: "Tienda de Barrio",
            id: 2).Value
    ];
}