CREATE DEFINER=`root`@`localhost` PROCEDURE `Level_GetUserVoiceStats`(IN `szUserId` BIGINT)
    READS SQL DATA
    DETERMINISTIC
BEGIN
SELECT levelsExpToLevel.level, levels.voice_exp, levels.voice_last_join, levelsExpToLevel.badgeImageUrl FROM levelsExpToLevel
JOIN levels
where levels.user = szUserId and levelsExpToLevel.minExp <= levels.voice_exp
ORDER BY levelsExpToLevel.level DESC
LIMIT 1;
END