USE [HeuristicLab.Deployment]
GO
/****** Object:  Table [dbo].[Plugin]    Script Date: 03/22/2010 15:57:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plugin](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NOT NULL,
	[Version] [nvarchar](50) NOT NULL,
	[ContactName] [text] NULL,
	[ContactEmail] [text] NULL,
	[License] [text] NULL,
 CONSTRAINT [PK_Plugin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_Plugin_NameVersion] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[Version] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 03/22/2010 15:57:25 ******/
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
/****** Object:  Table [dbo].[ProductPlugin]    Script Date: 03/22/2010 15:57:25 ******/
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
/****** Object:  Table [dbo].[PluginPackage]    Script Date: 03/22/2010 15:57:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PluginPackage](
	[PluginId] [bigint] NOT NULL,
	[Data] [image] NOT NULL,
 CONSTRAINT [PK_PluginPackage] PRIMARY KEY CLUSTERED 
(
	[PluginId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Dependencies]    Script Date: 03/22/2010 15:57:25 ******/
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
/****** Object:  Check [CK_Dependencies]    Script Date: 03/22/2010 15:57:25 ******/
ALTER TABLE [dbo].[Dependencies]  WITH CHECK ADD  CONSTRAINT [CK_Dependencies] CHECK  (([PluginId]<>[DependencyId]))
GO
ALTER TABLE [dbo].[Dependencies] CHECK CONSTRAINT [CK_Dependencies]
GO
/****** Object:  ForeignKey [FK_ProductPlugin_Plugin]    Script Date: 03/22/2010 15:57:25 ******/
ALTER TABLE [dbo].[ProductPlugin]  WITH CHECK ADD  CONSTRAINT [FK_ProductPlugin_Plugin] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugin] ([Id])
GO
ALTER TABLE [dbo].[ProductPlugin] CHECK CONSTRAINT [FK_ProductPlugin_Plugin]
GO
/****** Object:  ForeignKey [FK_ProductPlugin_Product]    Script Date: 03/22/2010 15:57:25 ******/
ALTER TABLE [dbo].[ProductPlugin]  WITH CHECK ADD  CONSTRAINT [FK_ProductPlugin_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([Id])
GO
ALTER TABLE [dbo].[ProductPlugin] CHECK CONSTRAINT [FK_ProductPlugin_Product]
GO
/****** Object:  ForeignKey [FK_PluginPackage_Plugin]    Script Date: 03/22/2010 15:57:25 ******/
ALTER TABLE [dbo].[PluginPackage]  WITH CHECK ADD  CONSTRAINT [FK_PluginPackage_Plugin] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugin] ([Id])
GO
ALTER TABLE [dbo].[PluginPackage] CHECK CONSTRAINT [FK_PluginPackage_Plugin]
GO
/****** Object:  ForeignKey [FK_Dependencies_Plugin]    Script Date: 03/22/2010 15:57:25 ******/
ALTER TABLE [dbo].[Dependencies]  WITH CHECK ADD  CONSTRAINT [FK_Dependencies_Plugin] FOREIGN KEY([DependencyId])
REFERENCES [dbo].[Plugin] ([Id])
GO
ALTER TABLE [dbo].[Dependencies] CHECK CONSTRAINT [FK_Dependencies_Plugin]
GO
/****** Object:  ForeignKey [FK_Dependencies_Plugin2]    Script Date: 03/22/2010 15:57:25 ******/
ALTER TABLE [dbo].[Dependencies]  WITH CHECK ADD  CONSTRAINT [FK_Dependencies_Plugin2] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugin] ([Id])
GO
ALTER TABLE [dbo].[Dependencies] CHECK CONSTRAINT [FK_Dependencies_Plugin2]
GO
