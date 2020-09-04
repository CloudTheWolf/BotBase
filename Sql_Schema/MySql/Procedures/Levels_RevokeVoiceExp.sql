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
END