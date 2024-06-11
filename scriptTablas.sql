CREATE DATABASE  IF NOT EXISTS `bdinnovaschool` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `bdinnovaschool`;
-- MySQL dump 10.13  Distrib 8.0.34, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: bdinnovaschool
-- ------------------------------------------------------
-- Server version	8.0.35

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `alumno`
--

DROP TABLE IF EXISTS `alumno`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `alumno` (
  `codAlumno` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(20) NOT NULL,
  `correo` varchar(50) NOT NULL,
  `contraseña` varchar(45) NOT NULL,
  PRIMARY KEY (`codAlumno`),
  UNIQUE KEY `codAlumno_UNIQUE` (`codAlumno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `material`
--

DROP TABLE IF EXISTS `material`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `material` (
  `idMaterial` varchar(15) NOT NULL,
  `nombre` varchar(45) NOT NULL,
  `descripcion` varchar(45) NOT NULL,
  `stock` int NOT NULL,
  `tipo` varchar(25) NOT NULL,
  PRIMARY KEY (`idMaterial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `penalizacion`
--

DROP TABLE IF EXISTS `penalizacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `penalizacion` (
  `idPenalizacion` int NOT NULL AUTO_INCREMENT,
  `fechaPenalizacion` datetime DEFAULT NULL,
  `descripcion` varchar(255) NOT NULL,
  `estado` varchar(45) DEFAULT NULL,
  `Prestamo_idPrestamo` int NOT NULL,
  PRIMARY KEY (`idPenalizacion`),
  UNIQUE KEY `idPenalizacion_UNIQUE` (`idPenalizacion`),
  KEY `fk_Penalizacion_Prestamo1_idx` (`Prestamo_idPrestamo`),
  CONSTRAINT `fk_Penalizacion_Prestamo1` FOREIGN KEY (`Prestamo_idPrestamo`) REFERENCES `prestamo` (`idPrestamo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `prestamo`
--

DROP TABLE IF EXISTS `prestamo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `prestamo` (
  `idPrestamo` int NOT NULL AUTO_INCREMENT,
  `fechaPrestamo` datetime NOT NULL,
  `Solicitud_idSolicitud` int NOT NULL,
  `estado` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`idPrestamo`),
  UNIQUE KEY `idPrestamo_UNIQUE` (`idPrestamo`),
  KEY `fk_Prestamo_Solicitud1_idx` (`Solicitud_idSolicitud`),
  CONSTRAINT `fk_Prestamo_Solicitud1` FOREIGN KEY (`Solicitud_idSolicitud`) REFERENCES `solicitud` (`idSolicitud`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3 COMMENT='							';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `solicitud`
--

DROP TABLE IF EXISTS `solicitud`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `solicitud` (
  `idSolicitud` int NOT NULL AUTO_INCREMENT,
  `cantidad` int NOT NULL,
  `estado` varchar(15) DEFAULT NULL,
  `Alumno_codAlumno` int NOT NULL,
  `Material_idMaterial` varchar(15) NOT NULL,
  PRIMARY KEY (`idSolicitud`),
  UNIQUE KEY `idSolicitud_UNIQUE` (`idSolicitud`),
  KEY `fk_Solicitud_Alumno1_idx` (`Alumno_codAlumno`),
  KEY `fk_Solicitud_Material1_idx` (`Material_idMaterial`),
  CONSTRAINT `fk_Solicitud_Alumno1` FOREIGN KEY (`Alumno_codAlumno`) REFERENCES `alumno` (`codAlumno`),
  CONSTRAINT `fk_Solicitud_Material1` FOREIGN KEY (`Material_idMaterial`) REFERENCES `material` (`idMaterial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-06-10 23:43:58
