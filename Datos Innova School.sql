USE bdinnovaschool;

-- Insertar datos en la tabla Usuario
INSERT INTO Usuario (Contrasenia) VALUES ('Jhon Quiñones');
INSERT INTO Usuario (Contrasenia) VALUES ('Juan Trejo');
INSERT INTO Usuario (Contrasenia) VALUES ('Bug Bunny');
INSERT INTO Usuario (Contrasenia) VALUES ('Ian Bazan');
INSERT INTO Usuario (Contrasenia) VALUES ('Cristofer Jesus');

INSERT INTO Usuario (Contrasenia) VALUES ('UnicoEncargado');

ALTER TABLE Usuario ADD Role VARCHAR(20) NOT NULL DEFAULT 'Alumno';
SET SQL_SAFE_UPDATES = 1;
-- Actualizar los datos de usuario para asignar roles
UPDATE Usuario SET Role = 'Encargado' WHERE Contrasenia = 'UnicoEncargado';

-- Insertar datos en la tabla Alumno
INSERT INTO Alumno (NombresApellidos, Estado, Usuario_CodUsuario, DiasInhabilitado) VALUES ('Jhon Quiñones', 'Activo', 1, 0);
INSERT INTO Alumno (NombresApellidos, Estado, Usuario_CodUsuario, DiasInhabilitado) VALUES ('Juan Trejo', 'Activo', 2, 0);
INSERT INTO Alumno (NombresApellidos, Estado, Usuario_CodUsuario, DiasInhabilitado) VALUES ('Bug Bunny', 'Activo', 3, 0);
INSERT INTO Alumno (NombresApellidos, Estado, Usuario_CodUsuario, DiasInhabilitado) VALUES ('Ian Bazan', 'Activo', 4, 0);
INSERT INTO Alumno (NombresApellidos, Estado, Usuario_CodUsuario, DiasInhabilitado) VALUES ('Cristofer Jesus', 'Activo', 5, 0);

-- Insertar datos en la tabla Material
INSERT INTO Material (CodMaterial, Nombre, Descripcion, Stock, Tipo) VALUES ('100001', 'Libro de Matemáticas', 'Libro de Algebra', 10, 'Libro');
INSERT INTO Material (CodMaterial, Nombre, Descripcion, Stock, Tipo) VALUES ('100002', 'Libro de Ciencias', 'Libro de Biología', 8, 'Libro');
INSERT INTO Material (CodMaterial, Nombre, Descripcion, Stock, Tipo) VALUES ('100003', 'Tablet', 'Tablet para investigación', 5, 'Electrónico');
INSERT INTO Material (CodMaterial, Nombre, Descripcion, Stock, Tipo) VALUES ('100004', 'Proyector', 'Proyector para presentaciones', 3, 'Electrónico');
INSERT INTO Material (CodMaterial, Nombre, Descripcion, Stock, Tipo) VALUES ('100005', 'Calculadora', 'Calculadora científica', 15, 'Instrumento');

-- Insertar datos en la tabla Solicitud
INSERT INTO Solicitud (Cantidad, Estado, Material_CodMaterial, Alumno_Usuario_CodUsuario) VALUES (2, 'Generado', '100001', 1);
INSERT INTO Solicitud (Cantidad, Estado, Material_CodMaterial, Alumno_Usuario_CodUsuario) VALUES (1, 'Generado', '100002', 2);
INSERT INTO Solicitud (Cantidad, Estado, Material_CodMaterial, Alumno_Usuario_CodUsuario) VALUES (1, 'Generado', '100003', 3);
INSERT INTO Solicitud (Cantidad, Estado, Material_CodMaterial, Alumno_Usuario_CodUsuario) VALUES (1, 'Generado', '100004', 4);
INSERT INTO Solicitud (Cantidad, Estado, Material_CodMaterial, Alumno_Usuario_CodUsuario) VALUES (3, 'Generado', '100005', 5);

-- Insertar datos en la tabla Prestamo
INSERT INTO Prestamo (FechaPrestamo, Solicitud_idSolicitud, Estado) VALUES ('2024-06-01 10:00:00', 1, 'En Curso');
INSERT INTO Prestamo (FechaPrestamo, Solicitud_idSolicitud, Estado) VALUES ('2024-06-02 11:00:00', 2, 'En Curso');
INSERT INTO Prestamo (FechaPrestamo, Solicitud_idSolicitud, Estado) VALUES ('2024-06-03 12:00:00', 3, 'En Curso');
INSERT INTO Prestamo (FechaPrestamo, Solicitud_idSolicitud, Estado) VALUES ('2024-06-04 13:00:00', 4, 'En Curso');
INSERT INTO Prestamo (FechaPrestamo, Solicitud_idSolicitud, Estado) VALUES ('2024-06-05 14:00:00', 5, 'En Curso');

-- Insertar datos en la tabla Penalizacion
INSERT INTO Penalizacion (FechaPenalizacion, Descripcion, Prestamo_idPrestamo) VALUES ('2024-06-15 10:00:00', 'Devolución tardía', 1);
INSERT INTO Penalizacion (FechaPenalizacion, Descripcion, Prestamo_idPrestamo) VALUES ('2024-06-16 11:00:00', 'Material dañado',  2);
INSERT INTO Penalizacion (FechaPenalizacion, Descripcion, Prestamo_idPrestamo) VALUES ('2024-06-17 12:00:00', 'Pérdida de material', 3);
INSERT INTO Penalizacion (FechaPenalizacion, Descripcion, Prestamo_idPrestamo) VALUES ('2024-06-18 13:00:00', 'Devolución incompleta', 4);
INSERT INTO Penalizacion (FechaPenalizacion, Descripcion, Prestamo_idPrestamo) VALUES ('2024-06-19 14:00:00', 'Uso indebido del material', 5);
