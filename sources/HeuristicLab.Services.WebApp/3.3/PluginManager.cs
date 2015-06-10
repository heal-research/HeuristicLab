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

    private string PluginsDirectory {
      get { return string.Format(@"{0}WebApp\plugins", HttpRuntime.AppDomainAppPath); }
    }

    public PluginManager() {
      plugins = new ConcurrentDictionary<string, Plugin>();
    }

    public Plugin GetPlugin(string name) {
      Plugin plugin;
      plugins.TryGetValue(name, out plugin);
      if (plugin == null) {
        string directory = string.Format(@"{0}\{1}", PluginsDirectory, name);
        if (Directory.Exists(directory)) {
          plugin = new Plugin {
            Name = name,
            Directory = directory
          };
          plugin.Configure(Configuration);
          plugins.Add(name, plugin);
        }
      }
      return plugin;
    }

    public IEnumerable<Plugin> GetPlugins() {
      DiscoverPlugins();
      return plugins.Values;
    }

    private void DiscoverPlugins() {
      var pluginDirectories = Directory.GetDirectories(PluginsDirectory);
      foreach (var directory in pluginDirectories) {
        string pluginName = Path.GetFileName(directory);
        Plugin plugin;
        plugins.TryGetValue(pluginName, out plugin);
        if (plugin == null) {
          plugin = new Plugin {
            Name = pluginName,
            Directory = directory
          };
          plugin.Configure(Configuration);
          plugins.Add(pluginName, plugin);
        }
      }
    }
  }
}