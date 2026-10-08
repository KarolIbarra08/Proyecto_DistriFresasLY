using DistriFresasLY.Domain.Entities.Productos;

namespace DistriFresasLY.Api.Endpoints.Productos;

public static class ProductoDataStore
{
    public static readonly List<ProductoFresa> FresasDb = [];

    public static readonly List<ProductoInsumo> InsumosDb = [];
}