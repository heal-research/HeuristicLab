
using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.igraph {
  [Plugin("HeuristicLab.igraph", "Provides functionality of igraph in HeuristicLab", "0.8.0.0")]
  [PluginFile("HeuristicLab.igraph-0.8.0-pre.dll", PluginFileType.Assembly)]
  [PluginFile("igraph-0.8.0-pre-x86.dll", PluginFileType.NativeDll)]
  [PluginFile("igraph-0.8.0-pre-x64.dll", PluginFileType.NativeDll)]
  [PluginFile("igraph-0.8.0-pre-license.txt", PluginFileType.License)]
  public class HeuristicLabigraphPlugin : PluginBase {
  }
}
