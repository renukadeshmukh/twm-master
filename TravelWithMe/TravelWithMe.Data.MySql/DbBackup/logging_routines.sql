CREATE DATABASE  IF NOT EXISTS `logging` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `logging`;


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
-- Dumping events for database 'logging'
--

--
-- Dumping routines for database 'logging'
--
/*!50003 DROP PROCEDURE IF EXISTS `spGetExceptions` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=CURRENT_USER*/ /*!50003 PROCEDURE `spGetExceptions`(
inexceptionId int,
intimeStampFrom datetime,
intimeStampTo datetime,
inmachineName varchar(45),
insessionId varchar(45),
insource varchar(45),
intargetSite varchar(128),
inexceptionType varchar(128),
inappDomainName varchar(256),
inmessage varchar(512),
insearchText varchar(256),
inpageIndex int,
inpageSize int,
out outtotalRowCount int
)
BEGIN

	DECLARE StartIndex INT;
    DECLARE EndIndex INT;

	DECLARE innerStmt VARCHAR(1024);
	DECLARE orderByInnerStmt VARCHAR(1024);
	
	IF inpageindex is null or inpageindex = 0 THEN SET inpageindex = 1; 
	END IF;

	IF inpageSize is null or inpagesize = 0 then SET inpageSize = 10; 
	END IF;
	
        SET inPageIndex = inPageIndex - 1;
        SET StartIndex = inPageIndex * inPageSize;

        SET EndIndex = ( inPageIndex + 1 ) * inPageSize;

        
                
        SET innerStmt = 'SELECT @i := @i + 1 as ROW,`ExceptionId` FROM `exceptions`,(select @i := 0) temp WHERE 1=1 AND';
        
        SET orderByInnerStmt = ' ORDER BY `TimeStamp` DESC ';

        IF inexceptionId IS NOT NULL then
            SET innerStmt = Concat(innerStmt, '`ExceptionId` = ',CAST(inexceptionId AS CHAR(10)),' AND ');
		END IF;

        IF inTimeStampFrom IS NOT NULL
            AND inTimeStampTo IS NOT NULL then
            SET innerStmt = Concat(innerStmt, ' `Timestamp` BETWEEN ''',CAST(inTimeStampFrom AS char(20)), ''' AND ''',CAST(inTimeStampTo AS CHAR(20)),''' AND ');
         END IF;

        IF inMachineName IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `MachineName` LIKE ''%',inMachineName ,'%'' AND ');
		END IF;

        IF inSessionId IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `SessionId` = ''', CAST(inSessionId AS CHAR(36)) , ''' AND ');
		END IF;
        
		IF insource IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `Source` LIKE ''%' , insource , '%'' AND ') ;
		END IF;

        IF intargetSite IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `TargetSite` LIKE ''%' , intargetSite , '%'' AND ') ;
		END IF;

		IF inexceptionType IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `Type` LIKE ''%' , inexceptionType , '%'' AND ') ;
		END IF;

		IF inappDomainName IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `AppDomainName` LIKE ''%' , inappDomainName , '%'' AND ') ;
		END IF;

		IF inmessage IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `message` LIKE ''%' , inmessage , '%'' AND ') ;
		END IF;
                      
        IF inSearchText IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' (`Title` LIKE ''%' , inSearchText , '%'' OR `Message` LIKE ''%' , inSearchText
                , '%'' OR `InnerException` LIKE ''%' , inSearchText , '%'' ) AND ');
		END IF;
                        
        SET innerStmt = Concat(innerStmt, ' 1=1 ', orderByInnerStmt);

		
        SET @rowStmt = Concat('Create temporary table rowtable engine=memory SELECT * FROM   ( ' , innerStmt , ') AS DataRows WHERE  ROW > ' ,CAST(StartIndex AS CHAR(10)), ' AND ROW <= ' , CAST(EndIndex AS CHAR(10)));
            
        SET @totalCountStmt = Concat('INSERT  INTO t ( rowcnt ) SELECT count(*) FROM   ( ' , innerStmt , ') as A');

		drop temporary table if exists rowtable;

		drop temporary table if exists t;
		
		prepare stmt from @rowStmt;	
		execute stmt;        

        Create temporary table t ( rowcnt INT );
        
		prepare stmt from @totalCountStmt;	
		
		execute stmt;

        SELECT   rowcnt FROM t into outTotalRowCount;

        SELECT  E.`AdditionalInfo`,E.`AppDomainName`,E.`ExceptionId`,E.`InnerException`,E.`MachineName`,E.`Message`,E.`SessionId`,E.`Severity`,E.`Source`,E.`StackTrace`,E.`TargetSite`,E.`TimeStamp`,E.`Title`,E.`Type` 
        FROM    rowtable As R INNER JOIN exceptions AS E ON
        R.ExceptionId = E.ExceptionId;
        

		drop temporary table if exists rowtable;

		drop temporary table if exists t;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spGetLogs` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=CURRENT_USER*/ /*!50003 PROCEDURE `spGetLogs`(
inlogId int,
intimeStampFrom datetime,
intimeStampTo datetime,
inmachineName varchar(45),
insessionId varchar(45),
inserviceName varchar(256),
inname varchar(256),
intimeTaken float,
instatus varchar(45),
intimeMin float,
intimeMax float,
insearchText varchar(256),
inpageIndex int,
inpageSize int,
out outtotalRowCount int
)
BEGIN

	DECLARE StartIndex INT;
    DECLARE EndIndex INT;

	DECLARE innerStmt VARCHAR(1024);
	DECLARE orderByInnerStmt VARCHAR(1024);
	
	IF inpageindex is null or inpageindex = 0 THEN SET inpageindex = 1; 
	END IF;

	IF inpageSize is null or inpagesize = 0 then SET inpageSize = 10; 
	END IF;
	
        SET inPageIndex = inPageIndex - 1;
        SET StartIndex = inPageIndex * inPageSize;

        SET EndIndex = ( inPageIndex + 1 ) * inPageSize;

        
                
        SET innerStmt = 'SELECT @i := @i + 1 as ROW, LogId,MachineName,`Name`,ServiceId,ServiceName,SessionId,`Status`,`TimeStamp`,TimeTaken FROM `logs`,(select @i := 0) temp WHERE 1=1 AND';
        
        SET orderByInnerStmt = ' ORDER BY `TimeStamp` DESC ';

        IF inlogId IS NOT NULL then
            SET innerStmt = Concat(innerStmt, '`LogID` = ',CAST(inlogId AS CHAR(10)),' AND ');
		END IF;

        IF inTimeStampFrom IS NOT NULL
            AND inTimeStampTo IS NOT NULL then
            SET innerStmt = Concat(innerStmt, ' `Timestamp` BETWEEN ''',CAST(inTimeStampFrom AS char(20)), ''' AND ''',CAST(inTimeStampTo AS CHAR(20)),''' AND ');
         END IF;

        IF inMachineName IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `MachineName` LIKE ''%',inMachineName ,'%'' AND ');
		END IF;

        IF inSessionId IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `SessionId` = ''', CAST(inSessionId AS CHAR(36)) , ''' AND ');
		END IF;
        
		IF inServiceName IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `ServiceName` LIKE ''%' , inServiceName , '%'' AND ') ;
		END IF;

        
		IF inName IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `Name` LIKE ''%' , inName , '%'' AND ') ;
		END IF;
                      
        IF inTimeTaken IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `TimeTaken` >= ' , inTimeTaken , ' AND ');
		END IF;

        IF inStatus IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `Status` LIKE ''%' , inStatus , '%'' AND ');
		END IF;

        IF inTimeMin IS NOT NULL
            AND inTimeMax IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' `TimeTaken` BETWEEN ''' , CAST(inTimeMin AS CHAR(10)) , ''' AND '''
                , CAST(inTimeMax AS CHAR(10)) , ''' AND ');
		END IF;               
               
        IF inSearchText IS NOT NULL THEN
            SET innerStmt = Concat(innerStmt, ' (`Name` LIKE ''%' , inSearchText , '%'' OR `Request` LIKE ''%' , inSearchText
                , '%'' OR `Response` LIKE ''%' , inSearchText , '%'' ) AND ');
		END IF;
                        
        SET innerStmt = Concat(innerStmt, ' 1=1 ', orderByInnerStmt);

		
        SET @rowStmt = Concat('Create temporary table rowtable engine=memory SELECT * FROM   ( ' , innerStmt , ') AS DataRows WHERE  ROW > ' ,CAST(StartIndex AS CHAR(10)), ' AND ROW <= ' , CAST(EndIndex AS CHAR(10)));
            
        SET @totalCountStmt = Concat('INSERT  INTO t ( rowcnt ) SELECT count(*) FROM   ( ' , innerStmt , ') as A');

		drop temporary table if exists rowtable;

		drop temporary table if exists t;
		
		prepare stmt from @rowStmt;	
		execute stmt;        

        Create temporary table t ( rowcnt INT );
        
		prepare stmt from @totalCountStmt;	
		
		execute stmt;

        SELECT   rowcnt FROM t into outTotalRowCount;

        SELECT  LogId,
		MachineName,
		`Name`,
		ServiceId,
		ServiceName,
		SessionId,
		`Status`,
		`TimeStamp`,
		TimeTaken
        FROM    rowtable;

		drop temporary table if exists rowtable;

		drop temporary table if exists t;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spGetRequestByLogId` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=CURRENT_USER*/ /*!50003 PROCEDURE `spGetRequestByLogId`(
inlogId int
)
BEGIN

select Request from `logs` where logId = inLogid;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spGetResponseByLogId` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=CURRENT_USER*/ /*!50003 PROCEDURE `spGetResponseByLogId`(
inlogId int
)
BEGIN

select Response from `logs` where logId = inLogid;

END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSaveException` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=CURRENT_USER*/ /*!50003 PROCEDURE `spSaveException`(
inAdditionalInfo longtext,
inAppDomainName varchar(256),
inInnerException longtext,
inMachineName varchar(45),
inMessage varchar(512),
inSessionId varchar(45),
inSource varchar(45),
inStackTrace longtext,
inTargetSite varchar(128),
inTimeStamp datetime,
inTitle varchar(256),
inType varchar(128),
inSeverity varchar(45)
)
BEGIN
insert into `logging`.`exceptions`
(
`AdditionalInfo`,
`AppDomainName`,
`InnerException`,
`MachineName`,
`Message`,
`SessionId`,
`Source`,
`StackTrace`,
`TargetSite`,
`TimeStamp`,
`Title`,
`Type`,
`Severity`
)
values
(
inAdditionalInfo,
inAppDomainName,
inInnerException,
inMachineName,
inMessage,
inSessionId,
inSource,
inStackTrace,
inTargetSite,
inTimeStamp,
inTitle,
inType,
inSeverity
);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!50003 DROP PROCEDURE IF EXISTS `spSaveLog` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = '' */ ;
DELIMITER ;;
/*!50003 CREATE*/ /*!50020 DEFINER=CURRENT_USER*/ /*!50003 PROCEDURE `spSaveLog`(
inmachineName varchar(45),
inname varchar(256),
inrequest longtext,
inresponse longtext,
inserviceId int(11),
inserviceName varchar(256),
insessionId varchar(45),
instatus varchar(45),
intimeStamp datetime,
intimeTaken float
)
BEGIN
insert into `logging`.`logs`
(
`MachineName`,
`Name`,
`Request`,
`Response`,
`ServiceId`,
`ServiceName`,
`SessionId`,
`Status`,
`TimeStamp`,
`TimeTaken`
)
values
(
inmachineName,
inname,
inrequest,
inresponse,
inserviceId,
inserviceName,
insessionId,
instatus,
intimeStamp,
intimeTaken
);
END */;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2013-05-04 21:45:45
