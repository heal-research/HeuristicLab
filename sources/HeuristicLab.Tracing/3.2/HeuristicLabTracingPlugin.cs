using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Tracing {

  [Plugin("HeuristicLab.Tracing-3.2")]
  [PluginFile("HeuristicLab.Tracing-3.2.dll", PluginFileType.Assembly)]
  [PluginFile("log4net.dll", PluginFileType.Assembly)]
  [PluginFile("log4net licence.txt", PluginFileType.License)]
  [PluginFile("HeuristicLab.log4net.xml", PluginFileType.Data)]
  public class HeuristicLabPersistencePlugin : PluginBase {}

}