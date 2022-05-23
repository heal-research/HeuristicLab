#region License Information
/* HeuristicLab
 * Copyright (C) Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using HEAL.Hive.RestClient.HiveRestClient;
using HeuristicLab.Clients.Hive.Util;
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Clients.Hive {
  public static class PluginUtilRest {


    /// <summary>
    /// Checks if plugins are available on Hive Server. If not they are uploaded. Ids are returned.
    /// </summary>active service-proxy</param>
    /// <param name="onlinePlugins">List of plugins which are available online</param>
    /// <param name="alreadyUploadedPlugins">List of plugins which have been uploaded from this Task</param>
    /// <param name="neededPlugins">List of plugins which need to be uploaded</param>
    /// <returns></returns>
    public static List<Guid> GetPluginDependencies(HiveRestClient service, List<Plugin> onlinePlugins, List<Plugin> alreadyUploadedPlugins,
                                                   IEnumerable<IPluginDescription> neededPlugins) {
      var pluginIds = new List<Guid>();
      Dictionary<IPluginDescription, byte[]> checksumsNeededPlugins = CalcChecksumsForPlugins(neededPlugins);

      foreach (var neededPlugin in checksumsNeededPlugins) {
        Plugin foundPlugin = alreadyUploadedPlugins.FirstOrDefault(p => p.Hash.SequenceEqual(neededPlugin.Value));
        if (foundPlugin == null) {
          foundPlugin = onlinePlugins.FirstOrDefault(p => {
            if (p.Hash != null) {
              return p.Hash.SequenceEqual(neededPlugin.Value);
            } else {
              return false;
            }
          });

          if (foundPlugin == null) {
            Plugin p = CreatePlugin(neededPlugin.Key, neededPlugin.Value);
            List<PluginData> pd = CreatePluginDatas(neededPlugin.Key);
            try {
              p.Id = service.PluginPost(DtoConverterUtil.convertToNewDto(p)).Id;
              foreach(var pluginData in pd) {
                pluginData.PluginId = p.Id;
                service.PluginDataPost(DtoConverterUtil.convertToNewDto(pluginData));
              }
              alreadyUploadedPlugins.Add(p);
              pluginIds.Add(p.Id);
            } catch (FaultException<PluginAlreadyExistsFault> fault) {
              onlinePlugins.Add(DtoConverterUtil.convertFromNewDto(service.PluginGet(fault.Detail.Id)));
            }
          } else {
            pluginIds.Add(foundPlugin.Id);
          }
        } else {
          pluginIds.Add(foundPlugin.Id);
        }
      }
      return pluginIds;
    }

    private static Plugin CreatePlugin(IPluginDescription plugin, byte[] hash) {
      return new Plugin() { Name = plugin.Name, Version = plugin.Version, Hash = hash };
    }

    public static List<PluginData> CreatePluginDatas(IPluginDescription plugin) {
      List<PluginData> pluginDatas = new List<PluginData>();

      foreach (IPluginFile pf in plugin.Files) {
        PluginData pluginData = new PluginData();

        pluginData.Data = File.ReadAllBytes(pf.Name);
        pluginData.FileName = Path.GetFileName(pf.Name);
        pluginDatas.Add(pluginData);
      }
      return pluginDatas;
    }

    private static void CollectPluginDependencies(List<IPluginDescription> plugins, IPluginDescription plugin) {
      if (plugin == null) return;
      foreach (var dependency in plugin.Dependencies) {
        if (!plugins.Contains(dependency)) {
          plugins.Add(dependency);
          CollectPluginDependencies(plugins, dependency);
        }
      }
    }

    private static Dictionary<IPluginDescription, byte[]> CalcChecksumsForPlugins(IEnumerable<IPluginDescription> neededPlugins) {
      Dictionary<IPluginDescription, byte[]> pluginChecksums = new Dictionary<IPluginDescription, byte[]>();

      foreach (IPluginDescription desc in neededPlugins) {
        byte[] hash;
        byte[] buffer = new byte[0];

        //calculate checksum over all files belonging to a plugin
        foreach (IPluginFile pf in desc.Files) {
          byte[] tmpBuffer = File.ReadAllBytes(pf.Name);
          byte[] newBuffer = new byte[buffer.Length + tmpBuffer.Length];
          Array.Copy(buffer, newBuffer, buffer.Length);
          Array.Copy(tmpBuffer, 0, newBuffer, buffer.Length, tmpBuffer.Length);
          buffer = newBuffer;
        }

        using (SHA1 sha1 = SHA1.Create()) {
          hash = sha1.ComputeHash(buffer);
        }
        pluginChecksums.Add(desc, hash);
      }
      return pluginChecksums;
    }
  }
}
