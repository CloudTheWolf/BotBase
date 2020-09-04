CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_GiveMsgExp`(IN `userid` BIGINT, IN `exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
INSERT INTO levels (user,exp)
VALUES(userid,exp)
ON DUPLICATE KEY
UPDATE levels.exp = exp + levels.exp;
SELECT userid;
END