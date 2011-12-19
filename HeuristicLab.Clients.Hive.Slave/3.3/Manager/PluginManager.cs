#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using HeuristicLab.Clients.Hive.SlaveCore.Properties;
using HeuristicLab.Core;

namespace HeuristicLab.Clients.Hive.SlaveCore {
  public class PluginManager {
    private static object locker = new object();
    private string lastUsedFileName = Settings.Default.LastUsedFileName;
    //maximum number of days after which a plugin gets deleted if not used
    private int pluginLifetime = Settings.Default.PluginLifetime;

    private string PluginCacheDir { get; set; }
    public string PluginTempBaseDir { get; set; }
    private ILog log;
    private IPluginProvider pluginService;
    private List<Guid> cachedPluginsGuids = new List<Guid>();

    public PluginManager(IPluginProvider pluginService, ILog log) {
      this.pluginService = pluginService;
      this.log = log;
      PluginCacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PluginCacheDir);
      PluginTempBaseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.Default.PluginTempBaseDir);
      DoUpdateRun();
    }

    /// <summary>
    /// Returns the last directory of a path
    /// </summary>    
    private string GetFilenameFromPath(string path) {
      string[] dirParts = path.Split(Path.DirectorySeparatorChar);
      if (dirParts.Length > 0) {
        string fileGuid = dirParts[dirParts.Length - 1];
        return fileGuid;
      } else
        return "";
    }

    private void DoUpdateRun() {
      SafelyCreateDirectory(PluginCacheDir);
      lock (cachedPluginsGuids) {
        cachedPluginsGuids.Clear();
        foreach (string dir in Directory.EnumerateDirectories(PluginCacheDir)) {
          cachedPluginsGuids.Add(Guid.Parse(GetFilenameFromPath(dir)));
        }
      }
    }

    public void CopyPluginsForJob(List<Plugin> requests, Guid jobId, out string configFileName) {
      configFileName = string.Empty;
      String targetDir = Path.Combine(PluginTempBaseDir, jobId.ToString());

      RecreateDirectory(targetDir);

      foreach (Plugin requestedPlugin in requests) {
        var filePaths = GetPluginFilePaths(requestedPlugin.Id);
        foreach (string filePath in filePaths) {
          File.Copy(filePath, Path.Combine(targetDir, Path.GetFileName(filePath)));
        }

        if (requestedPlugin.Name == Settings.Default.ConfigurationName) {
          // configuration plugin consists only of 1 file (usually the "HeuristicLab X.X.exe.config")
          configFileName = Path.Combine(targetDir, Path.GetFileName(filePaths.SingleOrDefault()));
        }
      }

      // copy files from PluginInfrastructure (which are not declared in any plugins)
      string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      CopyFile(baseDir, targetDir, Settings.Default.PluginInfrastructureDll);
      CopyFile(baseDir, targetDir, Settings.Default.SharpZipLibDll);
      CopyFile(baseDir, targetDir, Settings.Default.SharpZipLibLicense);

      // copy slave plugins, otherwise its not possible to register the UnhandledException handler to the appdomain        
      CopyFile(baseDir, targetDir, Settings.Default.ClientsHiveSlaveCoreDll);
      CopyFile(baseDir, targetDir, Settings.Default.ClientsHiveDll);
      CopyFile(baseDir, targetDir, Settings.Default.HiveDll);
      CopyFile(baseDir, targetDir, Settings.Default.ClientsCommonDll);
    }

    private static DirectoryInfo RecreateDirectory(String targetDir) {
      var di = new DirectoryInfo(targetDir);
      if (di.Exists) Directory.Delete(targetDir, true);
      di.Refresh();
      while (di.Exists) {
        Thread.Sleep(Settings.Default.DirOpSleepTime);
        di.Refresh();
      }
      return SafelyCreateDirectory(targetDir);
    }

    private static DirectoryInfo SafelyCreateDirectory(String targetDir) {
      var di = new DirectoryInfo(targetDir);
      if (!di.Exists) {
        di = Directory.CreateDirectory(targetDir);
        while (!di.Exists) {
          Thread.Sleep(Settings.Default.DirOpSleepTime);
          di.Refresh();
        }
      }
      return di;
    }

    private void CopyFile(string baseDir, string targetDir, string fileName) {
      if (!File.Exists(Path.Combine(targetDir, fileName))) File.Copy(Path.Combine(baseDir, fileName), Path.Combine(targetDir, fileName));
    }

    /// <summary>
    /// Updates the plugin cache with missing plugins and 
    /// then copies the required plugins for the task.
    /// </summary>        
    public void PreparePlugins(Task task, out string configFileName) {
      lock (locker) {
        log.LogMessage("Fetching plugins for task " + task.Id);

        List<Guid> missingGuids = new List<Guid>();
        List<Plugin> neededPlugins = new List<Plugin>();
        lock (cachedPluginsGuids) {
          foreach (Guid pluginId in task.PluginsNeededIds) {
            Plugin plugin = pluginService.GetPlugin(pluginId);
            if (plugin != null) {
              neededPlugins.Add(plugin);
            }

            if (!cachedPluginsGuids.Contains(pluginId)) {
              missingGuids.Add(pluginId);
            }
          }
        }

        IEnumerable<PluginData> pluginDatas = pluginService.GetPluginDatas(missingGuids);

        if (pluginDatas != null) {
          foreach (PluginData pluginData in pluginDatas) {
            string pluginDir = Path.Combine(PluginCacheDir, pluginData.PluginId.ToString());

            //put all files belonging to a plugin in the same directory
            SafelyCreateDirectory(pluginDir);
            File.WriteAllBytes(Path.Combine(pluginDir, Path.GetFileName(pluginData.FileName)), pluginData.Data);
          }

          if (missingGuids.Count > 0) {
            DoUpdateRun();
          }
          CopyPluginsForJob(neededPlugins, task.Id, out configFileName);
        } else {
          configFileName = "";
        }
        log.LogMessage(string.Format("Fetched {0} plugins for task {1}", missingGuids.Count, task.Id));
      }
    }

    /// <summary>
    /// Returns a list of files which belong to a plugin from the plugincache
    /// </summary>
    private IEnumerable<string> GetPluginFilePaths(Guid pluginId) {
      string pluginPath = Path.Combine(PluginCacheDir, pluginId.ToString());

      if (Directory.Exists(pluginPath)) {
        WriteDateLastUsed(pluginPath);
        foreach (string filePath in Directory.GetFiles(pluginPath)) {
          string fn = Path.GetFileName(filePath);
          if (fn != lastUsedFileName)
            yield return filePath;
        }
      }
    }

    /// <summary>
    /// creates a file in path with the current date;
    /// this can later be used to find plugins which are outdated
    /// </summary>    
    private void WriteDateLastUsed(string path) {
      FileStream fs = null;
      try {
        fs = new FileStream(Path.Combine(path, lastUsedFileName), FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(fs, DateTime.Now);
      }
      catch (IOException) {
        log.LogMessage(string.Format("No used date written in path {0}.", path));
      }
      catch (SerializationException) {
        //rethrow...
        throw;
      }
      finally {
        if (fs != null) {
          fs.Close();
        }
      }
    }

    /// <summary>
    /// Checks the PluginTemp directory for orphaned directories and deletes them.
    /// This should be only called if no jobs are currently running.
    /// </summary>    
    public void CleanPluginTemp() {
      if (Directory.Exists(PluginTempBaseDir)) {
        foreach (string dir in Directory.EnumerateDirectories(PluginTempBaseDir)) {
          try {
            log.LogMessage("Deleting orphaned directory " + dir);
            Directory.Delete(dir, true);
          }
          catch (Exception ex) {
            log.LogMessage("Error cleaning up PluginTemp directory " + dir + ": " + ex.ToString());
          }
        }
      }
    }

    /// <summary>
    /// checks the pluginCacheDirectory and deletes plugin folders which are not used anymore
    /// </summary>    
    private void CleanPluginCache() {
      FileStream fs = null;
      DateTime luDate;
      bool changed = false;

      if (Directory.Exists(PluginCacheDir)) {
        lock (locker) {
          foreach (string dir in Directory.EnumerateDirectories(PluginCacheDir)) {
            try {
              fs = new FileStream(Path.Combine(dir, lastUsedFileName), FileMode.Open);
              BinaryFormatter formatter = new BinaryFormatter();
              luDate = (DateTime)formatter.Deserialize(fs);
              fs.Close();

              if (luDate.AddDays(pluginLifetime) < DateTime.Now) {
                Directory.Delete(dir, true);
                changed = true;
              }
            }
            catch (FileNotFoundException) {
              //nerver used
              Directory.Delete(dir, true);
              changed = true;
            }
            catch (Exception ex) {
              if (fs != null) {
                fs.Close();
              }
              log.LogMessage(string.Format("CleanPluginCache threw exception: {0}", ex.ToString()));
            }
          }

          if (changed)
            DoUpdateRun();
        }
      }
    }

    public void DeletePluginsForJob(Guid id) {
      try {
        log.LogMessage("Deleting plugins...");
        int tries = Settings.Default.PluginDeletionRetries;
        string path = Path.Combine(PluginTempBaseDir, id.ToString());
        while (tries > 0) {
          try {
            if (Directory.Exists(path)) Directory.Delete(path, true);
            tries = 0;
          }
          catch (Exception) {
            Thread.Sleep(Settings.Default.PluginDeletionTimeout);
            tries--;
            if (tries == 0) throw;
          }
        }
      }
      catch (Exception ex) {
        log.LogMessage("failed while unloading " + id + " with exception " + ex);
      }
      CleanPluginCache();
    }
  }
}