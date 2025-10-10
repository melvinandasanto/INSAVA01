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

-- 2. Geograf a y direcci n
CREATE TABLE DEPARTAMENTO (
    IDDepartamento INT IDENTITY(1,1) PRIMARY KEY,
    NombreDepartamento VARCHAR(100) NOT NULL
);
GO

CREATE TABLE MUNICIPIO (
    IDMunicipio INT IDENTITY(1,1) PRIMARY KEY,
    NombreMunicipio VARCHAR(100) NOT NULL,
    IDDepartamento INT NOT NULL,
    CONSTRAINT FK_Municipio_Departamento FOREIGN KEY (IDDepartamento)
        REFERENCES DEPARTAMENTO(IDDepartamento) ON DELETE CASCADE
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

CREATE TABLE DIRECCION_CLIENTE (
    IDDireccion INT IDENTITY(1,1) PRIMARY KEY,
    IDCliente INT NOT NULL,
    Calle VARCHAR(200) NOT NULL,
    IDMunicipio INT NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Direccion_Cliente FOREIGN KEY (IDCLiente)
        REFERENCES CLIENTE(IDCliente) ON DELETE CASCADE,
    CONSTRAINT FK_Direccion_Municipio FOREIGN KEY (IDMunicipio)
        REFERENCES MUNICIPIO(IDMunicipio) ON DELETE CASCADE
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

-- 4. Inventario y productos
CREATE TABLE PRODUCTO (
    IDProducto INT IDENTITY(1,1) PRIMARY KEY,
    Categoria VARCHAR(50) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    Cantidad DECIMAL(10,2) NOT NULL DEFAULT 0,
    PrecioUnitario DECIMAL(10,2) NOT NULL DEFAULT 0,
    PorcentajeGerminacion DECIMAL(4,2) CHECK (PorcentajeGerminacion BETWEEN 0 AND 1),
    IDProveedor INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Producto_Proveedor FOREIGN KEY (IDProveedor)
        REFERENCES PROVEEDOR(IDProveedor),
    CONSTRAINT CK_Producto_PorcentajeGerminacion_Semilla
CHECK
    (Categoria IN ('Semilla', 'Semilla Maquilada') AND PorcentajeGerminacion IS NOT NULL)
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


--INSERCIONES FIJAS

INSERT INTO ROL (NombreRol) VALUES ('Administrador'), ('Empleado');
GO


INSERT INTO TIPO_MOVIMIENTO (NombreMovimiento) VALUES
('Retiro'),
('Ingreso');
GO

INSERT INTO DEPARTAMENTO (NombreDepartamento) VALUES
('Atl ntida'), ('Col n'), ('Comayagua'), ('Cop n'),
('Cort s'), ('Choluteca'), ('El Para so'), ('Francisco Morazan'), ('Gracias a Dios'), ('Intibuc '),
('Islas de la Bah a'), ('La Paz'), ('Lempira'), ('Ocotepeque'), ('Olancho'),
('Santa B rbara'), ('Valle'), ('Yoro');
GO

INSERT INTO MUNICIPIO (NombreMunicipio, IDDepartamento) VALUES
  /* Atl ntida (ID = 1) */
  ('La Ceiba', 1), ('El Porvenir', 1), ('Esparta', 1), ('Jutiapa', 1), ('La Masica', 1), ('San Francisco', 1), ('Tela', 1), ('Arizona', 1),

  /* Col n (ID = 2) */
  ('Trujillo', 2), ('Balfate', 2), ('Iriona', 2), ('Lim n', 2), ('Sab ', 2), ('Santa Fe', 2), ('Santa Rosa de Agu n', 2), ('Sonaguera', 2), ('Tocoa', 2), ('Bonito Oriental', 2),

  /* Comayagua (ID = 3) */
  ('Comayagua', 3), ('Ajuterique', 3), ('El Rosario', 3), ('Esqu as', 3), ('Humuya', 3), ('La Libertad', 3), ('Laman ', 3), ('La Trinidad', 3), ('Lejaman ', 3), ('Me mbar', 3), ('Minas de Oro', 3), ('Ojos de Agua', 3), ('San Jer nimo', 3), ('San Jos  de Comayagua', 3), ('San Jos  del Potrero', 3), ('San Luis', 3), ('San Sebasti n', 3), ('Siguatepeque', 3), ('Villa de San Antonio', 3), ('Las Lajas', 3), ('Taulab ', 3),

  /* Cop n (ID = 4) */
  ('Santa Rosa de Cop n', 4), ('Caba as', 4), ('Concepci n', 4), ('Cop n Ruinas', 4), ('Corqu n', 4), ('Cucuyagua', 4), ('Dolores', 4), ('Dulce Nombre', 4), ('El Para so', 4), ('Florida', 4), ('La Jigua', 4), ('La Uni n', 4), ('Nueva Arcadia', 4), ('San Agust n', 4), ('San Antonio', 4), ('San Jer nimo', 4), ('San Jos ', 4), ('San Juan de Opoa', 4), ('San Nicol s', 4), ('San Pedro', 4),  ('Santa Rita', 4), ('Trinidad de Cop n', 4), ('Veracruz', 4),

  /* Cort s (ID = 5) */
  ('San Pedro Sula', 5), ('Choloma', 5), ('Omoa', 5), ('Pimienta', 5), ('Potrerillos', 5), ('Puerto Cort s', 5), ('San Antonio de Cort s', 5), ('San Francisco de Yojoa', 5), ('San Manuel', 5), ('Santa Cruz de Yojoa', 5), ('Villanueva', 5), ('La Lima', 5),

  /* Choluteca (ID = 6) */
  ('Choluteca', 6), ('Apacilagua', 6), ('Concepci n de Mar a', 6), ('Duyure', 6), ('El Corpus', 6), ('El Triunfo', 6), ('Marcovia', 6), ('Morolica', 6), ('Namasig e', 6), ('Orocuina', 6), ('Pespire', 6), ('San Antonio de Flores', 6), ('San Isidro', 6), ('San Jos ', 6), ('San Marcos de Col n', 6), ('Santa Ana de Yusguare', 6),

  /* El Para so (ID = 7) */
  ('Yuscar n', 7), ('Alauca', 7), ('Danl ', 7), ('El Para so', 7), ('G inope', 7), ('Jacaleapa', 7), ('Liure', 7), ('Morocel ', 7), ('Oropol ', 7), ('Potrerillos', 7), ('San Antonio de Flores', 7), ('San Lucas', 7), ('San Mat as', 7), ('Soledad', 7), ('Teupasenti', 7), ('Texiguat', 7), ('Vado Ancho', 7), ('Yauyupe', 7), ('Trojes', 7),

  /* Francisco Moraz n (ID = 8) */
  ('Distrito Central', 8), ('Alubar n', 8), ('Cedros', 8), ('Curar n', 8), ('El Porvenir', 8), ('Guaimaca', 8), ('La Libertad', 8), ('La Venta', 8), ('Lepaterique', 8), ('Maraita', 8), ('Marale', 8), ('Nueva Armenia', 8), ('Ojojona', 8), ('Orica', 8), ('Reitoca', 8), ('Sabanagrande', 8), ('San Antonio de Oriente', 8), ('San Buenaventura', 8), ('San Ignacio', 8), ('San Juan de Flores', 8), ('San Miguelito', 8), ('Santa Ana', 8), ('Santa Luc a', 8), ('Talanga', 8), ('Tatumbla', 8), ('Valle de  ngeles', 8), ('Villa de San Francisco', 8), ('Vallecillo', 8),

  /* Gracias a Dios (ID = 9) */
  ('Puerto Lempira', 9), ('Brus Laguna', 9), ('Ahuas', 9), ('Juan Francisco Bulnes', 9), ('Ram n Villeda Morales', 9), ('Wampusirpe', 9),

  /* Intibuc  (ID = 10) */
  ('La Esperanza', 10), ('Camasca', 10), ('Colomoncagua', 10), ('Concepci n', 10), ('Dolores', 10), ('Intibuc ', 10), ('Jes s de Otoro', 10), ('Magdalena', 10), ('Masaguara', 10), ('San Antonio', 10), ('San Isidro', 10), ('San Juan', 10),  ('San Marcos de la Sierra', 10), ('San Miguel Guancapla', 10), ('Santa Luc a', 10), ('Yamaranguila', 10), ('San Francisco de Opalaca', 10),

  /* Islas de la Bah a (ID = 11) */
  ('Roat n', 11), ('Guanaja', 11), ('Jos  Santos Guardiola', 11), ('Utila', 11),

  /* La Paz (ID = 12) */
  ('La Paz', 12), ('Aguanqueterique', 12), ('Caba as', 12), ('Cane', 12), ('Chinacla', 12), ('Guajiquiro', 12), ('Lauterique', 12), ('Marcala', 12), ('Mercedes de Oriente', 12), ('Opatoro', 12), ('San Antonio del Norte', 12), ('San Jos ', 12), ('San Juan', 12), ('San Pedro de Tutule', 12), ('Santa Ana', 12), ('Santa Elena', 12), ('Santa Mar a', 12), ('Santiago de Puringla', 12), ('Yarula', 12),

  /* Lempira (ID = 13) */
  ('Gracias', 13),
  ('Bel n', 13),
  ('Candelaria', 13),
  ('Cololaca', 13),
  ('Erandique', 13),
  ('Gualcince', 13),
  ('Guarita', 13),
  ('La Campa', 13),
  ('La Iguala', 13),
  ('Las Flores', 13),
  ('La Uni n', 13),
  ('La Virtud', 13),
  ('Lepaera', 13),
  ('Mapulaca', 13),
  ('Piraera', 13),
  ('San Andr s', 13),
  ('San Francisco', 13),
  ('San Juan Guarita', 13),
  ('San Manuel Colohete', 13),
  ('San Rafael', 13),
  ('San Sebasti n', 13),
  ('Santa Cruz', 13),
  ('Talgua', 13),
  ('Tambla', 13),
  ('Tomal ', 13),
  ('Valladolid', 13),
  ('Virginia', 13),
  ('San Marcos de Caiqu n', 13),

  /* Ocotepeque (ID = 14) */
  ('Nueva Ocotepeque', 14),
  ('Bel n Gualcho', 14),
  ('Concepci n', 14),
  ('Dolores Merend n', 14),
  ('Fraternidad', 14),
  ('La Encarnaci n', 14),
  ('La Labor', 14),
  ('Lucerna', 14),
  ('Mercedes', 14),
  ('San Fernando', 14),
  ('San Francisco del Valle', 14),
  ('San Jorge', 14),
  ('San Marcos', 14),
  ('Santa Fe', 14),
  ('Sensenti', 14),
  ('Sinuapa', 14),

  /* Olancho (ID = 15) */
  ('Juticalpa', 15),
  ('Campamento', 15),
  ('Catacamas', 15),
  ('Concordia', 15),
  ('Dulce Nombre de Culm ', 15),
  ('El Rosario', 15),
  ('Esquipulas del Norte', 15),
  ('Gualaco', 15),
  ('Guarizama', 15),
  ('Guata', 15),
  ('Guayape', 15),
  ('Jano', 15),
  ('La Uni n', 15),
  ('Mangulile', 15),
  ('Manto', 15),
  ('Salam ', 15),
  ('San Esteban', 15),
  ('San Francisco de Becerra', 15),
  ('San Francisco de la Paz', 15),
  ('Santa Mar a del Real', 15),
  ('Silca', 15),
  ('Yoc n', 15),
  ('Patuca', 15),

  /* Santa B rbara (ID = 16) */
  ('Santa B rbara', 16),
  ('Arada', 16),
  ('Atima', 16),
  ('Azacualpa', 16),
  ('Ceguaca', 16),
  ('San Jos  de las Colinas', 16),
  ('Concepci n del Norte', 16),
  ('Concepci n del Sur', 16),
  ('Chinda', 16),
  ('El N spero', 16),
  ('Gualala', 16),
  ('Ilama', 16),
  ('Macuelizo', 16),
  ('Naranjito', 16),
  ('Nuevo Celilac', 16),
  ('Petoa', 16),
  ('Protecci n', 16),
  ('Quimist n', 16),
  ('San Francisco de Ojuera', 16),
  ('San Luis', 16),
  ('San Marcos', 16),
  ('San Nicol s', 16),
  ('San Pedro Zacapa', 16),
  ('Santa Rita', 16),
  ('San Vicente Centenario', 16),
  ('Trinidad', 16),
  ('Las Vegas', 16),
  ('Nueva Frontera', 16),

  /* Valle (ID = 17) */
  ('Nacaome', 17), ('Alianza', 17), ('Amapala', 17), ('Aramecina', 17), ('Caridad', 17), ('Goascor n', 17), ('Langue', 17), ('San Francisco de Coray', 17), ('San Lorenzo', 17),

  /* Yoro (ID = 18) */
  ('Yoro', 18), ('Arenal', 18), ('El Negrito', 18), ('El Progreso', 18), ('Joc n', 18), ('Moraz n', 18), ('Olanchito', 18), ('Santa Rita', 18), ('Sulaco', 18), ('Victoria', 18), ('Yorito', 18)
;
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
    PrecioUnitario
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



-- PRODUCTO (relacionados a su proveedor, usando IDProveedor del 1 al 10)
INSERT INTO PRODUCTO (Categoria, Nombre, Cantidad, PrecioUnitario, PorcentajeGerminacion, IDProveedor, Activo) VALUES
('Semilla', 'Maíz Amarillo', 100, 25.00, 0.95, 1, 1),
('Semilla', 'Frijol Rojo', 80, 30.00, 0.92, 2, 1),
('Semilla', 'Arroz Integral', 120, 20.00, 0.90, 3, 1),
('Semilla', 'Sorgo Blanco', 60, 18.00, 0.93, 4, 1),
('Semilla', 'Cilantro', 200, 5.00, 0.85, 5, 1),
('Semilla', 'Tomate Cherry', 150, 12.00, 0.88, 6, 1),
('Semilla', 'Pepino', 90, 10.00, 0.87, 7, 1),
('Semilla', 'Zanahoria', 110, 8.00, 0.89,  8, 1),
('Semilla', 'Lechuga Romana', 130, 9.00, 0.91,  9, 1),
('Semilla', 'Espinaca', 140, 7.00, 0.90,  10, 1),
('Producto', 'Fertilizante Orgánico', 50, 15.00, NULL,  1, 1),
('Producto', 'Insecticida Natural', 30, 10.00, NULL,  2, 1),
('Producto', 'Fungicida Biológico', 20, 12.00, NULL,  3, 1),
('Producto', 'Herbicida Selectivo', 10, 18.00, NULL, 4, 1);
GO