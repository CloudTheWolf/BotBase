CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_SetVoiceState`(IN `userid` BIGINT, IN `timeJoined` DATETIME, IN `inVoice` TINYINT)
    MODIFIES SQL DATA
BEGIN
INSERT INTO levels (user,voice_last_join,isInVoice)
VALUES(userid,timeJoined,inVoice)
ON DUPLICATE KEY
UPDATE levels.voice_last_join = timeJoined, levels.isInVoice = inVoice;
SELECT userid;
END