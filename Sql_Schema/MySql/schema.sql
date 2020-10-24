-- phpMyAdmin SQL Dump
-- version 4.4.15.10
-- https://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Oct 24, 2020 at 11:26 PM
-- Server version: 5.7.29
-- PHP Version: 7.3.23

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

--
-- Database: `bots_cove`
--

DELIMITER $$
--
-- Procedures
--
CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_GiveMsgExp`(IN `_userid` BIGINT, IN `_exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
INSERT INTO levels (user,exp)
VALUES(_userid,_exp)
ON DUPLICATE KEY
UPDATE levels.exp = _exp + levels.exp;
SELECT _userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_GiveVoiceExp`(IN `_userid` BIGINT, IN `_exp` DECIMAL(11,3))
    NO SQL
BEGIN
INSERT INTO levels (user,voice_exp)
VALUES(_userid,_exp)
ON DUPLICATE KEY
UPDATE levels.voice_exp = _exp + levels.voice_exp;
SELECT _userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_Reset`(IN `_userid` BIGINT)
    MODIFIES SQL DATA
BEGIN
DELETE from levels where levels.user = _userid;
SELECT count(*) from levels where levels.user = _userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_RevokeMsgExp`(IN `_userid` BIGINT, IN `_exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
SET @oldExp = (SELECT `levels`.`exp` from `levels` where `levels`.`user` = _userid);
SET @newExp = @oldExp - _exp;
IF @newExp < 0 THEN
 	SET @newExp = 0;
END IF;
UPDATE levels 
SET levels.exp = @newExp
WHERE levels.user = _userid;

SELECT _userid, @newExp, @oldExp;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_RevokeVoiceExp`(IN `_userid` BIGINT, IN `_exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
SET @oldExp = (SELECT `levels`.`voice_exp` from `levels` where `levels`.`user` = _userid);
SET @newExp = @oldExp - _exp;
IF @newExp < 0 THEN
 	SET @newExp = 0;
END IF;
UPDATE levels 
SET `levels`.`voice_exp` = @newExp
WHERE levels.user = _userid;

SELECT _userid, @newExp, @oldExp;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_GetUserStats`(IN `_szUserId` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT levelsExpToLevel.level, levels.exp, levelsExpToLevel.badgeImageUrl FROM levelsExpToLevel
JOIN levels
where levels.user = _szUserId and levelsExpToLevel.minExp <= levels.exp
ORDER BY levelsExpToLevel.level DESC
LIMIT 1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_GetUserVoiceStats`(IN `_szUserId` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT levelsExpToLevel.level, levels.voice_exp, levels.voice_last_join, levelsExpToLevel.badgeImageUrl FROM levelsExpToLevel
JOIN levels
where levels.user = _szUserId and levelsExpToLevel.minExp <= levels.voice_exp
ORDER BY levelsExpToLevel.level DESC
LIMIT 1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_SetVoiceState`(IN `_userid` BIGINT, IN `_timeJoined` DATETIME, IN `_inVoice` TINYINT)
    MODIFIES SQL DATA
BEGIN
INSERT INTO levels (user,voice_last_join,isInVoice)
VALUES(_userid,_timeJoined,_inVoice)
ON DUPLICATE KEY
UPDATE levels.voice_last_join = _timeJoined, levels.isInVoice = _inVoice;
SELECT _userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `settings_AddUpdate`(IN `_guildId` BIGINT, IN `_module` VARCHAR(255), IN `_setting` VARCHAR(255), IN `_iValue` INT, IN `_sValue` VARCHAR(255), IN `_biValue` BIGINT)
    MODIFIES SQL DATA
BEGIN

DELETE FROM `settings` 
where `settings`.`guildId` = _guildId
AND `settings`.`module`= _module
AND `settings`.`setting` = _setting;

INSERT INTO `settings` (`guildId`,`module`,`setting`,`sValue`,`iValue`,`biValue`)
VALUES(_guildId,_module,_setting,_iValue,_sValue,_biValue);

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `settings_getSetting`(IN `_guildId` BIGINT, IN `_module` VARCHAR(255), IN `_setting` VARCHAR(255))
    READS SQL DATA
BEGIN
	SELECT * FROM `settings`
    WHERE `settings`.`guildId` = _guildId
    AND `settings`.`module` = _module
    AND `settings`.`setting` = _setting;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_AddStream`(IN `_channel` VARCHAR(255), IN `_ping` TINYINT, IN `_discordId` BIGINT, IN `_guildId` BIGINT, IN `_type` VARCHAR(50))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
INSERT INTO `streams`(`name`, `lastMessage`, `approved`, `discordId`,`guildId`,`type`) 
VALUES (_channel,'2000-01-01 00:00',_ping,_discordId,_guildid,_type);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_DeleteStream`(IN `_userid` BIGINT, IN `_guildId` BIGINT, IN `_type` VARCHAR(50))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN 

	DELETE FROM `streams`
    WHERE 
    	`streams`.`discordId` = _userid AND
        `streams`.`guildId` = _guildid AND
        `streams`.`type` = _type;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_GetAllStreamsForGuild`(IN `_guildId` BIGINT, IN `_type` VARCHAR(50))
    READS SQL DATA
    DETERMINISTIC
BEGIN

    SELECT 
        `name` as 'Channel', 
        `discordId`, 
        CASE 
        	WHEN approved = 0 THEN 'No' 
            WHEN approved = 1 THEN 'Yes' 
        END as '@here_Ping' 
    FROM `streams`
    WHERE `streams`.`type` = _type and `streams`.`guildId` = _guildId;
    

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_getLastAnnounce`(IN `_guildId` BIGINT, IN `_type` VARCHAR(50))
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT `name` as 'Channel', `discordId` 
FROM `streams` 
WHERE `guildId` = _guildId AND `type` = _type
ORDER BY `lastMessage` desc LIMIT 1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_GetStreamForChannelInGuild`(IN `_channel` VARCHAR(100), IN `_guildId` BIGINT, IN `_type` VARCHAR(50), IN `_userid` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
	SELECT * FROM `streams`
    WHERE `streams`.`type` = _type
    AND `streams`.`guildId`= _guildid
    AND (`streams`.`name`= _channel 
         OR `streams`.`discordId` = _userid);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_GetStreams`(IN `_type` VARCHAR(50))
    READS SQL DATA
    DETERMINISTIC
BEGIN
	SELECT `streams`.*, `settings`.`biValue` as `channelId` 
    FROM `streams`, `settings`
    WHERE `settings`.`guildId` = `streams`.`guildId`
    AND `streams`.lastMessage < DATE_ADD(NOW(), INTERVAL -1 HOUR)
    AND `settings`.`module` = 'twitch'
    AND `settings`.`setting` = 'StreamChannel'
    AND `type` = _type;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_setPing`(IN `_userId` BIGINT, IN `_guildId` BIGINT, IN `_ping` TINYINT)
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
    UPDATE `streams` 
    SET `approved` = _ping 
    WHERE `discordId` = _userid
    and `guildId` = _guildId;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `stream_updateTimeStamp`(IN `_timestamp` DATETIME, IN `_id` INT)
    NO SQL
BEGIN
	UPDATE `streams` 
    SET `lastMessage` = _timestamp 
    WHERE `id` = _id;
END$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Table structure for table `levels`
--

CREATE TABLE IF NOT EXISTS `levels` (
  `user` bigint(20) NOT NULL,
  `exp` decimal(11,3) NOT NULL DEFAULT '0.000',
  `voice_exp` decimal(11,3) NOT NULL DEFAULT '0.000',
  `voice_last_join` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isInVoice` tinyint(4) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `levelsExpToLevel`
--

CREATE TABLE IF NOT EXISTS `levelsExpToLevel` (
  `level` int(11) NOT NULL,
  `minExp` decimal(11,3) NOT NULL,
  `badgeImageUrl` varchar(255) COLLATE latin1_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `settings`
--

CREATE TABLE IF NOT EXISTS `settings` (
  `id` int(11) NOT NULL,
  `guildId` bigint(20) NOT NULL,
  `module` varchar(255) COLLATE latin1_general_ci NOT NULL,
  `setting` varchar(255) COLLATE latin1_general_ci NOT NULL,
  `sValue` varchar(255) COLLATE latin1_general_ci DEFAULT NULL,
  `iValue` int(11) DEFAULT NULL,
  `biValue` bigint(20) DEFAULT NULL
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `streams`
--

CREATE TABLE IF NOT EXISTS `streams` (
  `id` int(11) NOT NULL,
  `name` varchar(255) COLLATE latin1_general_ci NOT NULL,
  `lastMessage` datetime NOT NULL,
  `approved` tinyint(4) NOT NULL DEFAULT '0',
  `discordId` bigint(20) NOT NULL,
  `guildId` bigint(20) NOT NULL,
  `type` varchar(50) COLLATE latin1_general_ci DEFAULT NULL
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `levels`
--
ALTER TABLE `levels`
  ADD UNIQUE KEY `user` (`user`);

--
-- Indexes for table `levelsExpToLevel`
--
ALTER TABLE `levelsExpToLevel`
  ADD PRIMARY KEY (`level`);

--
-- Indexes for table `settings`
--
ALTER TABLE `settings`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `streams`
--
ALTER TABLE `streams`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `settings`
--
ALTER TABLE `settings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=18;
--
-- AUTO_INCREMENT for table `streams`
--
ALTER TABLE `streams`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=28;