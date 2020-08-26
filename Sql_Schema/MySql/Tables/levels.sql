-- phpMyAdmin SQL Dump
-- version 4.4.15.10
-- https://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Aug 26, 2020 at 08:00 PM
-- Server version: 5.7.29
-- PHP Version: 5.6.40

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


-- --------------------------------------------------------

--
-- Table structure for table `levels`
--

CREATE TABLE IF NOT EXISTS `levels` (
  `user` bigint(20) NOT NULL,
  `exp` int(11) NOT NULL DEFAULT '0',
  `voice_exp` int(11) NOT NULL DEFAULT '0',
  `voice_last_join` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `isInVoice` tinyint(4) NOT NULL DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `levels`
--
ALTER TABLE `levels`
  ADD UNIQUE KEY `user` (`user`);
