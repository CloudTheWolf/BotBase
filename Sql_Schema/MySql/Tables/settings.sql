SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

CREATE TABLE IF NOT EXISTS `settings` (
  `id` int(11) NOT NULL,
  `guildId` bigint(20) NOT NULL,
  `module` varchar(255) COLLATE latin1_general_ci NOT NULL,
  `setting` varchar(255) COLLATE latin1_general_ci NOT NULL,
  `sValue` varchar(255) COLLATE latin1_general_ci DEFAULT NULL,
  `iValue` int(11) DEFAULT NULL,
  `biValue` bigint(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;


ALTER TABLE `settings`
  ADD PRIMARY KEY (`id`);


ALTER TABLE `settings`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;