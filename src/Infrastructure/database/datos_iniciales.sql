-- =========================================================
-- DATOS DE PRUEBA (DML SQLite)
-- Ejecutar DESPUES de mi_base_datos.sql
-- (en orden: primero las tablas padre, luego las hijas)
-- =========================================================

PRAGMA foreign_keys = ON;

-- Roles
INSERT INTO Roles (nombre, descripcion) VALUES
('Administrador', 'Control total del sistema'),
('Vendedor', 'Encargado de la gestión de ventas y clientes'),
('Almacenista', 'Gestión de inventarios y recepciones');

-- Permisos
INSERT INTO Permisos (nombre) VALUES
('ventas.crear'),
('ventas.eliminar'),
('usuarios.gestionar'),
('clientes.gestionar'),
('inventario.gestionar'),
('recepciones.gestionar');

-- RolPermisos (tabla puente)
-- Administrador = 1, Vendedor = 2, Almacenista = 3
INSERT INTO RolPermisos (id_rol, id_permiso) VALUES
(1, 1), (1, 2), (1, 3), (1, 4), (1, 5), (1, 6),  -- Administrador: todo
(2, 1), (2, 4),                                   -- Vendedor: ventas.crear, clientes.gestionar
(3, 5), (3, 6);                                   -- Almacenista: inventario, recepciones

-- Usuarios (1 = TRUE)
INSERT INTO Usuarios (nombre, apellido, usuario, contrasena, estado, id_rol) VALUES
('Juan', 'Pérez', 'jperez', 'pass1234', 1, 1),
('Maria', 'Gomez', 'mgomez', 'pass5678', 1, 2);

-- Clientes
INSERT INTO Clientes (nombre, cedula_nit, telefono, direccion, tipo_de_negocio) VALUES
('Distribuidora Fresca', '900123456', '3001234567', 'Calle 10 #20-30', 'Supermercado'),
('Carlos Mendoza', '1012345678', '3109876543', 'Carrera 15 #5-12', 'Frutería');

-- Proveedores
INSERT INTO Proveedores (nombre, apellido, cedula_nit, telefono, direccion, nombre_empresa) VALUES
('Pedro', 'Alvarez', '800987654', '3201112233', 'Vereda El Carmen', 'Cultivos Alvarez'),
('Ana', 'Rios', '700555444', '3159998877', 'Zona Industrial 2', 'Insumos del Campo S.A.');

-- Producto (tabla madre)
-- El ID lo asigna SQLite (AUTOINCREMENT): el TIPO NO se codifica en el
-- número, se guarda en la columna tipo_producto. Por orden de inserción
-- quedan: 1 = Fresa, 2 = Fresa, 3 = Insumo, 4 = Insumo.
INSERT INTO Producto (tipo_producto) VALUES
('Fresa'),
('Fresa'),
('Insumo'),
('Insumo');

-- ProductoFresa (productos 1 y 2)
-- calibre = número de bayas por kilo (a menor número, fresa más grande)
INSERT INTO ProductoFresa (id_producto, calibre, calidad, peso, descripcion) VALUES
(1, 12, 'Primera / Extra', 1.0, 'Fresa de tamaño grande para exportación'),
(2, 20, 'Segunda / Estándar', 0.5, 'Fresa mediana para mercado local');

-- ProductoInsumo (productos 3 y 4)
INSERT INTO ProductoInsumo (id_producto, nombre, tipo_insumo, unidad_medida, descripcion) VALUES
(3, 'Caja de cartón 1Kg', 'Empaque', 'Unidad', 'Caja para embalaje de fresas'),
(4, 'Fertilizante Orgánico', 'Químico/Agrícola', 'Kilogramo', 'Abono para cultivo');

-- Venta
INSERT INTO Venta (fecha_venta, total, estado, id_cliente) VALUES
('2026-10-10', 150.5, 'Completada', 1),
('2026-10-12', 45.0, 'Pendiente', 2);

-- DetalleVenta (id_producto 1 y 2 = fresas, ya sin ambigüedad)
-- Venta 2: 3 x 15.0 = 45.0, cuadra con el total de la venta 2
INSERT INTO DetalleVenta (cantidad, precio_unitario, subtotal, id_producto, id_venta) VALUES
(10, 10.0, 100.0, 1, 1),
(5, 10.1, 50.5, 2, 1),
(3, 15.0, 45.0, 2, 2);

-- PagoCliente (estado 'Completado': cubre los 150.5 de la venta 1;
-- el código usa 'Completado' | 'Abono Parcial' | 'Anulado')
INSERT INTO PagoCliente (valor_total, medio_pago, fecha_pago, estado, id_cliente, id_venta) VALUES
(150.5, 'Efectivo', '2026-10-10', 'Completado', 1, 1);

-- Recibo
INSERT INTO Recibo (fecha, numero_recibo, total, id_venta) VALUES
('2026-10-10', 1001, 150.5, 1);

-- Pedidos (estado = 'Pendiente' según el Domain)
INSERT INTO Pedidos (fecha_pedido, estado, total, id_proveedor) VALUES
('2026-10-01', 'Pendiente', 500.0, 1),
('2026-10-05', 'Pendiente', 200.0, 2);

-- DetallePedido (id_producto coherente con DetalleRecepcion:
-- pedido 1 -> producto 3 (cajas), pedido 2 -> producto 4 (fertilizante))
INSERT INTO DetallePedido (descripcion, cantidad, precio_unitario, id_pedido, id_producto) VALUES
('Compra de insumos de empaque', 50, 10.0, 1, 3),
('Fertilizante', 10, 20.0, 2, 4);

-- Recepciones (procesada_en_inventario = 1: su stock ya está reflejado en Inventario)
INSERT INTO Recepciones (fecha_recepcion, id_pedido, procesada_en_inventario) VALUES
('2026-10-03', 1, 1),
('2026-10-07', 2, 1);

-- DetalleRecepcion (ambos pedidos eran de INSUMOS => productos 3 y 4)
INSERT INTO DetalleRecepcion (cantidad, precio_unitario, fecha_ingreso, id_recepcion, id_producto) VALUES
(50, 10.0, '2026-10-03', 1, 3),
(10, 20.0, '2026-10-07', 2, 4);

-- PagoProveedor
INSERT INTO PagoProveedor (valor_total, medio_pago, fecha_pago, estado, id_proveedor, id_pedido) VALUES
(500.0, 'Transferencia', '2026-10-04', 'Completado', 1, 1);

-- Inventario (1 y 2 = fresas; 3 y 4 = insumos que entraron por recepción:
-- 50 cajas según DetalleRecepcion 1 y 10 fertilizantes según DetalleRecepcion 2)
INSERT INTO Inventario (cantidad_disponible, fecha_actualizacion, id_producto) VALUES
(100, '2026-10-14', 1),
(250, '2026-10-14', 2),
(50, '2026-10-14', 3),
(10, '2026-10-14', 4);

-- ProductoDanado (producto 1 = fresa, coherente con tipo_producto 'Fresa')
-- descuento = cantidad x valor = 5 x 12.5 = 62.5 (regla de Descuento.CalcularValor)
INSERT INTO ProductoDanado (tipo_producto, cantidad, identificacion, valor, fecha_producto_danado, motivo, descuento_valor, descuento_fecha, id_producto) VALUES
('Fresa', 5, 'PROD-01', 12.5, '2026-10-11', 'Daño durante el transporte', 62.5, '2026-10-11', 1);
