/* HeuristicLab
 * Copyright (C) 2002-2015 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
USE [HeuristicLab.Hive-3.3]

EXEC sp_configure filestream_access_level, 2
GO
RECONFIGURE
GO 

SET ARITHABORT ON
CREATE TABLE [dbo].[AssignedResources](
  [ResourceId] UniqueIdentifier NOT NULL,
  [TaskId] UniqueIdentifier NOT NULL,
  CONSTRAINT [PK_dbo.ResourceIdTaskId] PRIMARY KEY ([ResourceId], [TaskId])
  )
CREATE TABLE [dbo].[Plugin](
  [PluginId] UniqueIdentifier NOT NULL,
  [Name] VarChar(MAX) NOT NULL,
  [Version] VarChar(MAX) NOT NULL,
  [UserId] UniqueIdentifier NOT NULL,
  [DateCreated] DateTime NOT NULL,
  [Hash] VarBinary(20) NOT NULL,
  CONSTRAINT [PK_dbo.Plugin] PRIMARY KEY ([PluginId])
  )
CREATE TABLE [dbo].[RequiredPlugins](
  [RequiredPluginId] UniqueIdentifier NOT NULL,
  [TaskId] UniqueIdentifier NOT NULL,
  [PluginId] UniqueIdentifier NOT NULL,
  CONSTRAINT [PK_dbo.RequiredPlugins] PRIMARY KEY ([RequiredPluginId])
  )
CREATE TABLE [dbo].[Resource](
  [ResourceId] UniqueIdentifier NOT NULL,
  [Name] VarChar(MAX) NOT NULL,
  [ResourceType] NVarChar(4000) NOT NULL,
  [ParentResourceId] UniqueIdentifier,
  [CpuSpeed] Int,
  [Memory] Int,
  [Login] DateTime,
  [SlaveState] VarChar(15),
  [Cores] Int,
  [FreeCores] Int,
  [FreeMemory] Int,
  [IsAllowedToCalculate] Bit,
  [CpuArchitecture] VarChar(3),
  [OperatingSystem] VarChar(MAX),
  [LastHeartbeat] DateTime,
  [CpuUtilization] float,
  [HbInterval] int NOT NULL,
  [IsDisposable] Bit,
  [OwnerUserId] UniqueIdentifier,
  CONSTRAINT [PK_dbo.Resource] PRIMARY KEY ([ResourceId])
  )
CREATE TABLE [dbo].[ResourcePermission](
  [ResourceId] UniqueIdentifier NOT NULL,
  [GrantedUserId] UniqueIdentifier NOT NULL,
  [GrantedByUserId] UniqueIdentifier NOT NULL,
  CONSTRAINT [PK_dbo.ResourcePermission] PRIMARY KEY ([ResourceId], [GrantedUserId])
  )
CREATE TABLE [dbo].[Task](
  [TaskId] UniqueIdentifier NOT NULL,
  [TaskState] VarChar(30) NOT NULL,
  [ExecutionTimeMs] float NOT NULL,
  [LastHeartbeat] DateTime,
  [ParentTaskId] UniqueIdentifier,
  [Priority] Int NOT NULL,
  [CoresNeeded] Int NOT NULL,
  [MemoryNeeded] Int NOT NULL,
  [IsParentTask] Bit NOT NULL,
  [FinishWhenChildJobsFinished] Bit NOT NULL,
  [Command] VarChar(30),
  [JobId] UniqueIdentifier NOT NULL,
  [IsPrivileged] Bit NOT NULL,
  CONSTRAINT [PK_dbo.Task] PRIMARY KEY ([TaskId])
  )
CREATE TABLE [dbo].[Downtime](
  [DowntimeId] UniqueIdentifier NOT NULL,
  [ResourceId] UniqueIdentifier NOT NULL,
  [StartDate] DateTime NOT NULL,
  [EndDate] DateTime NOT NULL,
  [AllDayEvent] Bit NOT NULL,
  [Recurring] Bit NOT NULL,
  [RecurringId] UniqueIdentifier NOT NULL,
  [DowntimeType] VarChar(MAX) NOT NULL,
  CONSTRAINT [PK_dbo.Downtime] PRIMARY KEY ([DowntimeId])
  )
CREATE TABLE [dbo].[Job](
  [JobId] UniqueIdentifier NOT NULL,
  [Name] VarChar(MAX) NOT NULL,
  [Description] VarChar(MAX),
  [ResourceIds] VarChar(MAX),
  [OwnerUserId] UniqueIdentifier NOT NULL,
  [DateCreated] DateTime NOT NULL,
  CONSTRAINT [PK_dbo.Job] PRIMARY KEY ([JobId])
  )
CREATE TABLE [dbo].[TaskData](
  [TaskId] UniqueIdentifier RowGuidCol NOT NULL,
  [Data] VarBinary(MAX) Filestream NOT NULL,
  [LastUpdate] DateTime NOT NULL,
  CONSTRAINT [PK_dbo.TaskData] PRIMARY KEY ([TaskId])
  )
CREATE TABLE [dbo].[PluginData](
  [PluginDataId] UniqueIdentifier RowGuidCol NOT NULL,
  [PluginId] UniqueIdentifier NOT NULL,
  [Data] VarBinary(MAX) FileStream NOT NULL,
  [FileName] VarChar(MAX) NOT NULL,
  CONSTRAINT [PK_dbo.PluginData] PRIMARY KEY ([PluginDataId])
  )
CREATE TABLE [dbo].[StateLog](
  [StateLogId] UniqueIdentifier NOT NULL,
  [State] VarChar(30) NOT NULL,
  [DateTime] DateTime NOT NULL,
  [TaskId] UniqueIdentifier NOT NULL,
  [UserId] UniqueIdentifier,
  [SlaveId] UniqueIdentifier,
  [Exception] VarChar(MAX),
  CONSTRAINT [PK_dbo.StateLog] PRIMARY KEY ([StateLogId])
  )
CREATE TABLE [dbo].[JobPermission](
  [JobId] UniqueIdentifier NOT NULL,
  [GrantedUserId] UniqueIdentifier NOT NULL,
  [GrantedByUserId] UniqueIdentifier NOT NULL,
  [Permission] VarChar(15) NOT NULL,
  CONSTRAINT [PK_dbo.JobPermission] PRIMARY KEY ([JobId], [GrantedUserId])
  )
CREATE TABLE [Lifecycle](
  [LifecycleId] Int NOT NULL,
  [LastCleanup] DateTime NOT NULL,
  CONSTRAINT [PK_Lifecycle] PRIMARY KEY ([LifecycleId])
  )
CREATE TABLE [UserPriority](
  [UserId] UniqueIdentifier NOT NULL,
  [DateEnqueued] DateTime NOT NULL,
  CONSTRAINT [PK_UserPriority] PRIMARY KEY ([UserId])
  )
ALTER TABLE [dbo].[AssignedResources]
  ADD CONSTRAINT [Resource_AssignedResource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource]([ResourceId])
ALTER TABLE [dbo].[AssignedResources]
  ADD CONSTRAINT [Task_AssignedResource] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Task]([TaskId])
ALTER TABLE [dbo].[RequiredPlugins]
  ADD CONSTRAINT [Plugin_RequiredPlugin] FOREIGN KEY ([PluginId]) REFERENCES [dbo].[Plugin]([PluginId])
ALTER TABLE [dbo].[RequiredPlugins]
  ADD CONSTRAINT [Task_RequiredPlugin] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Task]([TaskId])
ALTER TABLE [dbo].[Resource]
  ADD CONSTRAINT [Resource_Resource] FOREIGN KEY ([ParentResourceId]) REFERENCES [dbo].[Resource]([ResourceId])
ALTER TABLE [dbo].[ResourcePermission]
  ADD CONSTRAINT [Resource_ResourcePermission] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource]([ResourceId])
ALTER TABLE [dbo].[Task]
  ADD CONSTRAINT [Task_Task] FOREIGN KEY ([ParentTaskId]) REFERENCES [dbo].[Task]([TaskId])
ALTER TABLE [dbo].[Task]
  ADD CONSTRAINT [Job_Job] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job]([JobId])
ALTER TABLE [dbo].[Downtime]
  ADD CONSTRAINT [Resource_Downtime] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource]([ResourceId])
ALTER TABLE [dbo].[TaskData]
  ADD CONSTRAINT [Task_TaskData] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Task]([TaskId])
ALTER TABLE [dbo].[PluginData]
  ADD CONSTRAINT [Plugin_PluginData] FOREIGN KEY ([PluginId]) REFERENCES [dbo].[Plugin]([PluginId])
ALTER TABLE [dbo].[StateLog]
  ADD CONSTRAINT [Task_StateLog] FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Task]([TaskId])
ALTER TABLE [dbo].[StateLog]
  ADD CONSTRAINT [Resource_StateLog] FOREIGN KEY ([SlaveId]) REFERENCES [dbo].[Resource]([ResourceId])
ALTER TABLE [dbo].[JobPermission]
  ADD CONSTRAINT [Job_JobPermission] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job]([JobId])

GO
CREATE SCHEMA [statistics]
GO

CREATE TABLE [statistics].[DimTime] (
    [Time]   DATETIME NOT NULL,
    [Minute] DATETIME NOT NULL,
    [Hour]   DATETIME NOT NULL,
    [Day]    DATE     NOT NULL,
    [Month]  DATE     NOT NULL,
    [Year]   DATE     NOT NULL,
    CONSTRAINT [PK_DimTime] PRIMARY KEY CLUSTERED ([Time] ASC)
);
CREATE TABLE [statistics].[DimClient] (
    [Id]               UNIQUEIDENTIFIER CONSTRAINT [DF_DimClient_Id] DEFAULT (newsequentialid()) NOT NULL,
    [Name]             VARCHAR (MAX)    NOT NULL,
    [ResourceId]       UNIQUEIDENTIFIER NOT NULL,
    [ExpirationTime]   DATETIME         NULL,
    [ResourceGroupId]  UNIQUEIDENTIFIER NULL,
    [ResourceGroup2Id] UNIQUEIDENTIFIER NULL,
    [GroupName]        VARCHAR (MAX)    NULL,
    [GroupName2]       VARCHAR (MAX)    NULL,
    CONSTRAINT [PK_DimClient] PRIMARY KEY CLUSTERED ([Id] ASC)
);
CREATE TABLE [statistics].[DimJob] (
    [JobId]          UNIQUEIDENTIFIER NOT NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [JobName]        VARCHAR (MAX)    NOT NULL,
    [UserName]       VARCHAR (MAX)    NOT NULL,
    [DateCreated]    DATETIME		  NOT NULL,
    [TotalTasks]     INT              NOT NULL,
    [CompletedTasks] INT              NOT NULL,
    [DateCompleted]  DATETIME		  NULL,
    CONSTRAINT [PK_DimJob] PRIMARY KEY CLUSTERED ([JobId] ASC)
);
CREATE TABLE [statistics].[DimUser] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Name]   VARCHAR (MAX)    NOT NULL,
    CONSTRAINT [PK_DimUser] PRIMARY KEY CLUSTERED ([UserId] ASC)
);
CREATE TABLE [statistics].[FactClientInfo] (
    [ClientId]             UNIQUEIDENTIFIER NOT NULL,
    [Time]                 DATETIME         NOT NULL,
    [UserId]               UNIQUEIDENTIFIER NOT NULL,
    [NumUsedCores]         INT              NOT NULL,
    [NumTotalCores]        INT              NOT NULL,
    [UsedMemory]           INT              NOT NULL,
    [TotalMemory]          INT              NOT NULL,
    [CpuUtilization]       FLOAT (53)       NOT NULL,
    [SlaveState]           VarChar(15)      NOT NULL,
    [IdleTime]             INT              NOT NULL,
    [OfflineTime]          INT              NOT NULL,
    [UnavailableTime]      INT              NOT NULL,
    [IsAllowedToCalculate] BIT              NOT NULL,
    CONSTRAINT [PK_FactClientInfo] PRIMARY KEY CLUSTERED ([ClientId] ASC, [Time] ASC, [UserId] ASC),
    CONSTRAINT [FK_FactClientInfo_DimTime] FOREIGN KEY ([Time]) REFERENCES [statistics].[DimTime] ([Time]),
    CONSTRAINT [FK_FactClientInfo_DimClient] FOREIGN KEY ([ClientId]) REFERENCES [statistics].[DimClient] ([Id]),
    CONSTRAINT [FK_FactClientInfo_DimUser] FOREIGN KEY ([UserId]) REFERENCES [statistics].[DimUser] ([UserId])
);
CREATE TABLE [statistics].[FactTask] (
  [TaskId]             UNIQUEIDENTIFIER NOT NULL,
  [CalculatingTime]    INT              NOT NULL,
  [WaitingTime]        INT              NOT NULL,
  [TransferTime]       INT              NOT NULL,
  [NumCalculationRuns] INT              NOT NULL,
  [NumRetries]         INT              NOT NULL,
  [CoresRequired]      INT              NOT NULL,
  [MemoryRequired]     INT              NOT NULL,
  [Priority]           INT              NOT NULL,
  [LastClientId]       UNIQUEIDENTIFIER NULL,
  [JobId]		       UNIQUEIDENTIFIER NOT NULL,
  [StartTime]          DATETIME         NULL,
  [EndTime]            DATETIME         NULL,
  [TaskState]          VARCHAR (30)     NOT NULL,
  [Exception]          VARCHAR (MAX)    NULL,
  [InitialWaitingTime] INT              NULL,
  CONSTRAINT [PK_FactTask] PRIMARY KEY CLUSTERED ([TaskId] ASC),
  CONSTRAINT [FK_FactTask_DimClient] FOREIGN KEY ([LastClientId]) REFERENCES [statistics].[DimClient] ([Id]),
  CONSTRAINT [FK_FactTask_DimJob] FOREIGN KEY ([JobId]) REFERENCES [statistics].[DimJob] ([JobId])
);

/* dummy for nullable userIds in FactClientInfo */
INSERT INTO [statistics].[DimUser] ([UserId], [Name])
VALUES ('00000000-0000-0000-0000-000000000000', 'NULL');