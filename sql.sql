-- MySQL dump 10.13  Distrib 5.7.34, for Linux (x86_64)
--
-- Host: localhost    Database: tupaopao
-- ------------------------------------------------------
-- Server version	5.7.34-log

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
-- Table structure for table `admin_roles`
--

DROP TABLE IF EXISTS `admin_roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_roles` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `roleKey` varchar(20) NOT NULL COMMENT '角色key',
  `name` varchar(20) NOT NULL COMMENT '角色名称',
  `description` text COMMENT '角色简介',
  `routes` json NOT NULL COMMENT '角色路由',
  `createTime` int(11) NOT NULL,
  `updateTime` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admin_roles`
--

LOCK TABLES `admin_roles` WRITE;
/*!40000 ALTER TABLE `admin_roles` DISABLE KEYS */;
INSERT INTO `admin_roles` VALUES (1,'admin','admin','admin','[{\"meta\": {\"icon\": \"lock\", \"title\": \"权限管理\"}, \"name\": \"permission\", \"path\": \"/permission\", \"children\": [{\"meta\": {\"icon\": \"user\", \"title\": \"角色管理\"}, \"name\": \"role\", \"path\": \"role\", \"component\": \"/permission/role\"}, {\"meta\": {\"icon\": \"peoples\", \"title\": \"帐户管理\"}, \"name\": \"account\", \"path\": \"account\", \"component\": \"/permission/account\"}], \"redirect\": \"/permission/account\", \"component\": \"Layout\"}]',1591967996,1630159653);
/*!40000 ALTER TABLE `admin_roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `admin_users`
--

DROP TABLE IF EXISTS `admin_users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `admin_users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userName` varchar(64) NOT NULL,
  `passwordHash` varbinary(128) NOT NULL,
  `passwordSalt` varbinary(128) NOT NULL,
  `roleId` int(11) NOT NULL,
  `roleKey` varchar(20) NOT NULL,
  `name` varchar(20) DEFAULT '' COMMENT '真实姓名',
  `avatar` varchar(200) DEFAULT '',
  `email` varchar(200) DEFAULT '',
  `introduction` text,
  `phone` varchar(20) DEFAULT '',
  `userType` varchar(20) NOT NULL,
  `status` tinyint(1) NOT NULL,
  `createTime` int(11) NOT NULL,
  `updateTime` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `admin_users`
--

LOCK TABLES `admin_users` WRITE;
/*!40000 ALTER TABLE `admin_users` DISABLE KEYS */;
INSERT INTO `admin_users` VALUES (1,'hxl',_binary '\�U�\'%��;\�\�\�y\n�խ	e�@��1�nS,�B��S�>�;H\�}m1{\�\�P�\�7�. 9�\0[#\�P�/',_binary '\�D ��\r�Ix�s��o�\���-kJx[�\�\�\�d�˙q*\��\�\�\�4����+�Yv\'ܾj:�J\�\�\�\n�\�Y`/�\�&��[%J�\�A\�q��bش.*\�)���$��&^\�G�\��y�(��\�\Z�,��%',1,'admin','贺小龙','http://api.tongshuzaixian.com/static/avatar.png','lovehl@vip.qq.com','管理员介绍','13721710606','Admin',1,1611398687,1611398687);
/*!40000 ALTER TABLE `admin_users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'tupaopao'
--

--
-- Dumping routines for database 'tupaopao'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-08-28 23:14:39
