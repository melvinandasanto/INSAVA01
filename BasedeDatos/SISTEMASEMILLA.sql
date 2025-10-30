------------------------------------------------------------
-- SCRIPT COMPLETO FINAL: SISTEMASEMILLA CORREGIDO
-- Incluye usuario temporal que expira en 1 hora
------------------------------------------------------------

USE master;
GO
DROP DATABASE IF EXISTS SISTEMASEMILLA;
GO
CREATE DATABASE SISTEMASEMILLA;
GO
USE SISTEMASEMILLA;
GO

------------------------------------------------------------
-- 1. CATÁLOGOS BÁSICOS
------------------------------------------------------------
CREATE TABLE ROL (
    IDRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol VARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE METODO_PAGO (
    IDMetodoPago INT IDENTITY(1,1) PRIMARY KEY,
    NombreMetodo VARCHAR(50) NOT NULL
);
GO

CREATE TABLE TIPO_TRANSACCION (
    IDTipoTransaccion INT IDENTITY(1,1) PRIMARY KEY,
    NombreTipo VARCHAR(100) NOT NULL
);
GO

CREATE TABLE TIPO_MOVIMIENTO (
    IDTipoMovimiento INT IDENTITY(1,1) PRIMARY KEY,
    NombreMovimiento VARCHAR(50) NOT NULL
);
GO

CREATE TABLE TIPO_ORIGEN_SEMILLA (
    IDTipoOrigen INT IDENTITY(1,1) PRIMARY KEY,
    NombreOrigen VARCHAR(50) NOT NULL
);
GO

------------------------------------------------------------
-- 2. ENTIDADES PRINCIPALES
------------------------------------------------------------
CREATE TABLE CLIENTE (
    IDCliente INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    NumeroIdentidad VARCHAR(50) NOT NULL,
    PrimerNombre VARCHAR(50) NOT NULL,
    SegundoNombre VARCHAR(50),
    PrimerApellido VARCHAR(50) NOT NULL,
    SegundoApellido VARCHAR(50),
    NumTel VARCHAR(15) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE PROVEEDOR (
    IDProveedor INT IDENTITY(1,1) PRIMARY KEY,
    NombreProveedor VARCHAR(100) NOT NULL,
    TelefonoProveedor VARCHAR(50) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE USUARIO (
    IDUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NumeroIdentidad VARCHAR(15) NOT NULL UNIQUE,
    PrimerNombre VARCHAR(50) NOT NULL,
    SegundoNombre VARCHAR(50),
    PrimerApellido VARCHAR(50) NOT NULL,
    SegundoApellido VARCHAR(50),
    Clave VARCHAR(100) NOT NULL,
    IDRol INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Usuario_Rol FOREIGN KEY (IDRol)
        REFERENCES ROL(IDRol) ON DELETE CASCADE
);
GO

CREATE TABLE PRODUCTO (
    IDProducto INT IDENTITY(1,1) PRIMARY KEY,
    Categoria VARCHAR(50) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Cantidad DECIMAL(10,2) NOT NULL DEFAULT 0,
    PrecioUnitario DECIMAL(10,2) NOT NULL DEFAULT 0,
    PorcentajeGerminacion DECIMAL(4,2) CHECK (PorcentajeGerminacion BETWEEN 0 AND 1),
    PrecioMaquila DECIMAL(10,2) NULL,
    IDProveedor INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Producto_Proveedor FOREIGN KEY (IDProveedor)
        REFERENCES PROVEEDOR(IDProveedor),
    CONSTRAINT CK_Producto_PorcentajeGerminacion_Semilla
        CHECK (
            (Categoria IN ('Semilla', 'Semilla Maquilada') AND PorcentajeGerminacion IS NOT NULL)
            OR
            (Categoria NOT IN ('Semilla', 'Semilla Maquilada') AND PorcentajeGerminacion IS NULL)
        ),
    CONSTRAINT CK_Producto_PrecioMaquila_Semilla
        CHECK (
            (Categoria IN ('Semilla', 'Semilla Maquilada') AND PrecioMaquila IS NOT NULL)
            OR
            (Categoria NOT IN ('Semilla', 'Semilla Maquilada') AND PrecioMaquila IS NULL)
        )
);
GO

------------------------------------------------------------
-- 3. TABLAS DE OPERACIONES
------------------------------------------------------------
CREATE TABLE TRANSACCION (
    IDTransaccion INT IDENTITY(1,1) PRIMARY KEY,
    IDCliente INT NOT NULL,
    FechaEntrada DATETIME NOT NULL,
    FechaSalida DATETIME NULL,
    IDMetodoPago INT NULL,
    IDTipoTransaccion INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Trans_Cliente FOREIGN KEY (IDCliente) REFERENCES CLIENTE(IDCliente),
    CONSTRAINT FK_Trans_Metodo FOREIGN KEY (IDMetodoPago)
        REFERENCES METODO_PAGO(IDMetodoPago),
    CONSTRAINT FK_Trans_Tipo FOREIGN KEY (IDTipoTransaccion)
        REFERENCES TIPO_TRANSACCION(IDTipoTransaccion)
);
GO

CREATE TABLE MOVIMIENTO_PRODUCTO (
    IDMovimiento INT IDENTITY(1,1) PRIMARY KEY,
    IDProducto INT NOT NULL,
    IDTipoMovimiento INT NOT NULL,
    CantidadMovida DECIMAL(10,2) NOT NULL,
    FechaMovimiento DATETIME NOT NULL DEFAULT GETDATE(),
    Descripcion VARCHAR(MAX),
    IDTransaccion INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_MovProd_Prod FOREIGN KEY (IDProducto)
        REFERENCES PRODUCTO(IDProducto),
    CONSTRAINT FK_MovProd_TipoMov FOREIGN KEY (IDTipoMovimiento)
        REFERENCES TIPO_MOVIMIENTO(IDTipoMovimiento),
    CONSTRAINT FK_MovProd_Trans FOREIGN KEY (IDTransaccion)
        REFERENCES TRANSACCION(IDTransaccion)
);
GO

CREATE TABLE VENTA_PRODUCTO (
    IDVentaProducto INT IDENTITY(1,1) PRIMARY KEY,
    IDTransaccion INT NOT NULL,
    IDProducto INT NOT NULL,
    CantidadVendida DECIMAL(10,2) NOT NULL,
    TotalVenta DECIMAL(10,2) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_VentaProd_Trans FOREIGN KEY (IDTransaccion)
        REFERENCES TRANSACCION(IDTransaccion),
    CONSTRAINT FK_VentaProd_Prod FOREIGN KEY (IDProducto)
        REFERENCES PRODUCTO(IDProducto)
);
GO

CREATE TABLE MAQUILA_SEMILLA (
    IDMaquila INT IDENTITY(1,1) PRIMARY KEY,
    IDTransaccion INT NOT NULL,
    OrigenSemilla VARCHAR(20) NOT NULL DEFAULT 'Cliente' CHECK (OrigenSemilla IN ('Cliente', 'Inventario')),
    IDProducto INT NULL,
    CantidadMaquilada INT NOT NULL,
    PrecioPorUnidad DECIMAL(10,2) NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaEntrega DATE NOT NULL,
     Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Maquila_Trans FOREIGN KEY (IDTransaccion)
        REFERENCES TRANSACCION(IDTransaccion),
    CONSTRAINT FK_Maquila_Prod FOREIGN KEY (IDProducto)
        REFERENCES PRODUCTO(IDProducto)
);
GO

------------------------------------------------------------
-- 4. TRIGGER MAQUILA
------------------------------------------------------------
CREATE TRIGGER trg_InsertMaquilaSemilla
ON MAQUILA_SEMILLA
INSTEAD OF INSERT
AS
BEGIN
    INSERT INTO MAQUILA_SEMILLA (
        IDTransaccion,
        OrigenSemilla,
        IDProducto,
        CantidadMaquilada,
        PrecioPorUnidad,
        FechaInicio,
        FechaEntrega
    )
    SELECT
        IDTransaccion,
        OrigenSemilla,
        IDProducto,
        CantidadMaquilada,
        PrecioPorUnidad,
        FechaInicio,
        DATEADD(DAY, 30, FechaInicio)
    FROM inserted;
END;
GO

------------------------------------------------------------
-- 5. VISTAS
------------------------------------------------------------
CREATE VIEW VISTAFUSUARIO AS
SELECT 
    u.IDUsuario, 
    u.NumeroIdentidad, 
    (u.PrimerNombre + ' ' + ISNULL(u.SegundoNombre,'') + ' ' + u.PrimerApellido + ' ' + ISNULL(u.SegundoApellido,'')) AS NombreCompleto,
    u.Clave,
    r.NombreRol,
    u.Activo
FROM USUARIO u
INNER JOIN ROL r ON u.IDRol = r.IDRol;
GO

CREATE VIEW VISTAFCLIENTE AS
SELECT
    c.IDCliente,
    c.NumeroIdentidad,
    (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS NombreCompleto,
    c.NumTel AS Telefono,
    c.Activo
FROM CLIENTE c;
GO

CREATE VIEW VISTAPRODUCTOS AS
SELECT
    IDProducto,
    Categoria,
    CASE WHEN Categoria IN ('Semilla','Semilla Maquilada')
         THEN Nombre + ' (' + CAST(CAST(ISNULL(PorcentajeGerminacion*100,0) AS DECIMAL(5,2)) AS VARCHAR(6)) + '% Germinación)'
         ELSE Nombre END AS Producto,
    CASE WHEN Categoria IN ('Semilla','Semilla Maquilada') THEN CAST(ISNULL(PorcentajeGerminacion*100,0) AS DECIMAL(5,2)) ELSE NULL END AS PorcentajeGerminacion,
    PrecioUnitario,
    PrecioMaquila
FROM PRODUCTO
GO

CREATE VIEW VISTAFactura AS
SELECT
    T.IDTransaccion AS NumeroFactura,
    T.FechaEntrada AS FechaFactura,
    (C.PrimerNombre + ' ' + ISNULL(C.SegundoNombre, '') + ' ' + C.PrimerApellido + ' ' + ISNULL(C.SegundoApellido, '')) AS NombreCompletoCliente,
    TT.NombreTipo AS TipoTransaccion,
    MP.NombreMetodo AS MetodoPago,
    VP.IDVentaProducto AS IDDetalleVenta, -- Identificador único para cada línea de detalle de venta
    VP.IDProducto,
    VP_VIEW.Producto AS NombreProductoDetalle, -- Nombre del producto (incluye % de germinación para semillas)
    VP.CantidadVendida AS Cantidad,
    P.PrecioUnitario AS PrecioUnitarioVenta, -- Precio del producto en el momento de la venta
    (VP.CantidadVendida * P.PrecioUnitario) AS SubtotalLinea, -- Subtotal por esta línea de la factura
    VP.Activo AS DetalleVentaActivo
FROM
    TRANSACCION T
INNER JOIN CLIENTE C ON T.IDCliente = C.IDCliente
LEFT JOIN METODO_PAGO MP ON T.IDMetodoPago = MP.IDMetodoPago -- Se usa LEFT JOIN para incluir transacciones que aún no tienen un método de pago asignado.
INNER JOIN TIPO_TRANSACCION TT ON T.IDTipoTransaccion = TT.IDTipoTransaccion
INNER JOIN VENTA_PRODUCTO VP ON T.IDTransaccion = VP.IDTransaccion
INNER JOIN VISTAPRODUCTOS VP_VIEW ON VP.IDProducto = VP_VIEW.IDProducto
inner join PRODUCTO P ON P.IDProducto = VP_VIEW.IDProducto-- Se une con VISTAPRODUCTOS para obtener el nombre formateado del producto.
WHERE
    TT.NombreTipo = 'Venta de producto'; -- Se filtra para mostrar solo transacciones de venta.
GO

--VISTAS PARA REPORTERIA/BUSCADOR

CREATE VIEW VISTA_CLIENTE_SIMPLE AS
SELECT
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS NombreCompleto,
  c.NumeroIdentidad AS Identidad,
  c.NumTel AS Telefono
FROM CLIENTE c;
GO

CREATE VIEW VISTA_TRANSACCION_SIMPLE AS
SELECT
  'T-' + RIGHT('000000' + CAST(t.IDTransaccion AS VARCHAR(10)),6) AS NumeroTransaccion,
  CONVERT(VARCHAR(16), t.FechaEntrada, 120) AS FechaEntrada,
  CONVERT(VARCHAR(16), t.FechaSalida, 120) AS FechaSalida,
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS NombreCliente,
  tt.NombreTipo AS TipoTransaccion,
  mp.NombreMetodo AS MetodoPago,
  ISNULL(SUM(vp.TotalVenta),0) AS MontoTotal,
  COUNT(vp.IDVentaProducto) AS Lineas
FROM TRANSACCION t
LEFT JOIN CLIENTE c ON t.IDCliente = c.IDCliente
LEFT JOIN TIPO_TRANSACCION tt ON t.IDTipoTransaccion = tt.IDTipoTransaccion
LEFT JOIN METODO_PAGO mp ON t.IDMetodoPago = mp.IDMetodoPago
LEFT JOIN VENTA_PRODUCTO vp ON vp.IDTransaccion = t.IDTransaccion AND vp.Activo = 1
GROUP BY
  t.IDTransaccion, t.FechaEntrada, t.FechaSalida,
  c.PrimerNombre, c.SegundoNombre, c.PrimerApellido, c.SegundoApellido,
  tt.NombreTipo, mp.NombreMetodo, t.Activo;
GO

CREATE VIEW VISTA_VENTA_SIMPLE AS
SELECT
  p.Nombre AS NombreProducto,
  p.Categoria,
  vp.CantidadVendida,
  vp.TotalVenta
FROM VENTA_PRODUCTO vp
LEFT JOIN PRODUCTO p ON vp.IDProducto = p.IDProducto;
GO

CREATE VIEW VISTA_MOVIMIENTO_SIMPLE AS
SELECT
  CONVERT(VARCHAR(19), m.FechaMovimiento, 120) AS FechaMovimiento,
  p.Nombre AS NombreProducto,
  tm.NombreMovimiento AS TipoMovimiento,
  m.CantidadMovida,
  ISNULL(m.Descripcion,'') AS Descripcion,
  CASE WHEN m.IDTransaccion IS NOT NULL THEN 'T-' + RIGHT('000000' + CAST(m.IDTransaccion AS VARCHAR(10)),6) ELSE '' END AS TransaccionRef
FROM MOVIMIENTO_PRODUCTO m
LEFT JOIN PRODUCTO p ON m.IDProducto = p.IDProducto
LEFT JOIN TIPO_MOVIMIENTO tm ON m.IDTipoMovimiento = tm.IDTipoMovimiento;
GO

CREATE VIEW VISTA_INVENTARIO_SIMPLE AS
SELECT
  p.Nombre AS Producto,
  p.Categoria,
  p.Cantidad AS StockActual,
  p.PrecioUnitario
FROM PRODUCTO p
WHERE p.Activo = 1;
GO

CREATE VIEW VISTA_MAQUILA_ESTADO_DETALLE AS
SELECT
  'M-' + RIGHT('000000' + CAST(m.IDMaquila AS VARCHAR(10)),6) AS NumeroMaquila,
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS Cliente,
  CASE WHEN m.OrigenSemilla IS NULL OR LTRIM(RTRIM(m.OrigenSemilla)) = '' THEN 'Cliente' ELSE m.OrigenSemilla END AS OrigenSemilla,
  ISNULL(p.Nombre,'(Sin producto)') AS Producto,
  m.CantidadMaquilada,
  ROUND(m.PrecioPorUnidad,2) AS PrecioPorUnidad,
  ROUND(m.CantidadMaquilada * m.PrecioPorUnidad,2) AS MontoMaquila,
  CONVERT(VARCHAR(10), m.FechaInicio, 120) AS FechaInicio,
  CONVERT(VARCHAR(10), m.FechaEntrega, 120) AS FechaEntrega,
  DATEDIFF(DAY, CAST(GETDATE() AS DATE), m.FechaEntrega) AS DiasRestantes,
  CASE
    WHEN m.Activo = 0 THEN 'Desactivada manualmente'
    WHEN CAST(GETDATE() AS DATE) < m.FechaInicio THEN 'Programada'
    WHEN CAST(GETDATE() AS DATE) BETWEEN m.FechaInicio AND m.FechaEntrega THEN 'Activa'
    WHEN CAST(GETDATE() AS DATE) > m.FechaEntrega THEN 'Vencida'
    ELSE 'Desconocido'
  END AS EstadoMaquila,
  CASE WHEN t.IDTransaccion IS NOT NULL THEN 'T-' + RIGHT('000000' + CAST(t.IDTransaccion AS VARCHAR(10)) ,6) ELSE NULL END AS TransaccionAsociada
FROM MAQUILA_SEMILLA m
LEFT JOIN TRANSACCION t ON m.IDTransaccion = t.IDTransaccion
LEFT JOIN CLIENTE c ON t.IDCliente = c.IDCliente
LEFT JOIN PRODUCTO p ON m.IDProducto = p.IDProducto;
GO


-- VISTA_BUSCADOR_CLIENTES_RESUMEN
CREATE VIEW VISTA_BUSCADOR_CLIENTES_RESUMEN AS
SELECT
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS NombreCompleto,
  c.NumeroIdentidad AS Identidad,
  c.NumTel AS Telefono,
  ISNULL(SUM(vp.TotalVenta),0) AS TotalCompras,
  MAX(t.FechaEntrada) AS UltimaCompraFecha
FROM CLIENTE c
LEFT JOIN TRANSACCION t ON t.IDCliente = c.IDCliente
LEFT JOIN VENTA_PRODUCTO vp ON vp.IDTransaccion = t.IDTransaccion AND vp.Activo = 1
GROUP BY
  c.PrimerNombre, c.SegundoNombre, c.PrimerApellido, c.SegundoApellido,
  c.NumeroIdentidad, c.NumTel, c.Activo;
GO

-- VISTA_BUSCADOR_FACTURAS_RESUMEN
CREATE VIEW VISTA_BUSCADOR_FACTURAS_RESUMEN AS
SELECT
  'F-' + RIGHT('000000' + CAST(t.IDTransaccion AS VARCHAR(10)),6) AS NumeroFactura,
  t.FechaEntrada,
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS Cliente,
  mp.NombreMetodo AS MetodoPago,
  tt.NombreTipo AS TipoTransaccion,
  ISNULL(SUM(vp.TotalVenta),0) AS MontoTotal,
  COUNT(vp.IDVentaProducto) AS Lineas
FROM TRANSACCION t
LEFT JOIN CLIENTE c ON t.IDCliente = c.IDCliente
LEFT JOIN METODO_PAGO mp ON t.IDMetodoPago = mp.IDMetodoPago
LEFT JOIN TIPO_TRANSACCION tt ON t.IDTipoTransaccion = tt.IDTipoTransaccion
LEFT JOIN VENTA_PRODUCTO vp ON vp.IDTransaccion = t.IDTransaccion AND vp.Activo = 1
GROUP BY
  t.IDTransaccion, t.FechaEntrada,
  c.PrimerNombre, c.SegundoNombre, c.PrimerApellido, c.SegundoApellido,
  mp.NombreMetodo, tt.NombreTipo, t.Activo;
GO

-- VISTA_TRANSACCIONES_DETALLE_EXPORTABLE
CREATE VIEW VISTA_TRANSACCIONES_DETALLE_EXPORTABLE AS
SELECT
  'T-' + RIGHT('000000' + CAST(t.IDTransaccion AS VARCHAR(10)),6) AS NumeroTransaccion,
  CONVERT(VARCHAR(16), t.FechaEntrada, 120) AS Fecha,
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS Cliente,
  c.NumTel AS Telefono,
  tt.NombreTipo AS TipoTransaccion,
  mp.NombreMetodo AS MetodoPago,
  ISNULL(SUM(vp.TotalVenta),0) AS MontoTotal,
  STUFF((
    SELECT '; ' + p2.Nombre + ' x ' + CAST(vp2.CantidadVendida AS VARCHAR(20))
    FROM VENTA_PRODUCTO vp2
    LEFT JOIN PRODUCTO p2 ON vp2.IDProducto = p2.IDProducto
    WHERE vp2.IDTransaccion = t.IDTransaccion AND vp2.Activo = 1
    FOR XML PATH(''), TYPE).value('.','NVARCHAR(MAX)'),1,2,'') AS ProductosResumen,
  CASE WHEN EXISTS(SELECT 1 FROM MAQUILA_SEMILLA m WHERE m.IDTransaccion = t.IDTransaccion) THEN 'Sí' ELSE 'No' END AS TieneMaquila
FROM TRANSACCION t
LEFT JOIN CLIENTE c ON t.IDCliente = c.IDCliente
LEFT JOIN TIPO_TRANSACCION tt ON t.IDTipoTransaccion = tt.IDTipoTransaccion
LEFT JOIN METODO_PAGO mp ON t.IDMetodoPago = mp.IDMetodoPago
LEFT JOIN VENTA_PRODUCTO vp ON vp.IDTransaccion = t.IDTransaccion AND vp.Activo = 1
GROUP BY
  t.IDTransaccion, t.FechaEntrada,
  c.PrimerNombre, c.SegundoNombre, c.PrimerApellido, c.SegundoApellido,
  c.NumTel, tt.NombreTipo, mp.NombreMetodo, t.Activo;
GO

-- VISTA_VENTAS_POR_PRODUCTO_PERIODICA
CREATE VIEW VISTA_VENTAS_POR_PRODUCTO_PERIODICA AS
SELECT
  p.Nombre AS Producto,
  p.Categoria,
  SUM(vp.CantidadVendida) AS CantidadVendidaEnPeriodo,
  SUM(vp.TotalVenta) AS MontoTotalEnPeriodo,
  CASE WHEN SUM(vp.CantidadVendida) = 0 THEN 0
       ELSE ROUND(CAST(SUM(vp.CantidadVendida) AS DECIMAL(18,4)) / NULLIF(COUNT(DISTINCT vp.IDTransaccion),0),2) END AS UnidadesPorTransaccionPromedio,
  RANK() OVER (ORDER BY SUM(vp.CantidadVendida) DESC) AS Ranking
FROM VENTA_PRODUCTO vp
INNER JOIN PRODUCTO p ON vp.IDProducto = p.IDProducto
WHERE vp.Activo = 1
GROUP BY p.Nombre, p.Categoria;
GO

-- VISTA_MOVIMIENTOS_POR_PRODUCTO_AUDITORIA
CREATE VIEW VISTA_MOVIMIENTOS_POR_PRODUCTO_AUDITORIA AS
SELECT
  CONVERT(VARCHAR(19), m.FechaMovimiento, 120) AS FechaMovimiento,
  p.Nombre AS Producto,
  tm.NombreMovimiento AS TipoMovimiento,
  m.CantidadMovida,
  ISNULL(m.Descripcion,'') AS Descripcion,
  CASE WHEN m.IDTransaccion IS NOT NULL THEN 'T-' + RIGHT('000000' + CAST(m.IDTransaccion AS VARCHAR(10)),6) ELSE NULL END AS TransaccionRef,
  NULL AS Usuario
FROM MOVIMIENTO_PRODUCTO m
LEFT JOIN PRODUCTO p ON m.IDProducto = p.IDProducto
LEFT JOIN TIPO_MOVIMIENTO tm ON m.IDTipoMovimiento = tm.IDTipoMovimiento;
GO

-- VISTA_INVENTARIO_VALORIZADO
CREATE VIEW VISTA_INVENTARIO_VALORIZADO AS
SELECT
  p.Nombre AS Producto,
  p.Categoria,
  p.Cantidad AS StockActual,
  p.PrecioUnitario,
  ROUND(p.Cantidad * p.PrecioUnitario,2) AS ValorInventario
FROM PRODUCTO p
GO

-- VISTA_MAQUILAS_PENDIENTES_Y_HISTORICO
CREATE VIEW VISTA_MAQUILAS_PENDIENTES_Y_HISTORICO AS
SELECT
  'M-' + RIGHT('000000' + CAST(m.IDMaquila AS VARCHAR(10)),6) AS NumeroMaquila,
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS Cliente,
  m.OrigenSemilla,
  ISNULL(p.Nombre,'(Sin producto)') AS Producto,
  m.CantidadMaquilada,
  m.PrecioPorUnidad,
  ROUND(m.CantidadMaquilada * m.PrecioPorUnidad,2) AS MontoMaquila,
  m.FechaInicio,
  m.FechaEntrega,
  DATEDIFF(DAY, GETDATE(), m.FechaEntrega) AS DiasRestantes,
  CASE
    WHEN GETDATE() <= m.FechaEntrega THEN 'En proceso'
    WHEN GETDATE() > m.FechaEntrega THEN 'Entregada o retrasada'
    ELSE 'Desconocido'
  END AS Estado
FROM MAQUILA_SEMILLA m
LEFT JOIN TRANSACCION t ON m.IDTransaccion = t.IDTransaccion
LEFT JOIN CLIENTE c ON t.IDCliente = c.IDCliente
LEFT JOIN PRODUCTO p ON m.IDProducto = p.IDProducto;
GO

-- VISTA_PROVEEDORES_PRODUCTOS
CREATE VIEW VISTA_PROVEEDORES_PRODUCTOS AS
SELECT
  pr.NombreProveedor AS Proveedor,
  pr.TelefonoProveedor AS TelefonoProveedor,
  pr.Activo AS ProveedorActivo,
  ISNULL(
    STUFF((
      SELECT '; ' + p2.Nombre
      FROM PRODUCTO p2
      WHERE p2.IDProveedor = pr.IDProveedor
      FOR XML PATH(''), TYPE).value('.','VARCHAR(MAX)'),1,2,''), '') AS ProductosQueSuministra,
  ISNULL((
    SELECT SUM(p3.Cantidad) FROM PRODUCTO p3 WHERE p3.IDProveedor = pr.IDProveedor AND p3.Activo = 1
  ),0) AS StockTotalPorProveedor
FROM PROVEEDOR pr;
GO

-- VISTA_CLIENTES_TOP_Y_SEGMENTACION
CREATE VIEW VISTA_CLIENTES_TOP_Y_SEGMENTACION AS
SELECT
  (c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre,'') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido,'')) AS Cliente,
  ISNULL(SUM(vp.TotalVenta),0) AS TotalCompradoPeriodo,
  COUNT(DISTINCT t.IDTransaccion) AS FrecuenciaCompras,
  MAX(t.FechaEntrada) AS UltimaCompra
FROM CLIENTE c
LEFT JOIN TRANSACCION t ON t.IDCliente = c.IDCliente
LEFT JOIN VENTA_PRODUCTO vp ON vp.IDTransaccion = t.IDTransaccion AND vp.Activo = 1
GROUP BY c.PrimerNombre, c.SegundoNombre, c.PrimerApellido, c.SegundoApellido;
GO

------------------------------------------------------------
-- 6. INSERCIONES FIJAS
------------------------------------------------------------
INSERT INTO ROL (NombreRol) VALUES ('Administrador'), ('Empleado');
GO

INSERT INTO TIPO_MOVIMIENTO (NombreMovimiento) VALUES ('Retiro'), ('Ingreso');
GO

INSERT INTO TIPO_ORIGEN_SEMILLA (NombreOrigen) VALUES ('Stock'), ('Cliente');
GO

INSERT INTO METODO_PAGO (NombreMetodo) VALUES ('Contado'), ('Crédito');
GO

INSERT INTO TIPO_TRANSACCION (NombreTipo) VALUES ('Venta de producto'), ('Maquila');
GO

INSERT INTO USUARIO (NumeroIdentidad, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, Clave, IDRol)
VALUES 
('0318200601618', 'Melvin', 'Adan', 'Santos', 'Claros', 'melvinandasanto', 1),
('0000000000000', 'USUARIO', 'de','prueba','programa','12345',1);
GO

INSERT INTO CLIENTE (NumeroIdentidad, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, NumTel, Activo) VALUES
('0801199912345', 'Carlos', 'Eduardo', 'Martinez', 'Lopez', '98765432', 1),
('0801199812346', 'Ana', 'Maria', 'Gomez', 'Perez', '99887766', 1),
('0801199712347', 'Luis', 'Fernando', 'Castro', 'Ramirez', 91234567, 1),
('0801199612348', 'Sofia', 'Isabel', 'Hernandez', 'Mejia', 93456789, 1),
('0801199512349', 'Jorge', 'Alberto', 'Diaz', 'Santos', 94561234, 1),
('0801199412350', 'Paola', 'Andrea', 'Ruiz', 'Flores', 95678901, 1),
('0801199312351', 'Miguel', 'Angel', 'Vasquez', 'Torres', 96789012, 1),
('0801199212352', 'Gabriela', 'Lucia', 'Cruz', 'Mendoza', 97890123, 1),
('0801199112353', 'Ricardo', 'Jose', 'Ortega', 'Suazo', 98901234, 1),
('0801199012354', 'Valeria', 'Patricia', 'Morales', 'Aguilar', 99012345, 1);
GO

INSERT INTO PROVEEDOR (NombreProveedor, TelefonoProveedor, Activo) VALUES
('AgroSemillas S.A.', '22223333', 1),
('Semillas del Norte', '22334455', 1),
('PlantaVerde', '22445566', 1),
('BioCultivos', '22556677', 1),
('AgroHonduras', '22667788', 1),
('Semillas Selectas', '22778899', 1),
('CultivaFácil', '22889900', 1),
('AgroProveedores', '22990011', 1),
('Semillas Premium', '23001122', 1),
('VerdeVida', '23112233', 1);
GO

INSERT INTO PRODUCTO (Categoria, Nombre, Cantidad, PrecioUnitario, PorcentajeGerminacion, PrecioMaquila, IDProveedor, Activo) VALUES
('Semilla', 'Maíz Amarillo', 100, 25.00, 0.95, 5.00, 1, 1),
('Semilla', 'Frijol Rojo', 80, 30.00, 0.92, 6.00, 2, 1),
('Semilla', 'Arroz Integral', 120, 20.00, 0.90, 4.50, 3, 1),
('Semilla', 'Sorgo Blanco', 60, 18.00, 0.93, 4.00, 4, 1),
('Semilla', 'Cilantro', 200, 5.00, 0.85, 1.00, 5, 1),
('Semilla', 'Tomate Cherry', 150, 12.00, 0.88, 2.50, 6, 1),
('Semilla', 'Pepino', 90, 10.00, 0.87, 2.00, 7, 1),
('Semilla', 'Zanahoria', 110, 8.00, 0.89, 1.80, 8, 1),
('Semilla', 'Lechuga Romana', 130, 9.00, 0.91, 2.20, 9, 1),
('Semilla', 'Espinaca', 140, 7.00, 0.90, 1.50, 10, 1),
('Producto', 'Fertilizante Orgánico', 50, 15.00, NULL, NULL, 1, 1),
('Producto', 'Insecticida Natural', 30, 10.00, NULL, NULL, 2, 1),
('Producto', 'Fungicida Biológico', 20, 12.00, NULL, NULL, 3, 1),
('Producto', 'Herbicida Selectivo', 10, 18.00, NULL, NULL, 4, 1);
GO

------------------------------------------------------------
-- 7. USUARIO TEMPORAL CON FECHA REAL
------------------------------------------------------------
CREATE TABLE USUARIO_TEMPORAL_CONTROL (
    IDUsuario INT PRIMARY KEY,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaExpiracion DATETIME NOT NULL
);
GO

INSERT INTO USUARIO_TEMPORAL_CONTROL (IDUsuario, FechaExpiracion)
SELECT IDUsuario, DATEADD(HOUR,1,GETDATE())
FROM USUARIO
WHERE NumeroIdentidad='0000000000000';
GO

------------------------------------------------------------
-- 8. PROCEDIMIENTOS PARA DESACTIVAR Y AUTENTICAR
------------------------------------------------------------
CREATE OR ALTER PROCEDURE DesactivarUsuarioTemporal
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE U
    SET U.Activo=0
    FROM USUARIO U
    INNER JOIN USUARIO_TEMPORAL_CONTROL C ON U.IDUsuario=C.IDUsuario
    WHERE U.NumeroIdentidad='0000000000000' 
      AND U.Activo=1 
      AND GETDATE() > C.FechaExpiracion;
END;
GO

CREATE OR ALTER PROCEDURE AutenticarUsuario
    @NumeroIdentidad VARCHAR(15),
    @Clave VARCHAR(100)
AS
BEGIN
    EXEC DesactivarUsuarioTemporal;

    SELECT 
        U.IDUsuario,
        U.NumeroIdentidad,
        U.PrimerNombre,
        U.SegundoNombre,
        U.PrimerApellido,
        U.SegundoApellido,
        U.Clave,
        U.IDRol,
        U.Activo,
        C.FechaCreacion,
        C.FechaExpiracion
    FROM USUARIO U
    LEFT JOIN USUARIO_TEMPORAL_CONTROL C ON U.IDUsuario=C.IDUsuario
    WHERE U.NumeroIdentidad=@NumeroIdentidad AND U.Clave=@Clave;
END;
GO
ALTER TABLE USUARIO
ADD FechaExpiracion DATETIME NULL;

UPDATE USUARIO
SET FechaExpiracion = DATEADD(HOUR,1,GETDATE())
WHERE NumeroIdentidad = '0000000000000';
GO