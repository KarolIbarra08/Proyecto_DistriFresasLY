using DistriFresasLY.Api.Contracts.Proveedores;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public static class ProveedorDataStore
{
    public static readonly List<ProveedorResponse> ProveedoresDb =
    [
        new(
            Id: 1,
            Nombre: "Carlos",
            Apellido: "Perez",
            CedulaNit: "900123456-1",
            Telefono: "3101234567",
            Direccion: "Calle 5 # 10-20, Purace",
            NombreEmpresa: "Cultivos El Campestre"
        ),
        new(
            Id: 2,
            Nombre: "Maria Eugenia",
            Apellido: "Gómez",
            CedulaNit: "34567890",
            Telefono: "3129876543",
            Direccion: "Carrera 8 # 12-45, Popayan",
            NombreEmpresa: "AgroFresas del Cauca"
        )
    ];
}