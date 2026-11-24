# DistriFresasLY — API de Gestión Comercial e Inventario

API REST desarrollada en **ASP.NET Core (.NET)** con **SQLite** y **Dapper** para la gestión integral de ventas, pedidos a proveedores, recepciones de mercancía, control de inventario, productos dañados, clientes, usuarios y roles.

---

## Arquitectura del Proyecto (Clean Architecture + DDD)

La solución está dividida en cuatro capas con responsabilidades estrictamente separadas, donde las dependencias apuntan siempre hacia el núcleo (`Domain`):

```text
DISTRIFRESASLY/
└── src/
    ├── Domain/           # Entidades ricas, reglas de negocio, Result Pattern y utilidades de tiempo
    ├── Application/      # Casos de uso (UseCases), Commands, DTOs, errores e interfaces de repositorios
    ├── Infrastructure/   # Persistencia con SQLite + Dapper, Registros, Multi-Mapping y transacciones
    └── Api/              # Minimal APIs (Endpoints delgados), Contracts (Requests/Responses) y Scalar UI
```

### Descripción de cada capa

1. **`DistriFresasLY.Domain` (Núcleo del Negocio):**
   * Contiene las entidades ricas (`Producto`, `ProductoFresa`, `ProductoInsumo`, `Inventario`, `Recepcion`, `DetalleRecepcion`, `Venta`, `Pedido`, `ProductoDanado`, `Cliente`, `Proveedor`, `Usuario`, `Rol`).
   * Encapsula las invariantes, validaciones y transiciones de estado directamente en los métodos de cada entidad (por ejemplo: `Aumentar`, `Disminuir`, `Ajustar`, `MarcarComoProcesada`, `CalcularTotal`), devolviendo objetos `Result` / `Result<T>` sin lanzar excepciones para errores esperados de negocio.
   * No depende de ninguna otra capa ni de librerías de acceso a datos.

2. **`DistriFresasLY.Application` (Casos de Uso):**
   * Orquesta cada operación del sistema mediante clases individuales de casos de uso (`RegistrarRecepcionUseCase`, `RegistrarDetalleRecepcionUseCase`, `AjustarStockUseCase`, etc.).
   * Recibe comandos de entrada propios (`*Command`) y retorna DTOs de lectura (`*Dto`) envueltos en `Result`.
   * Valida reglas entre múltiples registros (por ejemplo, verificar existencia de recepción y producto antes de registrar un detalle e incrementar automáticamente el stock en inventario, o descontar mercancía dañada al consultar una recepción).
   * Define las abstracciones de persistencia (`IRecepcionRepository`, `IInventarioRepository`, `IVentaRepository`, etc.).

3. **`DistriFresasLY.Infrastructure` (Persistencia):**
   * Implementa los repositorios definidos en `Application` utilizando **Microsoft.Data.Sqlite** y **Dapper**.
   * Emplea consultas SQL parametrizadas (previniendo inyección SQL), `JOIN` y **Multi-Mapping** de Dapper para reconstruir agregados con sus detalles y relaciones.
   * Utiliza transacciones (`BeginTransaction`, `Commit`, `Rollback`) y tablas de control de secuencia para garantizar integridad referencial y atomicidad.

4. **`DistriFresasLY.Api` (Presentación):**
   * Expone los recursos mediante **Minimal APIs** organizadas en grupos (`IEndpoint`) con **endpoints delgados**: no acceden a repositorios directamente, sino que delegan toda la lógica a los casos de uso inyectados por dependencia.
   * Mapea los contratos HTTP (`*Request` y `*Response`) mediante métodos de extensión (`ToCommand()` y `ToResponse()`).
   * Incorpora manejo global de excepciones (`GlobalExceptionHandler`), conversión horaria a Colombia (`UTC-5`) y documentación interactiva con **Scalar**.

---

## Requisitos Previos

* [.NET SDK 9.0](https://dotnet.microsoft.com/download) (o superior)
* [Visual Studio Code](https://code.visualstudio.com/) con la extensión **SQLite** (autor: `alexcvzz`) o la herramienta CLI `sqlite3`.

---

## Creación y Configuración de la Base de Datos (SQLite)

### Opción A: Desde Visual Studio Code (Extensión SQLite)

1. Instala la extensión **SQLite** (`alexcvzz`) desde el panel de extensiones (`Ctrl + Shift + X`).
2. Verifica la ruta de la base de datos en `src/Api/appsettings.json` (por defecto `Data Source=distrifresas.db`, ubicada dentro de `src/Api/`).
3. Abre la paleta de comandos (`Ctrl + Shift + P`) y ejecuta **`SQLite: Open Database`**, seleccionando o creando el archivo `src/Api/distrifresas.db`.
4. Abre el archivo del esquema SQL del proyecto (`.sql`), selecciona todo el contenido (`Ctrl + A`) y presiona **`Ctrl + Shift + Q`** (o clic derecho → **`SQLite: Run Selected Query`**).
5. Verifica en el panel lateral **SQLite Explorer** que se hayan creado todas las tablas con sus claves foráneas e índices únicos (`Roles`, `Permisos`, `RolPermisos`, `Usuarios`, `Clientes`, `Proveedores`, `Producto`, `ProductoFresa`, `ProductoInsumo`, `Venta`, `DetalleVenta`, `PagoCliente`, `Recibo`, `Pedidos`, `DetallePedido`, `Recepciones`, `DetalleRecepcion`, `PagoProveedor`, `Inventario`, `ProductoDanado`).

### Opción B: Desde Terminal (CLI `sqlite3`)

Desde la raíz del repositorio, ejecuta:

```bash
sqlite3 src/Api/distrifresas.db < ruta/a/tu_esquema.sql
```

---

## Ejecución y Pruebas con Scalar

1. Navega al directorio de la API y ejecuta el proyecto:

```bash
cd src/Api
dotnet run
```

2. Una vez iniciada la aplicación (por defecto en `http://localhost:5221`), abre tu navegador en la interfaz interactiva de **Scalar**:

```text
http://localhost:5221/scalar/v1
```

3. Desde Scalar puedes probar todos los endpoints agrupados por módulo (`Clientes`, `Proveedores`, `Productos`, `Pedidos`, `Recepciones`, `DetalleRecepciones`, `Inventario`, `ProductosDanados`, `Ventas`, `Usuarios` y `Roles`).
