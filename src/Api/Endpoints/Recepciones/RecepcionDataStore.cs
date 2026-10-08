using DistriFresasLY.Domain.Entities.Recepciones;

namespace DistriFresasLY.Api.Endpoints.Recepciones;

public static class RecepcionDataStore
{
    public static readonly List<Recepcion> RecepcionesDb = [];

    public static Recepcion? ObtenerPorId(int id)
    {
        return RecepcionesDb.FirstOrDefault(r => r.Id == id);
    }

    public static int GenerarNuevoId()
    {
        return RecepcionesDb.Count != 0
            ? RecepcionesDb.Max(r => r.Id) + 1
            : 1;
    }
}