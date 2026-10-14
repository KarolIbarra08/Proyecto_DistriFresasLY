namespace DistriFresasLY.Api.Contracts.Recepciones;

public record RecepcionResponse(
    int Id,
    int IdProveedor,
    DateTime FechaRecepcion,
    double ValorPago,         
    bool ProcesadaEnInventario,
    double TotalCalculado
);