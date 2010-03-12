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

    private static PluginCache instance = null;
    public static PluginCache Instance {
      get {
        if (instance == null)
          instance = new PluginCache();
        return instance;
      }
    } 
    
    private List<CachedHivePluginInfoDto> pluginCache;
    

    public PluginCache() {
      pluginCache = new List<CachedHivePluginInfoDto>();
    }
    
    public void AddPlugin(CachedHivePluginInfoDto plugin) {
      pluginCache.Add(plugin);    
    }

    public List<CachedHivePluginInfoDto> GetPlugins(List<HivePluginInfoDto> requests) {
      List<CachedHivePluginInfoDto> neededPlugins = new List<CachedHivePluginInfoDto>();
      List<HivePluginInfoDto> missingPlugins = new List<HivePluginInfoDto>();
      bool found = false;
            
      foreach (HivePluginInfoDto info in requests) {
        //we MAY run in problems here - if there is a plugin twice in requests, there may be added two different versions of the plugin
        foreach (CachedHivePluginInfoDto cache in pluginCache) {
          if (info.Name.Equals(cache.Name) && info.Version.Equals(cache.Version) && info.BuildDate <= cache.BuildDate) {
            neededPlugins.Add(cache);
            found = true;
            break;
          }
        }
        if (!found)
          missingPlugins.Add(info);
        found = false;
      }

      List<CachedHivePluginInfoDto> receivedPlugins = WcfService.Instance.RequestPlugins(missingPlugins);
      if (receivedPlugins != null) {
        neededPlugins.AddRange(receivedPlugins);
        pluginCache.AddRange(receivedPlugins);
      } else
        Logging.Instance.Error(this.ToString(), "Fetching of the plugins failed!");

      return neededPlugins;
    }

  }
}
