using System;
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class StateLogDTOWrapper : StateLog {

    private StateLogDTO StateLogDTO { get; set; }

    public StateLogDTOWrapper(StateLogDTO stateLogDTO) {
      this.StateLogDTO = stateLogDTO;
      this.State = (TaskState)(int)stateLogDTO.HiveTaskState;
      this.SlaveId = stateLogDTO.DroneId;
      this.TaskId = stateLogDTO.HiveTaskId.GetValueOrDefault();
      this.UserId = stateLogDTO.UserId.GetValueOrDefault();
      this.Exception = stateLogDTO.Exception;
      this.DateTime = DateTime.Parse(stateLogDTO.CreatedAt);
    }
  }
}
