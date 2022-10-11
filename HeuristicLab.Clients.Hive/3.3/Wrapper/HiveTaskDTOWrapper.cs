using System;
using System.Collections.Generic;
using System.Linq;
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper { 
  public class HiveTaskDTOWrapper : Task {

    private HiveTaskDTO HiveTaskDTO { get; set; }

    public HiveTaskDTOWrapper(HiveTaskDTO hiveTaskDTO) {
      this.HiveTaskDTO = hiveTaskDTO;
      this.Id = hiveTaskDTO.Id;
      this.ExecutionTime = TimeSpan.FromSeconds(hiveTaskDTO.ExecutionTime);
      this.ParentTaskId = hiveTaskDTO.ParentTaskId;
      this.StateLog = new List<StateLog>();
      this.State = (TaskState)(int)hiveTaskDTO.TaskState;
      if (hiveTaskDTO.Command == HiveTaskCommand.None) {
        this.Command = null;
      } else {
        this.Command = (Command)((int)hiveTaskDTO.Command - 1);
      }
      this.LastTaskDataUpdate = DateTime.Parse(hiveTaskDTO.ModifiedAt);
      this.Priority = hiveTaskDTO.Priority;
      this.CoresNeeded = hiveTaskDTO.CoresNeeded;
      this.MemoryNeeded = hiveTaskDTO.MemoryNeeded;
      this.PluginsNeededIds = hiveTaskDTO.RequiredPlugins.ToList();
      this.LastHeartbeat = DateTime.Parse(hiveTaskDTO.LastHeartbeatAt);
      this.IsParentTask = hiveTaskDTO.ChildHiveTasks.Any();
      this.FinishWhenChildJobsFinished = hiveTaskDTO.FinishWhenChildJobsFinished;
      this.JobId = hiveTaskDTO.HiveJobId;
    }
  }
}
