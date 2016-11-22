USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[ClientConfiguration]    Script Date: 09/27/2011 17:05:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientConfiguration](
	[Id] [uniqueidentifier] NOT NULL,
	[Hash] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_ClientConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClientConfiguration] ADD  CONSTRAINT [DF_ClientConfiguration_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[ClientType]    Script Date: 09/27/2011 17:06:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientType](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ClientType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClientType] ADD  CONSTRAINT [DF_ClientType_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[Country]    Script Date: 09/27/2011 17:06:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Country](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Country] ADD  CONSTRAINT [DF_Country_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[OperatingSystem]    Script Date: 09/27/2011 17:06:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OperatingSystem](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_OperatingSystem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[OperatingSystem] ADD  CONSTRAINT [DF_OperatingSystem_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[Plugin]    Script Date: 09/27/2011 17:06:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Plugin](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[StrongName] [nvarchar](max) NULL,
	[Version] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_Plugin] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Plugin] ADD  CONSTRAINT [DF_Plugin_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[Resource]    Script Date: 09/27/2011 17:07:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Resource](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Type] [nvarchar](max) NOT NULL,
	[ProcessorType] [nvarchar](max) NULL,
	[NumberOfCores] [int] NULL,
	[MemorySize] [int] NULL,
	[OperatingSystemId] [uniqueidentifier] NULL,
	[CountryId] [uniqueidentifier] NULL,
	[HeuristicLabVersion] [nvarchar](max) NULL,
	[ClientTypeId] [uniqueidentifier] NULL,
	[ClientConfigurationId] [uniqueidentifier] NULL,
	[Timestamp] [datetime] NULL,
	[PerformanceValue] [real] NULL,
 CONSTRAINT [PK_Resource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Resource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_ClientConfiguration] FOREIGN KEY([ClientConfigurationId])
REFERENCES [dbo].[ClientConfiguration] ([Id])
GO

ALTER TABLE [dbo].[Resource] CHECK CONSTRAINT [FK_Resource_ClientConfiguration]
GO

ALTER TABLE [dbo].[Resource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_ClientType] FOREIGN KEY([ClientTypeId])
REFERENCES [dbo].[ClientType] ([Id])
GO

ALTER TABLE [dbo].[Resource] CHECK CONSTRAINT [FK_Resource_ClientType]
GO

ALTER TABLE [dbo].[Resource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_Country] FOREIGN KEY([CountryId])
REFERENCES [dbo].[Country] ([Id])
GO

ALTER TABLE [dbo].[Resource] CHECK CONSTRAINT [FK_Resource_Country]
GO

ALTER TABLE [dbo].[Resource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_OperatingSystem] FOREIGN KEY([OperatingSystemId])
REFERENCES [dbo].[OperatingSystem] ([Id])
GO

ALTER TABLE [dbo].[Resource] CHECK CONSTRAINT [FK_Resource_OperatingSystem]
GO

ALTER TABLE [dbo].[Resource] ADD  CONSTRAINT [DF_Resource_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[ResourceResourceGroup]    Script Date: 09/27/2011 17:07:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ResourceResourceGroup](
	[ResourceId] [uniqueidentifier] NOT NULL,
	[ResourceGroupId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ResourceResourceGroup_Id] PRIMARY KEY CLUSTERED 
(
	[ResourceId] ASC,
	[ResourceGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ResourceResourceGroup]  WITH CHECK ADD  CONSTRAINT [FK_ResourceResourceGroup_ResourceGroupId] FOREIGN KEY([ResourceGroupId])
REFERENCES [dbo].[Resource] ([Id])
GO

ALTER TABLE [dbo].[ResourceResourceGroup] CHECK CONSTRAINT [FK_ResourceResourceGroup_ResourceGroupId]
GO

ALTER TABLE [dbo].[ResourceResourceGroup]  WITH CHECK ADD  CONSTRAINT [FK_ResourceResourceGroup_ResourceId] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO

ALTER TABLE [dbo].[ResourceResourceGroup] CHECK CONSTRAINT [FK_ResourceResourceGroup_ResourceId]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[ResourcePlugin]    Script Date: 09/27/2011 17:07:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ResourcePlugin](
	[ResourceId] [uniqueidentifier] NOT NULL,
	[PluginId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ResourcePlugin] PRIMARY KEY CLUSTERED 
(
	[ResourceId] ASC,
	[PluginId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ResourcePlugin]  WITH CHECK ADD  CONSTRAINT [FK_ResourcePlugin_Plugin] FOREIGN KEY([PluginId])
REFERENCES [dbo].[Plugin] ([Id])
GO

ALTER TABLE [dbo].[ResourcePlugin] CHECK CONSTRAINT [FK_ResourcePlugin_Plugin]
GO

ALTER TABLE [dbo].[ResourcePlugin]  WITH CHECK ADD  CONSTRAINT [FK_ResourcePlugin_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO

ALTER TABLE [dbo].[ResourcePlugin] CHECK CONSTRAINT [FK_ResourcePlugin_Resource]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[UserGroup]    Script Date: 09/27/2011 17:07:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserGroup](
	[Id] [uniqueidentifier] NOT NULL,
	[FullName] [nvarchar](max) NULL,
	[Name] [nvarchar](max) NULL,
	[Type] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_UserGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[UserGroupUserGroup]    Script Date: 09/27/2011 17:07:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UserGroupUserGroup](
	[UserGroupId] [uniqueidentifier] NOT NULL,
	[UserGroupUserGroupId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_UserGroupUserGroup] PRIMARY KEY CLUSTERED 
(
	[UserGroupId] ASC,
	[UserGroupUserGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[UserGroupUserGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupUserGroup_UserGroup] FOREIGN KEY([UserGroupId])
REFERENCES [dbo].[UserGroup] ([Id])
GO

ALTER TABLE [dbo].[UserGroupUserGroup] CHECK CONSTRAINT [FK_UserGroupUserGroup_UserGroup]
GO

ALTER TABLE [dbo].[UserGroupUserGroup]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupUserGroup_UserGroup1] FOREIGN KEY([UserGroupUserGroupId])
REFERENCES [dbo].[UserGroup] ([Id])
GO

ALTER TABLE [dbo].[UserGroupUserGroup] CHECK CONSTRAINT [FK_UserGroupUserGroup_UserGroup1]
GO


USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[ClientError]    Script Date: 09/27/2011 17:05:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientError](
	[Id] [uniqueidentifier] NOT NULL,
	[Timestamp] [datetime] NOT NULL,
	[Exception] [nvarchar](max) NULL,
	[UserComment] [nvarchar](max) NULL,
	[ConfigDump] [nvarchar](max) NULL,
	[ClientId] [uniqueidentifier] NULL,
	[UserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_ClientError] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClientError]  WITH CHECK ADD  CONSTRAINT [FK_ClientError_Resource] FOREIGN KEY([ClientId])
REFERENCES [dbo].[Resource] ([Id])
GO

ALTER TABLE [dbo].[ClientError] CHECK CONSTRAINT [FK_ClientError_Resource]
GO

ALTER TABLE [dbo].[ClientError]  WITH CHECK ADD  CONSTRAINT [FK_ClientError_UserGroup] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserGroup] ([Id])
GO

ALTER TABLE [dbo].[ClientError] CHECK CONSTRAINT [FK_ClientError_UserGroup]
GO

ALTER TABLE [dbo].[ClientError] ADD  CONSTRAINT [DF_ClientError_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

USE [HeuristicLab.AccessService]
GO

/****** Object:  Table [dbo].[ClientLog]    Script Date: 09/27/2011 17:05:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ClientLog](
	[Timestamp] [datetime] NOT NULL,
	[ResourceId] [uniqueidentifier] NOT NULL,
	[Message] [nvarchar](max) NULL,
 CONSTRAINT [PK_ClientLog] PRIMARY KEY CLUSTERED 
(
	[Timestamp] ASC,
	[ResourceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ClientLog]  WITH CHECK ADD  CONSTRAINT [FK_ClientLog_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([Id])
GO

ALTER TABLE [dbo].[ClientLog] CHECK CONSTRAINT [FK_ClientLog_Resource]
GO






