using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Tracing {

  [ClassInfo(Name = "HeuristicLab.Tracing-3.2")]
  [PluginFile(Filename = "HeuristicLab.Tracing-3.2.dll", Filetype = PluginFileType.Assembly)]
  [PluginFile(Filename = "log4net.dll", Filetype = PluginFileType.Assembly)]
  [PluginFile(Filename = "log4net licence.txt", Filetype = PluginFileType.License)]
  [PluginFile(Filename = "HeuristicLab.log4net.xml", Filetype = PluginFileType.Data)]
  public class HeuristicLabPersistencePlugin : PluginBase {}

}