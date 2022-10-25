using System;
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class HiveTaskDataDTOWrapper : TaskData {

    private HiveTaskDataDTO HiveTaskDTO { get; set; }

    public HiveTaskDataDTOWrapper(HiveTaskDataDTO hiveTaskDataDTO) {
      this.HiveTaskDTO = hiveTaskDataDTO;
      this.LastUpdate = DateTime.Parse(hiveTaskDataDTO.ModifiedAt);
      this.Data = hiveTaskDataDTO.Data;
    }
  }
}
