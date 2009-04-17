using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Hive.Client.Communication;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Contracts.BusinessObjects;

namespace HeuristicLab.Hive.Client.Core {
  public class PluginCache {
    private List<CachedHivePluginInfo> pluginCache;
    
    public PluginCache() {
      pluginCache = new List<CachedHivePluginInfo>();
    }
    
    public void AddPlugin(CachedHivePluginInfo plugin) {
      pluginCache.Add(plugin);    
    }

    public List<CachedHivePluginInfo> GetPlugins(List<HivePluginInfo> requests) {
      List<CachedHivePluginInfo> neededPlugins = new List<CachedHivePluginInfo>();
      List<HivePluginInfo> missingPlugins = new List<HivePluginInfo>();
      bool found = false;
      foreach (HivePluginInfo info in requests) {
        foreach (CachedHivePluginInfo cache in pluginCache) {
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

      List<CachedHivePluginInfo> receivedPlugins = WcfService.Instance.RequestPlugins(missingPlugins);
      if (receivedPlugins != null) {
        neededPlugins.AddRange(receivedPlugins);
        pluginCache.AddRange(receivedPlugins);
      } else
        Logging.Instance.Error(this.ToString(), "Fetching of the plugins failed!");

      return neededPlugins;
    }

  }
}
