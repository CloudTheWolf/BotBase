SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

DELIMITER $$
CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_GiveMsgExp`(IN `userid` BIGINT, IN `exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
INSERT INTO levels (user,exp)
VALUES(userid,exp)
ON DUPLICATE KEY
UPDATE levels.exp = exp + levels.exp;
SELECT userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_GiveVoiceExp`(IN `userid` BIGINT, IN `exp` DECIMAL(11,3))
    NO SQL
BEGIN
INSERT INTO levels (user,voice_exp)
VALUES(userid,exp)
ON DUPLICATE KEY
UPDATE levels.voice_exp = exp + levels.voice_exp;
SELECT userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_Reset`(IN `userid` BIGINT)
    MODIFIES SQL DATA
BEGIN
DELETE from levels where levels.user = userid;
SELECT count(*) from levels where levels.user = userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_RevokeMsgExp`(IN `userid` BIGINT, IN `exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
SET @oldExp = (SELECT `levels`.`exp` from `levels` where `levels`.`user` = userid);
SET @newExp = @oldExp - exp;
IF @newExp < 0 THEN
 	SET @newExp = 0;
END IF;
UPDATE levels 
SET levels.exp = @newExp
WHERE levels.user = userid;

SELECT userid, @newExp, @oldExp;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_RevokeVoiceExp`(IN `userid` BIGINT, IN `exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
SET @oldExp = (SELECT `levels`.`voice_exp` from `levels` where `levels`.`user` = userid);
SET @newExp = @oldExp - exp;
IF @newExp < 0 THEN
 	SET @newExp = 0;
END IF;
UPDATE levels 
SET `levels`.`voice_exp` = @newExp
WHERE levels.user = userid;

SELECT userid, @newExp, @oldExp;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_GetUserStats`(IN `szUserId` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT levelsExpToLevel.level, levels.exp, levelsExpToLevel.badgeImageUrl FROM levelsExpToLevel
JOIN levels
where levels.user = szUserId and levelsExpToLevel.minExp <= levels.exp
ORDER BY levelsExpToLevel.level DESC
LIMIT 1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_GetUserVoiceStats`(IN `szUserId` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT levelsExpToLevel.level, levels.voice_exp, levels.voice_last_join, levelsExpToLevel.badgeImageUrl FROM levelsExpToLevel
JOIN levels
where levels.user = szUserId and levelsExpToLevel.minExp <= levels.voice_exp
ORDER BY levelsExpToLevel.level DESC
LIMIT 1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_SetVoiceState`(IN `userid` BIGINT, IN `timeJoined` DATETIME, IN `inVoice` TINYINT)
    MODIFIES SQL DATA
BEGIN
INSERT INTO levels (user,voice_last_join,isInVoice)
VALUES(userid,timeJoined,inVoice)
ON DUPLICATE KEY
UPDATE levels.voice_last_join = timeJoined, levels.isInVoice = inVoice;
SELECT userid;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `settings_AddUpdate`(IN `guildId` BIGINT, IN `module` VARCHAR(255), IN `setting` VARCHAR(255), IN `iValue` INT, IN `sValue` VARCHAR(255), IN `biValue` BIGINT)
    MODIFIES SQL DATA
BEGIN

DELETE FROM `settings` 
where `settings`.`guildId` = guildId
AND `settings`.`module`= module
AND `settings`.`setting` = setting;

INSERT INTO `settings` (`guildId`,`module`,`setting`,`sValue`,`iValue`,`biValue`)
VALUES(guildId,module,setting,iValue,sValue,biValue);

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `settings_getSetting`(IN `guildId` BIGINT, IN `module` VARCHAR(255), IN `setting` VARCHAR(255))
    READS SQL DATA
BEGIN
	SELECT * FROM `settings`
    WHERE `settings`.`guildId` = guildId
    AND `settings`.`module` = module
    AND `settings`.`setting` = setting;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_AddStream`(IN `channel` VARCHAR(255), IN `ping` TINYINT, IN `discordId` BIGINT, IN `guildId` BIGINT, IN `type` VARCHAR(50))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
INSERT INTO `streams`(`name`, `lastMessage`, `approved`, `discordId`,`guildId`,`type`) 
VALUES (channel,'2000-01-01 00:00',ping,discordId,guildid,type);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_DeleteStream`(IN `userid` BIGINT, IN `guildId` BIGINT, IN `type` VARCHAR(50))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN 

	DELETE FROM `streams`
    WHERE 
    	`streams`.`discordId` = userid AND
        `streams`.`guildId` = guildid AND
        `streams`.`type` = type;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_GetAllStreamsForGuild`(IN `guildId` BIGINT, IN `type` VARCHAR(50))
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
    WHERE `streams`.`type` = type and `streams`.`guildId` = guildId;
    

END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_getLastAnnounce`(IN `guildId` BIGINT, IN `type` VARCHAR(50))
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT `name` as 'Channel', `discordId` 
FROM `streams` 
WHERE `guildId` = guildId AND `type` = type
ORDER BY `lastMessage` desc LIMIT 1;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_GetStreamForChannelInGuild`(IN `channel` VARCHAR(100), IN `guildId` BIGINT, IN `type` VARCHAR(50), IN `userid` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
	SELECT * FROM `streams`
    WHERE `streams`.`type` = type
    AND `streams`.`guildId`= guildid
    AND (`streams`.`name`= channel 
         OR `streams`.`discordId` = userid);
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_GetStreams`(IN `type` VARCHAR(50))
    READS SQL DATA
    DETERMINISTIC
BEGIN
	SELECT `streams`.*, `settings`.`biValue` as `channelId` 
    FROM `streams`, `settings`
    WHERE `settings`.`guildId` = `streams`.`guildId`
    AND `streams`.lastMessage < DATE_ADD(NOW(), INTERVAL -1 HOUR)
    AND `settings`.`module` = 'twitch'
    AND `settings`.`setting` = 'StreamChannel'
    AND `type` = type;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `streams_setPing`(IN `userId` BIGINT, IN `guildId` BIGINT, IN `ping` TINYINT)
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
    UPDATE `streams` 
    SET `approved` = ping 
    WHERE `discordId` = userid
    and `guildId` = guildId;
END$$

CREATE DEFINER=`root`@`localhost` PROCEDURE `stream_updateTimeStamp`(IN `timestamp` DATETIME, IN `id` INT)
    NO SQL
BEGIN
	UPDATE `streams` 
    SET `lastMessage` = timestamp 
    WHERE `id` = id;
END$$

DELIMITER ;
