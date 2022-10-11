using System;
using System.Collections.Generic;
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  
  public class LightweightHiveTaskDTOWrapper : LightweightTask {

    private HiveTaskDTO HiveTaskDTO { get; set; }

    public LightweightHiveTaskDTOWrapper(HiveTaskDTO hiveTaskDTO) {
      this.HiveTaskDTO = hiveTaskDTO;
      this.Id = hiveTaskDTO.Id;
      this.ExecutionTime = TimeSpan.FromSeconds(hiveTaskDTO.ExecutionTime);
      this.ParentTaskId = hiveTaskDTO.ParentTaskId;
      this.StateLog = new List<StateLog>();
      this.State = (TaskState)(int)hiveTaskDTO.TaskState;
      if(hiveTaskDTO.Command == HiveTaskCommand.None) {
        this.Command = null;
      }
      else {
        this.Command = (Command)((int)hiveTaskDTO.Command - 1);
      }
      this.LastTaskDataUpdate = DateTime.Parse(hiveTaskDTO.ModifiedAt);
    }
  }
  
}
