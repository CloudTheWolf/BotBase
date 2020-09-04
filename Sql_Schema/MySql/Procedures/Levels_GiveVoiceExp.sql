CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_GiveVoiceExp`(IN `userid` BIGINT, IN `exp` DECIMAL(11,3))
    NO SQL
BEGIN
INSERT INTO levels (user,voice_exp)
VALUES(userid,exp)
ON DUPLICATE KEY
UPDATE levels.voice_exp = exp + levels.voice_exp;
SELECT userid;
END