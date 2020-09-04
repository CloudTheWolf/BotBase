SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

DROP TABLE IF EXISTS `levelsExpToLevel`;
CREATE TABLE IF NOT EXISTS `levelsExpToLevel` (
  `level` int(11) NOT NULL,
  `minExp` decimal(11,3) NOT NULL,
  `badgeImageUrl` varchar(255) COLLATE latin1_general_ci DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

INSERT INTO `levelsExpToLevel` (`level`, `minExp`, `badgeImageUrl`) VALUES
(0, '0.000', NULL),
(1, '1.000', NULL),
(2, '2.000', NULL),
(3, '4.000', NULL),
(4, '8.000', NULL),
(5, '16.000', NULL),
(6, '34.000', NULL),
(7, '64.000', NULL),
(8, '128.000', NULL),
(9, '256.000', NULL);


ALTER TABLE `levelsExpToLevel`
  ADD PRIMARY KEY (`level`);
