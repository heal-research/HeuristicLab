#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2008 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.ComponentModel;

namespace HeuristicLab.PluginInfrastructure.GUI {
  class PluginSource {

    private string url;
    private WebClient client;
    private string cacheDir = HeuristicLab.PluginInfrastructure.GUI.Properties.Settings.Default.CacheDir;

    private PluginSource(string url) {
      this.url = url;
    }

    /// <summary>
    /// Factory method for new PluginRepositories
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static PluginSource TryCreate(string url) {
      PluginSource newSource = new PluginSource(url);
      if(newSource.VerifyRepositoryLocation()) {
        return newSource;
      } else {
        return null;
      }
    }

    private bool VerifyRepositoryLocation() {
      try {
        this.client = new WebClient();
        client.DownloadData(url + "/plugins.xml");
      } catch(Exception) {
        return false;
      }
      return true;
    }

    public List<PluginDescription> AvailablePlugins() {
      List<PluginDescription> availablePlugins = new List<PluginDescription>();

      Stream xmlStream = client.OpenRead(url + "/plugins.xml");

      XmlDocument pluginList = new XmlDocument();
      pluginList.Load(xmlStream);

      XmlNode list = pluginList.SelectSingleNode("/Plugins");
      foreach(XmlNode child in list.ChildNodes) { 
       
        string name = child.Attributes["Name"].Value;
        string version = child.Attributes["Version"].Value;

        PluginDescription description = new PluginDescription(name, new Version(version), this);
        availablePlugins.Add(description);

        // retrieve the list of dependencies
        XmlNodeList dependencies = child.SelectNodes("Dependency");
        foreach(XmlNode dependencyNode in dependencies) {
          string dependencyName = dependencyNode.Attributes["Name"].Value;
          description.Dependencies.Add(dependencyName);
        }

      }

      return availablePlugins;
    }

    internal long DownloadPlugin(PluginDescription description) {
      string fileName = description.Name + "-" + description.Version + ".zip";
      client.DownloadFile(url + "/" + fileName, cacheDir + "/" + fileName);

      // return size of downloaded file
      FileInfo info = new FileInfo(cacheDir + "/" + fileName);
      return info.Length;
    }

    public void CancelAsyncDownload() {
      client.CancelAsync();
    }

    public override string ToString() {
      return url;
    }

  }
}
