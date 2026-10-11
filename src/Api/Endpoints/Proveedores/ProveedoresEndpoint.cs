using DistriFresasLY.Domain.Entities.Proveedores;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public static class ProveedorDataStore
{
    public static readonly List<Proveedor> ProveedoresDb =
    [
        Proveedor.Create(
            nombre: "Carlos",
            apellido: "Perez",
            cedulaNit: "900123456-1",
            telefono: "3101234567",
            direccion: "Calle 5 # 10-20, Purace",
            nombreEmpresa: "Cultivos El Campestre",
            id: 1).Value,
        Proveedor.Create(
            nombre: "Maria Eugenia",
            apellido: "Gómez",
            cedulaNit: "34567890",
            telefono: "3129876543",
            direccion: "Carrera 8 # 12-45, Popayan",
            nombreEmpresa: "AgroFresas del Cauca",
            id: 2).Value
    ];
}