PRAGMA foreign_keys = ON;

-- =========================================================
-- INSERCIÓN DE DATOS
-- =========================================================

INSERT OR IGNORE INTO Roles (nombre, descripcion) VALUES
('Administrador', 'Control total del sistema'),
('Vendedor', 'Encargado de la gestión de ventas y clientes'),
('Almacenista', 'Gestión de inventarios y recepciones');

INSERT OR IGNORE INTO Permisos (nombre) VALUES
('ventas.crear'),
('ventas.eliminar'),
('usuarios.gestionar'),
('clientes.gestionar'),
('inventario.gestionar'),
('recepciones.gestionar');

INSERT OR IGNORE INTO RolPermisos (id_rol, id_permiso) VALUES
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6),
(2, 1), (2, 4),
(3, 5), (3, 6);

INSERT OR IGNORE INTO Usuarios (nombre, apellido, usuario, contrasena, estado, id_rol) VALUES
('Juan', 'Pérez', 'jperez', 'pass1234', 1, 1),
('Maria', 'Gomez', 'mgomez', 'pass5678', 1, 2);

INSERT OR IGNORE INTO Clientes (nombre, cedula_nit, telefono, direccion, tipo_de_negocio) VALUES
('Distribuidora Fresca', '900123456', '3001234567', 'Calle 10 #20-30', 'Supermercado'),
('Carlos Mendoza', '1012345678', '3109876543', 'Carrera 15 #5-12', 'Frutería');

INSERT OR IGNORE INTO Proveedores (nombre, apellido, cedula_nit, telefono, direccion, nombre_empresa) VALUES
('Pedro', 'Alvarez', '800987654', '3201112233', 'Vereda El Carmen', 'Cultivos Alvarez'),
('Ana', 'Rios', '700555444', '3159998877', 'Zona Industrial 2', 'Insumos del Campo S.A.');

INSERT OR IGNORE INTO Producto (tipo_producto) VALUES
('Fresa'),
('Fresa'),
('Insumo'),
('Insumo');

INSERT OR IGNORE INTO ProductoFresa (id_producto, calibre, calidad, peso, descripcion) VALUES
(1, 12, 'Primera / Extra', 1.0, 'Fresa de tamaño grande para exportación'),
(2, 20, 'Segunda / Estándar', 0.5, 'Fresa mediana para mercado local');

INSERT OR IGNORE INTO ProductoInsumo (id_producto, nombre, tipo_insumo, unidad_medida, descripcion) VALUES
(3, 'Caja de cartón 1Kg', 'Empaque', 'Unidad', 'Caja para embalaje de fresas'),
(4, 'Fertilizante Orgánico', 'Químico/Agrícola', 'Kilogramo', 'Abono para cultivo');

INSERT OR IGNORE INTO Venta (fecha_venta, total, estado, id_cliente) VALUES
('2026-10-10', 150.5, 'Completada', 1),
('2026-10-12', 45.0, 'Pendiente', 2);

INSERT OR IGNORE INTO DetalleVenta (cantidad, precio_unitario, subtotal, id_producto, id_venta) VALUES
(10, 10.0, 100.0, 1, 1),
(5, 10.1, 50.5, 2, 1),
(3, 15.0, 45.0, 2, 2);

INSERT OR IGNORE INTO PagoCliente (valor_total, medio_pago, fecha_pago, estado, id_cliente, id_venta) VALUES
(150.5, 'Efectivo', '2026-10-10', 'Completado', 1, 1);

INSERT OR IGNORE INTO Recibo (fecha, numero_recibo, total, id_venta) VALUES
('2026-10-10', 1001, 150.5, 1);

INSERT OR IGNORE INTO Pedidos (fecha_pedido, estado, total, id_proveedor) VALUES
('2026-10-01', 'Pendiente', 500.0, 1),
('2026-10-05', 'Pendiente', 200.0, 2);

INSERT OR IGNORE INTO DetallePedido (descripcion, cantidad, precio_unitario, id_pedido, id_producto) VALUES
('Compra de insumos de empaque', 50, 10.0, 1, 3),
('Fertilizante', 10, 20.0, 2, 4);

INSERT OR IGNORE INTO Recepciones (fecha_recepcion, id_pedido, procesada_en_inventario) VALUES
('2026-10-03', 1, 1),
('2026-10-07', 2, 1);

INSERT OR IGNORE INTO DetalleRecepcion (cantidad, precio_unitario, fecha_ingreso, id_recepcion, id_producto) VALUES
(50, 10.0, '2026-10-03', 1, 3),
(10, 20.0, '2026-10-07', 2, 4);

INSERT OR IGNORE INTO PagoProveedor (valor_total, medio_pago, fecha_pago, estado, id_proveedor, id_pedido) VALUES
(500.0, 'Transferencia', '2026-10-04', 'Completado', 1, 1);

INSERT OR IGNORE INTO Inventario (cantidad_disponible, fecha_actualizacion, id_producto) VALUES
(100, '2026-10-14', 1),
(250, '2026-10-14', 2),
(50, '2026-10-14', 3),
(10, '2026-10-14', 4);

INSERT OR IGNORE INTO ProductoDanado (tipo_producto, cantidad, identificacion, valor, fecha_producto_danado, motivo, descuento_valor, descuento_fecha, id_producto) VALUES
('Fresa', 5, 'PROD-01', 12.5, '2026-10-11', 'Daño durante el transporte', 62.5, '2026-10-11', 1);