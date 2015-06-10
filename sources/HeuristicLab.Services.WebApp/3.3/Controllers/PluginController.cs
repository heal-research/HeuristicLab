using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using DTO = HeuristicLab.Services.WebApp.Controllers.DataTransfer;

namespace HeuristicLab.Services.WebApp.Controllers {

  [Authorize(Roles = "Hive Administrator")]
  public class PluginController : ApiController {

    private readonly PluginManager pluginManager = PluginManager.Instance;

    public IEnumerable<DTO.Plugin> GetPlugins() {
      var plugins = pluginManager.GetPlugins();
      return plugins.Select(plugin => new DTO.Plugin {
        Name = plugin.Name,
        AssemblyName = plugin.AssemblyName,
        LastReload = plugin.LastReload
      });
    }

    public bool ReloadPlugin(string name) {
      var plugin = PluginManager.Instance.GetPlugin(name);
      plugin.ReloadControllers();
      return true;
    }
  }
}
