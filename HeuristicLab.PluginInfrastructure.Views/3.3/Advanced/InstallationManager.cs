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
using System.IO.Compression;
using System.Linq;
using System.ServiceModel;
using HeuristicLab.PluginInfrastructure.Manager;

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

    /// <summary>
    /// Retrieves a list of plugins available at the remote server
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IPluginDescription> GetRemotePluginList() {
      var client = DeploymentService.UpdateServiceClientFactory.CreateClient();
      try {
        List<IPluginDescription> plugins = new List<IPluginDescription>(client.GetPlugins());
        client.Close();
        return plugins;
      }
      catch (TimeoutException e) {
        client.Abort();
        throw new InstallationManagerException("Time out while connecting to server.", e);
      }
      catch (FaultException e) {
        client.Abort();
        throw new InstallationManagerException("Fault in connection to server.", e);
      }
      catch (CommunicationException e) {
        client.Abort();
        throw new InstallationManagerException("General communication exception in connection to server.", e);
      }
    }

    /// <summary>
    /// Retrieves the list of products available at the remote server
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DeploymentService.ProductDescription> GetRemoteProductList() {
      var client = DeploymentService.UpdateServiceClientFactory.CreateClient();
      try {
        List<DeploymentService.ProductDescription> products = new List<DeploymentService.ProductDescription>(client.GetProducts());
        client.Close();
        return products;
      }
      catch (TimeoutException e) {
        client.Abort();
        throw new InstallationManagerException("Time out while connecting to server.", e);
      }
      catch (FaultException e) {
        client.Abort();
        throw new InstallationManagerException("Fault in connection to server.", e);
      }
      catch (CommunicationException e) {
        client.Abort();
        throw new InstallationManagerException("General communication exception in connection to server.", e);
      }
    }

    /// <summary>
    ///  Installs plugins from remote server
    /// </summary>
    /// <param name="plugins"></param>
    public void Install(IEnumerable<IPluginDescription> plugins, out bool cancelled) {
      var args = new PluginInfrastructureCancelEventArgs(plugins);
      OnPreInstall(args);
      if (!args.Cancel) {
        cancelled = false;
        var client = DeploymentService.UpdateServiceClientFactory.CreateClient();
        try {
          foreach (DeploymentService.PluginDescription plugin in plugins) {
            byte[] zippedPackage = client.GetPlugin(plugin);
            Unpack(zippedPackage);
            OnInstalled(new PluginInfrastructureEventArgs(plugin));
          }
          client.Close();
        }
        catch (TimeoutException e) {
          client.Abort();
          throw new InstallationManagerException("Time out while connecting to server.", e);
        }
        catch (FaultException e) {
          client.Abort();
          throw new InstallationManagerException("Fault in connection to server.", e);
        }
        catch (CommunicationException e) {
          client.Abort();
          throw new InstallationManagerException("General communication exception in connection to server.", e);
        }
      } else {
        cancelled = true;
      }
    }

    /// <summary>
    /// Updates plugins from remote server
    /// </summary>
    /// <param name="plugins"></param>
    public void Update(IEnumerable<IPluginDescription> plugins, out bool cancelled) {
      PluginInfrastructureCancelEventArgs args = new PluginInfrastructureCancelEventArgs(plugins);
      OnPreUpdate(args);
      if (!args.Cancel) {
        cancelled = false;
        var client = DeploymentService.UpdateServiceClientFactory.CreateClient();
        try {
          foreach (DeploymentService.PluginDescription plugin in plugins) {
            byte[] zippedPackage = client.GetPlugin(plugin);
            Unpack(zippedPackage);
            OnUpdated(new PluginInfrastructureEventArgs(plugin));
          }
          client.Close();
        }
        catch (TimeoutException e) {
          client.Abort();
          throw new InstallationManagerException("Time out while connecting to server.", e);
        }
        catch (FaultException e) {
          client.Abort();
          throw new InstallationManagerException("Fault in connection to server.", e);
        }
        catch (CommunicationException e) {
          client.Abort();
          throw new InstallationManagerException("General communication exception in connection to server.", e);
        }
      } else {
        cancelled = true;
      }
    }

    /// <summary>
    /// Deletes all plugin files from local installation
    /// </summary>
    /// <param name="plugins"></param>
    public void Remove(IEnumerable<IPluginDescription> plugins) {
      var fileNames = from pluginToDelete in plugins
                      from file in pluginToDelete.Files
                      select Path.Combine(pluginDir, file.Name);
      var args = new PluginInfrastructureCancelEventArgs(plugins);
      OnPreDelete(args);
      if (!args.Cancel) {
        foreach (string fileName in fileNames) {
          File.Delete(fileName);
          OnDeleted(new PluginInfrastructureEventArgs(fileName));
        }
      }
    }

    private void Unpack(byte[] zippedPackage) {
      using (MemoryStream memStream = new MemoryStream(zippedPackage)) {
        using (ZipArchive zip = new ZipArchive(memStream, ZipArchiveMode.Read)) {
          foreach (var theEntry in zip.Entries) {
            string directoryName = pluginDir;
            string fileName = Path.GetFileName(theEntry.Name);
            // create directory 
            if (!string.IsNullOrEmpty(directoryName)) {
              Directory.CreateDirectory(directoryName);
            }
            if (!string.IsNullOrEmpty(fileName)) {
              string fullPath = Path.Combine(directoryName, fileName);
              string fullDirPath = Path.GetDirectoryName(fullPath);
              if (!Directory.Exists(fullDirPath)) Directory.CreateDirectory(fullDirPath);
              using (FileStream streamWriter = File.Create(fullPath)) {
                int size = 2048;
                byte[] data = new byte[2048];

                using (BinaryReader reader = new BinaryReader(theEntry.Open())) {
                  while (true) {
                    size = reader.Read(data, 0, data.Length);
                    if (size > 0) {
                      streamWriter.Write(data, 0, size);
                    } else {
                      break;
                    }
                  }
                }
                streamWriter.Close();
              }
            }
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
