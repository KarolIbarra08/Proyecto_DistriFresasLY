-- =========================================================
-- 1. CREACIÓN DE TABLAS (Sintaxis SQLite)
-- =========================================================

-- Tabla: Roles
CREATE TABLE IF NOT EXISTS Roles (
    id_rol INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    descripcion TEXT
);

-- Tabla: Usuarios
CREATE TABLE IF NOT EXISTS Usuarios (
    id_usuario INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    apellido TEXT NOT NULL,
    usuario TEXT NOT NULL UNIQUE,
    contrasena TEXT NOT NULL,
    estado INTEGER NOT NULL DEFAULT 1,
    id_rol INTEGER NOT NULL,
    FOREIGN KEY (id_rol) REFERENCES Roles(id_rol)
);

-- Tabla: Clientes
CREATE TABLE IF NOT EXISTS Clientes (
    id_cliente INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    cedulaNIT TEXT NOT NULL UNIQUE,
    telefono TEXT,
    direccion TEXT,
    tipo_de_negocio TEXT
);

-- Tabla: Proveedores
CREATE TABLE IF NOT EXISTS Proveedores (
    id_proveedores INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    apellido TEXT NOT NULL,
    cedulaNIT TEXT NOT NULL UNIQUE,
    telefono TEXT,
    direccion TEXT,
    nombre_empresa TEXT
);

-- Tabla: ProductoFresa
CREATE TABLE IF NOT EXISTS ProductoFresa (
    id_producto_fresa INTEGER PRIMARY KEY AUTOINCREMENT,
    calidad TEXT NOT NULL,
    peso REAL,
    descripcion TEXT
);

-- Tabla: ProductoInsumo
CREATE TABLE IF NOT EXISTS ProductoInsumo (
    id_producto_insumo INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre TEXT NOT NULL,
    tipo_insumo TEXT,
    unidad_medida TEXT,
    descripcion TEXT
);

-- Tabla: Venta
CREATE TABLE IF NOT EXISTS Venta (
    id_venta INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha_venta TEXT NOT NULL,
    total REAL NOT NULL,
    estado TEXT,
    id_cliente INTEGER NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente)
);

-- Tabla: DetalleVenta
CREATE TABLE IF NOT EXISTS DetalleVenta (
    id_detalle_venta INTEGER PRIMARY KEY AUTOINCREMENT,
    cantidad INTEGER NOT NULL,
    precio_unitario REAL NOT NULL,
    subtotal REAL NOT NULL,
    total_venta REAL NOT NULL,
    id_producto INTEGER NOT NULL,
    id_venta INTEGER NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES Venta(id_venta)
);

-- Tabla: PagoCliente
CREATE TABLE IF NOT EXISTS PagoCliente (
    id_pago_cliente INTEGER PRIMARY KEY AUTOINCREMENT,
    valor_total REAL NOT NULL,
    medio_pago TEXT NOT NULL,
    fecha_pago TEXT NOT NULL,
    estado TEXT,
    id_cliente INTEGER NOT NULL,
    id_venta INTEGER NOT NULL,
    FOREIGN KEY (id_cliente) REFERENCES Clientes(id_cliente),
    FOREIGN KEY (id_venta) REFERENCES Venta(id_venta)
);

-- Tabla: Recibo
CREATE TABLE IF NOT EXISTS Recibo (
    id_recibo INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha TEXT NOT NULL,
    numero_recibo INTEGER NOT NULL,
    total REAL NOT NULL,
    id_venta INTEGER NOT NULL,
    FOREIGN KEY (id_venta) REFERENCES Venta(id_venta)
);

-- Tabla: Pedidos
CREATE TABLE IF NOT EXISTS Pedidos (
    id_pedido INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha_pedido TEXT NOT NULL,
    estado INTEGER NOT NULL DEFAULT 1,
    total REAL NOT NULL,
    id_proveedor INTEGER NOT NULL,
    FOREIGN KEY (id_proveedor) REFERENCES Proveedores(id_proveedores)
);

-- Tabla: Detalle_Pedido
CREATE TABLE IF NOT EXISTS Detalle_Pedido (
    id_detalle_pedido INTEGER PRIMARY KEY AUTOINCREMENT,
    descripcion TEXT,
    cantidad INTEGER NOT NULL,
    precio_unitario REAL NOT NULL,
    id_pedido INTEGER NOT NULL,
    FOREIGN KEY (id_pedido) REFERENCES Pedidos(id_pedido)
);

-- Tabla: Recepciones
CREATE TABLE IF NOT EXISTS Recepciones (
    id_recepcion INTEGER PRIMARY KEY AUTOINCREMENT,
    fecha_recepcion TEXT NOT NULL,
    id_pedido INTEGER NOT NULL,
    FOREIGN KEY (id_pedido) REFERENCES Pedidos(id_pedido)
);

-- Tabla: DetalleRecepcion
CREATE TABLE IF NOT EXISTS DetalleRecepcion (
    id_detalle_recepcion INTEGER PRIMARY KEY AUTOINCREMENT,
    cantidad INTEGER NOT NULL,
    precio_unitario REAL NOT NULL,
    fecha_ingreso TEXT NOT NULL,
    id_recepcion INTEGER NOT NULL,
    id_producto INTEGER NOT NULL,
    FOREIGN KEY (id_recepcion) REFERENCES Recepciones(id_recepcion)
);

-- Tabla: PagoProveedores
CREATE TABLE IF NOT EXISTS PagoProveedores (
    id_pago_proveedores INTEGER PRIMARY KEY AUTOINCREMENT,
    valor_total REAL NOT NULL,
    medio_pago TEXT NOT NULL,
    fecha_pago TEXT NOT NULL,
    estado TEXT,
    id_proveedor INTEGER NOT NULL,
    id_pedido INTEGER NOT NULL,
    FOREIGN KEY (id_proveedor) REFERENCES Proveedores(id_proveedores),
    FOREIGN KEY (id_pedido) REFERENCES Pedidos(id_pedido)
);

-- Tabla: Inventario
CREATE TABLE IF NOT EXISTS Inventario (
    id_inventario INTEGER PRIMARY KEY AUTOINCREMENT,
    cantidad_disponible INTEGER NOT NULL,
    fecha_actualizacion TEXT NOT NULL,
    id_producto INTEGER NOT NULL
);

-- Tabla: ProductoDañado
CREATE TABLE IF NOT EXISTS ProductoDañado (
    id_producto_dañado INTEGER PRIMARY KEY AUTOINCREMENT,
    tipo_producto TEXT NOT NULL,
    cantidad INTEGER NOT NULL,
    identificacion TEXT,
    valor REAL NOT NULL,
    fecha_producto_dañado TEXT NOT NULL,
    motivo TEXT,
    id_producto INTEGER NOT NULL
);


-- =========================================================
-- 2. INSERCIÓN DE DATOS DE PRUEBA (DML SQLite)
-- =========================================================

-- Roles
INSERT INTO Roles (nombre, descripcion) VALUES
('Administrador', 'Control total del sistema'),
('Vendedor', 'Encargado de la gestión de ventas y clientes'),
('Almacenista', 'Gestión de inventarios y recepciones');

-- Usuarios (1 = TRUE)
INSERT INTO Usuarios (nombre, apellido, usuario, contrasena, estado, id_rol) VALUES
('Juan', 'Pérez', 'jperez', 'pass1234', 1, 1),
('Maria', 'Gomez', 'mgomez', 'pass5678', 1, 2);

-- Clientes
INSERT INTO Clientes (nombre, cedulaNIT, telefono, direccion, tipo_de_negocio) VALUES
('Distribuidora Fresca', '900123456', '3001234567', 'Calle 10 #20-30', 'Supermercado'),
('Carlos Mendoza', '1012345678', '3109876543', 'Carrera 15 #5-12', 'Frutería');

-- Proveedores
INSERT INTO Proveedores (nombre, apellido, cedulaNIT, telefono, direccion, nombre_empresa) VALUES
('Pedro', 'Alvarez', '800987654', '3201112233', 'Vereda El Carmen', 'Cultivos Alvarez'),
('Ana', 'Rios', '700555444', '3159998877', 'Zona Industrial 2', 'Insumos del Campo S.A.');

-- ProductoFresa
INSERT INTO ProductoFresa (calidad, peso, descripcion) VALUES
('Primera / Extra', 1.0, 'Fresa de tamaño grande para exportación'),
('Segunda / Estándar', 0.5, 'Fresa mediana para mercado local');

-- ProductoInsumo
INSERT INTO ProductoInsumo (nombre, tipo_insumo, unidad_medida, descripcion) VALUES
('Caja de cartón 1Kg', 'Empaque', 'Unidad', 'Caja para embalaje de fresas'),
('Fertilizante Orgánico', 'Químico/Agrícola', 'Kilogramo', 'Abono para cultivo');

-- Venta
INSERT INTO Venta (fecha_venta, total, estado, id_cliente) VALUES
('2026-10-10', 150.5, 'Completada', 1),
('2026-10-12', 45.0, 'Pendiente', 2);

-- DetalleVenta
INSERT INTO DetalleVenta (cantidad, precio_unitario, subtotal, total_venta, id_producto, id_venta) VALUES
(10, 10.0, 100.0, 100.0, 1, 1),
(5, 10.1, 50.5, 50.5, 2, 1);

-- PagoCliente
INSERT INTO PagoCliente (valor_total, medio_pago, fecha_pago, estado, id_cliente, id_venta) VALUES
(150.5, 'Efectivo', '2026-10-10', 'Aprobado', 1, 1);

-- Recibo
INSERT INTO Recibo (fecha, numero_recibo, total, id_venta) VALUES
('2026-10-10', 1001, 150.5, 1);

-- Pedidos (1 = TRUE)
INSERT INTO Pedidos (fecha_pedido, estado, total, id_proveedor) VALUES
('2026-10-01', 1, 500.0, 1),
('2026-10-05', 1, 200.0, 2);

-- Detalle_Pedido
INSERT INTO Detalle_Pedido (descripcion, cantidad, precio_unitario, id_pedido) VALUES
('Compra de insumos de empaque', 50, 10.0, 1),
('Fertilizante', 10, 20.0, 2);

-- Recepciones
INSERT INTO Recepciones (fecha_recepcion, id_pedido) VALUES
('2026-10-03', 1),
('2026-10-07', 2);

-- DetalleRecepcion
INSERT INTO DetalleRecepcion (cantidad, precio_unitario, fecha_ingreso, id_recepcion, id_producto) VALUES
(50, 10.0, '2026-10-03', 1, 1),
(10, 20.0, '2026-10-07', 2, 2);

-- PagoProveedores
INSERT INTO PagoProveedores (valor_total, medio_pago, fecha_pago, estado, id_proveedor, id_pedido) VALUES
(500.0, 'Transferencia', '2026-10-04', 'Pagado', 1, 1);

-- Inventario
INSERT INTO Inventario (cantidad_disponible, fecha_actualizacion, id_producto) VALUES
(100, '2026-10-14', 1),
(250, '2026-10-14', 2);

-- ProductoDañado
INSERT INTO ProductoDañado (tipo_producto, cantidad, identificacion, valor, fecha_producto_dañado, motivo, id_producto) VALUES
('Fresa', 5, 'PROD-01', 12.5, '2026-10-11', 'Daño durante el transporte', 1);