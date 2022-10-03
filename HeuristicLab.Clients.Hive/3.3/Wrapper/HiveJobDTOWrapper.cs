using System;
using System.Collections.Generic;
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class HiveJobDTOWrapper : Job {

    private HiveJobDTO HiveJobDTO { get; set; }

    public HiveJobDTOWrapper(HiveJobDTO hiveJobDTO) {
      this.HiveJobDTO = hiveJobDTO;
      this.Id = hiveJobDTO.Id;
      this.Name = hiveJobDTO.Name;
      this.Description = hiveJobDTO.Description;
      //this.DateCreated = hiveJobDTO.CreatedAt;
      this.OwnerUserId = hiveJobDTO.OwnerId ?? Guid.Empty;
      this.State = (JobState) (int) hiveJobDTO.HiveJobState;
      this.ResourceIds = new List<Guid>(hiveJobDTO.AssignedComputingResources);
    }
  }
}
