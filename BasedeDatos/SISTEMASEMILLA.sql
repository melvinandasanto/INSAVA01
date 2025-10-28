-- Script de creaci n de la base de datos y todas las tablas ordenadas por dependencias
use master 
go
CREATE DATABASE SISTEMASEMILLA;
GO
USE SISTEMASEMILLA;
GO

-- 1. Cat logos b sicos
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

-- 3. Entidades principales
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


-- 5. Operaciones de venta y servicio
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
    CONSTRAINT FK_Maquila_Trans FOREIGN KEY (IDTransaccion)
        REFERENCES TRANSACCION(IDTransaccion),
    CONSTRAINT FK_Maquila_Prod FOREIGN KEY (IDProducto)
        REFERENCES PRODUCTO(IDProducto)
);
GO



--TIGRES
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
        DATEADD(DAY, 30, FechaInicio)  -- Calcula FechaEntrega autom ticamente
    FROM inserted;
END;
GO



--VISTAS PARA DATAGRIDS
CREATE VIEW VISTAFUSUARIO AS
SELECT 
    u.IDUsuario, 
    u.NumeroIdentidad, 
    (u.PrimerNombre + ' ' + ISNULL(u.SegundoNombre, '') + ' ' + u.PrimerApellido + ' ' + ISNULL(u.SegundoApellido, '')) AS NombreCompleto,
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
(c.PrimerNombre + ' ' + ISNULL(c.SegundoNombre, '') + ' ' + c.PrimerApellido + ' ' + ISNULL(c.SegundoApellido, '')) AS NombreCompleto,
c.NumTel as Telefono,
c.Activo
FROM CLIENTE c
GO

CREATE VIEW VISTAPRODUCTOS AS
SELECT
    IDProducto,
    Categoria,
    -- Concatenación condicional según la categoría
    CASE 
        WHEN Categoria IN ('Semilla', 'Semilla maquilada')
            THEN Nombre + ' (' + CAST(CAST(ISNULL(PorcentajeGerminacion * 100, 0) AS DECIMAL(5,2)) AS VARCHAR(6)) + '% Germinación)'
        ELSE Nombre
    END AS Producto,
    -- Solo muestra el porcentaje si es semilla o semilla maquilada, si no, NULL
    CASE 
        WHEN Categoria IN ('Semilla', 'Semilla maquilada') THEN CAST(ISNULL(PorcentajeGerminacion * 100, 0) AS DECIMAL(5,2))
        ELSE NULL
    END AS PorcentajeGerminacion,
    PrecioUnitario,
    PrecioMaquila
FROM PRODUCTO
WHERE Activo = 1;
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


--INSERCIONES FIJAS

INSERT INTO ROL (NombreRol) VALUES ('Administrador'), ('Empleado');
GO


INSERT INTO TIPO_MOVIMIENTO (NombreMovimiento) VALUES
('Retiro'),
('Ingreso');
GO

INSERT INTO TIPO_ORIGEN_SEMILLA (NombreOrigen) VALUES
('Stock'),
('Cliente');
GO

INSERT INTO METODO_PAGO (NombreMetodo)
VALUES
  ('Contado'),
  ('Cr dito');
GO

INSERT INTO TIPO_TRANSACCION (NombreTipo) VALUES
('Venta de producto'),
('Maquila');
GO


--PRUEBA INSERCIONES

Insert into USUARIO(NumeroIdentidad, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, Clave, IDRol) 
values ('0318200601618', 'Melvin', 'Adan', 'Santos', 'Claros', 'melvinandasanto', 1),
('0000000000000', 'USUARIO', 'de','prueba','programa','12345',1);
GO

-- CLIENTE
INSERT INTO CLIENTE (NumeroIdentidad, PrimerNombre, SegundoNombre, PrimerApellido, SegundoApellido, NumTel, Activo) VALUES
('0801199912345', 'Carlos', 'Eduardo', 'Martinez', 'Lopez', '98765432', 1),
('0801199812346', 'Ana', 'Maria', 'Gomez', 'Perez', '99887766', 1),
('0801199712347', 'Luis', 'Fernando', 'Castro', 'Ramirez', '91234567', 1),
('0801199612348', 'Sofia', 'Isabel', 'Hernandez', 'Mejia', '93456789', 1),
('0801199512349', 'Jorge', 'Alberto', 'Diaz', 'Santos', '94561234', 1),
('0801199412350', 'Paola', 'Andrea', 'Ruiz', 'Flores', '95678901', 1),
('0801199312351', 'Miguel', 'Angel', 'Vasquez', 'Torres', '96789012', 1),
('0801199212352', 'Gabriela', 'Lucia', 'Cruz', 'Mendoza', '97890123', 1),
('0801199112353', 'Ricardo', 'Jose', 'Ortega', 'Suazo', '98901234', 1),
('0801199012354', 'Valeria', 'Patricia', 'Morales', 'Aguilar', '99012345', 1);

-- PROVEEDOR
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

-- PRODUCTO (adaptado con PrecioMaquila obligatorio para Semilla y Semilla Maquilada)
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

