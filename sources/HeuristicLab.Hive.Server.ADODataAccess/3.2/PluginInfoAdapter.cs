using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.Hive.Server.DataAccess;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Hive.Contracts.BusinessObjects;
using HeuristicLab.Hive.Server.ADODataAccess.TableAdapterWrapper;
using System.Xml.Serialization;
using HeuristicLab.PluginInfrastructure;
using System.IO;

namespace HeuristicLab.Hive.Server.ADODataAccess {
  class PluginInfoAdapter:
    DataAdapterBase<dsHiveServerTableAdapters.PluginInfoTableAdapter,
      HivePluginInfo,
      dsHiveServer.PluginInfoRow>,
    IPluginInfoAdapter {

    public PluginInfoAdapter() : 
      base(new PluginInfoAdapterWrapper()) {
    }

    protected override dsHiveServer.PluginInfoRow ConvertObj(HivePluginInfo pluginInfo, 
      dsHiveServer.PluginInfoRow row) {
      if (row != null && pluginInfo != null) {
        row.PluginId = pluginInfo.Id;
        row.Name = pluginInfo.Name;
        row.Version = pluginInfo.Version;
        row.BuildDate = pluginInfo.BuildDate;

        return row;
      } else {
        return null;
      }       
    }

    protected override HivePluginInfo ConvertRow(dsHiveServer.PluginInfoRow row,
      HivePluginInfo pluginInfo) {
      if (row != null && pluginInfo != null) {
        pluginInfo.Id = row.PluginId;

        if (!row.IsNameNull()) {
          pluginInfo.Name = row.Name;
        } else {
          pluginInfo.Name = null;
        }

        if (!row.IsVersionNull()) {
          pluginInfo.Version = row.Version;
        } else {
          pluginInfo.Version = null;
        }

        if (!row.IsBuildDateNull()) {
          pluginInfo.BuildDate = row.BuildDate;
        } else {
          pluginInfo.BuildDate = DateTime.Now;
        }

        return pluginInfo;
      } else {
        return null;
      } 
    }

    public HivePluginInfo GetByNameVersionBuilddate(String name, String version, DateTime buildDate) {
      return
         base.FindSingle(
           delegate() {
             return Adapter.GetDataByNameVersionBuilddate(name, version, buildDate);
           });
    }

    public ICollection<HivePluginInfo> GetOrphanedPluginInfos() {
      return
        base.FindMultiple(
          delegate() {
            return Adapter.GetDataByOrphaned();
          });
    }
  }
}
