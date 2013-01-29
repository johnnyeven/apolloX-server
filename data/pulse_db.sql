-- phpMyAdmin SQL Dump
-- version 3.5.5
-- http://www.phpmyadmin.net
--
-- 主机: localhost
-- 生成日期: 2013 年 01 月 27 日 15:23
-- 服务器版本: 5.0.90-community-nt
-- PHP 版本: 5.2.14

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
--
-- 数据库: `pulse_db_platform`
--
CREATE DATABASE `pulse_db_platform` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `pulse_db_platform`;

-- --------------------------------------------------------

--
-- 表的结构 `pulse_account`
--

CREATE TABLE IF NOT EXISTS `pulse_account` (
  `GUID` bigint(20) NOT NULL auto_increment,
  `account_name` char(32) NOT NULL,
  `account_pass` char(64) NOT NULL,
  `account_email` char(64) NOT NULL,
  `account_nickname` char(16) NOT NULL,
  `account_secret_key` char(128) NOT NULL,
  `account_firstname` char(32) NOT NULL,
  `account_lastname` char(32) NOT NULL,
  `account_country` char(16) NOT NULL,
  `account_pass_question` char(128) NOT NULL,
  `account_pass_answer` char(128) NOT NULL,
  `account_point` int(11) NOT NULL default '0',
  `account_regtime` int(11) NOT NULL default '0',
  `account_lastlogin` int(11) NOT NULL default '0',
  `account_currentlogin` int(11) NOT NULL default '0',
  `account_lastip` char(16) NOT NULL,
  `account_currentip` char(16) NOT NULL,
  `account_status` bit(1) NOT NULL default b'1',
  PRIMARY KEY  (`GUID`),
  KEY `account_name` USING BTREE (`account_name`,`account_pass`)
) ENGINE=MyISAM  DEFAULT CHARSET=utf8 AUTO_INCREMENT=3 ;

--
-- 转存表中的数据 `pulse_account`
--

INSERT INTO `pulse_account` (`GUID`, `account_name`, `account_pass`, `account_email`, `account_nickname`, `account_secret_key`, `account_firstname`, `account_lastname`, `account_country`, `account_pass_question`, `account_pass_answer`, `account_point`, `account_regtime`, `account_lastlogin`, `account_currentlogin`, `account_lastip`, `account_currentip`, `account_status`) VALUES
(1, 'Guest5177a3b6bd83410596dc7b886f0', '7dd9da2361918e5718e3b62927f564bc', '', '', '', '', '', '', '', '', 0, 0, 0, 0, '', '', b'1'),
(2, 'Guest300a69ab3f7c4c5e9e031a9cb45', 'bc1e0476e653b765fb46b5cdf35d7927', '', '', '', '', '', '', '', '', 0, 0, 0, 0, '', '', b'1');

-- --------------------------------------------------------

--
-- 表的结构 `pulse_closure_account`
--

CREATE TABLE IF NOT EXISTS `pulse_closure_account` (
  `GUID` bigint(20) NOT NULL auto_increment,
  `account_closure_reason` text,
  `account_closure_starttime` int(11) NOT NULL,
  `account_closure_endtime` int(11) NOT NULL,
  PRIMARY KEY  (`GUID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `pulse_order`
--

CREATE TABLE IF NOT EXISTS `pulse_order` (
  `funds_id` int(11) NOT NULL auto_increment,
  `account_guid` bigint(20) NOT NULL,
  `game_id` int(11) default NULL,
  `server_id` int(11) default NULL,
  `funds_flow_dir` enum('CHECK_IN','CHECK_OUT') NOT NULL,
  `funds_amount` int(11) NOT NULL,
  `funds_time` int(11) NOT NULL,
  PRIMARY KEY  (`funds_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `pulse_product`
--

CREATE TABLE IF NOT EXISTS `pulse_product` (
  `game_id` int(11) NOT NULL,
  `game_name` char(64) NOT NULL,
  `game_version` char(16) NOT NULL,
  `game_platform` enum('web','ios','android') default 'ios',
  `auth_key` char(128) NOT NULL,
  `game_pic_small` text,
  `game_pic_middium` text,
  `game_pic_big` text,
  `game_download_iphone` text,
  `game_download_ipad` text,
  `game_status` tinyint(4) NOT NULL default '0' COMMENT '0=正式,1=内测,2=公测',
  PRIMARY KEY  (`game_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- 转存表中的数据 `pulse_product`
--

INSERT INTO `pulse_product` (`game_id`, `game_name`, `game_version`, `game_platform`, `auth_key`, `game_pic_small`, `game_pic_middium`, `game_pic_big`, `game_download_iphone`, `game_download_ipad`, `game_status`) VALUES
(1001, '黑暗轨迹', '1.00', 'web', 'bbc904d185bb824e5ae5eebf5cc831cf49f44b2b', 'http://66.148.112.175/battle/resources/other/game_pic.png', 'http://66.148.112.175/battle/resources/other/game_pic3.png', NULL, NULL, NULL, 0);

--
-- 数据库: `pulse_db_game`
--
CREATE DATABASE `pulse_db_game` DEFAULT CHARACTER SET utf8 COLLATE utf8_general_ci;
USE `pulse_db_game`;

-- --------------------------------------------------------

--
-- 表的结构 `game_account`
--

CREATE TABLE IF NOT EXISTS `game_account` (
  `account_id` bigint(20) NOT NULL,
  `account_guid` bigint(20) NOT NULL,
  `account_server_id` int(11) NOT NULL,
  `nick_name` char(32) NOT NULL,
  `account_cash` bigint(20) NOT NULL default '0',
  PRIMARY KEY  (`account_guid`,`account_server_id`),
  KEY `account_id` USING BTREE (`account_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- 表的结构 `game_closure_account`
--

CREATE TABLE IF NOT EXISTS `game_closure_account` (
  `GUID` bigint(20) NOT NULL auto_increment,
  `account_closure_reason` text,
  `account_closure_starttime` int(11) NOT NULL,
  `account_closure_endtime` int(11) NOT NULL,
  PRIMARY KEY  (`GUID`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `game_log_account`
--

CREATE TABLE IF NOT EXISTS `game_log_account` (
  `log_id` bigint(20) NOT NULL auto_increment,
  `log_account_id` bigint(20) NOT NULL,
  `log_account_name` char(32) NOT NULL,
  `log_action` char(64) NOT NULL,
  `log_parameter` text NOT NULL,
  `log_time` int(11) NOT NULL default '0',
  PRIMARY KEY  (`log_id`),
  KEY `log_GUID` USING BTREE (`log_account_id`),
  KEY `log_account_name` USING BTREE (`log_account_name`),
  KEY `log_time` USING BTREE (`log_time`),
  KEY `log_action` USING BTREE (`log_action`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `game_order`
--

CREATE TABLE IF NOT EXISTS `game_order` (
  `funds_id` int(11) NOT NULL auto_increment,
  `account_id` bigint(20) NOT NULL,
  `server_id` int(11) NOT NULL,
  `funds_flow_dir` enum('CHECK_IN','CHECK_OUT') NOT NULL,
  `funds_amount` int(11) NOT NULL,
  `funds_time` int(11) NOT NULL,
  PRIMARY KEY  (`funds_id`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 AUTO_INCREMENT=1 ;

-- --------------------------------------------------------

--
-- 表的结构 `game_server`
--

CREATE TABLE IF NOT EXISTS `game_server` (
  `game_id` int(11) NOT NULL,
  `account_server_id` int(11) NOT NULL,
  `server_name` char(32) NOT NULL,
  `server_ip` char(32) NOT NULL,
  `server_port` int(11) NOT NULL,
  `server_message_ip` char(32) NOT NULL,
  `server_message_port` int(11) NOT NULL,
  `server_max_player` int(11) NOT NULL default '0',
  `account_count` int(11) NOT NULL default '0',
  `server_language` char(16) default NULL,
  `server_recommend` tinyint(1) NOT NULL default '0',
  PRIMARY KEY  (`game_id`,`account_server_id`),
  KEY `server_recommend` USING BTREE (`server_recommend`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8;

--
-- 转存表中的数据 `game_server`
--

INSERT INTO `game_server` (`game_id`, `account_server_id`, `server_name`, `server_ip`, `server_port`, `server_message_ip`, `server_message_port`, `server_max_player`, `account_count`, `server_language`, `server_recommend`) VALUES
(1001, 3001, '测试服', '127.0.0.1', 8080, '', 0, 10000, 692, 'CN', 1);

-- --------------------------------------------------------

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
