using System;
using HEAL.Hive.Domain.DTOs.Entities;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class DroneDTOWrapper : Slave {

    private DroneDTO DroneDTO { get; set; }

    public DroneDTOWrapper(DroneDTO droneDTO) {
      this.DroneDTO = droneDTO;
      this.Id = droneDTO.Id;
      this.Name = droneDTO.Name;
      this.Cores = droneDTO.Cores;
      this.FreeCores = droneDTO.FreeCores;
      this.Memory = droneDTO.Memory;
      this.FreeMemory = droneDTO.FreeMemory;
      this.HbInterval = droneDTO.HeartbeatInterval;
      this.IsAllowedToCalculate = droneDTO.IsAllowedToCalculate;
      this.IsDisposable = droneDTO.DisposeOnInactivity;
      this.LastHeartbeat = droneDTO.LastHeartbeatAt;
      this.OperatingSystem = droneDTO.OperatingSystem;
      this.OwnerUserId = droneDTO.OwnerId;
      this.ParentResourceId = droneDTO.ParentDroneGroupId;
      this.SlaveState = (SlaveState) (int) droneDTO.DroneState;      
    }
  }
}
