USE [HeuristicLab.Hive-3.3]
/* create and initialize hive database tables */

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
  CONSTRAINT [PK_dbo.Resource] PRIMARY KEY ([ResourceId])
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
  [TaskId] UniqueIdentifier NOT NULL,
  [Data] VarBinary(MAX) NOT NULL,
  [LastUpdate] DateTime NOT NULL,
  CONSTRAINT [PK_dbo.TaskData] PRIMARY KEY ([TaskId])
  )
CREATE TABLE [dbo].[PluginData](
  [PluginDataId] UniqueIdentifier NOT NULL,
  [PluginId] UniqueIdentifier NOT NULL,
  [Data] VarBinary(MAX) NOT NULL,
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
CREATE TABLE [DeletedJobStatistics](
  [UserId] UniqueIdentifier NOT NULL,
  [ExecutionTimeS] float NOT NULL,
  [ExecutionTimeSFinishedJobs] float NOT NULL,
  [StartToEndTimeS] float NOT NULL,
  [DeletedJobStatisticsId] UniqueIdentifier NOT NULL,
  CONSTRAINT [PK_DeletedJobStatistics] PRIMARY KEY ([DeletedJobStatisticsId])
  )
CREATE TABLE [UserStatistics](
  [StatisticsId] UniqueIdentifier NOT NULL,
  [UserId] UniqueIdentifier NOT NULL,
  [ExecutionTimeMs] float NOT NULL,
  [UsedCores] Int NOT NULL,
  [ExecutionTimeMsFinishedJobs] float NOT NULL,
  [StartToEndTimeMs] float NOT NULL,
  CONSTRAINT [PK_UserStatistics] PRIMARY KEY ([StatisticsId], [UserId])
  )
CREATE TABLE [SlaveStatistics](
  [StatisticsId] UniqueIdentifier NOT NULL,
  [SlaveId] UniqueIdentifier NOT NULL,
  [Cores] Int NOT NULL,
  [FreeCores] Int NOT NULL,
  [CpuUtilization] float NOT NULL,
  [Memory] Int NOT NULL,
  [FreeMemory] Int NOT NULL,
  CONSTRAINT [PK_SlaveStatistics] PRIMARY KEY ([StatisticsId], [SlaveId])
  )
CREATE TABLE [Statistics](
  [StatisticsId] UniqueIdentifier NOT NULL,
  [Timestamp] DateTime NOT NULL,
  CONSTRAINT [PK_Statistics] PRIMARY KEY ([StatisticsId])
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
ALTER TABLE [UserStatistics]
  ADD CONSTRAINT [Statistics_UserStatistics] FOREIGN KEY ([StatisticsId]) REFERENCES [Statistics]([StatisticsId])
ALTER TABLE [SlaveStatistics]
  ADD CONSTRAINT [Statistics_SlaveStatistics] FOREIGN KEY ([StatisticsId]) REFERENCES [Statistics]([StatisticsId])
