using System;
using HEAL.Hive.Domain.DTOs.Entities;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class PluginDTOWrapper : Plugin {

    private PluginDTO PluginDTO { get; set; }

    public PluginDTOWrapper(PluginDTO pluginDTO) {
      this.PluginDTO = pluginDTO;
      this.Id = pluginDTO.Id;
      this.Name = pluginDTO.Name;
      this.Version = pluginDTO.Version;
      this.UserId = pluginDTO.OwnerId ?? Guid.Empty;
      this.Hash = pluginDTO.Hash;
      this.DateCreated = pluginDTO.CreatedAt;
    }
  }
}
