using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class DroneGroupDTOWrapper : SlaveGroup {

    private DroneGroupDTO DroneGroupDTO { get; set; }

    public DroneGroupDTOWrapper(DroneGroupDTO droneGroupDTO) {
      this.DroneGroupDTO = droneGroupDTO;
      this.Id = droneGroupDTO.Id;
      this.Name = droneGroupDTO.Name;
      this.HbInterval = droneGroupDTO.HeartbeatInterval;
      this.OwnerUserId = droneGroupDTO.OwnerId;
      this.ParentResourceId = droneGroupDTO.ParentDroneGroupId;
    }
  }
}
