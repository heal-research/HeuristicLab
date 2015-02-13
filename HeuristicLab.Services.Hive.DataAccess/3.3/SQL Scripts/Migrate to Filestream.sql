/* Upgrade script from HeuristicLab Hive 3.3.8 to 3.3.9 */

USE [HeuristicLab.Hive-3.3]

EXEC sp_configure filestream_access_level, 2
GO
RECONFIGURE
GO 

/**********************************************************/

/* Move old Task Data */

CREATE TABLE [dbo].[TaskDataTemp](
  [TaskId] UniqueIdentifier RowGuidCol NOT NULL,
  [Data] VarBinary(MAX) Filestream NOT NULL,
  [LastUpdate] DateTime NOT NULL,
  CONSTRAINT [PK_dbo.TaskDataTemp] PRIMARY KEY ([TaskId])
)

INSERT INTO dbo.TaskDataTemp (TaskId, Data, LastUpdate)
SELECT TaskId, Data, LastUpdate
FROM dbo.TaskData

DELETE FROM dbo.TaskData

/* Alter TaskId and Data Column */

ALTER TABLE dbo.TaskData
ALTER COLUMN [TaskId] ADD RowGuidCol

ALTER TABLE dbo.TaskData
DROP COLUMN Data

ALTER TABLE dbo.TaskData
ADD [Data] VarBinary(MAX) Filestream NOT NULL

/* Insert data */

INSERT INTO dbo.TaskData (TaskId, Data, LastUpdate)
SELECT TaskId, Data, LastUpdate
FROM dbo.TaskDataTemp

DROP TABLE dbo.TaskDataTemp


/**********************************************************/

/* Move old Plugin Data */

CREATE TABLE [dbo].[PluginDataTemp](
  [PluginDataId] UniqueIdentifier RowGuidCol NOT NULL,
  [PluginId] UniqueIdentifier NOT NULL,
  [Data] VarBinary(MAX) FileStream NOT NULL,
  [FileName] VarChar(MAX) NOT NULL,
  CONSTRAINT [PK_dbo.PluginDataTemp] PRIMARY KEY ([PluginDataId])
)

INSERT INTO dbo.PluginDataTemp (PluginDataId, PluginId, Data, [FileName])
SELECT PluginDataId, PluginId, Data, [FileName]
FROM dbo.PluginData

DELETE FROM dbo.PluginData

/* Alter Data Column */

ALTER TABLE dbo.PluginData
DROP COLUMN Data

ALTER TABLE dbo.PluginData
ADD [Data] VarBinary(MAX) Filestream NOT NULL

/* Insert data */

INSERT INTO dbo.PluginData (PluginDataId, PluginId, Data, [FileName])
SELECT PluginDataId, PluginId, Data, [FileName]
FROM dbo.PluginDataTemp

DROP TABLE dbo.PluginDataTemp