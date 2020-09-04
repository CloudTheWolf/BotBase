CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_GetUserStats`(IN `szUserId` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT levelsExpToLevel.level, levels.exp, levelsExpToLevel.badgeImageUrl FROM levelsExpToLevel
JOIN levels
where levels.user = szUserId and levelsExpToLevel.minExp <= levels.exp
ORDER BY levelsExpToLevel.level DESC
LIMIT 1;
END