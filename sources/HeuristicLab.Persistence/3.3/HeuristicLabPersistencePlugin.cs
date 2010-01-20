using HeuristicLab.PluginInfrastructure;

namespace HeuristicLab.Persistence {

  [Plugin("HeuristicLab.Persistence-3.3")]
  [PluginFile("HeuristicLab.Persistence-3.3.dll", PluginFileType.Assembly)]
  [PluginDependency("HeuristicLab.Tracing-3.2")]
  public class HeuristicLabPersistencePlugin : PluginBase { }

}