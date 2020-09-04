CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_RevokeMsgExp`(IN `userid` BIGINT, IN `exp` DECIMAL(11,3))
    MODIFIES SQL DATA
    DETERMINISTIC
BEGIN
DECLARE newExp decimal;
DECLARE oldExp decimal;
SET oldExp = (SELECT levels.exp from levels where levels.user = userid);
SET newExp = oldExp - exp;
IF newExp < 0 THEN
	SET newExp = 0;
END IF;
UPDATE levels 
SET levels.exp = newExp
WHERE levels.user = userid;
SELECT userid;
END