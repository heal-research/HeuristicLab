#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Linq;
using System.Text;
using HeuristicLab.PluginInfrastructure.Manager;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;

namespace HeuristicLab.PluginInfrastructure.Advanced {
  internal class InstallationManager {

    internal event EventHandler<PluginInfrastructureCancelEventArgs> PreUpdatePlugin;
    internal event EventHandler<PluginInfrastructureCancelEventArgs> PreRemovePlugin;
    internal event EventHandler<PluginInfrastructureCancelEventArgs> PreInstallPlugin;

    internal event EventHandler<PluginInfrastructureEventArgs> PluginUpdated;
    internal event EventHandler<PluginInfrastructureEventArgs> PluginRemoved;
    internal event EventHandler<PluginInfrastructureEventArgs> PluginInstalled;

    private string pluginDir;
    public InstallationManager(string pluginDir) {
      this.pluginDir = pluginDir;
    }

    //public IEnumerable<string> Show(IEnumerable<string> pluginNames) {
    //  foreach (PluginDescription desc in GetPluginDescriptions(pluginNames)) {
    //    yield return GetInformation(desc);
    //  }
    //}

    //internal string GetInformation(string pluginName) {
    //  return GetInformation(GetPluginDescription(pluginName));
    //}

    //private string GetInformation(PluginDescription desc) {
    //  StringBuilder builder = new StringBuilder();
    //  builder.Append("Name: ").AppendLine(desc.Name);
    //  builder.Append("Version: ").AppendLine(desc.Version.ToString());
    //  builder.AppendLine("Description:").AppendLine(desc.Description);
    //  if (!string.IsNullOrEmpty(desc.ContactName)) {
    //    builder.Append("Contact: ").Append(desc.ContactName).Append(", ").AppendLine(desc.ContactEmail);
    //  }
    //  builder.AppendLine("This plugin is " + desc.PluginState.ToString().ToLowerInvariant() + ".");
    //  builder.AppendLine("Files: ");
    //  foreach (var file in desc.Files) {
    //    builder.AppendLine(file.Type + " " + file.Name);
    //  }
    //  builder.AppendLine().AppendLine("Directly depends on:");
    //  if (desc.Dependencies.Count() == 0) builder.AppendLine("None");
    //  foreach (var dependency in desc.Dependencies) {
    //    builder.AppendLine(dependency.Name + " " + dependency.Version);
    //  }
    //  builder.AppendLine().AppendFormat("Plugins directly dependent on {0}:", desc.Name).AppendLine();
    //  var dependents = from x in pluginManager.Plugins
    //                   where x.Dependencies.Contains(desc)
    //                   select x;
    //  if (dependents.Count() == 0) builder.AppendLine("None");
    //  foreach (var dependent in dependents) {
    //    builder.AppendLine(dependent.Name + " " + dependent.Version);
    //  }
    //  builder.AppendLine();
    //  if (desc.PluginState == PluginState.Disabled) {
    //    builder.AppendLine(DetermineProblem(desc));
    //  }

    //  return builder.ToString();
    //}

    //private static string DetermineProblem(PluginDescription desc) {
    //  // either any file is missing
    //  StringBuilder builder = new StringBuilder();
    //  builder.AppendLine("Problem report:");
    //  builder.AppendLine(desc.LoadingErrorInformation);
    //  return builder.ToString();
    //}

    //private PluginDescription GetPluginDescription(string pluginName) {
    //  var exactMatch = from pluginDesc in pluginManager.Plugins
    //                   where string.Equals(pluginName, pluginDesc.Name, StringComparison.InvariantCultureIgnoreCase)
    //                   select pluginDesc;
    //  var inexactMatch = from pluginDesc in pluginManager.Plugins
    //                     where MatchPluginNameInexact(pluginName, pluginDesc.Name)
    //                     select pluginDesc;
    //  return exactMatch.Count() > 0 ? exactMatch.Single() : inexactMatch.First();
    //}

    //private IEnumerable<PluginDescription> GetPluginDescriptions(IEnumerable<string> pluginNames) {
    //  return from pluginName in pluginNames
    //         select GetPluginDescription(pluginName);
    //}

    //private static bool MatchPluginNameInexact(string similarName, string actualName) {
    //  return
    //    // Core-3.2 == HeuristicLab.Core-3.2
    //    actualName.Equals("HeuristicLab." + similarName, StringComparison.InvariantCultureIgnoreCase) ||
    //    // HeuristicLab.Core == HeuristicLab.Core-3.2 (this should be save because we checked for exact matches first)
    //    (Math.Abs(actualName.Length - similarName.Length) <= 4 && actualName.StartsWith(similarName, StringComparison.InvariantCultureIgnoreCase)) ||
    //    // Core == HeuristicLab.Core-3.2
    //    (Math.Abs(actualName.Length - similarName.Length) <= 17 && actualName.StartsWith("HeuristicLab." + similarName, StringComparison.InvariantCultureIgnoreCase));
    //}


    /// <summary>
    /// Retrieves a list of plugins available at the remote server
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public IEnumerable<IPluginDescription> GetRemotePluginList(string connectionString) {
      using (var client = new DeploymentService.UpdateClient()) {
        return client.GetPlugins();
      }
    }

    /// <summary>
    /// Retrieves the list of products available at the remote server
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public IEnumerable<DeploymentService.ProductDescription> GetRemoteProductList(string connectionString) {
      using (var client = new DeploymentService.UpdateClient()) {
        return client.GetProducts();
      }
    }

    /// <summary>
    ///  Installs plugins from remote server
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="pluginNames"></param>
    public void Install(string connectionString, IEnumerable<IPluginDescription> plugins) {
      using (var client = new DeploymentService.UpdateClient()) {
        var args = new PluginInfrastructureCancelEventArgs(plugins.Select(x => x.Name + " " + x.Version));
        OnPreInstall(args);
        foreach (DeploymentService.PluginDescription plugin in plugins) {
          byte[] zippedPackage = client.GetPlugin(plugin);
          Unpack(zippedPackage);
          OnInstalled(new PluginInfrastructureEventArgs(plugin));
        }
      }
    }

    /// <summary>
    /// Updates plugins from remote server
    /// </summary>
    /// <param name="pluginNames"></param>
    public void Update(string connectionString, IEnumerable<IPluginDescription> plugins) {
      PluginInfrastructureCancelEventArgs args = new PluginInfrastructureCancelEventArgs(plugins.Select(x => x.Name + " " + x.Version));
      OnPreUpdate(args);
      if (!args.Cancel) {
        using (var client = new DeploymentService.UpdateClient()) {
          foreach (DeploymentService.PluginDescription plugin in plugins) {
            byte[] zippedPackage = client.GetPlugin(plugin);
            Unpack(zippedPackage);
            OnUpdated(new PluginInfrastructureEventArgs(plugin));
          }
        }
      }
    }

    /// <summary>
    /// Deletes all plugin files from local installation
    /// </summary>
    /// <param name="pluginNames"></param>
    public void Remove(IEnumerable<IPluginDescription> plugins) {
      var fileNames = from pluginToDelete in plugins
                      from file in pluginToDelete.Files
                      select Path.Combine(pluginDir, file.Name);
      var args = new PluginInfrastructureCancelEventArgs(fileNames);
      OnPreDelete(args);
      if (!args.Cancel) {
        foreach (string fileName in fileNames) {
          File.Delete(fileName);
          OnDeleted(new PluginInfrastructureEventArgs(fileName));
        }
      }
    }

    private void Unpack(byte[] zippedPackage) {
      using (ZipInputStream s = new ZipInputStream(new MemoryStream(zippedPackage))) {
        ZipEntry theEntry;
        string tmpEntry = String.Empty;
        while ((theEntry = s.GetNextEntry()) != null) {
          string directoryName = pluginDir;
          string fileName = Path.GetFileName(theEntry.Name);
          // create directory 
          if (directoryName != "") {
            Directory.CreateDirectory(directoryName);
          }
          if (fileName != String.Empty) {
            string fullPath = Path.Combine(directoryName, fileName);
            string fullDirPath = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
            FileStream streamWriter = File.Create(fullPath);
            int size = 2048;
            byte[] data = new byte[2048];
            while (true) {
              size = s.Read(data, 0, data.Length);
              if (size > 0) {
                streamWriter.Write(data, 0, size);
              } else {
                break;
              }
            }
            streamWriter.Close();
          }
        }
      }
    }

    private void OnPreUpdate(PluginInfrastructureCancelEventArgs args) {
      if (PreUpdatePlugin != null) PreUpdatePlugin(this, args);
    }

    private void OnUpdated(PluginInfrastructureEventArgs args) {
      if (PluginUpdated != null) PluginUpdated(this, args);
    }

    private void OnPreDelete(PluginInfrastructureCancelEventArgs args) {
      if (PreRemovePlugin != null) PreRemovePlugin(this, args);
    }

    private void OnDeleted(PluginInfrastructureEventArgs args) {
      if (PluginRemoved != null) PluginRemoved(this, args);
    }

    private void OnPreInstall(PluginInfrastructureCancelEventArgs args) {
      if (PreInstallPlugin != null) PreInstallPlugin(this, args);
    }

    private void OnInstalled(PluginInfrastructureEventArgs args) {
      if (PluginInstalled != null) PluginInstalled(this, args);
    }
  }
}
