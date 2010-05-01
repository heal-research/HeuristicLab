using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure;
using HeuristicLab.Hive.Client.Communication;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Tracing;

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
      Logger.Debug("Fetching plugins for job");
      List<CachedHivePluginInfoDto> neededPlugins = new List<CachedHivePluginInfoDto>();
      List<HivePluginInfoDto> missingPlugins = new List<HivePluginInfoDto>();
      bool found = false;
            
      foreach (HivePluginInfoDto info in requests) {
        //we MAY run in problems here - if there is a plugin twice in requests, there may be added two different versions of the plugin
        foreach (CachedHivePluginInfoDto cache in pluginCache) {
          if (info.Name.Equals(cache.Name) && info.Version.Equals(cache.Version) && info.BuildDate <= cache.BuildDate) {
            Logger.Debug("Found plugin " + info.Name + ", " + info.Version);
            neededPlugins.Add(cache);
            found = true;
            break;
          }
        }
        if (!found)
          Logger.Debug("Found NOT found " + info.Name + ", " + info.Version);
          missingPlugins.Add(info);
        found = false;
      }

      Logger.Debug("Requesting missing plugins");
      List<CachedHivePluginInfoDto> receivedPlugins = WcfService.Instance.RequestPlugins(missingPlugins);
      Logger.Debug("Requested missing plugins");

      if (receivedPlugins != null) {
        neededPlugins.AddRange(receivedPlugins);
        pluginCache.AddRange(receivedPlugins);
      } else
        Logger.Error("Fetching of the plugins failed!");

      return neededPlugins;
    }

  }
}
