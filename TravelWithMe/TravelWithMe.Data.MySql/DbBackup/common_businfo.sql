CREATE DATABASE  IF NOT EXISTS `common` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `common`;
-- MySQL dump 10.13  Distrib 5.5.16, for Win32 (x86)
--
-- Host: localhost    Database: common
-- ------------------------------------------------------
-- Server version	5.5.27

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `businfo`
--

DROP TABLE IF EXISTS `businfo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `businfo` (
  `BusTripId` bigint(20) NOT NULL AUTO_INCREMENT,
  `FromLoc` int(11) DEFAULT NULL,
  `ToLoc` int(11) DEFAULT NULL,
  `DepartureTime` varchar(64) DEFAULT NULL,
  `ArrivalTime` varchar(64) DEFAULT NULL,
  `IsAc` bit(1) DEFAULT NULL,
  `BusName` varchar(64) DEFAULT NULL,
  `BusOperatorId` bigint(20) DEFAULT NULL,
  `BusType` varchar(16) DEFAULT NULL,
  `AddDate` datetime DEFAULT NULL,
  `SeatMapId` int(10) DEFAULT NULL,
  `IsEnabled` bit(1) DEFAULT b'0',
  `IsPublished` bit(1) DEFAULT b'0',
  PRIMARY KEY (`BusTripId`),
  KEY `fromloc_cityid_idx` (`FromLoc`),
  KEY `toloc_cityid_idx` (`ToLoc`),
  CONSTRAINT `fromloc_cityid` FOREIGN KEY (`FromLoc`) REFERENCES `city` (`CityId`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `toloc_cityid` FOREIGN KEY (`ToLoc`) REFERENCES `city` (`CityId`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-05-02 19:59:49
