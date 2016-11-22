USE [master]
GO

/****** Object:  Database [HeuristicLab.AccessService]    Script Date: 09/19/2011 13:21:28 ******/
CREATE DATABASE [HeuristicLab.AccessService] ON  PRIMARY 
( NAME = N'HeuristicLab.AccessService', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\HeuristicLab.AccessService.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'HeuristicLab.AccessService_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\HeuristicLab.AccessService_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [HeuristicLab.AccessService] SET COMPATIBILITY_LEVEL = 100
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HeuristicLab.AccessService].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [HeuristicLab.AccessService] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET ARITHABORT OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET  DISABLE_BROKER 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET  READ_WRITE 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET RECOVERY FULL 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET  MULTI_USER 
GO

ALTER DATABASE [HeuristicLab.AccessService] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [HeuristicLab.AccessService] SET DB_CHAINING OFF 
GO


