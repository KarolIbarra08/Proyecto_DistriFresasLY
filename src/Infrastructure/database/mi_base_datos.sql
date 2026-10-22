-- =========================================================
-- 1. CREACIÓN DE TABLAS (SQLite)
-- =========================================================
--
-- IMPORTANTE: PRAGMA foreign_keys NO se guarda en el archivo.
-- Es por conexión. La API debe ejecutarlo en CADA conexión
-- (Microsoft.Data.Sqlite), o las FOREIGN KEY se ignoran.
PRAGMA foreign_keys = ON;

-- =========================================================
-- SEGURIDAD Y USUARIOS
-- =========================================================

-- Tabla: Roles
CREATE TABLE IF NOT EXISTS Roles (
    id_rol INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL UNIQUE,
    descripcion TEXT NOT NULL
);

-- Tabla: Permisos (catálogo de permisos del sistema)
-- Un permiso se guarda una sola vez y se reutiliza entre roles.
CREATE TABLE IF NOT EXISTS Permisos (
    id_permiso INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL UNIQUE
);

-- Tabla: RolPermisos (tabla puente: muchos-a-muchos Roles <-> Permisos)
-- Solo resuelve "qué rol tiene qué permiso".
CREATE TABLE IF NOT EXISTS RolPermisos (
    id_rol INTEGER NOT NULL,
    id_permiso INTEGER NOT NULL,
    PRIMARY KEY (id_rol, id_permiso),
    FOREIGN KEY (id_rol) REFERENCES Roles(id_rol) ON DELETE CASCADE,
    FOREIGN KEY (id_permiso) REFERENCES Permisos(id_permiso) ON DELETE CASCADE
);

-- Tabla: Usuarios
-- estado: 1 = activo, 0 = inactivo (el login rechaza inactivos).
CREATE TABLE IF NOT EXISTS Usuarios (
    id_usuario INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    apellido TEXT NOT NULL,
    usuario TEXT NOT NULL UNIQUE,
    contrasena TEXT NOT NULL,
    estado INTEGER NOT NULL DEFAULT 1 CHECK (estado IN (0, 1)),
    id_rol INTEGER NOT NULL,
    FOREIGN KEY (id_rol) REFERENCES Roles(id_rol) ON DELETE RESTRICT
);

-- =========================================================
-- CLIENTES Y PROVEEDORES
-- =========================================================

-- Tabla: Clientes
-- activo: 1 = activo, 0 = desactivado (baja lógica).
CREATE TABLE IF NOT EXISTS Clientes (
    id_cliente INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    cedula_nit TEXT NOT NULL UNIQUE,
    telefono TEXT NOT NULL,
    direccion TEXT NOT NULL,
    tipo_de_negocio TEXT NOT NULL,
    activo INTEGER NOT NULL DEFAULT 1 CHECK (activo IN (0, 1))
);

-- Tabla: Proveedores
-- activo: 1 = activo, 0 = desactivado (baja lógica).
CREATE TABLE IF NOT EXISTS Proveedores (
    id_proveedor INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    apellido TEXT NOT NULL,
    cedula_nit TEXT NOT NULL UNIQUE,
    telefono TEXT NOT NULL,
    direccion TEXT NOT NULL,
    nombre_empresa TEXT NOT NULL,
    activo INTEGER NOT NULL DEFAULT 1 CHECK (activo IN (0, 1))
);

-- =========================================================
-- PRODUCTOS (tabla madre + herencia por tipo)
-- =========================================================

-- Tabla: Producto (tabla "madre" de los productos)
CREATE TABLE IF NOT EXISTS Producto (
    id_producto INTEGER PRIMARY KEY AUTOINCREMENT,
    tipo_producto TEXT NOT NULL CHECK (tipo_producto IN ('Fresa', 'Insumo'))
);

-- Tabla: ProductoFresa (hereda el ID de Producto: PK = FK, relación 1:1)
CREATE TABLE IF NOT EXISTS ProductoFresa (
    id_producto INTEGER PRIMARY KEY,
    calibre INTEGER NOT NULL UNIQUE CHECK (calibre > 0),
    calidad TEXT NOT NULL,
    peso REAL NOT NULL CHECK (peso > 0),
    descripcion TEXT NOT NULL,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto) ON DELETE CASCADE
);

-- Tabla: ProductoInsumo (hereda el ID de Producto: PK = FK, relación 1:1)
CREATE TABLE IF NOT EXISTS ProductoInsumo (
    id_producto INTEGER PRIMARY KEY,
    nombre TEXT NOT NULL,
    tipo_insumo TEXT NOT NULL,
    unidad_medida TEXT NOT NULL,
    descripcion TEXT NOT NULL,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto) ON DELETE CASCADE
);

-- =========================================================
-- VENTAS
-- =========================================================

-- Tabla: Venta
-- id_cliente es OPCIONAL: el Domain permite vender sin cliente.
-- total se recalcula desde DetalleVenta al guardar.
CREATE TABLE IF NOT EXISTS Venta (
    id_venta INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha_venta TEXT NOT NULL,
    total REAL NOT NULL CHECK (total >= 0),
    estado TEXT NOT NULL DEFAULT 'Pendiente'
        CHECK (estado IN ('Pendiente', 'Completada', 'Cancelada')),
    id_cliente INTEGER,
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente) ON DELETE RESTRICT
);

-- Tabla: DetalleVenta
CREATE TABLE IF NOT EXISTS DetalleVenta (
    id_detalle_venta INTEGER PRIMARY KEY AUTOINCREMENT,
    cantidad INTEGER NOT NULL CHECK (cantidad > 0),
    precio_unitario REAL NOT NULL CHECK (precio_unitario > 0),
    subtotal REAL NOT NULL CHECK (subtotal >= 0),
    id_producto INTEGER NOT NULL,
    id_venta INTEGER NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES Venta(id_venta) ON DELETE CASCADE,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto) ON DELETE RESTRICT
);

-- Tabla: PagoCliente
CREATE TABLE IF NOT EXISTS PagoCliente (
    id_pago_cliente INTEGER PRIMARY KEY AUTOINCREMENT,
    valor_total REAL NOT NULL CHECK (valor_total > 0),
    medio_pago TEXT NOT NULL,
    fecha_pago TEXT NOT NULL,
    estado TEXT NOT NULL
        CHECK (estado IN ('Completado', 'Abono Parcial', 'Anulado')),
    id_cliente INTEGER NOT NULL,
    id_venta INTEGER NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente) ON DELETE RESTRICT,
    FOREIGN KEY (id_venta) REFERENCES Venta(id_venta) ON DELETE RESTRICT
);

-- Tabla: Recibo
CREATE TABLE IF NOT EXISTS Recibo (
    id_recibo INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha TEXT NOT NULL,
    numero_recibo INTEGER NOT NULL UNIQUE,
    total REAL NOT NULL CHECK (total > 0),
    id_venta INTEGER NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES Venta(id_venta) ON DELETE RESTRICT
);

-- =========================================================
-- PEDIDOS Y RECEPCIONES
-- =========================================================

-- Tabla: Pedidos
-- total se recalcula desde DetallePedido al guardar.
CREATE TABLE IF NOT EXISTS Pedidos (
    id_pedido INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha_pedido TEXT NOT NULL,
    estado TEXT NOT NULL DEFAULT 'Pendiente',
    total REAL NOT NULL CHECK (total >= 0),
    id_proveedor INTEGER NOT NULL,
    FOREIGN KEY (id_proveedor) REFERENCES Proveedores(id_proveedor) ON DELETE RESTRICT
);

-- Tabla: DetallePedido
-- id_producto es OBLIGATORIO: permite cruzar lo pedido con lo recibido
-- (DetalleRecepcion) y reflejarlo en Inventario.
CREATE TABLE IF NOT EXISTS DetallePedido (
    id_detalle_pedido INTEGER PRIMARY KEY AUTOINCREMENT,
    descripcion TEXT NOT NULL,
    cantidad INTEGER NOT NULL CHECK (cantidad > 0),
    precio_unitario REAL NOT NULL CHECK (precio_unitario > 0),
    id_pedido INTEGER NOT NULL,
    id_producto INTEGER NOT NULL,
    FOREIGN KEY (id_pedido) REFERENCES Pedidos(id_pedido) ON DELETE CASCADE,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto) ON DELETE RESTRICT
);

-- Tabla: Recepciones
-- id_pedido es OPCIONAL: el Domain permite recepciones sin pedido.
-- Si se elimina el pedido, la recepción se conserva con id_pedido = NULL.
CREATE TABLE IF NOT EXISTS Recepciones (
    id_recepcion INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha_recepcion TEXT NOT NULL,
    id_pedido INTEGER,
    procesada_en_inventario INTEGER NOT NULL DEFAULT 0 CHECK (procesada_en_inventario IN (0, 1)),
    FOREIGN KEY (id_pedido) REFERENCES Pedidos(id_pedido) ON DELETE SET NULL
);

-- Tabla: DetalleRecepcion
CREATE TABLE IF NOT EXISTS DetalleRecepcion (
    id_detalle_recepcion INTEGER PRIMARY KEY AUTOINCREMENT,
    cantidad INTEGER NOT NULL CHECK (cantidad > 0),
    precio_unitario REAL NOT NULL CHECK (precio_unitario >= 0),
    fecha_ingreso TEXT NOT NULL,
    id_recepcion INTEGER NOT NULL,
    id_producto INTEGER NOT NULL,
    FOREIGN KEY (id_recepcion) REFERENCES Recepciones(id_recepcion) ON DELETE CASCADE,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto) ON DELETE RESTRICT
);

-- Tabla: PagoProveedor
CREATE TABLE IF NOT EXISTS PagoProveedor (
    id_pago_proveedor INTEGER PRIMARY KEY AUTOINCREMENT,
    valor_total REAL NOT NULL CHECK (valor_total > 0),
    medio_pago TEXT NOT NULL,
    fecha_pago TEXT NOT NULL,
    estado TEXT NOT NULL
        CHECK (estado IN ('Completado', 'Abono Parcial', 'Anulado')),
    id_proveedor INTEGER NOT NULL,
    id_pedido INTEGER NOT NULL,
    FOREIGN KEY (id_proveedor) REFERENCES Proveedores(id_proveedor) ON DELETE RESTRICT,
    FOREIGN KEY (id_pedido) REFERENCES Pedidos(id_pedido) ON DELETE RESTRICT
);

-- =========================================================
-- INVENTARIO
-- =========================================================

-- Tabla: Inventario
CREATE TABLE IF NOT EXISTS Inventario (
    id_inventario INTEGER PRIMARY KEY AUTOINCREMENT,
    cantidad_disponible INTEGER NOT NULL CHECK (cantidad_disponible >= 0),
    fecha_actualizacion TEXT NOT NULL,
    id_producto INTEGER NOT NULL,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto) ON DELETE CASCADE
);

-- =========================================================
-- PRODUCTOS DAÑADOS
-- =========================================================

-- Tabla: ProductoDanado
-- tipo_producto se guarda como "foto" histórica del tipo al momento del daño
-- (se puede derivar con JOIN a Producto.tipo_producto).
CREATE TABLE IF NOT EXISTS ProductoDanado (
    id_producto_danado INTEGER PRIMARY KEY AUTOINCREMENT,
    tipo_producto TEXT NOT NULL CHECK (tipo_producto IN ('Fresa', 'Insumo')),
    cantidad INTEGER NOT NULL CHECK (cantidad > 0),
    identificacion TEXT NOT NULL,
    valor REAL NOT NULL CHECK (valor > 0),
    fecha_producto_danado TEXT NOT NULL,
    motivo TEXT NOT NULL,
    descuento_valor REAL CHECK (descuento_valor IS NULL OR descuento_valor >= 0),
    descuento_fecha TEXT,
    id_producto INTEGER NOT NULL,
    FOREIGN KEY (id_producto) REFERENCES Producto(id_producto) ON DELETE RESTRICT
);


-- =========================================================
-- 1.1 ÍNDICES
-- =========================================================

-- Un SOLO registro de inventario por producto. El código lo asume:
-- InventarioDataStore.ObtenerPorProductoId devuelve la primera coincidencia.
CREATE UNIQUE INDEX IF NOT EXISTS ux_inventario_producto
    ON Inventario (id_producto);

-- Índices en columnas FK (SQLite no los crea automáticamente)
CREATE INDEX IF NOT EXISTS ix_usuarios_rol               ON Usuarios (id_rol);
CREATE INDEX IF NOT EXISTS ix_venta_cliente              ON Venta (id_cliente);
CREATE INDEX IF NOT EXISTS ix_detalleventa_venta         ON DetalleVenta (id_venta);
CREATE INDEX IF NOT EXISTS ix_detalleventa_producto      ON DetalleVenta (id_producto);
CREATE INDEX IF NOT EXISTS ix_pagocliente_cliente        ON PagoCliente (id_cliente);
CREATE INDEX IF NOT EXISTS ix_pagocliente_venta          ON PagoCliente (id_venta);
CREATE INDEX IF NOT EXISTS ix_recibo_venta               ON Recibo (id_venta);
CREATE INDEX IF NOT EXISTS ix_pedidos_proveedor          ON Pedidos (id_proveedor);
CREATE INDEX IF NOT EXISTS ix_detallepedido_pedido       ON DetallePedido (id_pedido);
CREATE INDEX IF NOT EXISTS ix_detallepedido_producto     ON DetallePedido (id_producto);
CREATE INDEX IF NOT EXISTS ix_recepciones_pedido         ON Recepciones (id_pedido);
CREATE INDEX IF NOT EXISTS ix_detallerecepcion_recepcion ON DetalleRecepcion (id_recepcion);
CREATE INDEX IF NOT EXISTS ix_detallerecepcion_producto  ON DetalleRecepcion (id_producto);
CREATE INDEX IF NOT EXISTS ix_pagoproveedor_proveedor    ON PagoProveedor (id_proveedor);
CREATE INDEX IF NOT EXISTS ix_pagoproveedor_pedido       ON PagoProveedor (id_pedido);
CREATE INDEX IF NOT EXISTS ix_productodanado_producto    ON ProductoDanado (id_producto);
