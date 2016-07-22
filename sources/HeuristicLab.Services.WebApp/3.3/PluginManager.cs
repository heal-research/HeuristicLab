#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2016 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;

namespace HeuristicLab.Services.WebApp {
  public class PluginManager {

    private static PluginManager instance;
    public static PluginManager Instance {
      get { return instance ?? (instance = new PluginManager()); }
    }

    private readonly IDictionary<string, Plugin> plugins;

    public HttpConfiguration Configuration { get; set; }

    public IEnumerable<Plugin> Plugins {
      get { return plugins.Values; }
    }

    public static string PluginsDirectory {
      get {
        return Path.Combine(HttpRuntime.AppDomainAppPath, "WebApp", "plugins");
      }
    }

    public PluginManager() {
      plugins = new ConcurrentDictionary<string, Plugin>();
      var fileWatcher = new FileSystemWatcher(PluginsDirectory, "*") {
        IncludeSubdirectories = true,
        EnableRaisingEvents = true
      };
      fileWatcher.Created += OnFilesChanged;
      fileWatcher.Changed += OnFilesChanged;
      fileWatcher.Deleted += OnFilesChanged;
      fileWatcher.Renamed += OnFilesChanged;
    }

    private void OnFilesChanged(object sender, FileSystemEventArgs args) {
      string path = args.FullPath.Remove(0, PluginsDirectory.Length + 1);
      var pathParts = path.Split(Path.DirectorySeparatorChar);
      string pluginName = pathParts[0];
      if (pathParts.Length <= 2) {
        switch (args.ChangeType) {
          case WatcherChangeTypes.Created:
            GetPlugin(pluginName);
            break;

          case WatcherChangeTypes.Deleted:
            plugins.Remove(pluginName);
            break;

          case WatcherChangeTypes.Renamed:
            RenamedEventArgs renamedArgs = (RenamedEventArgs)args;
            string oldPath = renamedArgs.OldFullPath.Remove(0, PluginsDirectory.Length + 1);
            var oldPathParts = oldPath.Split(Path.DirectorySeparatorChar);
            string oldPluginName = oldPathParts[0];
            plugins.Remove(oldPluginName);
            GetPlugin(pluginName);
            break;

          case WatcherChangeTypes.Changed:
            Plugin plugin = LookupPlugin(pluginName);
            if (plugin != null) {
              plugin.ReloadControllers();
            }
            break;
        }
      }
    }

    public Plugin GetPlugin(string name) {
      Plugin plugin = LookupPlugin(name);
      if (plugin == null) {
        string directory = Path.Combine(PluginsDirectory, name);
        if (Directory.Exists(directory)) {
          plugin = new Plugin(name, directory, Configuration);
          plugins.Add(name, plugin);
        }
      }
      return plugin;
    }

    public IEnumerable<Plugin> GetPlugins() {
      return plugins.Values;
    }

    public void DiscoverPlugins() {
      var pluginDirectories = Directory.GetDirectories(PluginsDirectory);
      foreach (var directory in pluginDirectories) {
        string pluginName = Path.GetFileName(directory);
        Plugin plugin = LookupPlugin(pluginName);
        if (plugin == null) {
          plugin = new Plugin(pluginName, directory, Configuration);
          plugins.Add(pluginName, plugin);
        }
      }
    }

    private Plugin LookupPlugin(string name) {
      Plugin plugin;
      plugins.TryGetValue(name, out plugin);
      return plugin;
    }
  }
}