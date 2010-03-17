USE [master]
GO

/****** Object:  Database [HeuristicLab.PluginStore]    Script Date: 02/08/2010 19:01:45 ******/
CREATE DATABASE [HeuristicLab.PluginStore] ON  PRIMARY 
( NAME = N'HeuristicLab.PluginStore', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\HeuristicLab.PluginStore.mdf' , SIZE = 22528KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'HeuristicLab.PluginStore_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL.1\MSSQL\DATA\HeuristicLab.PluginStore_log.ldf' , SIZE = 2816KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET COMPATIBILITY_LEVEL = 90
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [HeuristicLab.PluginStore].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET ARITHABORT OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET AUTO_CREATE_STATISTICS ON 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET  DISABLE_BROKER 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET  READ_WRITE 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET  MULTI_USER 
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [HeuristicLab.PluginStore] SET DB_CHAINING OFF 
GO

USE [HeuristicLab.PluginStore]
GO

/****** Object:  Table [dbo].[Dependencies]    Script Date: 02/08/2010 19:02:19 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Dependencies](
	[PluginId] [bigint] NOT NULL,
	[DependencyId] [bigint] NOT NULL,
 CONSTRAINT [PK_Dependencies] PRIMARY KEY CLUSTERED 
(
	[PluginId] ASC,
	[DependencyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Dependencies]  WITH CHECK ADD  CONSTRAINT [FK_Dependencies_Plugin] FOREIGN KEY([DependencyId])
REFERENCES [dbo].[Plugin] ([Id])
GO

ALTER TABLE [dbo].[Dependencies] CHECK CONSTRAINT [FK_Dependencies_Plugin]
GO

ALTER TABLE [dbo].[Dependencies]  WITH CHECK ADD  CONSTRAINT [FK_Dependencies_Plugin2] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugin] ([Id])
GO

ALTER TABLE [dbo].[Dependencies] CHECK CONSTRAINT [FK_Dependencies_Plugin2]
GO


USE [HeuristicLab.PluginStore]
GO

/****** Object:  Table [dbo].[Plugin]    Script Date: 02/08/2010 19:02:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Plugin](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[Version] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Plugin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_Plugin_NameVersion] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Version] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


USE [HeuristicLab.PluginStore]
GO

/****** Object:  Table [dbo].[PluginPackage]    Script Date: 02/08/2010 19:02:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PluginPackage](
	[PluginId] [bigint] NOT NULL,
	[FileName] [text] NOT NULL,
	[Data] [image] NOT NULL,
 CONSTRAINT [PK_PluginPackage] PRIMARY KEY CLUSTERED 
(
	[PluginId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[PluginPackage]  WITH CHECK ADD  CONSTRAINT [FK_PluginPackage_Plugin] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugin] ([Id])
GO

ALTER TABLE [dbo].[PluginPackage] CHECK CONSTRAINT [FK_PluginPackage_Plugin]
GO



USE [HeuristicLab.PluginStore]
GO

/****** Object:  Table [dbo].[Product]    Script Date: 02/08/2010 19:02:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Product](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[Version] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_Product_NameVersion] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Version] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


USE [HeuristicLab.PluginStore]
GO

/****** Object:  Table [dbo].[ProductPlugin]    Script Date: 02/08/2010 19:03:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ProductPlugin](
	[ProductId] [bigint] NOT NULL,
	[PluginId] [bigint] NOT NULL,
 CONSTRAINT [PK_ProductPlugin] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[PluginId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ProductPlugin]  WITH CHECK ADD  CONSTRAINT [FK_ProductPlugin_Plugin] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugin] ([Id])
GO

ALTER TABLE [dbo].[ProductPlugin] CHECK CONSTRAINT [FK_ProductPlugin_Plugin]
GO

ALTER TABLE [dbo].[ProductPlugin]  WITH CHECK ADD  CONSTRAINT [FK_ProductPlugin_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO

ALTER TABLE [dbo].[ProductPlugin] CHECK CONSTRAINT [FK_ProductPlugin_Product]
GO


