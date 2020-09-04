CREATE DEFINER=`root`@`localhost` PROCEDURE `Levels_Reset`(IN `userid` BIGINT)
    MODIFIES SQL DATA
BEGIN
DELETE from levels where levels.user = userid;
SELECT count(*) from levels where levels.user = userid;
END