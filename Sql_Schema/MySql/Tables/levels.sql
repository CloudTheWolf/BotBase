SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

CREATE TABLE IF NOT EXISTS `levels` (
  `user` bigint(20) NOT NULL,
  `exp` decimal(11,3) NOT NULL DEFAULT '0.000',
  `voice_exp` decimal(11,3) NOT NULL DEFAULT '0.000',
  `voice_last_join` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isInVoice` tinyint(4) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;


ALTER TABLE `levels`
  ADD UNIQUE KEY `user` (`user`);
