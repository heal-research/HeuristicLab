using System;
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class PluginDTOWrapper : Plugin {

    private PluginDTO PluginDTO { get; set; }

    public PluginDTOWrapper(PluginDTO pluginDTO) {
      this.PluginDTO = pluginDTO;
      this.Id = pluginDTO.Id;
      this.Name = pluginDTO.Name;
      this.Version = new Version(pluginDTO.Version);
      this.UserId = pluginDTO.OwnerId ?? Guid.Empty;
      this.Hash = pluginDTO.Hash;
      //this.DateCreated = pluginDTO.CreatedAt;
    }
  }
}
