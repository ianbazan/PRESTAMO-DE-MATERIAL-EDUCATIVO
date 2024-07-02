CREATE DATABASE IF NOT EXISTS bdinnovaschool;
USE bdinnovaschool;

-- Crear tabla Usuario
DROP TABLE IF EXISTS `Usuario`;
CREATE TABLE `Usuario` (
  `CodUsuario` int NOT NULL AUTO_INCREMENT,
  `Contrasenia` varchar(20) NOT NULL,
  `Role` varchar(20) NOT NULL DEFAULT 'Alumno',
  PRIMARY KEY (`CodUsuario`),
  UNIQUE KEY `CodUsuario_UNIQUE` (`CodUsuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

SET SQL_SAFE_UPDATES = 1;

-- Actualizar los datos de usuario para asignar roles
UPDATE Usuario SET Role = 'Encargado' WHERE Contrasenia = 'UnicoEncargado';

-- Crear tabla Alumno
DROP TABLE IF EXISTS `Alumno`;
CREATE TABLE `Alumno` (
  `NombresApellidos` varchar(90) NOT NULL,
  `Estado` varchar(20) NOT NULL,
  `Usuario_CodUsuario` int NOT NULL,
  `DiasInhabilitado` INT DEFAULT 0,
  PRIMARY KEY (`Usuario_CodUsuario`),
  KEY `fk_Alumno_Usuario1_idx` (`Usuario_CodUsuario`),
  CONSTRAINT `fk_Alumno_Usuario1` FOREIGN KEY (`Usuario_CodUsuario`) REFERENCES `Usuario` (`CodUsuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Crear tabla Material
DROP TABLE IF EXISTS `Material`;
CREATE TABLE `Material` (
  `CodMaterial` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(45) NOT NULL,
  `Descripcion` varchar(45) NOT NULL,
  `Stock` int NOT NULL,
  `Tipo` varchar(25) NOT NULL,
  PRIMARY KEY (`CodMaterial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Crear tabla Solicitud
DROP TABLE IF EXISTS `Solicitud`;
CREATE TABLE `Solicitud` (
  `IdSolicitud` int NOT NULL AUTO_INCREMENT,
  `Cantidad` int NOT NULL,
  `Estado` varchar(15) NOT NULL,
  `Material_CodMaterial` int NOT NULL,
  `Alumno_Usuario_CodUsuario` int NOT NULL,
  PRIMARY KEY (`IdSolicitud`),
  UNIQUE KEY `IdSolicitud_UNIQUE` (`IdSolicitud`),
  KEY `fk_Solicitud_Material1_idx` (`Material_CodMaterial`),
  KEY `fk_Solicitud_Alumno1_idx` (`Alumno_Usuario_CodUsuario`),
  CONSTRAINT `fk_Solicitud_Alumno1` FOREIGN KEY (`Alumno_Usuario_CodUsuario`) REFERENCES `Alumno` (`Usuario_CodUsuario`),
  CONSTRAINT `fk_Solicitud_Material1` FOREIGN KEY (`Material_CodMaterial`) REFERENCES `Material` (`CodMaterial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Crear tabla Prestamo
DROP TABLE IF EXISTS `Prestamo`;
CREATE TABLE `Prestamo` (
  `IdPrestamo` int NOT NULL AUTO_INCREMENT,
  `FechaPrestamo` datetime NOT NULL,
  `Solicitud_idSolicitud` int NOT NULL,
  `Estado` varchar(15) NOT NULL,
  `Fecha_dev_real` datetime DEFAULT NULL,
  PRIMARY KEY (`IdPrestamo`),
  UNIQUE KEY `IdPrestamo_UNIQUE` (`IdPrestamo`),
  KEY `fk_Prestamo_Solicitud1_idx` (`Solicitud_idSolicitud`),
  CONSTRAINT `fk_Prestamo_Solicitud1` FOREIGN KEY (`Solicitud_idSolicitud`) REFERENCES `Solicitud` (`IdSolicitud`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Crear tabla Penalizacion
DROP TABLE IF EXISTS `Penalizacion`;
CREATE TABLE `Penalizacion` (
  `IdPenalizacion` int NOT NULL AUTO_INCREMENT,
  `FechaPenalizacion` datetime NOT NULL,
  `Descripcion` varchar(255) NOT NULL,
  `Prestamo_idPrestamo` int NOT NULL,
  PRIMARY KEY (`IdPenalizacion`),
  UNIQUE KEY `IdPenalizacion_UNIQUE` (`IdPenalizacion`),
  KEY `fk_Penalizacion_Prestamo1_idx` (`Prestamo_idPrestamo`),
  CONSTRAINT `fk_Penalizacion_Prestamo1` FOREIGN KEY (`Prestamo_idPrestamo`) REFERENCES `Prestamo` (`IdPrestamo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;

-- Procedimiento para registrar una solicitud
DELIMITER $$
CREATE PROCEDURE `registrarSolicitud` (
    IN p_alumno_codUsuario INT,
    IN p_material_cod INT,
    IN p_cantidad INT
)
BEGIN
    DECLARE v_stock INT;
    
    -- Verificar el stock disponible
    SELECT Stock INTO v_stock
    FROM Material
    WHERE CodMaterial = p_material_cod;
    
    IF v_stock >= p_cantidad THEN
        -- Insertar la nueva solicitud
        INSERT INTO Solicitud (Cantidad, Estado, Alumno_Usuario_CodUsuario, Material_CodMaterial)
        VALUES (p_cantidad, 'Generado', p_alumno_codUsuario, p_material_cod);
        
        -- Actualizar el stock del material
        UPDATE Material
        SET Stock = Stock - p_cantidad
        WHERE CodMaterial = p_material_cod;
    ELSE
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Stock insuficiente para la solicitud';
    END IF;
END$$
DELIMITER ;

-- Procedimiento para actualizar el estado de una solicitud
DELIMITER $$
CREATE PROCEDURE `actualizarEstadoSolicitud` (
    IN p_solicitud_id INT,
    IN p_nuevo_estado VARCHAR(15)
)
BEGIN
    UPDATE Solicitud
    SET Estado = p_nuevo_estado
    WHERE IdSolicitud = p_solicitud_id;
END$$
DELIMITER ;

-- Procedimiento para registrar un préstamo
DELIMITER $$
CREATE PROCEDURE `registrarPrestamo` (
    IN p_solicitud_id INT,
    IN p_fecha_prestamo DATETIME
)
BEGIN
    INSERT INTO Prestamo (FechaPrestamo, Solicitud_idSolicitud, Estado)
    VALUES (p_fecha_prestamo, p_solicitud_id, 'En Curso');
    
    -- Actualizar el estado de la solicitud a 'Aprobado'
    CALL actualizarEstadoSolicitud(p_solicitud_id, 'Aprobado');
END$$
DELIMITER ;

-- Procedimiento para registrar una devolución
DELIMITER $$
CREATE PROCEDURE `registrarDevolucion` (
    IN p_prestamo_id INT,
    IN p_fecha_devolucion DATETIME
)
BEGIN
    DECLARE v_solicitud_id INT;
    DECLARE v_material_cod INT;
    DECLARE v_cantidad INT;
    DECLARE v_alumno_codUsuario INT;
    DECLARE v_estado_prestamo VARCHAR(255);
    DECLARE v_fecha_devolucion_programada DATE;
    DECLARE v_dias_tardios INT;

    -- Recuperar estado y fecha de devolución programada del préstamo
    SELECT Estado, FechaPrestamo + INTERVAL 7 DAY INTO v_estado_prestamo, v_fecha_devolucion_programada
    FROM Prestamo
    WHERE IdPrestamo = p_prestamo_id;

    -- Actualizar el estado del préstamo a 'Devuelto'
    UPDATE Prestamo
    SET Estado = 'Devuelto',
        Fecha_dev_real = p_fecha_devolucion
    WHERE IdPrestamo = p_prestamo_id;
    
    -- Recuperar idSolicitud del préstamo
    SELECT Solicitud_idSolicitud INTO v_solicitud_id
    FROM Prestamo
    WHERE IdPrestamo = p_prestamo_id;
    
    -- Recuperar idMaterial y cantidad de la solicitud
    SELECT Material_CodMaterial, Cantidad INTO v_material_cod, v_cantidad
    FROM Solicitud
    WHERE IdSolicitud = v_solicitud_id;
    
    -- Devolver el stock del material
    UPDATE Material
    SET Stock = Stock + v_cantidad
    WHERE CodMaterial = v_material_cod;
    
    -- Actualizar el estado de la solicitud a 'Completado'
    CALL actualizarEstadoSolicitud(v_solicitud_id, 'Completado');

    -- Inhabilitar al alumno si el préstamo estaba en estado 'Tardio'
    IF v_estado_prestamo = 'Tardio' THEN
        SELECT Alumno_Usuario_CodUsuario INTO v_alumno_codUsuario
        FROM Solicitud
        WHERE IdSolicitud = v_solicitud_id;
        
        -- Calcular días tardíos
        SET v_dias_tardios = DATEDIFF(p_fecha_devolucion, v_fecha_devolucion_programada);
        
        -- Inhabilitar al alumno por la cantidad de días tardíos
        UPDATE Alumno
        SET Estado = 'Inhabilitado', DiasInhabilitado = DiasInhabilitado + v_dias_tardios
        WHERE Usuario_CodUsuario = v_alumno_codUsuario;
    END IF;
END$$
DELIMITER ;

-- Procedimiento para registrar una penalización
DELIMITER $$
CREATE PROCEDURE `registrarPenalizacion` (
    IN p_prestamo_id INT,
    IN p_fecha_penalizacion DATETIME,
    IN p_descripcion VARCHAR(255)
)
BEGIN
    DECLARE v_solicitud_id INT;
    DECLARE v_alumno_codUsuario INT;
    DECLARE v_fecha_prestamo DATETIME;
    DECLARE v_fecha_devolucion_programada DATE;
    DECLARE v_dias_tardios INT;
    DECLARE v_estado_prestamo VARCHAR(50);

    -- Verificar si el préstamo existe
    IF (SELECT COUNT(*) FROM Prestamo WHERE IdPrestamo = p_prestamo_id) = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'El préstamo no existe.';
    END IF;

    -- Obtener estado y fecha de devolución programada del préstamo
    SELECT Estado, FechaPrestamo + INTERVAL 7 DAY INTO v_estado_prestamo, v_fecha_devolucion_programada
    FROM Prestamo
    WHERE IdPrestamo = p_prestamo_id;

    -- Insertar la penalización
    INSERT INTO Penalizacion (FechaPenalizacion, Descripcion, Prestamo_idPrestamo)
    VALUES (p_fecha_penalizacion, p_descripcion, p_prestamo_id);

    -- Actualizar el estado del préstamo a 'Penalizado'
    UPDATE Prestamo
    SET Estado = 'Penalizado'
    WHERE IdPrestamo = p_prestamo_id;

    -- Si el préstamo está tardío, inhabilitar al alumno
    IF v_estado_prestamo = 'Tardio' THEN
        -- Obtener el código de usuario del alumno
        SELECT Alumno_Usuario_CodUsuario INTO v_alumno_codUsuario
        FROM Solicitud
        WHERE IdSolicitud = (SELECT Solicitud_idSolicitud FROM Prestamo WHERE IdPrestamo = p_prestamo_id);

        -- Calcular días tardíos
        SET v_dias_tardios = DATEDIFF(p_fecha_penalizacion, v_fecha_devolucion_programada);

        -- Inhabilitar al alumno por la cantidad de días tardíos
        UPDATE Alumno
        SET Estado = 'Inhabilitado', DiasInhabilitado = DiasInhabilitado + v_dias_tardios
        WHERE Usuario_CodUsuario = v_alumno_codUsuario;
    END IF;
END$$
DELIMITER ;
	
DELIMITER $$
CREATE PROCEDURE filtrarMateriales (
    IN p_categoria VARCHAR(50),
    IN p_buscar VARCHAR(100)
)
BEGIN
    SELECT CodMaterial, Nombre, Descripcion, Tipo, Stock
    FROM Material
    WHERE (p_categoria = 'TODOS' OR Tipo = p_categoria)
      AND (Nombre LIKE CONCAT('%', p_buscar, '%') OR Descripcion LIKE CONCAT('%', p_buscar, '%'));
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `actualizarPrestamo` (
    IN p_prestamo_id INT,
    IN p_nueva_fecha_prestamo DATETIME
)
BEGIN
    -- Actualizar la fecha de préstamo del préstamo existente
    UPDATE Prestamo
    SET FechaPrestamo = p_nueva_fecha_prestamo
    WHERE IdPrestamo = p_prestamo_id;
END$$
DELIMITER ;
