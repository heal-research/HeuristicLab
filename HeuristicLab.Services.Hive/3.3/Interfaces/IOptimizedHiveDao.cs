#region License Information
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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using HeuristicLab.Services.Hive.DataAccess;
using DT = HeuristicLab.Services.Hive.DataTransfer;
namespace HeuristicLab.Services.Hive {
  public interface IOptimizedHiveDao {
    #region Task Methods
    Task GetTaskById(Guid task);
    Task GetTaskByDto(DT.Task taskDto);
    Tuple<Task, Guid?> GetTaskByIdAndLastStateLogSlaveId(Guid taskId);

    IEnumerable<TaskInfoForScheduler> GetWaitingTasks(Slave slave);
    IQueryable<DT.LightweightTask> GetLightweightTasks(Guid jobId);

    void UpdateTask(Task task);
    Task UpdateTaskState(Guid taskId, TaskState taskState, Guid? slaveId, Guid? userId, string exception);

    Guid AddTask(Task task);
    void AssignJobToResource(Guid taskId, IEnumerable<Guid> resourceIds);

    bool TaskIsAllowedToBeCalculatedBySlave(Guid taskId, Guid slaveId);
    #endregion

    #region TaskData Methods
    TaskData GetTaskDataByDto(DT.TaskData taskDataDto);
    void UpdateTaskData(TaskData taskData);
    #endregion

    #region Plugin Methods
    Plugin GetPluginById(Guid pluginId);
    #endregion

    #region Slave Methods
    Slave GetSlaveById(Guid id);

    void UpdateSlave(Slave slave);

    bool SlaveHasToShutdownComputer(Guid slaveId);
    bool SlaveIsAllowedToCalculate(Guid slaveId);
    #endregion

    #region Resource Methods
    IEnumerable<Guid> GetAssignedResourceIds(Guid jobId);
    #endregion

    #region Website Methods

    IEnumerable<Guid> GetAllResourceIds();

    int GetNumberOfWaitingTasks();

    Dictionary<Guid, int> GetCalculatingTasksByUser();

    Dictionary<Guid, int> GetWaitingTasksByUser();

    #endregion
  }
}
