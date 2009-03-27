using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Hive.Client.Communication;
using HeuristicLab.Hive.Client.Common;

namespace HeuristicLab.Hive.Client.Core {
  public class PluginCache {
    private List<CachedPlugin> pluginCache;
    
    public PluginCache() {
      pluginCache = new List<CachedPlugin>();
    }
    
    public void AddPlugin(CachedPlugin plugin) {
      pluginCache.Add(plugin);    
    }

    public List<CachedPlugin> GetPlugins(List<PluginInfo> requests) {
      List<CachedPlugin> neededPlugins = new List<CachedPlugin>();
      List<PluginInfo> missingPlugins = new List<PluginInfo>();
      bool found = false;
      foreach (PluginInfo info in requests) {
        foreach (CachedPlugin cache in pluginCache) {
          if (info.Equals(cache)) {
            neededPlugins.Add(cache);
            found = true;
            break;
          }
        }
        if (!found)
          missingPlugins.Add(info);
        found = false;
      }

      List<CachedPlugin> receivedPlugins = WcfService.Instance.RequestPlugins(missingPlugins);
      if (receivedPlugins != null)
        neededPlugins.AddRange(receivedPlugins);
      else
        Logging.Instance.Error(this.ToString(), "Fetching of the plugins failed!");

      return neededPlugins;
    }

  }
}
