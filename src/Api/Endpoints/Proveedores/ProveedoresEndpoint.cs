using DistriFresasLY.Api.Contracts.Proveedores;

namespace DistriFresasLY.Api.Endpoints.Proveedores;

public static class ProveedorDataStore
{
    public static readonly List<ProveedorResponse> ProveedoresDb =
    [
        new(
            Id: 1,
            Nombre: "Carlos",
            Apellido: "Pérez",
            CedulaNit: "900123456-1",
            Telefono: "3101234567",
            Direccion: "Calle 5 # 10-20, Puracé",
            NombreEmpresa: "Cultivos El Campestre"
        ),
        new(
            Id: 2,
            Nombre: "María Eugenia",
            Apellido: "Gómez",
            CedulaNit: "34567890",
            Telefono: "3129876543",
            Direccion: "Carrera 8 # 12-45, Popayán",
            NombreEmpresa: "AgroFresas del Cauca"
        )
    ];
}